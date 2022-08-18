using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maf.Services.Core.ClassifiedDocument.Model;
using Serilog.Formatting;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Serilog.Sinks.MSSqlServer;
using Newtonsoft.Json;
using RestSharp;

namespace Maf.Services.Core.ClassifiedDocument.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassifiedDocumentController : Controller
    {
        private static IConfiguration _iconfiguration;
        private readonly ILogger<ClassifiedDocumentController> _logger;
        private static string hostName = Dns.GetHostName();
        private static string FilePathDescrypt = string.Empty;

        public ClassifiedDocumentController(ILogger<ClassifiedDocumentController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ResponseClassified SetClassified(RequestClassified requestClassified)
        {
            string guardoLog;
            ResponseClassified respondeClassified = new ResponseClassified();
            RequestClassifiedLog requestClassifiedLog = new RequestClassifiedLog();

            ResponseProcessAutomation responseProcessAutomation = new ResponseProcessAutomation();
            DateTime startProcess = DateTime.Now;
            ResponseDataEnterFolder responseDataEnterFolder = new ResponseDataEnterFolder();

            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();

            ITextFormatter jsonFormatter = new Serilog.Formatting.Json.JsonFormatter(renderMessage: true);
            string fileLog = string.Concat(hostName, "_");
            fileLog = string.Concat(fileLog, _iconfiguration["Environment"]);
            fileLog = string.Concat(fileLog, _iconfiguration["FileName"]);
            FilePathDescrypt = _iconfiguration["LogFileDirectory"];
            string pathLog = string.Concat(FilePathDescrypt, fileLog);

            requestClassifiedLog.workItemIdOriginal = requestClassified.workItemIdOriginal;
            requestClassifiedLog.newWorkItemId = requestClassified.newWorkItemId;
            requestClassifiedLog.classificationId = requestClassified.classificationId;
            requestClassifiedLog.fileName = requestClassified.fileName;
            requestClassifiedLog.fieldValueList = requestClassified.fieldValueList;
     
            //requestClassifiedLog.base = requestClassified.base64File;

            var sqlLoggerOptions = new MSSqlServerSinkOptions
            {
                AutoCreateSqlTable = true,
                TableName = "Logs_ClassifiedDocument",
                BatchPostingLimit = 1
            };

            System.Data.SqlClient.SqlCommand comandoSql;
            SqlConnection conexionLog;
            SqlConnection conexionBusines;
            System.Data.SqlClient.SqlDataReader dataReader;

            conexionBusines = new Conection(_iconfiguration).SqlEntregaConexion(_iconfiguration, Constant.ConnectionType.IntegrationBusiness);
            conexionLog = new Conection(_iconfiguration).SqlEntregaConexion(_iconfiguration, Constant.ConnectionType.IntegrationLog);

            Log.Logger = new LoggerConfiguration()
              .WriteTo.MSSqlServer(conexionLog.ConnectionString, sqlLoggerOptions)
              .CreateLogger();

            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Library.Core.Validate.Model.ErrorHandler validaTokens = Library.Core.Validate.Token.ValidationToken(token, _iconfiguration);

            if (validaTokens.ErrorCode == Constant.CodErrorOK)
            {
                using (var logger = new LoggerConfiguration()
                .WriteTo.File(jsonFormatter,
                pathLog,
                Serilog.Events.LogEventLevel.Debug,
                rollingInterval: RollingInterval.Day)
                .WriteTo.MSSqlServer(conexionLog.ConnectionString, sqlLoggerOptions)
                .CreateLogger())
                {
                    logger.Information("INICIO PROCESO {ProcessName}", Constant.SetClassified);
                    //logger.Information("JSON DE ENTRADA {@Entry}", JsonConvert.SerializeObject(requestClassifiedLog));
                    logger.Information("JSON DE ENTRADA {@Entry}", requestClassified);
                    
                    try
                    {
                        conexionBusines.Open();
                        comandoSql = new SqlCommand(Constant.SpSetClassifiedDocument, conexionBusines);
                        comandoSql.CommandType = System.Data.CommandType.StoredProcedure;
                        comandoSql.Parameters.Add(new SqlParameter(Constant.StrWorkItemIdOriginal, requestClassified.workItemIdOriginal));
                        comandoSql.Parameters.Add(new SqlParameter(Constant.StrNewWorkItemId, requestClassified.newWorkItemId));
                        comandoSql.Parameters.Add(new SqlParameter(Constant.StrClasificationId, requestClassified.classificationId));
                        comandoSql.Parameters.Add(new SqlParameter(Constant.StrClasificationStatus, requestClassified.classificationStatus));
                        comandoSql.Parameters.Add(new SqlParameter(Constant.StrFileUploadName, requestClassified.fileName));
                        dataReader = comandoSql.ExecuteReader();
                        int convertCode;
                        ErrorHandler errorHandler = new ErrorHandler();

                        guardoLog = SaveRequest(requestClassified.workItemIdOriginal, requestClassified.newWorkItemId, JsonConvert.SerializeObject(requestClassifiedLog), "", "", "", "");

                        while (dataReader.Read())
                        {
                            if (int.TryParse(dataReader[Constant.ErrorCode].ToString(), out convertCode))
                            {
                                errorHandler.ErrorCode = convertCode;
                                errorHandler.MessageError = dataReader[Constant.ErrorMessage].ToString();
                            }
                            else
                            {
                                errorHandler.ErrorCode = Constant.NegativeOne;
                                errorHandler.MessageError = string.Empty;
                            }
                            respondeClassified.serviceStatus = errorHandler;
                            respondeClassified.response = true;
                        }
                        if (dataReader.NextResult())
                        {
                            while (dataReader.Read())
                            {
                                if (errorHandler.ErrorCode == Constant.Zero)
                                {
                                    var idRequest0 = int.Parse(dataReader["idRequest"].ToString());
                                    var idRequest = idRequest0.ToString("D20");
                                    var workitemid = requestClassified.workItemIdOriginal;
                                    var newworkitemid = requestClassified.newWorkItemId;
                                    var omniaId = int.Parse(dataReader["omniaId"].ToString());

                                    var documentTypeFisa = int.Parse(dataReader["documentType"].ToString());

                                    responseProcessAutomation = GetProcessAutomation(requestClassified, idRequest);

                                    ResponseFisaClassified responseFisaClassified = new ResponseFisaClassified();
                                    ResponseFisaValidated responseFisaValidated = new ResponseFisaValidated();

                                    if (requestClassified.classificationStatus == Constant.Zero)
                                    {
                                        if (documentTypeFisa == Constant.Zero)
                                        {
                                            responseFisaClassified = SetFisaClassified(requestClassified.classificationId, idRequest, workitemid, Constant.Zero, requestClassified.fileName, requestClassified.base64File, newworkitemid);
                                            responseFisaValidated = SetFisaValidated(responseProcessAutomation, idRequest, workitemid, Constant.Zero, requestClassified.classificationId, Constant.validated, newworkitemid);
                                        }
                                        else if (documentTypeFisa != requestClassified.classificationId)
                                        {
                                            responseFisaClassified = SetFisaClassified(requestClassified.classificationId, idRequest, workitemid, omniaId, requestClassified.fileName, requestClassified.base64File, newworkitemid);
                                            responseFisaValidated = SetFisaValidated(responseProcessAutomation, idRequest, workitemid, omniaId, requestClassified.classificationId, Constant.validated, newworkitemid);
                                        }
                                        else if (documentTypeFisa == requestClassified.classificationId)
                                        {
                                            responseFisaClassified = SetFisaClassified(requestClassified.classificationId, idRequest, workitemid, omniaId, requestClassified.fileName, requestClassified.base64File, newworkitemid);
                                            responseFisaValidated = SetFisaValidated(responseProcessAutomation, idRequest, workitemid, omniaId, requestClassified.classificationId, Constant.validated, newworkitemid);
                                        }

                                        if (responseFisaClassified.RspMessage.RspHeader.Result.status == "ERROR")
                                        {
                                            errorHandler.ErrorCode = Constant.NegativeOne;
                                            errorHandler.MessageError = responseFisaClassified.RspMessage.RspHeader.Result.status;
                                            respondeClassified.serviceStatus = errorHandler;
                                            respondeClassified.response = true;

                                            //ACA deberia haber alerta de TCG
                                        }
                                        var listDocumentDictionary = DocumentFileType.GetDictionaryData(Constant.Eighteen);

                                        List<int> DocumentsFileType = new List<int>();
                                        foreach (var responseGetDictionaryData in listDocumentDictionary)
                                        {
                                            DocumentsFileType.Add(Int32.Parse(responseGetDictionaryData.InternalValuesp));
                                        }


                                        if (DocumentsFileType.Contains(requestClassified.classificationId))
                                        {
                                            List<LiftFile> documentLiftFiles = new List<LiftFile>();
                                            documentLiftFiles = DocumentFileType.SetLiftFile(Int32.Parse(idRequest), workitemid, requestClassified.classificationId, requestClassified.fileName, requestClassified.base64File);

                                            if (DocumentsFileType.Except(documentLiftFiles.Select(o => o.documentType).Distinct().ToList()).ToList().Count() == 0)
                                            {
                                                responseDataEnterFolder = DocumentFileType.CallElectronicFolder(documentLiftFiles);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (requestClassified.classificationId != Constant.Zero)
                                        {
                                            if (documentTypeFisa == Constant.Zero)
                                            {
                                                responseFisaClassified = SetFisaClassified(requestClassified.classificationId, idRequest, workitemid, Constant.Zero, requestClassified.fileName, requestClassified.base64File, newworkitemid);
                                                responseFisaValidated = SetFisaValidated(responseProcessAutomation, idRequest, workitemid, Constant.Zero, requestClassified.classificationId,Constant.NotValidated, newworkitemid);
                                            }
                                            else if (documentTypeFisa != requestClassified.classificationId)
                                            {
                                                responseFisaClassified = SetFisaClassified(requestClassified.classificationId, idRequest, workitemid, omniaId, requestClassified.fileName, requestClassified.base64File, newworkitemid);
                                                responseFisaValidated = SetFisaValidated(responseProcessAutomation, idRequest, workitemid, omniaId, requestClassified.classificationId, Constant.NotValidated, newworkitemid);
                                            }
                                            else if (documentTypeFisa == requestClassified.classificationId)
                                            {
                                                responseFisaClassified = SetFisaClassified(requestClassified.classificationId, idRequest, workitemid, omniaId, requestClassified.fileName, requestClassified.base64File, newworkitemid);
                                                responseFisaValidated = SetFisaValidated(responseProcessAutomation, idRequest, workitemid, omniaId, requestClassified.classificationId, Constant.NotValidated, newworkitemid);
                                            }
                                        }
                                        else
                                        {
                                            GetAlertConfiguration(requestClassified, idRequest);
                                        }
                                    }
                                    logger.Information("JSON DE SALIDA {@Entry}", respondeClassified);
                                    logger.Information("FIN PROCESO {ProcessName}", Constant.SetClassified);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler exError = new ErrorHandler();
                        exError.MessageError = ex.Message;
                        exError.ErrorCode = ((System.Runtime.InteropServices.ExternalException)ex).ErrorCode;
                        TimeSpan timeTranscurrido = DateTime.Now - startProcess;
                        logger.Error("FIN PROCESO: ERROR {@Error}, TIEMPO TRASCURRIDO {Time} SEG", exError, timeTranscurrido.TotalSeconds);
                        respondeClassified.serviceStatus = exError;
                    }
                };
            }
            else
            {
                respondeClassified.serviceStatus = new ErrorHandler { ErrorCode = validaTokens.ErrorCode, MessageError = validaTokens.MessageError }; ;
                respondeClassified.response = false;
            }
            return respondeClassified;
        }
        private ResponseLogin AuthorizationServiceMAF()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();

            try
            {
                RequestLogin requestLogin = new RequestLogin
                {
                    username = _iconfiguration["usernametoken"],
                    password = _iconfiguration["passtoken"]
                };
                string urlLoginToken = _iconfiguration["UrlLoginToken"];

                var clientRest = new RestClient(urlLoginToken);
                var requestClient = new RestRequest(RestSharp.Method.POST);
                requestClient.AddJsonBody(requestLogin);
                var restResponse = clientRest.Execute(requestClient);
                var response = JsonConvert.DeserializeObject<ResponseLogin>(restResponse.Content.ToString());
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private ResponseFisaClassified SetFisaClassified(int classificationId, string requestId, int workitemid, int omniaId, string FileName, string base64File, int newworkitemid)
        {
            ITextFormatter jsonFormatter = new Serilog.Formatting.Json.JsonFormatter(renderMessage: true);
            string fileLog = string.Concat(hostName, "_");
            string guardoLog;
            fileLog = string.Concat(fileLog, _iconfiguration["Environment"]);
            fileLog = string.Concat(fileLog, _iconfiguration["FileName"]);
            FilePathDescrypt = _iconfiguration["LogFileDirectory"];
            string pathLog = string.Concat(FilePathDescrypt, fileLog);
            var sqlLoggerOptions = new MSSqlServerSinkOptions
            {
                AutoCreateSqlTable = true,
                TableName = "Logs_ClassifiedDocument",
                BatchPostingLimit = Constant.One
            };

            SqlConnection conexionLog;
            conexionLog = new Conection(_iconfiguration).SqlEntregaConexion(_iconfiguration, Constant.ConnectionType.IntegrationLog);

            RequestFisaClassified requestFisaClassified = new RequestFisaClassified();
            RequestFisaClassified requestFisaClassifiedLog = new RequestFisaClassified();
            ResponseFisaClassified responseFisaClassified = new ResponseFisaClassified();
            Envelop envelop = new Envelop();
            Envelop envelopLog = new Envelop();
            DocumentInformation documentInformation = new DocumentInformation();
            DocumentInformation documentInformationLog = new DocumentInformation();
            ReqMessage reqMessage = new ReqMessage();
            ReqMessage reqMessageLog = new ReqMessage();
            ReqHeader reqHeader = new ReqHeader();
            ReqHeader reqHeaderLog = new ReqHeader();
            Info info = new Info();
            Info infoLog = new Info();
            Consumer consumer = new Consumer();
            Consumer consumerLog = new Consumer();
            Trace trace = new Trace();
            Trace traceLog = new Trace();

            classificationId = GetTypeDocumentParity(classificationId);

            documentInformation.classificationGroupID = classificationId;
            documentInformation.classificationGroupIDTCG = workitemid;
            documentInformation.documentExtension = ".pdf";
            documentInformation.documentName = FileName;
            documentInformation.documentSize = Constant.Fifty;
            documentInformation.documentType = classificationId;
            documentInformation.omniaDocumentID = omniaId;
            documentInformation.documentBase64 = base64File;//ADD

            documentInformationLog.classificationGroupID = classificationId;
            documentInformationLog.classificationGroupIDTCG = workitemid;
            documentInformationLog.documentExtension = ".pdf";
            documentInformationLog.documentName = FileName;
            documentInformationLog.documentSize = Constant.Fifty;
            documentInformationLog.documentType = classificationId;
            documentInformationLog.omniaDocumentID = omniaId;

            envelop.documentInformation = documentInformation;
            envelop.processDate = System.DateTime.Now.ToString("yyyy-MM-dd");
            envelop.requestId = requestId.ToString();

            envelopLog.documentInformation = documentInformationLog;
            envelopLog.processDate = System.DateTime.Now.ToString("yyyy-MM-dd");
            envelopLog.requestId = requestId.ToString();

            requestFisaClassified.Envelop = envelop;
            requestFisaClassifiedLog.Envelop = envelopLog;

            consumer.enterpriseCode = Constant.enterpriseCode;
            consumer.sysCode = Constant.sysCode;
            consumerLog.enterpriseCode = Constant.enterpriseCode;
            consumerLog.sysCode = Constant.sysCode;

            trace.branchId = Constant.Two;
            trace.carDealerId = Constant.stringThree;
            trace.channelId = Constant.stringOne;
            trace.conversationId = Constant.conversationId;
            trace.officeId = Constant.One;
            trace.sellerId = Constant.MiloneThousandFifty;
            trace.userId = Constant.MiloneThousandFifty;

            traceLog.branchId = Constant.Two;
            traceLog.carDealerId = Constant.stringThree;
            traceLog.channelId = Constant.stringOne;
            traceLog.conversationId = Constant.conversationId;
            traceLog.officeId = Constant.One;
            traceLog.sellerId = Constant.MiloneThousandFifty;
            traceLog.userId = Constant.MiloneThousandFifty;

            info.Consumer = consumer;
            info.Trace = trace;

            infoLog.Consumer = consumerLog;
            infoLog.Trace = traceLog;

            reqHeader.Info = info;
            reqHeaderLog.Info = infoLog;

            reqMessage.ReqHeader = reqHeader;
            reqMessageLog.ReqHeader = reqHeaderLog;

            requestFisaClassified.ReqMessage = reqMessage;
            requestFisaClassifiedLog.ReqMessage = reqMessageLog;

            using (var logger = new LoggerConfiguration()
            .WriteTo.File(jsonFormatter,
            pathLog,
            Serilog.Events.LogEventLevel.Debug,
            rollingInterval: RollingInterval.Day)
            .WriteTo.MSSqlServer(conexionLog.ConnectionString, sqlLoggerOptions)
            .CreateLogger())
            {
                try
                {
                    //logger.Information("REQUEST FISA CLASIFICACION {@Entry}", JsonConvert.SerializeObject(requestFisaClassified));
                    var jsonVar = JsonConvert.SerializeObject(requestFisaClassified);

                    guardoLog = SaveRequest(workitemid, newworkitemid, "", JsonConvert.SerializeObject(requestFisaClassifiedLog), "", "", "");

                    logger.Information("REQUEST FISA CLASIFICACION {@Entry}", requestFisaClassified);
                    string urlFisaClassified = _iconfiguration["UrlFisaClassified"];
                    var clientRest = new RestClient(urlFisaClassified);
                    var requestClient = new RestRequest(RestSharp.Method.POST);
                    requestClient.AddHeader("cache-control", "no-cache");
                    requestClient.AddHeader("content-type", "application/json");
                    //requestClient.AddHeader("Authorization", "Bearer " + respOauth.token);
                    requestClient.AddJsonBody(requestFisaClassified);
                    var restResponse = clientRest.Execute(requestClient);
                    responseFisaClassified = JsonConvert.DeserializeObject<ResponseFisaClassified>(restResponse.Content.ToString());
                    //logger.Information("RESPONSE FISA CLASIFICACION {@Entry}", JsonConvert.SerializeObject(responseFisaClassified));

                    logger.Information("RESPONSE FISA CLASIFICACION {@Entry}", responseFisaClassified);
                    guardoLog = SaveRequest(workitemid, newworkitemid, "", "", JsonConvert.SerializeObject(responseFisaClassified), "", "");
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return responseFisaClassified;
        }
        private ResponseFisaValidated SetFisaValidated(ResponseProcessAutomation responseProcessAutomation, string requestId, int workitemid, int omniaId, int classificationId, string validation, int newworkitemid)
        {
            ITextFormatter jsonFormatter = new Serilog.Formatting.Json.JsonFormatter(renderMessage: true);
            string fileLog = string.Concat(hostName, "_");
            fileLog = string.Concat(fileLog, _iconfiguration["Environment"]);
            fileLog = string.Concat(fileLog, _iconfiguration["FileName"]);
            FilePathDescrypt = _iconfiguration["LogFileDirectory"];
            string pathLog = string.Concat(FilePathDescrypt, fileLog);
            var sqlLoggerOptions = new MSSqlServerSinkOptions
            {
                AutoCreateSqlTable = true,
                TableName = "Logs_ClassifiedDocument",
                BatchPostingLimit = 1
            };
            SqlConnection conexionLog;
            conexionLog = new Conection(_iconfiguration).SqlEntregaConexion(_iconfiguration, Constant.ConnectionType.IntegrationLog);

            classificationId = GetTypeDocumentParity(classificationId);

            //ResponseFileAdminGet responseFileAdminGet = new ResponseFileAdminGet();
            string guardoLog;
            RequestFisaValidated requestFisaValidated = new RequestFisaValidated();
            ResponseFisaValidated responseFisaValidated = new ResponseFisaValidated();
            EnvelopValidate envelop = new EnvelopValidate();
            DocumentInformationValidated documentInformation = new DocumentInformationValidated();
            DocumentValidate documentValidate = new DocumentValidate();
            ReqMessage reqMessage = new ReqMessage();
            ReqHeader reqHeader = new ReqHeader();
            Info info = new Info();
            Consumer consumer = new Consumer();
            Trace trace = new Trace();

            if (responseProcessAutomation.responseTaxFolderShowRequestId != null)
            {
                foreach (ResponseTaxFolderIndex taxFolderRocket in responseProcessAutomation.responseTaxFolderShowRequestId)
                {
                    TaxFolder taxFolder = new TaxFolder();
                    taxFolder.documentsTypedps = "";
                    taxFolder.file = "";
                    taxFolder.folioF22 = taxFolderRocket.FolioF22;
                    taxFolder.folioF29 = taxFolderRocket.FolioF29;
                    taxFolder.requestId = taxFolderRocket.RequestID;
                    taxFolder.rut = taxFolderRocket.ClientRut;
                    taxFolder.workItemId = taxFolderRocket.Workitemid;
                    documentValidate.taxFolder = taxFolder;
                }
            }
            if (responseProcessAutomation.responseFeeTicketShowRequestId != null)
            {
                foreach (ResponseFeeTicketIndex feeTicketRocket in responseProcessAutomation.responseFeeTicketShowRequestId)
                {
                    //FISA NO RECIBE ESTE DOCUMENTO
                }
            }
            if (responseProcessAutomation.responseCavShowRequestId != null)
            {
                foreach (ResponseCavIndex cavRocket in responseProcessAutomation.responseCavShowRequestId)
                {
                    Cav cav = new Cav();
                    cav.folio = cavRocket.folio;
                    cav.patent = cavRocket.patent;
                    documentValidate.cav = cav;
                }
            }
            if (responseProcessAutomation.responseCabQuotaShowRequestId != null)
            {
                foreach (ResponseCabQuotaIndex cabQuotaRocket in responseProcessAutomation.responseCabQuotaShowRequestId)
                {
                    //FISA NO RECIBE ESTE DOCUMENTO
                }
            }
            if (responseProcessAutomation.responseTaxSituationShowRequestId != null)
            {
                foreach (ResponseTaxSituationIndex taxSituationRocket in responseProcessAutomation.responseTaxSituationShowRequestId)
                {
                    TaxSituation taxSituation = new TaxSituation();
                    taxSituation.rut = taxSituationRocket.taxpayerRut;
                    documentValidate.taxSituation = taxSituation;
                }
            }
            if (responseProcessAutomation.responseAfpShowRequestId != null)
            {
                foreach (ResponseAfpIndex afpRocket in responseProcessAutomation.responseAfpShowRequestId)
                {
                    AfpCertificate afpCertificate = new AfpCertificate();
                    afpCertificate.rut = afpRocket.clientRut;
                    afpCertificate.folio = afpRocket.folio;
                    documentValidate.afpCertificate = afpCertificate;
                }
            }
            if (responseProcessAutomation.responseCarabinerosSettlementShowRequestId != null)
            {
                foreach (ResponseCarabinerosSettlementIndex carabinerosSettlementRocket in responseProcessAutomation.responseCarabinerosSettlementShowRequestId)
                {
                    LiquidationCarabineros liquidationCarabineros = new LiquidationCarabineros();
                    liquidationCarabineros.clientName = carabinerosSettlementRocket.clientName;
                    liquidationCarabineros.familyBurden = carabinerosSettlementRocket.familyBurden;
                    liquidationCarabineros.legalDiscounts = double.Parse(carabinerosSettlementRocket.legalDiscounts);
                    liquidationCarabineros.netAmount = int.Parse(carabinerosSettlementRocket.debtAmount);
                    liquidationCarabineros.period = carabinerosSettlementRocket.period;
                    liquidationCarabineros.rut = carabinerosSettlementRocket.clientRut;
                    liquidationCarabineros.settlementNumber = carabinerosSettlementRocket.settlementNumbre;
                    liquidationCarabineros.totalCredit = int.Parse(carabinerosSettlementRocket.totalCredit);
                    documentValidate.liquidationCarabineros = liquidationCarabineros;
                }
            }
            if (responseProcessAutomation.responsePensionSettlementShowRequestId != null)
            {
                foreach (ResponsePensionSettlementIndex pensionSettlementRocket in responseProcessAutomation.responsePensionSettlementShowRequestId)
                {
                    LiquidationPensions liquidationPensions = new LiquidationPensions();
                    liquidationPensions.clientName = pensionSettlementRocket.clientName;
                    liquidationPensions.codeValidation = pensionSettlementRocket.validationCode;
                    liquidationPensions.month = pensionSettlementRocket.month;
                    liquidationPensions.netTotal = pensionSettlementRocket.netTotal;
                    liquidationPensions.period = pensionSettlementRocket.period;
                    liquidationPensions.subtotalAssets = pensionSettlementRocket.subTotalCredit;
                    liquidationPensions.subtotalDiscounts = pensionSettlementRocket.subTotalDiscounts;
                    documentValidate.liquidationPensions = liquidationPensions;
                }
            }
            if (responseProcessAutomation.responseSalarySettlementShowRequestId != null)
            {
                foreach (ResponseSalarySettlementIndex salarySettlementRocket in responseProcessAutomation.responseSalarySettlementShowRequestId)
                {
                    Salarysettlement salarysettlement = new Salarysettlement();
                    salarysettlement.Month = salarySettlementRocket.month;
                    salarysettlement.adjustment = salarySettlementRocket.adjustments;
                    salarysettlement.afp = salarySettlementRocket.afp;
                    salarysettlement.apv = salarySettlementRocket.apv.ToString();
                    salarysettlement.exemptUnemploymentInsurance = double.Parse(salarySettlementRocket.exemptSeveranceInsurance);
                    salarysettlement.forecast = int.Parse(salarySettlementRocket.medicalInsurance);
                    salarysettlement.grossAmount = salarySettlementRocket.grossAmount;
                    salarysettlement.healthAmountOne = salarySettlementRocket.healthAmount1;
                    salarysettlement.healthAmountTwo = salarySettlementRocket.healthAmount2;
                    salarysettlement.rut = salarySettlementRocket.clientRut;
                    salarysettlement.taxs = salarySettlementRocket.tax;
                    documentValidate.salarysettlement = salarysettlement;
                }
            }
            if (responseProcessAutomation.responseProofofAddressShowRequestId != null)
            {
                foreach (ResponseProofofAddressIndex proofofAddressRocket in responseProcessAutomation.responseProofofAddressShowRequestId)
                {
                    ProofOfAddress proofOfAddress = new ProofOfAddress();
                    proofOfAddress.address = proofofAddressRocket.clientAbbress;
                    proofOfAddress.clientName = proofofAddressRocket.clientName;
                    proofOfAddress.commune = proofofAddressRocket.clientComune;
                    proofOfAddress.companyName = proofofAddressRocket.serviceProvider.companyName;
                    documentValidate.proofOfAddress = proofOfAddress;
                }
            }

            //DPS

            //PAGARE22
            if (responseProcessAutomation.responsePromissoryNoteShowRequestId != null)
            {
                foreach (ResponsePromissoryNoteIndex responsePN in responseProcessAutomation.responsePromissoryNoteShowRequestId)
                {
                    PromissoryNote promissoryNote = new PromissoryNote();
                    if (responsePN.PAGA_DEUDOR_RUT_VALUE != "" && responsePN.PAGA_DEUDOR_RUT_VALUE != Constant.TypeText)
                    {
                        #region RespPG
                        //promissoryNote.backFee = Convert.ToInt32(responsePN.PAGA_CUOTA_DORSO_VALUE);
                        //promissoryNote.coDebtorNameOne = responsePN.PAGA_CODEU_NOMBRE_VALUE;
                        //promissoryNote.coDebtorNameTwo = responsePN.PAGA_CODEU2_NOMBRE_VALUE;
                        //if (responsePN.PAGA_CODEU_NACION_VALUE == Constant.nationalityChilean)
                        //{
                        //    promissoryNote.coDebtorNationalityOne = Constant.One.ToString();
                        //}
                        //else if (responsePN.PAGA_CODEU_NACION_VALUE != Constant.nationalityChilean && responsePN.PAGA_CODEU_NACION_VALUE != "")
                        //{
                        //    promissoryNote.coDebtorNationalityOne = Constant.Two.ToString();
                        //}
                        //else
                        //{
                        //    promissoryNote.coDebtorNationalityOne = Constant.ninetyNine.ToString();
                        //}

                        //if (responsePN.PAGA_CODEU2_NACION_VALUE == Constant.nationalityChilean)
                        //{
                        //    promissoryNote.coDebtorNationalityTwo = Constant.One.ToString();
                        //}
                        //else if (responsePN.PAGA_CODEU2_NACION_VALUE != Constant.nationalityChilean && responsePN.PAGA_CODEU2_NACION_VALUE != "")
                        //{
                        //    promissoryNote.coDebtorNationalityTwo = Constant.Two.ToString();
                        //}
                        //else
                        //{
                        //    promissoryNote.coDebtorNationalityTwo = Constant.ninetyNine.ToString();
                        //}
                        //promissoryNote.coDebtorRutOne = responsePN.PAGA_CODEU_RUT_VALUE.Replace("-","");
                        //promissoryNote.coDebtorRutTwo = responsePN.PAGA_CODEU2_RUT_VALUE.Replace("-", "");
                        //promissoryNote.debtorName = responsePN.PAGA_DEUDOR_NOMBRE_VALUE;
                        //promissoryNote.debtorNationality = responsePN.PAGA_DEUDOR_NACION_VALUE;
                        //promissoryNote.debtorRut = responsePN.PAGA_DEUDOR_RUT_VALUE.Replace("-", "");
                        //promissoryNote.debtorSign = responsePN.PAGA_FIRMA_DEUDOR_VALUE;
                        //promissoryNote.duesNumber = responsePN.PAGA_NUM_CUOTAS_VALUE;
                        //promissoryNote.interestRate = float.Parse(responsePN.PAGA_TASA_INTERES_VALUE);
                        //promissoryNote.letterAmmount = responsePN.PAGA_MONTO_LETRAS_VALUE;
                        //promissoryNote.noteNumber = responsePN.PAGA_NUM_CUOTAS_VALUE;
                        //promissoryNote.numberAmmount = Convert.ToInt32(responsePN.PAGA_MONTO_NUM_VALUE);
                        //promissoryNote.signDate = Convert.ToDateTime(responsePN.PAGA_FECHA_FIRMA_VALUE).ToString("yyyy-MM-dd");
                        //promissoryNote.signPlace = responsePN.PAGA_DEUDOR_LUGAR_FIRMA_VALUE;
                        #endregion
                        promissoryNote.backFee = Convert.ToInt32(responsePN.PAGA_CUOTA_DORSO_VALUE);
                        promissoryNote.coDebtorNameOne = responsePN.PAGA_CODEU_NOMBRE_VALUE;
                        promissoryNote.coDebtorNameTwo = responsePN.PAGA_CODEU2_NOMBRE_VALUE;
                        promissoryNote.coDebtorNationalityOne = responsePN.PAGA_CODEU_NACION_VALUE;
                        promissoryNote.coDebtorNationalityTwo = responsePN.PAGA_CODEU2_NACION_VALUE;
                        promissoryNote.coDebtorRutOne = responsePN.PAGA_CODEU_RUT_VALUE;
                        promissoryNote.coDebtorRutTwo = responsePN.PAGA_CODEU2_RUT_VALUE;
                        promissoryNote.debtorName = responsePN.PAGA_DEUDOR_NOMBRE_VALUE;
                        promissoryNote.debtorNationality = responsePN.PAGA_DEUDOR_NACION_VALUE;
                        promissoryNote.debtorRut = responsePN.PAGA_DEUDOR_RUT_VALUE;
                        promissoryNote.debtorSign = responsePN.PAGA_FIRMA_DEUDOR_VALUE;
                        promissoryNote.duesNumber = responsePN.PAGA_NUM_CUOTAS_VALUE;
                        promissoryNote.interestRate = float.Parse(responsePN.PAGA_TASA_INTERES_VALUE);
                        promissoryNote.letterAmmount = responsePN.PAGA_MONTO_LETRAS_VALUE;
                        promissoryNote.noteNumber = responsePN.PAGA_NUM_CUOTAS_VALUE;//promissoryNote.noteNumber = responsePN.PAGA_NUM_CUOTAS_VALUE;
                        promissoryNote.numberAmmount = Convert.ToInt32(responsePN.PAGA_MONTO_NUM_VALUE);
                        promissoryNote.signDate = responsePN.PAGA_FECHA_FIRMA_VALUE;
                        promissoryNote.signPlace = responsePN.PAGA_DEUDOR_LUGAR_FIRMA_VALUE;

                        documentValidate.promissoryNote = promissoryNote;
                    }
                    else
                    {
                        responseProcessAutomation.responsePromissoryNoteShowRequestId = null;
                    }
                }
            }
            //MANDATO
            if (responseProcessAutomation.responseContractMandateShowRequestId != null)
            {
                foreach (ResponseContractMandateIndex responseMandate in responseProcessAutomation.responseContractMandateShowRequestId)
                {
                    MandatePledge mandatePledge = new MandatePledge();
                    if (responseMandate.MANDATO_PREND_ESP_MAN_RUT_VALUE != "" && responseMandate.MANDATO_PREND_ESP_MAN_RUT_VALUE != Constant.TypeText)
                    {
                        #region RespMn
                        //mandatePledge.debtorSignDate = Convert.ToDateTime(responseMandate.MANDATO_FECHA_FIRMA_DEUDOR_VALUE).ToString("yyyy-MM-dd");
                        //mandatePledge.debtorSignPlace = responseMandate.MANDATO_LUGAR_FIRMA_DEUDOR_VALUE;
                        //mandatePledge.mafRepresentative = responseMandate.MANDATO_ESP_REPRE_NOM_VALUE;
                        //mandatePledge.ownedName = responseMandate.MANDATO_ESP_REPRE_NOM_VALUE;
                        //mandatePledge.ownedRut = responseMandate.MANDATO_ESP_REPRE_RUT_VALUE.Replace("-", ""); 
                        //mandatePledge.representativeName = responseMandate.MANDATO_ESP_REPRE_NOM_VALUE;
                        //mandatePledge.representativeRut = responseMandate.MANDATO_ESP_REPRE_RUT_VALUE.Replace("-", "");
                        #endregion

                        mandatePledge.debtorSignDate = responseMandate.MANDATO_FECHA_FIRMA_DEUDOR_VALUE;
                        mandatePledge.debtorSignPlace = responseMandate.MANDATO_LUGAR_FIRMA_DEUDOR_VALUE;
                        mandatePledge.mafRepresentaive = responseMandate.MANDATO_ESP_REPRE_NOM_VALUE;
                        mandatePledge.ownedName = responseMandate.MANDATO_ESP_MAN_NOM_VALUE;
                        mandatePledge.ownedRut = responseMandate.MANDATO_PREND_ESP_MAN_RUT_VALUE;
                        mandatePledge.representativeName = responseMandate.MANDATO_ESP_REPRE_NOM_VALUE;
                        mandatePledge.representativeRut = responseMandate.MANDATO_ESP_REPRE_RUT_VALUE;

                        documentValidate.mandatePledge = mandatePledge;
                    }
                    else
                    {
                        responseProcessAutomation.responseContractMandateShowRequestId = null;
                    }
                }
            }
            //CONTRATO DE CREDITO
            if (responseProcessAutomation.responseCreditContractShowRequestId != null)
            {
                foreach (ResponseCreditContractIndex responseContractMandateIndex in responseProcessAutomation.responseCreditContractShowRequestId)
                {
                    CreditContract creditContract = new CreditContract();
                    if (responseContractMandateIndex.CTOCRED_RUT_DEUDOR_VALUE != "" && responseContractMandateIndex.CTOCRED_RUT_DEUDOR_VALUE != null &&
                        responseContractMandateIndex.CTOCRED_RUT_DEUDOR_VALUE != Constant.TypeText)
                    {
                        #region RespCc
                        //creditContract.brand = responseContractMandateIndex.CTOCRED_MARCA_VALUE;
                        //creditContract.coDebtorName = responseContractMandateIndex.CTOCRED_NOM_CODEUDOR_VALUE;
                        //creditContract.coDebtorRut = responseContractMandateIndex.CTOCRED_RUT_CODEUDOR_VALUE.Replace("-", "");
                        //creditContract.comercialValue = Convert.ToInt32(responseContractMandateIndex.CTOCRED_VALOR_COMERCIAL_VALUE);
                        //creditContract.contractType = responseContractMandateIndex.CTOCRED_TIPO_VALUE;
                        //creditContract.debterSign = responseContractMandateIndex.CTOCRED_FIRMA_DEUDOR_VALUE;
                        //creditContract.debtorName = responseContractMandateIndex.CTOCRED_NOM_DEUDOR_VALUE;
                        //creditContract.debtorRut = responseContractMandateIndex.CTOCRED_RUT_DEUDOR_VALUE.Replace("-", "");
                        //creditContract.doubleCapitalInsurance = Convert.ToDouble(responseContractMandateIndex.CTOCRED_SEG_DOBLE_CAPITAL_VALUE);
                        //creditContract.dueDate = Convert.ToDateTime(responseContractMandateIndex.CTOCRED_FECHA_CUOT_VALUE).ToString("yyyy-MM-dd");
                        //var varDueDay = DateTime.Parse(responseContractMandateIndex.CTOCRED_FECHA_CUOT_VALUE).Day;
                        //creditContract.dueDay = varDueDay.ToString();
                        //creditContract.dueNumber = responseContractMandateIndex.CTOCRED_NUM_CUOTA_VALUE;
                        //creditContract.dueValue = Convert.ToInt32(responseContractMandateIndex.CTOCRED_VALOR_CUOTA_VALUE);
                        //creditContract.externalServices = responseContractMandateIndex.CTOCRED_MARCA_VALUE;
                        //creditContract.gapRate = Convert.ToDouble(responseContractMandateIndex.CTOCRED_INTERES_DESFACE_VALUE);
                        //creditContract.legalCapacityDate = Convert.ToDateTime(responseContractMandateIndex.CTOCRED_FEC_PERSONERIA_VALUE).ToString("yyyy-MM-dd");
                        //creditContract.legalCapacityNotary = responseContractMandateIndex.CTOCRED_NOTARIO_PERSONERIA_VALUE;
                        //creditContract.maintenanceExpenses = Convert.ToInt32(responseContractMandateIndex.CTOCRED_GASTOS_MANTEN_VALUE);
                        //creditContract.model = responseContractMandateIndex.CTOCRED_MODELO_VALUE;
                        //creditContract.pie = Convert.ToInt32(responseContractMandateIndex.CTOCRED_PIE_VALUE);
                        //creditContract.pledgeService = responseContractMandateIndex.CTOCRED_MARCA_VALUE;
                        //creditContract.representativeName = responseContractMandateIndex.CTOCRED_NOM_REP_LEGAL_VALUE;
                        //creditContract.representativeRut = responseContractMandateIndex.CTOCRED_RUT_REP_LEGAL_VALUE.Replace("-", "");
                        //creditContract.taxesAndStamps = Convert.ToInt32(responseContractMandateIndex.CTOCRED_IMPU_TIMB_VALUE);
                        //creditContract.taxyClub = responseContractMandateIndex.CTOCRED_CLUB_TAXI_VALUE;
                        //creditContract.total = Convert.ToInt32(responseContractMandateIndex.CTOCRED_VALOR_VEHI_VALUE);
                        //creditContract.variableDueDay = varDueDay.ToString();
                        //creditContract.vehicleInsurance = Convert.ToDouble(responseContractMandateIndex.CTOCRED_SEGURO_AUTO_VALUE);
                        //creditContract.vehicleValue = Convert.ToInt32(responseContractMandateIndex.CTOCRED_VALOR_VEHI_VALUE);
                        #endregion
                        creditContract.brand = responseContractMandateIndex.CTOCRED_MARCA_VALUE;
                        creditContract.coDebtorName = responseContractMandateIndex.CTOCRED_NOM_CODEUDOR_VALUE;
                        creditContract.coDebtorRut = responseContractMandateIndex.CTOCRED_RUT_CODEUDOR_VALUE;
                        creditContract.comercialValue = Convert.ToDouble(responseContractMandateIndex.CTOCRED_VALOR_COMERCIAL_VALUE);
                        creditContract.comercialYear = Convert.ToInt32(responseContractMandateIndex.CTOCRED_ANIO_COMERCIAL_VALUE);
                        creditContract.contractType = responseContractMandateIndex.CTOCRED_TIPO_VALUE;
                        creditContract.debterSign = responseContractMandateIndex.CTOCRED_FIRMA_DEUDOR_VALUE;
                        creditContract.debtorName = responseContractMandateIndex.CTOCRED_NOM_DEUDOR_VALUE;
                        creditContract.debtorRut = responseContractMandateIndex.CTOCRED_RUT_DEUDOR_VALUE;
                        creditContract.doubleCapitalInsurance = Convert.ToDouble(responseContractMandateIndex.CTOCRED_SEG_DOBLE_CAPITAL_VALUE);
                        creditContract.dueDate = responseContractMandateIndex.CTOCRED_FECHA_CUOT_VALUE;

                        var varDueDay = DateTime.Parse(responseContractMandateIndex.CTOCRED_FECHA_CUOT_VALUE).Day;
                        creditContract.dueDay = varDueDay.ToString();

                        creditContract.dueNumber = responseContractMandateIndex.CTOCRED_NUM_CUOTA_VALUE;
                        creditContract.dueValue = Convert.ToInt32(responseContractMandateIndex.CTOCRED_VALOR_CUOTA_VALUE);
                        creditContract.externalServices = responseContractMandateIndex.CTOCRED_CLUB_TAXI_VALUE;
                        creditContract.gapRate = Convert.ToDouble(responseContractMandateIndex.CTOCRED_INTERES_DESFACE_VALUE);
                        creditContract.legalCapacityDate = responseContractMandateIndex.CTOCRED_FEC_PERSONERIA_VALUE;
                        creditContract.legalCapacityNotary = responseContractMandateIndex.CTOCRED_NOTARIO_PERSONERIA_VALUE;
                        creditContract.maintenanceExpenses = Convert.ToDouble(responseContractMandateIndex.CTOCRED_GASTOS_MANTEN_VALUE);
                        creditContract.model = responseContractMandateIndex.CTOCRED_MODELO_VALUE;
                        creditContract.pie = Convert.ToDouble(responseContractMandateIndex.CTOCRED_PIE_VALUE);
                        creditContract.pledgeService = responseContractMandateIndex.CTOCRED_SERVI_PREN_VALUE;
                        creditContract.representativeName = responseContractMandateIndex.CTOCRED_NOM_REP_LEGAL_VALUE;
                        creditContract.representativeRut = responseContractMandateIndex.CTOCRED_RUT_REP_LEGAL_VALUE;
                        creditContract.taxesAndStamps = Convert.ToDouble(responseContractMandateIndex.CTOCRED_IMPU_TIMB_VALUE);
                        creditContract.taxyClub = responseContractMandateIndex.CTOCRED_CLUB_TAXI_VALUE;
                        creditContract.total = Convert.ToDouble(responseContractMandateIndex.CTOCRED_VALOR_VEHI_VALUE);
                        creditContract.variableDueDay = varDueDay.ToString();
                        creditContract.vehicleInsurance = Convert.ToDouble(responseContractMandateIndex.CTOCRED_SEGURO_AUTO_VALUE);
                        creditContract.vehicleValue = Convert.ToDouble(responseContractMandateIndex.CTOCRED_VALOR_VEHI_VALUE);

                        documentValidate.creditContract = creditContract;
                    }
                    else
                    {
                        responseProcessAutomation.responseCreditContractShowRequestId = null;
                    }
                }
            }
            //SOLPRIM
            if (responseProcessAutomation.responseFirstRegistrationShowRequestId != null)
            {
                foreach (ResponseFirstRegistrationIndex responseFirst in responseProcessAutomation.responseFirstRegistrationShowRequestId)
                {
                    FirstInscriptionRequest firstInscription = new FirstInscriptionRequest();

                    if (responseFirst.SOL_PRIM_RUT_ADQUI_VALUE != "" && responseFirst.SOL_PRIM_RUT_ADQUI_VALUE != Constant.TypeText)
                    {
                        #region RespSI
                        //firstInscription.acquirerName = responseFirst.SOL_PRIM_NOM_ADQUI_VALUE;
                        //firstInscription.acquirerRut = responseFirst.SOL_PRIM_RUT_ADQUI_VALUE.Replace("-", "");
                        //firstInscription.brand = responseFirst.SOL_PRIM_MARCA_VALUE;
                        //firstInscription.chassisNumber = responseFirst.SOL_PRIM_CHASIS_VALUE;
                        //firstInscription.colour = responseFirst.SOL_PRIM_COLOR_VALUE;
                        //firstInscription.engineNumber = responseFirst.SOL_PRIM_MOTOR_VALUE;
                        //firstInscription.invoiceNumber = responseFirst.SOL_PRIM_NUM_FAC_VALUE == null ? "0" : responseFirst.SOL_PRIM_NUM_FAC_VALUE;
                        //firstInscription.model = responseFirst.SOL_PRIM_MODELO_VALUE;
                        //firstInscription.patent = responseFirst.SOL_PRIM_PATENTE_VALUE;
                        //firstInscription.year = Convert.ToInt32(responseFirst.SOL_PRIM_ANIO_VALUE);
                        #endregion
                        firstInscription.acquirerName = responseFirst.SOL_PRIM_NOM_ADQUI_VALUE;
                        firstInscription.acquirerRut = responseFirst.SOL_PRIM_RUT_ADQUI_VALUE;
                        firstInscription.brand = responseFirst.SOL_PRIM_MARCA_VALUE;
                        firstInscription.chassisNumber = responseFirst.SOL_PRIM_CHASIS_VALUE;
                        firstInscription.colour = responseFirst.SOL_PRIM_COLOR_VALUE;
                        firstInscription.engineNumber = responseFirst.SOL_PRIM_MOTOR_VALUE;
                        firstInscription.invoiceNumber = responseFirst.SOL_PRIM_NUM_FAC_VALUE;
                        firstInscription.model = responseFirst.SOL_PRIM_MODELO_VALUE;
                        firstInscription.patent = responseFirst.SOL_PRIM_PATENTE_VALUE;
                        firstInscription.year = Convert.ToInt32(responseFirst.SOL_PRIM_ANIO_VALUE);

                        documentValidate.firstInscriptionRequest = firstInscription;
                    }
                    else
                    {
                        responseProcessAutomation.responseFirstRegistrationShowRequestId = null;
                    }
                }
            }
            //FACTURA
            if (responseProcessAutomation.responseInvoiceShowRequestId != null)
            {
                foreach (ResponseInvoiceIndex responseInvoice in responseProcessAutomation.responseInvoiceShowRequestId)
                {
                    Invoice invoice = new Invoice();
                    if (responseInvoice.FAC_RUT_CLIEN_VALUE != "" && responseInvoice.FAC_RUT_CLIEN_VALUE != Constant.TypeText)
                    {
                        #region RespIn
                        //invoice.box = responseInvoice.FAC_CAJON_VALUE;
                        //invoice.brand = responseInvoice.FAC_MARCA_VALUE;
                        //invoice.buyForName = responseInvoice.FAC_NOM_COMPRA_VALUE;
                        //invoice.buyForRut = responseInvoice.FAC_RUT_COMPRA_VALUE;
                        //invoice.chassisNumber = responseInvoice.FAC_CHASIS_VALUE;
                        //invoice.clientName = responseInvoice.FAC_NOM_CLIEN_VALUE;
                        //invoice.comercialYear = responseInvoice.FAC_ANIO_COMERCIAL_VALUE == "" ? 0 : Convert.ToInt32(responseInvoice.FAC_ANIO_COMERCIAL_VALUE);
                        //invoice.concessionaryRut = responseInvoice.FAC_RUT_CONCE_VALUE;
                        //invoice.engineNumber = responseInvoice.FAC_MOTOR_VALUE;
                        //invoice.invoiceNumber = responseInvoice.FAC_NUM_VALUE;
                        //invoice.model = responseInvoice.FAC_MODELO_VALUE;
                        #endregion

                        invoice.box = responseInvoice.FAC_CAJON_VALUE;
                        invoice.brand = responseInvoice.FAC_MARCA_VALUE;
                        invoice.buyForName = responseInvoice.FAC_NOM_COMPRA_VALUE;
                        invoice.buyForRut = responseInvoice.FAC_RUT_COMPRA_VALUE;
                        invoice.chassisNumber = responseInvoice.FAC_CHASIS_VALUE;
                        invoice.clientName = responseInvoice.FAC_NOM_CLIEN_VALUE;
                        invoice.comercialYear = Convert.ToInt32(responseInvoice.FAC_ANIO_COMERCIAL_VALUE);
                        invoice.concessionaryRut = responseInvoice.FAC_RUT_CONCE_VALUE;
                        invoice.engineNumber = responseInvoice.FAC_MOTOR_VALUE;
                        invoice.invoiceNumber = responseInvoice.FAC_NUM_VALUE;
                        invoice.model = responseInvoice.FAC_MODELO_VALUE;
                        invoice.totalAmmount = Convert.ToDouble(responseInvoice.FAC_MONTO_TOTAL_VALUE);

                        documentValidate.invoice = invoice;
                    }
                    else
                    {
                        responseProcessAutomation.responseInvoiceShowRequestId = null;
                    }
                }
            }

            #region CAV
            //if (responseProcessAutomation.responseCAVTCGShowRequestId != null)//CAV
            //{
            //    foreach (ResponseCAVTCGIndex responseCav in responseProcessAutomation.responseCAVTCGShowRequestId)
            //    {
            //        Cav cav = new Cav();
            //        cav.baningDate = "";//responseCav.p;//????
            //        cav.baningDocumentType = ""; //responseCav.tipo;
            //        documentValidate.invoice = cav;
            //    }
            //}
            #endregion

            documentInformation.documentIDTCG = workitemid;
            documentInformation.documentType = classificationId;//*
            documentInformation.message = "";
            documentInformation.documentInformation = documentValidate;
            documentInformation.omniaDocumentID = omniaId;
            documentInformation.validated = validation;

            envelop.documentInformationValidated = documentInformation;
            envelop.processDate = System.DateTime.Now.ToString("yyyy-MM-dd");
            envelop.requestId = requestId.ToString();

            requestFisaValidated.Envelop = envelop;

            consumer.enterpriseCode = Constant.enterpriseCode;
            consumer.sysCode = Constant.sysCode;

            trace.branchId = Constant.Two;
            trace.carDealerId = Constant.stringThree;
            trace.channelId = Constant.stringOne;
            trace.conversationId = Constant.conversationId;
            trace.officeId = Constant.One;
            trace.sellerId = Constant.MiloneThousandFifty;
            trace.userId = Constant.MiloneThousandFifty;

            info.Consumer = consumer;
            info.Trace = trace;

            reqHeader.Info = info;

            reqMessage.ReqHeader = reqHeader;

            requestFisaValidated.ReqMessage = reqMessage;
            using (var logger = new LoggerConfiguration()
            .WriteTo.File(jsonFormatter,
            pathLog,
            Serilog.Events.LogEventLevel.Debug,
            rollingInterval: RollingInterval.Day)
            .WriteTo.MSSqlServer(conexionLog.ConnectionString, sqlLoggerOptions)
            .CreateLogger())
            {
                try
                {
                    //logger.Information("REQUEST FISA VALIDACION {@Entry}", JsonConvert.SerializeObject(requestFisaValidated));
                    var jsonVar = JsonConvert.SerializeObject(requestFisaValidated);
                    //var jsonVar2 = jsonVar.Replace("\"", " ");
                    //logger.Information("REQUEST FISA VALIDACION {@Entry}", jsonVar2);
                    guardoLog = SaveRequest(workitemid, newworkitemid, "", "", "", JsonConvert.SerializeObject(requestFisaValidated), "");
                    logger.Information("REQUEST FISA VALIDACION {@Entry}", requestFisaValidated);
                    string urlFisaValidated = _iconfiguration["UrlFisaValidated"];
                    var clientRest = new RestClient(urlFisaValidated);
                    var requestClient = new RestRequest(RestSharp.Method.POST);
                    requestClient.AddHeader("cache-control", "no-cache");
                    requestClient.AddHeader("content-type", "application/json");
                    //requestClient.AddHeader("Authorization", "Bearer " + respOauth.token);
                    requestClient.AddJsonBody(requestFisaValidated);
                    var restResponse = clientRest.Execute(requestClient);
                    logger.Information("RESPONSE FISA VALIDACION ORIGINAL {@Entry}", restResponse.Content.ToString());
                    responseFisaValidated = JsonConvert.DeserializeObject<ResponseFisaValidated>(restResponse.Content.ToString());
                    //logger.Information("RESPONSE FISA VALIDACION {@Entry}", JsonConvert.SerializeObject(responseFisaValidated));
                    logger.Information("RESPONSE FISA VALIDACION {@Entry}", responseFisaValidated);
                    guardoLog = SaveRequest(workitemid, newworkitemid, "", "", "", "", JsonConvert.SerializeObject(responseFisaValidated));
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return responseFisaValidated;
        }

        private ConfigTypeDoc GetConfigTypeDoc(int classificationId)
        {
            System.Data.SqlClient.SqlCommand comandoSql;
            SqlConnection conexion;
            System.Data.SqlClient.SqlDataReader dataReader;
            ConfigTypeDoc configTypeDoc = new ConfigTypeDoc();

            conexion = new Conection(_iconfiguration).SqlEntregaConexion(_iconfiguration, Constant.ConnectionType.GeneralInformation);

            try
            {
                conexion.Open();
                comandoSql = new SqlCommand("SpGetConfigTypeDoc", conexion);
                comandoSql.CommandType = System.Data.CommandType.StoredProcedure;
                comandoSql.Parameters.Add(new SqlParameter(Constant.ClassificationId, classificationId));
                dataReader = comandoSql.ExecuteReader();

                while (dataReader.Read())
                {
                    configTypeDoc.Rocketbot = bool.Parse(dataReader["Rocketbot"].ToString());
                    configTypeDoc.DPS = bool.Parse(dataReader["DPS"].ToString());
                    configTypeDoc.StringConfiguration = (dataReader["StringConfiguration"].ToString());
                }

                int convertCode;
                ErrorHandler errorHandler = new ErrorHandler();
            }
            catch (Exception ex)
            {
                throw;
            }
            return configTypeDoc;
        }

        private ResponseProcessAutomation GetProcessAutomation(RequestClassified requestClassified, string requestId)
        {
            var respOauth = AuthorizationServiceMAF();
            RequestProcessAutomation requestProcessAutomation = new RequestProcessAutomation();
            ResponseProcessAutomation responseProcessAutomation = new ResponseProcessAutomation();

            var classificationId = requestClassified.classificationId;

            var classifReader = GetConfigTypeDoc(classificationId);

            if (classifReader.Rocketbot)
            {
                var list = classifReader.StringConfiguration.Split(Constant.Pipe);
                requestProcessAutomation.DocumentType = Convert.ToInt32(list[Constant.Zero]);
                requestProcessAutomation.MethodType = Convert.ToInt32(list[Constant.One]);
                requestProcessAutomation.Request = "{\"requestId\":#rpc}";
                requestProcessAutomation.Request = requestProcessAutomation.Request.Replace("#rpc", requestId.ToString());
                try
                {
                    string UrlProcessAutomation = _iconfiguration["UrlProcessAutomation"];
                    var clientRest = new RestClient(UrlProcessAutomation);
                    var requestClient = new RestRequest(Method.POST);
                    requestClient.AddHeader("cache-control", "no-cache");
                    requestClient.AddHeader("content-type", "application/json");
                    requestClient.AddHeader("Authorization", "Bearer " + respOauth.token);
                    requestClient.AddJsonBody(requestProcessAutomation);
                    var restResponse = clientRest.Execute(requestClient);
                    ResponseProcessAutomation responseProcessAutomationTemp = JsonConvert.DeserializeObject<ResponseProcessAutomation>(restResponse.Content.ToString());

                    if (classificationId == Convert.ToInt32(Constant.DocumentType.CarpetaTributaria) || classificationId == Constant.Zero)//CARPETA TRIBUTARIA
                    {
                        responseProcessAutomation.responseTaxSituationShowRequestId = responseProcessAutomationTemp.responseTaxSituationShowRequestId;
                    }
                    else if (classificationId == Convert.ToInt32(Constant.DocumentType.BoletaHonorarios) || classificationId == Constant.Zero)//BOLETA HONORARIO
                    {
                        responseProcessAutomation.responseFeeTicketShowRequestId = responseProcessAutomationTemp.responseFeeTicketShowRequestId;
                    }
                    else if (classificationId == Convert.ToInt32(Constant.DocumentType.cav) || classificationId == Constant.Zero)//CAV
                    {
                        responseProcessAutomation.responseCavShowRequestId = responseProcessAutomationTemp.responseCavShowRequestId;
                    }
                    else if (classificationId == Convert.ToInt32(Constant.DocumentType.CupoTaxi) || classificationId == Constant.Zero)//CUPO TAXI
                    {
                        responseProcessAutomation.responseCabQuotaShowRequestId = responseProcessAutomationTemp.responseCabQuotaShowRequestId;
                    }
                    else if (classificationId == Convert.ToInt32(Constant.DocumentType.SituacionTributaria) || classificationId == Constant.Zero)//SITUACION TRITUTARIA
                    {
                        responseProcessAutomation.responseTaxSituationShowRequestId = responseProcessAutomationTemp.responseTaxSituationShowRequestId;
                    }
                    else if (classificationId == Convert.ToInt32(Constant.DocumentType.AFP) || classificationId == Constant.Zero) //AFP
                    {
                        responseProcessAutomation.responseAfpShowRequestId = responseProcessAutomationTemp.responseAfpShowRequestId;
                    }
                    else if (classificationId == Convert.ToInt32(Constant.DocumentType.LiquidacionPension) || classificationId == Constant.Zero) //LIQUIDACION PENSION
                    {
                        responseProcessAutomation.responsePensionSettlementShowRequestId = responseProcessAutomationTemp.responsePensionSettlementShowRequestId;
                    }
                    else if (classificationId == Convert.ToInt32(Constant.DocumentType.LiquidacionSueldo) || classificationId == Constant.Zero) //LIQUIDACION SUELDO)
                    {
                        responseProcessAutomation.responseSalarySettlementShowRequestId = responseProcessAutomationTemp.responseSalarySettlementShowRequestId;
                    }
                    else if (classificationId == Convert.ToInt32(Constant.DocumentType.ComprobanteDomicilio) || classificationId == Constant.Zero) //COMPROBANTE DOMICILIO)
                    {
                        responseProcessAutomation.responseProofofAddressShowRequestId = responseProcessAutomationTemp.responseProofofAddressShowRequestId;
                        //LIQUIDACION CARABINEROS PENDIENTE
                    }

                    if (classificationId != Constant.Zero)
                    {
                        return responseProcessAutomationTemp;
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else//DPS
            {

                if (requestClassified.fieldValueList != null && classifReader.DPS)
                {
                    var fieldValueList = requestClassified.fieldValueList;

                    List<ResponsePromissoryNoteIndex> responsePromissoryNoteId = PromissoryNoteMethod(fieldValueList);//pagare 22
                    List<ResponseContractMandateIndex> responseContractMandateIndex = ContractMandateMethod(fieldValueList);//mandato 23
                    List<ResponseCreditContractIndex> responseCreditContractId = CreditContracMethod(fieldValueList);//Contrato de Credito 37
                    List<ResponseFirstRegistrationIndex> responseFirstRegistrationId = FirstRegistrationMethod(fieldValueList);//Solicitud Primera Inscripción 31
                    List<ResponseInvoiceIndex> responseInvoiceId = InvoiceIdMethod(fieldValueList);//Factura 30
                    List<ResponseCAVTCGIndex> responseCAVTCGId = responseCAVTCGIdMethod(fieldValueList);//CAV ??

                    responseProcessAutomation.responsePromissoryNoteShowRequestId = responsePromissoryNoteId;
                    responseProcessAutomation.responseContractMandateShowRequestId = responseContractMandateIndex;
                    responseProcessAutomation.responseCreditContractShowRequestId = responseCreditContractId;
                    responseProcessAutomation.responseFirstRegistrationShowRequestId = responseFirstRegistrationId;
                    responseProcessAutomation.responseInvoiceShowRequestId = responseInvoiceId;
                    responseProcessAutomation.responseCAVTCGShowRequestId = responseCAVTCGId;
                }
            }
            return responseProcessAutomation;
        }

        //Pagare
        private List<ResponsePromissoryNoteIndex> PromissoryNoteMethod(FieldValueList fieldValueList)
        {
            var responsePromissoryNoteId = new ResponsePromissoryNoteIndex();
            List<ResponsePromissoryNoteIndex> responsePromissoryNoteIdList = new List<ResponsePromissoryNoteIndex>();

            try
            {
                #region RespP
                //responsePromissoryNoteId.PAGA_NRO_VALUE = fieldValueList.PAGA_NRO_VALUE == null ? "" : fieldValueList.PAGA_NRO_VALUE;
                //responsePromissoryNoteId.PAGA_NRO_VALID = fieldValueList.PAGA_NRO_VALID == null ? "" : fieldValueList.PAGA_NRO_VALID;
                //responsePromissoryNoteId.PAGA_MONTO_LETRAS_VALUE = fieldValueList.PAGA_MONTO_LETRAS_VALUE == null ? "" : fieldValueList.PAGA_MONTO_LETRAS_VALUE;
                //responsePromissoryNoteId.PAGA_MONTO_LETRAS_VALID = fieldValueList.PAGA_MONTO_LETRAS_VALID == null ? "" : fieldValueList.PAGA_MONTO_LETRAS_VALID;
                //responsePromissoryNoteId.PAGA_NUM_CUOTAS_VALUE = fieldValueList.PAGA_NUM_CUOTAS_VALUE == null ? "" : fieldValueList.PAGA_NUM_CUOTAS_VALUE;
                //responsePromissoryNoteId.PAGA_NUM_CUOTAS_VALID = fieldValueList.PAGA_NUM_CUOTAS_VALID == null ? "" : fieldValueList.PAGA_NUM_CUOTAS_VALID;
                //responsePromissoryNoteId.PAGA_VALOR_CUOTAS_VALUE = fieldValueList.PAGA_VALOR_CUOTAS_VALUE == null ? "" : fieldValueList.PAGA_VALOR_CUOTAS_VALUE;
                //responsePromissoryNoteId.PAGA_VALOR_CUOTAS_VALID = fieldValueList.PAGA_VALOR_CUOTAS_VALID == null ? "" : fieldValueList.PAGA_VALOR_CUOTAS_VALID;
                //responsePromissoryNoteId.PAGA_TASA_INTERES_VALUE = fieldValueList.PAGA_TASA_INTERES_VALUE == null ? "" : fieldValueList.PAGA_TASA_INTERES_VALUE;
                //responsePromissoryNoteId.PAGA_TASA_INTERES_VALID = fieldValueList.PAGA_TASA_INTERES_VALID == null ? "" : fieldValueList.PAGA_TASA_INTERES_VALID;
                //responsePromissoryNoteId.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALUE = fieldValueList.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALUE == null ? "" : fieldValueList.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALUE;
                //responsePromissoryNoteId.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALID = fieldValueList.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALID == null ? "" : fieldValueList.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALID;
                //responsePromissoryNoteId.PAGA_CUOTA_DORSO_VALUE = fieldValueList.PAGA_CUOTA_DORSO_VALUE == null ? "" : fieldValueList.PAGA_CUOTA_DORSO_VALUE;
                //responsePromissoryNoteId.PAGA_CUOTA_DORSO_VALID = fieldValueList.PAGA_CUOTA_DORSO_VALID == null ? "" : fieldValueList.PAGA_CUOTA_DORSO_VALID;
                //responsePromissoryNoteId.PAGA_DEUDOR_NOMBRE_VALUE = fieldValueList.PAGA_DEUDOR_NOMBRE_VALUE == null ? "" : fieldValueList.PAGA_DEUDOR_NOMBRE_VALUE;
                //responsePromissoryNoteId.PAGA_DEUDOR_NOMBRE_VALID = fieldValueList.PAGA_DEUDOR_NOMBRE_VALID == null ? "" : fieldValueList.PAGA_DEUDOR_NOMBRE_VALID;
                //responsePromissoryNoteId.PAGA_DEUDOR_NACION_VALUE = fieldValueList.PAGA_DEUDOR_NACION_VALUE == null ? "" : fieldValueList.PAGA_DEUDOR_NACION_VALUE;
                //responsePromissoryNoteId.PAGA_DEUDOR_NACION_VALID = fieldValueList.PAGA_DEUDOR_NACION_VALID == null ? "" : fieldValueList.PAGA_DEUDOR_NACION_VALID;
                //responsePromissoryNoteId.PAGA_DEUDOR_RUT_VALUE = fieldValueList.PAGA_DEUDOR_RUT_VALUE == null ? "" : fieldValueList.PAGA_DEUDOR_RUT_VALUE;
                //responsePromissoryNoteId.PAGA_DEUDOR_RUT_VALID = fieldValueList.PAGA_DEUDOR_RUT_VALID == null ? "" : fieldValueList.PAGA_DEUDOR_RUT_VALID;
                //responsePromissoryNoteId.PAGA_FIRMA_DEUDOR_VALUE = fieldValueList.PAGA_FIRMA_DEUDOR_VALUE == null ? "" : fieldValueList.PAGA_FIRMA_DEUDOR_VALUE;
                //responsePromissoryNoteId.PAGA_FIRMA_DEUDOR_VALID = fieldValueList.PAGA_FIRMA_DEUDOR_VALID == null ? "" : fieldValueList.PAGA_FIRMA_DEUDOR_VALID;
                //responsePromissoryNoteId.PAGA_DEUDOR_LUGAR_FIRMA_VALUE = fieldValueList.PAGA_DEUDOR_LUGAR_FIRMA_VALUE == null ? "" : fieldValueList.PAGA_DEUDOR_LUGAR_FIRMA_VALUE;
                //responsePromissoryNoteId.PAGA_DEUDOR_LUGAR_FIRMA_VALID = fieldValueList.PAGA_DEUDOR_LUGAR_FIRMA_VALID == null ? "" : fieldValueList.PAGA_DEUDOR_LUGAR_FIRMA_VALID;
                //responsePromissoryNoteId.PAGA_FECHA_FIRMA_VALUE = fieldValueList.PAGA_FECHA_FIRMA_VALUE == null ? "" : fieldValueList.PAGA_FECHA_FIRMA_VALUE;
                //responsePromissoryNoteId.PAGA_FECHA_FIRMA_VALID = fieldValueList.PAGA_FECHA_FIRMA_VALID == null ? "" : fieldValueList.PAGA_FECHA_FIRMA_VALID;
                //responsePromissoryNoteId.PAGA_CODEU_NOMBRE_VALUE = fieldValueList.PAGA_CODEU_NOMBRE_VALUE == null ? "" : fieldValueList.PAGA_CODEU_NOMBRE_VALUE;
                //responsePromissoryNoteId.PAGA_CODEU_NOMBRE_VALID = fieldValueList.PAGA_CODEU_NOMBRE_VALID == null ? "" : fieldValueList.PAGA_CODEU_NOMBRE_VALID;
                //responsePromissoryNoteId.PAGA_CODEU_NACION_VALUE = fieldValueList.PAGA_CODEU_NACION_VALUE == null ? "" : fieldValueList.PAGA_CODEU_NACION_VALUE;
                //responsePromissoryNoteId.PAGA_CODEU_NACION_VALID = fieldValueList.PAGA_CODEU_NACION_VALID == null ? "" : fieldValueList.PAGA_CODEU_NACION_VALID;
                //responsePromissoryNoteId.PAGA_CODEU_RUT_VALUE = fieldValueList.PAGA_CODEU_RUT_VALUE == null ? "" : fieldValueList.PAGA_CODEU_RUT_VALUE;
                //responsePromissoryNoteId.PAGA_CODEU_RUT_VALID = fieldValueList.PAGA_CODEU_RUT_VALID == null ? "" : fieldValueList.PAGA_CODEU_RUT_VALID;
                //responsePromissoryNoteId.PAGA_CODEU2_NOMBRE_VALUE = fieldValueList.PAGA_CODEU2_NOMBRE_VALUE_;
                //responsePromissoryNoteId.PAGA_CODEU2_VALID = fieldValueList.PAGA_CODEU2_NOMBRE_VALID == null ? "" : fieldValueList.PAGA_CODEU2_NOMBRE_VALID;
                //responsePromissoryNoteId.PAGA_CODEU2_NACION_VALUE = fieldValueList.PAGA_CODEU2_NACION_VALUE == null ? "" : fieldValueList.PAGA_CODEU2_NACION_VALUE;
                //responsePromissoryNoteId.PAGA_CODEU2_NACION_VALID = fieldValueList.PAGA_CODEU2_NACION_VALID == null ? "" : fieldValueList.PAGA_CODEU2_NACION_VALID;
                //responsePromissoryNoteId.PAGA_CODEU2_RUT_VALUE = fieldValueList.PAGA_CODEU2_RUT_VALUE == null ? "" : fieldValueList.PAGA_CODEU2_RUT_VALUE;
                //responsePromissoryNoteId.PAGA_CODEU2_RUT_VALID = fieldValueList.PAGA_CODEU2_RUT_VALID == null ? "" : fieldValueList.PAGA_CODEU2_RUT_VALID;
                //responsePromissoryNoteId.PAGA_MONTO_NUM_VALUE = fieldValueList.PAGA_MONTO_NUM_VALUE == null ? "" : fieldValueList.PAGA_MONTO_NUM_VALUE;
                //responsePromissoryNoteId.PAGA_MONTO_NUM_VALID = fieldValueList.PAGA_MONTO_NUM_VALID == null ? "" : fieldValueList.PAGA_MONTO_NUM_VALID;
                #endregion
                responsePromissoryNoteId.PAGA_NRO_VALUE = fieldValueList.PAGA_NRO_VALUE_;
                responsePromissoryNoteId.PAGA_NRO_VALID = fieldValueList.PAGA_NRO_VALID;
                responsePromissoryNoteId.PAGA_MONTO_LETRAS_VALUE = fieldValueList.PAGA_MONTO_LETRAS_VALUE_;
                responsePromissoryNoteId.PAGA_MONTO_LETRAS_VALID = fieldValueList.PAGA_MONTO_LETRAS_VALID;
                responsePromissoryNoteId.PAGA_NUM_CUOTAS_VALUE = fieldValueList.PAGA_NUM_CUOTAS_VALUE_;
                responsePromissoryNoteId.PAGA_NUM_CUOTAS_VALID = fieldValueList.PAGA_NUM_CUOTAS_VALID;
                responsePromissoryNoteId.PAGA_VALOR_CUOTAS_VALUE = fieldValueList.PAGA_VALOR_CUOTAS_VALUE_;
                responsePromissoryNoteId.PAGA_VALOR_CUOTAS_VALID = fieldValueList.PAGA_VALOR_CUOTAS_VALID;
                responsePromissoryNoteId.PAGA_TASA_INTERES_VALUE = fieldValueList.PAGA_TASA_INTERES_VALUE_;
                responsePromissoryNoteId.PAGA_TASA_INTERES_VALID = fieldValueList.PAGA_TASA_INTERES_VALID;
                responsePromissoryNoteId.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALUE = fieldValueList.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALUE_;
                responsePromissoryNoteId.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALID = fieldValueList.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALID;
                responsePromissoryNoteId.PAGA_CUOTA_DORSO_VALUE = fieldValueList.PAGA_CUOTA_DORSO_VALUE_;
                responsePromissoryNoteId.PAGA_CUOTA_DORSO_VALID = fieldValueList.PAGA_CUOTA_DORSO_VALID;
                responsePromissoryNoteId.PAGA_DEUDOR_NOMBRE_VALUE = fieldValueList.PAGA_DEUDOR_NOMBRE_VALUE_;
                responsePromissoryNoteId.PAGA_DEUDOR_NOMBRE_VALID = fieldValueList.PAGA_DEUDOR_NOMBRE_VALID;
                responsePromissoryNoteId.PAGA_DEUDOR_NACION_VALUE = fieldValueList.PAGA_DEUDOR_NACION_VALUE_;
                responsePromissoryNoteId.PAGA_DEUDOR_NACION_VALID = fieldValueList.PAGA_DEUDOR_NACION_VALID;
                responsePromissoryNoteId.PAGA_DEUDOR_RUT_VALUE = fieldValueList.PAGA_DEUDOR_RUT_VALUE_;
                responsePromissoryNoteId.PAGA_DEUDOR_RUT_VALID = fieldValueList.PAGA_DEUDOR_RUT_VALID;
                responsePromissoryNoteId.PAGA_FIRMA_DEUDOR_VALUE = fieldValueList.PAGA_FIRMA_DEUDOR_VALUE_;
                responsePromissoryNoteId.PAGA_FIRMA_DEUDOR_VALID = fieldValueList.PAGA_FIRMA_DEUDOR_VALID;
                responsePromissoryNoteId.PAGA_DEUDOR_LUGAR_FIRMA_VALUE = fieldValueList.PAGA_DEUDOR_LUGAR_FIRMA_VALUE_;
                responsePromissoryNoteId.PAGA_DEUDOR_LUGAR_FIRMA_VALID = fieldValueList.PAGA_DEUDOR_LUGAR_FIRMA_VALID;
                responsePromissoryNoteId.PAGA_FECHA_FIRMA_VALUE = fieldValueList.PAGA_FECHA_FIRMA_VALUE_;
                responsePromissoryNoteId.PAGA_FECHA_FIRMA_VALID = fieldValueList.PAGA_FECHA_FIRMA_VALID;
                responsePromissoryNoteId.PAGA_CODEU_NOMBRE_VALUE = fieldValueList.PAGA_CODEU_NOMBRE_VALUE_;
                responsePromissoryNoteId.PAGA_CODEU_NOMBRE_VALID = fieldValueList.PAGA_CODEU_NOMBRE_VALID;
                responsePromissoryNoteId.PAGA_CODEU_NACION_VALUE = fieldValueList.PAGA_CODEU_NACION_VALUE_;
                responsePromissoryNoteId.PAGA_CODEU_NACION_VALID = fieldValueList.PAGA_CODEU_NACION_VALID;
                responsePromissoryNoteId.PAGA_CODEU_RUT_VALUE = fieldValueList.PAGA_CODEU_RUT_VALUE_;
                responsePromissoryNoteId.PAGA_CODEU_RUT_VALID = fieldValueList.PAGA_CODEU_RUT_VALID;
                responsePromissoryNoteId.PAGA_CODEU2_NOMBRE_VALUE = fieldValueList.PAGA_CODEU2_NOMBRE_VALUE_;
                responsePromissoryNoteId.PAGA_CODEU2_VALID = fieldValueList.PAGA_CODEU2_NOMBRE_VALID;
                responsePromissoryNoteId.PAGA_CODEU2_NACION_VALUE = fieldValueList.PAGA_CODEU2_NACION_VALUE_;
                responsePromissoryNoteId.PAGA_CODEU2_NACION_VALID = fieldValueList.PAGA_CODEU2_NACION_VALID;
                responsePromissoryNoteId.PAGA_CODEU2_RUT_VALUE = fieldValueList.PAGA_CODEU2_RUT_VALUE_;
                responsePromissoryNoteId.PAGA_CODEU2_RUT_VALID = fieldValueList.PAGA_CODEU2_RUT_VALID;
                responsePromissoryNoteId.PAGA_MONTO_NUM_VALUE = fieldValueList.PAGA_MONTO_NUM_VALUE_;
                responsePromissoryNoteId.PAGA_MONTO_NUM_VALID = fieldValueList.PAGA_MONTO_NUM_VALID;

                responsePromissoryNoteIdList.Add(responsePromissoryNoteId);
            }
            catch (Exception ex)
            {
                throw;
            }
            return responsePromissoryNoteIdList;
        }

        //mandato
        private List<ResponseContractMandateIndex> ContractMandateMethod(FieldValueList fieldValueList)
        {
            var responseContractMandateIndex = new ResponseContractMandateIndex();
            List<ResponseContractMandateIndex> responseContractMandateIndexList = new List<ResponseContractMandateIndex>();
            try
            {
                #region RespM
                //responseContractMandateIndex.MANDATO_PREND_ESP_MAN_RUT_VALUE = fieldValueList.MANDATO_PREND_ESP_MAN_RUT_VALUE == null ? "" : fieldValueList.MANDATO_PREND_ESP_MAN_RUT_VALUE;
                //responseContractMandateIndex.MANDATO_PREND_ESP_MAN_RUT_VALID = fieldValueList.MANDATO_PREND_ESP_MAN_RUT_VALID == null ? "" : fieldValueList.MANDATO_PREND_ESP_MAN_RUT_VALID;
                //responseContractMandateIndex.MANDATO_ESP_MAN_NOM_VALUE = fieldValueList.MANDATO_ESP_MAN_NOM_VALUE == null ? "" : fieldValueList.MANDATO_ESP_MAN_NOM_VALUE;
                //responseContractMandateIndex.MANDATO_ESP_MAN_NOM_VALID = fieldValueList.MANDATO_ESP_MAN_NOM_VALID == null ? "" : fieldValueList.MANDATO_ESP_MAN_NOM_VALID;
                //responseContractMandateIndex.MANDATO_ESP_REPRE_RUT_VALUE = fieldValueList.MANDATO_ESP_REPRE_RUT_VALUE == null ? "" : fieldValueList.MANDATO_ESP_REPRE_RUT_VALUE;
                //responseContractMandateIndex.MANDATO_ESP_REPRE_RUT_VALID = fieldValueList.MANDATO_ESP_REPRE_RUT_VALID == null ? "" : fieldValueList.MANDATO_ESP_REPRE_RUT_VALID;
                //responseContractMandateIndex.MANDATO_ESP_PERSO_FECHA_VALUE = fieldValueList.MANDATO_ESP_PERSO_FECHA_VALUE == null ? "" : fieldValueList.MANDATO_ESP_PERSO_FECHA_VALUE;
                //responseContractMandateIndex.MANDATO_ESP_PERSO_FECHA_VALID = fieldValueList.MANDATO_ESP_PERSO_FECHA_VALID == null ? "" : fieldValueList.MANDATO_ESP_PERSO_FECHA_VALID;
                //responseContractMandateIndex.MANDATO_ESP_NOTA_PERSONERIA_VALUE = fieldValueList.MANDATO_ESP_NOTA_PERSONERIA_VALUE == null ? "" : fieldValueList.MANDATO_ESP_NOTA_PERSONERIA_VALUE;
                //responseContractMandateIndex.MANDATO_ESP_NOTA_PERSONERIA_VALID = fieldValueList.MANDATO_ESP_NOTA_PERSONERIA_VALID == null ? "" : fieldValueList.MANDATO_ESP_NOTA_PERSONERIA_VALID;
                //responseContractMandateIndex.MANDATO_ESP_REPRE_NOM_VALUE = fieldValueList.MANDATO_ESP_REPRE_NOM_VALUE == null ? "" : fieldValueList.MANDATO_ESP_REPRE_NOM_VALUE;
                //responseContractMandateIndex.MANDATO_ESP_REPRE_NOM_VALID = fieldValueList.MANDATO_ESP_REPRE_NOM_VALID == null ? "" : fieldValueList.MANDATO_ESP_REPRE_NOM_VALID;
                //responseContractMandateIndex.MANDATO_FECHA_FIRMA_DEUDOR_VALUE = fieldValueList.MANDATO_FECHA_FIRMA_DEUDOR_VALUE == null ? "" : fieldValueList.MANDATO_FECHA_FIRMA_DEUDOR_VALUE;
                //responseContractMandateIndex.MANDATO_FECHA_FIRMA_DEUDOR_VALID = fieldValueList.MANDATO_FECHA_FIRMA_DEUDOR_VALID == null ? "" : fieldValueList.MANDATO_FECHA_FIRMA_DEUDOR_VALID;
                //responseContractMandateIndex.MANDATO_LUGAR_FIRMA_DEUDOR_VALUE = fieldValueList.MANDATO_LUGAR_FIRMA_DEUDOR_VALUE == null ? "" : fieldValueList.MANDATO_LUGAR_FIRMA_DEUDOR_VALUE;
                //responseContractMandateIndex.MANDATO_LUGAR_FIRMA_DEUDOR_VALID = fieldValueList.MANDATO_LUGAR_FIRMA_DEUDOR_VALID == null ? "" : fieldValueList.MANDATO_LUGAR_FIRMA_DEUDOR_VALID;
                #endregion

                responseContractMandateIndex.MANDATO_PREND_ESP_MAN_RUT_VALUE = fieldValueList.MANDATO_PREND_ESP_MAN_RUT_VALUE_;
                responseContractMandateIndex.MANDATO_PREND_ESP_MAN_RUT_VALID = fieldValueList.MANDATO_PREND_ESP_MAN_RUT_VALID;
                responseContractMandateIndex.MANDATO_ESP_MAN_NOM_VALUE = fieldValueList.MANDATO_ESP_MAN_NOM_VALUE_;
                responseContractMandateIndex.MANDATO_ESP_MAN_NOM_VALID = fieldValueList.MANDATO_ESP_MAN_NOM_VALID;
                responseContractMandateIndex.MANDATO_ESP_REPRE_RUT_VALUE = fieldValueList.MANDATO_ESP_REPRE_RUT_VALUE_;
                responseContractMandateIndex.MANDATO_ESP_REPRE_RUT_VALID = fieldValueList.MANDATO_ESP_REPRE_RUT_VALID;
                responseContractMandateIndex.MANDATO_ESP_PERSO_FECHA_VALUE = fieldValueList.MANDATO_ESP_PERSO_FECHA_VALUE_;
                responseContractMandateIndex.MANDATO_ESP_PERSO_FECHA_VALID = fieldValueList.MANDATO_ESP_PERSO_FECHA_VALID;
                responseContractMandateIndex.MANDATO_ESP_NOTA_PERSONERIA_VALUE = fieldValueList.MANDATO_ESP_NOTA_PERSONERIA_VALUE_;
                responseContractMandateIndex.MANDATO_ESP_NOTA_PERSONERIA_VALID = fieldValueList.MANDATO_ESP_NOTA_PERSONERIA_VALID;
                responseContractMandateIndex.MANDATO_ESP_REPRE_NOM_VALUE = fieldValueList.MANDATO_ESP_REPRE_NOM_VALUE_;
                responseContractMandateIndex.MANDATO_ESP_REPRE_NOM_VALID = fieldValueList.MANDATO_ESP_REPRE_NOM_VALID;
                responseContractMandateIndex.MANDATO_FECHA_FIRMA_DEUDOR_VALUE = fieldValueList.MANDATO_FECHA_FIRMA_DEUDOR_VALUE_;
                responseContractMandateIndex.MANDATO_FECHA_FIRMA_DEUDOR_VALID = fieldValueList.MANDATO_FECHA_FIRMA_DEUDOR_VALID;
                responseContractMandateIndex.MANDATO_LUGAR_FIRMA_DEUDOR_VALUE = fieldValueList.MANDATO_LUGAR_FIRMA_DEUDOR_VALUE_;
                responseContractMandateIndex.MANDATO_LUGAR_FIRMA_DEUDOR_VALID = fieldValueList.MANDATO_LUGAR_FIRMA_DEUDOR_VALID;


                responseContractMandateIndexList.Add(responseContractMandateIndex);
            }
            catch (Exception ex)
            {
                throw;
            }
            return responseContractMandateIndexList;
        }

        //Contrato de Credito
        private List<ResponseCreditContractIndex> CreditContracMethod(FieldValueList fieldValueList)
        {
            var responseCreditContractIndex = new ResponseCreditContractIndex();
            List<ResponseCreditContractIndex> responseCreditContractIndexList = new List<ResponseCreditContractIndex>();
            try
            {
                #region RespC
                //responseCreditContractIndex.CTOCRED_RUT_DEUDOR_VALUE = fieldValueList.CTOCRED_RUT_DEUDOR_VALUE == null ? "" : fieldValueList.CTOCRED_RUT_DEUDOR_VALUE;
                //responseCreditContractIndex.CTOCRED_RUT_DEUDOR_VALID = fieldValueList.CTOCRED_RUT_DEUDOR_VALID == null ? "" : fieldValueList.CTOCRED_RUT_DEUDOR_VALID;
                //responseCreditContractIndex.CTOCRED_NOM_DEUDOR_VALUE = fieldValueList.CTOCRED_NOM_DEUDOR_VALUE == null ? "" : fieldValueList.CTOCRED_NOM_DEUDOR_VALUE;
                //responseCreditContractIndex.CTOCRED_NOM_DEUDOR_VALID = fieldValueList.CTOCRED_NOM_DEUDOR_VALID == null ? "" : fieldValueList.CTOCRED_NOM_DEUDOR_VALID;
                //responseCreditContractIndex.CTOCRED_NOM_CODEUDOR_VALUE = fieldValueList.CTOCRED_NOM_CODEUDOR_VALUE == null ? "" : fieldValueList.CTOCRED_NOM_CODEUDOR_VALUE;
                //responseCreditContractIndex.CTOCRED_NOM_CODEUDOR_VALID = fieldValueList.CTOCRED_NOM_CODEUDOR_VALID == null ? "" : fieldValueList.CTOCRED_NOM_CODEUDOR_VALID;
                //responseCreditContractIndex.CTOCRED_RUT_CODEUDOR_VALUE = fieldValueList.CTOCRED_RUT_CODEUDOR_VALUE == null ? "" : fieldValueList.CTOCRED_RUT_CODEUDOR_VALUE;
                //responseCreditContractIndex.CTOCRED_RUT_CODEUDOR_VALID = fieldValueList.CTOCRED_RUT_CODEUDOR_VALID == null ? "" : fieldValueList.CTOCRED_RUT_CODEUDOR_VALID;
                //responseCreditContractIndex.CTOCRED_RUT_REP_LEGAL_VALUE = fieldValueList.CTOCRED_RUT_REP_LEGAL_VALUE == null ? "" : fieldValueList.CTOCRED_RUT_REP_LEGAL_VALUE;
                //responseCreditContractIndex.CTOCRED_RUT_REP_LEGAL_VALID = fieldValueList.CTOCRED_RUT_REP_LEGAL_VALID == null ? "" : fieldValueList.CTOCRED_RUT_REP_LEGAL_VALID;
                //responseCreditContractIndex.CTOCRED_NOM_REP_LEGAL_VALUE = fieldValueList.CTOCRED_NOM_REP_LEGAL_VALUE == null ? "" : fieldValueList.CTOCRED_NOM_REP_LEGAL_VALUE;
                //responseCreditContractIndex.CTOCRED_NOM_REP_LEGAL_VALID = fieldValueList.CTOCRED_NOM_REP_LEGAL_VALID == null ? "" : fieldValueList.CTOCRED_NOM_REP_LEGAL_VALID;
                //responseCreditContractIndex.CTOCRED_FEC_PERSONERIA_VALUE = fieldValueList.CTOCRED_FEC_PERSONERIA_VALUE == null ? "" : fieldValueList.CTOCRED_FEC_PERSONERIA_VALUE;
                //responseCreditContractIndex.CTOCRED_FEC_PERSONERIA_VALID = fieldValueList.CTOCRED_FEC_PERSONERIA_VALID == null ? "" : fieldValueList.CTOCRED_FEC_PERSONERIA_VALID;
                //responseCreditContractIndex.CTOCRED_NOTARIO_PERSONERIA_VALUE = fieldValueList.CTOCRED_NOTARIO_PERSONERIA_VALUE == null ? "" : fieldValueList.CTOCRED_NOTARIO_PERSONERIA_VALUE;
                //responseCreditContractIndex.CTOCRED_NOTARIO_PERSONERIA_VALID = fieldValueList.CTOCRED_NOTARIO_PERSONERIA_VALID == null ? "" : fieldValueList.CTOCRED_NOTARIO_PERSONERIA_VALID;
                //responseCreditContractIndex.CTOCRED_TIPO_VALUE = fieldValueList.CTOCRED_TIPO_VALUE == null ? "" : fieldValueList.CTOCRED_TIPO_VALUE;
                //responseCreditContractIndex.CTOCRED_TIPO_VALID = fieldValueList.CTOCRED_TIPO_VALID == null ? "" : fieldValueList.CTOCRED_TIPO_VALID;
                //responseCreditContractIndex.CTOCRED_MARCA_VALUE = fieldValueList.CTOCRED_MARCA_VALUE == null ? "" : fieldValueList.CTOCRED_MARCA_VALUE;
                //responseCreditContractIndex.CTOCRED_MARCA_VALID = fieldValueList.CTOCRED_MARCA_VALID == null ? "" : fieldValueList.CTOCRED_MARCA_VALID;
                //responseCreditContractIndex.CTOCRED_MODELO_VALUE = fieldValueList.CTOCRED_MODELO_VALUE == null ? "" : fieldValueList.CTOCRED_MODELO_VALUE;
                //responseCreditContractIndex.CTOCRED_MODELO_VALID = fieldValueList.CTOCRED_MODELO_VALID == null ? "" : fieldValueList.CTOCRED_MODELO_VALID;
                //responseCreditContractIndex.CTOCRED_ANIO_COMERCIAL_VALUE = fieldValueList.CTOCRED_ANIO_COMERCIAL_VALUE == null ? "" : fieldValueList.CTOCRED_ANIO_COMERCIAL_VALUE;
                //responseCreditContractIndex.CTOCRED_ANIO_COMERCIAL_VALID = fieldValueList.CTOCRED_ANIO_COMERCIAL_VALID == null ? "" : fieldValueList.CTOCRED_ANIO_COMERCIAL_VALID;
                //responseCreditContractIndex.CTOCRED_VALOR_COMERCIAL_VALUE = fieldValueList.CTOCRED_VALOR_COMERCIAL_VALUE == null ? "" : fieldValueList.CTOCRED_VALOR_COMERCIAL_VALUE;
                //responseCreditContractIndex.CTOCRED_VALOR_COMERCIAL_VALID = fieldValueList.CTOCRED_VALOR_COMERCIAL_VALID == null ? "" : fieldValueList.CTOCRED_VALOR_COMERCIAL_VALID;
                //responseCreditContractIndex.CTOCRED_VALOR_VEHI_VALUE = fieldValueList.CTOCRED_VALOR_VEHI_VALUE == null ? "" : fieldValueList.CTOCRED_VALOR_VEHI_VALUE;
                //responseCreditContractIndex.CTOCRED_VALOR_VEHI_VALID = fieldValueList.CTOCRED_VALOR_VEHI_VALID == null ? "" : fieldValueList.CTOCRED_VALOR_VEHI_VALID;
                //responseCreditContractIndex.CTOCRED_PIE_VALUE = fieldValueList.CTOCRED_PIE_VALUE == null ? "" : fieldValueList.CTOCRED_PIE_VALUE;
                //responseCreditContractIndex.CTOCRED_PIE_VALID = fieldValueList.CTOCRED_PIE_VALID == null ? "" : fieldValueList.CTOCRED_PIE_VALID;
                //responseCreditContractIndex.CTOCRED_TOTAL_VALUE = fieldValueList.CTOCRED_TOTAL_VALUE == null ? "" : fieldValueList.CTOCRED_TOTAL_VALUE;
                //responseCreditContractIndex.CTOCRED_TOTAL_VALID = fieldValueList.CTOCRED_TOTAL_VALID == null ? "" : fieldValueList.CTOCRED_TOTAL_VALID;
                //responseCreditContractIndex.CTOCRED_NUM_CUOTA_VALUE = fieldValueList.CTOCRED_NUM_CUOTA_VALUE == null ? "" : fieldValueList.CTOCRED_NUM_CUOTA_VALUE;
                //responseCreditContractIndex.CTOCRED_NUM_CUOTA_VALID = fieldValueList.CTOCRED_NUM_CUOTA_VALID == null ? "" : fieldValueList.CTOCRED_NUM_CUOTA_VALID;
                //responseCreditContractIndex.CTOCRED_VALOR_CUOTA_VALUE = fieldValueList.CTOCRED_VALOR_CUOTA_VALUE == null ? "" : fieldValueList.CTOCRED_VALOR_CUOTA_VALUE;
                //responseCreditContractIndex.CTOCRED_VALOR_CUOTA_VALID = fieldValueList.CTOCRED_VALOR_CUOTA_VALID == null ? "" : fieldValueList.CTOCRED_VALOR_CUOTA_VALID;
                //responseCreditContractIndex.CTOCRED_DIAS_CUOT_VALUE = fieldValueList.CTOCRED_DIAS_CUOT_VALUE == null ? "" : fieldValueList.CTOCRED_DIAS_CUOT_VALUE;
                //responseCreditContractIndex.CTOCRED_DIAS_CUOT_VALID = fieldValueList.CTOCRED_DIAS_CUOT_VALID == null ? "" : fieldValueList.CTOCRED_DIAS_CUOT_VALID;
                //responseCreditContractIndex.CTOCRED_FECHA_CUOT_VALUE = fieldValueList.CTOCRED_FECHA_CUOT_VALUE == null ? "" : fieldValueList.CTOCRED_FECHA_CUOT_VALUE;
                //responseCreditContractIndex.CTOCRED_FECHA_CUOT_VALID = fieldValueList.CTOCRED_FECHA_CUOT_VALID == null ? "" : fieldValueList.CTOCRED_FECHA_CUOT_VALID;
                //responseCreditContractIndex.CTOCRED_NUM_CUOTA_VARIABLE_VALUE = fieldValueList.CTOCRED_NUM_CUOTA_VARIABLE_VALUE == null ? "" : fieldValueList.CTOCRED_NUM_CUOTA_VARIABLE_VALUE;
                //responseCreditContractIndex.CTOCRED_NUM_CUOTA_VARIABLE_VALID = fieldValueList.CTOCRED_NUM_CUOTA_VARIABLE_VALID == null ? "" : fieldValueList.CTOCRED_NUM_CUOTA_VARIABLE_VALID;                
                //responseCreditContractIndex.CTOCRED_SERVI_PREN_VALUE = fieldValueList.CTOCRED_SERVI_PREN_VALUE == null ? "" : fieldValueList.CTOCRED_SERVI_PREN_VALUE;
                //responseCreditContractIndex.CTOCRED_SERVI_PREN_VALID = fieldValueList.CTOCRED_SERVI_PREN_VALID == null ? "" : fieldValueList.CTOCRED_SERVI_PREN_VALID;
                //responseCreditContractIndex.CTOCRED_INTERES_DESFACE_VALUE = fieldValueList.CTOCRED_INTERES_DESFACE_VALUE == null ? "" : fieldValueList.CTOCRED_INTERES_DESFACE_VALUE;
                //responseCreditContractIndex.CTOCRED_INTERES_DESFACE_VALID = fieldValueList.CTOCRED_INTERES_DESFACE_VALID == null ? "" : fieldValueList.CTOCRED_INTERES_DESFACE_VALID;
                //responseCreditContractIndex.CTOCRED_IMPU_TIMB_VALUE = fieldValueList.CTOCRED_IMPU_TIMB_VALUE == null ? "" : fieldValueList.CTOCRED_IMPU_TIMB_VALUE;
                //responseCreditContractIndex.CTOCRED_IMPU_TIMB_VALID = fieldValueList.CTOCRED_IMPU_TIMB_VALID == null ? "" : fieldValueList.CTOCRED_IMPU_TIMB_VALID;
                //responseCreditContractIndex.CTOCRED_GASTOS_MANTEN_VALUE = fieldValueList.CTOCRED_GASTOS_MANTEN_VALUE == null ? "" : fieldValueList.CTOCRED_GASTOS_MANTEN_VALUE;
                //responseCreditContractIndex.CTOCRED_GASTOS_MANTEN_VALID = fieldValueList.CTOCRED_GASTOS_MANTEN_VALID == null ? "" : fieldValueList.CTOCRED_GASTOS_MANTEN_VALID;
                //responseCreditContractIndex.CTOCRED_CLUB_TAXI_VALUE = fieldValueList.CTOCRED_CLUB_TAXI_VALUE == null ? "" : fieldValueList.CTOCRED_CLUB_TAXI_VALUE;
                //responseCreditContractIndex.CTOCRED_CLUB_TAXI_VALID = fieldValueList.CTOCRED_CLUB_TAXI_VALID == null ? "" : fieldValueList.CTOCRED_CLUB_TAXI_VALID;
                //responseCreditContractIndex.CTOCRED_SEGURO_DESG_VALUE = fieldValueList.CTOCRED_SEGURO_DESG_VALUE == null ? "" : fieldValueList.CTOCRED_SEGURO_DESG_VALUE;
                //responseCreditContractIndex.CTOCRED_SEGURO_DESG_VALID = fieldValueList.CTOCRED_SEGURO_DESG_VALID == null ? "" : fieldValueList.CTOCRED_SEGURO_DESG_VALID;
                //responseCreditContractIndex.CTOCRED_SEG_DOBLE_CAPITAL_VALUE = fieldValueList.CTOCRED_SEG_DOBLE_CAPITAL_VALUE == null ? "" : fieldValueList.CTOCRED_SEG_DOBLE_CAPITAL_VALUE;
                //responseCreditContractIndex.CTOCRED_SEG_DOBLE_CAPITAL_VALID = fieldValueList.CTOCRED_SEG_DOBLE_CAPITAL_VALID == null ? "" : fieldValueList.CTOCRED_SEG_DOBLE_CAPITAL_VALID;
                //responseCreditContractIndex.CTOCRED_SEGURO_DESE_VALUE = fieldValueList.CTOCRED_SEGURO_DESE_VALUE == null ? "" : fieldValueList.CTOCRED_SEGURO_DESE_VALUE;
                //responseCreditContractIndex.CTOCRED_SEGURO_DESE_VALID = fieldValueList.CTOCRED_SEGURO_DESE_VALID == null ? "" : fieldValueList.CTOCRED_SEGURO_DESE_VALID;
                //responseCreditContractIndex.CTOCRED_SEGURO_AUTO_VALUE = fieldValueList.CTOCRED_SEGURO_AUTO_VALUE == null ? "" : fieldValueList.CTOCRED_SEGURO_AUTO_VALUE;
                //responseCreditContractIndex.CTOCRED_SEGURO_AUTO_VALID = fieldValueList.CTOCRED_SEGURO_AUTO_VALID == null ? "" : fieldValueList.CTOCRED_SEGURO_AUTO_VALID;
                //responseCreditContractIndex.CTOCRED_FIRMA_DEUDOR_VALUE = fieldValueList.CTOCRED_FIRMA_DEUDOR_VALUE == null ? "" : fieldValueList.CTOCRED_FIRMA_DEUDOR_VALUE;
                //responseCreditContractIndex.CTOCRED_FIRMA_DEUDOR_VALID = fieldValueList.CTOCRED_FIRMA_DEUDOR_VALID == null ? "" : fieldValueList.CTOCRED_FIRMA_DEUDOR_VALID;
                //responseCreditContractIndex.CTOCRED_SERV_EXTERNO_VALUE = fieldValueList.CTOCRED_SERV_EXTERNO_VALUE == null ? "" : fieldValueList.CTOCRED_SERV_EXTERNO_VALUE;
                //responseCreditContractIndex.CTOCRED_SERV_EXTERNO_VALID = fieldValueList.CTOCRED_SERV_EXTERNO_VALID == null ? "" : fieldValueList.CTOCRED_SERV_EXTERNO_VALID;
                #endregion
                responseCreditContractIndex.CTOCRED_RUT_DEUDOR_VALUE = fieldValueList.CTOCRED_RUT_DEUDOR_VALUE_;
                responseCreditContractIndex.CTOCRED_RUT_DEUDOR_VALID = fieldValueList.CTOCRED_RUT_DEUDOR_VALID;
                responseCreditContractIndex.CTOCRED_NOM_DEUDOR_VALUE = fieldValueList.CTOCRED_NOM_DEUDOR_VALUE_;
                responseCreditContractIndex.CTOCRED_NOM_DEUDOR_VALID = fieldValueList.CTOCRED_NOM_DEUDOR_VALID;
                responseCreditContractIndex.CTOCRED_NOM_CODEUDOR_VALUE = fieldValueList.CTOCRED_NOM_CODEUDOR_VALUE_;
                responseCreditContractIndex.CTOCRED_NOM_CODEUDOR_VALID = fieldValueList.CTOCRED_NOM_CODEUDOR_VALID;
                responseCreditContractIndex.CTOCRED_RUT_CODEUDOR_VALUE = fieldValueList.CTOCRED_RUT_CODEUDOR_VALUE_;
                responseCreditContractIndex.CTOCRED_RUT_CODEUDOR_VALID = fieldValueList.CTOCRED_RUT_CODEUDOR_VALID;
                responseCreditContractIndex.CTOCRED_RUT_REP_LEGAL_VALUE = fieldValueList.CTOCRED_RUT_REP_LEGAL_VALUE_;
                responseCreditContractIndex.CTOCRED_RUT_REP_LEGAL_VALID = fieldValueList.CTOCRED_RUT_REP_LEGAL_VALID;
                responseCreditContractIndex.CTOCRED_NOM_REP_LEGAL_VALUE = fieldValueList.CTOCRED_NOM_REP_LEGAL_VALUE_;
                responseCreditContractIndex.CTOCRED_NOM_REP_LEGAL_VALID = fieldValueList.CTOCRED_NOM_REP_LEGAL_VALID;
                responseCreditContractIndex.CTOCRED_FEC_PERSONERIA_VALUE = fieldValueList.CTOCRED_FEC_PERSONERIA_VALUE_;
                responseCreditContractIndex.CTOCRED_FEC_PERSONERIA_VALID = fieldValueList.CTOCRED_FEC_PERSONERIA_VALID;
                responseCreditContractIndex.CTOCRED_NOTARIO_PERSONERIA_VALUE = fieldValueList.CTOCRED_NOTARIO_PERSONERIA_VALUE_;
                responseCreditContractIndex.CTOCRED_NOTARIO_PERSONERIA_VALID = fieldValueList.CTOCRED_NOTARIO_PERSONERIA_VALID;
                responseCreditContractIndex.CTOCRED_TIPO_VALUE = fieldValueList.CTOCRED_TIPO_VALUE_;
                responseCreditContractIndex.CTOCRED_TIPO_VALID = fieldValueList.CTOCRED_TIPO_VALID;
                responseCreditContractIndex.CTOCRED_MARCA_VALUE = fieldValueList.CTOCRED_MARCA_VALUE_;
                responseCreditContractIndex.CTOCRED_MARCA_VALID = fieldValueList.CTOCRED_MARCA_VALID;
                responseCreditContractIndex.CTOCRED_MODELO_VALUE = fieldValueList.CTOCRED_MODELO_VALUE_;
                responseCreditContractIndex.CTOCRED_MODELO_VALID = fieldValueList.CTOCRED_MODELO_VALID;
                responseCreditContractIndex.CTOCRED_ANIO_COMERCIAL_VALUE = fieldValueList.CTOCRED_ANIO_COMERCIAL_VALUE_;
                responseCreditContractIndex.CTOCRED_ANIO_COMERCIAL_VALID = fieldValueList.CTOCRED_ANIO_COMERCIAL_VALID;
                responseCreditContractIndex.CTOCRED_VALOR_COMERCIAL_VALUE = fieldValueList.CTOCRED_VALOR_COMERCIAL_VALUE_;
                responseCreditContractIndex.CTOCRED_VALOR_COMERCIAL_VALID = fieldValueList.CTOCRED_VALOR_COMERCIAL_VALID;
                responseCreditContractIndex.CTOCRED_VALOR_VEHI_VALUE = fieldValueList.CTOCRED_VALOR_VEHI_VALUE_;
                responseCreditContractIndex.CTOCRED_VALOR_VEHI_VALID = fieldValueList.CTOCRED_VALOR_VEHI_VALID;
                responseCreditContractIndex.CTOCRED_PIE_VALUE = fieldValueList.CTOCRED_PIE_VALUE_;
                responseCreditContractIndex.CTOCRED_PIE_VALID = fieldValueList.CTOCRED_PIE_VALID;
                responseCreditContractIndex.CTOCRED_TOTAL_VALUE = fieldValueList.CTOCRED_TOTAL_VALUE_;
                responseCreditContractIndex.CTOCRED_TOTAL_VALID = fieldValueList.CTOCRED_TOTAL_VALID;
                responseCreditContractIndex.CTOCRED_NUM_CUOTA_VALUE = fieldValueList.CTOCRED_NUM_CUOTA_VALUE_;
                responseCreditContractIndex.CTOCRED_NUM_CUOTA_VALID = fieldValueList.CTOCRED_NUM_CUOTA_VALID;
                responseCreditContractIndex.CTOCRED_VALOR_CUOTA_VALUE = fieldValueList.CTOCRED_VALOR_CUOTA_VALUE_;
                responseCreditContractIndex.CTOCRED_VALOR_CUOTA_VALID = fieldValueList.CTOCRED_VALOR_CUOTA_VALID;
                responseCreditContractIndex.CTOCRED_DIAS_CUOT_VALUE = fieldValueList.CTOCRED_DIAS_CUOT_VALUE_;
                responseCreditContractIndex.CTOCRED_DIAS_CUOT_VALID = fieldValueList.CTOCRED_DIAS_CUOT_VALID;
                responseCreditContractIndex.CTOCRED_FECHA_CUOT_VALUE = fieldValueList.CTOCRED_FECHA_CUOT_VALUE_;
                responseCreditContractIndex.CTOCRED_FECHA_CUOT_VALID = fieldValueList.CTOCRED_FECHA_CUOT_VALID;
                responseCreditContractIndex.CTOCRED_NUM_CUOTA_VARIABLE_VALUE = fieldValueList.CTOCRED_NUM_CUOTA_VARIABLE_VALUE_;
                responseCreditContractIndex.CTOCRED_NUM_CUOTA_VARIABLE_VALID = fieldValueList.CTOCRED_NUM_CUOTA_VARIABLE_VALID;
                responseCreditContractIndex.CTOCRED_SERVI_PREN_VALUE = fieldValueList.CTOCRED_SERVI_PREN_VALUE_;
                responseCreditContractIndex.CTOCRED_SERVI_PREN_VALID = fieldValueList.CTOCRED_SERVI_PREN_VALID;
                responseCreditContractIndex.CTOCRED_INTERES_DESFACE_VALUE = fieldValueList.CTOCRED_INTERES_DESFACE_VALUE_;
                responseCreditContractIndex.CTOCRED_INTERES_DESFACE_VALID = fieldValueList.CTOCRED_INTERES_DESFACE_VALID;
                responseCreditContractIndex.CTOCRED_IMPU_TIMB_VALUE = fieldValueList.CTOCRED_IMPU_TIMB_VALUE_;
                responseCreditContractIndex.CTOCRED_IMPU_TIMB_VALID = fieldValueList.CTOCRED_IMPU_TIMB_VALID;
                responseCreditContractIndex.CTOCRED_GASTOS_MANTEN_VALUE = fieldValueList.CTOCRED_GASTOS_MANTEN_VALUE_;
                responseCreditContractIndex.CTOCRED_GASTOS_MANTEN_VALID = fieldValueList.CTOCRED_GASTOS_MANTEN_VALID;
                responseCreditContractIndex.CTOCRED_CLUB_TAXI_VALUE = fieldValueList.CTOCRED_CLUB_TAXI_VALUE_;
                responseCreditContractIndex.CTOCRED_CLUB_TAXI_VALID = fieldValueList.CTOCRED_CLUB_TAXI_VALID;
                responseCreditContractIndex.CTOCRED_SEGURO_DESG_VALUE = fieldValueList.CTOCRED_SEGURO_DESG_VALUE_;
                responseCreditContractIndex.CTOCRED_SEGURO_DESG_VALID = fieldValueList.CTOCRED_SEGURO_DESG_VALID;
                responseCreditContractIndex.CTOCRED_SEG_DOBLE_CAPITAL_VALUE = fieldValueList.CTOCRED_SEG_DOBLE_CAPITAL_VALUE_;
                responseCreditContractIndex.CTOCRED_SEG_DOBLE_CAPITAL_VALID = fieldValueList.CTOCRED_SEG_DOBLE_CAPITAL_VALID;
                responseCreditContractIndex.CTOCRED_SEGURO_DESE_VALUE = fieldValueList.CTOCRED_SEGURO_DESE_VALUE_;
                responseCreditContractIndex.CTOCRED_SEGURO_DESE_VALID = fieldValueList.CTOCRED_SEGURO_DESE_VALID;
                responseCreditContractIndex.CTOCRED_SEGURO_AUTO_VALUE = fieldValueList.CTOCRED_SEGURO_AUTO_VALUE_;
                responseCreditContractIndex.CTOCRED_SEGURO_AUTO_VALID = fieldValueList.CTOCRED_SEGURO_AUTO_VALID;
                responseCreditContractIndex.CTOCRED_FIRMA_DEUDOR_VALUE = fieldValueList.CTOCRED_FIRMA_DEUDOR_VALUE_;
                responseCreditContractIndex.CTOCRED_FIRMA_DEUDOR_VALID = fieldValueList.CTOCRED_FIRMA_DEUDOR_VALID;
                responseCreditContractIndex.CTOCRED_SERV_EXTERNO_VALUE = fieldValueList.CTOCRED_SERV_EXTERNO_VALUE_;
                responseCreditContractIndex.CTOCRED_SERV_EXTERNO_VALID = fieldValueList.CTOCRED_SERV_EXTERNO_VALID;

                responseCreditContractIndexList.Add(responseCreditContractIndex);
            }
            catch (Exception ex)
            {
                throw;
            }
            return responseCreditContractIndexList;
        }

        //Solicitud Primera Inscripción
        private List<ResponseFirstRegistrationIndex> FirstRegistrationMethod(FieldValueList fieldValueList)
        {
            var responseFirstRegistrationIndex = new ResponseFirstRegistrationIndex();
            List<ResponseFirstRegistrationIndex> responseFirstRegistrationIndexList = new List<ResponseFirstRegistrationIndex>();
            try
            {
                #region RespPI
                //responseFirstRegistrationIndex.SOL_PRIM_NUM_FAC_VALUE = fieldValueList.SOL_PRIM_NUM_FAC_VALUE == null ? "" : fieldValueList.SOL_PRIM_NUM_FAC_VALUE;
                //responseFirstRegistrationIndex.SOL_PRIM_NUM_FAC_VALID = fieldValueList.SOL_PRIM_NUM_FAC_VALID == null ? "" : fieldValueList.SOL_PRIM_NUM_FAC_VALID;
                //responseFirstRegistrationIndex.SOL_PRIM_NOM_ADQUI_VALUE = fieldValueList.SOL_PRIM_NOM_ADQUI_VALUE == null ? "" : fieldValueList.SOL_PRIM_NOM_ADQUI_VALUE;
                //responseFirstRegistrationIndex.SOL_PRIM_NOM_ADQUI_VALID = fieldValueList.SOL_PRIM_NOM_ADQUI_VALID == null ? "" : fieldValueList.SOL_PRIM_NOM_ADQUI_VALID;
                //responseFirstRegistrationIndex.SOL_PRIM_RUT_ADQUI_VALUE = fieldValueList.SOL_PRIM_RUT_ADQUI_VALUE == null ? "" : fieldValueList.SOL_PRIM_RUT_ADQUI_VALUE;
                //responseFirstRegistrationIndex.SOL_PRIM_RUT_ADQUI_VALID = fieldValueList.SOL_PRIM_RUT_ADQUI_VALID == null ? "" : fieldValueList.SOL_PRIM_RUT_ADQUI_VALID;
                //responseFirstRegistrationIndex.SOL_PRIM_COLOR_VALUE = fieldValueList.SOL_PRIM_COLOR_VALUE == null ? "" : fieldValueList.SOL_PRIM_COLOR_VALUE;
                //responseFirstRegistrationIndex.SOL_PRIM_COLOR_VALID = fieldValueList.SOL_PRIM_COLOR_VALID == null ? "" : fieldValueList.SOL_PRIM_COLOR_VALID;
                //responseFirstRegistrationIndex.SOL_PRIM_PATENTE_VALUE = fieldValueList.SOL_PRIM_PATENTE_VALUE == null ? "" : fieldValueList.SOL_PRIM_PATENTE_VALUE;
                //responseFirstRegistrationIndex.SOL_PRIM_PATENTE_VALID = fieldValueList.SOL_PRIM_PATENTE_VALID == null ? "" : fieldValueList.SOL_PRIM_PATENTE_VALID;
                //responseFirstRegistrationIndex.SOL_PRIM_MARCA_VALUE = fieldValueList.SOL_PRIM_MARCA_VALUE == null ? "" : fieldValueList.SOL_PRIM_MARCA_VALUE;
                //responseFirstRegistrationIndex.SOL_PRIM_MARCA_VALID = fieldValueList.SOL_PRIM_MARCA_VALID == null ? "" : fieldValueList.SOL_PRIM_MARCA_VALID;
                //responseFirstRegistrationIndex.SOL_PRIM_MODELO_VALUE = fieldValueList.SOL_PRIM_MODELO_VALUE == null ? "" : fieldValueList.SOL_PRIM_MODELO_VALUE;
                //responseFirstRegistrationIndex.SOL_PRIM_MODELO_VALID = fieldValueList.SOL_PRIM_MODELO_VALID == null ? "" : fieldValueList.SOL_PRIM_MODELO_VALID;
                //responseFirstRegistrationIndex.SOL_PRIM_ANIO_VALUE = fieldValueList.SOL_PRIM_ANIO_VALUE == null ? "" : fieldValueList.SOL_PRIM_ANIO_VALUE;
                //responseFirstRegistrationIndex.SOL_PRIM_ANIO_VALID = fieldValueList.SOL_PRIM_ANIO_VALID == null ? "" : fieldValueList.SOL_PRIM_ANIO_VALID;
                //responseFirstRegistrationIndex.SOL_PRIM_MOTOR_VALUE = fieldValueList.SOL_PRIM_MOTOR_VALUE == null ? "" : fieldValueList.SOL_PRIM_MOTOR_VALUE;
                //responseFirstRegistrationIndex.SOL_PRIM_MOTOR_VALID = fieldValueList.SOL_PRIM_MOTOR_VALID == null ? "" : fieldValueList.SOL_PRIM_MOTOR_VALID;
                //responseFirstRegistrationIndex.SOL_PRIM_CHASIS_VALUE = fieldValueList.SOL_PRIM_CHASIS_VALUE == null ? "" : fieldValueList.SOL_PRIM_CHASIS_VALUE;
                //responseFirstRegistrationIndex.SOL_PRIM_CHASIS_VALID = fieldValueList.SOL_PRIM_CHASIS_VALID == null ? "" : fieldValueList.SOL_PRIM_CHASIS_VALID;
                #endregion
                responseFirstRegistrationIndex.SOL_PRIM_NUM_FAC_VALUE = fieldValueList.SOL_PRIM_NUM_FAC_VALUE_;
                responseFirstRegistrationIndex.SOL_PRIM_NUM_FAC_VALID = fieldValueList.SOL_PRIM_NUM_FAC_VALID;
                responseFirstRegistrationIndex.SOL_PRIM_NOM_ADQUI_VALUE = fieldValueList.SOL_PRIM_NOM_ADQUI_VALUE_;
                responseFirstRegistrationIndex.SOL_PRIM_NOM_ADQUI_VALID = fieldValueList.SOL_PRIM_NOM_ADQUI_VALID;
                responseFirstRegistrationIndex.SOL_PRIM_RUT_ADQUI_VALUE = fieldValueList.SOL_PRIM_RUT_ADQUI_VALUE_;
                responseFirstRegistrationIndex.SOL_PRIM_RUT_ADQUI_VALID = fieldValueList.SOL_PRIM_RUT_ADQUI_VALID;
                responseFirstRegistrationIndex.SOL_PRIM_COLOR_VALUE = fieldValueList.SOL_PRIM_COLOR_VALUE_;
                responseFirstRegistrationIndex.SOL_PRIM_COLOR_VALID = fieldValueList.SOL_PRIM_COLOR_VALID;
                responseFirstRegistrationIndex.SOL_PRIM_PATENTE_VALUE = fieldValueList.SOL_PRIM_PATENTE_VALUE_;
                responseFirstRegistrationIndex.SOL_PRIM_PATENTE_VALID = fieldValueList.SOL_PRIM_PATENTE_VALID;
                responseFirstRegistrationIndex.SOL_PRIM_MARCA_VALUE = fieldValueList.SOL_PRIM_MARCA_VALUE_;
                responseFirstRegistrationIndex.SOL_PRIM_MARCA_VALID = fieldValueList.SOL_PRIM_MARCA_VALID;
                responseFirstRegistrationIndex.SOL_PRIM_MODELO_VALUE = fieldValueList.SOL_PRIM_MODELO_VALUE_;
                responseFirstRegistrationIndex.SOL_PRIM_MODELO_VALID = fieldValueList.SOL_PRIM_MODELO_VALID;
                responseFirstRegistrationIndex.SOL_PRIM_ANIO_VALUE = fieldValueList.SOL_PRIM_ANIO_VALUE_;
                responseFirstRegistrationIndex.SOL_PRIM_ANIO_VALID = fieldValueList.SOL_PRIM_ANIO_VALID;
                responseFirstRegistrationIndex.SOL_PRIM_MOTOR_VALUE = fieldValueList.SOL_PRIM_MOTOR_VALUE_;
                responseFirstRegistrationIndex.SOL_PRIM_MOTOR_VALID = fieldValueList.SOL_PRIM_MOTOR_VALID;
                responseFirstRegistrationIndex.SOL_PRIM_CHASIS_VALUE = fieldValueList.SOL_PRIM_CHASIS_VALUE_;
                responseFirstRegistrationIndex.SOL_PRIM_CHASIS_VALID = fieldValueList.SOL_PRIM_CHASIS_VALID;

                responseFirstRegistrationIndexList.Add(responseFirstRegistrationIndex);

            }
            catch (Exception ex)
            {
                throw;
            }
            return responseFirstRegistrationIndexList;
        }

        //factura
        private List<ResponseInvoiceIndex> InvoiceIdMethod(FieldValueList fieldValueList)
        {
            var responseInvoiceIndex = new ResponseInvoiceIndex();
            List<ResponseInvoiceIndex> responseInvoiceIndexList = new List<ResponseInvoiceIndex>();
            try
            {
                #region RespF
                //responseInvoiceIndex.FAC_NUM_VALUE = fieldValueList.FAC_NUM_VALUE == null ? "" : fieldValueList.FAC_NUM_VALUE;
                //responseInvoiceIndex.FAC_NUM_VALID = fieldValueList.FAC_NUM_VALID == null ? "" : fieldValueList.FAC_NUM_VALID;
                //responseInvoiceIndex.FAC_RUT_CONCE_VALUE = fieldValueList.FAC_RUT_CONCE_VALUE == null ? "" : fieldValueList.FAC_RUT_CONCE_VALUE;
                //responseInvoiceIndex.FAC_RUT_CONCE_VALID = fieldValueList.FAC_RUT_CONCE_VALID == null ? "" : fieldValueList.FAC_RUT_CONCE_VALID;
                //responseInvoiceIndex.FAC_RUT_CLIEN_VALUE = fieldValueList.FAC_RUT_CLIEN_VALUE == null ? "" : fieldValueList.FAC_RUT_CLIEN_VALUE;
                //responseInvoiceIndex.FAC_RUT_CLIEN_VALID = fieldValueList.FAC_RUT_CLIEN_VALID == null ? "" : fieldValueList.FAC_RUT_CLIEN_VALID;
                //responseInvoiceIndex.FAC_NOM_CLIEN_VALUE = fieldValueList.FAC_NOM_CLIEN_VALUE == null ? "" : fieldValueList.FAC_NOM_CLIEN_VALUE;
                //responseInvoiceIndex.FAC_NOM_CLIEN_VALID = fieldValueList.FAC_NOM_CLIEN_VALID == null ? "" : fieldValueList.FAC_NOM_CLIEN_VALID;
                //responseInvoiceIndex.FAC_RUT_COMPRA_VALUE = fieldValueList.FAC_RUT_COMPRA_VALUE == null ? "" : fieldValueList.FAC_RUT_COMPRA_VALUE;
                //responseInvoiceIndex.FAC_RUT_COMPRA_VALID = fieldValueList.FAC_RUT_COMPRA_VALID == null ? "" : fieldValueList.FAC_RUT_COMPRA_VALID;
                //responseInvoiceIndex.FAC_NOM_COMPRA_VALUE = fieldValueList.FAC_NOM_COMPRA_VALUE == null ? "" : fieldValueList.FAC_NOM_COMPRA_VALUE;
                //responseInvoiceIndex.FAC_NOM_COMPRA_VALID = fieldValueList.FAC_NOM_COMPRA_VALID == null ? "" : fieldValueList.FAC_NOM_COMPRA_VALID;
                //responseInvoiceIndex.FAC_MARCA_VALUE = fieldValueList.FAC_MARCA_VALUE == null ? "" : fieldValueList.FAC_MARCA_VALUE;
                //responseInvoiceIndex.FAC_MARCA_VALID = fieldValueList.FAC_MARCA_VALID == null ? "" : fieldValueList.FAC_MARCA_VALID;
                //responseInvoiceIndex.FAC_MODELO_VALUE = fieldValueList.FAC_MODELO_VALUE == null ? "" : fieldValueList.FAC_MODELO_VALUE;
                //responseInvoiceIndex.FAC_MODELO_VALID = fieldValueList.FAC_MODELO_VALID == null ? "" : fieldValueList.FAC_MODELO_VALID;
                //responseInvoiceIndex.FAC_CHASIS_VALUE = fieldValueList.FAC_CHASIS_VALUE == null ? "" : fieldValueList.FAC_CHASIS_VALUE;
                //responseInvoiceIndex.FAC_CHASIS_VALID = fieldValueList.FAC_CHASIS_VALID == null ? "" : fieldValueList.FAC_CHASIS_VALID;
                //responseInvoiceIndex.FAC_CAJON_VALUE = fieldValueList.FAC_CAJON_VALUE == null ? "" : fieldValueList.FAC_CAJON_VALUE;
                //responseInvoiceIndex.FAC_CAJON_VALID = fieldValueList.FAC_CAJON_VALID == null ? "" : fieldValueList.FAC_CAJON_VALID;
                //responseInvoiceIndex.FAC_ANIO_COMERCIAL_VALUE = fieldValueList.FAC_ANIO_COMERCIAL_VALUE == null ? "" : fieldValueList.FAC_ANIO_COMERCIAL_VALUE;
                //responseInvoiceIndex.FAC_ANIO_COMERCIAL_VALID = fieldValueList.FAC_ANIO_COMERCIAL_VALID == null ? "" : fieldValueList.FAC_ANIO_COMERCIAL_VALID;
                //responseInvoiceIndex.FAC_MONTO_TOTAL_VALUE = fieldValueList.FAC_MONTO_TOTAL_VALUE == null ? "" : fieldValueList.FAC_MONTO_TOTAL_VALUE;
                //responseInvoiceIndex.FAC_MONTO_TOTAL_VALID = fieldValueList.FAC_MONTO_TOTAL_VALID == null ? "" : fieldValueList.FAC_MONTO_TOTAL_VALID;
                //responseInvoiceIndex.FAC_MOTOR_VALUE = fieldValueList.FAC_MOTOR_VALUE == null ? "" : fieldValueList.FAC_MOTOR_VALUE;
                //responseInvoiceIndex.FAC_MOTOR_VALID = fieldValueList.FAC_MOTOR_VALID == null ? "" : fieldValueList.FAC_MOTOR_VALID;
                #endregion
                responseInvoiceIndex.FAC_NUM_VALUE = fieldValueList.FAC_NUM_VALUE_;
                responseInvoiceIndex.FAC_NUM_VALID = fieldValueList.FAC_NUM_VALID;
                responseInvoiceIndex.FAC_RUT_CONCE_VALUE = fieldValueList.FAC_RUT_CONCE_VALUE_;
                responseInvoiceIndex.FAC_RUT_CONCE_VALID = fieldValueList.FAC_RUT_CONCE_VALID;
                responseInvoiceIndex.FAC_RUT_CLIEN_VALUE = fieldValueList.FAC_RUT_CLIEN_VALUE_;
                responseInvoiceIndex.FAC_RUT_CLIEN_VALID = fieldValueList.FAC_RUT_CLIEN_VALID;
                responseInvoiceIndex.FAC_NOM_CLIEN_VALUE = fieldValueList.FAC_NOM_CLIEN_VALUE_;
                responseInvoiceIndex.FAC_NOM_CLIEN_VALID = fieldValueList.FAC_NOM_CLIEN_VALID;
                responseInvoiceIndex.FAC_RUT_COMPRA_VALUE = fieldValueList.FAC_RUT_COMPRA_VALUE_;
                responseInvoiceIndex.FAC_RUT_COMPRA_VALID = fieldValueList.FAC_RUT_COMPRA_VALID;
                responseInvoiceIndex.FAC_NOM_COMPRA_VALUE = fieldValueList.FAC_NOM_COMPRA_VALUE_;
                responseInvoiceIndex.FAC_NOM_COMPRA_VALID = fieldValueList.FAC_NOM_COMPRA_VALID;
                responseInvoiceIndex.FAC_MARCA_VALUE = fieldValueList.FAC_MARCA_VALUE_;
                responseInvoiceIndex.FAC_MARCA_VALID = fieldValueList.FAC_MARCA_VALID;
                responseInvoiceIndex.FAC_MODELO_VALUE = fieldValueList.FAC_MODELO_VALUE_;
                responseInvoiceIndex.FAC_MODELO_VALID = fieldValueList.FAC_MODELO_VALID;
                responseInvoiceIndex.FAC_CHASIS_VALUE = fieldValueList.FAC_CHASIS_VALUE_;
                responseInvoiceIndex.FAC_CHASIS_VALID = fieldValueList.FAC_CHASIS_VALID;
                responseInvoiceIndex.FAC_CAJON_VALUE = fieldValueList.FAC_CAJON_VALUE_;
                responseInvoiceIndex.FAC_CAJON_VALID = fieldValueList.FAC_CAJON_VALID;
                responseInvoiceIndex.FAC_ANIO_COMERCIAL_VALUE = fieldValueList.FAC_ANIO_COMERCIAL_VALUE_;
                responseInvoiceIndex.FAC_ANIO_COMERCIAL_VALID = fieldValueList.FAC_ANIO_COMERCIAL_VALID;
                responseInvoiceIndex.FAC_MONTO_TOTAL_VALUE = fieldValueList.FAC_MONTO_TOTAL_VALUE_;
                responseInvoiceIndex.FAC_MONTO_TOTAL_VALID = fieldValueList.FAC_MONTO_TOTAL_VALID;
                responseInvoiceIndex.FAC_MOTOR_VALUE = fieldValueList.FAC_MOTOR_VALUE_;
                responseInvoiceIndex.FAC_MOTOR_VALID = fieldValueList.FAC_MOTOR_VALID;

                responseInvoiceIndexList.Add(responseInvoiceIndex);
            }
            catch (Exception ex)
            {
                throw;
            }
            return responseInvoiceIndexList;
        }

        //CAV
        private List<ResponseCAVTCGIndex> responseCAVTCGIdMethod(FieldValueList fieldValueList)
        {
            var responseCAVTCGId = new ResponseCAVTCGIndex();
            List<ResponseCAVTCGIndex> responseCAVTCGIdList = new List<ResponseCAVTCGIndex>();
            try
            {
                responseCAVTCGId.CAV_RUT_CLIENTE_VALUE = fieldValueList.CAV_RUT_CLIENTE_VALUE == null ? "" : fieldValueList.CAV_RUT_CLIENTE_VALUE;
                responseCAVTCGId.CAV_RUT_CLIENTE_VALID = fieldValueList.CAV_RUT_CLIENTE_VALID == null ? "" : fieldValueList.CAV_RUT_CLIENTE_VALID;
                responseCAVTCGId.CAV_NOM_PROPIETARIO_VALUE = fieldValueList.CAV_NOM_PROPIETARIO_VALUE == null ? "" : fieldValueList.CAV_NOM_PROPIETARIO_VALUE;
                responseCAVTCGId.CAV_NOM_PROPIETARIO_VALID = fieldValueList.CAV_NOM_PROPIETARIO_VALID == null ? "" : fieldValueList.CAV_NOM_PROPIETARIO_VALID;
                responseCAVTCGId.CAV_FOLIO_DOCUMENTO_VALUE = fieldValueList.CAV_FOLIO_DOCUMENTO_VALUE == null ? "" : fieldValueList.CAV_FOLIO_DOCUMENTO_VALUE;
                responseCAVTCGId.CAV_FOLIO_DOCUMENTO_VALID = fieldValueList.CAV_FOLIO_DOCUMENTO_VALID == null ? "" : fieldValueList.CAV_FOLIO_DOCUMENTO_VALID;
                responseCAVTCGId.CAV_COD_VERIFICACION_VALUE = fieldValueList.CAV_COD_VERIFICACION_VALUE == null ? "" : fieldValueList.CAV_COD_VERIFICACION_VALUE;
                responseCAVTCGId.CAV_COD_VERIFICACION_VALID = fieldValueList.CAV_COD_VERIFICACION_VALID == null ? "" : fieldValueList.CAV_COD_VERIFICACION_VALID;
                responseCAVTCGId.CAV_PATENTE_VALUE = fieldValueList.CAV_PATENTE_VALUE == null ? "" : fieldValueList.CAV_PATENTE_VALUE;
                responseCAVTCGId.CAV_PATENTE_VALID = fieldValueList.CAV_PATENTE_VALID == null ? "" : fieldValueList.CAV_PATENTE_VALID;
                responseCAVTCGId.CAV_MARCA_VALUE = fieldValueList.CAV_MARCA_VALUE == null ? "" : fieldValueList.CAV_MARCA_VALUE;
                responseCAVTCGId.CAV_MARCA_VALID = fieldValueList.CAV_MARCA_VALID == null ? "" : fieldValueList.CAV_MARCA_VALID;
                responseCAVTCGId.CAV_MODELO_VALUE = fieldValueList.CAV_MODELO_VALUE == null ? "" : fieldValueList.CAV_MODELO_VALUE;
                responseCAVTCGId.CAV_MODELO_VALID = fieldValueList.CAV_MODELO_VALID == null ? "" : fieldValueList.CAV_MODELO_VALID;
                responseCAVTCGId.CAV_NUM_MOTOR_VALUE = fieldValueList.CAV_NUM_MOTOR_VALUE == null ? "" : fieldValueList.CAV_NUM_MOTOR_VALUE;
                responseCAVTCGId.CAV_NUM_MOTOR_VALID = fieldValueList.CAV_NUM_MOTOR_VALID == null ? "" : fieldValueList.CAV_NUM_MOTOR_VALID;
                responseCAVTCGId.CAV_NUM_CHASIS_VALUE = fieldValueList.CAV_NUM_CHASIS_VALUE == null ? "" : fieldValueList.CAV_NUM_CHASIS_VALUE;
                responseCAVTCGId.CAV_NUM_CHASIS_VALID = fieldValueList.CAV_NUM_CHASIS_VALID == null ? "" : fieldValueList.CAV_NUM_CHASIS_VALID;
                responseCAVTCGId.CAV_COLOR_VALUE = fieldValueList.CAV_COLOR_VALUE == null ? "" : fieldValueList.CAV_COLOR_VALUE;
                responseCAVTCGId.CAV_COLOR_VALID = fieldValueList.CAV_COLOR_VALID == null ? "" : fieldValueList.CAV_COLOR_VALID;
                responseCAVTCGId.CAV_ANIO_VALUE = fieldValueList.CAV_ANIO_VALUE == null ? "" : fieldValueList.CAV_ANIO_VALUE;
                responseCAVTCGId.CAV_ANIO_VALID = fieldValueList.CAV_ANIO_VALID == null ? "" : fieldValueList.CAV_ANIO_VALID;

                responseCAVTCGIdList.Add(responseCAVTCGId);

            }
            catch (Exception ex)
            {
                throw;
            }
            return responseCAVTCGIdList;
        }

        private ResponseSendMail SendMailMAF(RequestSendMail requestSend)
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();

            try
            {
                string urlSendMail = _iconfiguration["UrlSendMail"];
                var respOauth = AuthorizationServiceMAF();

                var clientRest = new RestClient(urlSendMail);
                var requestClient = new RestRequest(RestSharp.Method.POST);
                requestClient.AddHeader("Authorization", "Bearer " + respOauth.token);
                requestClient.AddJsonBody(requestSend);
                var restResponse = clientRest.Execute(requestClient);
                var response = JsonConvert.DeserializeObject<ResponseSendMail>(restResponse.Content.ToString());
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool GetAlertConfiguration(RequestClassified requestClassified, string requestId)
        {
            bool response = true;
            try
            {
                var builder = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                _iconfiguration = builder.Build();

                System.Data.SqlClient.SqlCommand comandoSql;
                SqlConnection conexion;
                System.Data.SqlClient.SqlDataReader dataReader;

                conexion = new Conection(_iconfiguration).SqlEntregaConexion(_iconfiguration, Constant.ConnectionType.IntegrationBusiness);

                conexion.Open();
                comandoSql = new SqlCommand(Constant.Spgetalertconfig, conexion);
                comandoSql.CommandType = System.Data.CommandType.StoredProcedure;
                comandoSql.Parameters.Add(new SqlParameter(Constant.StrIdRole, 18));
                dataReader = comandoSql.ExecuteReader();
                int convertCode;
                ErrorHandler errorHandler = new ErrorHandler();

                while (dataReader.Read())
                {
                    if (int.TryParse(dataReader[Constant.ErrorCode].ToString(), out convertCode))
                    {
                        errorHandler.ErrorCode = convertCode;
                        errorHandler.MessageError = dataReader[Constant.ErrorMessage].ToString();
                    }
                    else
                    {
                        errorHandler.ErrorCode = Constant.NegativeOne;
                        errorHandler.MessageError = string.Empty;
                    }
                }
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        if (errorHandler.ErrorCode == Constant.Zero)
                        {
                            //Cuerpo del correo
                            //var bodyMail = System.IO.File.ReadAllText(mailBody);
                            var bodyMail = dataReader["bodyhtml"].ToString();
                            bodyMail = bodyMail.Replace("dia1", DateTime.Today.ToString("dd"));
                            bodyMail = bodyMail.Replace("mes1", DateTime.Today.ToString("MMMM"));
                            bodyMail = bodyMail.Replace("anno1", DateTime.Today.ToString("yyyy"));
                            bodyMail = bodyMail.Replace("nameSuccessful", dataReader["userfullname"].ToString());
                            bodyMail = bodyMail.Replace("idRequest", requestId);
                            bodyMail = bodyMail.Replace("workitemid", requestClassified.workItemIdOriginal.ToString());
                            bodyMail = bodyMail.Replace("newWorkitemid", requestClassified.newWorkItemId.ToString());
                            bodyMail = bodyMail.Replace("fileName", requestClassified.fileName);

                            var reqSendMail = new RequestSendMail();
                            var mail = new MailData();
                            var atachList = new List<Attachments>();
                            var mailto = new List<string>();
                            var mailcc = new List<string>();

                            //atachList.Add(new Attachments
                            //{
                            //    fileName = "",
                            //    base64 = ""
                            //});

                            mailto.Add(dataReader["email"].ToString());
                            mail.to = mailto;
                            mail.cc = mailcc;
                            mail.subject = dataReader["subject"].ToString();
                            mail.body = bodyMail;
                            reqSendMail.mailData = mail;
                            reqSendMail.mailData.attachments = atachList;
                            var resSendMail = SendMailMAF(reqSendMail);
                            response = true;
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                response = false;
            }
            return response;
        }

        private string SaveRequest(int workItemIdOriginal, int newWorkItemId, string tcgJson, string strFisaClassifiedRequest, string strFisaClassifiedResponse, string strFisaValidatedRequest, string strFisaValidatedResponse)
        {
            System.Data.SqlClient.SqlCommand comandoSql;
            SqlConnection conexion;
            System.Data.SqlClient.SqlDataReader dataReader;
            ConfigTypeDoc configTypeDoc = new ConfigTypeDoc();
            string response = "";

            conexion = new Conection(_iconfiguration).SqlEntregaConexion(_iconfiguration, Constant.ConnectionType.IntegrationBusiness);

            try
            {
                conexion.Open();
                comandoSql = new SqlCommand(Constant.SpUpdateClassifiedDocument, conexion);
                comandoSql.CommandType = System.Data.CommandType.StoredProcedure;
                comandoSql.Parameters.Add(new SqlParameter(Constant.StrWorkItemIdOriginal, workItemIdOriginal));
                comandoSql.Parameters.Add(new SqlParameter(Constant.StrNewWorkItemId, newWorkItemId));
                comandoSql.Parameters.Add(new SqlParameter(Constant.StrTcgJson, tcgJson));
                comandoSql.Parameters.Add(new SqlParameter(Constant.StrFisaClassifiedRequest, strFisaClassifiedRequest));
                comandoSql.Parameters.Add(new SqlParameter(Constant.StrFisaClassifiedResponse, strFisaClassifiedResponse));
                comandoSql.Parameters.Add(new SqlParameter(Constant.StrFisaValidatedRequest, strFisaValidatedRequest));
                comandoSql.Parameters.Add(new SqlParameter(Constant.StrFisaValidatedResponse, strFisaValidatedResponse));
                dataReader = comandoSql.ExecuteReader();

                int convertCode;
                ErrorHandler errorHandler = new ErrorHandler();
                while (dataReader.Read())
                {
                    if (int.TryParse(dataReader[Constant.ErrorCode].ToString(), out convertCode))
                    {
                        errorHandler.ErrorCode = convertCode;
                        errorHandler.MessageError = dataReader[Constant.ErrorMessage].ToString();
                    }
                    else
                    {
                        errorHandler.ErrorCode = Constant.NegativeOne;
                        errorHandler.MessageError = string.Empty;
                    }
                }
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        if (errorHandler.ErrorCode == Constant.Zero)
                        {
                            response = dataReader["response"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = "";
            }
            return response;
        }

        private int GetTypeDocumentParity(int classificationId)
        {
            System.Data.SqlClient.SqlCommand comandoSql;
            SqlConnection conexion;
            System.Data.SqlClient.SqlDataReader dataReader;
            int configTypeDoc = 0;

            conexion = new Conection(_iconfiguration).SqlEntregaConexion(_iconfiguration, Constant.ConnectionType.IntegrationBusiness);

            try
            {
                conexion.Open();
                comandoSql = new SqlCommand(Constant.SpGetTypeDocumentParity, conexion);
                comandoSql.CommandType = System.Data.CommandType.StoredProcedure;
                comandoSql.Parameters.Add(new SqlParameter(Constant.StrClasificationId, classificationId));
                dataReader = comandoSql.ExecuteReader();

                while (dataReader.Read())
                {
                    configTypeDoc = Int32.Parse(dataReader[Constant.ClassificationId].ToString());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return configTypeDoc;
        }
    }
}
