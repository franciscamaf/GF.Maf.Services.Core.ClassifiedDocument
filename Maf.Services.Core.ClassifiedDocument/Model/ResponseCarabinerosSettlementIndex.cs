using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseCarabinerosSettlementIndex
    {
        public int id { get; set; }
        [JsonProperty("document_type")]
        public string documentType { get; set; }
        [JsonProperty("file_name")]
        public string fileName { get; set; }
        public string ext { get; set; }
        [JsonProperty("nro_liquidacion")]
        public string settlementNumbre { get; set; }
        [JsonProperty("nombre_cliente")]
        public string clientName { get; set; }
        [JsonProperty("rut_cliente")]
        public string clientRut { get; set; }
        [JsonProperty("carga_familiar")]
        public string familyBurden { get; set; }
        [JsonProperty("total_haber")]
        public string totalCredit { get; set; }
        [JsonProperty("descuentos_legales")]
        public string legalDiscounts { get; set; }
        [JsonProperty("monto_liquido")]
        public string debtAmount { get; set; }
        [JsonProperty("periodo")]
        public string period { get; set; }
        [JsonProperty("id_solicitud")]
        public string requestID { get; set; }
        public string workitemid { get; set; }
        public string validation { get; set; }
        public Status status { get; set; }
    }
}
