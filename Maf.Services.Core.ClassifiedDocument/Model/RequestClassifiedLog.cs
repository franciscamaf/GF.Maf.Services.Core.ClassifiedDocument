using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class RequestClassifiedLog
    {
        public int classificationStatus { get; set; }
        public int workItemIdOriginal { get; set; }
        public int newWorkItemId { get; set; }
        public string fileName { get; set; }
        public int classificationId { get; set; }
        public FieldValueList fieldValueList { get; set; }

    }
}
