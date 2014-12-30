using EditorsCommon;
using EditorsDbLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace ErpContent.Controllers
{
    public class ArticleController : Controller
    {
        private IArticleStorage _articleStorage;

        public ArticleController(IArticleStorage storageObject)
        {
            _articleStorage = storageObject; 
        }

        //
        // GET: /Article/
        public ActionResult Index()
        {
            return View();
        }


        /**
         * Executes the process of adding an article into the database
         * 
         * @param       String      article         JSON string representing the article object
         * @param       Int         assignmentID    Assignment ID related to the article
         * 
         * @return      JSON        newArticle      Article object
         * 
         * @author: Joshua Fuentes <joshua.fuentes@robertparker.com>
         */
        [HttpPost]
        public ActionResult AddArticle(String article, int assignmentID)
        {
            Article newArticle = new JavaScriptSerializer().Deserialize<Article>(article);

            newArticle.authorId = (int)Membership.GetUser(User.Identity.Name).ProviderUserKey;

            newArticle = _articleStorage.Create(newArticle);

            bool result = _articleStorage.AddArticleToAssignment(Convert.ToInt32(newArticle.id), Convert.ToInt32(assignmentID));

            if (result == false)
            {
                return null;
            }
            
            return Json(newArticle); 
        }

        public ActionResult DeleteArticle(int ID)
        {

            Article article = new Article(); 

            _articleStorage.Delete(article); 

            return Json(article.id, JsonRequestBehavior.AllowGet); 
        }

        /**
         * Returns a JSON string, representing a collection of objects that
         * are articles.
         * 
         * @return      JSON        JSON object a collection of article objects
         * 
         * @author: Joshua Fuentes <joshua.fuentes@robertparker.com>
         * 
         */
        public JsonResult GetArticles()
        {

            Dictionary<string, string> parameters = new Dictionary<string, string>(); 

            List<Article> articles = _articleStorage.GetArticles(parameters);

            var JSONResult = Json(articles, JsonRequestBehavior.AllowGet);
            JSONResult.MaxJsonLength = int.MaxValue;
            return JSONResult;
        }

        /**
         * This method executes the process of fetching an article, either by 
         * article ID, or assignment ID, depending on what paramter is passed on to it. 
         * This is to be executed by ajax , through GET. 
         * 
         * @param       String      fieldNameToMatch
         * @param       int         assignmentID
         * 
         * @return      JSON        JSON object representing an article
         * 
         * @author: Joshua Fuentes <joshua.fuentes@robertparker.com>
         */
        [Authorize]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult ViewArticle(int ID, string fieldNameToMatch)
        {
            Debug.Assert(fieldNameToMatch != null); 

            Article article = new Article(); 

            if(fieldNameToMatch == "assignmentID")
            {
                // Fetches article by assignment ID
                article = this._articleStorage.SearchArticleByAssignmentID(ID); 
            } else {
                // Fetches Article by article's ID
                article = this._articleStorage.SearchArticleByID(ID); 
            }

            return Json(article, JsonRequestBehavior.AllowGet);
        }


        /**
         * Returns an empty article object. This is used for adding an article 
         * into the data. 
         * 
         * @return      JSON        article        JSON string representing an article object
         * 
         * @author:     Joshua Fuentes <joshua.fuentes@robertparker.com>
         */
        [Authorize]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult GetNewArticle()
        {
            Article article = new Article();
            return Json(article, JsonRequestBehavior.AllowGet);
        }
        
        /**
         * This method executes the process of fetching an article based
         * on the given assignment ID
         * 
         * @param       int         ID      Assignment ID
         * 
         * @return      JSON        JSON object representing a collection of articles
         * 
         * @author: Joshua Fuentes <joshua.fuentes@robertparker.com>
         */
        [Authorize]
        [OutputCache(Duration = 0, VaryByParam = "none")]
        public ActionResult SearchArticlesByAssignment(int ID)
        {

            Article article = new Article();

            // Setting parameters
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("AssignmentID", ID.ToString()); 

            List<Article> articles = _articleStorage.GetArticles(parameters); 

            if (articles.Count > 0)
            {
                return Json(articles[0], JsonRequestBehavior.AllowGet);
            }

            return null; 
        }

        /**
         * Executes the process of updating an article record
         * 
         * @param       String      article     JSON string representing an article object
         * 
         * @return      JSON        article     JSON string representing an updated article object
         * 
         * @author:     Joshua Fuentes  <joshua.fuentes@robertparker.com>
         */
        public ActionResult UpdateArticle(String article)
        {

            Article articleToBeUpdated = new JavaScriptSerializer().Deserialize<Article>(article);

            _articleStorage.Update(articleToBeUpdated); 

            return Json(articleToBeUpdated); 
        }
    }
}