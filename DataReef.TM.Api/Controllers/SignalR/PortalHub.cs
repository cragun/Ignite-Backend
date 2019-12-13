using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Controllers.SignalR
{
    public class PortalHub : Hub
    {
        public async Task SetUserID(string userId)
        {
            await Groups.Add(Context.ConnectionId, userId);
        }

    }
}