using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Extensions
{
    public static class ImageExpensions
    {
        public static Bitmap CropAtRect(this Bitmap b, Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            Graphics g = Graphics.FromImage(nb);
            g.DrawImage(b, -r.X, -r.Y);
            return nb;
        }

        public static Image GetResizedImage(this Stream stream, int width, int height)
        {
            using (Image imgPhoto = Image.FromStream(stream))
            {
                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;
                int sourceX = 0;
                int sourceY = 0;
                int destX = 0;
                int destY = 0;

                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;

                nPercentW = ((float)width / (float)sourceWidth);
                nPercentH = ((float)height / (float)sourceHeight);
                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                    destX = Convert.ToInt16((width - (sourceWidth * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentW;
                    destY = Convert.ToInt16((height - (sourceHeight * nPercent)) / 2);
                }

                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);

                Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

                using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
                {
                    grPhoto.Clear(Color.Transparent);
                    grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    grPhoto.DrawImage(imgPhoto,
                        new Rectangle(destX, destY, destWidth, destHeight),
                        new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                        GraphicsUnit.Pixel);

                    return bmPhoto;
                }
            }
        }

        public static byte[] ImageToByte(this Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static byte[] GetResizedContent(this Stream stream, int width, int height)
        {
            return ImageToByte(GetResizedImage(stream, width, height));
        }

        public static byte[] GetResizedContent(this byte[] data, int width, int height)
        {
            return GetResizedContent(new MemoryStream(data), width, height);
        }

        public static byte[] GetResizedImageContent(this Image image, int width, int height)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);

                ms.Position = 0;
                return GetResizedContent(ms, width, height);
            }
        }

        public static string GetMimeType(this ImageFormat imageFormat)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs?.First(codec => codec.FormatID == imageFormat.Guid)?.MimeType ?? "image/jpeg";
        }
    }

}
