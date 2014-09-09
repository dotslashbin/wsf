using EditorsCommon;
using EditorsDbLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ErpContent.Controllers
{
    public class PublicationController : Controller
    {

        private PublicationDataStorage _PublicationDataStorage;



        public PublicationController(PublicationDataStorage storageObject)
        {
            _PublicationDataStorage = storageObject; 
        }

        //
        // GET: /PublicationData/
        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor
            + "," + EditorsCommon.Constants.roleNameAdmin)]
        [HttpPost]
        public ActionResult AddPublication(String PublicationData)
        {

            PublicationData newPublicationData = new JavaScriptSerializer().Deserialize<PublicationData>(PublicationData);

            var result = _PublicationDataStorage.Create(newPublicationData);

            return Json(result);
        }

        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor
            + "," + EditorsCommon.Constants.roleNameAdmin)]
        public ActionResult DeletePublication(int ID)
        {

            PublicationData PublicationData = new PublicationData(); 

            _PublicationDataStorage.Delete(PublicationData); 

            return Json(PublicationData.ID, JsonRequestBehavior.AllowGet); 
        }


        [Authorize]
        [HttpPost]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetPublications()
        {

            var result = new PublicationData() { Created = DateTime.Now };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}