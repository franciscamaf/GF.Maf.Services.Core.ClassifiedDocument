using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseServiceStatus
    {
        public int errorCode { get; set; }
        public string messageError { get; set; }
    }
}
