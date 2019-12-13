using DataReef.Integrations.Google.Helpers;
using DataReef.Integrations.Google.Models;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Google.Processors
{
    public class WriteSheetProcessor
    {
        private string spreadsheetId = null;

        public void Process(WriteSheetOptions options)
        {
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = CredentialsHelper.GetCredentials()
            });


        }
    }
}
