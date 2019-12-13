using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Classes
{
	public class SalesOrderConfirmationTemplate
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Phone { get; set; }

		public string Phone2 { get; set; }

		public string Street { get; set; }

		public string City { get; set; }

		public string State { get; set; }

		public string Zip { get; set; }

		public string SSN { get; set; }

		public bool IsBillingAddressDifferentThanInstallAddress { get; set; }

		public string BillingAddress { get; set; }

		public string BillingAddress2 { get; set; }

		public string BillingCity { get; set; }

		public string BillingState { get; set; }

		public string BillingZip { get; set; }

		public string SalesPerson { get; set; }

		public string Office { get; set; }

		public string Language { get; set; }

		public string BasePackage { get; set; }

		public int TVCount { get; set; }

		public int MiniReceiverCount { get; set; }

		public int WirelessMiniReceiverCount { get; set; }

		public int GenieReceiverCount { get; set; }

		public int DVRReceiverCount { get; set; }

		public int HDDVRReceiverCount { get; set; }

		public int HDReceiverCount { get; set; }

		public int StandardReceiverCount { get; set; }

		public string[] DTVAddons { get; set; }

		public string[] PremiumAddOns { get; set; }

		public string[] InternationalPrograms { get; set; }

		public string[] SportsPackages { get; set; }

		public string[] SpanishAddOns { get; set; }

		public string[] OtherEquipments { get; set; }

		public string PaymentName { get; set; }

		public string PaymentAddress{ get; set; }

		public string PaymentCity { get; set; }

		public string PaymentState { get; set; }

		public string PaymentZip { get; set; }

		public string AcceptsAutoPay { get; set; }

		public string PaymentEmailAddress { get; set; }

		public string ProcessingFee { get; set; }

		public string EquipmentAndProcessingFee { get; set; }

		public string ProtectionPlan { get; set; }

		public string MonthlyFeeProgramming { get; set; }

		public string MonthlyFeeEquipment { get; set; }

		public string MonthlyFeeTotal { get; set; }
	}
}
