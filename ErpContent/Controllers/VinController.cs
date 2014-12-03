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
    public class VinController : Controller
    {

       private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

         private IVinStorage  _vinStorage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vinStorage"></param>
         public VinController(IVinStorage vinStorage)
         {
            _vinStorage = vinStorage;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <param name="p"></param>
        /// <returns></returns>
         [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
         [OutputCache(Duration = 0, VaryByParam = "none")]
         public ActionResult SearchWineLabel(String term, String p)
         {
             return Json(_vinStorage.SearchLabelProducer(p, term), JsonRequestBehavior.AllowGet);
         }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
         [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
         [OutputCache(Duration = 0, VaryByParam = "none")]
         public ActionResult SearchProducer(String term)
         {

             return Json(_vinStorage.SearchProducer(term), JsonRequestBehavior.AllowGet);
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
         [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
         [OutputCache(Duration = 0, VaryByParam = "none")]
         public ActionResult SearchProducerExtended(String term)
         {

             return Json(_vinStorage.SearchProducerExtended(term), JsonRequestBehavior.AllowGet);
         }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="r"></param>
        /// <param name="l"></param>
        /// <param name="lc"></param>
        /// <param name="s"></param>
        /// <returns></returns>
         [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
         [OutputCache(Duration = 0, VaryByParam = "none")]
         public ActionResult SearchLocation(String c, String r, String l, String lc, String s)
         {
             return Json(_vinStorage.SearchLocation(c, r, l, lc, s), JsonRequestBehavior.AllowGet);
         }



         /// <summary>
         /// 
         /// </summary>
         /// <param name="term"></param>
         /// <returns></returns>
         [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
         [OutputCache(Duration = 0, VaryByParam = "none")]
         public ActionResult SearchWineN(String term, int state = EditorsCommon.WorkFlowState.STATE_GROUP_ALL)
         {

             IEnumerable<VinN> result;

             if (String.IsNullOrEmpty(term) && state == EditorsCommon.WorkFlowState.STATE_GROUP_IN_PROCESS)
             {
                 result = _vinStorage.SearchNewWineN();
             }
             else
             {
                 result = _vinStorage.SearchWineN(term);

                 if (state == EditorsCommon.WorkFlowState.STATE_GROUP_IN_PROCESS)
                 {
                     result = from r in result where r.HasState(EditorsCommon.WorkFlowState.STATE_GROUP_IN_PROCESS) select r;
                 }

                 ///
                 if (state == EditorsCommon.WorkFlowState.STATE_GROUP_PUBLISHED)
                 {
                     result = from r in result where r.HasState(EditorsCommon.WorkFlowState.STATE_GROUP_PUBLISHED) select r;
                 }
             }

             //return Json(result, JsonRequestBehavior.AllowGet);

             var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
             jsonResult.MaxJsonLength = 3000000;
             return jsonResult;

         }

         /// <summary>
         /// 
         /// </summary>
         /// <param name="term"></param>
         /// <returns></returns>
         [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
         [OutputCache(Duration = 0, VaryByParam = "none")]
         public ActionResult ApproveVinN(int id)
         {
             var result = _vinStorage.ApproveVin(id);

             return Json(result, JsonRequestBehavior.AllowGet);
         }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag">flag will define set of bit and they define how similar VINs should  be</param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
         [OutputCache(Duration = 0, VaryByParam = "none")]
         public ActionResult LoadSimilar(int flag)
         {
             var result = _vinStorage.LoadSimilar(flag);

             return Json(result, JsonRequestBehavior.AllowGet);
         }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag">flag will define set of bit and they define how similar VINs should  be</param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Update(string  str)
        {
            var o = new JavaScriptSerializer().Deserialize<VinN>(str);

            var result = _vinStorage.Update(o);

            return Json(result, JsonRequestBehavior.AllowGet);
        }






	}
}