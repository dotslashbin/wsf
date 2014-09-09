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
    public class WineProducerItem
    {
        public int ID;
        public string Name;
        public string NameToShow;
        public string WebSiteURL;

        public string Profile;
        public string ContactInfo;

    }

    /// <summary>
    /// 
    /// </summary>
    public interface IProducerStorage : IStorage<WineProducerItem>
    {
    }
}
