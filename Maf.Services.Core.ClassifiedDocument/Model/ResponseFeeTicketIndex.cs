using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseFeeTicketIndex
    {
        public int id { get; set; }
        [JsonProperty("document_type")]
        public string documentType { get; set; }
        [JsonProperty("id_solicitud")]
        public string requestID { get; set; }
        public string barcode { get; set; }
        public string workItemId { get; set; }
        public string Validation { get; set; }
        [JsonProperty("rut")]
        public string clientRut { get; set; }
        [JsonProperty("mes")]
        public string month { get; set; }
        public Status Status { get; set; }
    }
}
