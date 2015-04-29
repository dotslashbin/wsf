using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsCommon.Publication {

    /// <summary>
    /// 
    /// </summary>
    public enum ActorRole
    {
        unknown = 0,
        admin = 1,
        reviewer = 2,
        editor = 3,
        proofread = 4,
        editor_and_chief = 5
    }

    /// <summary>
    /// 
    /// </summary>
    public class AssignmentActor {
        public int id;
        public String name;
        public ActorRole role;
    }

    /// <summary>
    /// 
    /// </summary>
    public enum DeadlineType
    {
        publish = 0,
        submission = 1,
        proofread = 2,
        approve = 3
    };

    /// <summary>
    /// 
    /// </summary>
    public class AssignmentDeadline
    {
        public int id;
        public String name;
        public DeadlineType typeid;
        public DateTime deadline;
    }

    /// <summary>
    /// 
    /// </summary>
    public class AssignmentItem {
        public int id;
        public string title;
        public string notes;


        public int    issueId;
        public string issue;
        public int    publicationId;
        public string publication;

        public AssignmentActor author = new AssignmentActor();
        public AssignmentActor editor = new AssignmentActor();
        public AssignmentActor proofread = new AssignmentActor();


        public DateTime submitDate;
        public DateTime proofreadDate;
        public DateTime approveDate;



        public DateTime CreatedDate;
        public DateTime UpdatedDate;

        public short wfState = short.MinValue;


        public void ProcessDeadline(DeadlineType deadlineType, DateTime dt ){
            switch (deadlineType)
            {
                case DeadlineType.approve :
                    approveDate = dt;
                    break;
                case DeadlineType.proofread:
                    proofreadDate =  dt;
                    break;
                case DeadlineType.submission:
                    submitDate =  dt;
                    break;
            }
        }

        public void ProcessActor(int userId, ActorRole actorRole, String name)
        {
            switch (actorRole)
            {
                case ActorRole.editor:
                    editor = new AssignmentActor() { id = userId, name = name, role = actorRole };
                    break;
                case ActorRole.reviewer:
                    author =  new AssignmentActor() { id = userId, name = name, role = actorRole };
                    break;
                case ActorRole.proofread:
                    proofread =  new AssignmentActor() { id = userId, name = name, role = actorRole };
                    break;
            }
        }


        public int notesCount;
        public int notesCountWaiting;
        public int notesCountApproved;

    }

    /// <summary>
    /// 
    /// </summary>
    public class AssignmentItemComplete : AssignmentItem
    {

        public IEnumerable<TastingEventComplete> tastingEvents;


    }



    public interface IAssignmentStorage : IStorage<AssignmentItem> {
        /// <summary>
        /// Gets AssignmentItem by internal ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AssignmentItem GetByID(int id);

        IEnumerable<AssignmentActor> AssignmentResources_GetActors(int assignmentID);

        AssignmentActor AssignmentResources_AddActor(int assignmentID, AssignmentActor actor);

        AssignmentActor AssignmentResources_DelActor(int assignmentID, AssignmentActor actor);

        IEnumerable<AssignmentActor> AssignmentResources_AddActorBulk(int assignmentID, IEnumerable<AssignmentActor> actors);

        IEnumerable<AssignmentDeadline> AssignmentResources_GetDeadlines(int assignmentID);

        AssignmentDeadline AssignmentResources_AddDeadline(int assignmentID, AssignmentDeadline deadline);

        AssignmentDeadline AssignmentResources_DelDeadline(int assignmentID, AssignmentDeadline deadline);

        IEnumerable<AssignmentDeadline> AssignmentResources_AddDeadlineBulk(int assignmentID, IEnumerable<AssignmentDeadline> deadlines);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        IEnumerable<AssignmentItem> SearchByIssue(int issueId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<AssignmentItem> SearchByIssueByUser(int issueId, int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       IEnumerable<AssignmentItem> SearchByUser(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentID"></param>
       void AssignmenetDeleteAllResources(int assignmentID);

       bool DeleteAssignment(int assignmentID); 


       // /// <summary>
       // /// Set 'Ready' status for all notes withing the assignment.
       // /// </summary>
       // /// <param name="assignmentId"></param>
       //void SetReady(int assignmentId);


       // /// <summary>
       // /// 
       // /// </summary>
       // /// <param name="assignmentId"></param>
       //void SetApproved(int assignmentId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <param name="stateId"></param>
        /// <returns></returns>
       bool SetAssignmentState(int assignmentId, int stateId);
    }

}
