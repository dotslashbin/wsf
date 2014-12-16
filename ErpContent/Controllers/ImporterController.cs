
using EditorsCommon;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ErpContent.Controllers
{
    public class ImporterController : Controller
    {

 private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IImporterStorage _importerStorage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        public ImporterController(IImporterStorage storage)
         {
            _importerStorage = storage;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult SearchByName(String term)
        {

            IEnumerable<WineImporterItem> result;

            result = _importerStorage.Search(new WineImporterItem() { name = term });

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult SearchByProducerId(int id)
        {

            IEnumerable<WineImporterItem> result;

            result = _importerStorage.SearchByProducerId(id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult AddProducer(int importerId, int producerId)
        {
            WineImporterItem result = _importerStorage.AddToProducer(importerId,producerId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult RemoveProducer(int importerId, int producerId)
        {
            WineImporterItem result = _importerStorage.RemoveFromProducer(importerId, producerId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetNewImporter()
        {

            return Json(new WineImporterItem(), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Insert(String str)
        {
            var o = new JavaScriptSerializer().Deserialize<WineImporterItem>(str);

            var result = _importerStorage.Create(o);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Remove(String str)
        {
            var o = new JavaScriptSerializer().Deserialize<WineImporterItem>(str);

            var result = _importerStorage.Delete(o);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Update(String str)
        {
            var o = new JavaScriptSerializer().Deserialize<WineImporterItem>(str);

            var result = _importerStorage.Update(o);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


	}
}