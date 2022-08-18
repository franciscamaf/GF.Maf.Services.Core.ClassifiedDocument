using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseClassified
    {
        public bool response { get; set; }
        public ErrorHandler serviceStatus { get; set; }
    }
}
