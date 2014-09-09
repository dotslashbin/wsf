using System;
using System.Collections.Generic;



namespace EditorsCommon
{

    public class WineN
    {
        public Int32 id;
        public VinN vinN;
        public string vintage;
        public int workflow;
    }

    public class VinN
    {
        public Int32 id;
        public Int32 count;
        public String producer;
        public String label;

        public String colorClass;
        public String variety;
        public String dryness;
        public String wineType;


        public String country;
        public String region;
        public String location;
        public String locale;
        public String site;

        public int workflow;

        protected DateTime created;
        protected DateTime updated;

        public List<WineN> wines;

        public String appellation
        {
            get
            {
                if (!String.IsNullOrEmpty(site))
                    return site;

                if (!String.IsNullOrEmpty(locale))
                    return locale;
                if (!String.IsNullOrEmpty(location))
                    return location;
                if (!String.IsNullOrEmpty(site))
                    return region;
                if (!String.IsNullOrEmpty(site))
                    return country;

                return "";
            }
        }

        public bool HasState(int state)
        {

            if (state == EditorsCommon.WorkFlowState.STATE_GROUP_IN_PROCESS)
            {
                if (workflow < EditorsCommon.WorkFlowState.PUBLISHED)
                    return true;
            }

            if (state == EditorsCommon.WorkFlowState.STATE_GROUP_PUBLISHED)
            {
                if (workflow == EditorsCommon.WorkFlowState.PUBLISHED)
                    return true;
            }

            if (state == EditorsCommon.WorkFlowState.STATE_GROUP_ARCHIVED)
            {
                if (workflow > EditorsCommon.WorkFlowState.PUBLISHED)
                    return true;
            }



            foreach (var v in wines)
            {
                if (state == EditorsCommon.WorkFlowState.STATE_GROUP_IN_PROCESS)
                {
                    if (v.workflow < EditorsCommon.WorkFlowState.PUBLISHED)
                        return true;
                }

                if (state == EditorsCommon.WorkFlowState.STATE_GROUP_PUBLISHED)
                {
                    if (v.workflow == EditorsCommon.WorkFlowState.PUBLISHED)
                        return true;
                }

                if (state == EditorsCommon.WorkFlowState.STATE_GROUP_ARCHIVED)
                {
                    if (v.workflow > EditorsCommon.WorkFlowState.PUBLISHED)
                        return true;
                }
            }

            return false;
        }



             //if (state == EditorsCommon.WorkFlowState.STATE_GROUP_IN_PROCESS)
             //{
             //    result = from r in result where r.workflow < EditorsCommon.WorkFlowState.PUBLISHED select r;
             //}

             /////
             //if (state == EditorsCommon.WorkFlowState.STATE_GROUP_PUBLISHED)


    }


    public interface IVinStorage : IStorage<VinN>
    {


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="vinn"></param>
        ///// <returns></returns>
        //VinN Delete(VinN vinn);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Delete(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<VinN> Search(String filter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<VinN> SearchAppelation(String filter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<VinN> SearchLabel(String filter);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<VinN> SearchLabelProducer(string producer, string filter);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IEnumerable<VinN> SearchProducer(string filter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        IEnumerable<VinN> SearchProducerExtended(string filter);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        IEnumerable<VinN> SearchWineN(string filter);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<VinN> SearchNewWineN();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="country"></param>
        /// <param name="region"></param>
        /// <param name="location"></param>
        /// <param name="locale"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        IEnumerable<VinN> SearchLocation(string country, string region, string location, string locale, string site);



        int ApproveVin(int id);
    }
}
