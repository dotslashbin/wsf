using EditorsCommon;
using EditorsCommon.Publication;
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
    public class TastingEventController : Controller
    {

        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private ITastingEventStorage _storage;

        /// <summary>
        /// IOC container should provide implementation of IAssignmentStorage
        /// </summary>
        /// <param name="assignmentStorage"></param>
        public TastingEventController(ITastingEventStorage storage)
        {
            _storage = storage;
        }

        
        
        //
        // GET: /Assignment/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetNewTastingEventToAssignment(int assignmentId)
        {

            var result = new TastingEvent() { assignmentId = assignmentId};

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetTastingEventByAssignment(int assignmentId)
        {

            var result = _storage.SearchTastingEventByAssignment(assignmentId);

            result = from r in result orderby r.title select r;


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult AddTastingEvent(String str)
        {
            var o = new JavaScriptSerializer().Deserialize<TastingEvent>(str);

            var result = _storage.Create(o);


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult EditTastingEvent(String str)
        {

            var o = new JavaScriptSerializer().Deserialize<TastingEvent>(str);

            var result = _storage.Update(o);


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult DeleteTastingEvent(String str)
        {
            var o = new JavaScriptSerializer().Deserialize<TastingEvent>(str);

            var result = _storage.Delete(o);


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult MoveTastingEvent(int assignmentId, int tastingEventId)
        {

            _storage.MoveToAssingment(tastingEventId, assignmentId);

            return Json(true, JsonRequestBehavior.AllowGet);
        }


	}
}