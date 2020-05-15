using DataReef.Core.Extensions;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.Integrations;
using DataReef.Integrations.Geo.DataViews;
using DataReef.Integrations.Pictometry;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models.Accounting;
using DataReef.TM.Models.DTOs.Blobs;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Data.Entity;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using Newtonsoft.Json.Linq;
using DataReef.TM.Models.FinancialIntegration;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class ImageryService : IImageryService
    {
        private readonly string _s3BucketName = ConfigurationManager.AppSettings["AWS_S3_ImageryBucketName"];
        private readonly ILogger _logger;
        private readonly ITokensProvider _tokensProvider;
        private readonly IBlobService _blobService;
        private readonly IEagleViewService _eagleViewService;
        private readonly IGeographyBridge _geoBridge;
        private readonly Func<IOUSettingService> _ouSettingServiceFactory;
        private const double LatOffset = 0.00000585d;
        private const double LongOffset = 0.00000925d;

        public ImageryService(
            ILogger logger,
            ITokensProvider tokensProvider,
            IBlobService blobService,
            IEagleViewService eagleViewService,
            IGeographyBridge geoBridge,
            Func<IOUSettingService> ouSettingServiceFactory)
        {
            _logger = logger;
            _tokensProvider = tokensProvider;
            _blobService = blobService;
            _eagleViewService = eagleViewService;
            _geoBridge = geoBridge;
            _ouSettingServiceFactory = ouSettingServiceFactory;
        }

        public List<string> AvailableOrientationsAtLocation(double lat, double lon)
        {
            var results = _eagleViewService.GetLinksForLatLon(lat, lon);
            var ret = results.Select(r => r.OrientationString).ToList();
            return ret;
        }

        protected void SaveRequest(string request, string response, string url, string headers, string apikey)
        {
            var adapterRequest = new AdapterRequest
            {
                AdapterName = "test",
                Request = request,
                Response = response,
                Url = url,
                Headers = headers,
                TagString = apikey
            };

            using (var context = new DataContext())
            {
                context.AdapterRequests.Add(adapterRequest);
                context.SaveChanges();
            }
        }


        public BlobModel PurchaseHighResImageAtCoordinates(Guid propertyID, double top, double left, double bottom, double right, string direction)
        {
            double lon, lat;

            int tokenCost = 1;
            Guid userID = SmartPrincipal.UserId;
            TokenLedger ledger = null;

            var freeHiResImages = false;
            if (propertyID == new Guid("55C40F25-F7B9-43E9-87AF-460C8C2C804D")) //special guid that bypasses the propertyid lookup and tokens.  used for non TM functions
            {
                lon = (left + right) / 2.0;
                lat = (top + bottom) / 2.0;
                tokenCost = 0;
            }
            else
            {
                using (var context = new DataContext())
                {
                    var property = context.Properties
                        .Include(p => p.Territory)
                        .FirstOrDefault(p => p.Guid == propertyID);
                    if (property == null)
                        throw new Exception("Property was not found");

                    lon = property.Longitude ?? (left + right) / 2;
                    lat = property.Latitude ?? (top + bottom) / 2;

                    var ouSettings = _ouSettingServiceFactory().GetSettings(property.Territory.OUID, null);
                    if (ouSettings != null && ouSettings.ContainsKey(OUSetting.LegionOUFreeHiResImages))
                    {
                        string tokenizeHiResSetting = ouSettings[OUSetting.LegionOUFreeHiResImages].Value;
                        freeHiResImages = tokenizeHiResSetting == "1";
                    }
                }

                #region Tokenology

                // 1 image = .25 cents = 1 Token
                // check to see if there are Tokens available


#if (DEBUG)
                userID = new Guid("7D49D3AF-C08C-4AA9-AC76-A6652A8EE884"); //make sure there is an account.  this is Vlads test account
#endif
                if (!freeHiResImages)
                {
                    ledger = _tokensProvider.GetDefaultLedgerForPerson(userID);
                    if (ledger == null) throw new ApplicationException("No Token Ledger Exists For This User");

                    double balance = _tokensProvider.GetBalanceForLedger(ledger.Guid);
                    if (balance < tokenCost) throw new ApplicationException("Insufficient Tokens");
                }

                #endregion

            }


            BlobModel hiResImageContent = null;
            HighResImage hiResImg = null;

            var searchResults = _eagleViewService.GetLinksForLatLon(lat, lon);

            //try
            //{
            //    SaveRequest("GetLinksForLatLon", "test","test","test","test");
            //}
            //catch (Exception ex)
            //{
            //    throw new ApplicationException(ex.Message);
            //}
            SearchResult result = searchResults.FirstOrDefault(sr => sr.OrientationString == direction);

            if (result != null)
            {
                // first check to see if the lat/lon exists in an existing cached image
                using (var dc = new DataContext())
                {
                    bool imageWasCached = false;
                    hiResImg = _geoBridge.GetHiResImageByLatLon(lat, lon);

                    if (hiResImg == null)
                    {
                        // now get the binary
                        var bytes = _eagleViewService.GetImageBytesForSearchResult(result, result.Width, result.Height);                       

                        Image image = null;

                        if (bytes != null)
                        {
                            using (var ms = new MemoryStream(bytes))
                            {
                                image = new Bitmap(ms);
                            }
                        }

                        _eagleViewService.PopulateMetaDataForSearchResult(result);

                        // create a new imageMetaModel, the guid will also be the name of the key for AWS-S3
                        hiResImg = new HighResImage
                        {
                            Top = result.Top,
                            Left = result.Left,
                            Bottom = result.Bottom,
                            Right = result.Right,
                            Width = image.Width,
                            Height = image.Height,
                            MapUnitsPerPixelX = result.MapUnitsPerPixelX,
                            MapUnitsPerPixelY = result.MapUnitsPerPixelY,
                            SkewX = result.SkewX,
                            SkewY = result.SkewY,
                            Resolution = result.Resolution,
                            Source = "Pictometry",
                            CreatedBy = SmartPrincipal.UserId
                        };

                        try
                        {
                            SaveRequest("hiResImg", hiResImg.Top.ToString(), image.Width.ToString(), "test", "test");
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(ex.Message);
                        }

                       
                        try
                        {
                            var s = _geoBridge.SaveHiResImagetest(hiResImg);
                            SaveRequest("SaveHiResImagetest", s, image.Width.ToString(), "test", "test");
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(ex.Message);
                        }

                        _geoBridge.SaveHiResImage(hiResImg);

                      

                        if (bytes != null)
                        {
                            hiResImageContent = new BlobModel { Content = bytes, ContentType = "image/jpeg" };
                            _blobService.UploadByName(hiResImg.Guid.ToString(), hiResImageContent, BlobAccessRights.Private, _s3BucketName);
                        }

                        dc.SaveChanges();

                    }
                    else // metadata exists
                    {
                        hiResImageContent = _blobService.DownloadByName(hiResImg.Guid.ToString(), _s3BucketName);
                        imageWasCached = true;
                        if (hiResImageContent == null) // binary doesn't actually exists on amazon even though we have meta data. We could have deleted the s3 files or something
                        {
                            // so let's go get the binary again
                            var bytes = _eagleViewService.GetImageBytesForSearchResult(result, result.Width, result.Height);

                            if (bytes != null)
                            {
                                hiResImageContent = new BlobModel
                                {
                                    Content = bytes,
                                    ContentType = "image/jpeg"
                                };
                                _blobService.UploadByName(hiResImg.Guid.ToString(), hiResImageContent, BlobAccessRights.Private, _s3BucketName);
                            }
                        }
                    }

                    var croppedImage = new CroppedImage();
                    if (hiResImageContent != null)
                    {
                        croppedImage = CropImage(hiResImg, hiResImageContent, top, left, bottom, right);
                        hiResImageContent.Content = croppedImage.Content;
                    }

                    //and lets record the read
                    var purchase = new ImagePurchase
                    {
                        //ImageID = hiResImage.Guid,
                        ImageID = hiResImg.Guid,
                        OUID = SmartPrincipal.OuId,
                        PropertyID =
                            propertyID, // we record the propertyID for subsequent lookups once the image has been acquired (purchased) for that property
                        UserID = userID,
                        Lat = lat,
                        Lon = lon,
                        Tokens = freeHiResImages ? 0 : tokenCost,
                        ImageWasCached = imageWasCached,
                        ImageDate = DateTime.Parse(result.Date),
                        LocationX = result.SearchPoint.X,
                        LocationY = result.SearchPoint.Y,
                        ImageType = direction,
                        Left = left,
                        Right = right,
                        Top = top,
                        Bottom = bottom,
                    };
                    dc.ImagePurchases.Add(purchase);

                    if (!freeHiResImages && tokenCost > 0 && ledger != null)
                    {
                        //record the token expense
                        var te = new TokenExpense
                        {
                            Amount = tokenCost,
                            TenantID = SmartPrincipal.TenantId,
                            LedgerID = ledger.Guid,
                            CreatedByID = userID,
                            ExternalID = purchase.Guid.ToString(), //link the purchase to the expense
                            Notes = "High Res Image Purchase"
                        };

                        dc.TokenExpenses.Add(te);
                    }

                    dc.SaveChanges();

                } //data context
            } //if result

            return hiResImageContent;
        }

        public BlobModel GetExistingHiResImageForProperty(Guid propertyID, double top, double left, double bottom, double right, string direction)
        {
            HighResImage hiResImg = null;
            ImagePurchase purchase = null;

            using (var dc = new DataContext())
            {
                purchase = dc.ImagePurchases.Where(ip => ip.PropertyID == propertyID && ip.ImageType == direction && !ip.IsDeleted).OrderByDescending(ip => ip.DateCreated).FirstOrDefault();
                if (purchase == null) throw new ApplicationException("PurchaseNotFound");

                hiResImg = _geoBridge.GetHiResImageById(purchase.ImageID);
                if (hiResImg == null) throw new ApplicationException("HighResImageNotFound");
            }

            BlobModel hiResImageContent = _blobService.DownloadByName(hiResImg.Guid.ToString(), _s3BucketName);
            if (hiResImageContent == null) return null; // got deleted, go get it again

            var croppedImage = CropImage(hiResImg, hiResImageContent, top, left, bottom, right);

            hiResImageContent.Content = croppedImage.Content;
            return hiResImageContent;
        }

        public bool HighResImageExistsAtLocation(double lat, double lon)
        {
            var orientations = AvailableOrientationsAtLocation(lat, lon);
            return orientations != null && orientations.Contains("Down");
        }

        public void MigrateHiResImages()
        {
            using (var dc = new DataContext())
            {
                // get hi-res image entries from DB
                // and verify that the binary files exist on S3
                var images = dc
                                .HighResolutionImages
                                .Where(hires => !hires.IsDeleted)
                                .ToList()
                                .Where(img => _blobService.FileExists(img.Guid.ToString(), _s3BucketName))
                                .ToList();

                foreach (var image in images)
                {
                    _geoBridge.MigrateHiResImage(image.ToHighResImg());
                }
            }
        }

        private struct CroppedImage
        {
            public byte[] Content { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        private CroppedImage CropImage(HighResImage hiResImage, BlobModel hiResImageContent, double top, double left, double bottom, double right)
        {
            top = top - LatOffset;
            bottom = bottom - LatOffset;
            left = left + LongOffset;
            right = right + LongOffset;

            var croppedImage = new CroppedImage();

            if (hiResImage != null && hiResImageContent != null && hiResImageContent.Content != null)
            {
                using (var ms = new MemoryStream(hiResImageContent.Content))
                {
                    Bitmap img = new Bitmap(ms);

                    double dTop = hiResImage.Top - top;
                    double dLeft = left - hiResImage.Left;
                    double dRight = hiResImage.Right - right;
                    double dBottom = bottom - hiResImage.Bottom;

                    double topPixels = Math.Abs(dTop / hiResImage.MapUnitsPerPixelY);
                    double leftPixels = Math.Abs(dLeft / hiResImage.MapUnitsPerPixelX);
                    double pixelWidth = Math.Abs((right - left) / hiResImage.MapUnitsPerPixelX);
                    double pixelHeight = Math.Abs((bottom - top) / hiResImage.MapUnitsPerPixelY);

                    croppedImage.X = (int)leftPixels;
                    croppedImage.Y = (int)topPixels;
                    croppedImage.Width = (int)pixelWidth;
                    croppedImage.Height = (int)pixelHeight;

                    var cropRect = new Rectangle(croppedImage.X, croppedImage.Y, croppedImage.Width, croppedImage.Height);
                    var croppedImageContent = img.CropAtRect(cropRect);

                    using (var croppedMs = new MemoryStream())
                    {
                        croppedImageContent.Save(croppedMs, ImageFormat.Jpeg);
                        croppedImage.Content = croppedMs.ToArray();
                    }
                }
            }

            return croppedImage;
        }
    }
}
