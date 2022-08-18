using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class Conection
    {
        private string _connectionString;
        public Conection(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("ConnCredito");
        }

        private string cadenaConexion;
        private string enumString;
        SqlConnection conexionActiva;

        public System.Data.SqlClient.SqlConnection SqlEntregaConexion(IConfiguration iconfiguration, Constant.ConnectionType tipoConexionParam)
        {
            try
            {
                enumString = tipoConexionParam.ToString();
                _connectionString = iconfiguration.GetConnectionString(enumString);
                string asd = "pg49nVv92Me9m9LT5lq3DXWXsLtXk5HA4TaHD+Pn82DYwcp1Q9ifIdE6HPEUWM6e8zcDY8spgaNjdV6T6BUJlSUcY//314s2NOlEPG6ZkXopLLpI0MMXmUOcA+E6wH1Og2M1JKdPrYl7yc7fEccWkw==";
                cadenaConexion = Maf.Library.Core.Security.Cryptography.Decrypt(_connectionString);
                conexionActiva = new SqlConnection(cadenaConexion);
                return conexionActiva;
            }
            catch (Exception err)
            {
                throw;
            }
            finally
            {
                conexionActiva = null;
            }

        }
    }
}
