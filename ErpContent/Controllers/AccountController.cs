using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ErpContent.Models;
using EditorsCommon;
using System.Web.Profile;

namespace ErpContent.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        /// <summary>
        /// 
        /// </summary>
        private ICachedUserStorage _userStorage;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueStorage"></param>
        public AccountController(ICachedUserStorage userStorage)
        {
            _userStorage = userStorage;
        }


        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && Membership.ValidateUser(model.UserName, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                CachedUser user = new CachedUser() { userId = (int)Membership.GetUser(model.UserName).ProviderUserKey };


                MembershipUser mu = ((ERP.Providers.SqlMembershipProvider)Membership.Provider).GetUser(user.userId, false);
                ProfileBase p = ProfileBase.Create(mu.UserName);

                user.firstName = (string)p["FirstName"];
                user.lastName = (string)p["LastName"];
                user.isAvailable = true;
                user.userName = model.UserName;
                user.fullName = user.firstName + " " + user.lastName;
                user =  _userStorage.FindOrRegister(user);


                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }



        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        #endregion
    }
}
    