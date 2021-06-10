using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace GP.Test
{
    [TestClass]
    public class CoreTest
    {
        public static IConfigurationRoot configuration;

        [TestMethod]
        public void GradeoProcesstest()
        {
            try
            {
                // Initialize serilog logger
                //Log.Logger = new LoggerConfiguration()
                //     //.WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                //     .MinimumLevel.Debug()
                //     .Enrich.FromLogContext()
                //     .CreateLogger();

                // Create service collection
                //Log.Information("Creating service collection");
                ServiceCollection serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);

                // Create service provider
                //Log.Information("Building service provider");
                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                StringBuilder sql = new StringBuilder();


                sql.Append("select * ");
                sql.Append(",resultado = ");
                sql.Append("case when TipoJugadas = '1P' then IIF(LlegadaEjemplar =1, 'GANADOR','PERDEDOR') ");
                sql.Append("when TipoJugadas = '2P' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=2, 'GANADOR','PERDEDOR') ");
                sql.Append("when TipoJugadas = '3P' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=3, 'GANADOR','PERDEDOR') ");
                sql.Append("when TipoJugadas = '4P' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=4, 'GANADOR','PERDEDOR') ");
                sql.Append("when TipoJugadas = '5P' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=5, 'GANADOR','PERDEDOR') ");
                sql.Append("when TipoJugadas = '1P2N' then IIF(LlegadaEjemplar =1, 'GANADOR', IIF(LlegadaEjemplar =2, 'PERDEDORMITAD', 'PERDEDOR')) ");
                sql.Append("when TipoJugadas = '2P2N' then IIF(LlegadaEjemplar =1, 'GANADOR', IIF(LlegadaEjemplar =2, 'GANADORMITAD', 'PERDEDOR')) ");
                sql.Append("when TipoJugadas = '2P3N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=2, 'GANADOR', IIF(LlegadaEjemplar =3, 'PERDEDORMITAD', 'PERDEDOR')) ");
                sql.Append("when TipoJugadas = '3P3N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=2, 'GANADOR', IIF(LlegadaEjemplar =3, 'GANADORMITAD', 'PERDEDOR')) ");
                sql.Append("when TipoJugadas = '3P4N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=3, 'GANADOR', IIF(LlegadaEjemplar =4, 'PERDEDORMITAD', 'PERDEDOR')) ");
                sql.Append("when TipoJugadas = '4P4N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=3, 'GANADOR', IIF(LlegadaEjemplar =4, 'GANADORMITAD', 'PERDEDOR')) ");
                sql.Append("when TipoJugadas = '4P5N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=4, 'GANADOR', IIF(LlegadaEjemplar =5, 'PERDEDORMITAD', 'PERDEDOR')) ");
                sql.Append("when TipoJugadas = '5P5N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=4, 'GANADOR', IIF(LlegadaEjemplar =5, 'GANADORMITAD', 'PERDEDOR')) ");
                sql.Append("when TipoJugadas = '2N' then IIF(LlegadaEjemplar =1, 'GANADOR',IIF(LlegadaEjemplar =2, 'PUSH', 'PERDEDOR')) ");
                sql.Append("when TipoJugadas = '3N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=2, 'GANADOR',IIF(LlegadaEjemplar =3, 'PUSH', 'PERDEDOR')) ");
                sql.Append("when TipoJugadas = '4N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=3, 'GANADOR',IIF(LlegadaEjemplar =4, 'PUSH', 'PERDEDOR')) ");
                sql.Append("when TipoJugadas = '5N' then IIF(LlegadaEjemplar >=1 AND LlegadaEjemplar <=4, 'GANADOR',IIF(LlegadaEjemplar =5, 'PUSH', 'PERDEDOR')) ");
                sql.Append("else 'PUSH' ");
                sql.Append("end ");
                sql.Append("from ( ");
                sql.Append("select ");
                sql.Append("c.HipodromoID, ");
                sql.Append("c.NumeroCarrera, ");
                sql.Append("j.JugadaID, ");
                sql.Append("AceptacionID = '', ");
                sql.Append("j.CarreraID, ");
                sql.Append("TipoJugadas = (select Codigo from TipoJugadas where TipoJugadaID = j.TipoJugadaID), ");
                sql.Append("j.NumeroEjemplar, ");
                sql.Append("NombreEjemplar = (select carr.NombreEjemplar from Carreras carr  where carr.HipodromoID = c.HipodromoID and carr.NumeroCarrera = j.NumeroCarrera and carr.NumeroEjemplar = j.NumeroEjemplar and CAST(carr.fechaCarrera as date) = CAST(getdate() as date)), ");
                sql.Append("c.LlegadaEjemplar, ");
                sql.Append("j.Usuario, ");
                sql.Append("j.Monto ");
                sql.Append("from carreras c ");
                sql.Append("inner join jugadas j on j.[CarreraID] = c.[CarreraID] ");
                sql.Append("where CAST(fechaCarrera as date) = CAST(getdate() as date) and CAST( FechaJugada as date) =  CAST(getdate() as date) and c.EstatusEjemplar = 1 and FechaCierreCarrera is not null and ([LlegadaEjemplar] is not null ) and j.Status not in ('GANADOR','PERDEDOR','PUSH') ");
                sql.Append("union all ");
                sql.Append("select ");
                sql.Append("c.HipodromoID, ");
                sql.Append("c.NumeroCarrera, ");
                sql.Append("j.JugadaID, ");
                sql.Append("a.AceptacionID, ");
                sql.Append("j.CarreraID, ");
                sql.Append("TipoJugadas = (select Codigo from TipoJugadas where TipoJugadaID = j.TipoJugadaID), ");
                sql.Append("j.NumeroEjemplar, ");
                sql.Append("NombreEjemplar = (select carr.NombreEjemplar from Carreras carr  where carr.HipodromoID = c.HipodromoID and carr.NumeroCarrera = j.NumeroCarrera and carr.NumeroEjemplar = j.NumeroEjemplar and CAST(carr.fechaCarrera as date) = CAST(getdate() as date)), ");
                sql.Append("c.LlegadaEjemplar, ");
                sql.Append("a.Usuario, ");
                sql.Append("a.Monto ");
                sql.Append("from carreras c ");
                sql.Append("inner join jugadas j on j.[CarreraID] = c.[CarreraID] ");
                sql.Append("inner join Aceptaciones a on j.[JugadaID] = a.[JugadaID] ");
                sql.Append("where CAST(fechaCarrera as date) = CAST(getdate() as date) and CAST( FechaJugada as date) =  CAST(getdate() as date) and CAST(Fecha as date) = CAST(getdate() as date) and c.EstatusEjemplar = 1 and FechaCierreCarrera is not null  and ([LlegadaEjemplar] is not null ) and j.Status not in ('GANADOR','PERDEDOR','PUSH') ");
                sql.Append(") tbl ");
                sql.Append("order by Usuario , HipodromoID ");

                string connectionString = "data source=FRANK-PC;initial catalog=GrupoPitazoDB;User=sa;PASSWORD=11742421;MultipleActiveResultSets=True";//configuration.GetConnectionString("AppContext");

                using (var cn = new SqlConnection(connectionString))
                {
                    cn.Open();
                    //var tran = cn.BeginTransaction();
                    StringBuilder sqlCmd = new StringBuilder();

                    SqlCommand cmd = cn.CreateCommand();
                    cmd.CommandText = sql.ToString();
                    //cmd.Transaction = tran;
                    SqlDataReader reader = cmd.ExecuteReader();

                    string resultado;
                    string usuario;
                    decimal monto;
                    Int64 jugadaID;
                    string ticket;
                    decimal Comision = 5;
                    string detalle = "";
                    string NombreEjemplar = "";
                    while (reader.Read())
                    {

                        resultado = reader["resultado"].ToString();
                        usuario = reader["Usuario"].ToString();
                        ticket = reader["JugadaID"].ToString() ;
                        jugadaID = Int64.Parse(reader["JugadaID"].ToString());
                        NombreEjemplar = reader["NombreEjemplar"].ToString();
                        detalle = reader["HipodromoID"].ToString() + "/" + reader["NumeroCarrera"].ToString() + "/" + NombreEjemplar  + "/" + reader["TipoJugadas"].ToString();

                        if (reader["AceptacionID"].ToString() != "0" && reader["AceptacionID"].ToString() != "")
                            ticket +="-" + reader["AceptacionID"].ToString();

                        decimal.TryParse(reader["Monto"].ToString(), out monto);
                        

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


                        int trans = 0;
                        string mensaje = "";
                        switch (resultado)
                        {
                            case "GANADOR":
                                trans = 1;
                                mensaje = "Acreditar Usuario GANADOR";
                                break;
                            case "GANADORMITAD":
                                trans = 1;
                                mensaje = "Acreditar Usuario GANADOR";
                                monto = monto / 2;
                                break;
                            case "PERDEDOR":
                                mensaje = "Debitar Usuario PERDEDOR";
                                trans = 2;
                                break;
                            case "PERDEDORMITAD":
                                trans = 2;
                                mensaje = "Debitar Usuario PERDEDOR";
                                monto = monto / 2;
                                break;
                            default:
                                mensaje = "Devolucion dinero Usuario PUSH";
                                trans = 1;
                                break;
                        }

                        Comision = monto * (decimal)((decimal)5 / (decimal)100);

                                                
                        SqlCommand cmd2 = cn.CreateCommand();
                        cmd2.CommandText = sqlCmd.ToString();
                        cmd2.Parameters.Add(new SqlParameter("@Ticket", ticket ));
                        cmd2.Parameters.Add(new SqlParameter("@usuario",usuario ));
                        cmd2.Parameters.Add(new SqlParameter("@Detalle", detalle ));
                        cmd2.Parameters.Add("@Monto", System.Data.SqlDbType.Decimal,18).Value = monto;
                        cmd2.Parameters.Add("@Comision", System.Data.SqlDbType.Decimal,18).Value = Comision;
                        cmd2.Parameters.Add(new SqlParameter("@Status", resultado.StartsWith("GANADOR") ? "GANADOR" : (resultado.StartsWith("PERDEDOR") ? "PERDEDOR" : resultado) ));


                        //cmd2.Transaction = tran;

                        try
                        {

                            //var result = await GP.Core.Utilities.Utils.RegistrarMonto(monto.ToString().Replace('.', ','), trans, usuario, mensaje);
                            GP.Core.Utilities.Utils.RegistrarMontoAsync(monto.ToString().Replace('.', ','), trans, usuario, mensaje);

                            var res = cmd2.ExecuteNonQuery();

                            sqlCmd.Clear();

                            sqlCmd.Append("UPDATE Jugadas SET Status = '" + (resultado.StartsWith("GANADOR") ? "GANADOR" : (resultado.StartsWith("PERDEDOR") ? "PERDEDOR" : resultado)) + "' WHERE JugadaID = " + jugadaID);

                            SqlCommand cmd3 = cn.CreateCommand();
                            cmd3.CommandText = sqlCmd.ToString();
                            //cmd3.Transaction = tran;

                            res = cmd3.ExecuteNonQuery();

                            //tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            //tran.Rollback();

                        }



                    }

                    cn.Close();
                }



            }
            catch (Exception ex)
            {

                
            }
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add logging
            //serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            //{
            //    builder.AddSerilog(dispose: true);
            //}));

            //serviceCollection.AddLogging();

            // Build configuration
            configuration = new ConfigurationBuilder { }
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);

            // Add app
            serviceCollection.AddTransient<CoreTest>();
        }
    }
}
