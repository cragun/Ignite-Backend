using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.PropertyAttachments
{
    public class PhotosDefinitionSettingModel
    {
        public HeaderModel Header { get; set; }
        public DataModel Data { get; set; }

        public string GetSectionName(string sectionID)
        {
            return Data?
                    .Sections?
                    .FirstOrDefault(s => s.Id == sectionID)?
                    .Name;
        }

        public string GetTaskName(string taskID)
        {
            return Data?
                    .Tasks?
                    .FirstOrDefault(s => s.Id == taskID)?
                    .Name;
        }

        public bool IsTaskOptional(string taskID)
        {
            return Data?
                        .Tasks?
                        .FirstOrDefault(t => t.Id == taskID)?
                        .IsOptional == true;
        }

        public class HeaderModel
        {
            public string Instructions { get; set; }
        }

        public class DataModel
        {
            public List<SectionsModel> Sections { get; set; }
            public List<TaskModel> Tasks { get; set; }
            public List<SystemTypeModel> SystemTypes { get; set; }
            public List<DefinitionModel> Definitions { get; set; }
        }

        public class BaseModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class SectionsModel : BaseModel { }

        public class TaskModel : BaseModel
        {
            public string Instructions { get; set; }
            public bool? IsOptional { get; set; }
        }

        public class SystemTypeModel : BaseModel { }

        public class DefinitionModel
        {
            public string SystemTypeID { get; set; }
            public List<DefinitionSectionModel> Sections { get; set; }
        }

        public class DefinitionSectionModel
        {
            public string SectionId { get; set; }
            public List<string> TaskIDs { get; set; }
        }

    }
}
