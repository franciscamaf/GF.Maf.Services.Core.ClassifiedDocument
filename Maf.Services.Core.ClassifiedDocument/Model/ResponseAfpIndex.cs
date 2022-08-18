using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseAfpIndex
    {
        public int id { get; set; }
        [JsonProperty("document_type")]
        public string documentType { get; set; }
        [JsonProperty("id_solicitud")]
        public string requestID { get; set; }
        [JsonProperty("afp_id")]
        public string afpId { get; set; }
        [JsonProperty("rut")]
        public string clientRut { get; set; }
        public string folio { get; set; }
        public string workitemid { get; set; }
        public string validation { get; set; }
        public Status status { get; set; }
        public AfpName afp { get; set; }
    }
}
