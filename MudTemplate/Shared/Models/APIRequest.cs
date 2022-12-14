using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static MudTemplate.Shared.Utilities.SD;

namespace MudTemplate.Shared.Models
{
    public class APIRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public bool CheckTokens { get; set; } = true;
        public string Url { get; set; }
        public string Token { get; set; }
        public object Data { get; set; }
    }
}
