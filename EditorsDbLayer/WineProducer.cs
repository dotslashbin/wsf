using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EditorsCommon;
using System.Data.SqlClient;
using System.Data;

namespace EditorsDbLayer
{
    #region ----- WineProducerStorage -----
    public class WineProducerStorage : IWineProducerStorage 
    
    {
        string _auditUserName = "";
        ISqlConnectionFactory _connFactory;

        public WineProducerStorage(ISqlConnectionFactory connFactory) {
            _connFactory = connFactory;
        
        }

   


        /// <summary>
        /// 
        /// </summary>
        /// <param name="producer"></param>
        /// <returns></returns>
        public WineProducer Create(WineProducer producer)
        {
            throw new Exception();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wineProducer"></param>
        /// <returns></returns>
        public WineProducer Update(WineProducer wineProducer) {
           // return Save(wineProducer);


            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"
    update WineProducer set
		Name = @Name, 
		NameToShow = @NameToShow,
        WebSiteURL = @WebSiteURL  
	where ID = @ID";


                    cmd.Parameters.AddWithValue("@Name", wineProducer.name);
                    cmd.Parameters.AddWithValue("@NameToShow", wineProducer.nameToShow);
                    cmd.Parameters.AddWithValue("@ID", wineProducer.id);
                    cmd.Parameters.AddWithValue("@WebSiteURL", String.IsNullOrEmpty(wineProducer.websiteUrl) ? "" : wineProducer.websiteUrl);

                    if (cmd.ExecuteNonQuery() != 1)
                        return null;
                }
            }

            return wineProducer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wineProducer"></param>
        /// <returns></returns>
        public WineProducer Delete(WineProducer wineProducer) {
            int id = wineProducer.id;

            if (id < 1) {
                throw new ArgumentException("Wine Producer must have a positive id to be deleted.");
                //IEnumerable<WineProducer> l = Search(wineProducer.Name);
                //if (l.Count() != 1)
                //    throw new ArgumentException("Wine Producer cannot be uniquely identified for delete operation.");
                //else
                //    id = l.First().ID;
            }

            Del(id);
            return wineProducer;
        }

        public int Delete(int id) {
            Del(id);
            return id;
        }





        public IEnumerable<WineProducer> SearchByName(string searchString)
        {
            List<WineProducer> res = new List<WineProducer>();

            if (String.IsNullOrEmpty(searchString))
                return res;

            searchString = "%" + searchString.Trim().Replace("%", "") + "%";



            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"
            select top(300)
	            ID = wp.ID, 
	            Name = wp.Name, 
	            NameToShow = wp.NameToShow,
	            WF_StatusID = wp.WF_StatusID,
                WebSiteURL = wp.WebSiteURL,
	            SortOrder = case when wp.NameToShow like right(@SearchString, len(@SearchString)-1) then 0 else 20 end

            from WineProducer wp (nolock)
            where wp.Name like @SearchString
            order by SortOrder, NameToShow, ID
";


                    cmd.Parameters.AddWithValue("@SearchString", searchString);


                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                            while (dr.Read())
                            {
                                WineProducer item = new WineProducer();
                                item.id = dr.GetInt32(0);
                                item.name = dr.GetString(1);
                                item.nameToShow = dr.GetString(2);
                                item.wfState = (dr.IsDBNull(3) ? (short)0 : dr.GetInt16(3));
                                item.websiteUrl = (dr.IsDBNull(4) ? "" : dr.GetString(4));

                                res.Add(item);
                            } 
                        }
                } 
            } 

            return res;
        }


        public IEnumerable<WineProducerExt> SearchByNameExt(string searchString)
        {
            List<WineProducerExt> res = new List<WineProducerExt>();

            if (String.IsNullOrEmpty(searchString))
                return res;

            searchString = "%" + searchString.Trim().Replace("%", "") + "%";



            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"
            select top(300)
	            ID = wp.ID, 
	            Name = wp.Name, 
	            NameToShow = wp.NameToShow,
	            WF_StatusID = wp.WF_StatusID,
                WebSiteURL = wp.WebSiteURL,
	            SortOrder = case when wp.NameToShow like right(@SearchString, len(@SearchString)-1) then 0 else 20 end,
                linkCount = (select count(*) from WineProducer_WineImporter where ProducerId = wp.ID)
            from WineProducer wp (nolock)
            where wp.Name like @SearchString
            order by SortOrder, NameToShow, ID
";


                    cmd.Parameters.AddWithValue("@SearchString", searchString);


                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            WineProducerExt item = new WineProducerExt();
                            item.id = dr.GetInt32(0);
                            item.name = dr.GetString(1);
                            item.nameToShow = dr.GetString(2);
                            item.wfState = (dr.IsDBNull(3) ? (short)0 : dr.GetInt16(3));
                            item.websiteUrl = (dr.IsDBNull(4) ? "" : dr.GetString(4));
                            item.linkImportersCount = (dr.IsDBNull(6) ? 0 : dr.GetInt32(6));

                            res.Add(item);
                        }
                    }
                }
            }

            return res;
        }



        public IEnumerable<WineProducer> SearchByWorkflowStatus(int status)
        {
            List<WineProducer> res = new List<WineProducer>();

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"
            select top(300)
	            ID = wp.ID, 
	            Name = wp.Name, 
	            NameToShow = wp.NameToShow,
	            WF_StatusID = wp.WF_StatusID,
                WebSiteURL = wp.WebSiteURL
            from WineProducer wp (nolock)
            where wp.WF_StatusID = @Status
            order by NameToShow, ID
";


                    cmd.Parameters.AddWithValue("@Status", status);
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            WineProducer item = new WineProducer();
                            item.id = dr.GetInt32(0);
                            item.name = dr.GetString(1);
                            item.nameToShow = dr.GetString(2);
                            item.wfState = (dr.IsDBNull(3) ? (short)0 : dr.GetInt16(3));
                            item.websiteUrl = (dr.IsDBNull(4) ? "" : dr.GetString(4));

                            res.Add(item);
                        }
                    }
                }
            }

            return res;
        }



        public IEnumerable<WineProducerExt> SearchByWorkflowStatusExt(int status)
        {
            List<WineProducerExt> res = new List<WineProducerExt>();

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"
            select top(300)
	            ID = wp.ID, 
	            Name = wp.Name, 
	            NameToShow = wp.NameToShow,
	            WF_StatusID = wp.WF_StatusID,
                WebSiteURL = wp.WebSiteURL,
                linkCount = (select count(*) from WineProducer_WineImporter where ProducerId = wp.ID)
            from WineProducer wp (nolock)
            where wp.WF_StatusID = @Status
            order by NameToShow, ID
";


                    cmd.Parameters.AddWithValue("@Status", status);
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (dr.Read())
                        {
                            WineProducerExt item = new WineProducerExt();
                            item.id = dr.GetInt32(0);
                            item.name = dr.GetString(1);
                            item.nameToShow = dr.GetString(2);
                            item.wfState = (dr.IsDBNull(3) ? (short)0 : dr.GetInt16(3));
                            item.websiteUrl = (dr.IsDBNull(4) ? "" : dr.GetString(4));
                            item.linkImportersCount = (dr.IsDBNull(5) ? 0 : dr.GetInt32(5));

                            res.Add(item);
                        }
                    }
                }
            }

            return res;
        }


        public IEnumerable<WineProducer> Search(WineProducer wineProducer) 
        {
            if (wineProducer == null || string.IsNullOrWhiteSpace(wineProducer.name))
                throw new ArgumentException("wineProducer with populated Name is required.");

            List<WineProducer> res = new List<WineProducer>();


            using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand("WineProducer_GetList", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Name", wineProducer.name);


                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection)) {
                            if (dr != null && dr.HasRows) {

                                while (dr.Read()) {
                                    WineProducer item = new WineProducer();
                                    item.id = dr.GetInt32(0);
                                    item.name = dr.GetString(1);
                                    item.nameToShow = dr.GetString(2);
                                    item.websiteUrl = (dr.IsDBNull(3) ? "" : dr.GetString(3));

                                    item.wfState = (dr.IsDBNull(11) ? (short)0 : dr.GetInt16(11));

                                    if (!dr.IsDBNull(13))
                                        item.dateCreated = dr.GetDateTime(13);

                                    if (!dr.IsDBNull(14))
                                        item.dateUpdated = dr.GetDateTime(14);


                                    res.Add(item);
                                } // while
                            }
                            if (dr != null && !dr.IsClosed)
                                dr.Close();
                        } // datareader
                    } // sqlCommand
                } // sqlConn

            return res;
        } // Search

        public WineProducer GetByID(int id) 
        {
            return Load(id);
        } // GetByID

        #region --- protected ---
        /// <summary>
        /// Gets WineProducer by internal ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected WineProducer Load(int id) {
            // WineProducer_GetList	@ID int
            // Returns (nullable values are allowed):
            //  ID, Name, NameToShow, WebSiteURL, 
            //  locCountry, locRegion, locLocation, locLocale, locSiteID,
            //  Profile, ContactInfo, WF_StatusID, WF_StatusName, created, updated, CreatorName, EditorName
            WineProducer res = null;

            using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand("WineProducer_GetList", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ID", id);


                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection)) {
                            if (dr != null && dr.HasRows && dr.Read()) {
                                res = new WineProducer();
                                res.id = dr.GetInt32(0);
                                res.name = dr.GetString(1);
                                res.nameToShow = dr.GetString(2);
                                res.websiteUrl = (dr.IsDBNull(3) ? "" : dr.GetString(3));

                                res.wfState = (dr.IsDBNull(11) ? (short) 0 : dr.GetInt16(11));

                                if (!dr.IsDBNull(13))
                                    res.dateCreated = dr.GetDateTime(13);
                                if (!dr.IsDBNull(14))
                                    res.dateUpdated = dr.GetDateTime(14);


                            }
                            if (dr != null && !dr.IsClosed)
                                dr.Close();
                        } // datareader
                    } // sqlCommand
                } // sqlConn

            return res;
        } // Load


        /// <summary>
        /// Deletes WineProducer by internal ID.
        /// </summary>
        /// <param name="auditUserName">User Name used for Audit purposes. Usually, System.Security.Principal.Identity.Name.</param>
        /// <returns></returns>
        protected bool Del(int id) {
            if (id < 1)
                throw new Exception("ID is required for delete operation. Initialize (load) instance before deleting it.");

            bool res = false;

            // WineProducer_Del
                //  @ID int, 
                //  @UserName varchar(50), @ShowRes smallint = 1
                //
                using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand("WineProducer_Del", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        #region -- parameters --
                        if (id > 0)
                            cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@UserName", _auditUserName);
                        cmd.Parameters.AddWithValue("@ShowRes", 1);
                        #endregion -- parameters --

                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection)) {
                            if (dr != null && dr.HasRows && dr.Read()) {
                                res = true;
                            }
                            if (dr != null && !dr.IsClosed)
                                dr.Close();
                        } // datareader
                    } // sqlCommand
                } // sqlConn

            return res;
        } // Del
        #endregion --- protected ---





        public bool SetProducerStatus(int id, int status)
        {
            List<WineProducer> res = new List<WineProducer>();

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @" update WineProducer set WF_StatusID = @Status where ID = @ID";

                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ID", id);
                    return cmd.ExecuteNonQuery() == 1;
                }
            }
        }



    }
    #endregion ----- WineProducerStorage -----
}
