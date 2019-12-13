using DataReef.Core;
using DataReef.Core.Extensions;
using DataReef.TM.Api.Classes.Enums;
using DataReef.TM.Api.Classes.ViewModels;
using DataReef.TM.Api.Common;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
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

        public HomeController(Lazy<IAuthenticationService> authService,
            Lazy<IDataService<PasswordReset>> resetService)
        {
            _authService = authService;
            _resetService = resetService;
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

            return View("UserRegistration", model?.ToRegistration());
        }

        [HttpPost]
        public ActionResult RegisterUser(RegistrationViewModel model)
        {
            try
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
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Success", new { id = 1 });
            }
            model.ShowFormIfModelHasErrors = true;

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
                    _authService.Value.CompletePasswordReset(model.Guid, model.Password);
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

    }
}
