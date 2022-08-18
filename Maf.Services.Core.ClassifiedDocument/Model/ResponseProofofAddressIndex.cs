using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseProofofAddressIndex
    {
        public int id { get; set; }
        [JsonProperty("document_type")]
        public string documentType { get; set; }
        [JsonProperty("id_solicitud")]
        public string requestID { get; set; }
        [JsonProperty("proveedor_servicio_id")]
        public int serviceProviderId { get; set; }
        [JsonProperty("nro_cliente")]
        public string clientNumber { get; set; }
        [JsonProperty("nombre_cliente")]
        public string clientName { get; set; }
        [JsonProperty("direccion_cliente")]
        public string clientAbbress { get; set; }
        [JsonProperty("comuna_cliente")]
        public string clientComune { get; set; }
        [JsonProperty("monto")]
        public int amount { get; set; }
        [JsonProperty("fecha_emision")]
        public string issueDate { get; set; }
        public string workitemid { get; set; }
        public string validation { get; set; }
        public Status Status { get; set; }
        [JsonProperty("proveedor_servicio")]
        public ServiceProvider serviceProvider { get; set; }
    }
}
