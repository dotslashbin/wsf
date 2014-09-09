using System;
using System.Collections.Generic;

namespace EditorsCommon
{

    /// <summary>
    /// 
    /// </summary>
    public class TastingEvent
    {
        public int id;

        public int assignmentId;

        public String title;

        public String location;

        public String comments;

        public DateTime created;

        public int notesCount;

    }

    /// <summary>
    /// 
    /// </summary>
    public class TastingEventComplete : TastingEvent
    {
        public IEnumerable<TastingNote> tastingNotes;



        public String NoteAppellation(TastingNote n)
        {
            if( !String.IsNullOrEmpty(n.site))
                  return n.site;

            if (!String.IsNullOrEmpty(n.locale))
                return n.locale;

            if (!String.IsNullOrEmpty(n.location))
                return n.location;

            if (!String.IsNullOrEmpty(n.region))
                return n.region;

            if (!String.IsNullOrEmpty(n.country))
                return n.country;


            return "Unknown";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public String NotePrice(TastingNote n)
        {
            if (!String.IsNullOrEmpty(n.estimatedCost))
            {
                if (!String.IsNullOrEmpty(n.estimatedCostHi))
                {
                    if (n.estimatedCost.CompareTo(n.estimatedCostHi) != 0)
                    {
                        return "$" + n.estimatedCost + "-" + n.estimatedCostHi;
                    }
                }
                return "$" + n.estimatedCost;
            }

            return "Unknown";
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public interface ITastingEventStorage : IStorage<TastingEvent>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <returns></returns>
        IEnumerable<TastingEvent> SearchTastingEventByAssignment(int assignmentId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tastingEventId"></param>
        /// <param name="assignmentId"></param>
        /// <returns></returns>
        bool MoveToAssingment(int tastingEventId, int assignmentId);



    }
}
