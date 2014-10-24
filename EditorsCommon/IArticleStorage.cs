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
        public int id;
        public int publicationId;
        public string publication;
        public int authorId;
        public string author;
        public string authorName;
        public string title;
        public string shortTitle;
        public DateTime date;
        public string notes;
        public string metaTags;
        public string Event;
        public int cuisineID;
        public string cuisine;
        public int LocCountryID;
        public int LocRegionID;
        public int locLocationID;
        public int WF_StatusID;
        public int OldArticleIdN;
        public int OldArticleId;
        public int OldArticleIdNKey;
        
    }
    #endregion

    public interface IArticleStorage : IStorage<Article>
    {
        //IEnumerable<Article> Search(Article article);

        List<Article> GetArticles(Dictionary<string, string> parameters);

        Article SearchArticleByID(int ID);

        Article SearchArticleByAssignmentID(int ID);

        Boolean AddArticleToAssignment(int articleID, int assignmentID);
    }

}
