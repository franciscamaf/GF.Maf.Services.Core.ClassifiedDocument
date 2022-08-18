using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maf.Services.Core.ClassifiedDocument.Model;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseSendMail
    {
        public MailData mailData { get; set; }
        public ResponseServiceStatus serviceStatus { get; set; }
    }
}
