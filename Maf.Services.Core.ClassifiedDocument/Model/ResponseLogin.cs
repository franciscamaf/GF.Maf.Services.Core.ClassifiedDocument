using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseLogin
    {
        public string token { get; set; }
        public string expiration { get; set; }
        public ResponseServiceStatus serviceStatus { get; set; }
    }
}
