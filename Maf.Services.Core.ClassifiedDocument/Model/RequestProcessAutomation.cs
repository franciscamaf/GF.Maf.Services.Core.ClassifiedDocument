using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class RequestProcessAutomation
    {
        public int DocumentType { get; set; }
        public int MethodType { get; set; }
        public string Request { get; set; }
    }
}
