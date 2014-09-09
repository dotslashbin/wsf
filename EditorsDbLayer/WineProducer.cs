using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EditorsCommon;
using System.Data.SqlClient;
using System.Data;
using EditorsCommon.Wine;

namespace EditorsDbLayer.Data.Wine
{
    #region ----- WineProducerStorage -----
    public class WineProducerStorage : IWineProducer 
    
    {
        string _auditUserName = "";
        ISqlConnectionFactory _connFactory;

        public WineProducerStorage(ISqlConnectionFactory connFactory, string auditUserName = "") {
            _connFactory = connFactory;
            _auditUserName = auditUserName;
        
        }

   

        #region --- IWineProducer Members ---

        /// <summary>
        /// 
        /// </summary>
        /// <param name="producer"></param>
        /// <returns></returns>
        public WineProducer Create(WineProducer producer)
        {
            return Save(producer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wineProducer"></param>
        /// <returns></returns>
        public WineProducer Update(WineProducer wineProducer) {
            return Save(wineProducer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wineProducer"></param>
        /// <returns></returns>
        public WineProducer Delete(WineProducer wineProducer) {
            int id = wineProducer.ID;

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

        public IEnumerable<WineProducer> Search(WineProducer wineProducer) 
        {
            if (wineProducer == null || string.IsNullOrWhiteSpace(wineProducer.Name))
                throw new ArgumentException("wineProducer with populated Name is required.");

            List<WineProducer> res = new List<WineProducer>();

            // WineProducer_GetList	@ID int, @Name nvarchar(100) = NULL,
                //  @StartRow int = NULL, @EndRow int = NULL,
                //  @SortBy varchar(20) = NULL, @SortOrder varchar(3) = NULL
                // Returns (nullable values are allowed):
                //  ID, Name, NameToShow, WebSiteURL, 
                //  locCountry, locRegion, locLocation, locLocale, locSiteID,
                //  Profile, ContactInfo, WF_StatusID, WF_StatusName, created, updated, CreatorName, EditorName,
                //  RowNumber, TotalRows -- true values only if @StartRow and @EndRow are specified, otherwise 0.
                using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand("WineProducer_GetList", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        #region -- parameters --
                        cmd.Parameters.AddWithValue("@Name", wineProducer.Name);
                        #endregion -- parameters --

                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection)) {
                            if (dr != null && dr.HasRows) {

                                while (dr.Read()) {
                                    WineProducer item = new WineProducer();
                                    item.ID = dr.GetInt32(0);
                                    item.Name = dr.GetString(1);
                                    item.NameToShow = dr.GetString(2);
                                    item.WebSiteURL = (dr.IsDBNull(3) ? "" : dr.GetString(3));

                                    item.Country = (dr.IsDBNull(4) ? "" : dr.GetString(4));
                                    item.Region = (dr.IsDBNull(5) ? "" : dr.GetString(5));
                                    item.Location = (dr.IsDBNull(6) ? "" : dr.GetString(6));
                                    item.Locale = (dr.IsDBNull(7) ? "" : dr.GetString(7));
                                    item.Site = (dr.IsDBNull(8) ? "" : dr.GetString(8));

                                    item.Profile = (dr.IsDBNull(9) ? "" : dr.GetString(9));
                                    item.ContactInfo = (dr.IsDBNull(10) ? "" : dr.GetString(10));

                                    item.WF_StatusID = (dr.IsDBNull(11) ? (short)0 : dr.GetInt16(11));
                                    item.WF_StatusName = (dr.IsDBNull(12) ? "" : dr.GetString(12));

                                    if (!dr.IsDBNull(13))
                                        item.DateCreated = dr.GetDateTime(13);

                                    if (!dr.IsDBNull(14))
                                        item.DateUpdated = dr.GetDateTime(14);

                                    item.CreatorName = (dr.IsDBNull(15) ? "" : dr.GetString(15));
                                    item.EditorName = (dr.IsDBNull(16) ? "" : dr.GetString(16));

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
        #endregion --- IWineProducer Members ---

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
                        #region -- parameters --
                        cmd.Parameters.AddWithValue("@ID", id);
                        #endregion -- parameters --

                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection)) {
                            if (dr != null && dr.HasRows && dr.Read()) {
                                res = new WineProducer();
                                res.ID = dr.GetInt32(0);
                                res.Name = dr.GetString(1);
                                res.NameToShow = dr.GetString(2);
                                res.WebSiteURL = (dr.IsDBNull(3) ? "" : dr.GetString(3));

                                res.Country = (dr.IsDBNull(4) ? "" : dr.GetString(4));
                                res.Region = (dr.IsDBNull(5) ? "" : dr.GetString(5));
                                res.Location = (dr.IsDBNull(6) ? "" : dr.GetString(6));
                                res.Locale = (dr.IsDBNull(7) ? "" : dr.GetString(7));
                                res.Site = (dr.IsDBNull(8) ? "" : dr.GetString(8));

                                res.Profile = (dr.IsDBNull(9) ? "" : dr.GetString(9));
                                res.ContactInfo = (dr.IsDBNull(10) ? "" : dr.GetString(10));

                                res.WF_StatusID = (dr.IsDBNull(11) ? (short) 0 : dr.GetInt16(11));
                                res.WF_StatusName = (dr.IsDBNull(12) ? "" : dr.GetString(12));
                                if (!dr.IsDBNull(13))
                                    res.DateCreated = dr.GetDateTime(13);
                                if (!dr.IsDBNull(14))
                                    res.DateUpdated = dr.GetDateTime(14);

                                res.CreatorName = (dr.IsDBNull(15) ? "" : dr.GetString(15));
                                res.EditorName = (dr.IsDBNull(16) ? "" : dr.GetString(16));

                            }
                            if (dr != null && !dr.IsClosed)
                                dr.Close();
                        } // datareader
                    } // sqlCommand
                } // sqlConn

            return res;
        } // Load

        /// <summary>
        /// Saves all WineProducer attributes.
        /// </summary>
        /// <param name="auditUserName">User Name used for Audit purposes. Usually, System.Security.Principal.Identity.Name.</param>
        /// <returns></returns>
        protected WineProducer Save(WineProducer wineProducer) {
            if (wineProducer == null)
                throw new ArgumentException("wineProducer is required.");

            // WineProducer_Add
                //  @Name nvarchar(100), @NameToShow nvarchar(100), @WebSiteURL nvarchar(255) = NULL,
                //  --@locCountry nvarchar(50) = NULL, @locRegion nvarchar(50) = NULL, 
                //  --@locLocation nvarchar(50) = NULL, @locLocale nvarchar(50) = NULL, @locSite nvarchar(50) = NULL,
                //  @Profile nvarchar(max) = NULL, @ContactInfo nvarchar(max) = NULL, 
                //  @WF_StatusID smallint = NULL, @UserName varchar(50), @ShowRes smallint = 1
                //
                // WineProducer_Update
                //  @ID int, 
                //  @Name nvarchar(100) = NULL, @NameToShow nvarchar(100) = NULL, @WebSiteURL nvarchar(255) = NULL,
                //  --@locCountry nvarchar(50) = NULL, @locRegion nvarchar(50) = NULL, 
                //  --@locLocation nvarchar(50) = NULL, @locLocale nvarchar(50) = NULL, @locSite nvarchar(50) = NULL,
                //  @Profile nvarchar(max) = NULL, @ContactInfo nvarchar(max) = NULL, 
                //  @WF_StatusID smallint = NULL, @UserName varchar(50), @ShowRes smallint = 1
                //
                using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand((wineProducer.ID > 0 ? "WineProducer_Update" : "WineProducer_Add"), conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        #region -- parameters --
                        if (wineProducer.ID > 0)
                            cmd.Parameters.AddWithValue("@ID", wineProducer.ID);
                        cmd.Parameters.AddWithValue("@Name", wineProducer.Name);
                        cmd.Parameters.AddWithValue("@NameToShow", wineProducer.NameToShow);
                        cmd.Parameters.AddWithValue("@WebSiteURL", wineProducer.WebSiteURL);
                        cmd.Parameters.AddWithValue("@Profile", wineProducer.Profile);
                        cmd.Parameters.AddWithValue("@ContactInfo", wineProducer.ContactInfo);
                        cmd.Parameters.AddWithValue("@WF_StatusID", wineProducer.WF_StatusID);
                        cmd.Parameters.AddWithValue("@UserName", _auditUserName);
                        cmd.Parameters.AddWithValue("@ShowRes", 1);
                        #endregion -- parameters --

                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection)) {
                            if (dr != null && dr.HasRows && dr.Read()) {
                                if (wineProducer.ID < 1)
                                    wineProducer.ID = dr.GetInt32(0);
                            }
                            if (dr != null && !dr.IsClosed)
                                dr.Close();
                        } // datareader
                    } // sqlCommand
                } // sqlConn

            return wineProducer;
        } // Save

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



    }
    #endregion ----- WineProducerStorage -----
}
