using DataReef.Core;
using DataReef.Core.Extensions;
using DataReef.TM.Api.Classes.Enums;
using DataReef.TM.Api.Classes.ViewModels;
using DataReef.TM.Api.Common;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Integrations;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataReef.TM.Api.Controllers.Web
{
    public class HomeController : Controller
    {
        private static readonly Dictionary<int, string> Messages = new Dictionary<int, string>
        {
            { 1, "Registration completed successfully"},
            { 2, "Password reset completed successfully"},
        };

        private readonly Lazy<IAuthenticationService> _authService;
        private readonly Lazy<IDataService<PasswordReset>> _resetService;
        private readonly Lazy<IOUSettingService> _ouSettingService;

        public HomeController(Lazy<IAuthenticationService> authService,
            Lazy<IDataService<PasswordReset>> resetService,
            Lazy<IOUSettingService> ouSettingService)
        {
            _authService = authService;
            _resetService = resetService;
            _ouSettingService = ouSettingService;
        }


        // GET: Home
        public ActionResult UserInvitation(UserInvitationViewModel model)
        {
            string username = model.Username.UrlEncodeEmail();

            string customUrl = string.Format("{0}userinvitation?guid={1}&username={2}", Constants.CustomURL, model.Id, username);
            return PartialView("UserInvitation", customUrl);
        }

        /// <summary>
        /// This action is used as a proxy from an email messages to custom URLs intended for the mobile app to handle
        /// You'll need to send the action and the parameters
        /// </summary>
        /// <returns></returns>
        public ActionResult Redirect()
        {
            var query = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            var action = WebRedirectActions.Invalid;
            var stringAction = query["action"];
            Enum.TryParse(stringAction, true, out action);

            switch (action)
            {
                case WebRedirectActions.UserInvitation:
                    return Redirect($"~/home/userregistration?{query}");
                case WebRedirectActions.ResetPassword:
                    return Redirect($"~/home/PasswordReset?{query}");
            }

            string customUrl = string.Format("{0}{1}?{2}", Constants.CustomURL, stringAction, query.ToString().EscapeEmail());
            if (customUrl.Contains("noteID"))
            {
                customUrl = string.Format("{0}{1}", Constants.CustomURL, query.ToString().EscapeEmail()).Replace("%3f", "?");
            }
            return View("Redirect", (object)customUrl);
        }

        [HttpGet]
        public ActionResult UserRegistration(UserRegistrationViewModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError("InvalidRequest", "Request data is invalid!");
            }

            var invitation = _authService.Value.GetPendingUserInvitation(model.Guid);
            if (invitation == null)
            {
                ModelState.AddModelError("InvitationId", "Invitation ID is invalid!");
            }
            else
            {
                if (invitation.ExpirationDate < DateTime.UtcNow)
                {
                    ModelState.AddModelError("InvitationExpired", "Invitation has expired!");
                }
                else
                {
                    model.Username = invitation.EmailAddress;
                }
            }

            var isUserexist = _authService.Value.CheckUserExist(invitation.EmailAddress);
            if (isUserexist)
            {
                var response = _authService.Value.CreateUser(model.ToNewUser());
                if (response.ClientSecret != null)
                {
                    return RedirectToAction("Success", new { id = 1 });
                }
            }
            return View("UserRegistration", model?.ToRegistration());
        }

        [HttpPost]
        public ActionResult RegisterUser(RegistrationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    byte[] photo = null;
                    if (model.Photo?.InputStream != null)
                    {
                        try
                        {
                            photo = model.Photo.InputStream.GetResizedContent(200, 200);
                        }
                        catch
                        {
                            throw new ApplicationException("The file you chose is not a valid image!");
                        }
                    }

                    var response = _authService.Value.CreateUser(model.ToNewUser(), photo, model.PhoneNumber);

                    return RedirectToAction("Success", new { id = 1 });
                }
                else
                {
                    model.ShowFormIfModelHasErrors = true;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }

            //if (ModelState.IsValid)
            //{
            //    return RedirectToAction("Success", new { id = 1 });
            //}
            //model.ShowFormIfModelHasErrors = true;

            return View("UserRegistration", model);
        }

        [HttpGet]
        public ActionResult Success(int id)
        {
            string message = null;

            if (Messages.ContainsKey(id))
            {
                message = Messages[id];
            }
            return View("Success", (object)message);
        }

        [HttpGet]
        public ActionResult PasswordReset(Guid guid)
        {
            var resetItem = _resetService.Value.Get(guid);

            if (resetItem == null)
            {
                ModelState.AddModelError("InvalidGuid", "The id you provided is invalid!");
            }
            else
            {
                if (resetItem.PasswordWasReset)
                {
                    ModelState.AddModelError("IdAlreadyUsed", "The id you provided has already been used!");
                }
                if (DateTime.UtcNow.Ticks > resetItem.ExpirationDate.Value.Ticks)
                {
                    ModelState.AddModelError("Expired", "Password Reset Has Expired!");
                }
            }

            var model = new ResetPasswordViewModel
            {
                Guid = guid,
                ShowFormIfModelHasErrors = false
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult PasswordReset(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _authService.Value.CompletePasswordReset(model.Guid, model.Password, "");
                    return RedirectToAction("Success", new { id = 2 });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                }
            }
            model.ShowFormIfModelHasErrors = true;

            return View(model);
        }


        /// <summary>
        /// This method is used to retrieve a text file stored in App_Data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Logs(string id)
        {
            string path = Server.MapPath(string.Format("~/App_Data/{0}", id));
            if (!System.IO.File.Exists(path))
            {
                return HttpNotFound();
            }
            return File(path, "text/plain");

        }

        public static string GetCustomURL(NameValueCollection queryString)
        {
            var query = HttpUtility.ParseQueryString(queryString.ToString());
            var stringAction = query["action"];
            return $"{Constants.CustomURL}{stringAction}?{query.ToString().EscapeEmail()}";
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> getapikey()
        {
            List<Guid> Live = new List<Guid>();
            ////Live.Add(Guid.Parse("15BE0447-E406-4CF0-9626-3208A62E8BB1"));
            Live.Add(Guid.Parse("15BE0447-E406-4CF0-9626-3208A62E8BB1"));
            Live.Add(Guid.Parse("E40CD182-F8B9-499D-9514-FA08835FFD15"));
            Live.Add(Guid.Parse("C05F1FBC-C3DE-4B9D-A20A-C79A096D3CEF"));
            Live.Add(Guid.Parse("4988A135-C98F-4F78-9472-4DC99FAA35A2"));
            Live.Add(Guid.Parse("1807CCB7-AD52-45A2-9B7E-E29FB59F8A6A"));
            Live.Add(Guid.Parse("02326810-3FAD-4E96-BEB9-3B3F550240C5"));
            Live.Add(Guid.Parse("5E938B38-D094-4A66-A752-3D873C9A9D09"));
            Live.Add(Guid.Parse("9B36AB8B-AC26-42A5-B2B8-19741D5688B4"));
            Live.Add(Guid.Parse("6B47670A-9E3D-44DB-8951-91BB12B96DE1"));
            Live.Add(Guid.Parse("D6EAB35B-9F5D-4F81-A77D-10E604EC84DA"));
            Live.Add(Guid.Parse("3EA46D49-311C-4B28-A5B9-894EB9C8493A"));
            Live.Add(Guid.Parse("CEFB2830-3F7A-45F7-8D62-D8332E8F13FF"));
            Live.Add(Guid.Parse("C3FA8BED-0A17-41FD-948D-B3BD82161C43"));
            Live.Add(Guid.Parse("CA63B937-82DE-42CF-9F4F-70B8101E5EB0"));
            Live.Add(Guid.Parse("D325B6B0-29E3-41A4-89F4-8E6B2EA621A9"));
            Live.Add(Guid.Parse("9C3BEECB-B885-46D6-BFC4-E1CC93793618"));
            Live.Add(Guid.Parse("912365E6-136C-4F9B-877F-F7FCFD13F363"));
            Live.Add(Guid.Parse("0AE851B0-B0A5-490E-8F5A-3B52C03302EF"));
            Live.Add(Guid.Parse("010980F0-215D-4500-9D46-426915897CB4"));
            Live.Add(Guid.Parse("79B448CE-D818-444D-8D25-63F89809B9FC"));
            Live.Add(Guid.Parse("8780D382-6D1E-4FF5-AB6C-3EF003670C7B"));
            Live.Add(Guid.Parse("62B39439-462C-44BE-80E8-4EFBC696EF71"));
            Live.Add(Guid.Parse("131D44F3-85A4-43A8-A74E-45B2E484B7F8"));
            Live.Add(Guid.Parse("3577EDF7-244D-4FA9-979F-9D8EF95BDF2E"));
            Live.Add(Guid.Parse("C6726A6C-F0DA-430C-8E81-C19D5ED2CB9A"));
            Live.Add(Guid.Parse("48CC4473-0F46-4B99-A904-DE7C97592209"));
            Live.Add(Guid.Parse("AAAD8E91-C244-4566-8D3E-3325AB124438"));
            Live.Add(Guid.Parse("3631C88D-FE4C-4F2B-B9A8-1DDDEBC82EE1"));
            Live.Add(Guid.Parse("A46DF83D-7626-4ADE-BA87-3993E482F046"));
            Live.Add(Guid.Parse("180F8A23-1CC0-4F85-853A-9AD0185B6E0A"));
            Live.Add(Guid.Parse("CF2C8839-1C96-4DD2-9AAA-60358EA5861E"));
            Live.Add(Guid.Parse("F433F579-C6CF-41C1-97B6-15FE386FBC8B"));
            Live.Add(Guid.Parse("E21D5D88-EE61-42E0-A459-D5B0F5D6125B"));
            Live.Add(Guid.Parse("0B6DDC01-75CB-4222-BA96-BB9163679C78"));
            Live.Add(Guid.Parse("B5CF233A-1BE3-4C75-9420-76F515609983"));
            Live.Add(Guid.Parse("8ACD73CB-D371-4FCF-B986-BA20EE0BF0B5"));
            Live.Add(Guid.Parse("9C5624C0-9CFC-406D-8D79-CC418CAE13A0"));
            Live.Add(Guid.Parse("0857B853-DCDC-4F61-AC3B-57EA41EAE989"));
            Live.Add(Guid.Parse("A138CC0F-DF2B-4AA7-BB25-F17DFE530CF4"));
            Live.Add(Guid.Parse("E189A183-74DD-40E3-ABEB-F51E29FEA150"));
            Live.Add(Guid.Parse("447A36FD-021F-4307-A9FA-283272845CB3"));
            Live.Add(Guid.Parse("6E5EE395-7124-4525-AEE5-A4306E526CC4"));
            Live.Add(Guid.Parse("A943E3AC-CE0D-4FEC-80F9-E4EFDC428789"));
            Live.Add(Guid.Parse("371686C0-514B-46C4-B0CF-F9B5A3A22351"));
            Live.Add(Guid.Parse("59178DE2-3CB8-4ACB-8911-8FA8BD7790EE"));
            Live.Add(Guid.Parse("E429D0BC-7B6B-4DF7-91BF-BF58F635A806"));
            Live.Add(Guid.Parse("B741FE6C-0DDF-438F-B38E-199D2B1A064D"));
            Live.Add(Guid.Parse("E41382FA-F3C4-4A68-AB5E-85FB774C7106"));
            Live.Add(Guid.Parse("F5ABFA15-E7AE-4790-874A-EAAE20AABDF5"));
            Live.Add(Guid.Parse("66FA4DCE-3FE9-496B-95D6-EC06E2DF1F60"));
            Live.Add(Guid.Parse("1F5731D4-8B87-4306-BD6A-9E19C9F2CA86"));
            Live.Add(Guid.Parse("629B3B1C-F132-4B11-AF36-5AFBB5D06690"));
            Live.Add(Guid.Parse("CCC6C0CE-BD96-4E4E-9A05-4E902FEF3FA8"));
            Live.Add(Guid.Parse("910B5DBA-A0DA-4295-A791-FA480CFB05BC"));
            Live.Add(Guid.Parse("60A5D3DB-EA82-49C2-95AF-5F5265D66F78"));
            Live.Add(Guid.Parse("A5CE0651-5586-4A2C-9D97-F24C8F78D70F"));
            Live.Add(Guid.Parse("0A0DD140-D030-42CA-BF0E-60489869B394"));
            Live.Add(Guid.Parse("166BEA00-EB8C-45E0-9A66-619795D41F98"));
            Live.Add(Guid.Parse("703BC324-C7B6-48F1-9954-05AF1E6B9E1D"));
            Live.Add(Guid.Parse("FC0F5C9A-FCE2-41F5-BF5C-D5F64CBAEDF0"));
            Live.Add(Guid.Parse("993A2243-9224-4B87-9C5F-BB00063B7117"));
            Live.Add(Guid.Parse("E17D1DC4-96D4-453F-B4D1-CEBB4711D49C"));
            Live.Add(Guid.Parse("26B41097-DFCD-4CBD-8410-5B3973F04F7A"));
            Live.Add(Guid.Parse("6BADADCB-E1D9-46D8-8145-28D4EAA10EAB"));
            Live.Add(Guid.Parse("7B1D3BB1-27D6-4E51-B563-76C7436C8C06"));
            Live.Add(Guid.Parse("3D4DFC9A-A39A-48E3-BEBA-E1D3A144FA54"));
            Live.Add(Guid.Parse("C4BFA6A7-A7FE-4119-8D68-285547CB2526"));
            Live.Add(Guid.Parse("CEF0BD14-EDD2-442E-852C-B5F83BCF75D8"));
            Live.Add(Guid.Parse("878BA074-CB13-462B-BD71-7AA8103BB7F7"));
            Live.Add(Guid.Parse("5D1C6B87-2B44-4A78-8D71-53EFBA22D96D"));
            Live.Add(Guid.Parse("970AB8F3-5CF5-4255-9322-27FB448A8D1D"));
            Live.Add(Guid.Parse("F3A17101-A741-4CE9-9345-A41AA2EDCD0D"));
            Live.Add(Guid.Parse("986640F3-8DAE-4F56-B786-33B5515366DF"));
            Live.Add(Guid.Parse("14D76D51-6054-4797-826E-63862EEDC7EB"));
            Live.Add(Guid.Parse("EAA1BFA7-0352-4164-AC08-72992E3744FB"));
            Live.Add(Guid.Parse("4A366249-5D1A-4B30-B934-45690EBC0356"));
            Live.Add(Guid.Parse("8B02B168-A7C6-49FB-A338-A3B3ADF01045"));
            Live.Add(Guid.Parse("E4F23ABD-966C-4AF6-9111-69B78941A44D"));
            Live.Add(Guid.Parse("F9B95A67-5E3F-4599-8268-20EE6B958769"));
            Live.Add(Guid.Parse("D7BC1CB1-7FD1-42BB-A87A-0D152219187C"));
            Live.Add(Guid.Parse("564CCABB-5320-4106-82FA-7E2608DC6010"));
            Live.Add(Guid.Parse("20A7EA5E-89A3-4E02-AFA8-C93961C5F0D1"));
            Live.Add(Guid.Parse("65686EED-8740-4A3F-AB53-796B1CC07AB1"));
            Live.Add(Guid.Parse("D70E43F4-87D7-46ED-AB29-78A4789CFBF7"));
            Live.Add(Guid.Parse("852ECBC3-4B9E-4D80-8DED-CB53B0A47C9E"));
            Live.Add(Guid.Parse("DAC0468B-E140-4A7C-B598-1A942BB2AF75"));
            Live.Add(Guid.Parse("9213D2BB-DBD7-48FD-936A-5DF5FB82907C"));
            Live.Add(Guid.Parse("21ECF5AF-90D3-47AF-B8FB-A8B888CF769F"));
            Live.Add(Guid.Parse("9D0027B5-7165-476B-B0B5-72107E34965F"));
            Live.Add(Guid.Parse("D4ECB4D3-7D09-4D25-B99A-1FFC2976B2BA"));
            Live.Add(Guid.Parse("A139CA24-2C3A-44B4-A062-612ADCA0A8B9"));
            Live.Add(Guid.Parse("87DD2EA5-5ECB-46A4-B6DE-A15932B75A29"));
            Live.Add(Guid.Parse("7BA1DB40-C3FC-4B2F-A1B8-A4DCB4F1D956"));
            Live.Add(Guid.Parse("1C5556CF-3B14-4330-893E-1CB44E76232A"));
            Live.Add(Guid.Parse("C65F75B7-6558-4661-9E50-154BCF2D4DCF"));
            Live.Add(Guid.Parse("7ECEF2E8-5358-4041-82F4-335213E2AC11"));
            Live.Add(Guid.Parse("B816B9B8-B418-422E-83C4-E4327ABF47ED"));
            Live.Add(Guid.Parse("B5183549-0A4A-424D-AED5-8B4C2C1BA588"));
            Live.Add(Guid.Parse("18357C49-4C41-4565-85AD-5C73AABF5A54"));
            Live.Add(Guid.Parse("3C94BFEA-5A77-4D11-9BA3-83B581CD6E8F"));
            Live.Add(Guid.Parse("25C3F5A6-71A4-4F9C-B667-398D6465317F"));
            Live.Add(Guid.Parse("02DF7F0C-8623-4977-9600-8641E3F6EB52"));
            Live.Add(Guid.Parse("F03C4BB9-701A-492A-B522-652735BD0DFC"));
            Live.Add(Guid.Parse("1A351937-6BF5-456B-A828-AF774F20C9E0"));
            Live.Add(Guid.Parse("B7875FCB-595E-4114-91E9-573056F4BC97"));
            Live.Add(Guid.Parse("AF1D1AA7-7A37-4252-A9B9-46552D9811A7"));
            Live.Add(Guid.Parse("328F8548-B39B-4ADD-9D2C-E11D21914E58"));
            Live.Add(Guid.Parse("6392DBCB-2B05-49DD-8D72-90E5010CAB78"));
            Live.Add(Guid.Parse("FC0D7EA4-AD1C-473F-86CB-E62DD4C7F67A"));
            Live.Add(Guid.Parse("A711C6EE-3045-4F58-B2C5-D0BE2345F505"));
            Live.Add(Guid.Parse("49D69CD3-8901-4014-8EDD-68BB9E4AC986"));
            Live.Add(Guid.Parse("686BF11B-06E0-41F1-ABEA-A66E737B586B"));
            Live.Add(Guid.Parse("B55963E6-7907-450A-B6D1-E4000137EE02"));
            Live.Add(Guid.Parse("95B5D1CB-2725-44A2-A6B0-0DF4618AFB31"));
            Live.Add(Guid.Parse("7A8CCF4F-73EF-40A1-8BBF-254AE1A354FB"));
            Live.Add(Guid.Parse("3F9ABF28-9A41-4CEA-A29D-F00279822116"));
            Live.Add(Guid.Parse("75CC291C-6865-4F7A-A580-9EDB4D4707D2"));
            Live.Add(Guid.Parse("0BE7CE42-1FA3-4916-ACC4-E285EEFB32AA"));
            Live.Add(Guid.Parse("D3F0A907-C041-4760-B61D-78D9F5E6D335"));
            Live.Add(Guid.Parse("9B8509B3-D43B-4BE4-9752-9EAB5D3CBCA4"));
            Live.Add(Guid.Parse("6F39F9D2-6C38-4F1E-9F6F-D062A1A780D3"));
            Live.Add(Guid.Parse("4E97A87F-99CB-40AB-9250-6D14FA664D6C"));
            Live.Add(Guid.Parse("F9B9D584-0685-4470-96E4-E3159CB17CD7"));
            Live.Add(Guid.Parse("38D795B5-FE20-47D1-9F32-1037C2B6CFE6"));
            Live.Add(Guid.Parse("E8677345-1FF7-4A9F-8515-1F7C0C60AF14"));
            Live.Add(Guid.Parse("88802909-1E55-4BE7-AC5A-D9116464E1A1"));
            Live.Add(Guid.Parse("DFEA0B54-93A1-47E7-AD36-3B8E12BFFD1E"));
            Live.Add(Guid.Parse("094AC9CF-355A-49E1-A8AC-C4316E832880"));
            Live.Add(Guid.Parse("B079C3ED-381C-441F-BE54-3754CF59067E"));


            var list = new List<test>();


            foreach (var i in Live)
            {
                var sbSettings = _ouSettingService
                                    .Value
                                    .GetOUSettingForPropertyID<ICollection<SelectedIntegrationOption>>(i, "Integrations.Options.Selected")?
                                    .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                    .Data?
                                    .SMARTBoard;
                var lst = new test();
                lst.apikey = sbSettings?.ApiKey;
                lst.guid = i.ToString();
                list.Add(lst);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public class test
        {
            public string guid { get; set; }
            public string apikey { get; set; }

        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> checktime()
        {
            return Json(DateTime.Now.ToString() +  "_  " + DateTime.UtcNow.ToString(), JsonRequestBehavior.AllowGet);
        }
    }
}
