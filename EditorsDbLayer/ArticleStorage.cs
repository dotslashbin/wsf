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

        private ISqlConnectionFactory _ISQLConnectionFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionFactory"></param>
        public ArticleStorage(ISqlConnectionFactory connectionFactory)
        {
            _ISQLConnectionFactory = connectionFactory;
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
            //
            //it does not make sense compare value type to null
            //todo. remove asserts
            //
            Debug.Assert(articleID != null);
            Debug.Assert(assignmentID != null);

            using (SqlConnection connection = _ISQLConnectionFactory.GetConnection())
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
            using (var connection = _ISQLConnectionFactory.GetConnection())
            {
                using (var command = new SqlCommand("Article_Del", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID", article.ID);
                    SqlDataReader reader = command.ExecuteReader();
                }
            }

            return article;
        }

        private Article GetArticle(string fieldName, int ID)
        {
            Debug.Assert(fieldName != null);
            Debug.Assert(ID != null);

            Article article = new Article();

            using (var connection = _ISQLConnectionFactory.GetConnection())
            {
                using (var command = new SqlCommand("Article_GetList", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@" + fieldName, ID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                            while (reader.Read())
                            {
                                article.ID              = reader.GetInt32(0);
                                article.PublicationID   = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                                article.Publication     = reader.IsDBNull(2) ? null : reader.GetString(2);
                                article.AuthorId        = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                                article.AuthorName      = reader.IsDBNull(4) ? null : reader.GetString(4);
                                article.Author          = reader.IsDBNull(5) ? null : reader.GetString(5);
                                article.Title           = reader.IsDBNull(6) ? null : reader.GetString(6);
                                article.ShortTitle      = reader.IsDBNull(7) ? null : reader.GetString(7);
                                article.Notes           = reader.IsDBNull(9) ? null : reader.GetString(9);
                                article.MetaTags        = reader.IsDBNull(10) ? null : reader.GetString(10);
                                article.Event           = reader.IsDBNull(11) ? null : reader.GetString(11);
                                article.CuisineID       = reader.IsDBNull(12) ? 0 : reader.GetInt32(12);
                                article.Cuisine         = reader.IsDBNull(13) ? null : reader.GetString(13);
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

            using (var connection = _ISQLConnectionFactory.GetConnection())
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
                            article.ID = reader.GetInt32(0);
                            article.PublicationID = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                            article.Publication = reader.IsDBNull(2) ? null : reader.GetString(2);
                            article.AuthorId = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                            article.AuthorName = reader.IsDBNull(4) ? null : reader.GetString(4);
                            article.Author = reader.IsDBNull(5) ? null : reader.GetString(5);
                            article.Title = reader.IsDBNull(6) ? null : reader.GetString(6);
                            article.ShortTitle = reader.IsDBNull(7) ? null : reader.GetString(7);
                            // article.Date                = reader.GetDateTime(7);
                            article.Notes = reader.IsDBNull(9) ? null : reader.GetString(9);
                            article.MetaTags = reader.IsDBNull(10) ? null : reader.GetString(10);
                            article.Event = reader.IsDBNull(11) ? null : reader.GetString(11);
                            article.CuisineID = reader.IsDBNull(12) ? 0 : reader.GetInt32(12);
                            article.Cuisine = reader.IsDBNull(13) ? null : reader.GetString(13);
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

        /**
         * Executes the process of saving an article into the database
         */
        protected Article Save(Article article)
        {
            // 
            //
            // TODO: Remove the asserts and dependency to the Debug class after development period
            Debug.Assert(article != null, "This expects an article to be passed");

            if (article == null)
            {
                throw new ArgumentException("Parameter Missing: Article item required.");
            }


            using (SqlConnection connection = _ISQLConnectionFactory.GetConnection())
            {
                string SQL = (article.ID > 0 ? "Article_Update" : "Article_Add");

                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    // TODO: add implementation to build stored procedure command with paramteres
                    command.CommandType = CommandType.StoredProcedure;

                    if (article.ID > 0)
                    {
                        // Assigns ID if present. This will be the determining factor for update
                        // This also means that the date UPDATED should be updated logged
                        command.Parameters.AddWithValue("@ID", article.ID);
                    }
                    else
                    {
                        // Since this means it is a new record, the Date CREATED is should be saved
                        // command.Parameters.AddWithValue(); 
                        command.Parameters.AddWithValue("@Date", DateTime.Now);
                    }

                    // Assigning paramters for the stored procedure call
                    command.Parameters.AddWithValue("@PublicationID", article.PublicationID);
                    command.Parameters.AddWithValue("@Title", article.Title);
                    command.Parameters.AddWithValue("@ShortTitle", article.ShortTitle);
                    command.Parameters.AddWithValue("@Notes", article.Notes);
                    command.Parameters.AddWithValue("@AuthorId", article.AuthorId); 
                    command.Parameters.AddWithValue("@Author", article.Author);
                    command.Parameters.AddWithValue("@ShowRes", 1);
                    

                    // Reading the results
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {

                        if (dataReader.Read())
                        {
                            if (article.ID == 0 || article.ID == null)
                            {
                                article.ID = dataReader.GetInt32(0);
                            }
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
