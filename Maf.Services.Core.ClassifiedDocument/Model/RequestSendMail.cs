using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class RequestSendMail
    {
        public MailData mailData { get; set; }
    }

    public class MailData
    {
        public List<string> to { get; set; }
        public List<string> cc { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public List<Attachments> attachments { get; set; }
    }

    public class Attachments
    {
        public string fileName { get; set; }
        public string base64 { get; set; }
    }
}
