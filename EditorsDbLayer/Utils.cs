using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorsDbLayer
{
    internal class Utils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string BuildSearchString(string str)
        {
            var result = "";

            if (String.IsNullOrEmpty(str))
            {
                return result;
            }

            var search_parts = str.Split(new char[] { ' ' });

            for (int i = search_parts.Length - 1; i >= 0; i--)
            {

                search_parts[i] = search_parts[i].Replace('"', ' ');
                search_parts[i] = search_parts[i].Replace('*', ' ');
                search_parts[i] = search_parts[i].Trim();

                if (string.IsNullOrEmpty(search_parts[i]))
                    continue;

                if (search_parts[i].CompareTo(",") == 0)
                    continue;

                if (search_parts[i].CompareTo("&") == 0)
                    continue;


                if (string.IsNullOrEmpty(result))
                {
                        result = "\"" + search_parts[i] + "*\"";
                }
                else
                {
                        result = "\"" + search_parts[i] + "\" near " + result;
                }

            }

            return result;

        }


    }
}
