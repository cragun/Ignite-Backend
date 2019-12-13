using DataReef.Integrations.Google.Models;
using DataReef.Integrations.Google.Util;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataReef.Integrations.Google.Processors
{
    public class SheetProcessor
    {
        private string spreadsheetId = null;

        public List<FormModel> Process(SheetOptions options)
        {
            spreadsheetId = options?.SheetID ?? Constants.SheetID;
            var endColumn = options?.EndColumn ?? "U";
            var inputRanges = options?.GetRanges(endColumn);

            var hasInputRows = inputRanges?.Count > 0;
            var ranges = inputRanges ?? new List<string> { $"A2:{endColumn}100" };

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = Credentials()
            });

            // get headers
            var headers = GetRangeValues($"A1:{endColumn}1", service)[0]
                            .Select(o => o?.ToString())
                            .ToList();

            // get values
            var values = GetRangeValues(ranges, service);

            // process values from sheet, build a FormModel list
            var models = values
                            .Select((v, index) => new FormModel(headers, v, index + 2))
                            .ToList();

            // if options hasInputRows, we will skipp the Process flag
            var modelsToProcess = models
                                    .Where(m => hasInputRows || m.Process)
                                    .ToList();

            if (options?.SetProcessedFlag == true)
            {
                var processColumn = options?.ProcessColumn ?? "T";
                var processTimestampColumn = options?.ProcessTimestampColumn ?? "U";

                foreach (var item in modelsToProcess)
                {
                    UpdateProcessed(service, item.RowNumber, processColumn, processTimestampColumn);
                }
            }
            return modelsToProcess;
        }

        private IList<IList<string>> GetRangeValues(List<string> ranges, SheetsService service)
        {
            SpreadsheetsResource.ValuesResource.BatchGetRequest request = new SpreadsheetsResource.ValuesResource.BatchGetRequest(service, spreadsheetId);
            request.Ranges = ranges;

            var response = request.Execute();

            // convert to IList<IList<string>>
            var values = response
                            .ValueRanges
                            .Where(vr => vr?.Values?.Count > 0)
                            .SelectMany(vr => vr.Values.Select(v => v.ToStringsList()))
                            .ToList();

            return values;
        }

        private IList<IList<string>> GetRangeValues(string range, SheetsService service)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);


            var ranges = service.Spreadsheets.Get(spreadsheetId).Ranges;

            ValueRange response = request.Execute();

            // convert to IList<IList<string>>
            var values = response
                            .Values
                            .Select(v => v.ToStringsList())
                            .ToList();

            return values;
        }

        /// <summary>
        /// Set values for last two columns: "Process (on next run)" and "Processed timestamp"
        /// </summary>
        /// <param name="service"></param>
        /// <param name="rowNumber"></param>
        private void UpdateProcessed(SheetsService service, int rowNumber, string processColumn, string processTimestampColumn)
        {
            var range = $"{processColumn}{rowNumber}:{processTimestampColumn}{rowNumber}";
            var data = new ValueRange
            {
                Range = range,
                Values = new List<IList<object>>() { new List<object>() { "no", DateTime.UtcNow.ToString() } }
            };

            var req = service.Spreadsheets.Values.Update(data, spreadsheetId, range);
            req.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            req.Execute();
        }

        private ServiceAccountCredential Credentials()
        {
            var keyFile = File.ReadAllText(Constants.AccountKeyFilePath);
            var account = ServiceAccountModel.FromString(keyFile);

            var clientEmail = account.ClientEmail;
            var privateKey = account.PrivateKey;

            var xCred = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(clientEmail)
            {
                Scopes = Constants.SheetScopes
            }
            .FromPrivateKey(privateKey));

            return xCred;
        }
    }
}
