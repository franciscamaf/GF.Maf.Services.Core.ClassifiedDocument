using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class Constant
    {
        #region Varibles
        public const string ClassificationId = "ClassificationId";
        public const string SetClassified = "SetClassified";
        public const string ErrorCode = "ErrorCode";
        public const string ErrorMessage = "ErrorMessage";
        public const int NegativeOne = -1;
        public const int Zero = 0;
        public const int One = 1;
        public const int Two = 2;
        public const int Three = 3;
        public const int Four = 4;
        public const int Five = 5;
        public const int Six = 6;
        public const int Seven = 7;
        public const int Eight = 8;
        public const int Nine = 9;
        public const int Ten = 10;
        public const int CodErrorOK = 0;
        public const int MiloneThousandFifty = 1050;
        public const int Fifty = 50;
        public const int ninetyNine = 99;
        public const int Eighteen = 18;

        public const string Pipe = "|";
        public const string enterpriseCode = "MAF";
        public const string sysCode = "SISCRED";
        public const string stringZero = "0";
        public const string stringOne = "1";
        public const string stringTwo = "2";
        public const string stringThree = "3";
        public const string stringNinetyNine = "99";
        public const string conversationId = "59491c7e-ad88-49ec-a2ad-99ddcb1f5674";
        public const string validated = "S";
        public const string NotValidated = "N";
        public const string NoAplica = "NO APLICA";

        public const string StrWorkItemIdOriginal = "@workItemIdOriginal";
        public const string StrNewWorkItemId = "@newWorkItemId";
        public const string StrClasificationId = "@classificationId";
        public const string StrClasificationStatus = "@classificationStatus";
        public const string StrFileUploadName = "@fileUploadName";
        public const string StrTcgJson = "@tcgJson";
        public const string StrFisaClassifiedRequest = "@fisaClassifiedRequest";
        public const string StrFisaClassifiedResponse = "@fisaClassifiedResponse";
        public const string StrFisaValidatedRequest = "@fisaValidatedRequest";
        public const string StrFisaValidatedResponse = "@fisaValidatedResponse";
        public const string StrIdRole = "@idRole";
        public const string IdDictionary = "@IdDictionary";
        public const string InternalValue = "@InternalValue";
        public const string IdValue = "@IdValue";
        public const string IdValueFK = "@IdValueFK";
        public const string IdRequest = "@idRequest";
        public const string Workitemid = "@workitemid";
        public const string DocType = "@documentType";
        public const string DocName = "@documentName";
        public const string FileB64 = "@fileB64";
        public const string StrIdRequest = "idRequest";
        public const string StrWorkitemid = "workitemid";
        public const string StrDocType = "documentType";
        public const string StrDocname = "documentName";
        public const string StrFileB64 = "fileB64";
        public const string Year = "@Year";
        public const string PDF = "PDF";

        public const string IdValuesp = "IdValue";
        public const string InternalValuesp = "InternalValue";
        public const string NameValuesp = "NameValue";

        public const string SpSetClassifiedDocument = "DPS.spSetClassifiedDocument";
        public const string SpUpdateClassifiedDocument = "DPS.spUpdateClassifiedDocument";
        public const string SpSetLiftFile = "DPS.spSetLiftFile";
        public const string SpGetTypeDocumentParity = "DPS.spGetTypeDocumentParity";

        public const string spGetDictionaryData = "Dictionary.spGetDictionaryData";
        public const string Spgetalertconfig = "spgetalertconfig";
        public const string nationalityChilean = "CHILENA";

        public const string TypeText = "Sin Información en el documento";
        public const string TypeDate = "1900-01-01";
        public const int TypeMount = 0;
        #endregion

        #region Enums
        public enum ConnectionType
        {
            IntegrationLog = 1,
            IntegrationBusiness = 2,
            GeneralInformation = 3
        }

        public enum DocumentType
        {
            //ROCKETBOT
            CarpetaTributaria = 167,
            BoletaHonorarios = 224,
            cav = 14,
            CupoTaxi = 15,
            SituacionTributaria = 118,
            AFP = 6,
            LiquidacionCarabineros = 666666,//DUDA
            LiquidacionPension = 160,
            LiquidacionSueldo = 159,
            ComprobanteDomicilio = 3,

            //DPS
            Pagare = 22,
            Mandato = 23,
            CAV = 00000000,//DUDA
            SolicitudPrimeraInscripcion = 31,//duda
            ContratodeCredito = 37,
            Factura = 30
        }

        public enum NationalityType
        {
            Chilena = 1,
            Extranjera = 2,
            NoDefinido = 99
        }
        #endregion
    }
}
