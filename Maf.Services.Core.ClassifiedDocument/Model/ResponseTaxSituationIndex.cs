using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseTaxSituationIndex
    {
        public int id { get; set; }
        [JsonProperty("document_type")]
        public string documentType { get; set; }
        [JsonProperty("id_solicitud")]
        public string requestID { get; set; }
        [JsonProperty("rut_contribuyente")]
        public string taxpayerRut { get; set; }
        public string workitemid { get; set; }
        public string validation { get; set; }
        public Status Status { get; set; }
    }
}
