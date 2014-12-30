using EditorsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsDbLayer
{
    /// <summary>
    /// 
    /// </summary>
    public class TastingEventStorage : ITastingEventStorage
    {
        private ISqlConnectionFactory _connFactory;



        public TastingEventStorage(ISqlConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }


        public IEnumerable<TastingEvent> SearchTastingEventByAssignment(int assignmentId)
        {
            List<TastingEvent> result = new List<TastingEvent>();

            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                using (var cmd = new SqlCommand("", con))
                {

//                    cmd.CommandText = @"
//                        select ID, Title, Location,COALESCE(Notes, '') as notes,  t.created as created, COUNT( tn.TasteNoteID) 
//                        from TastingEvent as t 
//                        left join Assignment_TastingEvent as a  on a.TastingEventID = t.ID
//                        left join TastingEvent_TasteNote  as tn on tn.TastingEventID = t.ID
//                        where a.AssignmentID = @assignmentId
//                        group by ID, Title, Location,notes, t.created";



                    cmd.CommandText = @"select t.ID, t.Title, t.Location,COALESCE(t.Notes, '') as notes,  t.created as created
 ,COUNT( tn.TasteNoteID) as totalCount 
 ,COUNT( tnn0.ID)   as draftCount 
 ,COUNT( tnn10.ID)  as proofreadCount 
 ,COUNT( tnn50.ID)  as editorCount
 ,COUNT( tnn100.ID) as publishedCount
from TastingEvent as t 
left join Assignment_TastingEvent as a  on a.TastingEventID = t.ID
left join TastingEvent_TasteNote  as tn  on tn.TastingEventID = t.ID
left outer join (select ID, WF_StatusID from  TasteNote where WF_StatusID = 0) as tnn0 on tnn0.ID =  tn.TasteNoteID
left outer join (select ID, WF_StatusID from  TasteNote where WF_StatusID = 10) as tnn10 on tnn10.ID =  tn.TasteNoteID
left outer join (select ID, WF_StatusID from  TasteNote where WF_StatusID = 50) as tnn50 on tnn50.ID =  tn.TasteNoteID
left outer join (select ID, WF_StatusID from  TasteNote where WF_StatusID = 100) as tnn100 on tnn100.ID =  tn.TasteNoteID
where a.AssignmentID = @assignmentId

group by t.ID, t.Title, t.Location,t.notes, t.created";


                    cmd.Parameters.AddWithValue("@assignmentId", assignmentId);

                    using (var rdr = cmd.ExecuteReader())
                    {

                        while (rdr.Read())
                        {
                            TastingEvent evt = new TastingEvent();

                            evt.id = rdr.GetInt32(0);
                            evt.assignmentId = assignmentId;
                            evt.title = rdr.IsDBNull(1) ? "" : rdr.GetString(1);
                            evt.location = rdr.IsDBNull(2) ? "" : rdr.GetString(2);
                            evt.comments = rdr.IsDBNull(3) ? "" : rdr.GetString(3);
                            evt.created = rdr.GetDateTime(4);
                            evt.notesCount = rdr.GetInt32(5);
                            evt.draftCount = rdr.GetInt32(6);
                            evt.proofreadCount = rdr.GetInt32(7);
                            evt.editorCount = rdr.GetInt32(8);
                            evt.publishedCount = rdr.GetInt32(9);

                            result.Add(evt);
                        }
                    }

                    return result;
                }
            }
        }

        public TastingEvent Create(TastingEvent e)
        {

            using (var con = _connFactory.GetConnection())
            {
                using (var transation = con.BeginTransaction())
                {
                    var query = new StringBuilder();

                    using (var cmd = new SqlCommand("", con))
                    {
                        cmd.Transaction = transation;
                        cmd.CommandText =

                        @" 
                        
                            declare @Result int;
       
                            set nocount on;
                 
                            exec @Result = TastingEvent_Add @Title = @Title ,@Location = @Location, @Notes = @Notes;

                            exec Assignment_TastingEvent_Add @AssignmentID=@AssignmentID, @TastingEventID = @Result;

                            select @Result;
                            ";


                        cmd.Parameters.AddWithValue("@Title", String.IsNullOrEmpty(e.title) ? "" : e.title);
                        cmd.Parameters.AddWithValue("@Location", String.IsNullOrEmpty(e.location) ? "" : e.location);
                        cmd.Parameters.AddWithValue("@Notes", String.IsNullOrEmpty(e.comments) ? "" : e.comments);
                        cmd.Parameters.AddWithValue("@AssignmentID", e.assignmentId);


                        try
                        {
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

                                    e.id = rdr.GetInt32(0);
                                }
                            }

                            transation.Commit();
                            return e;
                        }
                        catch (Exception ex)
                        {
                            transation.Rollback();
                            throw new Exception("exception while create TastingEvent transaction", ex);
                        }

                    }
                }
            }
        }

        public IEnumerable<TastingEvent> Search(TastingEvent e)
        {
            throw new NotImplementedException();
        }

        public TastingEvent Update(TastingEvent e)
        {

            /*
            CREATE PROCEDURE [dbo].[TastingEvent_Update]
                @ID int, 
                @Title nvarchar(255) = NULL, 
                @Location nvarchar(100) = NULL,
                @Notes nvarchar(max) = NULL
            */

            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandText =

                    @" 
                            declare @Result int;
       
                            set nocount on;
                 
                            exec @Result = TastingEvent_Update @ID = @ID, @Title = @Title ,@Location = @Location, @Notes = @Notes

                            select @Result;

                            ";


                    cmd.Parameters.AddWithValue("@ID", e.id);
                    cmd.Parameters.AddWithValue("@Title", String.IsNullOrEmpty(e.title) ? "" : e.title);
                    cmd.Parameters.AddWithValue("@Location", String.IsNullOrEmpty(e.location) ? "" : e.location);
                    cmd.Parameters.AddWithValue("@Notes", String.IsNullOrEmpty(e.comments) ? "" : e.comments);

                    using (var rdr = cmd.ExecuteReader())
                    {

                        if (rdr.Read())
                        {
                            if (rdr.GetInt32(0) > 0)
                                return e;
                        }
                    }

                    return null;

                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public TastingEvent Delete(TastingEvent e)
        {
            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandText =

                    @" 
                            declare @Result int;
       
                            set nocount on;
                 
                            exec @Result = TastingEvent_Del @ID = @ID

                            select @Result;

                            ";


                    cmd.Parameters.AddWithValue("@ID", e.id);

                    using (var rdr = cmd.ExecuteReader())
                    {

                        if (rdr.Read())
                        {
                            if (rdr.GetInt32(0) > 0)
                                return e;
                        }
                    }

                    return null;

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tastingEventId"></param>
        /// <param name="assignmentId"></param>
        /// <returns></returns>
        public bool MoveToAssingment(int tastingEventId, int assignmentId)
        {
            using (var con = _connFactory.GetConnection())
            {
                var query = new StringBuilder();

                using (var cmd = new SqlCommand("", con))
                {
                    cmd.CommandText =
                    @" 
        
                         delete Assignment_TastingEvent where TastingEventID = @tastingEventId;

                         exec Assignment_TastingEvent_Add @AssignmentId=@assignmentID, @TastingEventId = @tastingEventId;

                     ";


                    cmd.Parameters.AddWithValue("@assignmentId", assignmentId);
                    cmd.Parameters.AddWithValue("@tastingEventId", tastingEventId);

                    cmd.ExecuteNonQuery();

                    return true;

                }
            }
        }


    }
}
