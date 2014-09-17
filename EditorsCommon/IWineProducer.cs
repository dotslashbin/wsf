using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsCommon
{

    public class WineProducer {
        public WineProducer() {
            id = -1;
        }

        public int id;
        public string name;
        public string nameToShow;
        public string webSiteURL;

        public string country;
        public string region;
        public string location;
        public string locale;
        public string site;

        public string profile;
        public string contactInfo;

        public DateTime dateCreated;
        public DateTime dateUpdated;


        // WF
        public short workflow;
    }

    public interface IWineProducerStorage : IStorage<WineProducer>
    {
        /// <summary>
        /// Gets Wine Producer by internal ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        WineProducer GetByID(int id);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        IEnumerable<WineProducer> SearchByName(string searchString);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        IEnumerable<WineProducer> SearchByWorkflowStatus(int status);


    }
}
