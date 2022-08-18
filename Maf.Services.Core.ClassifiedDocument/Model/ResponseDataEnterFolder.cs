using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseDataEnterFolder
    {
        private ErrorHandler serviceStatus;
        public ErrorHandler ServiceStatus
        {
            get
            {
                return this.serviceStatus;
            }
            set
            {
                this.serviceStatus = value;
            }
        }
    }
}
