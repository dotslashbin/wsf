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

        public DateTime dateCreated;
        public DateTime dateUpdated;


        // WF
        public short wfState;
    }

    public class WineProducerExt : WineProducer
    {
        public int linkImportersCount;
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool SetProducerStatus(int id, int status);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        IEnumerable<WineProducerExt> SearchByWorkflowStatusExt(int p);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        IEnumerable<WineProducerExt> SearchByNameExt(string term);
    }
}
