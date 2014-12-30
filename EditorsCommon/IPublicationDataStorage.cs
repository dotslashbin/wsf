using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsCommon
{

    #region -- Members --
    public class PublicationData
    {
        public int? ID { get; set; }
        public int? PublisherId { get; set; }
        public string Name { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public Boolean Errorexist { get; set; }
        public string ErrorMessage { get; set; }
    }
    #endregion


    #region --interface--
    public interface IPublicationDataStorage : IStorage<PublicationData>
    {

        IEnumerable<PublicationData> GetPublications();

    }
    #endregion
}
