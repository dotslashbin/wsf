using EditorsCommon.Publication;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace ErpContent.Controllers
{
    public class AssignmentController : Controller
    {



        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private IAssignmentStorage _assignmentStorage;

        /// <summary>
        /// IOC container should provide implementation of IAssignmentStorage
        /// </summary>
        /// <param name="assignmentStorage"></param>
        public AssignmentController(IAssignmentStorage assignmentStorage)
        {
            _assignmentStorage = assignmentStorage;
        }
        
        //
        // GET: /Assignment/
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetNewAssignment()
        {

            var result = new AssignmentItem();

            result.submitDate = DateTime.Now;
            result.proofreadDate = DateTime.Now;
            result.approveDate = DateTime.Now; 

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetAssignmentsByIssue(int issueId)
        {
            var result = _assignmentStorage.SearchByIssue(issueId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult DeleteAssignment(int assignmentID)
        {

            bool result = _assignmentStorage.DeleteAssignment(assignmentID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetAssignmentsByIssueByUser(int issueId)
        {
            var id = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            //
            // select all issues for a user
            //
            var result = _assignmentStorage.SearchByUser(id);

            //
            // filter out only assignment for that issue
            //
            result = from r in result where r.issueId == issueId select r;


            //var result = _assignmentStorage.SearchByIssueByUser(issueId, id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetAssignmentsByUser(int publicationId, int state)
        {
            var id = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var result = _assignmentStorage.SearchByUser(id);

            if (publicationId > 0)
            {
                result = from r in result where r.publicationId == publicationId select r;
            }

            if (state != EditorsCommon.WorkFlowState.STATE_GROUP_ALL)
            {
                result = from r in result where EditorsCommon.WorkFlowState.IsInState(r.wfState, state) select r;
            }

            result = from r in result orderby r.submitDate descending select r;


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publicationId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetAssignmentsForEditor(int publicationId, int state)
        {
            var id = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var result = _assignmentStorage.SearchByUser(id);

            //
            // select only assignments where user is assigned as editot
            //
            result = from r in result where r.editor != null && r.editor.id == id select r;


            if (publicationId > 0)
            {
                result = from r in result where r.publicationId == publicationId select r;
            }

            if (state != EditorsCommon.WorkFlowState.STATE_GROUP_ALL)
            {
                result = from r in result where EditorsCommon.WorkFlowState.IsInState(r.wfState, state) select r;
            }

            result = from r in result orderby r.submitDate descending select r;


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="publicationId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetAssignmentsForProofreader(int publicationId, int state)
        {
            var id = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            var result = _assignmentStorage.SearchByUser(id);

            //
            // select only assignments where user is assigned as editot
            //
            result = from r in result where r.proofread != null && r.proofread.id == id select r;


            if (publicationId > 0)
            {
                result = from r in result where r.publicationId == publicationId select r;
            }

            if (state != EditorsCommon.WorkFlowState.STATE_GROUP_ALL)
            {
                result = from r in result where EditorsCommon.WorkFlowState.IsInState(r.wfState, state) select r;
            }

            result = from r in result orderby r.submitDate descending select r;


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor
            + "," + EditorsCommon.Constants.roleNameEditor
            + "," + EditorsCommon.Constants.roleNameSubEditor
            + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult AddAssignment(String str)
        {
            AssignmentItem o = new JavaScriptSerializer().Deserialize<AssignmentItem>(str);


            var result = _assignmentStorage.Create(o);

            // todo.  bulk does not work. stored procedure should be be changes
            //_assignmentStorage.AssignmentResources_AddActorBulk(result.id, o.actors);
            //
            //foreach (var actor in o.actors)
            //    _assignmentStorage.AssignmentResources_AddActor(result.id, actor); ;
            _assignmentStorage.AssignmentResources_AddActor(result.id, o.author);
            _assignmentStorage.AssignmentResources_AddActor(result.id, o.editor);
            _assignmentStorage.AssignmentResources_AddActor(result.id, o.proofread);



            // todo.  bulk does not work. stored procedure should be be changes
            //_assignmentStorage.AssignmentResources_AddDeadlineBulk(result.id, o.deadlines);
            //
            //foreach (var deadline in o.deadlines)
            //    _assignmentStorage.AssignmentResources_AddDeadline(result.id, deadline); 

            _assignmentStorage.AssignmentResources_AddDeadline(result.id, new AssignmentDeadline() { typeid = DeadlineType.submission, deadline = o.submitDate });
            _assignmentStorage.AssignmentResources_AddDeadline(result.id, new AssignmentDeadline() { typeid = DeadlineType.proofread, deadline = o.proofreadDate });
            _assignmentStorage.AssignmentResources_AddDeadline(result.id, new AssignmentDeadline() { typeid = DeadlineType.approve, deadline = o.approveDate }); 


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor
            + "," + EditorsCommon.Constants.roleNameEditor
            + "," + EditorsCommon.Constants.roleNameSubEditor
            + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult UpdateAssignment(String str)
        {
            AssignmentItem o = new JavaScriptSerializer().Deserialize<AssignmentItem>(str);


            var result = _assignmentStorage.Update(o);


            _assignmentStorage.AssignmenetDeleteAllResources(result.id);

            _assignmentStorage.AssignmentResources_AddActor(result.id, o.author);
            _assignmentStorage.AssignmentResources_AddActor(result.id, o.editor);
            _assignmentStorage.AssignmentResources_AddActor(result.id, o.proofread);



            _assignmentStorage.AssignmentResources_AddDeadline(result.id, new AssignmentDeadline() { typeid = DeadlineType.submission, deadline = o.submitDate });
            _assignmentStorage.AssignmentResources_AddDeadline(result.id, new AssignmentDeadline() { typeid = DeadlineType.proofread, deadline = o.proofreadDate });
            _assignmentStorage.AssignmentResources_AddDeadline(result.id, new AssignmentDeadline() { typeid = DeadlineType.approve, deadline = o.approveDate });


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor
            + "," + EditorsCommon.Constants.roleNameReviewer
            + "," + EditorsCommon.Constants.roleNameEditor
            + "," + EditorsCommon.Constants.roleNameSubEditor
            + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult SetAssignmentReady(int assignmentId)
        {

            _assignmentStorage.SetReady(assignmentId);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor
            + "," + EditorsCommon.Constants.roleNameReviewer
            + "," + EditorsCommon.Constants.roleNameEditor
            + "," + EditorsCommon.Constants.roleNameSubEditor
            + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult SetAssignmentApproved(int assignmentId)
        {

            _assignmentStorage.SetApproved(assignmentId);

            return Json(true, JsonRequestBehavior.AllowGet);
        }




    }
}