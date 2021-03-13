
namespace DataReef.TM.Models.DTOs.Persons
{
    public class IOSVersionDTO
    {
        public string VersionValue { get; set; }
        public bool IsPopupEnabled { get; set; }
    }


    public class IOSVersionResponseModel
    {
        public string LatestVersion { get; set; }
        public string UserVersion { get; set; }
        public string PopUpMsg { get; set; }
        public bool IsPopupEnabled { get; set; }
    }
}
