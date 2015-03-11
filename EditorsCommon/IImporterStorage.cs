using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsCommon
{
    /// <summary>
    /// 
    /// </summary>
    public class WineImporterItem
    {
        public int id;
        public string name;
        public string address;
        public string phone1;
        public string phone2;
        public string fax;
        public string email;
        public string url;
        public string notes;

        public int linkImportersCount;


    }

    /// <summary>
    /// 
    /// </summary>
    public interface IImporterStorage : IStorage<WineImporterItem>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="producerId"></param>
        //void AddLinkToProducer(int producerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="producerId"></param>
        //void RemoveLinkToProducer(int producerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="producerId"></param>
        /// <returns></returns>
        IEnumerable<WineImporterItem> GetLinksToProducer(int producerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<WineImporterItem> SearchByProducerId(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importerId"></param>
        /// <param name="producerId"></param>
        /// <returns></returns>
        WineImporterItem AddToProducer(int importerId, int producerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importerId"></param>
        /// <param name="producerId"></param>
        /// <returns></returns>
        WineImporterItem RemoveFromProducer(int importerId, int producerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importerId"></param>
        /// <returns></returns>
        IEnumerable<WineProducer> GetLinksToImporter(int importerId);
    
    
    
    }
}
