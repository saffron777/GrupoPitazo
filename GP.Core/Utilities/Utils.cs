using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WSA;
using GP.Core.Models;
using System.Globalization;
using System.Data;
using Dapper;

namespace GP.Core.Utilities
{
    public static class Utils
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //public static IOptions<AppSettings> settings;
        public static AppSettings settings;

        public static Int64 ExecuteOutputParam
            (this IDbConnection conn, string sql, object args)
        {
            // Stored procedures with output parameter require
            // dynamic params. This assumes the OUTPUT parameter in the
            // SQL is an INTEGER named @id.
            var p = new DynamicParameters();
            p.Add("id", dbType: DbType.Int64, direction: ParameterDirection.Output);

            var properties = args.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var key = prop.Name;
                var value = prop.GetValue(args);

                p.Add(key, value);
            }

            conn.Execute(sql, p);

            Int64 id = p.Get<Int64>("id");
            return id;
        }

        //public static List<object> ExecuteOutputParam
        //    (this IDbConnection conn, string sql, object args, object outputparams)
        //{
        //    // Stored procedures with output parameter require
        //    // dynamic params. This assumes the OUTPUT parameter in the
        //    // SQL is an INTEGER named @id.
        //    var p = new DynamicParameters();
        //    var props = outputparams.GetType().GetProperties();
        //    foreach (var prop in props)
        //    {


        //        p.Add(prop.Name, dbType: (DbType)prop.GetType(), direction: ParameterDirection.Output);
        //    }


                

        //    var properties = args.GetType().GetProperties();
        //    foreach (var prop in properties)
        //    {
        //        var key = prop.Name;
        //        var value = prop.GetValue(args);

        //        p.Add(key, value);
        //    }

        //    conn.Execute(sql, p);


        //    for (int i = 0; i < p.; i++)
        //    {

        //    }
        //    Int64 id = p.Get<Int64>("id");
        //    return id;
        //}

        public static string toHour12(this TimeSpan t)
        {
            DateTime time = DateTime.Today.Add(t);
            string displayTime = time.ToString("hh:mm tt", CultureInfo.InvariantCulture);

            return displayTime;
        }

        public static long ConvertToTimestamp(DateTime value)
        {
            TimeSpan elapsedTime = value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static LoginResponse GetLogin(int? pid)
        {

            //logsWriter("Inicio Login");
            string urllogin = settings.urlLoginGCIT;// ConfigurationManager.AppSettings["urlLoginGCIT"];
            string usr = settings.usuario;// ConfigurationManager.AppSettings["usuario"];
            string pwd = settings.password;// ConfigurationManager.AppSettings["password"];

            string url = $"{urllogin}?username={usr}&password={pwd}&idusuario={pid}";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.GET);

            var respuestaXml = client.Execute(request).Content;
            var serializer = new XmlSerializer(typeof(LoginResponse));
            LoginResponse result;


            using (TextReader reader = new StringReader(respuestaXml))
            {
                result = (LoginResponse)serializer.Deserialize(reader);
            }


            return result;
        }

        public static LoginResponse GetLogin(string login)
        {

            //logsWriter("Inicio Login");
            string urllogin = settings.urlLoginGCIT;// ConfigurationManager.AppSettings["urlLoginGCIT"];
            string usr = settings.usuario;// ConfigurationManager.AppSettings["usuario"];
            string pwd = settings.password;// ConfigurationManager.AppSettings["password"];

            string url = $"{urllogin}?username={usr}&password={pwd}&login={login}";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.GET);

            var respuestaXml = client.Execute(request).Content;
            var serializer = new XmlSerializer(typeof(LoginResponse));
            LoginResponse result;


            using (TextReader reader = new StringReader(respuestaXml))
            {
                result = (LoginResponse)serializer.Deserialize(reader);
            }


            return result;
        }

        public async static Task<string> SaldoDisponibleAsync(string usuario)
        {

            WSAdaptadorSoapClient client = new WSAdaptadorSoapClient(WSAdaptadorSoapClient.EndpointConfiguration.WSAdaptadorSoap);

            var response = await client.ConsultarSaldoAsync(new ConsultarSaldoRequest { clave= settings.clave, Clavecliente = settings.clavecliente, usuario = usuario });


            return response.ConsultarSaldoResult;

        }

        public static string SaldoDisponible(string usuario)
        {

            WSAdaptadorSoapClient client = new WSAdaptadorSoapClient(WSAdaptadorSoapClient.EndpointConfiguration.WSAdaptadorSoap);

            var response = client.ConsultarSaldo(new ConsultarSaldoRequest { clave = settings.clave, Clavecliente = settings.clavecliente, usuario = usuario });


            return response.ConsultarSaldoResult;

        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="monto"></param>
        ///// <param name="tipoTransaccion">1 - Deposito, 2 - Retiro, 3 - Apuesta Ganadora, 4 - Apuesta perdedora, 5 - Bono </param>
        ///// <param name="usuario"></param>
        ///// <returns></returns>
        public async static Task<int> RegistrarMontoAsync(string monto, int tipoTransaccion, string usuario, string msg = "")
        {
            WSAdaptadorSoapClient client = new WSAdaptadorSoapClient(WSAdaptadorSoapClient.EndpointConfiguration.WSAdaptadorSoap);

            RegistrarMontoDescripRequest request = new RegistrarMontoDescripRequest
            {
                 clave = settings.clave,
                  Clavecliente = settings.clavecliente,
                   tipoTransaccion = tipoTransaccion,
                    usuario = usuario,
                     monto = monto,
                      descripcion = $"Pitazo - {msg}"
            };

            var response = await client.RegistrarMontoDescripAsync(request);
            
            int trans = int.Parse(response.RegistrarMontoDescripResult);

            return trans;

        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="monto"></param>
        ///// <param name="tipoTransaccion">1 - Deposito, 2 - Retiro, 3 - Apuesta Ganadora, 4 - Apuesta perdedora, 5 - Bono </param>
        ///// <param name="usuario"></param>
        ///// <returns></returns>
        public static int RegistrarMonto(string monto, int tipoTransaccion, string usuario, string msg = "")
        {
            WSAdaptadorSoapClient client = new WSAdaptadorSoapClient(WSAdaptadorSoapClient.EndpointConfiguration.WSAdaptadorSoap);

            RegistrarMontoDescripRequest request = new RegistrarMontoDescripRequest
            {
                clave = settings.clave,
                Clavecliente = settings.clavecliente,
                tipoTransaccion = tipoTransaccion,
                usuario = usuario,
                monto = monto,
                descripcion = $"Pitazo - {msg}"
            };

            var response = client.RegistrarMontoDescrip(request);

            int trans = int.Parse(response.RegistrarMontoDescripResult);

            return trans;

        }
    }
}
