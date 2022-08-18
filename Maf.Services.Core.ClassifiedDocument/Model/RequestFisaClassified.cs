using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class RequestFisaClassified
    {
        public Envelop Envelop { get; set; }
        public ReqMessage ReqMessage { get; set; }
    }
    public class DocumentInformation
    {
        public int classificationGroupID { get; set; }
        public int classificationGroupIDTCG { get; set; }
        public string documentBase64 { get; set; }
        public string documentExtension { get; set; }
        public string documentName { get; set; }
        public int documentSize { get; set; }
        public int documentType { get; set; }
        public int omniaDocumentID { get; set; }
    }

    public class Envelop
    {
        public DocumentInformation documentInformation { get; set; }
        public string processDate { get; set; }
        public string requestId { get; set; }
    }

    public class Consumer
    {
        public string enterpriseCode { get; set; }
        public string sysCode { get; set; }
    }

    public class Trace
    {
        public int branchId { get; set; }
        public string carDealerId { get; set; }
        public string channelId { get; set; }
        public string conversationId { get; set; }
        public int officeId { get; set; }
        public int sellerId { get; set; }
        public int userId { get; set; }
    }

    public class Info
    {
        public Consumer Consumer { get; set; }
        public Trace Trace { get; set; }
    }

    public class ReqHeader
    {
        public Info Info { get; set; }
    }

    public class ReqMessage
    {
        public ReqHeader ReqHeader { get; set; }
    }
}
