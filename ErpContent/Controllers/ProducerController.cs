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
    public class ProducerController : Controller
    {

        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IWineProducerStorage _producerStorage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        public ProducerController(IWineProducerStorage storage)
         {
            _producerStorage = storage;
        }

         /// <summary>
         /// 
         /// </summary>
         /// <param name="term"></param>
         /// <returns></returns>
         [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
         [OutputCache(Duration = 0, VaryByParam = "none")]
         public ActionResult SearchProducer(String term, int state = EditorsCommon.WorkFlowState.STATE_GROUP_ALL)
         {

             IEnumerable<WineProducer> result;

             if (String.IsNullOrEmpty(term) && state == EditorsCommon.WorkFlowState.STATE_GROUP_IN_PROCESS)
             {
                 result  = _producerStorage.SearchByWorkflowStatus(0);
             }
             else
             {
                 result = _producerStorage.SearchByName(term);

                 if (state == EditorsCommon.WorkFlowState.STATE_GROUP_IN_PROCESS)
                 {
                     result = from r in result where r.wfState  <= EditorsCommon.WorkFlowState.READY_APPROVED select r;
                 }

                 ///
                 if (state == EditorsCommon.WorkFlowState.STATE_GROUP_PUBLISHED)
                 {
                     result = from r in result where r.wfState == EditorsCommon.WorkFlowState.PUBLISHED select r;
                 }
             }

             return Json(result, JsonRequestBehavior.AllowGet);
         }

         /// <summary>
         /// 
         /// </summary>
         /// <param name="term"></param>
         /// <returns></returns>
         [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
         [OutputCache(Duration = 0, VaryByParam = "none")]
         public ActionResult SearchProducerExt(String term, int state = EditorsCommon.WorkFlowState.STATE_GROUP_ALL)
         {

             IEnumerable<WineProducerExt> result;

             if (String.IsNullOrEmpty(term) && state == EditorsCommon.WorkFlowState.STATE_GROUP_IN_PROCESS)
             {
                 result = _producerStorage.SearchByWorkflowStatusExt(0);
             }
             else
             {
                 result = _producerStorage.SearchByNameExt(term);

                 if (state == EditorsCommon.WorkFlowState.STATE_GROUP_IN_PROCESS)
                 {
                     result = from r in result where r.wfState <= EditorsCommon.WorkFlowState.READY_APPROVED select r;
                 }

                 ///
                 if (state == EditorsCommon.WorkFlowState.STATE_GROUP_PUBLISHED)
                 {
                     result = from r in result where r.wfState == EditorsCommon.WorkFlowState.PUBLISHED select r;
                 }
             }

             return Json(result, JsonRequestBehavior.AllowGet);
         }



         /// <summary>
         /// 
         /// </summary>
         /// <param name="term"></param>
         /// <returns></returns>
         [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
         [OutputCache(Duration = 0, VaryByParam = "none")]
         public ActionResult Approve(int id)
         {
             var result = _producerStorage.SetProducerStatus(id,WorkFlowState.PUBLISHED);
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
            var o = new JavaScriptSerializer().Deserialize<WineProducer>(str);

            var result = _producerStorage.Update(o);

             return Json(result, JsonRequestBehavior.AllowGet);
         }


         /// <summary>
         /// 
         /// </summary>
         /// <param name="term"></param>
         /// <returns></returns>
         [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameReviewer + "," + EditorsCommon.Constants.roleNameAdmin)]
         [OutputCache(Duration = 0, VaryByParam = "none")]
         public ActionResult SearchNameToShow(String name)
         {
             WineProducer result = null;

             if (!String.IsNullOrEmpty(name))
             {
                 name = name.Trim();
                 var list = _producerStorage.SearchByName(name);

                 foreach (var p in list)
                 {
                     if (name.CompareTo(p.nameToShow) == 0)
                     {
                         result = p;
                         break;
                     }
                 }

             }


             if (result == null)
             {
                 return Json(new { }, JsonRequestBehavior.AllowGet);
             }

             return Json(result , JsonRequestBehavior.AllowGet);
         }
    }
}