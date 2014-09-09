using System;
using System.Data.SqlClient;


namespace EditorsCommon
{
   public interface ISqlConnectionFactory
    {


        SqlConnection GetConnection();
    }
}
