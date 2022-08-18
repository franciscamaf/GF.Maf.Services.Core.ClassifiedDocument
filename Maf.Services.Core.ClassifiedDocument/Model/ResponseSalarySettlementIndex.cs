using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseSalarySettlementIndex
    {
        public int id { get; set; }
        [JsonProperty("document_type")]
        public string documentType { get; set; }
        [JsonProperty("rut")]
        public string clientRut { get; set; }
        public string afp { get; set; }
        [JsonProperty("mes")]
        public string month { get; set; }
        [JsonProperty("anio")]
        public int year { get; set; }
        [JsonProperty("impuesto")]
        public int tax { get; set; }
        [JsonProperty("monto_bruto")]
        public int grossAmount { get; set; }
        public int apv { get; set; }
        [JsonProperty("ajustes")]
        public int adjustments { get; set; }
        [JsonProperty("prevision")]
        public string medicalInsurance { get; set; }
        [JsonProperty("monto_salud_1")]
        public int healthAmount1 { get; set; }
        [JsonProperty("monto_salud_2")]
        public int healthAmount2 { get; set; }
        [JsonProperty("exento_seguro_cesantia")]
        public string exemptSeveranceInsurance { get; set; }
        [JsonProperty("seguro_cesantia")]
        public int severanceInsurance { get; set; }
        [JsonProperty("id_solicitud")]
        public string requestID { get; set; }
        [JsonProperty("rut_contribuyente")]
        public string taxpayerRut { get; set; }
        public string workitemid { get; set; }
        public string validation { get; set; }
        public Status Status { get; set; }
    }
}
