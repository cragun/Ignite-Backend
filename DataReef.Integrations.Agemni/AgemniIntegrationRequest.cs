using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DataReef.TM.Models;
using DataReef.Integrations.Core;
using DataReef.Core.Attributes;

namespace DataReef.Integrations.Agemni
{
    public class IntegrationRequest : IIntegrationRequest
    {
        public IntegrationRequest()
        {
            this.IntegrationData = new IntegrationData();

        }

        public Guid PersonID { get; set; }
        public string IntegratorName { get; set; }
        public string DeviceID { get; set; }
        public Guid OUID { get; set; }
        public string OUName { get; set; }
        public string RequestType { get; set; }
        [PropertyDescriptor(typeof(IntegrationData))]
        public IIntegrationData IntegrationData { get; set; }

    }


    public class IntegrationData : IIntegrationData
    {
        public IntegrationData()
        {
            ProcessingFee = 49.99M;
            MonthlyFeeProgramming = 0M;
            MonthlyFeeEquipment = 0M;
        }

        #region Properties

        public string Language { get; set; }

        [Required(ErrorMessage = "Base Package is required")]
        public BasePackage BasePackage { get; set; }

        public ProtectionPackage ProtectionPlan { get; set; }
        public Property Property { get; set; }
        public PaymentInfo PaymentInfo { get; set; }



        [DisplayName("PRIMARY PHONE")]
        [Required(ErrorMessage = "Primary Phone is required")]
        public string PhoneNumber1 { get; set; }

        [DisplayName("SECONDARY PHONE")]
        [Required(ErrorMessage = "Secondary Phone is required")]
        public string PhoneNumber2 { get; set; }

        [DisplayName("HOW MANY TVs?")]
        public int TVCount { get; set; }

        [DisplayName("MINI RECEIVERS")]
        public int MiniReceiverCount { get; set; }

        [DisplayName("WIRELESS MINI RECEIVERS")]
        public int WirelessMiniReceiverCount { get; set; }

        [DisplayName("STANDARD RECEIVERS")]
        public int StandardReceiverCount { get; set; }

        [DisplayName("HD RECEIVERS")]
        public int HDReceiverCount { get; set; }

        [DisplayName("DVR RECEIVERS")]
        public int DVRReceiverCount { get; set; }

        [DisplayName("HD-DVR RECEIVERS")]
        public int HDDVRReceiverCount { get; set; }

        [DisplayName("GENIE")]
        public int GenieReceiverCount { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal ProcessingFee { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal EquipmentAndProcessingFee { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal MonthlyFeeProgramming { get; set; }

        public decimal MonthlyFeeEquipment { get; set; }

        public decimal MonthlyFeeTotal
        {
            get
            {
                return MonthlyFeeProgramming + MonthlyFeeEquipment;
            }
        }


        #endregion

        #region Collections

        public List<PremiumAddOn> PremiumAddOns { get; set; }
        public List<DirecTVAddOn> DirecTVAddOns { get; set; }
        public List<SportsPackage> SportsPackages { get; set; }
        public List<InternationalProgram> InternationalPrograms { get; set; }
        public List<SpanishAddOn> SpanishAddOns { get; set; }
        public List<OtherEquipment> OtherEquipment { get; set; }

        #endregion

    }

    public class PaymentInfo
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [DisplayName("Email (Req'd for Auto-pay)")]
        [StringLength(255)]
        public string EmailAddress { get; set; }

        [DisplayName("SSN")]
        [StringLength(11)]
        public string SSN { get; set; }

        [DisplayName("Address")]
        [Required(ErrorMessage = "Address is required")]
        [StringLength(200)]
        public string Address { get; set; }

        [DisplayName("Address 2")]
        [StringLength(200)]
        public string Address2 { get; set; }

        [DisplayName("City")]
        [Required(ErrorMessage = "City is required")]
        [StringLength(150)]
        public string City { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "State is required")]
        [StringLength(2)]
        public string State { get; set; }

        [Required(ErrorMessage = "Zip is required")]
        [StringLength(100)]
        public string Zip { get; set; }

        [DisplayName("Credit Card Number")]
        [Required(ErrorMessage = "Credit Card Number is required")]
        [RegularExpression("[0-9]{15,16}", ErrorMessage = "Credit Card Number should contain only numeric characters")]
        public string CreditCardNumber { get; set; }

        [DisplayName("CCV")]
        [Required]
        [RegularExpression("[0-9]{3}", ErrorMessage = "CCV should contain only numeric characters")]
        public int? CCV { get; set; }

        [DisplayName("Expiration Month")]
        public int ExpirationMonth { get; set; }

        [DisplayName("Expiration Year")]
        public int ExpirationYear { get; set; }

        [DisplayName("Signature")]
        [JsonIgnore]
        public string Signature { get; set; }

        [DisplayName("Billing Address")]
        [StringLength(200)]
        public string BillingAddress { get; set; }

        [DisplayName("Billing Address 2")]
        [StringLength(200)]
        public string BillingAddress2 { get; set; }

        [DisplayName("Billing City")]
        [StringLength(150)]
        public string BillingCity { get; set; }

        [DisplayName("Billing State")]
        [StringLength(2)]
        public string BillingState { get; set; }

        [DisplayName("Billing Zip")]
        [StringLength(100)]
        public string BillingZip { get; set; }

        public bool AcceptsAutoPay { get; set; }
    }

    public class PremiumAddOn
    {
        public string Value { get; set; }
        private string text;
        public string Text
        {
            get
            {
				string packagePrice = (this.Price - this.Discount).ToString("C") + "/mo";
				if (!string.IsNullOrEmpty(text))
				{
					// make sure we don't double the pricing info
					text = text.Replace(packagePrice, "");
				}

				return text + " " + packagePrice;
            }
            set
            {
                text = value;
            }

        }
        public Decimal Price { get; set; }
        public Decimal Discount { get; set; }
        public static List<PremiumAddOn> AvailableAddOns
        {
            get
            {
                List<PremiumAddOn> list = new List<PremiumAddOn>();
                list.Add(new PremiumAddOn { Text = "HBO", Value = "HBO", Price = new Decimal(17.99), Discount = new Decimal(0.00) });
                list.Add(new PremiumAddOn { Text = "Starz", Value = "Starz", Price = new Decimal(13.99), Discount = new Decimal(0.00) });
                list.Add(new PremiumAddOn { Text = "Showtime", Value = "Showtime", Price = new Decimal(13.99), Discount = new Decimal(0.00) });
                list.Add(new PremiumAddOn { Text = "Cinemax", Value = "Cinemax", Price = new Decimal(13.99), Discount = new Decimal(0.00) });
                list.Add(new PremiumAddOn { Text = "Sports Pack", Value = "SportsPack", Price = new Decimal(13.99), Discount = new Decimal(0.00) });
                list.Add(new PremiumAddOn { Text = "Outdoor Channel", Value = "OutdoorChannel", Price = new Decimal(3.50), Discount = new Decimal(0.00) });

                return list;
            }
        }

        public override string ToString()
        {
            return this.Text;
        }
    }

    public class DirecTVAddOn
    {
        public string Value { get; set; }
        private string text;
        public string Text
        {
            get
            {
				string packagePrice = (this.Price - this.Discount).ToString("C") + "/mo";
				if (!string.IsNullOrEmpty(text))
				{
					// make sure we don't double the pricing info
					text = text.Replace(packagePrice, "");
				}

				return text + " " + packagePrice;
            }
            set
            {
                text = value;
            }
        }
        public Decimal Price { get; set; }
        public Decimal Discount { get; set; }
        public static List<DirecTVAddOn> AvailableAddOns
        {
            get
            {
                List<DirecTVAddOn> list = new List<DirecTVAddOn>();
                list.Add(new DirecTVAddOn { Text = "Playboy TV", Value = "Playboy", Price = new Decimal(15.99), Discount = new Decimal(0.00) });
                list.Add(new DirecTVAddOn { Text = "VividTV", Value = "Vivid", Price = new Decimal(29.99), Discount = new Decimal(0.00) });
                list.Add(new DirecTVAddOn { Text = "Hustler TV", Value = "Hustler", Price = new Decimal(39.99), Discount = new Decimal(0.00) });
                list.Add(new DirecTVAddOn { Text = "DOG TV", Value = "DOG", Price = new Decimal(4.99), Discount = new Decimal(0.00) });
                list.Add(new DirecTVAddOn { Text = "Penthouse TV", Value = "Penthouse", Price = new Decimal(49.99), Discount = new Decimal(0.00) });
                list.Add(new DirecTVAddOn { Text = "Public Interest Add Ons", Value = "PublicInterest", Price = new Decimal(0.00), Discount = new Decimal(0.00) });
                list.Add(new DirecTVAddOn { Text = "HD Extra Pack", Value = "HDExtraPack", Price = new Decimal(4.99), Discount = new Decimal(0.00) });

                return list;
            }
        }

        public override string ToString()
        {
            return this.Text;
        }
    }

    public class InternationalProgram
    {
        public string Value { get; set; }
        private string text;
        public string Text
        {
            get
            {
				string packagePrice = (this.Price - this.Discount).ToString("C") + "/mo";
				if (!string.IsNullOrEmpty(text))
				{
					// make sure we don't double the pricing info
					text = text.Replace(packagePrice, "");
				}

				return text + " " + packagePrice;
            }
            set
            {
                text = value;
            }
        }
        public Decimal Price { get; set; }
        public Decimal Discount { get; set; }
        public static List<InternationalProgram> AvailablePrograms
        {
            get
            {
                List<InternationalProgram> internationalPrograms = new List<InternationalProgram>();
                internationalPrograms.Add(new InternationalProgram { Text = "Brazilian - PFC", Value = "BPFC", Price = new Decimal(19.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Brazilian - TV Globo", Value = "BTVG", Price = new Decimal(19.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Brazilian - Brazilian Direct", Value = "BBD", Price = new Decimal(29.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Chinese - Mandarin Direct", Value = "CMD", Price = new Decimal(16.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Chinese - Chinese Direct", Value = "CCD", Price = new Decimal(12.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Filipino - Filipino Direct", Value = "FFD", Price = new Decimal(35.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Filipino - TFC Direct", Value = "FTFCD", Price = new Decimal(26.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Filipino - Pinoy Direct", Value = "FPD", Price = new Decimal(25.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Korean - Korean Direct", Value = "KKD", Price = new Decimal(28.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Korean - Korean Direct Golf", Value = "KKDG", Price = new Decimal(33.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Russian - C1RW", Value = "RC1RW", Price = new Decimal(14.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Russian - Russian Direct", Value = "RRD", Price = new Decimal(14.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Russian - Russian Direct Plus", Value = "RRDP", Price = new Decimal(29.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Russian - Russian Direct II", Value = "RRD2", Price = new Decimal(34.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Russian - RTR Planeta", Value = "RRTRP", Price = new Decimal(14.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "South Asian - FILMY", Value = "SAFILMY", Price = new Decimal(19.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "South Asian - Hindi Direct", Value = "SAHD", Price = new Decimal(21.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "South Asian - Disha Indian", Value = "SADI", Price = new Decimal(14.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Vietnamese - Viet Direct Plus", Value = "VVDP", Price = new Decimal(19.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Vietnamese - SBTN", Value = "VSBTN", Price = new Decimal(14.99), Discount = new Decimal(0.00) });
                internationalPrograms.Add(new InternationalProgram { Text = "Sports - Willow Cricket", Value = "SWC", Price = new Decimal(14.99), Discount = new Decimal(0.00) });

                return internationalPrograms;
            }
        }

        public override string ToString()
        {
            return this.Text;
        }
    }

    public class BasePackage
    {
        public string Value { get; set; }
        private string text;
        public string Text
        {
            get
            {
				string packagePrice = (this.Price - this.Discount).ToString("C") + "/mo";
				if(!string.IsNullOrEmpty(text))
				{
					// make sure we don't double the pricing info
					text = text.Replace(packagePrice, "");
				}

				return text + " " + packagePrice;
            }
            set
            {
                text = value;
            }
        }
        public Decimal Price { get; set; }
        public Decimal Discount { get; set; }
        public static List<BasePackage> AvailableEnglishPackages
        {
            get
            {
                List<BasePackage> list = new List<BasePackage>();
                list.Add(new BasePackage { Text = "Select", Value = "Select", Price = new Decimal(39.99), Discount = new Decimal(20.00) });
                list.Add(new BasePackage { Text = "Entertainment", Value = "Entertainment", Price = new Decimal(47.99), Discount = new Decimal(23.00) });
                list.Add(new BasePackage { Text = "Choice", Value = "Choice", Price = new Decimal(56.99), Discount = new Decimal(27.00) });
                list.Add(new BasePackage { Text = "Extra", Value = "Extra", Price = new Decimal(63.99), Discount = new Decimal(29.00) });
                list.Add(new BasePackage { Text = "Ultimate", Value = "Ultimate", Price = new Decimal(71.99), Discount = new Decimal(32.00) });
                list.Add(new BasePackage { Text = "Premier", Value = "Premier", Price = new Decimal(121.99), Discount = new Decimal(32.00) });

                return list;
            }

        }

        public static List<BasePackage> AvailableSpanishPackages
        {
            get
            {
                List<BasePackage> list = new List<BasePackage>();
                list.Add(new BasePackage { Text = "Mas Latino", Value = "MasLatino", Price = new Decimal(36.99), Discount = new Decimal(12.00) });
                list.Add(new BasePackage { Text = "Optimo Mas", Value = "OptimoMas", Price = new Decimal(50.99), Discount = new Decimal(21.00) });
                list.Add(new BasePackage { Text = "Mas Ultra", Value = "MasUltra", Price = new Decimal(67.99), Discount = new Decimal(28.00) });
                list.Add(new BasePackage { Text = "Lo Maximo", Value = "LoMaximo", Price = new Decimal(129.99), Discount = new Decimal(28.00) });

                return list;
            }
        }

        public override string ToString()
        {
            return this.Text;
        }
    }

    public class SpanishAddOn
    {
        public string Value { get; set; }
        private string text;
        public string Text
        {
            get
            {
				string packagePrice = (this.Price - this.Discount).ToString("C") + "/mo";
				if (!string.IsNullOrEmpty(text))
				{
					// make sure we don't double the pricing info
					text = text.Replace(packagePrice, "");
				}

				return text + " " + packagePrice;
            }
            set
            {
                text = value;
            }
        }
        public Decimal Price { get; set; }
        public Decimal Discount { get; set; }
        public static List<SpanishAddOn> AvailableAddOns
        {
            get
            {
                List<SpanishAddOn> list = new List<SpanishAddOn>();
                list.Add(new SpanishAddOn { Text = "En Espanol", Value = "EnEspanol", Price = new Decimal(14.99), Discount = new Decimal(0.00) });
                list.Add(new SpanishAddOn { Text = "Americas Plus", Value = "AmericasPlus", Price = new Decimal(7.99), Discount = new Decimal(0.00) });
                list.Add(new SpanishAddOn { Text = "Mexico Plus", Value = "MexicoPlus", Price = new Decimal(7.99), Discount = new Decimal(0.00) });
                list.Add(new SpanishAddOn { Text = "Directv Deportes", Value = "DirectvDeportes", Price = new Decimal(4.99), Discount = new Decimal(0.00) });

                return list;
            }
        }

        public override string ToString()
        {
            return this.Text;
        }
    }

    public class SportsPackage
    {
        public string Value { get; set; }
        private string text;
        public string Text
        {
            get
            {
				string packagePrice = (this.Price - this.Discount).ToString("C") + "/mo";
				if (!string.IsNullOrEmpty(text))
				{
					// make sure we don't double the pricing info
					text = text.Replace(packagePrice, "");
				}

				return text + " " + packagePrice;
            }
            set
            {
                text = value;
            }
        }
        public Decimal Price { get; set; }
        public Decimal Discount { get; set; }
        public static List<SportsPackage> AvailablePackages
        {
            get
            {
                List<SportsPackage> list = new List<SportsPackage>();
                list.Add(new SportsPackage { Text = "NFL Sunday Ticket", Value = "NFLSundayTicket", Price = new Decimal(0.00), Discount = new Decimal(0.00) });
                list.Add(new SportsPackage { Text = "MLB Extra Innings", Value = "MLBExtraInnings", Price = new Decimal(197.94), Discount = new Decimal(0.00) });
                list.Add(new SportsPackage { Text = "MLS Direct Kick", Value = "MLSDirectKick", Price = new Decimal(79.00), Discount = new Decimal(0.00) });
                list.Add(new SportsPackage { Text = "Fox Soccer Plus", Value = "FoxSoccerPlus", Price = new Decimal(14.99), Discount = new Decimal(0.00) });
                list.Add(new SportsPackage { Text = "NBA League Pass", Value = "NBALeaguePass", Price = new Decimal(49.99), Discount = new Decimal(0.00) });

                return list;
            }
        }
        public override string ToString()
        {
            return this.Text;
        }
    }

    public class OtherEquipment
    {
        public string Value { get; set; }
        private string text;
        public string Text
        {
            get
            {
				string packagePrice = this.Price.ToString("C");
				if (!string.IsNullOrEmpty(text))
				{
					// make sure we don't double the pricing info
					text = text.Replace(packagePrice, "");
				}

                return string.Format("{0} - {1:C}", text, Price);
            }
            set
            {
                text = value;
            }
        }
        public Decimal Price { get; set; }
        public bool IsComplimentary { get; set; }
        public int Quantity { get; set; }
        public static List<OtherEquipment> AvailableEquipment
        {
            get
            {
                List<OtherEquipment> list = new List<OtherEquipment>();
                list.Add(new OtherEquipment { Text = "WALL FISH", Value = "WALLFISH", Price = 40M });
                list.Add(new OtherEquipment { Text = "LOOP", Value = "LOOP", Price = 35M });
                list.Add(new OtherEquipment { Text = "HDMI CABLE", Value = "HDMICABLE", Price = 20M });
                list.Add(new OtherEquipment { Text = "POLE MOUNT", Value = "POLEMOUNT", Price = 50M });
                list.Add(new OtherEquipment { Text = "EXTRA REMOTE", Value = "EXTRAREMOTE", Price = 20M });
                list.Add(new OtherEquipment { Text = "RF MODULATOR", Value = "RFMODULATOR", Price = 10M });

                return list;
            }
        }

        public override string ToString()
        {
            return this.Text;
        }
    }

    public class Equipment
    {
        public string Value { get; set; }
        private string text;
        public string Text
        {
            get
            {
				string packagePrice = this.Price.ToString("C");
				if (!string.IsNullOrEmpty(text))
				{
					// make sure we don't double the pricing info
					text = text.Replace(packagePrice, "");
				}

                return string.Format("{0} - {1:C}", text, Price);
            }
            set
            {
                text = value;
            }
        }
        public Decimal Price { get; set; }
        public bool IsComplimentary { get; set; }
        public int Quantity { get; set; }
        public static List<Equipment> AvailableEquipment
        {
            get
            {
                List<Equipment> list = new List<Equipment>();
                list.Add(new Equipment { Text = "GENIE", Value = "GENIE", Price = 25M });
                list.Add(new Equipment { Text = "MINI RECEIVERS", Value = "MINIRECEIVER", Price = 10M });
                list.Add(new Equipment { Text = "WIRELESS MINI RECEIVERS", Value = "WIRELESSMINIRECEIVER", Price = 10M });
                list.Add(new Equipment { Text = "HD-DVR RECEIVERS", Value = "HDDVRRECEIVER", Price = 25M });
                list.Add(new Equipment { Text = "HD RECEIVERS", Value = "HDRECEIVERS", Price = 10M });
                list.Add(new Equipment { Text = "DVR RECEIVERS", Value = "DVRRECEIVERS", Price = 10M });
                list.Add(new Equipment { Text = "STANDARD RECEIVERS", Value = "STANDARDRECEIVERS", Price = 10M });

                return list;
            }
        }

        public override string ToString()
        {
            return this.Text;
        }
    }

    public class ProtectionPackage
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public Decimal Price { get; set; }
        public static List<ProtectionPackage> AvailablePackages
        {
            get
            {
                List<ProtectionPackage> list = new List<ProtectionPackage>();
                list.Add(new ProtectionPackage { Text = "DirecTV Protection Plan $7.99", Value = "DTVProtectionPlan799", Price = 7.99M });
                list.Add(new ProtectionPackage { Text = "DirecTV Protection Plan $19.99", Value = "DTVProtectionPlan1999", Price = 19.99M });
                list.Add(new ProtectionPackage { Text = "DirecTV Protection Plan $24.99", Value = "DTVProtectionPlan2499", Price = 24.99M });
                return list;
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }


}
