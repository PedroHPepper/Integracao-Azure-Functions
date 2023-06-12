using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pepper.DynamicsService.Utils
{
    public class StringUtils
    {
        public static string[] formatName(string fullname)
        {
            string[] fullnameArray = fullname.Split(' ');

            string firstname = "";
            string lastname = "";
            for (int i = 0; i < fullnameArray.Length; i++)
            {
                if (i == 0)
                {
                    firstname += fullnameArray[i];
                }
                else if (fullnameArray.Length > 2 && i == 1)
                {
                    firstname += " " + fullnameArray[i];
                }
                else if ((fullnameArray.Length == 2 && i == 1) || (fullnameArray.Length > 2 && i == 2))
                {
                    lastname += fullnameArray[i];
                }
                else
                {
                    lastname += " " + fullnameArray[i];
                }
            }
            return new string[] { firstname, lastname };
        }
    }
}
