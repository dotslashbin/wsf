using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsCommon.Publication {


    public class PublicationItem
    {
        public int id;
        public int publisherId;
        public string name;
    }


    /// <summary>
    /// 
    /// </summary>
    public class Issue {
        public int wfState;

        public int id;
        public int publicationID;
        public string publicationName;

        public int chiefEditorId;
        public string chiefEditorName;

        public string title;

        public DateTime createdDate;
        public DateTime publicationDate;
        //public DateTime proofreadDate;

        public string Notes;

        internal DateTime dateCreated;
        internal DateTime dateUpdated;

        public int articlesCnt;
        public int articlesPublishedCnt;

        public int tasteNotesCnt;
        public int tasteNotesPublishedCnt;

        public int tastingEventsCnt;
        public int tastingEventsPublishedCnt;
    }

    /// <summary>
    /// 
    /// </summary>
    public class IssueComplete : Issue
    {
        public IEnumerable<AssignmentItemComplete> assignments; 
    }


    public interface IIssueStorage : IStorage<Issue> {
        /// <summary>
        /// Gets Issue by internal ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Issue GetByID(int id);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueID"></param>
        /// <returns></returns>
        IEnumerable<TastingEvent> GetAssignments(int issueID);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<PublicationItem> GetPublications();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <param name="fromStatus"></param>
        /// <param name="toStatus"></param>
        void UpdateStatus(int issueId, int fromStatus, int toStatus);
  

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        IssueComplete LoadIssueComplete(int issueId, int status);
 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        IssueComplete AddIssueComplete(IssueComplete issueId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        void Publish(int issueId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issueId"></param>
        void RollbackPublish(int issueId);

    
    }
}
