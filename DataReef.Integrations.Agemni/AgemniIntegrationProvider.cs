using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DataReef.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using DataReef.TM.Models;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Integrations.Core;
using System.Configuration;

namespace DataReef.Integrations.Agemni
{
	public class AgemniIntegrationProvider : IIntegrationProvider
	{
		/// <summary>
		/// Entity types as defined in Agemni
		/// </summary>
		private enum AgemniEntityType
		{
			/// <summary>
			/// Agemni entity type "Lead" (2)
			/// </summary>
			Lead = 2,

			/// <summary>
			/// Agemni entity type "Note" (5)
			/// </summary>
			Note = 5
		}

		public StringBuilder ErrorMessage
		{
			get;
			private set;
		}

		public StringBuilder DebugInfo
		{
			get;
			private set;
		}

		public AgemniIntegrationProvider()
		{
			this.ErrorMessage = new StringBuilder();
			this.DebugInfo = new StringBuilder();
		}

		// callback used to validate the certificate in an SSL conversation
		private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
		{

			return true;

			bool result = false;
			if (cert.Subject.ToLower().Contains("agemni.com"))
			{
				result = true;
			}

			return result;
		}

		private void LogDebugInfo(Exception ex, int nestingLevel = 0, int maxNestingLevel = 5)
		{
			if(ex != null)
			{
				if(nestingLevel < maxNestingLevel)
				{
					string errorType = ex.GetType().ToString();
					string errorText = ex.Message;
					string stackTrace = ex.StackTrace;

					this.DebugInfo.AppendLine(string.Format("Exception-type: {0}", errorType));
					this.DebugInfo.AppendLine(string.Format("Exception-message: {0}", errorText));
					this.DebugInfo.AppendLine(string.Format("Exception-stack-trace: {0}", stackTrace));

					if (ex.InnerException != null)
					{
						nestingLevel++;
						this.DebugInfo.AppendLine(string.Format("Inner-Exception ({0})", nestingLevel));
						this.LogDebugInfo(ex.InnerException, nestingLevel, maxNestingLevel);
					}
				}
			}
		}

		private string GenerateAgemniLeadNotes(IIntegrationRequest request)
		{
			StringBuilder sb = new StringBuilder();

			Dictionary<string, string> data = new Dictionary<string, string>();

			IntegrationData integrationData = (IntegrationData)request.IntegrationData;

			string[] nameParts = integrationData.PaymentInfo.Name.Replace("  ", " ").Replace("  ", " ").Split(' ');

			if (nameParts.Length > 0)
			{
				data["First Name"] = nameParts.First();
			}
			else
			{
				data["First Name"] = "Unknown";
			}

			if (nameParts.Length > 0)
			{
				data["Last Name"] = nameParts.Last();
			}
			else if (nameParts.Length == 3)
			{
				data["Last Name"] = "Unknown";
			}

			data["Phone"] = integrationData.PhoneNumber1.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
			data["Phone 2"] = integrationData.PhoneNumber2.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
			data["Street"] = integrationData.Property.Address1;
			data["City"] = integrationData.Property.City;
			data["State"] = integrationData.Property.State;
			data["Zip"] = integrationData.Property.ZipCode;
			data["Property Guid"] = integrationData.Property.ZipCode; integrationData.Property.Guid.ToString();

			if (integrationData.PaymentInfo != null && String.IsNullOrWhiteSpace(integrationData.PaymentInfo.SSN) == false)
			{
				data["SSN"] = integrationData.PaymentInfo.SSN.Replace(" ", string.Empty).Replace("-", string.Empty);
			}

			string newLine = Environment.NewLine;

			// start writing the e-mail body
			sb.AppendLine(newLine);

			foreach (KeyValuePair<string, string> kv in data)
			{
				sb.AppendLine(string.Format("{0}: {1}", kv.Key, kv.Value) + newLine);
			}

			sb.AppendLine(newLine);

			if (string.IsNullOrWhiteSpace(integrationData.PaymentInfo.BillingAddress) == false)
			{
				sb.AppendLine("--- BILLING ADDRESS DIFFERENT THAN INSTALL ADDRESS ---- " + newLine);
				if (string.IsNullOrWhiteSpace(integrationData.PaymentInfo.BillingAddress) == false)
				{
					sb.AppendLine(integrationData.PaymentInfo.BillingAddress + newLine);
				}

				if (string.IsNullOrWhiteSpace(integrationData.PaymentInfo.BillingAddress2) == false)
				{
					sb.AppendLine(integrationData.PaymentInfo.BillingAddress2 + newLine);
				}

				if (string.IsNullOrWhiteSpace(integrationData.PaymentInfo.BillingCity) == false)
				{
					sb.AppendLine(integrationData.PaymentInfo.BillingCity + newLine);
				}

				if (string.IsNullOrWhiteSpace(integrationData.PaymentInfo.BillingState) == false)
				{
					sb.AppendLine(integrationData.PaymentInfo.BillingState + newLine);
				}

				if (string.IsNullOrWhiteSpace(integrationData.PaymentInfo.BillingZip) == false)
				{
					sb.AppendLine(integrationData.PaymentInfo.BillingZip + newLine);
				}

				sb.AppendLine(newLine);
			}

			if (request.IntegratorName != null)
			{
				sb.AppendLine(string.Format("Sales Person: {0}", request.IntegratorName) + newLine);
			}

			sb.AppendLine(string.Format("Office: {0}", request.OUName) + newLine);
			sb.AppendLine(string.Format("Language: {0}", integrationData.Language) + newLine);
			sb.AppendLine(string.Format("Base Package: {0}", integrationData.BasePackage) + newLine);
			sb.AppendLine(string.Format("TV Count: {0}", integrationData.TVCount.ToString()) + newLine);

			if (integrationData.MiniReceiverCount > 0)
			{
				sb.AppendLine(string.Format("Mini Receiver Count: {0}", integrationData.MiniReceiverCount.ToString()) + newLine);
			}

			if (integrationData.WirelessMiniReceiverCount > 0)
			{
				sb.AppendLine(string.Format("Wireless Mini Receiver Count: {0}", integrationData.WirelessMiniReceiverCount.ToString()) + newLine);
			}

			if (integrationData.GenieReceiverCount > 0)
			{
				sb.AppendLine(string.Format("Genie Receivers: {0}", integrationData.GenieReceiverCount.ToString()) + newLine);
			}

			if (integrationData.DVRReceiverCount > 0)
			{
				sb.AppendLine(string.Format("DVR Receivers: {0}", integrationData.DVRReceiverCount.ToString()) + newLine);
			}

			if (integrationData.HDDVRReceiverCount > 0)
			{
				sb.AppendLine(string.Format("HD DVR Receivers: {0}", integrationData.HDDVRReceiverCount.ToString()) + newLine);
			}

			if (integrationData.HDReceiverCount > 0)
			{
				sb.AppendLine(string.Format("HD Receivers: {0}", integrationData.HDReceiverCount.ToString()) + newLine);
			}

			if (integrationData.StandardReceiverCount > 0)
			{
				sb.AppendLine(string.Format("Standard Receivers: {0}", integrationData.StandardReceiverCount.ToString()) + newLine);
			}

			sb.AppendLine(newLine);

			if (integrationData.DirecTVAddOns != null && integrationData.DirecTVAddOns.Any())
			{
				sb.AppendLine("DirecTV Add-ons:" + newLine);
				foreach (DirecTVAddOn addon in integrationData.DirecTVAddOns)
				{
					sb.AppendLine(addon.ToString() + newLine);
				}

				sb.AppendLine(newLine);
			}

			if (integrationData.PremiumAddOns != null && integrationData.PremiumAddOns.Any())
			{
				sb.AppendLine("Premium Add-ons:" + newLine);
				foreach (PremiumAddOn pao in integrationData.PremiumAddOns)
				{
					sb.AppendLine(pao.ToString() + newLine);
				}

				sb.AppendLine(newLine);
			}

			if (integrationData.InternationalPrograms != null && integrationData.InternationalPrograms.Any())
			{
				sb.AppendLine("International Programs:" + newLine);
				foreach (InternationalProgram ip in integrationData.InternationalPrograms)
				{
					sb.AppendLine(ip.ToString() + newLine);
				}

				sb.AppendLine(newLine);
			}

			if (integrationData.SportsPackages != null && integrationData.SportsPackages.Any())
			{
				sb.AppendLine("Sports Packages:" + newLine);
				foreach (SportsPackage sp in integrationData.SportsPackages)
				{
					sb.AppendLine(string.Format("Sports Package: {0}", sp.ToString()) + newLine);
				}

				sb.AppendLine(newLine);
			}

			if (integrationData.SpanishAddOns != null && integrationData.SpanishAddOns.Any())
			{
				sb.AppendLine("Spanish Addons:" + newLine);
				foreach (SpanishAddOn sao in integrationData.SpanishAddOns)
				{
					sb.AppendLine(sao.ToString() + newLine);
				}

				sb.AppendLine(newLine);
			}

			if (integrationData.OtherEquipment != null && integrationData.OtherEquipment.Any())
			{
				sb.AppendLine("Other Equipment:" + newLine);
				foreach (OtherEquipment oe in integrationData.OtherEquipment)
				{
					sb.AppendLine(string.Format(
						"{0},  Qty:{1}  Is Comp'd:{2}", 
						oe.ToString(),
						oe.Quantity,
						oe.IsComplimentary ? "Yes" : "No") + newLine);
				}

				sb.AppendLine(newLine);
			}

			sb.AppendLine("Payment Info:" + newLine);
			sb.AppendLine(string.Format("Name: {0}", integrationData.PaymentInfo.Name) + newLine);
			sb.AppendLine(string.Format("Address: {0}", integrationData.PaymentInfo.Address) + newLine);
			sb.AppendLine(string.Format("City: {0}", integrationData.PaymentInfo.City) + newLine);
			sb.AppendLine(string.Format("State: {0}", integrationData.PaymentInfo.State) + newLine);
			sb.AppendLine(string.Format("Zip: {0}", integrationData.PaymentInfo.Zip) + newLine);
			sb.AppendLine(string.Format("Credit Card No: {0}", integrationData.PaymentInfo.CreditCardNumber) + newLine);
			sb.AppendLine(string.Format("Expiration: {0}/{1}", integrationData.PaymentInfo.ExpirationMonth, integrationData.PaymentInfo.ExpirationYear) + newLine);
			sb.AppendLine(string.Format("CCV: {0}", integrationData.PaymentInfo.CCV.HasValue ? integrationData.PaymentInfo.CCV.Value.ToString() : "") + newLine);
			sb.AppendLine(string.Format("AutoPay: {0}", integrationData.PaymentInfo.AcceptsAutoPay ? "YES" : "NO") + newLine);

			if (!string.IsNullOrWhiteSpace(integrationData.PaymentInfo.EmailAddress))
			{
				sb.AppendLine(string.Format("Email: {0}", integrationData.PaymentInfo.EmailAddress.ToString()) + newLine);
			}

			if (!string.IsNullOrWhiteSpace(integrationData.PaymentInfo.SSN))
			{
				sb.AppendLine(string.Format("SSN: {0}", integrationData.PaymentInfo.SSN.ToString()) + newLine);
			}

			sb.AppendLine(string.Format("Processing Fee: {0}", integrationData.ProcessingFee.ToString("c")) + newLine);
			sb.AppendLine(string.Format("Equipment and Processing Fee: {0}", integrationData.EquipmentAndProcessingFee.ToString("c")) + newLine);

			sb.AppendLine(newLine);

			if (integrationData.ProtectionPlan != null)
			{
				sb.AppendLine(string.Format("Protection Plan: {0}", integrationData.ProtectionPlan.ToString()) + newLine);
			}

			sb.AppendLine(newLine);
			sb.AppendLine(newLine);

			return sb.ToString();
		}

		private DataReef.TM.Classes.SalesOrderConfirmationTemplate CreateSalesOrderConfirmationTemplate(IIntegrationRequest request)
		{
			DataReef.TM.Classes.SalesOrderConfirmationTemplate result = new TM.Classes.SalesOrderConfirmationTemplate();

			IntegrationData integrationData = (IntegrationData)request.IntegrationData;

			string[] nameParts = integrationData.PaymentInfo.Name.Replace("  ", " ").Replace("  ", " ").Split(' ');

			if (nameParts.Length > 0)
			{
				result.FirstName = nameParts.First();
			}
			else
			{
				result.FirstName = "Unknown";
			}

			if (nameParts.Length > 0)
			{
				result.LastName = nameParts.Last();
			}
			else if (nameParts.Length == 3)
			{
				result.LastName = "Unknown";
			}

			result.Phone = integrationData.PhoneNumber1.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
			result.Phone2 = integrationData.PhoneNumber2.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
			result.Street = integrationData.Property.Address1;
			result.City = integrationData.Property.City;
			result.State = integrationData.Property.State;
			result.Zip = integrationData.Property.ZipCode;

			if (!string.IsNullOrWhiteSpace(integrationData.PaymentInfo.SSN))
			{
				result.SSN = integrationData.PaymentInfo.SSN.Replace(" ", string.Empty).Replace("-", string.Empty);
			}

			string billingStreetName = integrationData.PaymentInfo.BillingAddress;
			if (string.IsNullOrEmpty(billingStreetName))
			{
				billingStreetName = result.Street;
			}
			else
			{
				result.BillingAddress = billingStreetName;
			}

			string billingCity = integrationData.PaymentInfo.BillingCity;
			if (string.IsNullOrEmpty(billingCity))
			{
				billingCity = result.City;
			}
			else
			{
				result.BillingCity = billingCity;
			}

			string billingStateName = integrationData.PaymentInfo.BillingState;
			if (string.IsNullOrEmpty(billingStateName))
			{
				billingStateName = result.State;
			}
			else
			{
				result.BillingState = billingStateName;
			}

			string billingZipCode = integrationData.PaymentInfo.BillingZip;
			if (string.IsNullOrEmpty(billingZipCode))
			{
				billingZipCode = result.Zip;
			}
			else
			{
				result.BillingZip = billingZipCode;
			}

			result.IsBillingAddressDifferentThanInstallAddress = !(
				string.Equals(result.Street, billingStreetName, StringComparison.OrdinalIgnoreCase) &&
				string.Equals(result.City, billingCity, StringComparison.OrdinalIgnoreCase) &&
				string.Equals(result.State, billingStateName, StringComparison.OrdinalIgnoreCase) &&
				string.Equals(result.Zip, billingZipCode, StringComparison.OrdinalIgnoreCase));

			if (!string.IsNullOrWhiteSpace(request.IntegratorName))
			{
				result.SalesPerson = request.IntegratorName;
			}

			result.Office = request.OUName;
			result.Language = integrationData.Language;
			
			if(integrationData.BasePackage != null)
			{
				result.BasePackage = integrationData.BasePackage.ToString();
			}

			if (integrationData.ProtectionPlan != null)
			{
				result.ProtectionPlan = integrationData.ProtectionPlan.ToString();
			}

			result.TVCount = integrationData.TVCount;
			result.MiniReceiverCount = integrationData.MiniReceiverCount;
			result.WirelessMiniReceiverCount = integrationData.WirelessMiniReceiverCount;
			result.GenieReceiverCount = integrationData.GenieReceiverCount;
			result.DVRReceiverCount = integrationData.DVRReceiverCount;
			result.HDDVRReceiverCount = integrationData.HDDVRReceiverCount;
			result.HDReceiverCount = integrationData.HDReceiverCount;
			result.StandardReceiverCount = integrationData.StandardReceiverCount;

			if (integrationData.DirecTVAddOns != null && integrationData.DirecTVAddOns.Any())
			{
				result.DTVAddons = integrationData.DirecTVAddOns.Select(w => w.ToString()).ToArray();
			}

			if (integrationData.PremiumAddOns != null && integrationData.PremiumAddOns.Any())
			{
				result.PremiumAddOns = integrationData.PremiumAddOns.Select(w => w.ToString()).ToArray();
			}

			if (integrationData.InternationalPrograms != null && integrationData.InternationalPrograms.Any())
			{
				result.InternationalPrograms = integrationData.InternationalPrograms.Select(w => w.ToString()).ToArray();
			}

			if (integrationData.SportsPackages != null && integrationData.SportsPackages.Any())
			{
				result.SportsPackages = integrationData.SportsPackages.Select(w => w.ToString()).ToArray();
			}

			if (integrationData.SpanishAddOns != null && integrationData.SpanishAddOns.Any())
			{
				result.SpanishAddOns = integrationData.SpanishAddOns.Select(w => w.ToString()).ToArray();
			}

			if (integrationData.OtherEquipment != null && integrationData.OtherEquipment.Any())
			{
				result.OtherEquipments = integrationData.OtherEquipment.Select(w => string.Format(
					"{0},  Qty:{1}  Is Comp'd:{2}",
					w.ToString(),
					w.Quantity,
					w.IsComplimentary ? "Yes" : "No")).ToArray();
			}

			result.PaymentName = integrationData.PaymentInfo.Name;
			result.PaymentAddress = integrationData.PaymentInfo.Address;
			result.PaymentCity = integrationData.PaymentInfo.City;
			result.PaymentState = integrationData.PaymentInfo.State;
			result.PaymentZip = integrationData.PaymentInfo.Zip;
			result.AcceptsAutoPay = integrationData.PaymentInfo.AcceptsAutoPay ? "YES" : "NO";

			if (!string.IsNullOrWhiteSpace(integrationData.PaymentInfo.EmailAddress))
			{
				result.PaymentEmailAddress = integrationData.PaymentInfo.EmailAddress;
			}

			result.ProcessingFee = integrationData.ProcessingFee.ToString("c");
			result.EquipmentAndProcessingFee = integrationData.EquipmentAndProcessingFee.ToString("c");
			result.MonthlyFeeProgramming = integrationData.MonthlyFeeProgramming.ToString("c");
			result.MonthlyFeeEquipment = integrationData.MonthlyFeeEquipment.ToString("c");
			result.MonthlyFeeTotal = integrationData.MonthlyFeeTotal.ToString("c");

			return result;
		}

		private bool CreateOrUpdateLeadInAgemni(IIntegrationRequest request)
		{
			string userName = System.Configuration.ConfigurationManager.AppSettings["AgemniUserName"];
			string password = System.Configuration.ConfigurationManager.AppSettings["AgemniPassword"];
			string companyName = System.Configuration.ConfigurationManager.AppSettings["AgemniCompanyName"];

			IntegrationData integrationData = (IntegrationData)request.IntegrationData;

			Guid userID = SmartPrincipal.UserId;


			//PersonProxy pproxy = new PersonProxy();
			//Person rep = pproxy.Get(0, request.PersonID);

			//OUProxy ouproxy = new OUProxy();
			//OU ou = null;
			//try
			//{
			//    ou = ouproxy.Get(0, request.OUID);
			//}
			//catch (Exception)
			//{
			//    Mailer.SendMail("jason@datareef.com", "jason@datareef.com", "Agemni Lead Engine", "Empty OUID Error", request.IntegrationData);
			//    return; ;
			//}


			//if (ou == null)
			//{
			//    Mailer.SendMail("jason@datareef.com", "jason@datareef.com", "Agemni Lead Engine", "Empty OUID Error", request.IntegrationData);
			//    return; ;
			//}


			//first update the account
			List<string> keys = new List<string>();
			List<object> values = new List<object>();

			#region First Name
			keys.Add("FName");
			string[] nameParts = integrationData.PaymentInfo.Name.Replace("  ", " ").Replace("  ", " ").Split(' ');
			string firstName = "UnknownFirst";
			if (nameParts.Length > 0)
			{
				firstName = nameParts.First();
			}

			values.Add(firstName);
			#endregion

			#region Last Name
			keys.Add("LName");
			string lastName = "UnkownLast";
			if (nameParts.Length > 0)
			{
				lastName = nameParts.Last();
			}

			values.Add(lastName);
			#endregion

			#region Middle Name
			string middleName = "UnknownMiddle";
			if (nameParts.Length == 3)
			{
				middleName = nameParts[1];
			}
			#endregion

			#region Phone Number 2
			string phoneNo2 = integrationData.PhoneNumber2.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
			keys.Add("Phone2");
			values.Add(phoneNo2);
			#endregion

			#region Area ID
			keys.Add("Area ID");
			values.Add("1");
			#endregion

			#region Lead ID
			keys.Add("Lead ID");
			values.Add("119"); //clear
			#endregion

			#region Street
			string streetName = integrationData.Property.Address1;
			keys.Add("Street");
			values.Add(streetName);
			#endregion

			#region City
			string cityName = integrationData.Property.City;
			keys.Add("City");
			values.Add(cityName);
			#endregion

			#region State
			string stateName = integrationData.Property.State;
			keys.Add("State");
			values.Add(stateName);
			#endregion

			#region Zip
			string zipCode = integrationData.Property.ZipCode;
			keys.Add("Zip");
			values.Add(zipCode);
			#endregion

			#region Custom3
			keys.Add("Custom3");
			values.Add(integrationData.Property.Guid.ToString());
			#endregion

			#region SSN
			string ssn = "";
			if (integrationData.PaymentInfo != null && String.IsNullOrWhiteSpace(integrationData.PaymentInfo.SSN) == false)
			{
				ssn = integrationData.PaymentInfo.SSN.Replace(" ", string.Empty).Replace("-", string.Empty);
				keys.Add("SSN");
				values.Add(ssn);
			}
			#endregion

			//Trust all certificates
			System.Net.ServicePointManager.ServerCertificateValidationCallback =
				((sender, certificate, chain, sslPolicyErrors) => true);

			// trust sender
			System.Net.ServicePointManager.ServerCertificateValidationCallback
							= ((sender, cert, chain, errors) => cert.Subject.Contains("agemni.com"));

			// validate cert by calling a function
			ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
			com.agemni.www.Service1 sv = new com.agemni.www.Service1();
			com.agemni.www.ExceptionReport rpt;

			int leadID;
			string phoneNo = integrationData.PhoneNumber1.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
			string strExistingLeadID = sv.GetLeadIDFromPhone(userName, password, companyName, phoneNo);
			if(int.TryParse(strExistingLeadID, out leadID))
			{
				#region Lead ID
				keys.Add("id");
				values.Add(leadID);
				#endregion

				rpt = (com.agemni.www.ExceptionReport)sv.UpdateEntity(userName, password, companyName, (int)AgemniEntityType.Lead, keys.ToArray(), values.ToArray());
			}
			else
			{
				#region Contact Date
				keys.Add("ContactDate");
				values.Add(System.DateTime.Today.ToShortDateString());
				#endregion

				#region Phone Number
				keys.Add("Phone");
				values.Add(phoneNo);
				#endregion

				rpt = (com.agemni.www.ExceptionReport)sv.CreateEntity(userName, password, companyName, (int)AgemniEntityType.Lead, keys.ToArray(), values.ToArray());
				if (rpt.statusCode == com.agemni.www.statusCodes.Succeeded)
				{
					leadID = rpt.EntityIDCreated;
				}
			}

			if (rpt.statusCode == com.agemni.www.statusCodes.Succeeded)
			{
				this.ErrorMessage.Append("Agemni succeeded");

				keys = new List<string>();
				values = new List<object>();

				#region Lead ID
				keys.Add("Lead ID");
				values.Add(leadID);
				#endregion

				#region Type (Status)
				keys.Add("Type");
				values.Add("Status");
				#endregion

				StringBuilder sb = new StringBuilder();

				#region Comment
				string comment = this.GenerateAgemniLeadNotes((IntegrationRequest)request).Replace("'", "''");
				keys.Add("Comment");
				values.Add(comment);
				#endregion

				rpt = (com.agemni.www.ExceptionReport)sv.CreateEntity(userName, password, companyName, (int)AgemniEntityType.Note, keys.ToArray(), values.ToArray());
			}
			else
			{
				this.ErrorMessage.Append("Agemni failed");
				if (!string.IsNullOrEmpty(rpt.description))
				{
					this.ErrorMessage.Append("(error: " + rpt.description + ")");
				}

				return false;
			}

			return true;
		}

		private bool CreateDTVSalesOrder(IIntegrationRequest request)
		{
			IntegrationData integrationData = (IntegrationData)request.IntegrationData;

			#region First Name
			string[] nameParts = integrationData.PaymentInfo.Name.Replace("  ", " ").Replace("  ", " ").Split(' ');
			string firstName = "UnknownFirst";
			if (nameParts.Length > 0)
			{
				firstName = nameParts.First();
			}
			#endregion

			#region Last Name
			string lastName = "UnkownLast";
			if (nameParts.Length > 0)
			{
				lastName = nameParts.Last();
			}
			#endregion

			#region Middle Name
			string middleName = "UnknownMiddle";
			if (nameParts.Length == 3)
			{
				middleName = nameParts[1];
			}
			#endregion

			#region Phone Number
			string phoneNo = integrationData.PhoneNumber1.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
			#endregion

			#region Phone Number 2
			string phoneNo2 = integrationData.PhoneNumber2.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
			#endregion

			#region Street
			string streetName = integrationData.Property.Address1;
			#endregion

			#region City
			string cityName = integrationData.Property.City;
			#endregion

			#region State
			string stateName = integrationData.Property.State;
			#endregion

			#region Zip
			string zipCode = integrationData.Property.ZipCode;
			#endregion

			#region SSN
			string ssn = "";
			if (integrationData.PaymentInfo != null && String.IsNullOrWhiteSpace(integrationData.PaymentInfo.SSN) == false)
			{
				ssn = integrationData.PaymentInfo.SSN.Replace(" ", string.Empty).Replace("-", string.Empty);
			}
			#endregion

			#region Comment
			string comment = string.Empty;	// it makes no sense to put the whole Agemni Lead Notes as a comment, 
											// anyway the Comments field on DTV site doesn't allow such length
											// this.GenerateAgemniLeadNotes((IntegrationRequest)request).Replace("'", "''");
			#endregion

			#region Billing Address
			string billingStreetName = integrationData.PaymentInfo.BillingAddress;
			if (string.IsNullOrEmpty(billingStreetName))
			{
				billingStreetName = streetName;
			}

			string billingCity = integrationData.PaymentInfo.BillingCity;
			if (string.IsNullOrEmpty(billingCity))
			{
				billingCity = cityName;
			}

			string billingStateName = integrationData.PaymentInfo.BillingState;
			if (string.IsNullOrEmpty(billingStateName))
			{
				billingStateName = stateName;
			}

			string billingZipCode = integrationData.PaymentInfo.BillingZip;
			if (string.IsNullOrEmpty(billingZipCode))
			{
				billingZipCode = zipCode;
			}

			bool sameAsInstallAddress = string.Equals(streetName, billingStreetName, StringComparison.OrdinalIgnoreCase) &&
											string.Equals(cityName, billingCity, StringComparison.OrdinalIgnoreCase) &&
											string.Equals(stateName, billingStateName, StringComparison.OrdinalIgnoreCase) &&
											string.Equals(zipCode, billingZipCode, StringComparison.OrdinalIgnoreCase);
			#endregion

			#region Contact Info
			string emailAddress = integrationData.PaymentInfo.EmailAddress;
			#endregion

			if (this.ErrorMessage.Length > 0)
			{
				this.ErrorMessage.Append(", ");
			}

			DataReef.Integrations.Agemni.DirecTVAutomation.PlaceSalesOrderResult result = null;
			using (DirecTVAutomation.DtvAutomationServiceClient service = new DirecTVAutomation.DtvAutomationServiceClient())
			{
				try
				{
					// force timeout to be 10 mins, for some reasons looks like the configured (web.config) setting is not listened
					service.Endpoint.Binding.SendTimeout =
						service.Endpoint.Binding.ReceiveTimeout = new TimeSpan(0, 10, 0);

					// call DTV
					result = service.PlaceSalesOrder(new DirecTVAutomation.SalesOrder()
					{
						StoreInfo = new DirecTVAutomation.StoreInfo()
						{
							StoreName = System.Configuration.ConfigurationManager.AppSettings["DirecTVStoreName"],
							StoreNumber = System.Configuration.ConfigurationManager.AppSettings["DirecTVStoreNumber"],
						},
						CustomerInfo = new DirecTVAutomation.CustomerInfo()
						{
							FirstName = firstName,
							LastName = lastName,
							MiddleName = middleName,
						},
						InstallAddress = new DirecTVAutomation.InstallAddress()
						{
							StreetName = streetName,
							City = cityName,
							State = stateName,
							ZipCode = zipCode
						},
						BillingAddress = new DirecTVAutomation.BillingAddress()
						{
							SameAsInstallAddress = sameAsInstallAddress,
							StreetName = billingStreetName,
							City = billingCity,
							State = billingStateName,
							ZipCode = billingZipCode
						},
						ContactInfo = new DirecTVAutomation.CustomerContactInfo()
						{
							PhoneNumber = phoneNo,
							AlternatePhoneNumber = phoneNo2,
							Email = emailAddress,
						},
						CreditConsent = new DirecTVAutomation.CreditConsent()
						{
							SSN = ssn,
						},
						Comments = comment
					});
				}
				catch (System.ServiceModel.CommunicationException e)
				{
					service.Abort();
					this.LogDebugInfo(e);
				}
				catch (TimeoutException e)
				{
					service.Abort();
					this.LogDebugInfo(e);
				}
				catch (Exception e)
				{
					service.Abort();
					this.LogDebugInfo(e);
					throw;
				}
			}
			
			if((result != null) && (result.Success))
			{
				this.ErrorMessage.Append("Retailer succeeded");
				return true;
			}
			else
			{
				this.ErrorMessage.Append("Retailer failed");
				if ((result != null) && (!string.IsNullOrEmpty(result.ErrorMessage)))
				{
					this.ErrorMessage.Append("(error: " + result.ErrorMessage + ")");
				}

				return false;
			}
		}

		private bool SendEMail(IIntegrationRequest request)
		{
			bool success = true;

			if (this.ErrorMessage.Length > 0)
			{
				this.ErrorMessage.Append(", ");
			}

			try
			{
				var template = this.CreateSalesOrderConfirmationTemplate(request);

				if((template != null) && (!string.IsNullOrWhiteSpace(template.PaymentEmailAddress)))
				{
					DataReef.TM.Mail.Library.SendSalesOrderConfirmationEmail(
						template: template,
						from: ConfigurationManager.AppSettings["DTV-SalesOrder-EMail-Sender-EMail"],
						fromName: ConfigurationManager.AppSettings["DTV-SalesOrder-EMail-Sender-Name"],
						subject: ConfigurationManager.AppSettings["DTV-SalesOrder-EMail-Subject"]
					);

					this.ErrorMessage.Append("Sending mail to customer succeeded");
				}
				else
				{
					this.ErrorMessage.Append("Sending mail to customer failed(error: e-mail address not specified)");

					success = false;
				}
				
			}
			catch(Exception e)
			{
				success = false;
				this.LogDebugInfo(e);
				this.ErrorMessage.Append("Sending mail to customer failed");
			}

			return success;
		}

		public bool Integrate(IIntegrationRequest request)
		{
			bool agemniOK = this.CreateOrUpdateLeadInAgemni(request);

			bool dtvOK = this.CreateDTVSalesOrder(request);

			if (agemniOK && dtvOK)
			{
				bool mailOK = this.SendEMail(request);
			}
			
			return agemniOK && dtvOK;
		}
	}
}
