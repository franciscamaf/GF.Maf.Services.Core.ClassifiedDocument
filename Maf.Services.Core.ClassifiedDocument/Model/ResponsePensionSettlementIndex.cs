using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponsePensionSettlementIndex
    {
        public int id { get; set; }
        [JsonProperty("document_type")]
        public string documentType { get; set; }
        [JsonProperty("id_solicitud")]
        public string requestID { get; set; }
        [JsonProperty("codigo_validacion")]
        public string validationCode { get; set; }
        [JsonProperty("nombre_cliente")]
        public string clientName { get; set; }
        [JsonProperty("rut_cliente")]
        public string clientRut { get; set; }
        [JsonProperty("periodo")]
        public string period { get; set; }
        [JsonProperty("mes")]
        public string month { get; set; }
        [JsonProperty("subtotal_haberes")]
        public int subTotalCredit { get; set; }
        [JsonProperty("subtotal_descuentos")]
        public int subTotalDiscounts { get; set; }
        [JsonProperty("total_neto")]
        public int netTotal { get; set; }
        [JsonProperty("rut_contribuyente")]
        public int taxpayerRut { get; set; }
        public string workItemId { get; set; }
        public string validation { get; set; }
        public Status Status { get; set; }
    }
}
