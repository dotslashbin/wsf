using EditorsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EditorsDbLayer
{
    /// <summary>
    /// 
    /// 
    /// 
    /// </summary>
    public class ArticleStorage : IArticleStorage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <Author>Joshua.Fuentes <joshua.fuentes@robertparker.com></Author>

        private ISqlConnectionFactory _connectionFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionFactory"></param>
        public ArticleStorage(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }



        /// <summary>
        /// Executes the processes of saving the relationship between an article
        /// and an assignment.         
        /// </summary>
        /// <param name="articleID"></param>
        /// <param name="assignmentID"></param>
        /// <returns></returns>
        public Boolean AddArticleToAssignment(int articleID, int assignmentID)
        {
 

            using (SqlConnection connection = _connectionFactory.GetConnection())
            {

                string SQL = ("Assignment_Article_Add");

                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    // TODO: add implementation to build stored procedure command with paramteres
                    command.CommandType = CommandType.StoredProcedure;

                    // Assigning paramters for the stored procedure call
                    command.Parameters.AddWithValue("@AssignmentID", assignmentID);
                    command.Parameters.AddWithValue("@ArticleID", articleID);
                    command.Parameters.AddWithValue("@ShowRes", 1);
                    // TODO: add implementation for the rest of the fields when finalized

                    // Reading the results
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {

                        if (dataReader.Read())
                        {
                            //int result = dataReader.GetInt16(0);

                            //if (result > 0)
                            //{
                            //    return true;
                            //}
                            //else
                            //{
                            //    return false;
                            //}

                            return true;
                        }
                    }
                }
            }



            return false;
        }


        public Article Create(Article article)
        {
            return Save(article);
        }

        public Article Delete(Article article)
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                using (var command = new SqlCommand("Article_Del", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", article.id);
                    SqlDataReader reader = command.ExecuteReader();
                }
            }

            return article;
        }

        private Article GetArticle(string fieldName, int ID)
        {
            Debug.Assert(fieldName != null);
 

            Article article = new Article();

            using (var connection = _connectionFactory.GetConnection())
            {
                using (var command = new SqlCommand("Article_GetList", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@" + fieldName, ID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                            while (reader.Read())
                            {
                                article.id              = reader.GetInt32(0);
                                article.publicationId   = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                article.publication     = reader.IsDBNull(2) ? null : reader.GetString(2);
                                article.authorId        = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                                article.authorName      = reader.IsDBNull(4) ? null : reader.GetString(4);
                                article.author          = reader.IsDBNull(5) ? null : reader.GetString(5);
                                article.title           = reader.IsDBNull(6) ? null : reader.GetString(6);
                                article.shortTitle      = reader.IsDBNull(7) ? null : reader.GetString(7);
                                article.notes           = reader.IsDBNull(9) ? null : reader.GetString(9);
                                article.metaTags        = reader.IsDBNull(10) ? null : reader.GetString(10);
                                article.Event           = reader.IsDBNull(11) ? null : reader.GetString(11);
                                article.cuisineID       = reader.IsDBNull(12) ? 0 : reader.GetInt32(12);
                                article.cuisine         = reader.IsDBNull(13) ? null : reader.GetString(13);
                                article.LocCountryID    = reader.IsDBNull(14) ? 0 : reader.GetInt32(14);
                                article.LocRegionID     = reader.IsDBNull(15) ? 0 : reader.GetInt32(15);
                                article.locLocationID   = reader.IsDBNull(16) ? 0 : reader.GetInt32(16);
                            }
                        }
                    }
                }
 
            return article;
        }

        /**
         * Executes the process of fetching articles from the database. 
         * Returns a list / collection of article objects.
         */
        public List<Article> GetArticles(Dictionary<string, string> parameters)
        {
            List<Article> result = new List<Article>();

            using (var connection = _connectionFactory.GetConnection())
            {

                using (var command = new SqlCommand("Article_GetList", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters.Count > 0)
                    {
                        //command.Parameters.AddWithValue("@" + fieldName, ID);
                        foreach (KeyValuePair<string, string> entry in parameters)
                        {
                            string parameterName = entry.Key;
                            string parameterValue = entry.Value;

                            command.Parameters.AddWithValue("@" + parameterName, parameterValue.ToString());
                        }
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        int counter = 0;

                        #region -- Building article object --
                        while (reader.Read() && counter < 10)
                        {
                            Article article = new Article();
                            article.id = reader.GetInt32(0);
                            article.publicationId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                            article.publication = reader.IsDBNull(2) ? null : reader.GetString(2);
                            article.authorId = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                            article.authorName = reader.IsDBNull(4) ? null : reader.GetString(4);
                            article.author = reader.IsDBNull(5) ? null : reader.GetString(5);
                            article.title = reader.IsDBNull(6) ? null : reader.GetString(6);
                            article.shortTitle = reader.IsDBNull(7) ? null : reader.GetString(7);
                            // article.Date                = reader.GetDateTime(7);
                            article.notes = reader.IsDBNull(9) ? null : reader.GetString(9);
                            article.metaTags = reader.IsDBNull(10) ? null : reader.GetString(10);
                            article.Event = reader.IsDBNull(11) ? null : reader.GetString(11);
                            article.cuisineID = reader.IsDBNull(12) ? 0 : reader.GetInt32(12);
                            article.cuisine = reader.IsDBNull(13) ? null : reader.GetString(13);
                            article.LocCountryID = reader.IsDBNull(14) ? 0 : reader.GetInt32(14);
                            article.LocRegionID = reader.IsDBNull(15) ? 0 : reader.GetInt32(15);
                            article.locLocationID = reader.IsDBNull(16) ? 0 : reader.GetInt32(16);

                            result.Add(article);
                        }
                        #endregion
                    }
                }
            }

            return result;
        }

        public IEnumerable<Article> Search(Article article)
        {
            throw new NotImplementedException();
        }

 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        protected Article Save(Article article)
        {
  

            if (article == null)
            {
                throw new ArgumentException("Parameter Missing: Article item required.");
            }


            using (SqlConnection connection = _connectionFactory.GetConnection())
            {
                string SQL = (article.id > 0 ? "Article_Update" : "Article_Add");

                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    // TODO: add implementation to build stored procedure command with paramteres
                    command.CommandType = CommandType.StoredProcedure;

                    if (article.id > 0)
                    {
                        // Assigns ID if present. This will be the determining factor for update
                        // This also means that the date UPDATED should be updated logged
                        command.Parameters.AddWithValue("@ID", article.id);
                    }
                    else
                    {
                        // Since this means it is a new record, the Date CREATED is should be saved
                        // command.Parameters.AddWithValue(); 
                        command.Parameters.AddWithValue("@Date", DateTime.Now);
                    }

                    // Assigning paramters for the stored procedure call
                    command.Parameters.AddWithValue("@PublicationID", article.publicationId);
                    command.Parameters.AddWithValue("@Title", article.title);
                    command.Parameters.AddWithValue("@ShortTitle", article.shortTitle);
                    command.Parameters.AddWithValue("@Notes", article.notes);
                    command.Parameters.AddWithValue("@AuthorId", article.authorId); 
                    command.Parameters.AddWithValue("@Author", article.author);
                    command.Parameters.AddWithValue("@ShowRes", 1);
                    

                    // Reading the results
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {

                        if (dataReader.Read())
                        {
                          article.id = dataReader.GetInt32(0);
                        }
                    }
                }
            }


            return article;
        }

        public Article SearchArticleByID(int ID)
        {
            Article article = this.GetArticle("ID", ID);
            return article;
        }

        public Article SearchArticleByAssignmentID(int ID)
        {
            Article article = this.GetArticle("AssignmentID", ID);
            return article;
        }

        public Article Update(Article article)
        {

            return Save(article);
        }
    }
}
