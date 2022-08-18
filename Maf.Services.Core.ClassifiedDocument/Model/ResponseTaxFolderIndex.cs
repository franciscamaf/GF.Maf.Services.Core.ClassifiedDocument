using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseTaxFolderIndex
    {
        public int id { get; set; }
        [JsonProperty("document_type")]
        public string DocumentType { get; set; }
        [JsonProperty("file_name")]
        public string FileName { get; set; }
        public string Ext { get; set; }
        [JsonProperty("id_solicitud")]
        public string RequestID { get; set; }
        public string Workitemid { get; set; }
        public string Validation { get; set; }
        [JsonProperty("comentario")]
        public string Commentary { get; set; }
        [JsonProperty("rut")]
        public string ClientRut { get; set; }
        [JsonProperty("folio_F22")]
        public string FolioF22 { get; set; }
        [JsonProperty("folio_F29")]
        public string FolioF29 { get; set; }
        public Status Status { get; set; }
    }
}
