using EditorsCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErpContent.Controllers
{
    public class CachedUserController : Controller
    {
        private ICachedUserStorage _storage;

        public CachedUserController(ICachedUserStorage storage)
        {
            _storage = storage;
        }

        //
        // GET: /CachedUser/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// should return list of all users
        /// </summary>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles =  EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult List()
        {
            return Json(_storage.Search(null), JsonRequestBehavior.AllowGet);
        }




	}
}