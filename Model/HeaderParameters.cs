using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class HeaderParameters
    {
        public string version { get; set; }
        public string sessionid { get; set; }
        public string token { get; set; }
        public string sourceapp { get; set; }
        public string subapiname { get; set; }
        public string apiname { get; set; }
    }//HeaderParameters
}
