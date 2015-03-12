using EditorsCommon;
using EditorsCommon.Publication;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsDbLayer
{


    public class IssueStorage : IIssueStorage
    {


        ISqlConnectionFactory _connFactory;

        public IssueStorage(ISqlConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public IssueStorage()
        {
            _connFactory = null;
        }



        /// <summary>
        /// Gets Issue by internal ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected Issue Load(int id)
        {
            // Issue_GetList @ID int
            // Returns (nullable values are allowed):
            //  ID,	PublicationID, PublicationName,	ChiefEditorID, ChiefEditorName, Title, CreatedDate, PublicationDate, Notes, 
            //  created, updated,
            //  Cnt_Articles, Cnt_ArticlesPublished, Cnt_TasteNotes, Cnt_TasteNotesPublished, Cnt_TastingEvents, Cnt_TastingEventsPublished
            Issue res = null;

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("Issue_GetList", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    #region -- parameters --
                    cmd.Parameters.AddWithValue("@ID", id);
                    #endregion -- parameters --

                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (dr != null && dr.HasRows && dr.Read())
                        {
                            res = new Issue();
                            res.id = dr.GetInt32(0);
                            res.publicationID = dr.GetInt32(1);
                            res.publicationName = dr.GetString(2);
                            res.chiefEditorId = (dr.IsDBNull(3) ? 0 : dr.GetInt32(3));
                            res.chiefEditorName = (dr.IsDBNull(4) ? "" : dr.GetString(4));
                            res.title = dr.GetString(5);
                            if (!dr.IsDBNull(6))
                                res.createdDate = dr.GetDateTime(6);
                            if (!dr.IsDBNull(7))
                                res.publicationDate = dr.GetDateTime(7);
                            res.Notes = (dr.IsDBNull(8) ? "" : dr.GetString(8));
                            res.wfState = dr.GetFieldValue<Int32>(9);

                            //if (!dr.IsDBNull(10))
                            //    res.dateCreated = dr.GetDateTime(10);
                            //if (!dr.IsDBNull(11))
                            //    res.dateUpdated = dr.GetDateTime(11);

                            res.articlesCnt = (dr.IsDBNull(12) ? 0 : dr.GetInt32(12));
                            res.articlesPublishedCnt = (dr.IsDBNull(13) ? 0 : dr.GetInt32(13));
                            res.tasteNotesCnt = (dr.IsDBNull(14) ? 0 : dr.GetInt32(14));
                            res.tasteNotesPublishedCnt = (dr.IsDBNull(15) ? 0 : dr.GetInt32(15));
                            res.tastingEventsCnt = (dr.IsDBNull(16) ? 0 : dr.GetInt32(16));
                            res.tastingEventsPublishedCnt = (dr.IsDBNull(17) ? 0 : dr.GetInt32(17));
                        }
                        if (dr != null && !dr.IsClosed)
                            dr.Close();
                    } // datareader
                } // sqlCommand
            } // sqlConn

            return res;
        } // Load

        /// <summary>
        /// Saves all Issue attributes.
        /// </summary>
        /// <returns></returns>
        protected Issue Save(Issue issue)
        {
            if (issue == null)
                throw new ArgumentException("issue is required.");

            // Issue_Add
            //  --@ID int = NULL, 
            //  @PublicationID int, @ChiefEditorID int = NULL, @ChiefEditorUserID int = NULL, @ChiefEditorName nvarchar(120) = NULL, 
            //  @Title nvarchar(255), @CreatedDate date, @PublicationDate date, @Notes nvarchar(max),
            //  --@UserName varchar(50),
            //  @ShowRes smallint = 1
            //
            // Issue_Update
            //  @ID int, 
            //  @PublicationID int = NULL, @ChiefEditorID int = NULL, @ChiefEditorUserID int = NULL, @ChiefEditorName nvarchar(120) = NULL, 
            //  @Title nvarchar(255) = NULL, @CreatedDate date = NULL, @PublicationDate date = NULL, @Notes nvarchar(max) = NULL,
            //  --@UserName varchar(50),
            //  @ShowRes smallint = 1
            //
            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand((issue.id > 0 ? "Issue_Update" : "Issue_Add"), conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    #region -- parameters --
                    // 2/15/2014 Sergiy
                    // modified, split set of parameters for Issue_Update and Issue_Add
                    if (issue.id > 0)
                    {
                        cmd.Parameters.AddWithValue("@ID", issue.id);
                        cmd.Parameters.AddWithValue("@PublicationID", issue.publicationID);
                        cmd.Parameters.AddWithValue("@ChiefEditorUserID", issue.chiefEditorId);
                        cmd.Parameters.AddWithValue("@Title", issue.title);
                        cmd.Parameters.AddWithValue("@CreatedDate", issue.createdDate);
                        cmd.Parameters.AddWithValue("@PublicationDate", issue.publicationDate);
                        cmd.Parameters.AddWithValue("@Notes", String.IsNullOrEmpty(issue.Notes) ? "" : issue.Notes);
                        //cmd.Parameters.AddWithValue("@UserName", _auditUserName);
                        cmd.Parameters.AddWithValue("@ShowRes", 1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PublicationID", issue.publicationID);
                        cmd.Parameters.AddWithValue("@ChiefEditorUserId", issue.chiefEditorId);
                        cmd.Parameters.AddWithValue("@Title", issue.title);
                        cmd.Parameters.AddWithValue("@CreatedDate", issue.createdDate);
                        cmd.Parameters.AddWithValue("@PublicationDate", issue.publicationDate);
                        cmd.Parameters.AddWithValue("@Notes", String.IsNullOrEmpty(issue.Notes) ? "" : issue.Notes);
                    }



                    #endregion -- parameters --

                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (dr != null && dr.HasRows && dr.Read())
                        {
                            if (issue.id < 1)
                                issue.id = dr.GetFieldValue<Int32>(0);
                        }
                        if (dr != null && !dr.IsClosed)
                            dr.Close();
                    } // datareader
                } // sqlCommand
            } // sqlConn

            return issue;
        } // Save

        /// <summary>
        /// Deletes Issue by internal ID.
        /// </summary>
        /// <returns></returns>
        protected bool Del(int id)
        {
            if (id < 1)
                throw new Exception("ID is required for delete operation. Initialize (load) instance before deleting it.");

            bool res = false;


            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("Issue_Del", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (id > 0)
                        cmd.Parameters.AddWithValue("@ID", id);

                    cmd.Parameters.AddWithValue("@ShowRes", 1);


                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (dr != null && dr.HasRows && dr.Read())
                        {
                            res = true;
                        }
                        if (dr != null && !dr.IsClosed)
                            dr.Close();
                    } 
                } 
            } 

            return res;
        } 



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Issue GetByID(int id)
        {
            return Load(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public Issue Create(Issue e)
        {
            return Save(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public Issue Update(Issue e)
        {
            return Save(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public Issue Delete(Issue e)
        {
            int id = e.id;
            if (id < 1)
                throw new ArgumentException("Issue must have a positive id to be deleted.");
            Del(id);
            return e;
        }



        public IEnumerable<Issue> GetIssuesForUser(int userId)
        {

            List<Issue> res = new List<Issue>();


            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("Issue_GetList_ForUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userId);


                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {

                            while (dr.Read())
                            {
                                Issue item = new Issue();
                                item.id = dr.GetInt32(0);
                                item.publicationID = dr.GetInt32(1);
                                item.publicationName = dr.GetString(2);
                                item.chiefEditorId = (dr.IsDBNull(3) ? 0 : dr.GetInt32(3));
                                item.chiefEditorName = (dr.IsDBNull(4) ? "" : dr.GetString(4));
                                item.title = dr.GetString(5);
                                if (!dr.IsDBNull(6))
                                    item.createdDate = dr.GetDateTime(6);
                                if (!dr.IsDBNull(7))
                                    item.publicationDate = dr.GetDateTime(7);
                                item.Notes = (dr.IsDBNull(8) ? "" : dr.GetString(8));
                                item.wfState = dr.GetInt32(9);



                                item.articlesCnt = (dr.IsDBNull(12) ? 0 : dr.GetInt32(12));
                                item.articlesPublishedCnt = (dr.IsDBNull(13) ? 0 : dr.GetInt32(13));
                                item.tasteNotesCnt = (dr.IsDBNull(14) ? 0 : dr.GetInt32(14));
                                item.tasteNotesPublishedCnt = (dr.IsDBNull(15) ? 0 : dr.GetInt32(15));
                                item.tastingEventsCnt = (dr.IsDBNull(16) ? 0 : dr.GetInt32(16));
                                item.tastingEventsPublishedCnt = (dr.IsDBNull(17) ? 0 : dr.GetInt32(17));

                                res.Add(item);
                            }
                    }
                }
            }

            return res;
        }


        public IEnumerable<Issue> Search(Issue e)
        {

            //
            // bb. 02.05.2014
            // in some cases null could valid input.
            //
            //if (e == null)
            //    throw new ArgumentException("Issue with filter parameters is required.");



            List<Issue> res = new List<Issue>();

            // Issue_GetList @ID int,
            //  @PublicationID int = NULL,
            //  @ChiefEditorID int = NULL, @ChiefEditorUserID int = NULL, @ChiefEditorName nvarchar(120) = NULL, 
            //  @Title nvarchar(255) = NULL, @CreatedDate date = NULL, @PublicationDate date = NULL,
            // Returns (nullable values are allowed):
            //  ID,	PublicationID, PublicationName,	ChiefEditorID, ChiefEditorName, Title, CreatedDate, PublicationDate, Notes, 
            //  created, updated,
            //  Cnt_Articles, Cnt_ArticlesPublished, Cnt_TasteNotes, Cnt_TasteNotesPublished, Cnt_TastingEvents, Cnt_TastingEventsPublished
            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("Issue_GetList", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // 
                    // use default parameters if filter is not defined
                    //
                    if (e != null)
                    {
                        if (e.id > 0)
                            cmd.Parameters.AddWithValue("@ID", e.id);

                        if (e.publicationID > 0)
                            cmd.Parameters.AddWithValue("@PublicationID", e.publicationID);

                        if (e.chiefEditorId > 0)
                            cmd.Parameters.AddWithValue("@ChiefEditorID", e.chiefEditorId);

                        if (!String.IsNullOrEmpty(e.chiefEditorName))
                            cmd.Parameters.AddWithValue("@ChiefEditorName", e.chiefEditorName);

                        if (!String.IsNullOrEmpty(e.title))
                            cmd.Parameters.AddWithValue("@Title", e.title);

                        if (e.createdDate.Ticks > 0)
                            cmd.Parameters.AddWithValue("@CreatedDate", e.createdDate);

                        if (e.publicationDate.Ticks > 0)
                            cmd.Parameters.AddWithValue("@PublicationDate", e.publicationDate);
                    }

                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (dr != null && dr.HasRows)
                        {

                            while (dr.Read())
                            {
                                Issue item = new Issue();
                                item.id = dr.GetInt32(0);
                                item.publicationID = dr.GetInt32(1);
                                item.publicationName = dr.GetString(2);
                                item.chiefEditorId = (dr.IsDBNull(3) ? 0 : dr.GetInt32(3));
                                item.chiefEditorName = (dr.IsDBNull(4) ? "" : dr.GetString(4));
                                item.title = dr.GetString(5);
                                if (!dr.IsDBNull(6))
                                    item.createdDate = dr.GetDateTime(6);
                                if (!dr.IsDBNull(7))
                                    item.publicationDate = dr.GetDateTime(7);
                                item.Notes = (dr.IsDBNull(8) ? "" : dr.GetString(8));
                                item.wfState = dr.GetInt32(9);



                                //if (!dr.IsDBNull(10))
                                //    item.DateCreated = dr.GetDateTime(10);
                                //if (!dr.IsDBNull(11))
                                //    item.DateUpdated = dr.GetDateTime(11);

                                item.articlesCnt = (dr.IsDBNull(12) ? 0 : dr.GetInt32(12));
                                item.articlesPublishedCnt = (dr.IsDBNull(13) ? 0 : dr.GetInt32(13));
                                item.tasteNotesCnt = (dr.IsDBNull(14) ? 0 : dr.GetInt32(14));
                                item.tasteNotesPublishedCnt = (dr.IsDBNull(15) ? 0 : dr.GetInt32(15));
                                item.tastingEventsCnt = (dr.IsDBNull(16) ? 0 : dr.GetInt32(16));
                                item.tastingEventsPublishedCnt = (dr.IsDBNull(17) ? 0 : dr.GetInt32(17));

                                res.Add(item);
                            } 
                        }
                        if (dr != null && !dr.IsClosed)
                            dr.Close();
                    } 
                } 
            } 

            return res;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueID"></param>
        /// <returns></returns>
        public IEnumerable<TastingEvent> GetAssignments(int issueID)
        {


            return null;
        }



        public IEnumerable<PublicationItem> GetPublications()
        {
            List<PublicationItem> result = new List<PublicationItem>();

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {

                    cmd.CommandText = "select id, publisherid, name from publication";


                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            PublicationItem ev = new PublicationItem()
                            {
                                id = rdr.GetInt32(0),
                                publisherId = rdr.GetInt32(1),
                                name = rdr.GetString(2)
                            };

                            result.Add(ev);
                        }
                    }
                    return result;
                }
            }
        }


        public void UpdateStatus(int issueId, int fromStatus, int toStatus)
        {
            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {

                    cmd.CommandText = @"
                        update tn set tn.WF_StatusID = @ToStatus 
                        from  Issue as i
                         join Assignment as a on a.IssueID = i.ID
                         join Assignment_TastingEvent as ate on ate.AssignmentID = a.ID
                         join TastingEvent as te on te.ID = ate.TastingEventID
                         join TastingEvent_TasteNote as tet on tet.TastingEventID = te.ID
                         join TasteNote as tn on tn.ID = tet.TasteNoteID 
                        where i.ID = @IssueId and tn.WF_StatusID = @FromStatus

";

                    cmd.Parameters.AddWithValue("@IssueId", issueId);
                    cmd.Parameters.AddWithValue("@FromStatus", fromStatus);
                    cmd.Parameters.AddWithValue("@ToStatus", toStatus);


                    cmd.ExecuteScalar();

                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        public void Publish(int issueId)
        {
            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {

                    cmd.CommandText = "Issue_Publish";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 360;
                    cmd.Parameters.AddWithValue("@ID", issueId);
                    cmd.ExecuteScalar();

                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        public void RollbackPublish(int issueId)
        {
            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = "Issue_Unpublish";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 360;
                    cmd.Parameters.AddWithValue("@ID", issueId);
                    cmd.ExecuteScalar();
                }
            }
        }



        public IssueComplete LoadIssueComplete(int issueId, int status)
        {

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"

-- read issue --
select 
    i.ID,
    u.UserId, u.FullName,
    i.PublicationID, p.Name,
    i.Title, i.Notes, i.CreatedDate, i.PublicationDate, i.WF_StatusID  
    from  Issue as i
    left join Users as u on i.ChiefEditorUserId = u.UserId
    left join Publication as p on i.PublicationID = p.ID
where i.ID = @IssueId    


-- read assignment and its resources ***********************************************************************************
               
select  a.ID, a.Title ,a.Notes, a.WF_StatusID, a.created, a.updated,u.UserId,u.FullName  from   Assignment as a 
    left join Users as u on a.AuthorId = u.UserId
where a.IssueID = @IssueId


select   ar.AssignmentID, ar.UserId,  u.FullName, ar.UserRoleID  from   Assignment as a 
    left join Assignment_Resource as ar on a.ID = ar.AssignmentID
    left join Users as u on ar.UserId = u.UserId
where a.IssueID = @IssueId
                        

select  ar.AssignmentID, ar.TypeID, ar.Deadline  from   Assignment as a 
    left join Assignment_ResourceD as ar on a.ID = ar.AssignmentID
where a.IssueID = @IssueId

-- read tasting events *************************************************************************************************

select a.ID, te.ID, te.Location, te.Title, te.Notes, te.created, te.updated  from  Issue as i
    join Assignment as a on a.IssueID = i.ID
    join Assignment_TastingEvent as ate on ate.AssignmentID = a.ID
    join TastingEvent as te on te.ID = ate.TastingEventID
where i.ID = @IssueId    
order by a.ID    

-- read tasting notes **************************************************************************************************

                        
select 

        te.ID,
        tn.ID,
		tn.OriginID,
		tn.UserId,
		u.FullName,
		
		tn.Wine_N_ID,
		w.ProducerID, w.ProducerToShow,
		w.Country,w.Region,w.Location,w.Locale,w.Site,
		w.Label,
		w.Vintage,
		w.Name,
		
		w.Type,
		w.Variety,
		w.Dryness,
		w.Color,
		
		tn.TasteDate, 

		tn.MaturityID, 
		wm.Name,
		wm.Suggestion,

		tn.Rating_Lo, 
		tn.Rating_Hi, 
		tn.DrinkDate_Lo, 
		tn.DrinkDate_Hi, 
		tn.IsBarrelTasting, 
		tn.Notes, 

		tn.EstimatedCost,
		tn.EstimatedCost_Hi, 
		WF_StatusID = tn.WF_StatusID,
        w.Wine_N_WF_StatusID,
		w.Vin_N_WF_StatusID,
		tn.created, 
		tn.updated 
        ,RatingQ
        ,Importers =  STUFF(  (select '+'+'---new-line---'+ Name 
                     +  case
                          when LEN( isnull(Address,'')) > 0 then (', ' + Address )
                         else ''
                        end   
                     +  case
                          when LEN( isnull(Phone1,'')) > 0 then (', ' + Phone1 )
                         else ''
                        end   
                     +  case
                          when LEN( isnull(URL,'')) > 0 then (', ' + URL)
                         else ''
                        end   
                    from WineImporter wi
                    join WineProducer_WineImporter wpi  (nolock) on wpi.ImporterId  = wi.ID
                    where 
                    wpi.ProducerId = w.ProducerID
                    FOR XML PATH('')), 1, 1, '' )


from  Issue as i

join Assignment as a on a.IssueID = i.ID
join Assignment_TastingEvent as ate on ate.AssignmentID = a.ID
join TastingEvent as te on te.ID = ate.TastingEventID
join TastingEvent_TasteNote as tet on tet.TastingEventID = te.ID
join TasteNote as tn on tn.ID = tet.TasteNoteID 
join vWineDetails as w  on w.Wine_N_ID = tn.Wine_N_ID 
left join Users as u on tn.UserId  = u.UserId
left join WineMaturity wm (nolock) on tn.MaturityID = wm.ID
                         
where i.ID = @IssueId and tn.WF_StatusID >= @Status 
order by te.ID

";

                    cmd.Parameters.AddWithValue("@IssueId", issueId);
                    cmd.Parameters.AddWithValue("@Status", status);

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {

                        IssueComplete result = new IssueComplete();
                        Dictionary<int, AssignmentItemComplete> assignmentsDictionary = new Dictionary<int, AssignmentItemComplete>();


                        if (rdr.Read())
                        {
                            result.id = rdr.GetInt32(0);
                            result.chiefEditorId = rdr.IsDBNull(1) ? 0 : rdr.GetInt32(1);
                            result.chiefEditorName = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
                            result.publicationID = rdr.IsDBNull(3) ? 0 : rdr.GetInt32(3);
                            result.publicationName = rdr.IsDBNull(4) ? "" : rdr.GetString(4);
                            result.title = rdr.IsDBNull(5) ? "" : rdr.GetString(5);
                            result.Notes = rdr.IsDBNull(6) ? "" : rdr.GetString(6);

                            if (!rdr.IsDBNull(7))
                                result.createdDate = rdr.GetDateTime(7);

                            if (!rdr.IsDBNull(8))
                                result.publicationDate = rdr.GetDateTime(8);

                            result.wfState = rdr.GetFieldValue<Int32>(9);

                            // read assignments **************************************************************
                            if (!rdr.NextResult())
                                return null;

                            while (rdr.Read())
                            {
                                AssignmentItemComplete item = new AssignmentItemComplete();

                                item.id = rdr.GetInt32(0);


                                if (!rdr.IsDBNull(1)) item.title = rdr.GetString(1);
                                if (!rdr.IsDBNull(2)) item.notes = rdr.GetString(2);
                                if (!rdr.IsDBNull(3)) item.wfState = rdr.GetInt16(3);


                                if (!rdr.IsDBNull(4)) item.CreatedDate = rdr.GetDateTime(4);
                                if (!rdr.IsDBNull(5)) item.UpdatedDate = rdr.GetDateTime(5);

                                if (!rdr.IsDBNull(6) && !rdr.IsDBNull(7))
                                {
                                    item.ProcessActor(rdr.GetInt32(6), ActorRole.reviewer, rdr.GetString(7));
                                }

                                assignmentsDictionary.Add(item.id, item);

                            }



                            if (!rdr.NextResult())
                            {
                                return null;
                            }
                            while (rdr.Read())
                            {
                                if (!rdr.IsDBNull(0))
                                {
                                    int id = rdr.GetInt32(0);
                                    AssignmentItem item = assignmentsDictionary[id];
                                    item.ProcessActor(rdr.GetInt32(1), (ActorRole)rdr.GetInt32(3), rdr.IsDBNull(2) ? "" : rdr.GetString(2));
                                }
                            }

                            if (!rdr.NextResult())
                            {
                                return null;
                            }
                            while (rdr.Read())
                            {
                                if (!rdr.IsDBNull(0))
                                {
                                    int id = rdr.GetInt32(0);

                                    AssignmentItem item = assignmentsDictionary[id];
                                    item.ProcessDeadline((DeadlineType)rdr.GetInt32(1), rdr.GetDateTime(2));
                                }
                            }


                            //
                            // read tasting records
                            //
                            if (!rdr.NextResult())
                                return null;

                            Dictionary<Int32, TastingEventComplete> tastingEvents = new Dictionary<Int32, TastingEventComplete>();
                            AssignmentItemComplete ta = null;
                            while (rdr.Read())
                            {
                                TastingEventComplete item = new TastingEventComplete();

                                item.assignmentId = rdr.GetInt32(0);
                                item.id = rdr.GetInt32(1);
                                item.location = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
                                item.title = rdr.IsDBNull(3) ? "" : rdr.GetString(3);
                                item.comments = rdr.IsDBNull(4) ? "" : rdr.GetString(4);
                                item.created = rdr.GetDateTime(5);
                                tastingEvents.Add(item.id, item);

                                if (ta == null || ta.id != item.assignmentId)
                                {
                                    ta = assignmentsDictionary[item.assignmentId];
                                    if (ta.tastingEvents == null)
                                        ta.tastingEvents = new List<TastingEventComplete>();
                                }
                                ((List<TastingEventComplete>)ta.tastingEvents).Add(item);

                            }

                            if (!rdr.NextResult())
                                return null;


                            List<TastingNote> tastingNotes = new List<TastingNote>();

                            TastingEventComplete te = null;
                            while (rdr.Read())
                            {
                                int tastingEventId = rdr.GetInt32(0);

                                if (te == null || te.id != tastingEventId)
                                {
                                    te = tastingEvents[tastingEventId];
                                    if (te.tastingNotes == null)
                                        te.tastingNotes = new List<TastingNote>();
                                }

                                ((List<TastingNote>)te.tastingNotes).Add(ReadTastingFromDb(rdr));
                            }

                            result.assignments = assignmentsDictionary.Values.ToList<AssignmentItemComplete>();

                            return result;
                        }
                    }

                }
            }

            return null;
        }

        private static TastingNote ReadTastingFromDb(SqlDataReader rdr)
        {
            var nullDate = new DateTime(0);
            TastingNote note = new TastingNote();

            note.id = rdr.GetInt32(1);
            note.noteId = rdr.IsDBNull(2) ? 0 : rdr.GetInt32(2);
            note.userId = rdr.IsDBNull(3) ? 0 : rdr.GetInt32(3);
            note.reviewer = rdr.IsDBNull(4) ? "" : rdr.GetString(4);
            note.wineN = rdr.GetInt32(5);

            note.producer = rdr.IsDBNull(7) ? "" : rdr.GetString(7);
            note.country = rdr.IsDBNull(8) ? "" : rdr.GetString(8);
            note.region = rdr.IsDBNull(9) ? "" : rdr.GetString(9);
            note.location = rdr.IsDBNull(10) ? "" : rdr.GetString(10);
            note.locale = rdr.IsDBNull(11) ? "" : rdr.GetString(11);
            note.site = rdr.IsDBNull(12) ? "" : rdr.GetString(12);

            note.wineName = rdr.IsDBNull(13) ? "" : rdr.GetString(13);
            note.vintage = rdr.IsDBNull(14) ? "" : rdr.GetString(14);

            note.wineType = rdr.IsDBNull(16) ? "" : rdr.GetString(16);
            note.variety = rdr.IsDBNull(17) ? "" : rdr.GetString(17);
            note.dryness = rdr.IsDBNull(18) ? "" : rdr.GetString(18);
            note.color = rdr.IsDBNull(19) ? "" : rdr.GetString(19);

            note.tastingDate = rdr.IsDBNull(20) ? nullDate : rdr.GetDateTime(20);

            note.ratingLo = rdr.IsDBNull(24) ? (short)0 : rdr.GetFieldValue<Int16>(24);
            note.ratingHi = rdr.IsDBNull(25) ? (short)0 : rdr.GetFieldValue<Int16>(25);
            note.encodeRating();

            if (!rdr.IsDBNull(26))
                note.drinkDateLo = rdr.GetDateTime(26);
            if (!rdr.IsDBNull(27))
                note.drinkDateHi = rdr.GetDateTime(27);

            note.isBarrelTasting = rdr.IsDBNull(28) ? false : rdr.GetBoolean(28);

            note.note = rdr.IsDBNull(29) ? "" : rdr.GetString(29);

            note.estimatedCost = rdr.IsDBNull(30) ? "" : rdr.GetDecimal(30).ToString("0.##");
            note.estimatedCostHi = rdr.IsDBNull(31) ? "" : rdr.GetDecimal(31).ToString("0.##");
            note.estimatedCost = note.estimatedCost.CompareTo("0") == 0 ? "" : note.estimatedCost;
            note.estimatedCostHi = note.estimatedCostHi.CompareTo("0") == 0 ? "" : note.estimatedCostHi;

            note.wfState = rdr.GetFieldValue<Int16>(32);
            note.wfStateWineN = rdr.GetFieldValue<Int16>(33);
            note.wfStateVinN = rdr.GetFieldValue<Int16>(34);

            note.ratingQ   = rdr.IsDBNull(37) ? "" : rdr.GetString(37);
            note.importers = rdr.IsDBNull(38) ? "" : rdr.GetString(38);
            note.importers = note.importers.Replace("---new-line---", "");

            return note;
        }


        public IssueComplete AddIssueComplete(IssueComplete issue)
        {
            using (SqlConnection conn = _connFactory.GetConnection())
            {
                SqlTransaction transaction = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand("", conn, transaction);
              
                try
                {
                    using(cmd){
                        AddIssue(issue, cmd);
                        AddAssigment(issue, cmd);
                        AddTastingRecord(issue,cmd);
                        AddTastingNote(issue,cmd);
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }

            }

            return issue;

        }

        private void AddTastingNote(IssueComplete issue, SqlCommand cmd)
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =

            @" 
                            declare @Wine_N_ID int;
                            declare @NoteId int;
       
                            set nocount on;
                
                            exec @Wine_N_ID = Wine_GetID @Producer=@Producer, @WineType=@WineType, @Label=@Label, @Variety=@Variety,
                                         @Dryness=@Dryness, @Color=@Color,@Vintage=@Vintage,
                                         @locCountry= @locCountry, @locRegion=@locRegion,@locLocation=@locLocation, @locLocale=@locLocale, @locSite=@locSite;


                            exec @NoteId = TastingNote_Add @UserId = @UserId,@Wine_N_ID=@Wine_N_ID,@TasteDate=@TasteDate,@MaturityID=@MaturityID,
                                 @Rating_Lo=@Rating_Lo, @Rating_Hi= @Rating_Hi,@DrinkDate_Lo=@DrinkDate_Lo,@DrinkDate_Hi=@DrinkDate_Hi,
                                 @EstimatedCost=@EstimatedCost, @EstimatedCost_Hi= @EstimatedCost_Hi,
                                 @IsBarrelTasting=@IsBarrelTasting, @Notes=@Notes, @IssueID = @IssueId, @WF_StatusID = @WF_StatusID;


                            exec TastingEvent_TasteNote_Add @TastingEventID=@TastingEventID, @TasteNote=@NoteId;
                            ";


            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@IssueId", issue.id);
            cmd.Parameters.AddWithValue("@Producer", "");
            cmd.Parameters.AddWithValue("@WineType", "");
            cmd.Parameters.AddWithValue("@Label", "");
            cmd.Parameters.AddWithValue("@Variety", "");
            cmd.Parameters.AddWithValue("@Dryness", "");
            cmd.Parameters.AddWithValue("@Color", "");
            cmd.Parameters.AddWithValue("@Vintage", "");
            cmd.Parameters.AddWithValue("@locCountry", "");
            cmd.Parameters.AddWithValue("@locRegion", "");
            cmd.Parameters.AddWithValue("@locLocation", "");
            cmd.Parameters.AddWithValue("@locLocale", "");
            cmd.Parameters.AddWithValue("@locSite", "");

            cmd.Parameters.AddWithValue("@UserId", "");
            cmd.Parameters.AddWithValue("@TasteDate", "");
            cmd.Parameters.AddWithValue("@MaturityID", "");

            cmd.Parameters.AddWithValue("@Rating_Lo", "");
            cmd.Parameters.AddWithValue("@Rating_Hi", "");
            cmd.Parameters.AddWithValue("@DrinkDate_Lo", DateTime.Now);
            cmd.Parameters.AddWithValue("@DrinkDate_Hi", DateTime.Now);

            cmd.Parameters.AddWithValue("@EstimatedCost",  Decimal.Parse("0"));
            cmd.Parameters.AddWithValue("@EstimatedCost_Hi",  Decimal.Parse("0"));
            cmd.Parameters.AddWithValue("@IsBarrelTasting", false);

            cmd.Parameters.AddWithValue("@TastingEventID", 0);
            cmd.Parameters.AddWithValue("@Notes", "");
            cmd.Parameters.AddWithValue("@WF_StatusID", 0);


            foreach (var assignment in issue.assignments)
            {
                foreach (var tr in assignment.tastingEvents)
                {
                    foreach (var tn in tr.tastingNotes)
                    {

                        cmd.Parameters["@Producer"].Value = String.IsNullOrEmpty(tn.producer) ? "" : tn.producer;
                        cmd.Parameters["@WineType"].Value = String.IsNullOrEmpty(tn.wineType) ? "" : tn.wineType;
                        cmd.Parameters["@Label"].Value = String.IsNullOrEmpty(tn.wineName) ? "" : tn.wineName;
                        cmd.Parameters["@Variety"].Value= String.IsNullOrEmpty(tn.variety) ? "" : tn.variety;
                        cmd.Parameters["@Dryness"].Value = String.IsNullOrEmpty(tn.dryness) ? "" : tn.dryness;
                        cmd.Parameters["@Color"].Value = String.IsNullOrEmpty(tn.color) ? "" : tn.color;
                        cmd.Parameters["@Vintage"].Value = String.IsNullOrEmpty(tn.vintage) ? "" : tn.vintage;
                        cmd.Parameters["@locCountry"].Value = String.IsNullOrEmpty(tn.country) ? "" : tn.country;
                        cmd.Parameters["@locRegion"].Value = String.IsNullOrEmpty(tn.region) ? "" : tn.region;
                        cmd.Parameters["@locLocation"].Value = String.IsNullOrEmpty(tn.location) ? "" : tn.location;
                        cmd.Parameters["@locLocale"].Value = String.IsNullOrEmpty(tn.locale) ? "" : tn.locale;
                        cmd.Parameters["@locSite"].Value = String.IsNullOrEmpty(tn.site) ? "" : tn.site;

                        cmd.Parameters["@UserId"].Value = tn.userId;
                        cmd.Parameters["@TasteDate"].Value = tn.tastingDate;

                        cmd.Parameters["@MaturityID"].Value = tn.maturityId;
                        cmd.Parameters["@Rating_Lo"].Value = tn.ratingLo;
                        cmd.Parameters["@Rating_Hi"].Value = tn.ratingHi;

                        if (tn.drinkDateLo.Ticks > 0){
                            cmd.Parameters["@DrinkDate_Lo"].Value = tn.drinkDateLo;
                        } else {
                            cmd.Parameters["@DrinkDate_Lo"].Value = DBNull.Value;
                        }


                        if (tn.drinkDateHi.Ticks > 0) {
                            cmd.Parameters["@DrinkDate_Hi"].Value =tn.drinkDateHi;
                        } else {
                            cmd.Parameters["@DrinkDate_Hi"].Value = DBNull.Value;
                        }


                        cmd.Parameters["@EstimatedCost"].Value =  String.IsNullOrEmpty(tn.estimatedCost) ? 0 : Decimal.Parse(tn.estimatedCost);
                        cmd.Parameters["@EstimatedCost_Hi"].Value =  String.IsNullOrEmpty(tn.estimatedCostHi) ? 0 : Decimal.Parse(tn.estimatedCostHi);
                        cmd.Parameters["@IsBarrelTasting"].Value = tn.isBarrelTasting;

                        cmd.Parameters["@TastingEventID"].Value = tr.id;
                        cmd.Parameters["@Notes"].Value = String.IsNullOrEmpty(tn.note) ? "" : tn.note;
                        cmd.Parameters["@WF_StatusID"].Value =  tn.wfState;


                        using (var rdr = cmd.ExecuteReader())
                        {
                            int id = 0, wineN = 0;

                            //
                            // order is important.
                            //
                            //
                            if (rdr.Read())
                            {
                                wineN = rdr.GetInt32(0);
                            }
                            if (rdr.NextResult() && rdr.Read())
                            {
                                id = rdr.GetInt32(0);
                            }

                            tn.id = id;
                            tn.wineN = wineN;

                        }
                    }
                }
            }


        }

        private void AddTastingRecord(IssueComplete issue, SqlCommand cmd)
        {

            cmd.CommandText = @" 
                        
            declare @Result int;
       
            set nocount on;
                 
            exec @Result = TastingEvent_Add @Title = @Title ,@Location = @Location, @Notes = @Notes;

            exec Assignment_TastingEvent_Add @AssignmentID=@AssignmentID, @TastingEventID = @Result;

            select @Result;
            ";

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Title", "");
            cmd.Parameters.AddWithValue("@Location", "");
            cmd.Parameters.AddWithValue("@Notes", "");
            cmd.Parameters.AddWithValue("@AssignmentID", 0);

            foreach (var assignment in issue.assignments)
            {
                foreach (var tr in assignment.tastingEvents)
                {
                    tr.assignmentId = assignment.id;

                    cmd.Parameters["@Title"].Value = tr.title;
                    cmd.Parameters["@Location"].Value = tr.location;
                    cmd.Parameters["@Notes"].Value = tr.comments;
                    cmd.Parameters["@AssignmentID"].Value = tr.assignmentId;
                    using (var rdr = cmd.ExecuteReader())
                    {
                        // first read should be number of records inserted. 
                        // seconds read is tasting event ID
                        //
                        if (rdr.Read())
                        {
                            if (rdr.GetInt32(0) != 1)
                                throw new Exception("error in logic. no tasting event assigned to a assignment");

                            if (!rdr.NextResult() || !rdr.Read())
                                throw new Exception("error in logic. no tasting event id returned");

                            tr.id = rdr.GetInt32(0);
                        }
                    }
                }
            }
        }

        private void AddAssigment(IssueComplete issue, SqlCommand cmd)
        {
            cmd.CommandText = "Assignment_Add";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@IssueID", 0);
            cmd.Parameters.AddWithValue("@Title", "");
            cmd.Parameters.AddWithValue("@Notes", "");
            cmd.Parameters.AddWithValue("@WF_StatusID", 0);


            foreach(var assignment in issue.assignments){

                assignment.issueId = issue.id;
                cmd.Parameters["@IssueID"].Value =assignment.issueId;
                cmd.Parameters["@Title"].Value = assignment.title;
                cmd.Parameters["@Notes"].Value = assignment.notes;
                cmd.Parameters["@WF_StatusID"].Value = assignment.wfState;

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                            assignment.id = dr.GetInt32(0);
                    }
                } 
            }

            cmd.CommandText = "Assignment_Resource_AddUpdate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@AssignmentID", 0);
            cmd.Parameters.AddWithValue("@UserId", 0);
            cmd.Parameters.AddWithValue("@UserRoleID", ActorRole.unknown);
            cmd.Parameters.AddWithValue("@ShowRes", 1);

            foreach (var assignment in issue.assignments)
            {
                assignment.issueId = issue.id;
                cmd.Parameters["@AssignmentID"].Value = assignment.id;

                if (assignment.editor.id > 0)
                {
                    cmd.Parameters["@UserId"].Value = assignment.editor.id;
                    cmd.Parameters["@UserRoleID"].Value = assignment.editor.role;
                    cmd.ExecuteScalar();
                }

                if (assignment.author.id > 0)
                {
                    cmd.Parameters["@UserId"].Value = assignment.author.id;
                    cmd.Parameters["@UserRoleID"].Value = assignment.author.role;
                    cmd.ExecuteScalar();
                }
                if (assignment.proofread.id > 0)
                {
                    cmd.Parameters["@UserId"].Value = assignment.proofread.id;
                    cmd.Parameters["@UserRoleID"].Value = assignment.proofread.role;
                    cmd.ExecuteScalar();
                }
            }



        }

        private void AddIssue(IssueComplete issue, SqlCommand cmd)
        {
            cmd.CommandText =  "Issue_Add";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@PublicationID", issue.publicationID);
            cmd.Parameters.AddWithValue("@ChiefEditorUserId", issue.chiefEditorId);
            cmd.Parameters.AddWithValue("@Title", issue.title);

            if( issue.createdDate.Ticks > 0)
               cmd.Parameters.AddWithValue("@CreatedDate", issue.createdDate);
            else
                cmd.Parameters.AddWithValue("@CreatedDate", DBNull.Value);

            if( issue.publicationDate.Ticks > 0 )
               cmd.Parameters.AddWithValue("@PublicationDate", issue.publicationDate);
            else
                cmd.Parameters.AddWithValue("@PublicationDate", DBNull.Value);

            cmd.Parameters.AddWithValue("@Notes", String.IsNullOrEmpty(issue.Notes) ? "" : issue.Notes);

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read())
                {
                   issue.id = dr.GetFieldValue<Int32>(0);
                }
            } 
        }


    }
}
