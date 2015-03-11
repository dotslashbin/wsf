using EditorsCommon;
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
    public class WineImporterStorage : IImporterStorage
    {

       ISqlConnectionFactory _connFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connFactory"></param>
       public WineImporterStorage(ISqlConnectionFactory connFactory)
       {
            _connFactory = connFactory;
        }



        //public void AddLinkToProducer(int producerId)
        //{
        //    throw new NotImplementedException();
        //}

        //public void RemoveLinkToProducer(int producerId)
        //{
        //    throw new NotImplementedException();
        //}

        public IEnumerable<WineImporterItem> GetLinksToProducer(int producerId)
        {
            throw new NotImplementedException();
        }

        public WineImporterItem Create(WineImporterItem e)
        {
            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {

                    cmd.CommandText = @"
     set nocount on;

     insert into WineImporter 
     ([Name],[Address],[Phone1],[Phone2],[Fax],[Email],[URL],[Notes])
     values
     (@Name,@Address,@Phone1,@Phone2,@Fax,@Email,@URL,@Notes);

     select id = scope_identity();


";

                    cmd.Parameters.AddWithValue("@Name", e.name);
                    cmd.Parameters.AddWithValue("@Address", String.IsNullOrEmpty(e.address) ? "" : e.address);
                    cmd.Parameters.AddWithValue("@Phone1", String.IsNullOrEmpty(e.phone1) ? "" : e.phone1);
                    cmd.Parameters.AddWithValue("@Phone2", String.IsNullOrEmpty(e.phone2) ? "" : e.phone2);
                    cmd.Parameters.AddWithValue("@Fax", String.IsNullOrEmpty(e.fax) ? "" : e.fax);
                    cmd.Parameters.AddWithValue("@Email", String.IsNullOrEmpty(e.email) ? "" : e.email);
                    cmd.Parameters.AddWithValue("@Url", String.IsNullOrEmpty(e.url) ? "" : e.url);
                    cmd.Parameters.AddWithValue("@Notes", String.IsNullOrEmpty(e.notes) ? "" : e.notes);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            e.id = (int)dr.GetDecimal(0);
                        }
                    } 
                } 
            } 

            return e;
        }

        public IEnumerable<WineImporterItem> Search(WineImporterItem e)
        {

            List<WineImporterItem> res = new List<WineImporterItem>();

            if (String.IsNullOrEmpty(e.name))
                return res;

            String searchString = "%" + e.name.Trim().Replace("%", "") + "%";



            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"
            select top(300)
	            ID = wp.ID, 
	            Name = wp.Name, 
                Address = wp.Address,
                Phone1  = wp.Phone1,
                Phone2  = wp.Phone2,
                Fax     = wp.Fax,
                Email   = wp.Email,
                URL     = wp.URL,
                Notes   = wp.Notes,
	            SortOrder = case when wp.Name like right(@SearchString, len(@SearchString)-1) then 0 else 20 end,
                linkCount = (select count(*) from WineProducer_WineImporter where ImporterId = wp.ID)

            from WineImporter wp (nolock)
            where wp.Name like @SearchString
            order by SortOrder, Name, ID
";


                    cmd.Parameters.AddWithValue("@SearchString", searchString);


                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            WineImporterItem item = new WineImporterItem();
                            item.id = dr.GetInt32(0);
                            item.name = dr.GetString(1);
                            item.address = (dr.IsDBNull(2) ? "" : dr.GetString(2));
                            item.phone1 = (dr.IsDBNull(3) ? "" : dr.GetString(3));
                            item.phone2 = (dr.IsDBNull(4) ? "" : dr.GetString(4));
                            item.fax = (dr.IsDBNull(5) ? "" : dr.GetString(5));
                            item.email = (dr.IsDBNull(6) ? "" : dr.GetString(6));
                            item.url = (dr.IsDBNull(7) ? "" : dr.GetString(7));
                            item.notes = (dr.IsDBNull(8) ? "" : dr.GetString(8));

                            item.linkImportersCount = dr.GetInt32(10); 

                            res.Add(item);
                        }
                    }
                }
            }

            return res;
        }

        public WineImporterItem Update(WineImporterItem e)
        {
            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {

                    cmd.CommandText = @"
     update WineImporter set  
     [Name]   = @Name,
     [Address]= @Address,
     [Phone1] = @Phone1,
     [Phone2] = @Phone2,
     [Fax]    = @Fax,
     [Email]  = @Email,
     [URL]    = @URL,
     [notes]  = @Notes
     where
     ID = @ID
";


                    cmd.Parameters.AddWithValue("@Name", e.name);
                    cmd.Parameters.AddWithValue("@Address", String.IsNullOrEmpty(e.address) ? "" : e.address);
                    cmd.Parameters.AddWithValue("@Phone1", String.IsNullOrEmpty(e.phone1) ? "" : e.phone1);
                    cmd.Parameters.AddWithValue("@Phone2", String.IsNullOrEmpty(e.phone2) ? "" : e.phone2);
                    cmd.Parameters.AddWithValue("@Fax", String.IsNullOrEmpty(e.fax) ? "" : e.fax);
                    cmd.Parameters.AddWithValue("@Email", String.IsNullOrEmpty(e.email) ? "" : e.email);
                    cmd.Parameters.AddWithValue("@Url", String.IsNullOrEmpty(e.url) ? "" : e.url);
                    cmd.Parameters.AddWithValue("@Notes", String.IsNullOrEmpty(e.notes) ? "" : e.notes);
                    cmd.Parameters.AddWithValue("@ID", e.id);
                    
                    
                    
                    cmd.ExecuteNonQuery();
                }
            }

            return e;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public WineImporterItem Delete(WineImporterItem e)
        {

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @" delete from  WineImporter  where ID = @ImporterId";
                    cmd.Parameters.AddWithValue("@ImporterId", e.id);

                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        return e;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<WineImporterItem> SearchByProducerId(int id)
        {
            List<WineImporterItem> res = new List<WineImporterItem>();

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"
            select 
	            ID = wp.ID, 
	            Name = wp.Name, 
                Address = wp.Address,
                Phone1  = wp.Phone1,
                Phone2  = wp.Phone2,
                Fax     = wp.Fax,
                Email   = wp.Email,
                URL     = wp.URL,
                Notes   = wp.Notes,
                linkCount = (select count(*) from WineProducer_WineImporter where ImporterId = wp.ID)

            from WineImporter wp (nolock)
            inner join  WineProducer_WineImporter pi on wp.ID =pi.ImporterId
            where pi.ProducerId = @ID
            order by Name
";

                    cmd.Parameters.AddWithValue("@ID", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            WineImporterItem item = new WineImporterItem();
                            item.id = dr.GetInt32(0);
                            item.name = dr.GetString(1);
                            item.address = (dr.IsDBNull(2) ? "" : dr.GetString(2));
                            item.phone1 = (dr.IsDBNull(3) ? "" : dr.GetString(3));
                            item.phone2 = (dr.IsDBNull(4) ? "" : dr.GetString(4));
                            item.fax = (dr.IsDBNull(5) ? "" : dr.GetString(5));
                            item.email = (dr.IsDBNull(6) ? "" : dr.GetString(6));
                            item.url = (dr.IsDBNull(7) ? "" : dr.GetString(7));
                            item.notes = (dr.IsDBNull(8) ? "" : dr.GetString(8));

                            item.linkImportersCount = dr.GetInt32(9); 

                            res.Add(item);
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importerId"></param>
        /// <param name="producerId"></param>
        /// <returns></returns>
        public WineImporterItem AddToProducer(int importerId, int producerId)
        {
            List<WineImporterItem> res = new List<WineImporterItem>();

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"

                    set nocount on;

                    insert into WineProducer_WineImporter 
                    ([ProducerId],[ImporterId]) values  (@ProducerId,@ImporterId);

                    select 
	                    ID = wp.ID, 
	                    Name = wp.Name, 
                        Address = wp.Address,
                        Phone1  = wp.Phone1,
                        Phone2  = wp.Phone2,
                        Fax     = wp.Fax,
                        Email   = wp.Email,
                        URL     = wp.URL,
                        Notes   = wp.Notes
                    from WineImporter wp (nolock)
                    where wp.ID = @ImporterId
";

                    cmd.Parameters.AddWithValue("@ImporterId", importerId);
                    cmd.Parameters.AddWithValue("@ProducerId", producerId);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            WineImporterItem item = new WineImporterItem();
                            item.id = dr.GetInt32(0);
                            item.name = dr.GetString(1);
                            item.address = (dr.IsDBNull(2) ? "" : dr.GetString(2));
                            item.phone1 = (dr.IsDBNull(3) ? "" : dr.GetString(3));
                            item.phone2 = (dr.IsDBNull(4) ? "" : dr.GetString(4));
                            item.fax = (dr.IsDBNull(5) ? "" : dr.GetString(5));
                            item.email = (dr.IsDBNull(6) ? "" : dr.GetString(6));
                            item.url = (dr.IsDBNull(7) ? "" : dr.GetString(7));
                            item.notes = (dr.IsDBNull(8) ? "" : dr.GetString(8));

                            return item;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importerId"></param>
        /// <param name="producerId"></param>
        /// <returns></returns>
        public WineImporterItem RemoveFromProducer(int importerId, int producerId)
        {
            List<WineImporterItem> res = new List<WineImporterItem>();

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"

                    set nocount on;

                    delete from WineProducer_WineImporter
                    where 
                    ProducerId = @ProducerId and ImporterId=  @ImporterId;

                    select 
	                    ID = wp.ID, 
	                    Name = wp.Name, 
                        Address = wp.Address,
                        Phone1  = wp.Phone1,
                        Phone2  = wp.Phone2,
                        Fax     = wp.Fax,
                        Email   = wp.Email,
                        URL     = wp.URL,
                        Notes   = wp.Notes
                    from WineImporter wp (nolock)
                    where wp.ID = @ImporterId
";

                    cmd.Parameters.AddWithValue("@ImporterId", importerId);
                    cmd.Parameters.AddWithValue("@ProducerId", producerId);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            WineImporterItem item = new WineImporterItem();
                            item.id = dr.GetInt32(0);
                            item.name = dr.GetString(1);
                            item.address = (dr.IsDBNull(2) ? "" : dr.GetString(2));
                            item.phone1 = (dr.IsDBNull(3) ? "" : dr.GetString(3));
                            item.phone2 = (dr.IsDBNull(4) ? "" : dr.GetString(4));
                            item.fax = (dr.IsDBNull(5) ? "" : dr.GetString(5));
                            item.email = (dr.IsDBNull(6) ? "" : dr.GetString(6));
                            item.url = (dr.IsDBNull(7) ? "" : dr.GetString(7));
                            item.notes = (dr.IsDBNull(8) ? "" : dr.GetString(8));

                            return item;
                        }
                    }
                }
            }

            return null;
        }


        public IEnumerable<WineProducer> GetLinksToImporter(int importerId)
        {
            List<WineProducer> res = new List<WineProducer>();

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"
            select 
	            ID = wp.ID, 
	            Name = wp.Name, 
	            NameToShow = wp.NameToShow,
	            WF_StatusID = wp.WF_StatusID,
            from WineProducer wp (nolock)
            join WineProducer_WineImporter as wpwi on wpwi.ProducerId = wp.ID 
                    ([ProducerId],[ImporterId]) values  (@ProducerId,@ImporterId);

            where wpwi.ImporterId = @ImporterId
";

                    cmd.Parameters.AddWithValue("@ImporterId", importerId);


                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            WineProducer item = new WineProducer();
                            item.id = dr.GetInt32(0);
                            item.name = dr.GetString(1);
                            item.nameToShow = dr.GetString(2);
                            item.wfState = (dr.IsDBNull(3) ? (short)0 : dr.GetInt16(3));

                            res.Add(item);
                        }
                    }
                }
            }

            return res;
        }
    }
}
