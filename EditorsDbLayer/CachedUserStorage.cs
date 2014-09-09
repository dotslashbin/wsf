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
    public class CachedUserStorage : ICachedUserStorage
    {

        ISqlConnectionFactory _connFactory;

        public CachedUserStorage(ISqlConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public CachedUser Create(CachedUser e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public IEnumerable<CachedUser> Search(CachedUser e)
        {
            List<CachedUser> result = new List<CachedUser>();

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"
                            SELECT  UserId
                                  ,UserName
                                  ,FirstName
                                  ,LastName
                                  ,IsAvailable
                                  ,created
                                  ,updated
                                  ,FullName
                              FROM Users";
 

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var item = new CachedUser
                            {
                                 userId = rdr.GetInt32(0),
                                 userName =  rdr.GetString(1),
                                 firstName = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                                 lastName = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                                 isAvailable = rdr.GetInt16(4) > 0, 
                                 fullName = rdr.IsDBNull(7) ? "" : rdr.GetString(7) 
                            };

                            result.Add(item);
                        }
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public CachedUser Update(CachedUser e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public CachedUser Delete(CachedUser e)
        {
            throw new NotImplementedException();
        }

        public CachedUser FindOrRegister(CachedUser user)
        {

            using (SqlConnection conn = _connFactory.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("", conn))
                {
                    cmd.CommandText = @"
        
                        if  not exists( select * from Users where UserId = @UserId)
                           insert into Users 
                             ( UserId, UserName, FirstName, LastName, IsAvailable ) 
                            values 
                             ( @UserId, @UserName, @FirstName, @LastName, @IsAvailable ) 


                          select UserId, UserName, FirstName, LastName, IsAvailable, FullName from Users where UserId = @UserId         
                         ";


                    cmd.Parameters.AddWithValue("@UserId", user.userId);
                    cmd.Parameters.AddWithValue("@UserName", user.userName);
                    cmd.Parameters.AddWithValue("@FirstName", user.firstName);
                    cmd.Parameters.AddWithValue("@LastName", user.lastName);
                    cmd.Parameters.AddWithValue("@IsAvailable", user.isAvailable);


                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            var item = new CachedUser
                            {
                                userId = rdr.GetInt32(0),
                                userName = rdr.GetString(1),
                                firstName = rdr.IsDBNull(2) ? "" : rdr.GetString(2),
                                lastName = rdr.IsDBNull(3) ? "" : rdr.GetString(3),
                                isAvailable = rdr.GetInt16(4) > 0,
                                fullName = rdr.IsDBNull(5) ? "" : rdr.GetString(5)
                            };

                            return item;
                        }
                    }
                    return null;
                }
            }

        }
    }

}
