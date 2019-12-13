using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Google.Util
{
    internal class Constants
    {
        public static char PipeDelimiter = '|';
        public static char CommaDelimiter = ',';
        public static string[] SheetScopes = { SheetsService.Scope.Spreadsheets };

        public static string _accountKeyFilePath;
        public static string AccountKeyFilePath
        {
            get
            {
                if (_accountKeyFilePath == null)
                {
                    _accountKeyFilePath = ConfigurationManager.AppSettings["DataReef.Integrations.Google.AccountKey_FileName"];
                    if (!Path.IsPathRooted(_accountKeyFilePath))
                    {
                        _accountKeyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _accountKeyFilePath);
                    }
                }
                return _accountKeyFilePath;
            }
        }

        public static string SheetID = ConfigurationManager.AppSettings["DataReef.Integrations.Google.SpreadSheetID"];
    }
}
