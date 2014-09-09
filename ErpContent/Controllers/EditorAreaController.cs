using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErpContent.Controllers
{
    public class EditorAreaController : Controller
    {
        //
        // GET: /EditorArea/


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameEditor 
            + "," + EditorsCommon.Constants.roleNameSubEditor 
            + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Index()
        {
            return View("EditorDashboard");
        }

    }
}
