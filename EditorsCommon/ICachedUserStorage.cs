using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsCommon
{

    public class CachedUser {

        /// <summary>
        /// 
        /// </summary>
        public int userId;
        /// <summary>
        /// 
        /// </summary>
        public string userName;
        /// <summary>
        /// 
        /// </summary>
        public string firstName;
        /// <summary>
        /// 
        /// </summary>
        public string lastName;
        /// <summary>
        /// 
        /// </summary>
        public bool isAvailable;
        /// <summary>
        /// 
        /// </summary>
        public string fullName;

    }

    public interface ICachedUserStorage : IStorage<CachedUser>
    {
        CachedUser FindOrRegister(CachedUser user); 
    }
}
