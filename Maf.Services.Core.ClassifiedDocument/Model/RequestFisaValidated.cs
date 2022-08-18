using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class RequestFisaValidated
    {
        public EnvelopValidate Envelop { get; set; }
        public ReqMessage ReqMessage { get; set; }
    }
    public class AfpCertificate
    {
        public string folio { get; set; }
        public string id { get; set; }
        public string rut { get; set; }
    }

    public class BallotFees
    {
        public string barcode { get; set; }
        public string month { get; set; }
        public string rut { get; set; }
    }

    public class Cav
    {
        public string baningDate { get; set; }
        public string baningDocumentType { get; set; }
        public string baningNumber { get; set; }
        public string brand { get; set; }
        public string chassisNumber { get; set; }
        public string colour { get; set; }
        public string creditor { get; set; }
        public string engineNumber { get; set; }
        public string folio { get; set; }
        public string fullName { get; set; }
        public string inscription { get; set; }
        public string limitationDate { get; set; }
        public string limitationNumber { get; set; }
        public string model { get; set; }
        public string nroRolCivilBaning { get; set; }
        public string nroRolCivilLimitation { get; set; }
        public string patent { get; set; }
        public string repertory { get; set; }
        public string rut { get; set; }
        public string vehicleType { get; set; }
        public string verificationCode { get; set; }
        public int year { get; set; }
    }

    public class CreditContract
    {
        public string brand { get; set; }
        public string coDebtorName { get; set; }
        public string coDebtorRut { get; set; }
        public double comercialValue { get; set; }
        public int comercialYear { get; set; }
        public string contractType { get; set; }
        public string debterSign { get; set; }
        public string debtorName { get; set; }
        public string debtorRut { get; set; }
        public double doubleCapitalInsurance { get; set; }
        public string dueDate { get; set; }
        public string dueDay { get; set; }
        public string dueNumber { get; set; }
        public int dueValue { get; set; }
        public string externalServices { get; set; }
        public double gapRate { get; set; }
        public string legalCapacityDate { get; set; }
        public string legalCapacityNotary { get; set; }
        public double maintenanceExpenses { get; set; }
        public string model { get; set; }
        public double pie { get; set; }
        public string pledgeService { get; set; }
        public string representativeName { get; set; }
        public string representativeRut { get; set; }
        public double taxesAndStamps { get; set; }
        public string taxyClub { get; set; }
        public double total { get; set; }
        public string variableDueDay { get; set; }
        public double vehicleInsurance { get; set; }
        public double vehicleValue { get; set; }
    }

    public class FirstInscriptionRequest
    {
        public string acquirerName { get; set; }
        public string acquirerRut { get; set; }
        public string brand { get; set; }
        public string chassisNumber { get; set; }
        public string colour { get; set; }
        public string engineNumber { get; set; }
        public string invoiceNumber { get; set; }
        public string model { get; set; }
        public string patent { get; set; }
        public int year { get; set; }


    }

    public class Invoice
    {
        public string box { get; set; }
        public string brand { get; set; }
        public string buyForName { get; set; }
        public string buyForRut { get; set; }
        public string chassisNumber { get; set; }
        public string clientName { get; set; }
        public int comercialYear { get; set; }
        public string concessionaryRut { get; set; }
        public string engineNumber { get; set; }
        public string invoiceNumber { get; set; }
        public string model { get; set; }
        public double totalAmmount { get; set; }
    }

    public class LiquidationCarabineros
    {
        public string clientName { get; set; }
        public string familyBurden { get; set; }
        public string file { get; set; }
        public double legalDiscounts { get; set; }
        public int netAmount { get; set; }
        public string period { get; set; }
        public string rut { get; set; }
        public string settlementNumber { get; set; }
        public string settlementType { get; set; }
        public string tlementNumber { get; set; }
        public string tlementType { get; set; }
        public int totalCredit { get; set; }
    }

    public class LiquidationPensions
    {
        public string clientName { get; set; }
        public string codeValidation { get; set; }
        public string month { get; set; }
        public int netTotal { get; set; }
        public string period { get; set; }
        public int subtotalAssets { get; set; }
        public int subtotalDiscounts { get; set; }
    }

    public class MandatePledge
    {
        public string debtorSignDate { get; set; }
        public string debtorSignPlace { get; set; }
        public string mafRepresentaive { get; set; }
        public string ownedName { get; set; }
        public string ownedRut { get; set; }
        public string representativeName { get; set; }
        public string representativeRut { get; set; }
    }

    public class PromissoryNote
    {
        public int backFee { get; set; }
        public string coDebtorNameOne { get; set; }
        public string coDebtorNameTwo { get; set; }
        public string coDebtorNationalityOne { get; set; }
        public string coDebtorNationalityTwo { get; set; }
        public string coDebtorRutOne { get; set; }
        public string coDebtorRutTwo { get; set; }
        public string debtorName { get; set; }
        public string debtorNationality { get; set; }
        public string debtorRut { get; set; }
        public string debtorSign { get; set; }
        public string duesNumber { get; set; }
        public float interestRate { get; set; }
        public string letterAmmount { get; set; }
        public string noteNumber { get; set; }
        public double numberAmmount { get; set; }
        public string signDate { get; set; }
        public string signPlace { get; set; }
    }

    public class ProofOfAddress
    {
        public string address { get; set; }
        public string clientName { get; set; }
        public string commune { get; set; }
        public string companyName { get; set; }
        public string customerNumber { get; set; }
        public string tcgCode { get; set; }
    }

    public class Register
    {
        public string text { get; set; }
    }

    public class Salarysettlement
    {
        public string Month { get; set; }
        public int adjustment { get; set; }
        public string afp { get; set; }
        public string apv { get; set; }
        public string contractTYype { get; set; }
        public double exemptUnemploymentInsurance { get; set; }
        public int forecast { get; set; }
        public double grossAmount { get; set; }
        public double healthAmountOne { get; set; }
        public double healthAmountTwo { get; set; }
        public string rut { get; set; }
        public string settlementNumber { get; set; }
        public double taxs { get; set; }
        public string tlementNumber { get; set; }
        public double totalAssets { get; set; }
        public double totalTaxable { get; set; }
        public double totalTributable { get; set; }
        public double unemploymentInsurance { get; set; }
        public int year { get; set; }
    }

    public class TaxFolder
    {
        public string documentsTypedps { get; set; }
        public string file { get; set; }
        public string folioF22 { get; set; }
        public string folioF29 { get; set; }
        public string requestId { get; set; }
        public string rut { get; set; }
        public string workItemId { get; set; }
    }

    public class TaxSituation
    {
        public string rut { get; set; }
    }

    public class TaxiSpace
    {
        public string patent { get; set; }
    }

    public class DocumentValidate
    {
        public AfpCertificate afpCertificate { get; set; }
        public BallotFees ballotFees { get; set; }
        public Cav cav { get; set; }
        public CreditContract creditContract { get; set; }
        public FirstInscriptionRequest firstInscriptionRequest { get; set; }
        public Invoice invoice { get; set; }
        public LiquidationCarabineros liquidationCarabineros { get; set; }
        public LiquidationPensions liquidationPensions { get; set; }
        public MandatePledge mandatePledge { get; set; }
        public PromissoryNote promissoryNote { get; set; }
        public ProofOfAddress proofOfAddress { get; set; }
        public Register register { get; set; }
        public Salarysettlement salarysettlement { get; set; }
        public TaxFolder taxFolder { get; set; }
        public TaxSituation taxSituation { get; set; }
        public TaxiSpace taxiSpace { get; set; }
    }

    public class DocumentInformationValidated
    {
        public int documentIDTCG { get; set; }
        public DocumentValidate documentInformation { get; set; }
        public int documentType { get; set; }
        public string message { get; set; }
        public int omniaDocumentID { get; set; }
        public string validated { get; set; }
    }

    public class EnvelopValidate
    {
        public DocumentInformationValidated documentInformationValidated { get; set; }
        public string processDate { get; set; }
        public string requestId { get; set; }
    }
}
