using EditorsCommon;
using EditorsCommon.Publication;
using Excel;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Data;
using System.Web.Profile; 

namespace ErpContent.Controllers
{
    public class TastingNoteController : Controller
    {

        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        private ITastingNoteStorage _storage;

        /// <summary>
        /// IOC container should provide implementation of IAssignmentStorage
        /// </summary>
        /// <param name="assignmentStorage"></param>
        public TastingNoteController(ITastingNoteStorage storage)
        {
            _storage = storage;
        }
        
        //
        // GET: /Assignment/
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        public ActionResult Index()
        {
            return View();
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetNotesByTastingEvent(int eventId)
        {

            var result = _storage.SearchTastingNoteByTastingEvent(eventId);

            //result = from r in result orderby r.producer select r;
            //
            // implicitly use IComparable<TastingNote>, so, notes would be sorted how it is defined
            // in  public int CompareTo(TastingNote other) method of the TastingNote
            //
            result = from r in result orderby r select r;


            return Json(result, JsonRequestBehavior.AllowGet);

        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetNotesByVinN(int vinN)
        {

            var result = _storage.SearchTastingNoteByVinN(vinN);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetNotesByWineN(int wineN)
        {

            var result = _storage.SearchTastingNoteByWineN(wineN);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetNotesByProducerN(int producerN)
        {

            var result = _storage.SearchTastingNoteByProducerN(producerN);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [HttpPost]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetNewNoteForEvent(int eventId, string vin = null,string vintage = null)
        {

            var result = new TastingNote() { tastingEventId = eventId, tastingDate = DateTime.Now };

            if (vin != null)
            {
                var v = new JavaScriptSerializer().Deserialize<VinN>(vin);

                result.producer = v.producer;
                result.wineName = v.label;

                result.country = v.country;
                result.region = v.region;
                result.location = v.location;
                result.locale = v.locale;
                result.site = v.site;

                result.variety = v.variety;
                result.color = v.colorClass;
                result.dryness = v.dryness;

                result.wineType = v.wineType;
            }
            else
            {
                result.producer = "";
                result.wineName = "";

                result.country = "";
                result.region = "";
                result.location = "";
                result.locale = "";
                result.site = "";

                result.variety = "";
                result.color = "";
                result.dryness = "";

                result.wineType = "";
            }

            if (vintage != null)
            {
                result.vintage = vintage;
            }
            else
            {
                result.vintage = "";
            }



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
        public ActionResult SetTastingNoteState(int noteId, int stateId)
        {

            var result = _storage.SetNoteState(noteId, stateId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private static int tmpNoteId = 0;
        private static string cartSessionName = "tastingNodeCart";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult AddTastingNoteToCart(String str)
        {
            var tn = new JavaScriptSerializer().Deserialize<TastingNote>(str);
            tn.id = tmpNoteId++;

            List<TastingNote> cart = Session[cartSessionName] as List<TastingNote>;

            if (cart == null)
            {
                cart = new List<TastingNote>();
                Session[cartSessionName] = cart;
            }

            cart.Add(tn);

            return Json(tn, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult GetTastingNoteCart()
        {
            List<TastingNote> cart = Session[cartSessionName] as List<TastingNote>;

            if (cart == null)
            {
                cart = new List<TastingNote>();
                Session[cartSessionName] = cart;
            }

            return Json(cart, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult DeleteTastingNoteFromCart(String str)
        {
            var tn = new JavaScriptSerializer().Deserialize<TastingNote>(str);

            List<TastingNote> cart = Session[cartSessionName] as List<TastingNote>;
            if (cart == null || cart.RemoveAll(t => t.id == tn.id) != 1)
            {
                tn = null;
            }

            return Json(tn, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult DeleteAllTastingNotesFromCart()
        {
            List<TastingNote> cart = Session[cartSessionName] as List<TastingNote>;
            if (cart != null)
            {
                cart.Clear();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult AddTastingNote(String str)
        {
            return Json(this.CreateNewTastingNoteRecord(str), JsonRequestBehavior.AllowGet);
        }

        /**
         * Create a new  tasting note record
         * 
         * @param       String      stringOject         String representation of a note
         * @param       Boolean     isImport            This is a boolean value to toggle if this call is being called by the import process, or not. 
         */
        private TastingNote CreateNewTastingNoteRecord(string stringObject, Boolean isImport = false) {

            var objectToEvaluate = new JavaScriptSerializer().Deserialize<TastingNote>(stringObject);


            if (!isImport)
            {
                var maturityId = objectToEvaluate.maturityId;
                int maturityIdValue = 5;

                switch (maturityId.ToString())
                {
                    case "Young":
                        maturityIdValue = 0;
                        break;
                    case "Early":
                        maturityIdValue = 1;
                        break;
                    case "Mature":
                        maturityIdValue = 2;
                        break;
                    case "Late":
                        maturityIdValue = 3;
                        break;
                    case "Old":
                        maturityIdValue = 4;
                        break;
                }

                objectToEvaluate.maturityId = maturityIdValue;
            }



            objectToEvaluate.userId = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            var tastingNote = _storage.Create(objectToEvaluate);

            return tastingNote; 
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult DeleteTastingNote(String str)
        {
            var o = new JavaScriptSerializer().Deserialize<TastingNote>(str);

            //o.userId = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;

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
        public ActionResult UpdateTastingNote(String str)
        {
            var o = new JavaScriptSerializer().Deserialize<TastingNote>(str);

           // o.userId = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            //var maturityId = o.maturityId;

            //int maturityIdValue = 5;

            //switch (maturityId.ToString())
            //{
            //    case "Young":
            //        maturityIdValue = 0;
            //        break;
            //    case "Early":
            //        maturityIdValue = 1;
            //        break;
            //    case "Mature":
            //        maturityIdValue = 2;
            //        break;
            //    case "Late":
            //        maturityIdValue = 3;
            //        break;
            //    case "Old":
            //        maturityIdValue = 4;
            //        break;
            //}

            //o.maturityId = maturityIdValue; 


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
        public ActionResult UpdateInPlaceTastingNote(String str)
        {
            var o = new JavaScriptSerializer().Deserialize<TastingNote>(str);

            var result = _storage.UpdateInPlace(o);

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult ImportTastingNotes(FormCollection formCollection)
        {

            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["notes"]; 

                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    // Saving file into system
                    string fileName = file.FileName;
                    var path = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                    file.SaveAs(path);
                    ModelState.Clear();

                    // Reading excel file
                    FileStream stream = System.IO.File.Open(Server.MapPath("~/App_Data/" + fileName), FileMode.Open, FileAccess.Read);

                    IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

                    // Close stream and delete file after reading
                    stream.Close();
                    System.IO.File.Delete(Server.MapPath("~/App_Data/" + fileName)); 

                    excelReader.IsFirstRowAsColumnNames = true; 
                    DataSet result = excelReader.AsDataSet();

                    int numberOfRows = result.Tables[0].Rows.Count; 

                    TastingNote newNote = new TastingNote();

                    int counter = 0;

                    TastingNote createResult = new TastingNote();

                    var tastingNotesCollection = new List<TastingNote>(); 

                    
                    while (excelReader.Read())
                    {
                        if (counter > 0)
                        {
                            newNote.producer            = excelReader.GetString(1);
                            newNote.wineName            = excelReader.GetString(2);
                            newNote.vintage             = excelReader.GetString(3);
                            newNote.color               = excelReader.GetString(4);
                            newNote.rating              = excelReader.GetString(5);
                            newNote.drinkDateLo         = excelReader.GetDateTime(6); 
                            newNote.drinkDateHi         = excelReader.GetDateTime(7);
                            newNote.variety             = excelReader.GetString(8);
                            newNote.dryness             = excelReader.GetString(9);
                            newNote.tastingDate         = excelReader.GetDateTime(10);
                            newNote.estimatedCost       = excelReader.GetString(11);
                            newNote.estimatedCostHi     = excelReader.GetString(12); 
                            newNote.maturityId          = excelReader.GetInt32(13);
                            newNote.note                = excelReader.GetString(14);
                            var isBarrelTasting         = excelReader.GetInt32(15);
                            newNote.isBarrelTasting     = (isBarrelTasting == 1) ? true : false;

                            newNote.country             = excelReader.GetString(16);
                            newNote.region              = excelReader.GetString(17);
                            newNote.location            = excelReader.GetString(18);
                            newNote.locale              = excelReader.GetString(19);
                            newNote.site                = excelReader.GetString(20);  
                            newNote.variety             = excelReader.GetString(21);
                            newNote.wineType            = excelReader.GetString(22); 


                            newNote.userId              = 0;
                            newNote.noteId              = 0;

                            newNote.wineN               = 0; 
                            newNote.tastingEventId      = Convert.ToInt32(Request.Form["tastingEventID"]); 
                            newNote.vinN                = 0;
                            newNote.tastingN            = 0;
                            newNote.wfState             = 0;
                            newNote.wfStateWineN        = 0;
                            newNote.wfStateVinN         = 0; 

                            int UserID = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey; 

                            MembershipUser membershipProvider = ((ERP.Providers.SqlMembershipProvider)Membership.Provider).GetUser(UserID, false);
                            ProfileBase profile = ProfileBase.Create(membershipProvider.UserName);

                            newNote.reviewer = (string)profile["FirstName"] + " " + (string)profile["LastName"]; 
    
                            string stringifiedObject = new JavaScriptSerializer().Serialize(newNote);


                            /**
                             * VALIDATION: 
                             * 
                             * Validate if there is a producer and a wine name. If there is , then  it means
                             * it can proceed to add, otherwise not. 
                             * 
                             * TODO: figure out a way to implement this better
                             */
                            

                            if ((newNote.producer != "" || newNote.producer != null) && (newNote.wineName != "" || newNote.wineName != null) && (newNote.rating != null || newNote.rating != "") )
                            {
                                var newTastingNote = this.CreateNewTastingNoteRecord(stringifiedObject, true);

                                tastingNotesCollection.Add(newTastingNote); 
                            }
                            
                        }

                        counter++; 
                    }

                    excelReader.Close();

                    // return RedirectToAction("Index", "Reviewer");

                    return Json(tastingNotesCollection, JsonRequestBehavior.AllowGet); 
                    
                }
                else
                {
                    return Json(null); 
                }
            }
            else
            {
                return Json(null); 
            }
        }

        /**
         *  This method will be responsible for providing a downloadable excel template 
         *  to the user. 
         */
        public ActionResult downloadImportTemplate()
        {
            return File("~/App_Data/downloadables/template.xls", "application/force-download", Path.GetFileName("~/App_Data/downloadables/template.xls")); 
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [HttpPost]
        public ActionResult MoveTastingNote(int tastingEventId, int tastingNoteId)
        {

            var result = _storage.MoveTastingNote(tastingEventId,tastingNoteId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetInQueueCount()
        {
            var result = _storage.GetInQueueCount();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAll)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetInQueue()
        {

            var result = _storage.GetInQueue();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.Authorize(Roles = EditorsCommon.Constants.roleNameAdmin)]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult Reload()
        {
            _storage.PublishFromQueue();
            return Json(true, JsonRequestBehavior.AllowGet);
        }


	}
}