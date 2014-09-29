
using EditorsCommon;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace ErpContent.Controllers
{
    public class ReviewerController : System.Web.Mvc.Controller
    {

        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);



         private IVinStorage  _vinStorage;


        public ReviewerController(IVinStorage vinStorage){
            _vinStorage = vinStorage;
        }


        //protected override void OnException(ExceptionContext context)
        //{
        //    // log error here
        //}




        [System.Web.Mvc.Authorize(Roles=EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration=0, VaryByParam="none")]
        public ActionResult  Index()
        {
            MembershipUser mu = Membership.GetUser(); ;
            ProfileBase p = ProfileBase.Create( mu.UserName);
            ViewBag.PageTitle = p["FirstName"] + " " + p["LastName"];


            return View("ReviewerDashboard");
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult Importers()
        {
            return View("Importers");
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult ProducersImporters()
        {
            return View("ProducersImporters");
        }



        [System.Web.Mvc.Authorize(Roles=EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration=0, VaryByParam="none")]
        public ActionResult  NewAddNote()
        {
            return View("AddNotes");
        }

        [System.Web.Mvc.Authorize(Roles=EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration=0, VaryByParam="none")]
        public ActionResult  Notes()
        {
            return View("ReviewerNotes");
        }



        [System.Web.Mvc.Authorize(Roles=EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration=0, VaryByParam="none")]
        public ActionResult  Articles()
        {
            return View("ReviewerArticles");
        }


        [System.Web.Mvc.Authorize(Roles=EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration=0, VaryByParam="none")]
        public ActionResult  SearchWineLabel(String term, String p)
        {
            return Json(_vinStorage.SearchLabelProducer(p, term), JsonRequestBehavior.AllowGet);
        }




        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult SearchProducer(String term)
        {

            return Json(  _vinStorage.SearchProducer(term) , JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult SearchProducerExtended(String term)
        {

            return Json(_vinStorage.SearchProducerExtended(term), JsonRequestBehavior.AllowGet);
        }



        



        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult SearchLocation(String c, String r, String l, String lc, String s)
        {
            return Json(_vinStorage.SearchLocation(c,r,l,lc,s), JsonRequestBehavior.AllowGet);
        }





        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public ActionResult GetEditors()
        {
            string[] un = Roles.GetUsersInRole(EditorsCommon.Constants.roleNameEditor);

            

            List<Object> reviewers = new List<Object>();
            foreach (var n in un)
            {
                MembershipUser mu = ((ERP.Providers.SqlMembershipProvider)Membership.Provider).GetUser(n, false);

                ProfileBase p = ProfileBase.Create(mu.UserName);

                if (mu != null)
                {
                    reviewers.Add(new { id = (int)mu.ProviderUserKey, name = p["FirstName"] + " " + p["LastName"] });
                }
            }


            return Json(reviewers, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public ActionResult GetReviewers()
        {
            string[] un = Roles.GetUsersInRole(EditorsCommon.Constants.roleNameReviewer);

            List<Object> reviewers = new List<Object>();
            foreach (var n in un)
            {
                MembershipUser mu = ((ERP.Providers.SqlMembershipProvider)Membership.Provider).GetUser(n, false);

                ProfileBase p = ProfileBase.Create(mu.UserName);

                if (mu != null)
                {
                    reviewers.Add(new { id = (int)mu.ProviderUserKey, name = p["FirstName"] + " " + p["LastName"] });
                }
            }


            return Json(reviewers, JsonRequestBehavior.AllowGet);
        }
    }
}
