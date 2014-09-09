
using EditorsCommon;
using EditorsCommon.Publication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace ErpContent.Controllers
{

    public class ChiefEditorController : System.Web.Mvc.Controller
    {

        private IIssueStorage _issueStorage;


        public ChiefEditorController(IIssueStorage issueStorage)
        {
            _issueStorage = issueStorage;
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Index()
        {
            
            return View("ChiefDashboard");
        }


        
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult TastingEvents(int id)
        {
            return Json(_issueStorage.GetAssignments(id), JsonRequestBehavior.AllowGet);
        }




        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Publications()
        {
            return Json(_issueStorage.GetPublications(), JsonRequestBehavior.AllowGet);
        }



        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public ActionResult GetSubEditors()
        {
           string[] un =  Roles.GetUsersInRole(EditorsCommon.Constants.roleNameEditor);

           HashSet<string> unique = new HashSet<string>();
           foreach (var n in un)
           {
               unique.Add(n);
           }

           un = Roles.GetUsersInRole(EditorsCommon.Constants.roleNameSubEditor);
           foreach (var n in un)
           {
               if (!unique.Contains(n))
               {
                   unique.Add(n);
               }
           }



           List<Object> reviewers = new List<Object>();
           foreach (var n in unique)
           {
               MembershipUser mu = ((ERP.Providers.SqlMembershipProvider)Membership.Provider).GetUser(n, false);
               ProfileBase p = ProfileBase.Create(mu.UserName);

               if (mu != null)
               {
                   reviewers.Add(new { id = (int)mu.ProviderUserKey,  name = p["FirstName"] + " " + p["LastName"] });
               }
           }


           return Json(reviewers, JsonRequestBehavior.AllowGet);
        }




        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public ActionResult GetEditors()
        {
            string[] un = Roles.GetUsersInRole(EditorsCommon.Constants.roleNameEditor);

            HashSet<string> unique = new HashSet<string>();
            foreach (var n in un)
            {
                unique.Add(n);
            }



            List<Object> reviewers = new List<Object>();
            foreach (var n in unique)
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



        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
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
                    reviewers.Add(new { id = (int)mu.ProviderUserKey,  name = p["FirstName"] + " " + p["LastName"] });
                }
            }


            return Json(reviewers, JsonRequestBehavior.AllowGet);
        }



    }
}
