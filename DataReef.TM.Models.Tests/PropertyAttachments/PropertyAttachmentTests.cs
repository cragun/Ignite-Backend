using DataReef.TM.Models.DTOs.PropertyAttachments;
using DataReef.TM.Models.PropertyAttachments;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static DataReef.TM.Models.DTOs.PropertyAttachments.PhotosDefinitionSettingModel;

namespace DataReef.TM.Models.Tests.PropertyAttachments
{
    public class PropertyAttachmentTests
    {
        [Fact]
        public void Test_AllHaveImages()
        {
            PropertyAttachment model = new PropertyAttachment();

            Image img = new Image
            {
                ID = Guid.NewGuid()
            };

            PropertyAttachmentItem itemModel = new PropertyAttachmentItem
            {
                SectionID = "s-01",
                ItemID = "t-01",
                ImagesJson = JsonConvert.SerializeObject(new List<Image> { img })
            };
            model.SystemTypeID = "st-01";
            model.Items = new List<PropertyAttachmentItem> { itemModel };

            var definition = new PhotosDefinitionSettingModel
            {
                Data = new DataModel
                {
                    Sections = new List<SectionsModel> { new SectionsModel { Id = "s-01" } },
                    Tasks = new List<TaskModel> { new TaskModel { Id = "t-01", IsOptional = false } },
                    Definitions = new List<DefinitionModel> { new DefinitionModel { SystemTypeID = "st-01", Sections = new List<DefinitionSectionModel> { new DefinitionSectionModel { SectionId = "s-01", TaskIDs = new List<string> { "t-01"} } } } }
                }
            };

            Assert.True(model.AllHaveImages(definition));

            itemModel.ImagesJson = null;
            model.Items = new List<PropertyAttachmentItem> { itemModel };

            Assert.False(model.AllHaveImages(definition));

        }
    }
}
