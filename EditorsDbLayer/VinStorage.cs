using EditorsCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsDbLayer
{


    /// <summary>
    /// 
    /// </summary>
    public class VinStorage : IVinStorage
    {

        ISqlConnectionFactory _connFactory;

        public VinStorage(ISqlConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public VinN Create(VinN e)
        {
            throw new NotImplementedException();
        }



        //            return l;
        //        }

        //    }
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        //public IEnumerable<VinN> Search(ISearchContext context)
        //{
        //}

   


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vinn"></param>
        /// <returns></returns>
        public VinN Delete(VinN vinn)
        {
            var result = Delete(vinn.id);
            if (result == Delete(vinn.id))
                return vinn;

            return null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Int32 Delete(Int32 id)
        {
            using (SqlConnection conn = _connFactory.GetConnection())
            {
                return id;
            }

        }



        public IEnumerable<VinN> Search(string filter)
        {
            throw new NotImplementedException();
        }



        public IEnumerable<VinN> Search(VinN filter)
        {
            throw new NotImplementedException();
        }




        public IEnumerable<VinN> SearchAppelation(string filter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VinN> SearchLabelProducer(string producer, string filter)
        {
            //ISearchContext context = new SearchContext();
            //context[VinStorageContextParams.ContextParamSearchString] = filter;
            //context[VinStorageContextParams.ContextParamField] = VinStorageContextParams.ContextFieldExtendedLabel;
            //return Search(context);

            var l = new List<VinN>();


            if (string.IsNullOrEmpty(filter))
            {
                return l;
            }

            var sb = new StringBuilder();

            sb.Append(" select top 100 label, producerToShow ");
            sb.Append(", COALESCE(country, ''), COALESCE(region, ''),  COALESCE(Location, ''),  COALESCE(locale, ''),  COALESCE(site, '')  ");
            sb.Append(", COALESCE(color, ''), COALESCE(variety, ''),  COALESCE(dryness, '') ");

            sb.Append("  FROM vWineVinNDetails as v  WITH (NOEXPAND) ");
            sb.Append("  INNER JOIN CONTAINSTABLE(vWineVinNDetails,label,@filterLabel) AS KEY_TBL1  ON v.Wine_VinN_ID =  KEY_TBL1.[KEY]");
            if( !String.IsNullOrEmpty(producer)){
                //sb.Append("  INNER JOIN CONTAINSTABLE(vWineVinNDetails,producerToShow,@filterProducer) AS KEY_TBL2  ON v.Wine_VinN_ID =  KEY_TBL2.[KEY]");
                sb.Append("  where producerToShow = @filterProducer");
            }



            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.AddWithValue("@filterLabel", Utils.BuildSearchString(filter));
                    if( !String.IsNullOrEmpty(producer)){
                        //cmd.Parameters.AddWithValue("@filterProducer", Utils.BuildSearchString(producer));
                        cmd.Parameters.AddWithValue("@filterProducer", producer);
                    }


                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var item = new VinN
                            {
                                label = rdr.GetString(0),
                                producer = rdr.GetString(1),
                                country = rdr.GetString(2),
                                region = rdr.GetString(3),
                                location = rdr.GetString(4),
                                locale = rdr.GetString(5),
                                site = rdr.GetString(6),

                                colorClass = rdr.GetString(7),
                                variety = rdr.GetString(8),
                                dryness = rdr.GetString(9)
                            };
                            l.Add(item);
                        }
                    }
                    return l;
                }

            }
        }

        public IEnumerable<VinN> SearchLabel(string filter)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<VinN> SearchProducer(string filter)
        {

            var l = new List<VinN>();


            if (string.IsNullOrEmpty(filter))
            {
                return l;
            }

            var sb = new StringBuilder();
            
            sb.Append("  select top 100 producerToShow " ); 
            sb.Append("  FROM vWineVinNDetails as v  WITH (NOEXPAND) " );
            sb.Append("  INNER JOIN CONTAINSTABLE(vWineVinNDetails,producerToShow,@filter,300) AS KEY_TBL  ON v.Wine_VinN_ID =  KEY_TBL.[KEY]");
            sb.Append("  group by producerToShow ");



            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@filter", Utils.BuildSearchString(filter));

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var item = new VinN
                            {
                                producer = rdr.GetString(0)
                            };
                            l.Add(item);
                        }
                    }


                    return l;
                }

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public IEnumerable<VinN> SearchProducerExtended(string filter)
        {
            var l = new List<VinN>();


            if (string.IsNullOrEmpty(filter))
            {
                return l;
            }

            var sb = new StringBuilder();


            sb.Append(" select top 100 label, producerToShow ");
            sb.Append(", COALESCE(country, ''), COALESCE(region, ''),  COALESCE(Location, ''),  COALESCE(locale, ''),  COALESCE(site, '')  ");
            sb.Append(", COALESCE(color, ''), COALESCE(variety, ''),  COALESCE(dryness, '')  ");

            sb.Append("  FROM vWineVinNDetails as v  WITH (NOEXPAND) ");
            sb.Append("  INNER JOIN CONTAINSTABLE(vWineVinNDetails,name,@filter,300) AS KEY_TBL  ON v.Wine_VinN_ID =  KEY_TBL.[KEY]");



            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@filter", Utils.BuildSearchString(filter));

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var item = new VinN
                            {
                                label = rdr.GetString(0),
                                producer = rdr.GetString(1),
                                country = rdr.GetString(2),
                                region = rdr.GetString(3),
                                location = rdr.GetString(4),
                                locale = rdr.GetString(5),
                                site = rdr.GetString(6),

                                colorClass = rdr.GetString(7),
                                variety = rdr.GetString(8),
                                dryness = rdr.GetString(9),
                                
                            }; 
                            l.Add(item);
                        }
                    }


                    return l;
                }

            }
        }





        public IEnumerable<VinN> SearchWineN(string filter)
        {
            //var l = new List<VinN>();
            var dict = new Dictionary<Int32, VinN>(); 

            if (string.IsNullOrEmpty(filter))
            {
                return dict.Values.ToList();
            }

            string sb = @" 


                     select top 500 
                          v.Wine_VinN_ID 
                        , v.label
                        , v.producerToShow 
                        , COALESCE(v.country, '')
                        , COALESCE(v.region, '')
                        , COALESCE(v.Location, '')
                        , COALESCE(v.locale, '')
                        , COALESCE(v.site, '')  
                        , COALESCE(v.color, '')
                        , COALESCE(v.variety, '')
                        , COALESCE(v.dryness, '') 
                        , COALESCE(v.type, 'Table') 
                        , v.Wine_VinN_WF_StatusID


						, rfc.id
						, rfc.label
						, rfc.producer 
						, COALESCE(rfc.loccountry, '')  as rfccountry
						, COALESCE(rfc.locregion, '')   as rfcregion
						, COALESCE(rfc.locLocation, '') as rfclocation
						, COALESCE(rfc.loclocale, '')  as rfclocale
						, COALESCE(rfc.locsite, '')    as rfcsite  
						, COALESCE(rfc.color, '')    as rfccolor
						, COALESCE(rfc.variety, '')  as rfcvariety
						, COALESCE(rfc.dryness, '') as rfcdryness 
						, COALESCE(rfc.type, 'Table') as rfctype 


                          FROM vWineVinNDetails as v  WITH (NOEXPAND) 
                          INNER JOIN CONTAINSTABLE(vWineVinNDetails,keywords,@filter,500) AS KEY_TBL  ON v.Wine_VinN_ID =  KEY_TBL.[KEY]
                          left join  Wine_VinN_RFC as rfc  on v.Wine_VinN_ID = rfc.VinN_ID
                          where v.color <> ''
                          order by v.Wine_VinN_ID

 
                         select top 1000 
                         v.Wine_VinN_ID
                        , w.ID 
                        , wv.name 
                        , w.WF_StatusID

                          FROM vWineVinNDetails as v  WITH (NOEXPAND) 
                          INNER JOIN CONTAINSTABLE(vWineVinNDetails,keywords,@filter,1000) AS KEY_TBL  ON v.Wine_VinN_ID =  KEY_TBL.[KEY]
                          INNER JOIN Wine_N as w on w.Wine_VinN_ID = v.Wine_VinN_ID
                          INNER JOIN WineVintage as wv on wv.ID = w.VintageID
                          where color <> ''
                          order by v.Wine_VinN_ID ,wv.name desc
           ";





            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();


                    filter = Utils.BuildSearchString(filter);

                    cmd.Parameters.AddWithValue("@filter", filter);

                    cmd.CommandTimeout = 60;

                    using (var rdr = cmd.ExecuteReader())
                    {
                        //
                        // sort wines by vintage  descending
                        //
                        //

                        while (rdr.Read())
                        {

                            if (!rdr.IsDBNull(13))
                            {
                                VinN_Ext_RFC vinN = new VinN_Ext_RFC
                                {
                                    id = rdr.GetInt32(0),
                                    label = rdr.GetString(1),
                                    producer = rdr.GetString(2),
                                    country = rdr.GetString(3),
                                    region = rdr.GetString(4),
                                    location = rdr.GetString(5),
                                    locale = rdr.GetString(6),
                                    site = rdr.GetString(7),

                                    colorClass = rdr.GetString(8),
                                    variety = rdr.GetString(9),
                                    dryness = rdr.GetString(10),
                                    wineType = rdr.GetString(11)

                                };


                                vinN.rfc = new VinN_RFC()
                                {
                                    id = rdr.GetInt32(13),
                                    label = rdr.GetString(14),
                                    producer = rdr.GetString(15),
                                    country = rdr.GetString(16),
                                    region = rdr.GetString(17),
                                    location = rdr.GetString(18),
                                    locale = rdr.GetString(19),
                                    site = rdr.GetString(20),

                                    colorClass = rdr.GetString(21),
                                    variety = rdr.GetString(22),
                                    dryness = rdr.GetString(23),
                                    wineType = rdr.GetString(24)
                                };

                                vinN.wines = new List<WineN>();
                                dict.Add(vinN.id, vinN);
                            }
                            else
                            {


                                VinN vinN = new VinN
                                    {
                                        id = rdr.GetInt32(0),
                                        label = rdr.GetString(1),
                                        producer = rdr.GetString(2),
                                        country = rdr.GetString(3),
                                        region = rdr.GetString(4),
                                        location = rdr.GetString(5),
                                        locale = rdr.GetString(6),
                                        site = rdr.GetString(7),

                                        colorClass = rdr.GetString(8),
                                        variety = rdr.GetString(9),
                                        dryness = rdr.GetString(10),
                                        wineType = rdr.GetString(11),
                                        workflow = rdr.GetFieldValue<Int16>(12)

                                    };
                                vinN.wines = new List<WineN>();
                                dict.Add(vinN.id, vinN);
                            }

                        }

                        if (rdr.NextResult())
                        {
                            while (rdr.Read())
                            {
                                Int32 vinId = rdr.GetInt32(0);

                                if (dict.ContainsKey(vinId))
                                {
                                    var vinN = dict[vinId];

                                    if (vinN != null)
                                    {
                                        vinN.wines.Add(new WineN()
                                        {
                                            id = rdr.GetInt32(1)
                                            ,vintage = rdr.GetString(2)
                                            ,workflow = rdr.GetFieldValue<Int16>(3)
                                        });
                                    }
                                }
                            }
                        }

                    }

                    return dict.Values.ToList();
                }

            }
        }



        public IEnumerable<VinN> SearchNewWineN()
        {
            //var l = new List<VinN>();
            var dict = new Dictionary<Int32, VinN>();


            string sb = @" 

          select top 1000  * from
             (
             select  
              v.Wine_VinN_ID 
            , v.label  as rfclabel
            , v.producerToShow 
            , COALESCE(v.country, '')  as country
            , COALESCE(v.region, '')   as region
            , COALESCE(v.Location, '') as location
            , COALESCE(v.locale, '')   as locale
            , COALESCE(v.site, '')     as site  
            , COALESCE(v.color, '')    as color
            , COALESCE(v.variety, '')  as variety
            , COALESCE(v.dryness, '')  as dryness 
            , COALESCE(v.type, 'Table')  as type 
            , v.Wine_VinN_WF_StatusID

			, rfc.id
			, rfc.label
			, rfc.producer 
			, COALESCE(rfc.loccountry, '')  as rfccountry
			, COALESCE(rfc.locregion, '')   as rfcregion
			, COALESCE(rfc.locLocation, '') as rfclocation
			, COALESCE(rfc.loclocale, '')  as rfclocale
			, COALESCE(rfc.locsite, '')    as rfcsite  
			, COALESCE(rfc.color, '')    as rfccolor
			, COALESCE(rfc.variety, '')  as rfcvariety
			, COALESCE(rfc.dryness, '') as rfcdryness 
			, COALESCE(rfc.type, 'Table') as rfctype 

              FROM vWineVinNDetails as v  WITH (NOEXPAND)
              left join  Wine_VinN_RFC as rfc  on v.Wine_VinN_ID = rfc.VinN_ID
              where  v.Wine_VinN_WF_StatusID < 100

            union

             select distinct 
              v.Wine_VinN_ID 
            , v.label
            , v.producerToShow 
            , COALESCE(v.country, '')  as country
            , COALESCE(v.region, '')   as region
            , COALESCE(v.Location, '') as location
            , COALESCE(v.locale, '')  as locale
            , COALESCE(v.site, '')    as site  
            , COALESCE(v.color, '')    as color
            , COALESCE(v.variety, '')  as variety
            , COALESCE(v.dryness, '') as dryness 
            , COALESCE(v.type, 'Table') as type 
            , v.Wine_VinN_WF_StatusID

			, rfc.id
			, rfc.label as rfclabel
			, rfc.producer 
			, COALESCE(rfc.loccountry, '')  as rfccountry
			, COALESCE(rfc.locregion, '')   as rfcregion
			, COALESCE(rfc.locLocation, '') as rfclocation
			, COALESCE(rfc.loclocale, '')  as rfclocale
			, COALESCE(rfc.locsite, '')    as rfcsite  
			, COALESCE(rfc.color, '')    as rfccolor
			, COALESCE(rfc.variety, '')  as rfcvariety
			, COALESCE(rfc.dryness, '') as rfcdryness 
			, COALESCE(rfc.type, 'Table') as rfctype 

              FROM vWineVinNDetails as v  WITH (NOEXPAND)
              left join Wine_N as w on w.Wine_VinN_ID = v.Wine_VinN_ID
              left join  Wine_VinN_RFC as rfc  on v.Wine_VinN_ID = rfc.VinN_ID
              where  w.WF_StatusID < 100

              )  as v 
              order by Wine_VinN_ID          

            ---
            ---
            ---

            select
                w.Wine_VinN_ID
                , w.ID 
                , wv.name 
                , w.WF_StatusID

                  FROM vWineVinNDetails as v  WITH (NOEXPAND) 
                  INNER JOIN Wine_N as w on w.Wine_VinN_ID = v.Wine_VinN_ID
                  INNER JOIN WineVintage as wv on wv.ID = w.VintageID
                  where v.Wine_VinN_WF_StatusID < 100

            union

            select
                w.Wine_VinN_ID
                , w.ID 
                , wv.name 
                , w.WF_StatusID

                  FROM  Wine_N as w
                  INNER JOIN WineVintage as wv on wv.ID = w.VintageID
                  where w.WF_StatusID < 100
      
               order by w.Wine_VinN_ID ,wv.name 
           ";





            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();

                    using (var rdr = cmd.ExecuteReader())
                    {
                        //
                        // sort wines by vintage descending
                        //
                        //

                        while (rdr.Read())
                        {
                            if (!rdr.IsDBNull(13))
                            {
                                VinN_Ext_RFC vinN = new VinN_Ext_RFC
                                {
                                    id = rdr.GetInt32(0),
                                    label = rdr.GetString(1),
                                    producer = rdr.GetString(2),
                                    country = rdr.GetString(3),
                                    region = rdr.GetString(4),
                                    location = rdr.GetString(5),
                                    locale = rdr.GetString(6),
                                    site = rdr.GetString(7),

                                    colorClass = rdr.GetString(8),
                                    variety = rdr.GetString(9),
                                    dryness = rdr.GetString(10),
                                    wineType = rdr.GetString(11)

                                };


                                vinN.rfc = new VinN_RFC()
                                {
                                    id = rdr.GetInt32(13),
                                    label = rdr.GetString(14),
                                    producer = rdr.GetString(15),
                                    country = rdr.GetString(16),
                                    region = rdr.GetString(17),
                                    location = rdr.GetString(18),
                                    locale = rdr.GetString(19),
                                    site = rdr.GetString(20),

                                    colorClass = rdr.GetString(21),
                                    variety = rdr.GetString(22),
                                    dryness = rdr.GetString(23),
                                    wineType = rdr.GetString(24)
                                };

                                vinN.wines = new List<WineN>();
                                dict.Add(vinN.id, vinN);
                            }
                            else
                            {

                                VinN vinN = new VinN
                                {
                                    id = rdr.GetInt32(0),
                                    label = rdr.GetString(1),
                                    producer = rdr.GetString(2),
                                    country = rdr.GetString(3),
                                    region = rdr.GetString(4),
                                    location = rdr.GetString(5),
                                    locale = rdr.GetString(6),
                                    site = rdr.GetString(7),

                                    colorClass = rdr.GetString(8),
                                    variety = rdr.GetString(9),
                                    dryness = rdr.GetString(10),
                                    wineType = rdr.GetString(11),
                                    workflow = rdr.GetFieldValue<Int16>(12)

                                };
                                vinN.wines = new List<WineN>();
                                dict.Add(vinN.id, vinN);
                            }

                        }

                        if (rdr.NextResult())
                        {
                            while (rdr.Read())
                            {
                                Int32 vinId = rdr.GetInt32(0);
                                VinN vinN = null;

                                if (dict.TryGetValue(vinId, out vinN))
                                {
                                    vinN.wines.Add(new WineN()
                                    {
                                        id = rdr.GetInt32(1),
                                        vintage = rdr.GetString(2),
                                        workflow = rdr.GetFieldValue<Int16>(3)
                                    });
                                }

                            }
                        }

                    }


                    return dict.Values.ToList();
                }

            }
        }




        public IEnumerable<VinN> SearchLocation(string country,string region, string location,string locale, string site)
        {

            var l = new List<VinN>();


            if (string.IsNullOrEmpty(country) 
                && string.IsNullOrEmpty(region) 
                && string.IsNullOrEmpty(location) 
                && string.IsNullOrEmpty(locale) 
                && string.IsNullOrEmpty(site))
            {
                return l;
            }

            var sb = new StringBuilder();

            sb.Append(" select distinct  ");

            if (!String.IsNullOrEmpty(country) || !String.IsNullOrEmpty(region) || !String.IsNullOrEmpty(location) || !String.IsNullOrEmpty(locale) || !String.IsNullOrEmpty(site))
            {
                sb.Append(" COALESCE(country, '')  ");
            }
            else
            {
                sb.Append(" '' as country  ");
            }

            if (!String.IsNullOrEmpty(region) || !String.IsNullOrEmpty(location) || !String.IsNullOrEmpty(locale) || !String.IsNullOrEmpty(site))
            {
                sb.Append(" ,COALESCE(region, '')  ");
            }
            else
            {
                sb.Append(" ,'' as region  ");
            }

            if (!String.IsNullOrEmpty(location) || !String.IsNullOrEmpty(locale) || !String.IsNullOrEmpty(site))
            {
                sb.Append(" ,COALESCE(location, '')  ");
            }
            else
            {
                sb.Append(" ,'' as location  ");
            }
            if (!String.IsNullOrEmpty(locale) || !String.IsNullOrEmpty(site))
            {
                sb.Append(" ,COALESCE(locale, '')  ");
            }
            else
            {
                sb.Append(" ,'' as locale  ");
            }
            if (!String.IsNullOrEmpty(site))
            {
                sb.Append(" ,COALESCE(site, '')  ");
            }
            else
            {
                sb.Append(" ,'' as site  ");
            }
        
            
            
            sb.Append("  FROM vWineVinNDetails as v  WITH (NOEXPAND) ");

            if (!String.IsNullOrEmpty(country))
            {
                sb.Append("  INNER JOIN CONTAINSTABLE(vWineVinNDetails,country,@filterCountry) AS KEY_TBL1  ON v.Wine_VinN_ID =  KEY_TBL1.[KEY]");
            }
            if (!String.IsNullOrEmpty(region))
            {
                sb.Append("  INNER JOIN CONTAINSTABLE(vWineVinNDetails,region,@filterRegion) AS KEY_TBL2  ON v.Wine_VinN_ID =  KEY_TBL2.[KEY]");
            }
            if (!String.IsNullOrEmpty(location))
            {
                sb.Append("  INNER JOIN CONTAINSTABLE(vWineVinNDetails,location,@filterLocation) AS KEY_TBL3  ON v.Wine_VinN_ID =  KEY_TBL3.[KEY]");
            }
            if (!String.IsNullOrEmpty(locale))
            {
                sb.Append("  INNER JOIN CONTAINSTABLE(vWineVinNDetails,locale,@filterLocale) AS KEY_TBL4  ON v.Wine_VinN_ID =  KEY_TBL4.[KEY]");
            }
            if (!String.IsNullOrEmpty(site))
            {
                sb.Append("  INNER JOIN CONTAINSTABLE(vWineVinNDetails,country,@filterSite) AS KEY_TBL5  ON v.Wine_VinN_ID =  KEY_TBL5.[KEY]");
            }



            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();
                    if (!String.IsNullOrEmpty(country))
                    {
                        cmd.Parameters.AddWithValue("@filterCountry", Utils.BuildSearchString(country));
                    }
                    if (!String.IsNullOrEmpty(region))
                    {
                        cmd.Parameters.AddWithValue("@filterRegion", Utils.BuildSearchString(region));
                    }
                    if (!String.IsNullOrEmpty(location))
                    {
                        cmd.Parameters.AddWithValue("@filterLocation", Utils.BuildSearchString(location));
                    }
                    if (!String.IsNullOrEmpty(locale))
                    {
                        cmd.Parameters.AddWithValue("@filterLocale", Utils.BuildSearchString(locale));
                    }
                    if (!String.IsNullOrEmpty(site))
                    {
                        cmd.Parameters.AddWithValue("@filterSite", Utils.BuildSearchString(site));
                    }


                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var item = new VinN
                            {
                                country = rdr.GetString(0),
                                region = rdr.GetString(1),
                                location = rdr.GetString(2),
                                locale = rdr.GetString(3),
                                site = rdr.GetString(4)
                            };
                            l.Add(item); 
                        }
                    }


                    return l;
                }

            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        public int ApproveVin(int id)
        {
            string sb = @"

                update Wine_N set WF_StatusID = 100 where Wine_VinN_ID = @ID

                update Wine_VinN set WF_StatusID = 100 where ID = @ID
          ";

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@ID", id);

                    return cmd.ExecuteNonQuery(); 

                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public IEnumerable<VinN> LoadSimilar(int flag)
        {
            //var l = new List<VinN>();
            var result = new List< VinN>();


            StringBuilder sb = new StringBuilder(@" 
        select distinct top 200
        v1.ID as ID,

        wl1.Name    as Label, 
        wp1.Name    as Producer,

        locC.Name   as Country,
        locR.Name   as Region,
        locLoc.Name as Location,
        locL.Name   as Locale,
        locS.Name   as Site,

        wt1.Name    as Type ,
        wv1.Name    as Variety ,
        wd1.Name    as Dryness,
        wc1.Name    as Color

        from Wine_VinN v1

        join Wine_VinN    v2   on  v1.ID  <> v2.ID
");

            sb.Append((flag & VinN.SIMILAR_PRODUCER) == 0 ? "" : " and v1.ProducerID    = v2.ProducerID ");
            sb.Append((flag & VinN.SIMILAR_TYPE) == 0 ? "" : " and v1.TypeID        = v2.TypeID ");
            sb.Append((flag & VinN.SIMILAR_LABEL) == 0 ? "" : " and v1.LabelID       = v2.LabelID ");
            sb.Append((flag & VinN.SIMILAR_VARIETY) == 0 ? "" : " and v1.VarietyID     = v2.VarietyID ");
            sb.Append((flag & VinN.SIMILAR_DRYNESS) == 0 ? "" : " and v1.DrynessID     = v2.DrynessID ");
            sb.Append((flag & VinN.SIMILAR_COUNTRY) == 0 ? "" : " and v1.locCountryID  = v2.locCountryID ");
            sb.Append((flag & VinN.SIMILAR_REGION) == 0 ? "" : " and v1.locRegionID   = v2.locRegionID ");
            sb.Append((flag & VinN.SIMILAR_LOCATION) == 0 ? "" : " and v1.locLocationID = v2.locLocationID ");
            sb.Append((flag & VinN.SIMILAR_LOCALE) == 0 ? "" : " and v1.locLocaleID   = v2.locLocaleID ");
            sb.Append((flag & VinN.SIMILAR_SITE) == 0 ? "" : " and v1.locSiteID     = v2.locSiteID ");

            sb.Append(flag == VinN.SIMILAR_ALL ? "" : " and v1.colorid = v2.colorid ");


 sb.Append(@" 

        join WineProducer wp1  on v1.ProducerID = wp1.ID
        join WineLabel    wl1  on v1.LabelID    = wl1.ID
        join WineType     wt1  on v1.typeID     = wt1.ID
        join WineVariety  wv1  on v1.VarietyID  = wv1.ID
        join WineColor    wc1  on v1.colorid    = wc1.ID
        join WineDryness  wd1  on v1.DrynessID  = wd1.ID


        join LocationCountry  locC on v1.locCountryID      = locC.ID
        join LocationRegion   locR on v1.locRegionID       = locR.ID
        join LocationLocation locLoc on v1.locLocationID   = locLoc.ID
        join LocationLocale   locL   on v1.locLocaleID     = locL.ID
        join LocationSite     locS   on v1.locSiteID       = locS.ID

     
        order by Label, Producer

");





            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();
                    cmd.CommandTimeout = 60;

                    using (var rdr = cmd.ExecuteReader())
                    {
                        //
                        // sort wines by vintage  descending
                        //
                        //

                        while (rdr.Read())
                        {
                            Int32 vinId = rdr.GetInt32(0);

                            VinN vinN = new VinExt
                            {
                                id = rdr.GetInt32(0),
                                label = rdr.GetString(1),
                                producer = rdr.GetString(2),
                                country = rdr.GetString(3),
                                region = rdr.GetString(4),
                                location = rdr.GetString(5),
                                locale = rdr.GetString(6),
                                site = rdr.GetString(7),

                                wineType = rdr.GetString(8),
                                variety = rdr.GetString(9),
                                dryness = rdr.GetString(10),
                                colorClass = rdr.GetString(11)

                            };

                            result.Add( vinN);
                        }
                    }
                    return result;
                }

            }
        }


//        public VinN Search(VinN vinN)
//        {
//            string sb = @" exec WineVin_GetID
//	                        @Producer=@Producer
//                           , @WineType=@WineType
//                           , @Label=@Label
//                           , @Variety=@Variety
//                           , @Dryness = @Dryness 
//                           , @Color = @Color
//                           , @locCountry = @locRegion
//                           , @locRegion = @locRegion
//                           , @locLocation = @locLocation
//                           , @locLocale = @locLocale
//                           , @locSite = @locSite
//                           , @IsAutoCreate bit = 0  ";

//            using (SqlConnection conn = _connFactory.GetConnection())
//            {
//                using (SqlCommand cmd = new SqlCommand("", conn))
//                {
//                    cmd.CommandText = sb.ToString();

//                    cmd.Parameters.AddWithValue("@Producer",String.IsNullOrEmpty(vinN.producer) == true ? "" : vinN.producer);
//                    cmd.Parameters.AddWithValue("@WineType",String.IsNullOrEmpty(vinN.wineType) == true ? "" : vinN.wineType);
//                    cmd.Parameters.AddWithValue("@Label",String.IsNullOrEmpty(vinN.label) == true ? "" : vinN.label);
//                    cmd.Parameters.AddWithValue("@Variety",String.IsNullOrEmpty(vinN.variety) == true ? "" : vinN.variety);
//                    cmd.Parameters.AddWithValue("@Dryness",String.IsNullOrEmpty(vinN.dryness) == true ? "" : vinN.dryness); 
//                    cmd.Parameters.AddWithValue("@Color",String.IsNullOrEmpty(vinN.colorClass) == true ? "" : vinN.colorClass);
//                    cmd.Parameters.AddWithValue("@locCountry",String.IsNullOrEmpty(vinN.country) == true ? "" : vinN.country);
//                    cmd.Parameters.AddWithValue("@locRegion",String.IsNullOrEmpty(vinN.region) == true ? "" : vinN.region);
//                    cmd.Parameters.AddWithValue("@locLocation",String.IsNullOrEmpty(vinN.location) == true ? "" : vinN.location);
//                    cmd.Parameters.AddWithValue("@locLocale",String.IsNullOrEmpty(vinN.locale) == true ? "" : vinN.locale);
//                    cmd.Parameters.AddWithValue("@locSite", String.IsNullOrEmpty(vinN.site) == true ? "" : vinN.site);



//                    using(SqlDataReader rdr = cmd.ExecuteReader()){

//                        if (rdr.Read())
//                        {
//                            vinN.id = rdr.GetInt32(0);
//                        }
//                        else
//                        {
//                            vinN.id = 0;
//                        }
//                    }

//                    return vinN;

//                }

//            }
//        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public VinN Update(VinN vinN)
        {
            string sb = @" exec WineVin_UpdateEx
	                        @ID=@ID
	                       , @Producer=@Producer
                           , @WineType=@WineType
                           , @Label=@Label
                           , @Variety=@Variety
                           , @Dryness = @Dryness 
                           , @Color = @Color
                           , @locCountry = @locCountry
                           , @locRegion = @locRegion
                           , @locLocation = @locLocation
                           , @locLocale = @locLocale
                           , @locSite = @locSite";


            using (SqlConnection conn = _connFactory.GetConnection())
            {

                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.AddWithValue("@ID", vinN.id);
                    cmd.Parameters.AddWithValue("@Producer", String.IsNullOrEmpty(vinN.producer) == true ? "" : vinN.producer.Trim());
                    cmd.Parameters.AddWithValue("@WineType", String.IsNullOrEmpty(vinN.wineType) == true ? "" : vinN.wineType.Trim());
                    cmd.Parameters.AddWithValue("@Label", String.IsNullOrEmpty(vinN.label) == true ? "" : vinN.label.Trim());
                    cmd.Parameters.AddWithValue("@Variety", String.IsNullOrEmpty(vinN.variety) == true ? "" : vinN.variety.Trim());
                    cmd.Parameters.AddWithValue("@Dryness", String.IsNullOrEmpty(vinN.dryness) == true ? "" : vinN.dryness.Trim());
                    cmd.Parameters.AddWithValue("@Color", String.IsNullOrEmpty(vinN.colorClass) == true ? "" : vinN.colorClass.Trim());
                    cmd.Parameters.AddWithValue("@locCountry", String.IsNullOrEmpty(vinN.country) == true ? "" : vinN.country.Trim());
                    cmd.Parameters.AddWithValue("@locRegion", String.IsNullOrEmpty(vinN.region) == true ? "" : vinN.region.Trim());
                    cmd.Parameters.AddWithValue("@locLocation", String.IsNullOrEmpty(vinN.location) == true ? "" : vinN.location.Trim());
                    cmd.Parameters.AddWithValue("@locLocale", String.IsNullOrEmpty(vinN.locale) == true ? "" : vinN.locale.Trim());
                    cmd.Parameters.AddWithValue("@locSite", String.IsNullOrEmpty(vinN.site) == true ? "" : vinN.site.Trim());



                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        vinN.id = 0;

                        if (rdr.NextResult() && rdr.Read())
                        {
                            vinN.id = rdr.GetInt32(0);
                        }

                        if (rdr.NextResult() && rdr.Read())
                        {
                            vinN.id = rdr.GetInt32(0);
                        }
                        
                    }

                    return vinN;
                }

            }
        }


        public VinN CreateRFC(int authorId, VinN vinN)
        {
            string sb = @" 

INSERT INTO Wine_VinN_RFC
           ([VinN_ID]
           ,[Producer]
           ,[Type]
           ,[Label]
           ,[Variety]
           ,[Dryness]
           ,[Color]
           ,[locCountry]
           ,[locRegion]
           ,[locLocation]
           ,[locLocale]
           ,[locSite]
           ,[author])
     VALUES
           (  @VinN_ID
	        , @Producer
            , @WineType
            , @Label
            , @Variety
            , @Dryness 
            , @Color
            , @locCountry
            , @locRegion
            , @locLocation
            , @locLocale
            , @locSite
            , @authorID);


       select  
              v.Wine_VinN_ID 
              , v.label
              , v.producerToShow 
            , COALESCE(v.country, '')
            , COALESCE(v.region, '')
            , COALESCE(v.Location, '')
            , COALESCE(v.locale, '')
            , COALESCE(v.site, '')  
            , COALESCE(v.color, '')
            , COALESCE(v.variety, '')
            , COALESCE(v.dryness, '') 
            , COALESCE(v.type, 'Table') 
            , v.Wine_VinN_WF_StatusID


			, rfc.id
			, rfc.label
			, rfc.producer 
			, COALESCE(rfc.loccountry, '')  as rfccountry
			, COALESCE(rfc.locregion, '')   as rfcregion
			, COALESCE(rfc.locLocation, '') as rfclocation
			, COALESCE(rfc.loclocale, '')  as rfclocale
			, COALESCE(rfc.locsite, '')    as rfcsite  
			, COALESCE(rfc.color, '')    as rfccolor
			, COALESCE(rfc.variety, '')  as rfcvariety
			, COALESCE(rfc.dryness, '') as rfcdryness 
			, COALESCE(rfc.type, 'Table') as rfctype 


            FROM vWineVinNDetails as v  WITH (NOEXPAND) 
            left join  Wine_VinN_RFC as rfc  on v.Wine_VinN_ID = rfc.VinN_ID
            where 
            v.Wine_VinN_ID = @VinN_ID 
            order by v.Wine_VinN_ID

";


            using (SqlConnection conn = _connFactory.GetConnection())
            {

                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();

                    cmd.Parameters.AddWithValue("@VinN_ID", vinN.id);
                    cmd.Parameters.AddWithValue("@Producer", String.IsNullOrEmpty(vinN.producer) == true ? "" : vinN.producer.Trim());
                    cmd.Parameters.AddWithValue("@WineType", String.IsNullOrEmpty(vinN.wineType) == true ? "" : vinN.wineType.Trim());
                    cmd.Parameters.AddWithValue("@Label", String.IsNullOrEmpty(vinN.label) == true ? "" : vinN.label.Trim());
                    cmd.Parameters.AddWithValue("@Variety", String.IsNullOrEmpty(vinN.variety) == true ? "" : vinN.variety.Trim());
                    cmd.Parameters.AddWithValue("@Dryness", String.IsNullOrEmpty(vinN.dryness) == true ? "" : vinN.dryness.Trim());
                    cmd.Parameters.AddWithValue("@Color", String.IsNullOrEmpty(vinN.colorClass) == true ? "" : vinN.colorClass.Trim());
                    cmd.Parameters.AddWithValue("@locCountry", String.IsNullOrEmpty(vinN.country) == true ? "" : vinN.country.Trim());
                    cmd.Parameters.AddWithValue("@locRegion", String.IsNullOrEmpty(vinN.region) == true ? "" : vinN.region.Trim());
                    cmd.Parameters.AddWithValue("@locLocation", String.IsNullOrEmpty(vinN.location) == true ? "" : vinN.location.Trim());
                    cmd.Parameters.AddWithValue("@locLocale", String.IsNullOrEmpty(vinN.locale) == true ? "" : vinN.locale.Trim());
                    cmd.Parameters.AddWithValue("@locSite", String.IsNullOrEmpty(vinN.site) == true ? "" : vinN.site.Trim());
                    cmd.Parameters.AddWithValue("@authorID", authorId);


                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            VinN_Ext_RFC result = new VinN_Ext_RFC
                            {
                                id = rdr.GetInt32(0),
                                label = rdr.GetString(1),
                                producer = rdr.GetString(2),
                                country = rdr.GetString(3),
                                region = rdr.GetString(4),
                                location = rdr.GetString(5),
                                locale = rdr.GetString(6),
                                site = rdr.GetString(7),

                                colorClass = rdr.GetString(8),
                                variety = rdr.GetString(9),
                                dryness = rdr.GetString(10),
                                wineType = rdr.GetString(11)

                            };


                            result.rfc = new VinN_RFC()
                            {
                                id = rdr.GetInt32(13),
                                label = rdr.GetString(14),
                                producer = rdr.GetString(15),
                                country = rdr.GetString(16),
                                region = rdr.GetString(17),
                                location = rdr.GetString(18),
                                locale = rdr.GetString(19),
                                site = rdr.GetString(20),

                                colorClass = rdr.GetString(21),
                                variety = rdr.GetString(22),
                                dryness = rdr.GetString(23),
                                wineType = rdr.GetString(24)
                            };

                            result.wines = new List<WineN>();
                            return result;

                        }
                        else
                        {
                            return null;
                        }

                    }

                }

            }
        }





        public IEnumerable<VinN> LoadRFC()
        {
            //var l = new List<VinN>();
            var dict = new Dictionary<Int32, VinN>();


            string sb = @" 


                     select top 500 
                          v.Wine_VinN_ID 
                        , v.label
                        , v.producerToShow 
                        , COALESCE(v.country, '')
                        , COALESCE(v.region, '')
                        , COALESCE(v.Location, '')
                        , COALESCE(v.locale, '')
                        , COALESCE(v.site, '')  
                        , COALESCE(v.color, '')
                        , COALESCE(v.variety, '')
                        , COALESCE(v.dryness, '') 
                        , COALESCE(v.type, 'Table') 
                        , v.Wine_VinN_WF_StatusID


						, rfc.id
						, rfc.label
						, rfc.producer 
						, COALESCE(rfc.loccountry, '')  as rfccountry
						, COALESCE(rfc.locregion, '')   as rfcregion
						, COALESCE(rfc.locLocation, '') as rfclocation
						, COALESCE(rfc.loclocale, '')  as rfclocale
						, COALESCE(rfc.locsite, '')    as rfcsite  
						, COALESCE(rfc.color, '')    as rfccolor
						, COALESCE(rfc.variety, '')  as rfcvariety
						, COALESCE(rfc.dryness, '') as rfcdryness 
						, COALESCE(rfc.type, 'Table') as rfctype 


                          FROM vWineVinNDetails as v  WITH (NOEXPAND) 
                          left join  Wine_VinN_RFC as rfc  on v.Wine_VinN_ID = rfc.VinN_ID
                          where v.color <> ''
                          and rfc.id is not null
                          order by v.Wine_VinN_ID

 
                         select top 1000 
                         v.Wine_VinN_ID
                        , w.ID 
                        , wv.name 
                        , w.WF_StatusID

                          FROM vWineVinNDetails as v  WITH (NOEXPAND) 
                          INNER JOIN Wine_N as w on w.Wine_VinN_ID = v.Wine_VinN_ID
                          INNER JOIN WineVintage as wv on wv.ID = w.VintageID
                          left join  Wine_VinN_RFC as rfc  on v.Wine_VinN_ID = rfc.VinN_ID
                          where v.color <> ''
                          and rfc.id is not null
                          order by v.Wine_VinN_ID ,wv.name desc
           ";





            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();
                    cmd.CommandTimeout = 60;

                    using (var rdr = cmd.ExecuteReader())
                    {
                        //
                        // sort wines by vintage  descending
                        //
                        //

                        while (rdr.Read())
                        {

                            if (!rdr.IsDBNull(13))
                            {
                                VinN_Ext_RFC vinN = new VinN_Ext_RFC
                                {
                                    id = rdr.GetInt32(0),
                                    label = rdr.GetString(1),
                                    producer = rdr.GetString(2),
                                    country = rdr.GetString(3),
                                    region = rdr.GetString(4),
                                    location = rdr.GetString(5),
                                    locale = rdr.GetString(6),
                                    site = rdr.GetString(7),

                                    colorClass = rdr.GetString(8),
                                    variety = rdr.GetString(9),
                                    dryness = rdr.GetString(10),
                                    wineType = rdr.GetString(11)

                                };


                                vinN.rfc = new VinN_RFC()
                                {
                                    id = rdr.GetInt32(13),
                                    label = rdr.GetString(14),
                                    producer = rdr.GetString(15),
                                    country = rdr.GetString(16),
                                    region = rdr.GetString(17),
                                    location = rdr.GetString(18),
                                    locale = rdr.GetString(19),
                                    site = rdr.GetString(20),

                                    colorClass = rdr.GetString(21),
                                    variety = rdr.GetString(22),
                                    dryness = rdr.GetString(23),
                                    wineType = rdr.GetString(24)
                                };

                                vinN.wines = new List<WineN>();
                                dict.Add(vinN.id, vinN);
                            }
                            else
                            {


                                VinN vinN = new VinN
                                {
                                    id = rdr.GetInt32(0),
                                    label = rdr.GetString(1),
                                    producer = rdr.GetString(2),
                                    country = rdr.GetString(3),
                                    region = rdr.GetString(4),
                                    location = rdr.GetString(5),
                                    locale = rdr.GetString(6),
                                    site = rdr.GetString(7),

                                    colorClass = rdr.GetString(8),
                                    variety = rdr.GetString(9),
                                    dryness = rdr.GetString(10),
                                    wineType = rdr.GetString(11),
                                    workflow = rdr.GetFieldValue<Int16>(12)

                                };
                                vinN.wines = new List<WineN>();
                                dict.Add(vinN.id, vinN);
                            }

                        }

                        if (rdr.NextResult())
                        {
                            while (rdr.Read())
                            {
                                Int32 vinId = rdr.GetInt32(0);

                                if (dict.ContainsKey(vinId))
                                {
                                    var vinN = dict[vinId];

                                    if (vinN != null)
                                    {
                                        vinN.wines.Add(new WineN()
                                        {
                                            id = rdr.GetInt32(1)
                                            ,
                                            vintage = rdr.GetString(2)
                                            ,
                                            workflow = rdr.GetFieldValue<Int16>(3)
                                        });
                                    }
                                }
                            }
                        }

                    }

                    return dict.Values.ToList();
                }

            }
        }




        public VinN DeleteRFC(VinN_Ext_RFC vinN)
        {
            string sb = @" DELETE FROM  Wine_VinN_RFC where id = @ID ";


            using (SqlConnection conn = _connFactory.GetConnection())
            {

                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@ID", vinN.rfc.id);


                    cmd.ExecuteNonQuery();

                    vinN.rfc = null;
                    return vinN;
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vinN"></param>
        /// <returns></returns>
        public VinN ApproveRFC(VinN_Ext_RFC vinN)
        {

            if (vinN.rfc != null)
            {
                vinN.copyFromRFC();

                Update(vinN);

                DeleteRFC(vinN);

                vinN.rfc = null;

                return vinN;

            }

            return null;
        }
    }

}
