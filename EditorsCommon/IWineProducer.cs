using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsCommon.Wine {

    public class WineProducer {
        public WineProducer() {
            ID = -1;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string NameToShow { get; set; }
        public string WebSiteURL { get; set; }

        public string Country { get; set; }
        public string Region { get; set; }
        public string Location { get; set; }
        public string Locale { get; set; }
        public string Site { get; set; }

        public string Profile { get; set; }
        public string ContactInfo { get; set; }

        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public string CreatorName { get; set; }
        public string EditorName { get; set; }

        // WF
        public short WF_StatusID { get; set; }
        public string WF_StatusName { get; set; }
        public int WF_AssignedByID { get; set; }
		public string WF_AssignedByLogin { get; set; }
		public string WF_AssignedByName { get; set; }
        public int WF_AssignedToID { get; set; }
		public string WF_AssignedToLogin { get; set; }
		public string WF_AssignedToName { get; set; }
		public DateTime? WF_AssignedDate { get; set; }
        public string WF_Note { get; set; }
    }

    public interface IWineProducer : IStorage<WineProducer>
    {
        /// <summary>
        /// Gets Wine Producer by internal ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        WineProducer GetByID(int id);
    }
}
