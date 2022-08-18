using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using RestSharp;
using Newtonsoft.Json;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class DocumentFileType
    {
        private static IConfiguration _iconfiguration;
        public static List<ResponseGetDictionaryData> GetDictionaryData(int IdDictionary)
        {
            List<ResponseGetDictionaryData> responseValue = new List<ResponseGetDictionaryData>();
            SqlCommand comandoSql = new SqlCommand();
            SqlConnection conexion = new SqlConnection();
            SqlDataReader dataReader;
            try
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                _iconfiguration = builder.Build();



                conexion = new Conection(_iconfiguration).SqlEntregaConexion(_iconfiguration, Constant.ConnectionType.GeneralInformation);
                conexion.Open();
                comandoSql = new SqlCommand(Constant.spGetDictionaryData, conexion);
                comandoSql.CommandType = System.Data.CommandType.StoredProcedure;
                comandoSql.Parameters.Add(new SqlParameter(Constant.IdDictionary, IdDictionary));
                comandoSql.Parameters.Add(new SqlParameter(Constant.InternalValue, string.Empty));
                comandoSql.Parameters.Add(new SqlParameter(Constant.IdValue, int.Parse(Constant.Zero.ToString())));
                comandoSql.Parameters.Add(new SqlParameter(Constant.IdValueFK, string.Empty));
                comandoSql.Parameters.Add(new SqlParameter(Constant.Year, int.Parse(Constant.Zero.ToString())));
                dataReader = comandoSql.ExecuteReader();

                while (dataReader.Read())
                {
                    ResponseGetDictionaryData masterDescAux = new ResponseGetDictionaryData();
                    masterDescAux.IdValue = int.Parse(dataReader[Constant.IdValuesp].ToString());
                    masterDescAux.InternalValuesp = dataReader[Constant.InternalValuesp].ToString();
                    masterDescAux.NameValue = dataReader[Constant.NameValuesp].ToString();
                    responseValue.Add(masterDescAux);
                }


                return responseValue;

            }
            catch (Exception ex)
            {

                return responseValue;
            }
            finally
            {
                comandoSql.Dispose();
                conexion.Dispose();
            }

        }
        public static List<LiftFile> SetLiftFile(int idRequest, int workitemid, int documentType, string documentName, string fileB64)
        {
            List<LiftFile> responseValue = new List<LiftFile>();
            SqlCommand comandoSql = new SqlCommand();
            SqlConnection conexion = new SqlConnection();
            SqlDataReader dataReader;
            try
            {
                var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                _iconfiguration = builder.Build();



                conexion = new Conection(_iconfiguration).SqlEntregaConexion(_iconfiguration, Constant.ConnectionType.IntegrationBusiness);
                conexion.Open();
                comandoSql = new SqlCommand(Constant.SpSetLiftFile, conexion);
                comandoSql.CommandType = System.Data.CommandType.StoredProcedure;
                comandoSql.Parameters.Add(new SqlParameter(Constant.IdRequest, idRequest));
                comandoSql.Parameters.Add(new SqlParameter(Constant.Workitemid, workitemid));
                comandoSql.Parameters.Add(new SqlParameter(Constant.DocType, documentType));
                comandoSql.Parameters.Add(new SqlParameter(Constant.DocName, documentName));
                comandoSql.Parameters.Add(new SqlParameter(Constant.FileB64, fileB64));
                dataReader = comandoSql.ExecuteReader();

                while (dataReader.Read())
                {
                    LiftFile liftFile = new LiftFile();
                    liftFile.idRequest = int.Parse(dataReader[Constant.StrIdRequest].ToString());
                    liftFile.workItemId = int.Parse(dataReader[Constant.StrWorkitemid].ToString());
                    liftFile.documentType = int.Parse(dataReader[Constant.StrDocType].ToString());
                    liftFile.documentName = dataReader[Constant.StrDocname].ToString();
                    liftFile.fileB64 = dataReader[Constant.StrFileB64].ToString();
                    responseValue.Add(liftFile);
                }


                return responseValue;

            }
            catch (Exception ex)
            {

                return responseValue;
            }
            finally
            {
                comandoSql.Dispose();
                conexion.Dispose();
            }

        }
        public static ResponseDataEnterFolder CallElectronicFolder(List<LiftFile> liftFiles)
        {
            List<LiftFile> responseValue = new List<LiftFile>();
            RequestEnterFolder requestEnterFolder = new RequestEnterFolder();
            List<Document> documents = new List<Document>();
            ResponseDataEnterFolder responseDataEnterFolder = new ResponseDataEnterFolder();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                _iconfiguration = builder.Build();
            try
            {
                requestEnterFolder.OperationCode = "100100010";
                requestEnterFolder.Dealer = "PruebaRECSA";
                requestEnterFolder.DealerRegionCode = "13";
                requestEnterFolder.ClientRut = "15825111-6";
                requestEnterFolder.CompanyName = "Prueba";
                requestEnterFolder.Email = "mmorales@mafchile.com";

                foreach (LiftFile liftFile in liftFiles)
                {
                    Document document = new Document();
                    document.Base64 = liftFile.fileB64;
                    document.FileExtension = Constant.PDF;
                    document.FileName = liftFile.documentName;
                    document.FileSize = Constant.MiloneThousandFifty.ToString();
                    document.FileType = liftFile.documentType.ToString();
                    documents.Add(document);
                }
                requestEnterFolder.Documents = documents;

                string urlLoginToken = _iconfiguration["Token"];
                string urlElectronicFolder = _iconfiguration["UrlElectronicFolder"];
                var clientRest = new RestClient(urlElectronicFolder);
                var requestClient = new RestRequest(RestSharp.Method.POST);
                requestClient.AddHeader("Authorization", "Bearer " + urlLoginToken);
                requestClient.AddJsonBody(requestEnterFolder);
                var restResponse = clientRest.Execute(requestClient);
                responseDataEnterFolder = JsonConvert.DeserializeObject<ResponseDataEnterFolder>(restResponse.Content.ToString());
            }
            catch (Exception ex)
            {
                responseDataEnterFolder.ServiceStatus = new ErrorHandler
                {
                    ErrorCode = -1,
                    MessageError = "Error: " + ex.Message
                };
            }
            return responseDataEnterFolder;
        }
    }
}
