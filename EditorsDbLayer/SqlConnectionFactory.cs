using EditorsCommon;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace EditorsDbLayer
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlConnectionFactory : ISqlConnectionFactory
    {

        public SqlConnectionFactory()
        {
        }

        public String ConnectionStingConfig
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public System.Data.SqlClient.SqlConnection GetConnection()
        {

           SqlConnection  conn = new SqlConnection();
           
            //conn.ConnectionString = ConfigurationManager.ConnectionStrings["EditorsDataConnectionString"].ConnectionString;

           var configName = "WineDataConnectionString";

            if( !String.IsNullOrEmpty( ConnectionStingConfig) )
            {
                configName = ConnectionStingConfig;
            }


            conn.ConnectionString = ConfigurationManager.ConnectionStrings[configName].ConnectionString;

            conn.Open();

            return conn;
        }
    }
}
