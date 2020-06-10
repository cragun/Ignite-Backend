using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.Commerce;
using DataReef.TM.Models.DataViews.Commerce;
using DataReef.TM.Models.DTOs.Commerce;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using ServiceStack;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using DataReef.TM.Models.DTOs.Common;
using OfficeOpenXml;
using System.Reflection;
using System.Configuration;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/portal")]
    public class PortalController : ApiController
    {
        private readonly IOrderService _service;

        private static readonly int MaxAmountOfHomes = Convert.ToInt32(ConfigurationManager.AppSettings["Datareef.Leads.MaxAmountOfHomes"] ?? "1000");

        public PortalController(IOrderService service)
        {
            _service = service;
        }

        // Orders list
        [Route("orders/{pageIndex?}/{pageSize?}/{sortColummn?}/{sortOrder?}")]
        [ResponseType(typeof(PaginatedResult<Order>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetOrders(int pageIndex = 0, int pageSize = 20, string sortColumn = "DateCreated", int sortOrder = 1)
        {
            return Ok(_service.GetOrders(SmartPrincipal.UserId, pageIndex, pageSize, sortColumn, sortOrder));
        }

        // Token Balance
        [Route("tokens/balance")]
        [ResponseType(typeof(double))]
        [HttpGet]
        public async Task<IHttpActionResult> GetTokensBalance()
        {
            return Ok(_service.GetTokensBalance(SmartPrincipal.UserId));
        }

        // Potential Results - Mapful + max cost
        [Route("leads/count/potential")]
        [HttpPost]
        public async Task<IHttpActionResult> CountPotentialLeads(CreateLeadsDto request)
        {
            var potentialLeadsCount = _service.CountPotentialLeads(request);

            int exclusiveMonths = request.LengthOfExclusivity / 30;
            int tokensRequired = potentialLeadsCount * exclusiveMonths;

            return Ok(new { PotentialLeadsCount = potentialLeadsCount, TokensRequired = tokensRequired });
        }

        // Create order - new leads        
        [Route("orders/create")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateOrder(CreateLeadsDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ZipCode))
            {
                throw new Exception("ZipCode is required!");
            }
            if (request.LengthOfExclusivity == 0)
            {
                throw new Exception("Length of Exclusivity is required!");
            }
            if (request.MaxNumberOfLeads == 0)
            {
                throw new Exception("Number of leads is required!");
            }
            if (request.MaxNumberOfLeads > MaxAmountOfHomes)
            {
                throw new ApplicationException($"You can only submit a max order of {MaxAmountOfHomes} homes.");
            }

            _service.CreateOrderForNewLeads(request);
            return Ok();
        }

        // Create order - Upload CSV
        [Route("orders/create/upload")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadCSV()
        {
            var httpRequest = HttpContext.Current?.Request;
            if (httpRequest?.Files?.Count != 1)
            {
                return BadRequest();
            }

            var file = httpRequest.Files[0];
            if (!new List<string> { ".XLS", ".XLSX" }.Contains(file.FileName.GetExtension().ToUpper()))
            {
                throw new Exception("Invalid file type uploaded. We support .xls and .xslx files.");
            }

            var addRoofAnalysis = httpRequest?.Form?["AddRoofAnalysis"]?.ToLower() == "true";

            CreateLeadEnhancementDto request = new CreateLeadEnhancementDto();
            request.UploadedLeads = new List<UploadedLeadDto>();

            using (var excel = new ExcelPackage(file.InputStream))
            {
                ExcelWorksheet ws = excel.Workbook.Worksheets.FirstOrDefault();

                //we require the first row to be a header and we require FirstName, LastName, Address, City, State,ZipCode
                Dictionary<string, int> columnMapping = new Dictionary<string, int>();

                var requiredCols = new List<string>
                {
                    "FirstName",
                    "LastName",
                    "Address",
                    "City",
                    "State",
                    "ZipCode"
                };
                var optionalCols = new List<string>
                {
                    "Phone",
                    "Email",
                    "Identifier",
                    "JanuaryConsumption",
                    "FebruaryConsumption",
                    "MarchConsumption",
                    "AprilConsumption",
                    "MayConsumption",
                    "JuneConsumption",
                    "JulyConsumption",
                    "AugustConsumption",
                    "SeptemberConsumption",
                    "OctoberConsumption",
                    "NovemberConsumption",
                    "DecemberConsumption",
                };

                var headerCols = ws.Cells[ws.Dimension.Start.Row, ws.Dimension.Start.Column, ws.Dimension.Start.Row, ws.Dimension.End.Column].ToList();

                //Check required columns
                foreach (var requiredCol in requiredCols)
                {
                    int index = headerCols.IndexOf(headerCols.FirstOrDefault(col => col.Text == requiredCol));
                    if (index < 0)
                    {
                        throw new Exception("Missing required column header: " + requiredCol);
                    }
                    columnMapping.Add(requiredCol, headerCols[index].Start.Column);
                }
                foreach (var optionalCol in optionalCols)
                {
                    int index = headerCols.IndexOf(headerCols.FirstOrDefault(col => col.Text == optionalCol));
                    if (index >= 0)
                    {
                        columnMapping.Add(optionalCol, headerCols[index].Start.Column);
                    }
                }

                //TODO Make sure all values are present in all reuqired fields.

                for (int i = ws.Dimension.Start.Row + 1; i <= ws.Dimension.End.Row; i++)
                {
                    UploadedLeadDto lead = new UploadedLeadDto();

                    foreach (var col in columnMapping.Keys)
                    {
                        PropertyInfo property = lead.GetType().GetProperties().Single(pi => pi.Name == col);
                        if (property != null)
                        {
                            var val = ws.Cells[i, columnMapping[col]].Value;
                            if (val == null)
                            {
                                continue;
                            }

                            if (property.PropertyType.IsAssignableFrom(val.GetType()))
                            {
                                property.SetValue(lead, val);
                            }
                            else
                            {
                                try
                                {
                                    val = val.ConvertTo(property.PropertyType);
                                    if (property.PropertyType.IsAssignableFrom(val.GetType()))
                                    {
                                        property.SetValue(lead, val);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //TODO log error some where that we couldn't handle the value
                                }
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(lead.Address)
                        || string.IsNullOrWhiteSpace(lead.City)
                        || string.IsNullOrWhiteSpace(lead.State)
                        || string.IsNullOrWhiteSpace(lead.ZipCode))
                    {
                        //todo:  log lead row failure
                    }
                    else
                    {
                        request.UploadedLeads.Add(lead);
                    }
                }
            }
            if (request.UploadedLeads.Count > MaxAmountOfHomes)
            {
                throw new ApplicationException($"You can only submit a max order of {MaxAmountOfHomes} homes.");
            }

            _service.CreateOrderForLeadEnhancement(request, addRoofAnalysis);
            return Ok();
        }


        // Order details
        [Route("orders/details/{orderID}/{pageIndex?}/{pageSize?}/{sortColummn?}/{sortOrder?}")]
        [ResponseType(typeof(PaginatedResult<LeadOrderDetailDto>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetOrderDetails(string orderID, int pageIndex = 0, int pageSize = 20, string sortColumn = "DateCreated", int sortOrder = 1)
        {
            var orderDetails = _service.GetOrderDetails(new Guid(orderID), SmartPrincipal.UserId, pageIndex, pageSize, sortColumn, sortOrder);

            orderDetails.Data.ForEach(od => od.DeSerializeDetails = true);

            var result = new PaginatedResult<LeadOrderDetailDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Total = orderDetails.Total,
                Data = orderDetails
                            .Data
                            .Where(od => od.Details != null)
                            .Select(od => od.Details)
                            .ToList()
            };

            return Ok(result);
        }

        // download order CSV
        [Route("orders/{orderID}/csv")]
        [HttpGet]
        public async Task<IHttpActionResult> GetOrderDetailsCSV(Guid orderID)
        {
            var data = _service.GetOrderDetails(orderID, SmartPrincipal.UserId, 0, 100000);

            var order = _service.Get(orderID, fields: "Id");

            data.Data.ForEach(od => od.DeSerializeDetails = true);
            var orderDetails = data
                                .Data
                                .Select(d => d.Details.ForCsv())
                                .ToList();
            var csv = orderDetails.ToCsv();

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv ?? ""));

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(stream.ToArray())
            };
            result.Headers.Add("Access-Control-Expose-Headers", "X-Filename");
            result.Headers.Add("X-Filename", $"Leads-{order.Id}.csv");

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return ResponseMessage(result);
        }

        private async Task<List<LeadOrderDetail>> ConvertOrderDetailsToDataViews(List<OrderDetail> orderDetails)
        {
            var leadOrderDetails = new List<LeadOrderDetail>();
            orderDetails.ForEach(od =>
            {
                JObject json = JObject.Parse(od.Json);
                LeadOrderDetail leadOrderDetail = new LeadOrderDetail();
                leadOrderDetail.Address = json["Address"]["Address"] != null ? json["Address"]["Address"].ToString() : "";
                leadOrderDetail.City = json["Address"]["City"] != null ? json["Address"]["City"].ToString() : "";
                leadOrderDetail.State = json["Address"]["State"] != null ? json["Address"]["State"].ToString() : "";
                leadOrderDetail.ZipCode = json["Address"]["ZipCode"] != null ? json["Address"]["ZipCode"].ToString() : "";
                try { leadOrderDetail.Latitude = json["Address"]["Latitude"] != null ? Convert.ToDouble(json["Address"]["Latitude"].ToString()) : 0; } catch { }
                try { leadOrderDetail.Longitude = json["Address"]["Longitude"] != null ? Convert.ToDouble(json["Address"]["Longitude"].ToString()) : 0; } catch { }
                leadOrderDetail.OrderDetailID = od.Guid;
                leadOrderDetail.Name = (json["OriginalPerson"]["FirstName"] != null ? json["OriginalPerson"]["FirstName"].ToString() : "") + " " + (json["OriginalPerson"]["LastName"] != null ? json["OriginalPerson"]["LastName"].ToString() : "");
                try { leadOrderDetail.Score = json["Financing"]["FinancialScore"] != null ? json["Financing"]["FinancialScore"].Value<int>() : 0; } catch { }
                leadOrderDetails.Add(leadOrderDetail);
            });
            return leadOrderDetails;
        }
        // Create CSV template and put it on AWS S3
    }
}