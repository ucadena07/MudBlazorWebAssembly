using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudTemplate.Shared.Utilities
{
    public static class SD
    {
        public enum ApiType
        {
            GET, POST
        }


        public static string BaseUrl;
        public static string TOKENKEY = "ed15e2c2-0934-4b6b-bb5e-10e0dbc43c2c";
        public static string REFRESHTOKENKEY = "ed1565e2c2-0934-4b6b-bb5e-10e0545dbc43c2c";
        public static string EXPTOKENKEY = "61ae0e2a-40cc-46ce-bd73-d041c491ac2d";
    }

}
