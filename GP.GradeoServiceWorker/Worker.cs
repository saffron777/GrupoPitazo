using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GP.Core.Models;
using GP.Core.Modules.App.Interface;
using GP.Core.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace GP.GradeoServiceWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly IConfiguration _configuration;
        //private readonly IAppServices _appServices;
        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            //_appServices = appServices;

            Utils.settings = new AppSettings
            {
                usuario = _configuration.GetSection("appSettings").GetSection("usuario").Value,
                clave = _configuration.GetSection("appSettings").GetSection("clave").Value,
                clavecliente = _configuration.GetSection("appSettings").GetSection("clavecliente").Value,
                password = _configuration.GetSection("appSettings").GetSection("password").Value,
                SecurityKey = _configuration.GetSection("appSettings").GetSection("SecurityKey").Value,
                urlLoginGCIT = _configuration.GetSection("appSettings").GetSection("urlLoginGCIT").Value,
                urlWSA = _configuration.GetSection("appSettings").GetSection("urlWSA").Value,
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                Log.Information("Worker running at: {time}", DateTimeOffset.Now);
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false);
                var configuration = builder.Build();
                string connectionString = configuration.GetConnectionString("AppContext");
                decimal Comision;

                try
                {  
                    StringBuilder sql = new StringBuilder();

                    sql.Append("[dbo].[SP_GradeoList]");

                    using (var cn = new SqlConnection(connectionString))
                    {
                        cn.Open();

                        SqlCommand cmdp = cn.CreateCommand();
                        cmdp.CommandType = System.Data.CommandType.Text;
                        cmdp.CommandText = "SELECT Description FROM Parameters WHERE TableId='GENERAL' AND Semantic='COMISION'";
                        var comisionp = cmdp.ExecuteScalar() as string;

                        decimal.TryParse(comisionp, out Comision);
                        //var tran = cn.BeginTransaction();
                        StringBuilder sqlCmd = new StringBuilder();

                        SqlCommand cmd = cn.CreateCommand();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandText = sql.ToString();
                        //cmd.Transaction = tran;
                        SqlDataReader reader = cmd.ExecuteReader();

                        string resultado;
                        string usuario;
                        decimal monto;
                        Int64 jugadaID;
                        string ticket;
                        int aceptaciones;
                        string detalle = "";
                        string NombreEjemplar = "";
                        while (reader.Read())
                        {

                            resultado = reader["resultado"].ToString();
                            usuario = reader["Usuario"].ToString();
                            ticket = reader["JugadaID"].ToString();
                            jugadaID = Int64.Parse(reader["JugadaID"].ToString());
                            NombreEjemplar = reader["NombreEjemplar"].ToString();
                            detalle = reader["HipodromoID"].ToString() + "/" + reader["NumeroCarrera"].ToString() + "/" + NombreEjemplar + "/" + reader["TipoJugadas"].ToString();

                            //if (reader["AceptacionID"].ToString() != "0" && reader["AceptacionID"].ToString() != "")
                            //    ticket += "-" + reader["AceptacionID"].ToString();

                            decimal.TryParse(reader["Monto"].ToString(), out monto);
                            int.TryParse(reader["aceptaciones"].ToString(), out aceptaciones);

                            
                            int trans = 0;
                            string mensaje = "";
                            decimal factor = 1;
                            //cmd2.Transaction = tran;

                            try
                            {
                                
                                switch (resultado)
                                {
                                    case "GANADOR":
                                        trans = 1;
                                        mensaje = "Acreditar Usuario GANADOR";
                                        if (aceptaciones > 0)
                                        {
                                            AsignarGradeo(cn, factor, Comision, monto, jugadaID, ticket, usuario, detalle, resultado, mensaje, trans);
                                        }
                                        
                                        break;
                                    case "GANADORMITAD":
                                        trans = 1;
                                        mensaje = "Acreditar Usuario GANADOR";
                                        monto = monto / 2;
                                        factor = 0.5M;
                                        if (aceptaciones > 0)
                                        {
                                            AsignarGradeo(cn, factor, Comision, monto, jugadaID, ticket, usuario, detalle, resultado, mensaje, trans);
                                        }


                                        break;
                                    case "PERDEDOR":
                                        mensaje = "Debitar Usuario PERDEDOR";
                                        trans = 2;
                                        factor =1;
                                        if (aceptaciones > 0)
                                        {
                                            AsignarGradeo(cn, factor, Comision, monto, jugadaID, ticket, usuario, detalle, resultado, mensaje, trans);
                                        }
                                        break;
                                    case "PERDEDORMITAD":
                                        trans = 2;
                                        mensaje = "Debitar Usuario PERDEDOR";
                                        monto = monto / 2;
                                        factor = 0.5M;
                                        if (aceptaciones > 0)
                                        {
                                            AsignarGradeo(cn, factor, Comision, monto, jugadaID, ticket, usuario, detalle, resultado, mensaje, trans);
                                        }
                                        break;
                                    case "PUSH":
                                        mensaje = "Devolucion dinero Usuario PUSH";
                                        trans = 1;
                                        if (aceptaciones > 0)
                                        {
                                            AsignarGradeo(cn, factor, 0, monto, jugadaID, ticket, usuario, detalle, resultado, mensaje, trans);
                                        }
                                        break;
                                    default:
                                        mensaje = "Devolucion dinero Usuario ANULADO";
                                        trans = 1;

                                        sqlCmd.Clear();
                                        sqlCmd.Append("INSERT INTO [dbo].[Gradeo] ");
                                        sqlCmd.Append("([Ticket] ");
                                        sqlCmd.Append(",[Usuario] ");
                                        sqlCmd.Append(",[Tipo] ");
                                        sqlCmd.Append(",[Detalle] ");
                                        sqlCmd.Append(",[Monto] ");
                                        sqlCmd.Append(",[Comision] ");
                                        sqlCmd.Append(",[Status] ");
                                        sqlCmd.Append(",[Fecha] ");
                                        sqlCmd.Append(",[Activo]) ");
                                        sqlCmd.Append("VALUES ");
                                        sqlCmd.Append("( @Ticket ");
                                        sqlCmd.Append(", @usuario ");
                                        sqlCmd.Append(", 'JUGAR' ");
                                        sqlCmd.Append(", @Detalle ");
                                        sqlCmd.Append(", @Monto ");
                                        sqlCmd.Append(", @Comision ");
                                        sqlCmd.Append(", @Status ");
                                        sqlCmd.Append(", GETDATE()");
                                        sqlCmd.Append(", 1)");

                                        var result = GP.Core.Utilities.Utils.RegistrarMonto(monto.ToString().Replace('.', ','), trans, usuario, mensaje);

                                        if (result < 0)
                                            throw new Exception("Error al registrar el monto");

                                        SqlCommand cmd2 = cn.CreateCommand();
                                        cmd2.CommandText = sqlCmd.ToString();

                                        cmd2.Parameters.Add(new SqlParameter("@Ticket", ticket));
                                        cmd2.Parameters.Add(new SqlParameter("@usuario", usuario));
                                        cmd2.Parameters.Add(new SqlParameter("@Detalle", detalle));
                                        cmd2.Parameters.Add("@Monto", System.Data.SqlDbType.Decimal, 18).Value = monto;
                                        cmd2.Parameters.Add("@Comision", System.Data.SqlDbType.Decimal, 18).Value = 0;
                                        cmd2.Parameters.Add(new SqlParameter("@Status", resultado.StartsWith("GANADOR") ? "GANADOR" : (resultado.StartsWith("PERDEDOR") ? "PERDEDOR" : resultado)));
                                        var res1 = cmd2.ExecuteNonQuery();

                                        break;
                                }                               
                               

                                sqlCmd.Clear();

                                sqlCmd.Append("UPDATE Jugadas SET Status = '" + (resultado.StartsWith("GANADOR") ? "GANADOR" : (resultado.StartsWith("PERDEDOR") ? "PERDEDOR" : resultado)) + "' WHERE JugadaID = " + jugadaID);

                                SqlCommand cmd3 = cn.CreateCommand();
                                cmd3.CommandText = sqlCmd.ToString();
                                //cmd3.Transaction = tran;

                                var resu = cmd3.ExecuteNonQuery();

                                Log.Information("Gradeo exitoso {time} usuario:" + usuario + ", ticket:" + ticket, DateTimeOffset.Now);
                                //tran.Commit();
                            }
                            catch (Exception ex)
                            {
                                //tran.Rollback();
                                _logger.LogError(ex.Message);
                                Log.Error("Worker error at: {time} " + ex.Message, DateTimeOffset.Now);
                            }



                        }

                        cn.Close();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    Log.Error("Worker error at: {time} " + ex.Message, DateTimeOffset.Now);
                }
                
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void AsignarGradeo(SqlConnection cn, decimal factor, decimal Comision, decimal monto, long jugadaID, string ticket, string usuario, string detalle, string resultado, string mensaje, int trans)
        {
            SqlCommand sqlCommand = cn.CreateCommand();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            StringBuilder sqlCmd = new StringBuilder();

            int result = -1;

            decimal montoTotalAceptaciones = 0;
            decimal ComisionTotal = 0;

            try
            {
                sqlCommand.CommandText = $"SELECT * FROM Aceptaciones WHERE JugadaID = {jugadaID}";
                da.SelectCommand = sqlCommand;
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        sqlCmd.Clear();
                        sqlCmd.Append("INSERT INTO [dbo].[Gradeo] ");
                        sqlCmd.Append("([Ticket] ");
                        sqlCmd.Append(",[Usuario] ");
                        sqlCmd.Append(",[Tipo] ");
                        sqlCmd.Append(",[Detalle] ");
                        sqlCmd.Append(",[Monto] ");
                        sqlCmd.Append(",[Comision] ");
                        sqlCmd.Append(",[Status] ");
                        sqlCmd.Append(",[Fecha] ");
                        sqlCmd.Append(",[Activo]) ");
                        sqlCmd.Append("VALUES ");
                        sqlCmd.Append("( @Ticket ");
                        sqlCmd.Append(", @usuario ");
                        sqlCmd.Append(", 'JUGAR' ");
                        sqlCmd.Append(", @Detalle ");
                        sqlCmd.Append(", @Monto ");
                        sqlCmd.Append(", @Comision ");
                        sqlCmd.Append(", @Status ");
                        sqlCmd.Append(", GETDATE()");
                        sqlCmd.Append(", 1)");

                        string ticketA = item["JugadaID"].ToString() + "-" + item["AceptacionID"].ToString();

                        decimal montoAcept;
                        decimal montoContra;

                        decimal.TryParse(item["Monto"].ToString(), out montoAcept);
                        decimal.TryParse(item["Monto"].ToString(), out montoContra);
                        string usuarioContraparte = item["Usuario"].ToString();

                        if (resultado.StartsWith("PERDEDOR"))
                            montoAcept = montoAcept * factor * (1 - (Comision / 100));
                        else if (resultado.StartsWith("GANADOR"))
                            montoAcept = montoAcept * factor * (1 - (Comision / 100));
                        else
                            montoAcept = monto;

                        if (resultado.StartsWith("PUSH"))
                            result =  GP.Core.Utilities.Utils.RegistrarMonto(montoAcept.ToString().Replace('.', ','), 1, usuarioContraparte, mensaje);
                        else
                        {
                            if (resultado.StartsWith("PERDEDOR"))
                            {
                                result = GP.Core.Utilities.Utils.RegistrarMonto((montoAcept + montoContra).ToString().Replace('.', ','), 1 , usuarioContraparte, "Acreditar Usuario GANADOR");
                            }
                            else
                                result = GP.Core.Utilities.Utils.RegistrarMonto(montoAcept.ToString().Replace('.', ','), 2 , usuarioContraparte,  "Debitar Usuario PERDEDOR" );
                        }



                        if (result < 0)
                            throw new Exception("Error al registrar el monto");

                        SqlCommand cmdg = cn.CreateCommand();
                        cmdg.CommandText = sqlCmd.ToString();

                        cmdg.Parameters.Add(new SqlParameter("@Ticket", ticketA));
                        cmdg.Parameters.Add(new SqlParameter("@usuario", usuarioContraparte));
                        cmdg.Parameters.Add(new SqlParameter("@Detalle", detalle));
                        if (resultado.StartsWith("PERDEDOR"))
                            cmdg.Parameters.Add("@Monto", System.Data.SqlDbType.Decimal, 18).Value = montoAcept + montoContra;
                        else
                            cmdg.Parameters.Add("@Monto", System.Data.SqlDbType.Decimal, 18).Value = montoAcept ;

                        if (resultado.StartsWith("PUSH"))
                            cmdg.Parameters.Add("@Comision", System.Data.SqlDbType.Decimal, 18).Value = 0;
                        else
                        {
                            if (resultado.StartsWith("PERDEDOR"))
                            {
                                ComisionTotal = (montoAcept + montoContra) * (Comision / 100);
                                cmdg.Parameters.Add("@Comision", System.Data.SqlDbType.Decimal, 18).Value = ComisionTotal;
                                cmdg.Parameters.Add(new SqlParameter("@Status", "GANADOR" ));
                            }
                            else
                            {
                                cmdg.Parameters.Add("@Comision", System.Data.SqlDbType.Decimal, 18).Value = 0;
                                cmdg.Parameters.Add(new SqlParameter("@Status", "PERDEDOR"));
                            }
                        }
                        
                        var resg = cmdg.ExecuteNonQuery();

                        Log.Information("Gradeo exitoso {time} usuario:" + usuarioContraparte + ", ticket:" + ticket, DateTimeOffset.Now);

                        montoTotalAceptaciones += montoAcept;
                    }

                    sqlCmd.Clear();
                    sqlCmd.Append("INSERT INTO [dbo].[Gradeo] ");
                    sqlCmd.Append("([Ticket] ");
                    sqlCmd.Append(",[Usuario] ");
                    sqlCmd.Append(",[Tipo] ");
                    sqlCmd.Append(",[Detalle] ");
                    sqlCmd.Append(",[Monto] ");
                    sqlCmd.Append(",[Comision] ");
                    sqlCmd.Append(",[Status] ");
                    sqlCmd.Append(",[Fecha] ");
                    sqlCmd.Append(",[Activo]) ");
                    sqlCmd.Append("VALUES ");
                    sqlCmd.Append("( @Ticket ");
                    sqlCmd.Append(", @usuario ");
                    sqlCmd.Append(", 'JUGAR' ");
                    sqlCmd.Append(", @Detalle ");
                    sqlCmd.Append(", @Monto ");
                    sqlCmd.Append(", @Comision ");
                    sqlCmd.Append(", @Status ");
                    sqlCmd.Append(", GETDATE()");
                    sqlCmd.Append(", 1)");

                    ComisionTotal = montoTotalAceptaciones * (Comision / 100);

                    montoTotalAceptaciones += monto;

                    if (resultado.StartsWith("PUSH"))
                        result = GP.Core.Utilities.Utils.RegistrarMonto(monto.ToString().Replace('.', ','), trans, usuario, mensaje);
                    else
                    {
                        if (resultado.StartsWith("PERDEDOR"))
                        {
                            result =  GP.Core.Utilities.Utils.RegistrarMonto(monto.ToString().Replace('.', ','), trans, usuario, mensaje);
                        }
                        else
                        {
                            result = GP.Core.Utilities.Utils.RegistrarMonto(montoTotalAceptaciones.ToString().Replace('.', ','), trans, usuario, mensaje);
                        }
                    }

                    if (result < 0)
                        throw new Exception("Error al registrar el monto");

                    SqlCommand cmd4 = cn.CreateCommand();
                    cmd4.CommandText = sqlCmd.ToString();

                    cmd4.Parameters.Add(new SqlParameter("@Ticket", ticket));
                    cmd4.Parameters.Add(new SqlParameter("@usuario", usuario));
                    cmd4.Parameters.Add(new SqlParameter("@Detalle", detalle));
                    cmd4.Parameters.Add("@Monto", System.Data.SqlDbType.Decimal, 18).Value = monto;

                    if (resultado.StartsWith("PUSH"))
                        cmd4.Parameters.Add("@Comision", System.Data.SqlDbType.Decimal, 18).Value = 0;
                    else
                    {
                        if (resultado.StartsWith("PERDEDOR"))
                            cmd4.Parameters.Add("@Comision", System.Data.SqlDbType.Decimal, 18).Value = 0;
                                else
                        cmd4.Parameters.Add("@Comision", System.Data.SqlDbType.Decimal, 18).Value = ComisionTotal;
                    }

                    cmd4.Parameters.Add(new SqlParameter("@Status", resultado.StartsWith("GANADOR") ? "GANADOR" : (resultado.StartsWith("PERDEDOR") ? "PERDEDOR" : resultado)));
                    var res = cmd4.ExecuteNonQuery();


                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
