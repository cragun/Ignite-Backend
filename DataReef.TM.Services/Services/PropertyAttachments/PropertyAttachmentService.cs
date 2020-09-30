using DataReef.Core.Extensions;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.TM.Classes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews.Geo;
using DataReef.TM.Models.DTOs.Blobs;
using DataReef.TM.Models.DTOs.Integrations;
using DataReef.TM.Models.DTOs.PropertyAttachments;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Enums.PropertyAttachments;
using DataReef.TM.Models.PropertyAttachments;
using DataReef.TM.Services.Services.FinanceAdapters.SolarSalesTracker;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services.Services.PropertyAttachments
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PropertyAttachmentService : DataService<PropertyAttachment>, IPropertyAttachmentService
    {
        public readonly int ThumbnailLargeWidth;
        public readonly int ThumbnailMediumWidth;
        public readonly int ThumbnailSmallWidth;
        private readonly Lazy<IBlobService> _blobService;
        private readonly Lazy<IDataService<PropertyAttachmentItem>> _propertyAttachmentItemService;
        private readonly Lazy<IAuthenticationService> _authService;
        private readonly Lazy<IOUSettingService> _ouSettingsService;
        private readonly Lazy<IPersonService> _personService;
        public PropertyAttachmentService(ILogger logger,
            Func<IUnitOfWork> unitOfWorkFactory,
            Lazy<IBlobService> blobService,
            Lazy<IAuthenticationService> authService,
            Lazy<IOUSettingService> ouSettingsService,
            Lazy<IDataService<PropertyAttachmentItem>> propertyAttachmentItemService,
            Lazy<IPersonService> personService)
            : base(logger, unitOfWorkFactory)
        {
            _blobService = blobService;
            _propertyAttachmentItemService = propertyAttachmentItemService;
            _authService = authService;
            _ouSettingsService = ouSettingsService;
            _personService = personService;

            ThumbnailLargeWidth = ParseConfiguration("PropertyAttachmentLargeThumbnailWidth", 960);
            ThumbnailMediumWidth = ParseConfiguration("PropertyAttachmentMediumThumbnailWidth", 640);
            ThumbnailSmallWidth = ParseConfiguration("PropertyAttachmentSmallThumbnailWidth", 480);
        }

        public override PropertyAttachment Insert(PropertyAttachment entity)
        {
            var userName = _authService.Value.GetCurrentUserFullName();
            var audit = new AuditItem
            {
                UserName = userName,
                Action = "created"
            };
            entity.AddAudit(audit);
            var result = base.Insert(entity);

            return result;
        }

        public PropertyAttachmentItemDTO UploadImage(Guid propertyAttachmentID, Guid? propertyAttachmentItemID, string sectionID, string itemID, List<string> images, List<ImageBase64WithNotes> imagesWithNotes, GeoPoint location)
        {
            if (images?.Any() == false && imagesWithNotes?.Any() == false)
            {
                throw new ArgumentNullException();
            }

            using (var repository = UnitOfWorkFactory())
            {
                PropertyAttachmentItem propertyAttachmentItem = null;
                List<Image> imagesList = new List<Image>();

                var propertyAttachmentEntry =
                    repository.Get<PropertyAttachment>()
                        .Include(x => x.Items)
                        .Include(x => x.Property)
                        .Include(x => x.Property.Territory)
                        .FirstOrDefault(x => x.Guid == propertyAttachmentID);


                if (propertyAttachmentEntry == null)
                {
                    throw new ArgumentException(nameof(propertyAttachmentID));
                }


                if (propertyAttachmentItemID.HasValue)
                {
                    propertyAttachmentItem =
                        repository
                            .Get<PropertyAttachmentItem>()
                            .FirstOrDefault(p => p.Guid == propertyAttachmentItemID.Value);
                }
                else
                {
                    if (!string.IsNullOrEmpty(itemID) && !string.IsNullOrEmpty(sectionID))
                    {
                        //find the item by section and id
                        propertyAttachmentItem = propertyAttachmentEntry.Items?.FirstOrDefault(x => x.SectionID.Equals(sectionID) && x.ItemID.Equals(itemID));

                        //if the item does not exist, create it
                        if (propertyAttachmentItem == null)
                        {
                            propertyAttachmentItem = new PropertyAttachmentItem
                            {
                                SectionID = sectionID,
                                ItemID = itemID,
                                Status = ItemStatus.New,
                                PropertyAttachmentID = propertyAttachmentID
                            };

                            repository.Add(propertyAttachmentItem);
                            repository.SaveChanges();
                        }
                    }

                }

                if (propertyAttachmentItem == null)
                {
                    throw new ArgumentException($"{nameof(itemID)} and/or {nameof(sectionID)}");
                }

                var propertyAttachmentItemName = $"{propertyAttachmentEntry.Property.Territory.OUID}/properties/{propertyAttachmentEntry.Property.Guid}/{propertyAttachmentEntry.Guid}/{propertyAttachmentItem.SectionID}_{propertyAttachmentItem.ItemID}/{propertyAttachmentItem.Guid}";
                propertyAttachmentItemName = propertyAttachmentItemName.ToLower();

                var imageIDs = new List<Guid>();
                var imgNotes = imagesWithNotes ?? images.Select(i => new ImageBase64WithNotes { EncodedImage = i, Notes = string.Empty });

                foreach (var imgWithNotes in imgNotes)
                {
                    //upload original image
                    var uniqueImgIdentifier = Guid.NewGuid();
                    var originalImageBytes = Convert.FromBase64String(imgWithNotes.EncodedImage);
                    System.Drawing.Image img;
                    using (var ms = new MemoryStream(originalImageBytes))
                    {
                        img = System.Drawing.Image.FromStream(ms);

                        var imageName = $"{propertyAttachmentItemName}/{uniqueImgIdentifier}";

                        var originalImageUrl = _blobService.Value.UploadByNameGetFileUrl(imageName + ".jpeg", new BlobModel { Content = originalImageBytes, ContentType = GetMimeType(img.RawFormat) }, BlobAccessRights.PublicRead);

                        //generate and upload thumbnails
                        var smallThumb = GetAndUploadThumbnail(img, ThumbnailType.Small, imageName);
                        var mediumThumb = GetAndUploadThumbnail(img, ThumbnailType.Medium, imageName);
                        var largeThumb = GetAndUploadThumbnail(img, ThumbnailType.Large, imageName);

                        imageIDs.Add(uniqueImgIdentifier);

                        imagesList.Add(new Image
                        {
                            ID = uniqueImgIdentifier,
                            UserID = SmartPrincipal.UserId,
                            Name = imageName + ".jpeg",
                            Url = originalImageUrl,
                            Thumbnails = new List<Thumbnail> { smallThumb, mediumThumb, largeThumb },
                            Location = location,
                            Notes = imgWithNotes.Notes,
                            Status = ItemStatus.New
                        });
                    }

                }

                var userName = _authService.Value.GetCurrentUserFullName();
                var definition = GetDefinition(propertyAttachmentEntry.PropertyID, propertyAttachmentEntry.AttachmentTypeID);
                var description = definition != null ? $" for {definition.GetTaskName(itemID)} in { definition.GetSectionName(sectionID)} section" : null;

                using (var context = new DataContext())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                    {
                        try
                        {
                            //get the item again from a new repo in order to avoid race conditions
                            propertyAttachmentItem = context
                                    .PropertyAttachmentItems
                                    .FirstOrDefault(p => p.Guid == propertyAttachmentItem.Guid);

                            var entityImages = propertyAttachmentItem.GetImages() ?? new List<Image>();
                            entityImages.AddRange(imagesList);
                            propertyAttachmentItem.SetImages(entityImages);
                            propertyAttachmentItem.Status = ItemStatus.Pending;


                            //update the status of the whole attachment
                            propertyAttachmentEntry = context.PropertyAttachments.FirstOrDefault(p => p.Guid == propertyAttachmentID);
                            if (propertyAttachmentEntry.Status != ItemStatus.Saved)
                            {
                                propertyAttachmentEntry.Status = ItemStatus.Pending;

                            }

                            var pluralize = imgNotes.Count() > 1 ? "s" : "";

                            var audit = new AuditItem
                            {
                                UserName = userName,
                                Action = $"uploaded {imgNotes.Count()} image{pluralize}{description}.",
                                ItemIDs = imageIDs.Select(id => id.ToString()).ToList()
                            };
                            propertyAttachmentEntry.AddAudit(audit);


                            context.SaveChanges();

                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();
                        }

                        return new PropertyAttachmentItemDTO(propertyAttachmentItem);
                    }
                }
            }
        }

        public PropertyAttachmentItemDTO DeleteImage(Guid propertyAttachmentItemID, Guid imageID)
        {
            using (var repository = UnitOfWorkFactory())
            {
                var propertyAttachmentItem = repository
                                .Get<PropertyAttachmentItem>()
                                .Include(pai => pai.PropertyAttachment)
                                .FirstOrDefault(x => x.Guid == propertyAttachmentItemID);
                if (propertyAttachmentItem == null)
                {
                    throw new Exception($"Could not find property attachment item with ID: {propertyAttachmentItemID}");
                }

                if (string.IsNullOrEmpty(propertyAttachmentItem.ImagesJson))
                {
                    throw new Exception("Property attachment item doesn't have any images");
                }

                var imagesList = propertyAttachmentItem.GetImages();
                var image = imagesList?.FirstOrDefault(x => x.ID == imageID);

                if (image == null)
                {
                    throw new Exception("Property attachment item does not contain an image with the specified ID");
                }

                imagesList.Remove(image);

                propertyAttachmentItem.SetImages(imagesList);
                propertyAttachmentItem.Status = ItemStatus.Pending;

                var propertyAttachmentEntry = propertyAttachmentItem.PropertyAttachment;
                var userName = _authService.Value.GetCurrentUserFullName();
                var definition = GetDefinition(propertyAttachmentEntry.PropertyID, propertyAttachmentEntry.AttachmentTypeID);
                var description = definition != null ? $" from {definition.GetTaskName(propertyAttachmentItem.ItemID)} in { definition.GetSectionName(propertyAttachmentItem.SectionID)} section." : null;

                var audit = new AuditItem
                {
                    UserName = userName,
                    Action = $"Deleted image {description}",
                    ItemIDs = new List<string> { image.ID.ToString() }
                };
                propertyAttachmentEntry.AddAudit(audit);

                if (propertyAttachmentEntry.Status != ItemStatus.Saved)
                {
                    propertyAttachmentEntry.Status = ItemStatus.Pending;
                }

                //repository.Update(propertyAttachmentItem);
                repository.SaveChanges();

                //delete original image
                _blobService.Value.DeleteByName(image.Name);

                //delete thumbnails
                image.Thumbnails.ForEach(x =>
                {
                    _blobService.Value.DeleteByName($"{image.Name}_{GetWidthByType(x.Type)}");
                });

                return new PropertyAttachmentItemDTO(propertyAttachmentItem);
            }
        }

        public PropertyAttachmentItemDTO EditImageNotes(Guid propertyAttachmentItemID, Guid imageID, string notes)
        {
            using (var repository = UnitOfWorkFactory())
            {
                var propertyAttachmentItem = repository
                                .Get<PropertyAttachmentItem>()
                                .Include(pai => pai.PropertyAttachment)
                                .FirstOrDefault(x => x.Guid == propertyAttachmentItemID);
                if (propertyAttachmentItem == null)
                {
                    throw new Exception($"Could not find property attachment item with ID: {propertyAttachmentItemID}");
                }

                if (string.IsNullOrEmpty(propertyAttachmentItem.ImagesJson))
                {
                    throw new Exception("Property attachment item doesn't have any images");
                }

                var imagesList = propertyAttachmentItem.GetImages();
                var image = imagesList?.FirstOrDefault(x => x.ID == imageID);

                if (image == null)
                {
                    throw new Exception("Property attachment item does not contain an image with the specified ID");
                }

                image.Notes = notes;

                propertyAttachmentItem.SetImages(imagesList);
                propertyAttachmentItem.Status = ItemStatus.Pending;

                var propertyAttachmentEntry = propertyAttachmentItem.PropertyAttachment;
                var userName = _authService.Value.GetCurrentUserFullName();
                var definition = GetDefinition(propertyAttachmentEntry.PropertyID, propertyAttachmentEntry.AttachmentTypeID);
                var description = definition != null ? $" from {definition.GetTaskName(propertyAttachmentItem.ItemID)} in { definition.GetSectionName(propertyAttachmentItem.SectionID)} section." : null;

                var audit = new AuditItem
                {
                    UserName = userName,
                    Action = $"Updated notes on image {description}",
                    ItemIDs = new List<string> { image.ID.ToString() }
                };
                propertyAttachmentEntry.AddAudit(audit);

                if (propertyAttachmentEntry.Status != ItemStatus.Saved)
                {
                    propertyAttachmentEntry.Status = ItemStatus.Pending;
                }

                repository.SaveChanges();

                return new PropertyAttachmentItemDTO(propertyAttachmentItem);
            }
        }

        public void UpdatePropertyAttachment(Guid guid, EditPropertyAttachmentRequest request)
        {
            if (request == null)
            {
                return;
            }
            using (var uow = UnitOfWorkFactory())
            {
                var entity = uow
                        .Get<PropertyAttachment>()
                        .FirstOrDefault(pa => pa.Guid == guid);
                if (entity == null)
                {
                    return;
                }

                if (entity.SystemTypeID != request.SystemtypeID)
                {
                    var userName = _authService.Value.GetCurrentUserFullName();
                    var definition = GetDefinition(entity.PropertyID, entity.AttachmentTypeID);
                    var audit = new AuditItem
                    {
                        UserName = userName,
                        Action = $"Changed system type from {definition?.GetSectionName(entity.SystemTypeID)} to {definition.GetSectionName(request.SystemtypeID)}"
                    };
                    entity.AddAudit(audit);
                    entity.SystemTypeID = request.SystemtypeID;
                }

                uow.SaveChanges();
            }
        }

        public bool SubmitPropertyAttachment(Guid guid)
        {
            using (var uow = UnitOfWorkFactory())
            {
                var entity = uow
                        .Get<PropertyAttachment>()
                        .Include(pa => pa.Items)
                        .Include(pa => pa.Property)
                        .FirstOrDefault(pa => pa.Guid == guid);
                if (entity == null)
                {
                    return false;
                }

                if (entity.Status != ItemStatus.Submitted)
                {
                    var def = GetDefinitionTuple(entity.PropertyID, entity.AttachmentTypeID);

                    if (!entity.AllHaveImages(def.definition))
                    {
                        return false;
                    }

                    var userName = _authService.Value.GetCurrentUserFullName();
                    var audit = new AuditItem
                    {
                        UserName = userName,
                        Action = $"Changed status to Submitted."
                    };
                    entity.Status = ItemStatus.Submitted;
                    entity.AddAudit(audit);

                    SendItemSubmittedEmail(entity, def.option, def.definition, entity.Status == ItemStatus.New);
                }

                uow.SaveChanges();
                return true;
            }
        }

        public bool SubmitPropertyAttachmentSection(Guid guid, string sectionId)
        {
            if (string.IsNullOrEmpty(sectionId))
            {
                return false;
            }

            using (var uow = UnitOfWorkFactory())
            {
                var entity = uow
                        .Get<PropertyAttachment>()
                        .Include(pa => pa.Items)
                        .Include(pa => pa.Property)
                        .FirstOrDefault(pa => pa.Guid == guid);
                if (entity == null)
                {
                    return false;
                }


                var def = GetDefinitionTuple(entity.PropertyID, entity.AttachmentTypeID);

                if (entity.AllWithinSectionHaveImages(def.definition, sectionId))
                {
                    var userName = _authService.Value.GetCurrentUserFullName();
                    entity
                        ?.Items
                        ?.Where(i => i.SectionID == sectionId)
                        ?.ToList()
                        ?.ForEach(x =>
                        {
                            x.Status = ItemStatus.Submitted;

                            var sectionName = def.definition.GetSectionName(x.SectionID);
                            entity.AddAudit(new AuditItem
                            {
                                UserName = userName,
                                Action = $"Changed status of section {sectionName} to Submitted."
                            });

                            //set images as submitted too
                            var images = x.GetImages();
                            images.ForEach(img =>
                            {
                                img.Status = ItemStatus.Submitted;
                            });
                            x.SetImages(images);

                            x.Updated(SmartPrincipal.UserId);
                        });
                    uow.SaveChanges();
                    return true;
                }

                return false;
            }
        }

        public bool SubmitPropertyAttachmentTask(Guid guid, string sectionId, string taskId)
        {
            if (string.IsNullOrEmpty(sectionId) || string.IsNullOrEmpty(taskId))
            {
                return false;
            }
            using (var uow = UnitOfWorkFactory())
            {
                var entity = uow
                        .Get<PropertyAttachment>()
                        .Include(pa => pa.Items)
                        .Include(pa => pa.Property)
                        .FirstOrDefault(pa => pa.Guid == guid);
                if (entity == null)
                {
                    return false;
                }

                var matchingItem = entity.Items.FirstOrDefault(i => i.SectionID == sectionId && i.ItemID == taskId);

                if (matchingItem == null || matchingItem.GetImages().Any() != true)
                {
                    return false;
                }

                matchingItem.Status = ItemStatus.Submitted;

                //Add Audit Log for this operation
                var def = GetDefinitionTuple(entity.PropertyID, entity.AttachmentTypeID);
                var userName = _authService.Value.GetCurrentUserFullName();
                var taskName = def.definition.GetTaskName(taskId);
                var sectionName = def.definition.GetSectionName(matchingItem.SectionID);
                entity.AddAudit(new AuditItem
                {
                    UserName = userName,
                    Action = $"Changed status of task {taskName} from section {sectionName} to Submitted."
                });

                //set images as submitted too
                var images = matchingItem.GetImages();
                images.ForEach(x =>
                {
                    x.Status = ItemStatus.Submitted;
                });
                matchingItem.SetImages(images);

                matchingItem.Updated(SmartPrincipal.UserId);
                uow.SaveChanges();
                return true;
            }
        }

        public bool ReviewPropertyAttachment(Guid guid, PropertyAttachmentSubmitReviewRequest request)
        {
            if (request == null || request.Items?.Any() != true)
            {
                return false;
            }
            using (var uow = UnitOfWorkFactory())
            {
                var entity = uow
                        .Get<PropertyAttachment>()
                        .Include(pa => pa.Property)
                        .Include(pa => pa.Items)
                        .FirstOrDefault(pa => pa.Guid == guid);
                if (entity == null)
                {
                    return false;
                }

                var definitionItems = GetDefinitionTuple(entity.PropertyID, entity.AttachmentTypeID);
                var definition = definitionItems.definition;
                var userName = _authService.Value.GetCurrentUserFullName();
                string reviewMessage = "";

                request.Items.ForEach(itm =>
                {
                    var entityItem = entity.Items?.FirstOrDefault(eItm => eItm.Guid == itm.Guid);
                    reviewMessage += itm.UpdateEntity(ref entityItem, definition);
                });

                //if user saved, but not submitted the review
                if (request.Status == ItemStatus.Saved)
                {
                    entity.Status = ItemStatus.Saved;
                    reviewMessage += "Changed status to Saved.";
                }
                else
                {
                    // check if all tasks (items) have been approved, and approve the whole attachment
                    if (entity.AllAreApproved(definition))
                    {
                        if (entity.Status != ItemStatus.Approved)
                        {
                            entity.Status = ItemStatus.Approved;
                            reviewMessage += "Changed status to Approved.";
                        }

                    }
                    else
                    {

                        //send emails to users with rejected items and/or images
                        entity.Status = ItemStatus.Pending;
                        var rejectedItemIds = request.Items?.Where(x => x.Status == ItemStatus.Rejected).Select(x => x.Guid);

                        var rejectedImageIDs =
                            request.Items
                                ?.Where(x => x.Status == ItemStatus.Rejected)
                                .SelectMany(x =>
                                    x.Images
                                        .Where(i => i.Status == ItemStatus.Rejected)
                                        .Select(i => i.ImageID));

                        //set status to rejected if any of the underlying items has that status
                        if (rejectedItemIds?.Any() == true || rejectedImageIDs.Any() == true || entity.Items.Any(i => i.Status == ItemStatus.Rejected))
                        {
                            entity.Status = ItemStatus.Rejected;
                        }
                        reviewMessage += $"Changed status to {(entity.Status == ItemStatus.Rejected ? "Rejected" : "Pending")}.";
                        SendItemsRejectedEmail(entity, rejectedItemIds, rejectedImageIDs, definitionItems.option, definition);
                    }
                }



                if (!string.IsNullOrEmpty(reviewMessage))
                {
                    var audit = new AuditItem
                    {
                        UserName = userName,
                        Action = reviewMessage
                    };
                    entity.AddAudit(audit);
                }

                uow.SaveChanges();
                return true;
            }
        }

        public IEnumerable<ExtendedPropertyAttachmentDTO> GetPropertyAttachmentsForProperty(Guid propertyID)
        {
            using (var repository = UnitOfWorkFactory())
            {
                var propertyAttachments =
                    repository
                        .Get<PropertyAttachment>()
                        .Include(x => x.Property)
                        .Include(x => x.Items)
                        .Where(x => x.PropertyID == propertyID)
                        .ToList();

                var createdByIDs = propertyAttachments?.Select(x => x.CreatedByID);
                var persons = createdByIDs != null ? repository.Get<Person>().Where(x => createdByIDs.Contains(x.Guid)).ToList() : null;

                return propertyAttachments.Select(x =>
                {
                    var definition = GetDefinitionTuple(x.PropertyID, x.AttachmentTypeID);
                    return new ExtendedPropertyAttachmentDTO(x, definition.definition, definition.option, persons?.FirstOrDefault(p => p.Guid == x.CreatedByID));
                });
            }
        }

        public ExtendedPropertyAttachmentDTO GetPropertyAttachmentData(Guid propertyAttachmentID)
        {
            var propertyAttachment = Get(propertyAttachmentID, include: "Property,Items,Property.Territory,Property.Territory.OU");

            if (propertyAttachment == null)
            {
                throw new Exception($"Could not find any property attachment with id: {propertyAttachmentID}");
            }

            var definitionItems = GetDefinitionTuple(propertyAttachment.PropertyID, propertyAttachment.AttachmentTypeID);

            return new ExtendedPropertyAttachmentDTO(propertyAttachment, definitionItems.definition, definitionItems.option, GetPerson(propertyAttachment.CreatedByID));

        }

        public ICollection<ExtendedPropertyAttachmentDTO> GetPagedPropertyAttachments(int pageIndex, int pageSize, string query)
        {
            using (var repository = UnitOfWorkFactory())
            {

                var propertyAttachmentsQuery = repository
                    .Get<PropertyAttachment>()
                    .Include(x => x.Property)
                    .Include(x => x.Property.Territory)
                    .Include(x => x.Property.Territory.OU)
                    //.Include(x => x.Items)
                    .Where(pa => !pa.IsDeleted);

                if (!string.IsNullOrEmpty(query))
                {
                    query = query.ToLower();

                    propertyAttachmentsQuery = propertyAttachmentsQuery.Where(x => x.Property.Name.StartsWith(query) || x.Property.Address1.StartsWith(query));
                }

                var propertyAttachments =
                    propertyAttachmentsQuery
                    .OrderByDescending(x => x.DateCreated)
                    .Skip(pageIndex * pageSize).Take(pageSize)
                    .ToList();

                var peopleIDs = propertyAttachments?
                                .Where(pa => pa.CreatedByID.HasValue)?
                                .Select(pa => pa.CreatedByID.Value)?
                                .Distinct()?
                                .ToList();

                var people = _personService.Value.GetMany(peopleIDs);

                //get OU settings corresponding to the properties
                var distinctOUIDs = propertyAttachments
                    .Where(x => !x.Property.IsDeleted && !x.Property.Territory.IsDeleted && !x.Property.Territory.OU.IsDeleted)
                    .Select(x => x.Property.Territory.OUID).Distinct();

                var ouSettings = _ouSettingsService.Value.GetOuSettingsMany(distinctOUIDs);


                var list =
                    propertyAttachments
                        ?.Select(x =>
                        {
                            var settKeyValue = ouSettings.FirstOrDefault(s => s.Key == x.Property.Territory.OUID);
                            List<OUSetting> matchingSettings = null;
                            if (!settKeyValue.Equals(default(KeyValuePair<Guid, List<OUSetting>>)))
                            {
                                matchingSettings = settKeyValue.Value;
                            }
                            var definitionItems = GetDefinitionTuple(matchingSettings, x.AttachmentTypeID);
                            return new ExtendedPropertyAttachmentDTO(x, definitionItems.definition, definitionItems.option, people?.FirstOrDefault(p => p.Guid == x.CreatedByID));
                        })
                        ?.ToList() ?? new List<ExtendedPropertyAttachmentDTO>();

                return list;
            }
        }

        public IEnumerable<SBAttachmentDataDTO> GetSBAttachmentData(long propertyID, string apiKey)
        {
            using (var uow = UnitOfWorkFactory())
            {
                // we 1st get the property
                var property = uow
                        .Get<Property>()
                        .Include(p => p.Attachments.Select(a => a.Items))
                        .FirstOrDefault(p => p.Id == propertyID);

                if (property == null || property?.Attachments?.Any() != true)
                {
                    yield break;
                }

                // validate apiKey
                //var sbSettings = _ouSettingsService.Value.GetOUSettingForPropertyID<SSTSettings>(property.Guid, SolarTrackerResources.SettingName);
                var sbSettings = _ouSettingsService
                                    .Value
                                    .GetOUSettingForPropertyID<ICollection<SelectedIntegrationOption>>(property.Guid, SolarTrackerResources.SelectedSettingName)?
                                    .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                    .Data?
                                    .SMARTBoard;

                if (sbSettings?.ApiKey != apiKey)
                {
                    yield break;
                }
                var orgSettings = _ouSettingsService.Value.GetSettingsByPropertyID(property.Guid);
                var attachmentOptions = orgSettings.GetValue<List<AttachmentOptionDefinition>>(OUSetting.Legion_Photos_Options);

                if (attachmentOptions?.Any() != true)
                {
                    yield break;
                }
                var defIDs = attachmentOptions?.Select(attOpt => attOpt.OUSettingName)?.ToList();

                var defOUSettings = orgSettings?
                                    .Where(os => defIDs.Contains(os.Name))?
                                    .ToList();

                foreach (var attOpt in attachmentOptions)
                {
                    var definition = defOUSettings?
                                        .FirstOrDefault(os => os.Name == attOpt.OUSettingName)?
                                        .GetValue<PhotosDefinitionSettingModel>();
                    var attachment = property
                                        .Attachments?
                                        .FirstOrDefault(att => att.AttachmentTypeID == attOpt.Id);
                    if (attachment == null)
                    {
                        continue;
                    }

                    var attachmentDTO = new PropertyAttachmentDTO(attachment);

                    var personIDs = attachmentDTO?
                                    .Items?
                                    .SelectMany(itm => itm.Images?.Select(img => img.UserID))?
                                    .ToList();
                    List<Person> people = null;

                    if (personIDs?.Any() == true)
                    {
                        people = uow
                            .Get<Person>()
                            .Where(p => personIDs.Contains(p.Guid))
                            .ToList();
                    }

                    yield return new SBAttachmentDataDTO(definition, attachmentDTO, people, attOpt);
                }
            }
            yield break;
        }

        private int ParseConfiguration(string key, int defaultValue)
        {
            int value;
            if (int.TryParse(ConfigurationManager.AppSettings[key], out value))
            {
                return value;
            }

            return defaultValue;
        }


        private int GetWidthByType(ThumbnailType type)
        {
            switch (type)
            {
                case ThumbnailType.Large: return ThumbnailLargeWidth;
                case ThumbnailType.Medium: return ThumbnailMediumWidth;
                case ThumbnailType.Small: return ThumbnailSmallWidth;
                default: return 0;
            }
        }

        private Thumbnail GetAndUploadThumbnail(System.Drawing.Image img, ThumbnailType thumbnailType, string name)
        {

            decimal width = GetWidthByType(thumbnailType);
            decimal resizeRatio = img.Width / width;
            decimal height = (img.Height / resizeRatio);

            var thumbBytes = img.GetResizedImageContent((int)width, (int)height);

            string thumbUrl =
                _blobService
                    .Value
                    .UploadByNameGetFileUrl($"{name}_{width}.jpeg",
                        new BlobModel { Content = thumbBytes, ContentType = GetMimeType(img.RawFormat) },
                        BlobAccessRights.PublicRead);

            return new Thumbnail
            {
                Url = thumbUrl,
                Type = thumbnailType
            };

        }

        private string GetMimeType(ImageFormat imageFormat)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs?.First(codec => codec.FormatID == imageFormat.Guid)?.MimeType ?? "image/jpeg";
        }

        private List<AttachmentOptionDefinition> GetAttachmentOptions(Guid propertyID)
        {
            return _ouSettingsService
                        .Value
                        .GetOUSettingForPropertyID<List<AttachmentOptionDefinition>>(propertyID, OUSetting.Legion_Photos_Options);
        }


        private PhotosDefinitionSettingModel GetDefinition(Guid propertyID, int definitionId)
        {
            var attachmentOptions = GetAttachmentOptions(propertyID);

            var definitionSettingName = attachmentOptions?
                                            .FirstOrDefault(x => x.Id == definitionId)?
                                            .OUSettingName;

            if (!string.IsNullOrEmpty(definitionSettingName))
            {
                return _ouSettingsService
                    .Value
                    .GetOUSettingForPropertyID<PhotosDefinitionSettingModel>(propertyID, definitionSettingName);
            }

            return null;
        }
        private (AttachmentOptionDefinition option, PhotosDefinitionSettingModel definition) GetDefinitionTuple(IEnumerable<OUSetting> settings, int definitionId)
        {
            var attachmentOptions = settings?
                            .FirstOrDefault(s => s.Name == OUSetting.Legion_Photos_Options)?
                            .GetValue<List<AttachmentOptionDefinition>>();
            if (attachmentOptions != null)
            {
                var matchingAttachmentOption = attachmentOptions.FirstOrDefault(x => x.Id == definitionId);
                var definitionSettingName = matchingAttachmentOption?.OUSettingName;
                if (!string.IsNullOrEmpty(definitionSettingName))
                {
                    var photosDefinition = settings?
                            .FirstOrDefault(s => s.Name == definitionSettingName)?
                            .GetValue<PhotosDefinitionSettingModel>();

                    return (option: matchingAttachmentOption, definition: photosDefinition);
                }
            }

            return (option: null, definition: null);
        }

        private (AttachmentOptionDefinition option, PhotosDefinitionSettingModel definition) GetDefinitionTuple(Guid propertyID, int definitionId)
        {
            var attachmentOptions = GetAttachmentOptions(propertyID);

            if (attachmentOptions != null)
            {
                var matchingAttachmentOption = attachmentOptions.FirstOrDefault(x => x.Id == definitionId);
                var definitionSettingName = matchingAttachmentOption?.OUSettingName;
                if (!string.IsNullOrEmpty(definitionSettingName))
                {
                    var photosDefinition = _ouSettingsService
                        .Value
                        .GetOUSettingForPropertyID<PhotosDefinitionSettingModel>(propertyID, definitionSettingName);

                    return (option: matchingAttachmentOption, definition: photosDefinition);
                }

            }

            return (option: null, definition: null);
        }

        private Person GetPerson(Guid? personID)
        {
            if (personID.HasValue)
            {
                return _personService.Value.Get(personID.Value);
            }

            return null;
        }

        private void SendItemSubmittedEmail(PropertyAttachment entity, AttachmentOptionDefinition option, PhotosDefinitionSettingModel definition, bool newSubmission)
        {
            if (option.SubmissionReceiverEmails != null && option.SubmissionReceiverEmails.Length > 0)
            {
                foreach (var email in option.SubmissionReceiverEmails)
                {
                    var template = new PropertyAttachmentSubmittedTemplate
                    {
                        EmailSubject = $"{(newSubmission ? "NEW SUBMISSION" : "RE-SUBMISSION")}  {entity?.Property?.Name} {entity?.Property?.City}",
                        RecipientEmailAddress = email,
                        PropertyName = entity?.Property?.Name,
                        Address = $"{entity?.Property?.Address1} {entity?.Property?.City}, {entity?.Property?.State}"
                    };

                    Mail.Library.SendPropertyAttachmentSubmittedEmail(template);
                }
            }
        }

        private void SendItemsRejectedEmail(PropertyAttachment entity, IEnumerable<Guid> rejectedItemIds, IEnumerable<Guid> rejectedImageIds, AttachmentOptionDefinition options, PhotosDefinitionSettingModel definition)
        {

            var sectionDictionary = new Dictionary<Guid, List<PropertyAttachmentRejectedItemTemplate>>();

            //add section to all people that have an image under that item
            entity
                ?.Items
                ?.Where(x => rejectedItemIds.Contains(x.Guid) && !string.IsNullOrEmpty(x.ImagesJson))
                ?.ToList()
                ?.ForEach(x =>
                {
                    foreach (var userId in x.GetImages().Select(i => i.UserID).Distinct())
                    {
                        var section = GetItemTemplate(new ExtendedPropertyAttachmentItemDTO(x, definition));
                        if (sectionDictionary.ContainsKey(userId))
                        {
                            sectionDictionary[userId].Add(section);
                        }
                        else
                        {
                            sectionDictionary.Add(userId, new List<PropertyAttachmentRejectedItemTemplate> { section });
                        }
                    }
                });



            //add sections for each rejected image
            var rejectedItemsForImages = entity.Items.Where(x => x.GetImages().Any(i => rejectedImageIds.Contains(i.ID)));

            foreach (var item in rejectedItemsForImages)
            {
                foreach (var image in item.GetImages().Where(i => rejectedImageIds.Contains(i.ID)))
                {
                    var section = GetItemTemplate(new ExtendedPropertyAttachmentItemDTO(item, definition), image);
                    if (sectionDictionary.ContainsKey(image.UserID))
                    {
                        sectionDictionary[image.UserID].Add(section);
                    }
                    else
                    {
                        sectionDictionary.Add(image.UserID, new List<PropertyAttachmentRejectedItemTemplate> { section });
                    }
                }
            }

            //send emails
            var people = _personService.Value.GetMany(sectionDictionary.Keys);
            foreach (var person in people)
            {
                var template = new PropertyAttachmentRejectedTemplate
                {
                    EmailSubject = $"[{options.Name}] {entity?.Property?.Name} (REJECTED)",
                    RecipientEmailAddress = person.EmailAddressString,
                    ToPersonName = $"{person.FirstName} {person.LastName}",
                    PropertyName = entity?.Property?.Name,
                    PropertyAddress = $"{entity?.Property?.Address1} {entity?.Property?.City}, {entity?.Property?.State}",
                    RejectedItems = sectionDictionary[person.Guid]
                };

                Mail.Library.SendPropertyAttachmentRejectedEmail(template);
            }

        }

        private PropertyAttachmentRejectedItemTemplate GetItemTemplate(ExtendedPropertyAttachmentItemDTO item)
        {

            return new PropertyAttachmentRejectedItemTemplate
            {
                SectionName = item.SectionName,
                ItemName = item.ItemName,
                RejectionMessage = item.RejectionMessage
            };
        }

        private PropertyAttachmentRejectedItemTemplate GetItemTemplate(ExtendedPropertyAttachmentItemDTO item, Image image)
        {
            var model = GetItemTemplate(item);
            model.RejectionMessage = image.RejectionMessage;
            model.ImageUrl = image?.Thumbnails?.FirstOrDefault(t => t.Type == ThumbnailType.Small)?.Url;
            return model;
        }

        public PropertyAttachmentItemDTO UploadUtilityBillImage(Guid PropertyId, UploadImageToPropertyAttachmentRequest uploadImageRequest)
        {

            PropertyAttachment item = new PropertyAttachment();

            using (var context = new DataContext())
            {
                item = context.PropertyAttachments.FirstOrDefault(p => p.PropertyID == PropertyId && p.AttachmentTypeID == 1);
            }

            if (item == null)
            {
                item = new PropertyAttachment();
                item.Guid = Guid.NewGuid();
                item.IsDeleted = false;
                item.PropertyID = PropertyId;
                item.SystemTypeID = "st-01";
                item.AttachmentTypeID = 1;
                item = Insert(item);
            }
            else
            {
                item = Update(item);
            }

            
                var propertyattech = base.Get(item.Guid, "Items");

            //var propertyattechitm = propertyattech.Items.Where();

            var attachitm = propertyattech?.Items?.Where(i => i.SectionID == "s-0" && i.ItemID == "t-10")?.FirstOrDefault();

            uploadImageRequest.PropertyAttachmentID = item.Guid;
            if(attachitm != null) uploadImageRequest.PropertyAttachmentItemID = attachitm.Guid;
            uploadImageRequest.SectionID = "s-0";
            uploadImageRequest.ItemID = "t-10";
            //uploadImageRequest.ImagesWithNotes = ;
            //uploadImageRequest.Location = ;

            var response = UploadImage(uploadImageRequest.PropertyAttachmentID,
                    uploadImageRequest.PropertyAttachmentItemID,
                    uploadImageRequest.SectionID,
                    uploadImageRequest.ItemID,
                    uploadImageRequest.Images,
                    uploadImageRequest.ImagesWithNotes,
                    uploadImageRequest.Location);

            return response;
        }



    }
}
