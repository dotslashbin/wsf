using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErpContent.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {


            if (User.IsInRole(EditorsCommon.Constants.roleNameAdmin))
                return Redirect("~/ChiefEditor");


            if (User.IsInRole(EditorsCommon.Constants.roleNameReviewer))
            {
                return Redirect("~/Reviewer");
            }

            if (User.IsInRole(EditorsCommon.Constants.roleNameSubEditor) || User.IsInRole(EditorsCommon.Constants.roleNameEditor))
            {
                return Redirect("~/EditorArea");
            }



            ViewBag.Message = "You  are authorized, but your do not have any role(s) assigned to access this resource.";

            return View();
        }


        /**
         * This will display a temporary page
         */
        public ActionResult ShowTemporaryPage()
        {
            return View(); 
        }

    }

}
