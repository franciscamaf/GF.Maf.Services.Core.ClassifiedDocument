using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseFisaClassified
    {
        public RspMessage RspMessage { get; set; }
    }

    public class SourceError
    {
        public string code { get; set; }
        public string description { get; set; }
    }

    public class Result
    {
        public SourceError SourceError { get; set; }
        public string description { get; set; }
        public string status { get; set; }
    }

    public class RspHeader
    {
        public Info Info { get; set; }
        public Result Result { get; set; }
    }

    public class RspMessage
    {
        public RspHeader RspHeader { get; set; }
    }
}
