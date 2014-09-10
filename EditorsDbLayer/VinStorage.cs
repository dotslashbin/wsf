﻿using EditorsCommon;
using System;
using System.Collections.Generic;
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
        /// <param name="e"></param>
        /// <returns></returns>
        public VinN Update(VinN e)
        {
            throw new NotImplementedException();
        }


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
                        , label, producerToShow 
                        , COALESCE(country, ''), COALESCE(region, ''),  COALESCE(Location, ''),  COALESCE(locale, ''),  COALESCE(site, '')  
                        , COALESCE(color, ''), COALESCE(variety, ''),  COALESCE(dryness, '') 
                        , v.Wine_VinN_WF_StatusID

                          FROM vWineVinNDetails as v  WITH (NOEXPAND) 
                          INNER JOIN CONTAINSTABLE(vWineVinNDetails,name,@filter,500) AS KEY_TBL  ON v.Wine_VinN_ID =  KEY_TBL.[KEY]
                          order by v.Wine_VinN_ID;


                         select top 1000 
                         v.Wine_VinN_ID
                        , w.ID 
                        , wv.name 
                        , w.WF_StatusID

                          FROM vWineVinNDetails as v  WITH (NOEXPAND) 
                          INNER JOIN CONTAINSTABLE(vWineVinNDetails,name,@filter,1000) AS KEY_TBL  ON v.Wine_VinN_ID =  KEY_TBL.[KEY]
                          INNER JOIN Wine_N as w on w.Wine_VinN_ID = v.Wine_VinN_ID
                          INNER JOIN WineVintage as wv on wv.ID = w.VintageID
                          order by v.Wine_VinN_ID ,wv.name desc
           ";





            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@filter", Utils.BuildSearchString(filter));

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

                            VinN  vinN = new VinN
                                {
                                    id = vinId,
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
                                    workflow = rdr.GetFieldValue<Int16>(11)

                                };
                                vinN.wines = new List<WineN>();

                                dict.Add(vinId, vinN);
                            

                        }

                        if (rdr.NextResult())
                        {
                            while (rdr.Read())
                            {
                                Int32 vinId = rdr.GetInt32(0);
                                var vinN = dict[vinId];

                                if (vinN != null)
                                {
                                    vinN.wines.Add(new WineN() { 
                                        id = rdr.GetInt32(1)
                                        , vintage = rdr.GetString(2)
                                        , workflow = rdr.GetFieldValue<Int16>(3)
                                    });
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

             select  
              v.Wine_VinN_ID 
            , label, producerToShow 
            , COALESCE(country, ''), COALESCE(region, ''),  COALESCE(Location, ''),  COALESCE(locale, ''),  COALESCE(site, '')  
            , COALESCE(color, ''), COALESCE(variety, ''),  COALESCE(dryness, '') 
            , v.Wine_VinN_WF_StatusID

              FROM vWineVinNDetails as v  WITH (NOEXPAND)
              where  v.Wine_VinN_WF_StatusID < 100

            union

             select distinct 
              v.Wine_VinN_ID 
            , label, producerToShow 
            , COALESCE(country, ''), COALESCE(region, ''),  COALESCE(Location, ''),  COALESCE(locale, ''),  COALESCE(site, '')  
            , COALESCE(color, ''), COALESCE(variety, ''),  COALESCE(dryness, '') 
            , v.Wine_VinN_WF_StatusID

              FROM vWineVinNDetails as v  WITH (NOEXPAND)
              left join Wine_N as w on w.Wine_VinN_ID = v.Wine_VinN_ID
              where  w.WF_StatusID < 100

              order by v.Wine_VinN_ID

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
                            Int32 vinId = rdr.GetInt32(0);

                            VinN vinN = new VinN
                            {
                                id = vinId,
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
                                workflow = rdr.GetFieldValue<Int16>(11)

                            };
                            vinN.wines = new List<WineN>();

                            dict.Add(vinId, vinN);


                        }

                        if (rdr.NextResult())
                        {
                            while (rdr.Read())
                            {
                                Int32 vinId = rdr.GetInt32(0);
                                var vinN = dict[vinId];

                                if (vinN != null)
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

            if (!String.IsNullOrEmpty(country))
            {
                sb.Append(" COALESCE(country, '')  ");
            }
            else
            {
                sb.Append(" '' as country  ");
            }

            if (!String.IsNullOrEmpty(region))
            {
                sb.Append(" ,COALESCE(region, '')  ");
            }
            else
            {
                sb.Append(" ,'' as region  ");
            }

            if (!String.IsNullOrEmpty(location))
            {
                sb.Append(" ,COALESCE(location, '')  ");
            }
            else
            {
                sb.Append(" ,'' as location  ");
            }
            if (!String.IsNullOrEmpty(locale))
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
                    if (!String.IsNullOrEmpty(location))
                    {
                        cmd.Parameters.AddWithValue("@filterLocale", Utils.BuildSearchString(locale));
                    }
                    if (!String.IsNullOrEmpty(site))
                    {
                        cmd.Parameters.AddWithValue("@filterSite", Utils.BuildSearchString(locale));
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




        public IEnumerable<VinN> Search(VinN filter)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
    }

}