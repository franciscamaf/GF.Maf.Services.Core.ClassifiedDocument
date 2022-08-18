using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class ResponseProcessAutomation
    {
        
        #region Tax Folder / Carpeta Tributaria
        public ResponseTaxFolderIndex responseTaxFolderShowId { get; set; }
        public List<ResponseTaxFolderIndex> responseTaxFolderShowRequestId { get; set; }
        public ResponseTaxFolderShowB64 responseTaxFolderShowB64 { get; set; }
        #endregion

        #region Fee Ticket / Boleta Honorarios
        public List<ResponseFeeTicketIndex> responseFeeTicketShowRequestId { get; set; }
        #endregion

        #region CAV
        public List<ResponseCavIndex> responseCavShowRequestId { get; set; }
        #endregion

        #region CabQuota / Cupo Taxi
        public List<ResponseCabQuotaIndex> responseCabQuotaShowRequestId { get; set; }
        #endregion

        #region TaxSituation / Situacion Tributaria
        public List<ResponseTaxSituationIndex> responseTaxSituationShowRequestId { get; set; }
        #endregion

        #region AFP
        public List<ResponseAfpIndex> responseAfpShowRequestId { get; set; }
        #endregion

        #region CarabinerosSettlement
        public List<ResponseCarabinerosSettlementIndex> responseCarabinerosSettlementShowRequestId { get; set; }
        public ResponseCarabinerosSettlementShowB64 responseCarabinerosSettlementShowB64 { get; set; }
        #endregion

        #region PensionSettlement / Liquidacion Pension
        public List<ResponsePensionSettlementIndex> responsePensionSettlementShowRequestId { get; set; }
        #endregion

        #region SalarySettlement / Liquidacion Sueldo
        public List<ResponseSalarySettlementIndex> responseSalarySettlementShowRequestId { get; set; }
        #endregion

        #region ProofofAddress / Comprobante Domicilio
        public List<ResponseProofofAddressIndex> responseProofofAddressShowRequestId { get; set; }
        #endregion



        //CLASES DPS
        #region Promissory Note/Pagaré
        //public List<ResponsePromissoryNoteIndex> responsePromissoryNoteId { get; set; } //ShowRequestId
        public List<ResponsePromissoryNoteIndex> responsePromissoryNoteShowRequestId { get; set; } //ShowRequestId
        #endregion

        #region Contract of Mandate/Mandato
        public List<ResponseContractMandateIndex> responseContractMandateShowRequestId { get; set; }
        #endregion

        #region Credit Contract/Contrato de Credito
        public List<ResponseCreditContractIndex> responseCreditContractShowRequestId { get; set; }
        #endregion

        #region first registration/Solicitud Primera Inscripción
        public List<ResponseFirstRegistrationIndex> responseFirstRegistrationShowRequestId { get; set; }
        #endregion

        #region Invoice/Factura
        public List<ResponseInvoiceIndex> responseInvoiceShowRequestId { get; set; }
        #endregion

        #region CAV
        public List<ResponseCAVTCGIndex> responseCAVTCGShowRequestId { get; set; }
        #endregion


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
