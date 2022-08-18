using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class LiftFile
    {
        public int idRequest { get; set; }
        public int workItemId { get; set; }
        public int documentType { get; set; }
        public string documentName { get; set; }
        public string fileB64 { get; set; }
    }
}
