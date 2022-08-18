using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class RequestEnterFolder
    {
        public string OperationCode { get; set; }
        public string Dealer { get; set; }
        public string DealerRegionCode { get; set; }
        public string ClientRut { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public List<Document> Documents { get; set; }
    }

    public class Document
    {
        public string Base64 { get; set; }
        public string FileExtension { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
    }
}
