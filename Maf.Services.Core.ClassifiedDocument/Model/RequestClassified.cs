using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class RequestClassified
    {
        public int classificationStatus { get; set; }
        public int workItemIdOriginal { get; set; }
        public int newWorkItemId { get; set; }

        private string fileNameField;

        private string base64FileField;

        [Required(ErrorMessage = "File Name is required")]
        public string fileName
        {
            get
            {
                return this.fileNameField;
            }
            set
            {
                this.fileNameField = value;
            }
        }

        [Required(ErrorMessage = "Base64 File is required")]
        public string base64File
        {
            get
            {
                return this.base64FileField;
            }
            set
            {
                this.base64FileField = value;
            }
        }
        public int classificationId { get; set; }

        public FieldValueList fieldValueList { get; set; }
    }
}
