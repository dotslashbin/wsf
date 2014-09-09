using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EditorsCommon;
using EditorsCommon.Publication;
using System.Data.SqlClient;
using System.Data;

namespace EditorsDbLayer.Data.Publication 
{

    
   /*
    * still in development 
    * 
    * 
    	select 
		ID = a.ID,
		Title = a.Title,
		Notes = a.Notes,
		created = a.created, 
		updated = a.updated,
		
		WF_StatusID = isnull(wfs.ID, -1),
		
		IssueID = a.IssueID,
		IssueTitle = i.Title,
		PublicationID = p.ID,
		PublicationName = p.Name
		
	from  

	    Assignment a (nolock)
	
		join Issue i (nolock) on a.IssueID = i.ID
		join Publication p (nolock) on i.PublicationID = p.ID
		join WF_Statuses wfs (nolock) on a.WF_StatusID = wfs.ID

	where a.IssueID = 311


---------------------------------------

	select 
		ID = a.ID,
		IssueID = a.IssueID,
		userId = u.UserId  ,
		userRoleId = u.UserRoleID,
		userFullName = uu.FullName
		
	from  

	    Assignment a (nolock)
		join Assignment_Resource u (nolock) on a.ID = u.AssignmentID
		join Users uu (nolock) on u.UserId = uu.UserId

	where a.IssueID = 311

----------------------------------------

select 
		ID = a.ID,
		IssueID = a.IssueID,
		deadlineId = d.TypeID,
		deadline = d.Deadline
		
	from Assignment a (nolock)
		join Issue i (nolock) on a.IssueID = i.ID
		join Assignment_ResourceD d (nolock) on a.ID = d.AssignmentID
	where a.IssueID = 311
-----------------------------------------------------------------

		  select 
		  SUM( case when tn.WF_StatusID < 100 then 1 else 0 end) as published,
		  notesCount = COUNT(tn.ID), 
		  AssignmentID = aa.ID 
		  from Assignment as aa   
		  left join Assignment_TastingEvent as ate on ate.AssignmentID = aa.ID
		  left join TastingEvent_TasteNote  as ten on ten.TastingEventID = ate.TastingEventID
		  left join TasteNote as tn on ten.TasteNoteID = tn.ID
		  where aa.IssueID = 311
		  group by aa.ID  


    * * 
    * 
    * 
    * 
    */
    
    
    
    
    
    public class AssignmentStorage : IAssignmentStorage {

        private ISqlConnectionFactory _connFactory;


        public AssignmentStorage(ISqlConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public AssignmentItem GetByID(int id) {
            List<AssignmentItem> list = Load(id);
            return list.First();
        }

        public AssignmentItem Create(AssignmentItem e) {
            return Save(e);
        }

        public IEnumerable<AssignmentItem> Search(AssignmentItem e) {
            return Load(null, e);
        }

        public AssignmentItem Update(AssignmentItem e) {
            return Save(e);
        }

        public Boolean DeleteAssignment(int assignmentID)
        {
            using (SqlConnection connection = _connFactory.GetConnection())
            {

                string SQL = ("Assignment_Del");

                using (SqlCommand command = new SqlCommand(SQL, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Assigning paramters for the stored procedure call
                    command.Parameters.AddWithValue("@ID", assignmentID);
                    command.Parameters.AddWithValue("@ShowRes", 1);

                    // Reading the results
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {

                        if (dataReader.Read())
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public AssignmentItem Delete(AssignmentItem e)
        {
            if (e == null || e.id < 1)
            {
                throw new ArgumentException("Assignment must have a positive id to be deleted.");
            }
            Del(e.id);
            return e;
        }


        /// <summary>
        /// Gets Assignments.
        /// </summary>
        /// <param name="id">If set to a positive number, only 1 Assignment will be loaded into list.</param>
        /// <returns></returns>
        protected List<AssignmentItem> Load(int? id, AssignmentItem filter = null, int? recourceUserId = null) {
            // Assignment_GetList @ID int = NULL, --@AuthorId int = NULL, 
            //      @Title nvarchar(255) = NULL, @WF_StatusID smallint = NULL, @Resource_UserID int = NULL,
            // Returns:
            // ID, --AuthorId, --AuthorName, Title, Deadline, Notes, 
            // created, updated, WF_StatusID, WF_StatusName,
            // IssueID,	IssueTitle,	PublicationID, PublicationName
            List<AssignmentItem> res = new List<AssignmentItem>();

                using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand("Assignment_GetList", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (id.HasValue && id.Value > 0)
                            cmd.Parameters.AddWithValue("@ID", id.Value);
                        else if (filter != null && filter.id > 0)
                            cmd.Parameters.AddWithValue("@ID", filter.id);

                        if (recourceUserId.HasValue)
                            cmd.Parameters.AddWithValue("@Resource_UserID", recourceUserId.Value);

                        if (filter != null) {
                            //if (filter.AuthorId > 0)
                            //    cmd.Parameters.AddWithValue("@AuthorId", filter.AuthorId);
                            if (!string.IsNullOrWhiteSpace(filter.title))
                                cmd.Parameters.AddWithValue("@Title", filter.title);
                            if (filter.wfState != short.MinValue)
                                cmd.Parameters.AddWithValue("@WF_StatusID", filter.wfState);
                            if (filter.issueId > 0)
                                cmd.Parameters.AddWithValue("@IssueID", filter.issueId);
                        }


                        AssignmentItem item = null;

                        using (SqlDataReader dr = cmd.ExecuteReader()) {
                                while (dr.Read()) {
                                    //item = new AssignmentItem();

                                    int assignmentId = dr.GetInt32(0);


                                    if (item == null || item.id != assignmentId)
                                    {
                                        item = new AssignmentItem();

                                        item.id = assignmentId;

                                        if (!dr.IsDBNull(1)) item.title = dr.GetString(1);
                                        if (!dr.IsDBNull(3)) item.notes = dr.GetString(3);

                                        if (!dr.IsDBNull(4)) item.CreatedDate = dr.GetDateTime(4);
                                        if (!dr.IsDBNull(5)) item.UpdatedDate = dr.GetDateTime(5);
                                        if (!dr.IsDBNull(6)) item.wfState = dr.GetInt16(6);
                                        //if (!dr.IsDBNull(7)) item.WF_StatusName = dr.GetString(7);

                                        if (!dr.IsDBNull(8)) item.issueId = dr.GetInt32(8);
                                        if (!dr.IsDBNull(9)) item.issue = dr.GetString(9);
                                        if (!dr.IsDBNull(10)) item.publicationId = dr.GetInt32(10);
                                        if (!dr.IsDBNull(11)) item.publication = dr.GetString(11);

                                        item.notesCount = dr.GetInt32(17);

                                        res.Add(item);
                                    }

                                    int userId = dr.IsDBNull(12) ? 0 :  dr.GetInt32(12);

                                    if (userId > 0)
                                    {

                                        item.ProcessActor(userId, (ActorRole)dr.GetInt32(13), dr.IsDBNull(14) ? "" : dr.GetString(14));

                                    }

                                    int deadlineId = dr.IsDBNull(15) ? -1 : dr.GetInt32(15);
                                    if (deadlineId >= 0)
                                    {

                                        item.ProcessDeadline((DeadlineType)deadlineId, dr.GetDateTime(16));

                                    }
                                }
                        } // datareader
                    } // sqlCommand
                } // sqlConn

            return res;
        } // Load


        protected List<AssignmentItem> LoadByIssueId(int issueId)
        {
            Dictionary<int, AssignmentItem> dict = new Dictionary<int, AssignmentItem>();

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("Assignment_GetList_ByIssueId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IssueID", issueId);


                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            AssignmentItem item = new AssignmentItem();

                            item.id = dr.GetInt32(0);


                                if (!dr.IsDBNull(1)) item.title = dr.GetString(1);
                                if (!dr.IsDBNull(3)) item.notes = dr.GetString(3);

                                if (!dr.IsDBNull(4)) item.CreatedDate = dr.GetDateTime(4);
                                if (!dr.IsDBNull(5)) item.UpdatedDate = dr.GetDateTime(5);
                                if (!dr.IsDBNull(6)) item.wfState = dr.GetInt16(6);

                                if (!dr.IsDBNull(8)) item.issueId = dr.GetInt32(8);
                                if (!dr.IsDBNull(9)) item.issue = dr.GetString(9);
                                if (!dr.IsDBNull(10)) item.publicationId = dr.GetInt32(10);
                                if (!dr.IsDBNull(11)) item.publication = dr.GetString(11);

                                item.notesCount = dr.GetInt32(12);

                                dict.Add(item.id, item);
                        }

                        if (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                int id = dr.GetInt32(0);

                                AssignmentItem item = dict[id];
                                item.ProcessActor(dr.GetInt32(1), (ActorRole)dr.GetInt32(2), dr.IsDBNull(3) ? "" : dr.GetString(3));
                            }
                        }

                        if (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                int id = dr.GetInt32(0);

                                AssignmentItem item = dict[id];
                                item.ProcessDeadline((DeadlineType)dr.GetInt32(1), dr.GetDateTime(2));
                            }
                        }

                    } 
                } 
            } 

            return dict.Values.ToList();
        }


        protected List<AssignmentItem> LoadByUserId(int issueId)
        {
            Dictionary<int, AssignmentItem> dict = new Dictionary<int, AssignmentItem>();

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("Assignment_GetList_ByUserId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserID", issueId);


                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            AssignmentItem item = new AssignmentItem();

                            item.id = dr.GetInt32(0);


                            if (!dr.IsDBNull(1)) item.title = dr.GetString(1);
                            if (!dr.IsDBNull(3)) item.notes = dr.GetString(3);

                            if (!dr.IsDBNull(4)) item.CreatedDate = dr.GetDateTime(4);
                            if (!dr.IsDBNull(5)) item.UpdatedDate = dr.GetDateTime(5);
                            if (!dr.IsDBNull(6)) item.wfState = dr.GetInt16(6);

                            if (!dr.IsDBNull(8)) item.issueId = dr.GetInt32(8);
                            if (!dr.IsDBNull(9)) item.issue = dr.GetString(9);
                            if (!dr.IsDBNull(10)) item.publicationId = dr.GetInt32(10);
                            if (!dr.IsDBNull(11)) item.publication = dr.GetString(11);

                            item.notesCount = dr.GetInt32(12);

                            dict.Add(item.id, item);
                        }

                        if (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                int id = dr.GetInt32(0);

                                AssignmentItem item = dict[id];
                                item.ProcessActor(dr.GetInt32(1), (ActorRole)dr.GetInt32(2), dr.IsDBNull(3) ? "" : dr.GetString(3));
                            }
                        }

                        if (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                int id = dr.GetInt32(0);

                                AssignmentItem item = dict[id];
                                item.ProcessDeadline((DeadlineType)dr.GetInt32(1), dr.GetDateTime(2));
                            }
                        }

                    }
                }
            }

            return dict.Values.ToList();
        } 

        /// <summary>
        /// Saves all Issue attributes.
        /// </summary>
        /// <returns></returns>
        protected AssignmentItem Save(AssignmentItem item) {
            if (item == null)
                throw new ArgumentException("AssignedItem is required.");


            // Assignment_Add or Assignment_Update
                // --@ID int = NULL, @IssueID int,
	            // @AuthorId int = NULL,  @Title nvarchar(255), @Deadline date = NULL, @Notes nvarchar(max) = NULL, @WF_StatusID,
                // @ShowRes smallint = 1
                //
                using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand((item.id > 0 ? "Assignment_Update" : "Assignment_Add"), conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (item.id > 0)
                            cmd.Parameters.AddWithValue("@ID", item.id);
                        cmd.Parameters.AddWithValue("@IssueID", item.issueId);
                        cmd.Parameters.AddWithValue("@Title", item.title);
                        cmd.Parameters.AddWithValue("@Notes", item.notes);
                        if (item.wfState >= 0)
                            cmd.Parameters.AddWithValue("@WF_StatusID", item.wfState);
                        cmd.Parameters.AddWithValue("@ShowRes", 1);

                        using (SqlDataReader dr = cmd.ExecuteReader()) {
                            if (dr.Read()) {
                                if (item.id == 0)
                                    item.id = dr.GetInt32(0);
                            }
                        } // datareader
                    } // sqlCommand
                } // sqlConn

           return item;
        } // Save

        /// <summary>
        /// Deletes Issue by internal ID.
        /// </summary>
        /// <returns></returns>
        protected bool Del(int id) {
            if (id < 1)
                throw new Exception("ID is required for delete operation.");

            bool res = false;

            // Assignment_Del @ID int, @ShowRes smallint = 1
                using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand("Assignment_Del", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@ShowRes", 1);

                        using (SqlDataReader dr = cmd.ExecuteReader()) {
                            if (dr != null && dr.HasRows && dr.Read()) {
                                res = true;
                            }
                        } // datareader
                    } // sqlCommand
                } // sqlConn

            return res;
        } // Del


        /// <summary>
        /// Gets AssignmentResources.
        /// </summary>
        /// <returns></returns>
        protected List<AssignmentActor> Resources_Actors_Get(int assignmentID) {
            if (assignmentID < 1)
                throw new ArgumentException("AssignmentID is required.");

            // Assignment_Resource_GetList @AssignmentID int
            // Returns:
            // AssignmentID, UserId, UserName, UserRoleID, UserRoleName
            List<AssignmentActor> res = new List<AssignmentActor>();

                using (SqlConnection conn = _connFactory.GetConnection()) {
                    Resources_Actors_Get(conn, assignmentID);
                } 
            

            return res;
        } // Resources_Actors_Get

        private List<AssignmentActor> Resources_Actors_Get(SqlConnection conn, int assignmentID)
        {
            List<AssignmentActor> res = new List<AssignmentActor>();
            using (SqlCommand cmd = new SqlCommand("Assignment_Resource_GetList", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                        while (dr.Read())
                        {
                            AssignmentActor item = new AssignmentActor();
                            // 0 - AssignmentID
                            item.id = dr.IsDBNull(1) ? 0 : dr.GetInt32(1);
                            item.name = dr.IsDBNull(2) ? "" : dr.GetString(2);
                            item.role = dr.IsDBNull(3) ? ActorRole.unknown : (ActorRole)dr.GetInt32(2);
                            res.Add(item);
                        }
                } 
            }

            return res;
        }



        /// <summary>
        /// Saves Assignment Resources.
        /// </summary>
        /// <returns></returns>
        protected AssignmentActor Resources_Actors_Save(int assignmentID, AssignmentActor item) {
            if (assignmentID < 1)
                throw new ArgumentException("AssignmentID is required.");
            if (item == null || item.id < 1)
                throw new ArgumentException("AssignmentResource with a proper UserId is required.");


                // Assignment_Resource_AddUpdate @AssignmentID int, @UserId int, 
                // @UserRoleID int = NULL, @UserRoleName varchar(30) = NULL, @Deadline date,
                // @ShowRes smallint = 1
                //
                using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand("Assignment_Resource_AddUpdate", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);
                        cmd.Parameters.AddWithValue("@UserId", item.id);
                        cmd.Parameters.AddWithValue("@UserRoleID", item.role);
                        cmd.Parameters.AddWithValue("@ShowRes", 1);


                        cmd.ExecuteScalar();
                    } // sqlCommand
                } // sqlConn

            return item;
        } // Resources_Actors_Save

        /// <summary>
        /// Saves Assignment Resources.
        /// </summary>
        /// <returns></returns>
        protected bool Resources_Actors_Save(int assignmentID, IEnumerable<AssignmentActor> items) {
            if (assignmentID < 1)
                throw new ArgumentException("AssignmentID is required.");
            if (items == null)
                throw new ArgumentException("AssignmentResources list is required.");
            bool res = true;

            // Assignment_Resource_AddUpdateTVP	@AssignmentID int, @tvpResourceList as dbo.AssignmentResource readonly,
                // @ShowRes smallint = 1
                //
                using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand("Assignment_Resource_AddUpdateTVP", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;

                        DataTable dt = new DataTable("tvpResourceList");
                        dt.Columns.Add("UserId", typeof(System.Int32));
                        dt.Columns.Add("UserRoleID", typeof(System.Int32));
                        dt.Columns.Add("UserRoleName", typeof(System.String));

                        foreach (AssignmentActor item in items) {
                            DataRow dr = dt.NewRow();
                            dr["UserId"] = item.id;
                            dr["UserRoleID"] = item.role;
                            //if (!string.IsNullOrWhiteSpace(item.UserRoleName)) dr["UserRoleName"] = item.UserRoleName;
                            dt.Rows.Add(dr);
                        }

                        cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);
                        SqlParameter par = cmd.Parameters.AddWithValue("@tvpResourceList", dt);
                        par.SqlDbType = SqlDbType.Structured;
                        cmd.Parameters.AddWithValue("@ShowRes", 1);


                        cmd.ExecuteScalar();
                    } // sqlCommand
                } // sqlConn

            return res;
        } // Resources_Actors_Save

        /// <summary>
        /// Deletes Actor from the Assignment by internal ID.
        /// </summary>
        /// <returns></returns>
        protected bool Resources_Actors_Del(int assignmentID, AssignmentActor item) {
            if (assignmentID < 1)
                throw new ArgumentException("AssignmentID is required.");
            if (item == null || item.id < 1)
                throw new ArgumentException("AssignmentResource with a proper UserId is required.");

            bool res = true;

            // Assignment_Resource_Del @AssignmentID int, @UserId int, @ShowRes smallint = 1
                using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand("Assignment_Resource_Del", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);
                        cmd.Parameters.AddWithValue("@UserId", item.id);
                        cmd.Parameters.AddWithValue("@ShowRes", 1);


                        cmd.ExecuteScalar();
                    } // sqlCommand
                } // sqlConn

            return res;
        } // Resources_Actors_Del

        /// <summary>
        /// Gets AssignmentResources.
        /// </summary>
        /// <returns></returns>
        protected List<AssignmentDeadline> Resources_Deadlines_Get(int assignmentID) {
            if (assignmentID < 1)
                throw new ArgumentException("AssignmentID is required.");

            // Assignment_ResourceD_GetList @AssignmentID int
            // Returns:
            // AssignmentID, TypeID, Deadline
            List<AssignmentDeadline> res = new List<AssignmentDeadline>();

            using (SqlConnection conn = _connFactory.GetConnection()) {

                    return Resources_Deadlines_Get(conn, assignmentID);

                } // sqlConn


        } // Resources_Deadlines_Get


        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="assignmentID"></param>
        /// <returns></returns>
        private List<AssignmentDeadline> Resources_Deadlines_Get(SqlConnection conn, int assignmentID)
        {
            List<AssignmentDeadline> res = new List<AssignmentDeadline>();

            using (SqlCommand cmd = new SqlCommand("Assignment_ResourceD_GetList", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        AssignmentDeadline item = new AssignmentDeadline();
                        // 0 - AssignmentID
                        item.id = dr.GetInt32(1);
                        item.deadline = dr.GetDateTime(2);
                        res.Add(item);
                    }
                } // datareader
            } // sqlCommand

            return res;

        }



        /// <summary>
        /// Saves Assignment Resources.
        /// </summary>
        /// <returns></returns>
        protected AssignmentDeadline Resources_Deadlines_Save(int assignmentID, AssignmentDeadline item) {
            if (assignmentID < 1)
                throw new ArgumentException("AssignmentID is required.");
            if (item == null || (int)item.typeid < 1)
                throw new ArgumentException("AssignmentResource with a proper deadline type is required.");


            // Assignment_ResourceD_AddUpdate @AssignmentID int,
                // @TypeID int, @Deadline date,
                // @ShowRes smallint = 1
                //
                using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand("Assignment_ResourceD_AddUpdate", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);
                        cmd.Parameters.AddWithValue("@TypeID", item.typeid);
                        cmd.Parameters.AddWithValue("@Deadline", item.deadline);
                        cmd.Parameters.AddWithValue("@ShowRes", 1);


                        cmd.ExecuteScalar();
                    } // sqlCommand
                } // sqlConn

            return item;
        } // Resources_Deadlines_Save

        /// <summary>
        /// Saves Assignment Resources.
        /// </summary>
        /// <returns></returns>
        protected bool Resources_Deadlines_Save(int assignmentID, IEnumerable<AssignmentDeadline> items) {
            if (assignmentID < 1)
                throw new ArgumentException("AssignmentID is required.");
            if (items == null)
                throw new ArgumentException("AssignmentResources list is required.");
            bool res = true;

            // Assignment_ResourceD_AddUpdateTVP @AssignmentID int, @tvpResourceList as dbo.AssignmentResourceD readonly,
                // @ShowRes smallint = 1
                //
                using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand("Assignment_ResourceD_AddUpdateTVP", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;

                        DataTable dt = new DataTable("tvpResourceList");
                        dt.Columns.Add("TypeID", typeof(System.Int32));
                        dt.Columns.Add("Deadline", typeof(System.DateTime));

                        foreach (AssignmentDeadline item in items) {
                            if (item.deadline.Ticks == 0)
                                continue;
                            DataRow dr = dt.NewRow();
                            dr["TypeID"] = item.id;
                            dr["Deadline"] = item.deadline;
                            dt.Rows.Add(dr);
                        }

                        cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);
                        SqlParameter par = cmd.Parameters.AddWithValue("@tvpResourceList", dt);
                        par.SqlDbType = SqlDbType.Structured;
                        cmd.Parameters.AddWithValue("@ShowRes", 1);


                        cmd.ExecuteScalar();
                    } // sqlCommand
                } // sqlConn

            return res;
        } // Resources_Deadlines_Save

        /// <summary>
        /// Deletes Deadline from the Assignment by internal ID.
        /// </summary>
        /// <returns></returns>
        protected bool Resources_Deadlines_Del(int assignmentID, AssignmentDeadline item) {
            if (assignmentID < 1)
                throw new ArgumentException("AssignmentID is required.");
            if (item == null || item.id < 1)
                throw new ArgumentException("AssignmentResource with a proper deadline type is required.");

            bool res = true;

            // Assignment_ResourceD_Del @AssignmentID int, @TypeID int, @ShowRes smallint = 1
                using (SqlConnection conn = _connFactory.GetConnection()) {
                    using (SqlCommand cmd = new SqlCommand("Assignment_ResourceD_Del", conn)) {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);
                        cmd.Parameters.AddWithValue("@TypeID", item.id);
                        cmd.Parameters.AddWithValue("@ShowRes", 1);

                        cmd.ExecuteScalar();
                    } // sqlCommand
                } // sqlConn

            return res;
        } // Resources_Deadlines_Del



        public void AssignmenetDeleteAllResources(int assignmentID)
        {
            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {

                    cmd.CommandText = @"
                       
                     delete Assignment_Resource where AssignmentID = @AssignmentID;

                     delete Assignment_ResourceD where AssignmentID = @AssignmentID;

                    ";

                    cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);
                    cmd.ExecuteNonQuery();
                } // sqlCommand
            } // sqlConn
        }


        public IEnumerable<AssignmentActor> AssignmentResources_GetActors(int assignmentID) {
            return Resources_Actors_Get(assignmentID);
        }

        public AssignmentActor AssignmentResources_AddActor(int assignmentID, AssignmentActor actor)
        {
            return Resources_Actors_Save(assignmentID, actor);
        }

        public IEnumerable<AssignmentActor> AssignmentResources_AddActorBulk(int assignmentID, IEnumerable<AssignmentActor> actors) {
            if (Resources_Actors_Save(assignmentID, actors))
                return actors;
            else
                return null;
        }

        public AssignmentActor AssignmentResources_DelActor(int assignmentID, AssignmentActor actor)
        {
            Resources_Actors_Del(assignmentID, actor);
            return actor;
        }

        public IEnumerable<AssignmentDeadline> AssignmentResources_GetDeadlines(int assignmentID) {
            return Resources_Deadlines_Get(assignmentID);
        }

        public AssignmentDeadline AssignmentResources_AddDeadline(int assignmentID, AssignmentDeadline deadline)
        {
            return Resources_Deadlines_Save(assignmentID, deadline);
        }

        public IEnumerable<AssignmentDeadline> AssignmentResources_AddDeadlineBulk(int assignmentID, IEnumerable<AssignmentDeadline> deadlines) {
            if (Resources_Deadlines_Save(assignmentID, deadlines))
                return deadlines;
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentID"></param>
        /// <param name="deadline"></param>
        /// <returns></returns>
        public AssignmentDeadline AssignmentResources_DelDeadline(int assignmentID, AssignmentDeadline deadline)
        {
            Resources_Deadlines_Del(assignmentID, deadline);
            return deadline;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public IEnumerable<AssignmentItem> SearchByIssue(int issueId) {
            AssignmentItem e = new AssignmentItem();
            return LoadByIssueId (issueId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<AssignmentItem> SearchByIssueByUser(int issueId, int userId)
        {
            AssignmentItem e = new AssignmentItem();
            e.issueId = issueId;
            return Load(null, e, userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<AssignmentItem> SearchByUser( int id)
        {
            return LoadByUserId(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentId"></param>
        public void SetReady(int assignmentId)
        {

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {

                    cmd.CommandText = @"
                       
update tn set tn.WF_StatusID = 10   from TasteNote tn
join TastingEvent_TasteNote  te  on te.TasteNoteID = tn.ID
join Assignment_TastingEvent ate on ate.TastingEventID = te.TastingEventID
where ate.AssignmentID = @AssignmentID 
and tn.WF_StatusID = 0

                    ";

                    cmd.Parameters.AddWithValue("@AssignmentID", assignmentId);
                    cmd.ExecuteNonQuery();
                } // sqlCommand
            } // sqlConn
        
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentId"></param>
        public void SetApproved(int assignmentId)
        {
            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {

                    cmd.CommandText = @"
                       
update tn set tn.WF_StatusID = 60   from TasteNote tn
join TastingEvent_TasteNote  te  on te.TasteNoteID = tn.ID
join Assignment_TastingEvent ate on ate.TastingEventID = te.TastingEventID
where ate.AssignmentID = @AssignmentID 
and tn.WF_StatusID = 10

                    ";

                    cmd.Parameters.AddWithValue("@AssignmentID", assignmentId);
                    cmd.ExecuteNonQuery();
                } // sqlCommand
            } // sqlConn
        }
    }
}

