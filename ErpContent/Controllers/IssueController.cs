using EditorsCommon;
using EditorsCommon.Publication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Xml;

namespace ErpContent.Controllers
{
    public class IssueController : Controller
    {

        /// <summary>
        /// 
        /// </summary>
        private IIssueStorage _issueStorage;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueStorage"></param>
        public IssueController(IIssueStorage issueStorage)
        {
            _issueStorage = issueStorage;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="publicationId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Issues(int publicationId = 0, int state = EditorsCommon.WorkFlowState.STATE_GROUP_ALL)
        {
            Issue filter = publicationId == 0 ? null : new Issue() { publicationID = publicationId };

            var result = _issueStorage.Search(filter);

            //
            // apply filter by status
            //
            if (state != EditorsCommon.WorkFlowState.STATE_GROUP_ALL)
            {
                result = from r in result where EditorsCommon.WorkFlowState.IsInState(r.wfState, state) select r;
            }
            //
            // sort by create date
            //
            result = from r in result orderby r.createdDate descending select r;


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
        public ActionResult IssuesForUser(int publicationId = 0, int state = EditorsCommon.WorkFlowState.STATE_GROUP_ALL)
        {
            // for now, this method will do that same as Issues, but in future we might filter out the issue
            // which do not have references to an user
            //
            int userId = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            var result = _issueStorage.GetIssuesForUser( userId);

            //
            // apply filter by status
            //
            if (state != EditorsCommon.WorkFlowState.STATE_GROUP_ALL)
            {
                result = from r in result where EditorsCommon.WorkFlowState.IsInState(r.wfState, state) select r;
            }

            //
            // apply filter by publication
            //
            if (publicationId != 0)
            {
                result = from r in result where  r.publicationID == publicationId select r;
            }

            //
            // sort by create date
            //
            result = from r in result orderby r.createdDate descending select r;


            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult AddIssue(String str)
        {
            var o = new JavaScriptSerializer().Deserialize<Issue>(str);
            o.chiefEditorId = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            //
            // 
            //
            var result = _issueStorage.Create(o);

            // combination of Create->GetByID looks weird, but I do not know at the moment
            // how to get PublicationName for newly created issue and GetByID is easiest way
            // to achieve it.
            //
            //
            result = _issueStorage.GetByID(result.id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult DeleteIssue(String str)
        {
            var o = new JavaScriptSerializer().Deserialize<Issue>(str);
            var result = _issueStorage.Delete( o);

            return Json(result, JsonRequestBehavior.AllowGet);
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult EditIssue(String str)
        {
            var o = new JavaScriptSerializer().Deserialize<Issue>(str);
            o.chiefEditorId = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            //
            // 
            //
            var result = _issueStorage.Update(o);

            // combination of Create->GetByID looks weird, but I do not know at the moment
            // how to get PublicationName for newly created issue and GetByID is easiest way
            // to achieve it.
            //
            //
            result = _issueStorage.GetByID(result.id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetNewIssue()
        {

            var result = new Issue() { createdDate  = DateTime.Now };

            //result.proofreadDate = DateTime.Now;
            result.publicationDate = DateTime.Now; 

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [HttpPost]
        [OutputCache(Duration = 5, VaryByParam = "none")]
        public ActionResult Export(int id)
        {

            var result = _issueStorage.LoadIssueComplete(id,  60 );

            Response.AddHeader("Content-Disposition", "attachment; filename=issue" + id + ".html");
            Response.ContentType = " text/html;";




            foreach (var a in result.assignments)
            {
                if (a.tastingEvents != null && a.tastingEvents.Count() > 0)
                {
                    a.tastingEvents = from e in a.tastingEvents orderby e.title select e;
                }
            }


            return View("Export2Doc", result);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [HttpPost]
        [OutputCache(Duration = 5, VaryByParam = "none")]
        public ActionResult ExportAssignmentAdmin(int issueId, int assignmentId)
        {

            var result = _issueStorage.LoadIssueComplete(issueId, -1);


            foreach (var a in result.assignments)
            {
                if (a.tastingEvents != null && a.tastingEvents.Count() > 0)
                {
                    a.tastingEvents = from e in a.tastingEvents orderby e.title select e;
                }
            }


            result.assignments = from a in result.assignments where a.id == assignmentId select a;


            Response.AddHeader("Content-Disposition", "attachment; filename=issue" + issueId + "_" + assignmentId + ".html");
            Response.ContentType = " text/html;";

            return View("Export2Doc", result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [HttpPost]
        [OutputCache(Duration = 5, VaryByParam = "none")]
        public ActionResult ExportAssignment(int issueId,int assignmentId)
        {

            var result = _issueStorage.LoadIssueComplete(issueId,-1);


            foreach (var a in result.assignments)
            {
                if (a.tastingEvents != null && a.tastingEvents.Count() > 0)
                {
                    a.tastingEvents = from e in a.tastingEvents orderby e.title select e;
                }
            }


            result.assignments = from a in result.assignments where a.id == assignmentId select a; 


            Response.AddHeader("Content-Disposition", "attachment; filename=issue" + issueId + "_" + assignmentId + ".html");
            Response.ContentType = " text/html;";

            return View("ExportAssignment2Doc", result);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [HttpPost]
        [OutputCache(Duration = 5, VaryByParam = "id")]
        public ActionResult Export2Xml(int id)
        {

            var result = _issueStorage.LoadIssueComplete(id,60);

            XmlDocument xml = new XmlDocument();
            XmlNode issue =  xml.AppendChild(xml.CreateElement("issue"));

            issue.Attributes.Append(xml.CreateAttribute("workflow-state")).Value = result.wfState.ToString();
            issue.Attributes.Append(xml.CreateAttribute("id")).Value = result.id.ToString();
            issue.AppendChild(xml.CreateElement("publication")).AppendChild(xml.CreateTextNode(result.publicationName));
            issue.AppendChild(xml.CreateElement("publication-id")).AppendChild(xml.CreateTextNode(result.publicationID.ToString()));
            issue.AppendChild(xml.CreateElement("issue-author")).AppendChild(xml.CreateTextNode(result.chiefEditorName));



            issue.AppendChild(xml.CreateElement("issue-author-id")).AppendChild(xml.CreateTextNode(result.chiefEditorId.ToString()));
            issue.AppendChild(xml.CreateElement("title")).AppendChild(xml.CreateTextNode(result.title));
            issue.AppendChild(xml.CreateElement("notes")).AppendChild(xml.CreateTextNode(result.Notes));

            issue.AppendChild(xml.CreateElement("create-date")).AppendChild(xml.CreateTextNode(result.createdDate.ToShortDateString()));
            issue.AppendChild(xml.CreateElement("publication-date")).AppendChild(xml.CreateTextNode(result.publicationDate.ToShortDateString()));




            foreach (var assignment in result.assignments)
            {

                var assignmentNode = issue.AppendChild(xml.CreateElement("assignment"));

                assignmentNode.Attributes.Append(xml.CreateAttribute("workflow-state")).Value = assignment.wfState.ToString();
                assignmentNode.AppendChild(xml.CreateElement("title")).AppendChild(xml.CreateTextNode(String.IsNullOrEmpty(assignment.title) ? "" : assignment.title));

                
                assignmentNode.AppendChild(xml.CreateElement("author")).AppendChild(xml.CreateTextNode(assignment.author != null ? assignment.author.name : ""));
                assignmentNode.AppendChild(xml.CreateElement("author-id")).AppendChild(xml.CreateTextNode(assignment.author != null ? assignment.author.id.ToString() : "0"));


                
                
                assignmentNode.AppendChild(xml.CreateElement("editor")).AppendChild(xml.CreateTextNode(assignment.editor != null ? assignment.editor.name : ""));
                assignmentNode.AppendChild(xml.CreateElement("editor-id")).AppendChild(xml.CreateTextNode(assignment.editor != null ? assignment.editor.id.ToString() : "0"));


                
                assignmentNode.AppendChild(xml.CreateElement("proofreader")).AppendChild(xml.CreateTextNode(assignment.proofread.name != null ? assignment.proofread.name : ""));
                assignmentNode.AppendChild(xml.CreateElement("proofreader-id")).AppendChild(xml.CreateTextNode(assignment.proofread.name != null ? assignment.proofread.id.ToString() : "0"));


                if (assignment.tastingEvents != null)
                {
                    foreach (var tastingRecord in assignment.tastingEvents)
                    {
                        var tastingRecordNode = assignmentNode.AppendChild(xml.CreateElement("tasting-record"));
                        tastingRecordNode.AppendChild(xml.CreateElement("title")).AppendChild(xml.CreateTextNode(tastingRecord.title));
                        tastingRecordNode.AppendChild(xml.CreateElement("location")).AppendChild(xml.CreateTextNode(tastingRecord.location));
                        tastingRecordNode.AppendChild(xml.CreateElement("comments")).AppendChild(xml.CreateTextNode(tastingRecord.comments));


                        if (tastingRecord.tastingNotes != null)
                        {
                            foreach (var tastingNote in tastingRecord.tastingNotes)
                            {
                                var tastingNoteNode = tastingRecordNode.AppendChild(xml.CreateElement("tasting-note"));


                                tastingNoteNode.Attributes.Append(xml.CreateAttribute("is-barrel-tasting")).Value = tastingNote.isBarrelTasting.ToString();
                                tastingNoteNode.Attributes.Append(xml.CreateAttribute("workflow-state")).Value = tastingNote.wfState.ToString();

                                var reviewer = tastingNoteNode.AppendChild(xml.CreateElement("reviewer"));
                                reviewer.AppendChild(xml.CreateTextNode(tastingNote.reviewer));



                                reviewer.Attributes.Append(xml.CreateAttribute("reviewer-id")).Value = tastingNote.userId.ToString();



                                tastingNoteNode.AppendChild(xml.CreateElement("note")).AppendChild(xml.CreateTextNode(tastingNote.note));
                                tastingNoteNode.AppendChild(xml.CreateElement("producer")).AppendChild(xml.CreateTextNode(tastingNote.producer));
                                tastingNoteNode.AppendChild(xml.CreateElement("wine-name")).AppendChild(xml.CreateTextNode(tastingNote.wineName));


                                tastingNoteNode.AppendChild(xml.CreateElement("rating-low")).AppendChild(xml.CreateTextNode(tastingNote.ratingLo.ToString()));
                                tastingNoteNode.AppendChild(xml.CreateElement("rating-high")).AppendChild(xml.CreateTextNode(tastingNote.ratingHi.ToString()));

                                tastingNoteNode.AppendChild(xml.CreateElement("estimated-cost")).AppendChild(xml.CreateTextNode(tastingNote.estimatedCost));
                                tastingNoteNode.AppendChild(xml.CreateElement("estimated-cost-high")).AppendChild(xml.CreateTextNode(tastingNote.estimatedCostHi));

                                tastingNoteNode.AppendChild(xml.CreateElement("country")).AppendChild(xml.CreateTextNode(tastingNote.country));
                                tastingNoteNode.AppendChild(xml.CreateElement("region")).AppendChild(xml.CreateTextNode(tastingNote.region));
                                tastingNoteNode.AppendChild(xml.CreateElement("location")).AppendChild(xml.CreateTextNode(tastingNote.location));
                                tastingNoteNode.AppendChild(xml.CreateElement("locale")).AppendChild(xml.CreateTextNode(tastingNote.locale));
                                tastingNoteNode.AppendChild(xml.CreateElement("site")).AppendChild(xml.CreateTextNode(tastingNote.site));

                                tastingNoteNode.AppendChild(xml.CreateElement("color")).AppendChild(xml.CreateTextNode(tastingNote.color));
                                tastingNoteNode.AppendChild(xml.CreateElement("dryness")).AppendChild(xml.CreateTextNode(tastingNote.dryness));
                                tastingNoteNode.AppendChild(xml.CreateElement("variety")).AppendChild(xml.CreateTextNode(tastingNote.variety));
                                tastingNoteNode.AppendChild(xml.CreateElement("wineType")).AppendChild(xml.CreateTextNode(tastingNote.wineType));
                                tastingNoteNode.AppendChild(xml.CreateElement("vintage")).AppendChild(xml.CreateTextNode(tastingNote.vintage));

                                tastingNoteNode.AppendChild(xml.CreateElement("drink-from")).AppendChild(xml.CreateTextNode(tastingNote.drinkDateLo.ToShortDateString()));
                                tastingNoteNode.AppendChild(xml.CreateElement("drink-to")).AppendChild(xml.CreateTextNode(tastingNote.drinkDateHi.ToShortDateString()));
                                tastingNoteNode.AppendChild(xml.CreateElement("tasting-date")).AppendChild(xml.CreateTextNode(tastingNote.tastingDate.ToShortDateString()));

                            }
                        }
                    }
                }

            }



            MemoryStream ms = new MemoryStream();
            xml.Save(ms);
            return File(new MemoryStream(ms.ToArray()), "text/xml", "issue_"+id+".xml");

        }


        private int ProdId2StageId(int id)
        {
            switch (id)
            {

                case 1066597:
                    return 1063518;
                case 1066598:
                    return 1063524;
                case 1067242:
                    return 1063532;
                case 1067986:
                    return 1063533;
                case 1074793:
                    return 1063525;
                case 1074948:
                    return 1063581;

            }
            return id;
        }

        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [HttpPost]
        public JsonResult Upload()
        {
            XmlDocument xml = new XmlDocument();
            var issueObject = new IssueComplete();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;

                xml.Load(fileContent);

                var issue = xml.FirstChild;

                issueObject.wfState = short.Parse(issue.Attributes["workflow-state"].Value);
                issueObject.publicationName = issue["publication"].InnerText;
                issueObject.title = issue["title"].InnerText + "_uploaded";
                issueObject.Notes = issue["notes"].InnerText;
                issueObject.publicationName = issue["publication"].InnerText;
                issueObject.publicationID = Int32.Parse(issue["publication-id"].InnerText);
                issueObject.chiefEditorName = issue["issue-author"].InnerText;

                issueObject.chiefEditorId = ProdId2StageId(  Int32.Parse(issue["issue-author-id"].InnerText));

                issueObject.createdDate = DateTime.Parse(issue["create-date"].InnerText);
                issueObject.publicationDate = DateTime.Parse(issue["publication-date"].InnerText);

                
                issueObject.assignments = new List<AssignmentItemComplete>();

                foreach (XmlNode assignment in issue.SelectNodes("assignment"))
                {
                    var assignmentObject = new AssignmentItemComplete();
                    assignmentObject.tastingEvents = new List<TastingEventComplete>();

                    ((List<AssignmentItemComplete>)issueObject.assignments).Add(assignmentObject);

                    assignmentObject.wfState = short.Parse(assignment.Attributes["workflow-state"].Value);
                    assignmentObject.title = assignment["title"].InnerText;
                    var name = assignment["author"].InnerText;

                    if( !string.IsNullOrEmpty(name)){
                        assignmentObject.ProcessActor(ProdId2StageId(Int32.Parse(assignment["author-id"].InnerText)), ActorRole.reviewer, name);
                    }

                    name = assignment["editor"].InnerText;
                    if (!string.IsNullOrEmpty(name))
                    {
                        assignmentObject.ProcessActor(ProdId2StageId(Int32.Parse(assignment["editor-id"].InnerText)), ActorRole.editor, name);
                    }

                    name = assignment["proofreader"].InnerText;
                    if (!string.IsNullOrEmpty(name))
                    {
                        assignmentObject.ProcessActor(ProdId2StageId(Int32.Parse(assignment["proofreader-id"].InnerText)), ActorRole.proofread, name);
                    }


                    foreach (XmlNode tastingRecord in assignment.SelectNodes("tasting-record"))
                    {
                        var tastingRecordObject = new TastingEventComplete();
                        tastingRecordObject.tastingNotes = new List<TastingNote>();

                        ((List<TastingEventComplete>)assignmentObject.tastingEvents).Add(tastingRecordObject);

                        tastingRecordObject.title = tastingRecord["title"].InnerText;
                        tastingRecordObject.location = tastingRecord["location"].InnerText;
                        tastingRecordObject.comments = tastingRecord["comments"].InnerText;

                        foreach (XmlNode tastingNote in tastingRecord.SelectNodes("tasting-note"))
                        {

                            var tastingNoteObject = new TastingNote();
                            ((List<TastingNote>)tastingRecordObject.tastingNotes).Add(tastingNoteObject);


                            tastingNoteObject.isBarrelTasting = bool.Parse(tastingNote.Attributes["is-barrel-tasting"].Value);
                            tastingNoteObject.wfState = Int32.Parse(tastingNote.Attributes["workflow-state"].Value);

                            tastingNoteObject.reviewer = tastingNote["reviewer"].InnerText;
                            tastingNoteObject.userId = ProdId2StageId( Int32.Parse( tastingNote["reviewer"].Attributes["reviewer-id"].Value));

                            tastingNoteObject.note = tastingNote["note"].InnerText;
                            tastingNoteObject.producer = tastingNote["producer"].InnerText;
                            tastingNoteObject.wineName = tastingNote["wine-name"].InnerText;

                            tastingNoteObject.ratingLo = short.Parse(tastingNote["rating-low"].InnerText);
                            tastingNoteObject.ratingHi = short.Parse(tastingNote["rating-high"].InnerText);

                            tastingNoteObject.estimatedCost = tastingNote["estimated-cost"].InnerText;
                            tastingNoteObject.estimatedCostHi = tastingNote["estimated-cost-high"].InnerText;

                            tastingNoteObject.country = tastingNote["country"].InnerText;
                            tastingNoteObject.region = tastingNote["region"].InnerText;
                            tastingNoteObject.location = tastingNote["location"].InnerText;
                            tastingNoteObject.locale = tastingNote["locale"].InnerText;
                            tastingNoteObject.site = tastingNote["site"].InnerText;

                            tastingNoteObject.color = tastingNote["color"].InnerText;
                            tastingNoteObject.dryness = tastingNote["dryness"].InnerText;
                            tastingNoteObject.variety = tastingNote["variety"].InnerText;
                            tastingNoteObject.wineType = tastingNote["wineType"].InnerText;
                            tastingNoteObject.vintage = tastingNote["vintage"].InnerText;

                            tastingNoteObject.drinkDateLo = DateTime.Parse(tastingNote["drink-from"].InnerText);
                            //
                            // it was an error in the initial version
                            //
                            //tastingNoteObject.drinkDateLo = DateTime.Parse(tastingNote["drink-to"].InnerText);
                            tastingNoteObject.drinkDateHi = DateTime.Parse(tastingNote["drink-to"].InnerText);
                            tastingNoteObject.tastingDate = DateTime.Parse(tastingNote["tasting-date"].InnerText);

                        }

                    }

                }


                _issueStorage.AddIssueComplete(issueObject);

            }
            return Json("Uploaded " + Request.Files.Count + " files, root element : " + xml.FirstChild.Name + " id :" + xml.FirstChild.Attributes["id"].Value);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [HttpPost]
        [OutputCache(Duration = 5, VaryByParam = "none")]
        public ActionResult IssueInfo(int id)
        {

            // R&D not completed yet
            var result = _issueStorage.LoadIssueComplete(id,60);

            int notesTotal = 0;
            int notesPublished = 0;
            int notesApproved = 0;
            int notesWaitingForApproval = 0;

            foreach (var a in result.assignments)
            {
                if (a.tastingEvents != null)
                {
                    foreach (var tr in a.tastingEvents)
                    {

                        if (tr.tastingNotes != null)
                        {
                            foreach (var tn in tr.tastingNotes)
                            {
                                notesTotal++;

                                if (tn.wfState == WorkFlowState.PUBLISHED)
                                {
                                    notesPublished++;
                                    continue;
                                }

                                if (tn.wfState == WorkFlowState.READY_APPROVED)
                                {
                                    notesApproved++;
                                    continue;
                                }

                                if (tn.wfState < WorkFlowState.READY_APPROVED)
                                {
                                    notesWaitingForApproval++;
                                    continue;
                                }

                            }
                        }
                    }
                }
            }


            var o = new
            {
                publication = result.publicationName,
                author = result.chiefEditorName,
                title = result.title,
                notesTotal = notesTotal,
                notesPublished = notesPublished,
                notesApproved = notesApproved,
                notesWaitingForApproval = notesWaitingForApproval
            };


            return Json(o, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [HttpPost]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Publish(int id)
        {

             _issueStorage.Publish(id);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameChiefEditor + "," + EditorsCommon.Constants.roleNameAdmin)]
        [HttpPost]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult RollbackPublish(int id)
        {

            _issueStorage.RollbackPublish(id);

            return Json(true, JsonRequestBehavior.AllowGet);
        }


	}
}