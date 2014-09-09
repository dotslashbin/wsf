using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsCommon
{

    #region -- Members --
    public class Article
    {
        public int? ID { get; set; }
        public int? PublicationID { get; set; }
        public string Publication { get; set; }
        public int? AuthorId { get; set; }
        public string Author { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public string MetaTags { get; set; }
        public string Event { get; set; }
        public int? CuisineID { get; set; }
        public string Cuisine { get; set; }
        public int? LocCountryID { get; set; }
        public int? LocRegionID { get; set; }
        public int? locLocationID { get; set; }
        public int? WF_StatusID { get; set; }
        public int? OldArticleIdN { get; set; }
        public int? OldArticleId { get; set; }
        public int? OldArticleIdNKey { get; set; }
        
    }
    #endregion

    #region --interface--
    public interface IArticleStorage : IStorage<Article>
    {
        //IEnumerable<Article> Search(Article article);

        List<Article> GetArticles(Dictionary<string, string> parameters);

        Article SearchArticleByID(int ID);

        Article SearchArticleByAssignmentID(int ID);

        Boolean AddArticleToAssignment(int articleID, int assignmentID);
    }
    #endregion
}
