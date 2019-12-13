using System;
using System.Collections.Generic;
using Smart.Core.Logging;
using Smart.DayCare.Contracts.Services;
using Smart.DayCare.Models;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;


namespace Smart.DayCare.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for Device, including registration
    /// </summary>
    public class DeviceController : ControllerBase<Device>
    {
        private readonly IDeviceService deviceService;
        
        public DeviceController(IDeviceService deviceService, ILogger logger)
            : base(deviceService, logger)
        {
            this.deviceService = deviceService;
        }

        public override Device Put(Device item)
        {
            return null;
        }

        public override ICollection<Device> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string include = "", string fields = "")
        {
            return base.List(deletedItems, pageNumber, itemsPerPage, include, fields);
        }

        public override Device Post(Device item)
        {
            return base.Post(item);
        }
    


    }
}
