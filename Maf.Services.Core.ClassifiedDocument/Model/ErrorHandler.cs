using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ErrorHandler
    {
        private int errorCode;
        private string messageError;

        public ErrorHandler(int errorCodeParam, string errorDescriptionParam)
        {
            this.errorCode = errorCodeParam;
            this.messageError = errorDescriptionParam;
        }

        public ErrorHandler()
        {

        }

        /// <summary>
        /// Get Error Code return for Stored Procedure or Service
        /// </summary>
        public int ErrorCode
        {
            get
            {
                return this.errorCode;
            }
            set
            {
                this.errorCode = value;
            }
        }

        /// <summary>
        /// Get or Set Message error gived by the data source 
        /// </summary>
        public string MessageError
        {
            get
            {
                return this.messageError;
            }
            set
            {
                this.messageError = value;
            }
        }
    }
}
