using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseCabQuotaIndex
    {
        public int id { get; set; }
        public string documentType { get; set; }
        public string requestID { get; set; }
        public string patent { get; set; }
        public string workitemid { get; set; }
        public string validation { get; set; }
        public Status Status { get; set; }
    }
}
