using EditorsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsDbLayer
{
    public class PublicationDataStorage : IPublicationDataStorage
    {



        #region --Members and Properties--
        // MEMBERS
        private ISqlConnectionFactory _ISQLConnectionFactory;
        #endregion

        #region --Methods / Implementation--
        // METHODS 
        public PublicationDataStorage(ISqlConnectionFactory connectionFactory)
        {
            _ISQLConnectionFactory = connectionFactory;
        }

        public PublicationData Create(PublicationData PublicationData)
        {
            return Save(PublicationData);
        }

        public PublicationData Delete(PublicationData PublicationData)
        {
            using (var connection = _ISQLConnectionFactory.GetConnection())
            {
                using (var command = new SqlCommand("PublicationData_Del", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;

                    #region -- Parameters --
                    command.Parameters.AddWithValue("@ID", PublicationData.ID);
                    #endregion

                    SqlDataReader reader = command.ExecuteReader();
                }
            }

            return PublicationData;
        }


        public IEnumerable<PublicationData> GetPublications()
        {
            List<PublicationData> result = new List<PublicationData>();

            using (SqlConnection conn = _ISQLConnectionFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {

                    cmd.CommandText = "select id, publisherid, name from publication";


                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            PublicationData ev = new PublicationData()
                            {
                                ID = rdr.GetInt32(0),
                                PublisherId = rdr.GetInt32(1),
                                Name = rdr.GetString(2)
                            };

                            result.Add(ev);
                        }
                    }
                    return result;
                }
            }
        }

        public IEnumerable<PublicationData> Search(PublicationData PublicationData)
        {
            throw new NotImplementedException();
        }


        
        /**
         * Executes the process of saving an PublicationData into the database
         */
        protected PublicationData Save(PublicationData PublicationData)
        {

            // TODO: Remove the asserts and dependency to the Debug class after development period
            Debug.Assert(PublicationData != null, "This expects an PublicationData to be passed");
            

            if (PublicationData == null)
            {
                throw new ArgumentException("Parameter Missing: PublicationData item required.");
            }

            using (SqlConnection connection = _ISQLConnectionFactory.GetConnection())
            {

                using (var command = new SqlCommand("Publication_Add", connection))
                {

                    command.CommandType = CommandType.StoredProcedure;


                    //using (SqlCommand command = new SqlCommand("INSERT into Publication (PublisherId, Name, Created) VALUES (@PublisherId, @Name, @Created); SELECT SCOPE_IDENTITY ();", connection))
                    //{
                    //    command.CommandType = CommandType.Text;
                    
                    command.Parameters.AddWithValue("@PublisherID ", PublicationData.PublisherId);
                    command.Parameters.AddWithValue("@Name", PublicationData.Name);
                    command.Parameters.AddWithValue("@ShowRes", 1);

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (reader != null && reader.HasRows && reader.Read())
                            {
                                PublicationData.Errorexist = false;
                                PublicationData.ErrorMessage = "";
                                PublicationData.ID = reader.GetInt32(0);
                            }
                        }
                    }catch(Exception e){

                        PublicationData.Errorexist = true;
                        PublicationData.ErrorMessage = e.Message;
                    }


                    //var id = Convert.ToInt32(command.ExecuteScalar());
                    //PublicationData.ID = id;


                }
            }


            return PublicationData;
        }


        public PublicationData Update(PublicationData PublicationData)
        {
            return Save(PublicationData);
        }
        #endregion

    }
}
