using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ServiceProvider
    {
        public int id { get; set; }
        [JsonProperty("nombre_empresa")]
        public string companyName { get; set; }
        [JsonProperty("nombre_fantasia")]
        public string fantasyName { get; set; }
        [JsonProperty("rut")]
        public string companyRut { get; set; }
        [JsonProperty("direccion")]
        public string address { get; set; }
        [JsonProperty("ciudad")]
        public string city { get; set; }
    }
}
