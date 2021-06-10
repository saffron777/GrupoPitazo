using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using GP.Application.Web.Models;
using GP.Core.Modules.BO.Interface;
using GP.Core.Modules.App.Interface;
using GP.Core.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using GP.Core.Modules.Account.Interface;
using System.Security.Claims;
using Nancy.Json;

namespace GP.Application.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBackOfficeServices _backOfficeServices;
        private readonly IAppServices _appServices;
        private readonly IAccountServices _accountServices;
        private IHttpContextAccessor _accessor;
        private readonly ISession session;
        private Dictionary<string, List<string>> jugadasList = new Dictionary<string, List<string>>();
        public HomeController(IBackOfficeServices backOfficeServices, IAppServices appServices, IAccountServices accountServices, IHttpContextAccessor accessor)
        {
            _backOfficeServices = backOfficeServices;
            _appServices = appServices;
            _accountServices = accountServices;
            _accessor = accessor;
            this.session = accessor.HttpContext.Session;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Inicio");
        }

        [HttpPost]
        public IActionResult Inicio(string id, string password/*,int idagente, string agente, string currency, string saldo,string comision*/ )
        {

            ClaimsIdentity identity = null;
            bool isAuthenticated = false;
            if (id != null && id != "0")
            {
                session.SetString("id", id);
                TempData["id"] = id;
            }

            if (password != null || password != "")
            {
                session.SetString("password", password);
                TempData["password"] = password;
            }

            string comision = _appServices.GetParametersByTableIdSemantic("GENERAL", "COMISION").Description;
            //if (currency != null && currency != "0")
            //{
            //    HttpContext.Session.SetString("currency", currency);
            //    ViewBag.currency = currency;
            //}
            //if (!string.IsNullOrEmpty(agente))
            //{
            //    HttpContext.Session.SetString("agente", agente);
            //    ViewBag.agente = agente;
            //}

            var login = _accountServices.GetUser(id, password);


            if (login != null && login.Existe != "0")
            {

                session.SetString("currency", login.Moneda);
                session.SetString("agente", login.Agente);
                session.SetInt32("idagente", int.Parse(login.IdAgente));
                session.SetString("saldo", login.BalanceDisponible);
                session.SetString("comision", comision);
                TempData["currency"] = login.Moneda;
                TempData["idagente"] = login.IdAgente;
                TempData["agente"] = login.Agente;


                //Create the identity for the user  
                identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, id),
                    new Claim(ClaimTypes.Role, "User")
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                isAuthenticated = true;

                decimal balance;

                decimal.TryParse(login.BalanceDisponible, out balance);

                var tran = new TransaccionesViewModel
                {
                    Usuario = id,
                    IdAgente = int.Parse(login.IdAgente),
                    Agente = login.Agente,
                    Moneda = login.Moneda,
                    IP = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    Token = null,
                    CarreraID = -1,
                    JugadaID = null,
                    BanqueadaID = null,
                    NotificacionID = null,
                    AceptacionID = null,
                    MontoJugada = null,
                    MontoBanqueada = null,
                    MontoAceptacion = null,
                    Balance = balance,
                    Accion = "LOGIN",
                    Descripcion = "usuario valido",
                    ErrorMensaje = "",
                    JsonRequest = "",
                    Fecha = DateTime.Now
                };

                _backOfficeServices.GuardarTransaccion(tran);
                TempData.Keep();
                return View("IndexV2");
            }
            else
            {
                var tran = new TransaccionesViewModel
                {
                    Usuario = id,
                    IdAgente = -1,
                    Agente = "",
                    Moneda = "",
                    IP = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    Token = null,
                    CarreraID = -1,
                    JugadaID = null,
                    BanqueadaID = null,
                    NotificacionID = null,
                    AceptacionID = null,
                    MontoJugada = null,
                    MontoBanqueada = null,
                    MontoAceptacion = null,
                    Balance = null,
                    Accion = "LOGIN",
                    Descripcion = "ERROR",
                    ErrorMensaje = "Usuario no existe o no es valido",
                    JsonRequest = "",
                    Fecha = DateTime.Now
                };

                _backOfficeServices.GuardarTransaccion(tran);
            }

            return View("Inicio");
        }

        public IActionResult CargarTodasJugadas()
        {
            var jugadas = _appServices.GetAllJugadasByFecha(DateTime.Now);

            jugadasList.Add(session.GetString("id"), jugadas.Select(s => s.CajaNro).ToList());
            session.SetObject("JugadasList_" + session.GetString("id"), jugadasList);

            return new JsonResult(jugadas);
        }

        public IActionResult CargarTodasJugadasConcretadas()
        {
            var jugadas = _appServices.GetAllJugadasByConcretadas(DateTime.Now);
            //jugadasList.Add(session.GetString("id"), jugadas.Select(s => s.CajaNro).ToList());
            //session.SetObject("JugadasList_" + session.GetString("id"), jugadasList);

            return new JsonResult(jugadas);
        }

        public IActionResult CargarTodasJugadasHipodromos(string hipodromoId)
        {
            var jugadas = _appServices.GetAllJugadasByHipodromo(hipodromoId, DateTime.Now);
            //jugadasList.Add(session.GetString("id"), jugadas.Select(s => s.CajaNro).ToList());
            //session.SetObject("JugadasList_" + session.GetString("id"), jugadasList);

            return new JsonResult(jugadas);
        }

        public IActionResult CargarMisJugadas(string usuario)
        {
            var jugadas = _appServices.GetJugadasByUsuarioFecha(usuario/*HttpContext.Session.GetString("id")*/, DateTime.Now);
            //jugadasList.Add(usuario, jugadas.Select(s => s.CajaNro).ToList());
            //session.SetObject("JugadasList_" + session.GetString("id"), jugadasList);

            return new JsonResult(jugadas);
        }

        public IActionResult CargarJugadasOtrosUsuarios(string usuario)
        {
            var jugadas = _appServices.GetJugadasByOtrosFecha(usuario/*HttpContext.Session.GetString("id")*/, DateTime.Now);
            //jugadasList.Add(usuario, jugadas.Select(s => s.CajaNro).ToList());
            //session.SetObject("JugadasList_" + session.GetString("id"), jugadasList);

            return new JsonResult(jugadas);
        }

        public IActionResult CargarNuevasJugadas(string hipodromoId, string usuario, string status, string filtro)
        {
            Dictionary<string, List<string>> jugadasList = session.GetObject<Dictionary<string, List<string>>>("JugadasList_" + session.GetString("id"));
            List<string> m_status = new List<string>();

            if (filtro == "ME")
            {
                m_status = status.Split(',').ToList();
            }

            var jugadas = _appServices.GetNewJugadas(jugadasList, hipodromoId, usuario, status, filtro, DateTime.Now);
            if (jugadas != null && jugadas.Count > 0)
            {
                jugadas.ForEach(f => { jugadasList[usuario].Add(f.CajaNro); });
                session.SetObject("JugadasList_" + session.GetString("id"), jugadasList);

                var jugadasFiltradas = jugadas.Where(exp => 
                    (!string.IsNullOrEmpty(hipodromoId) ? exp.Carrera.HipodromoID == hipodromoId : true)
                    && (filtro == "ACT" || filtro == "ME") ? (!string.IsNullOrEmpty(usuario) ? (filtro == "ACT" ? exp.Usuario != usuario : exp.Usuario == usuario) : true) : true
                    && (!string.IsNullOrEmpty(status) ? (filtro == "ME" ? m_status.Contains(exp.Status) : exp.Status == status) : true))
                    .ToList();

                jugadas = jugadasFiltradas;
            }

            return new JsonResult(jugadas);
        }

        public IActionResult LimpiarJugadasNoValidas(string hipodromoId, string usuario, string status, string filtro)
        {
            Dictionary<string, List<string>> jugadasList = session.GetObject<Dictionary<string, List<string>>>("JugadasList_" + session.GetString("id"));
            List<string> m_status = new List<string>();

            if (filtro == "ME")
            {
                m_status = status.Split(',').ToList();
            }

            var jugadas = _appServices.GetJugadasVencidas(jugadasList, hipodromoId, usuario, status, filtro, DateTime.Now);

            return new JsonResult(jugadas);
        }


        public IActionResult RefreshAceptaciones(string usuario)
        {
            Dictionary<string, List<string>> jugadasList = session.GetObject<Dictionary<string, List<string>>>("JugadasList_" + session.GetString("id"));

            var jugadas = _appServices.GetNewAceptacionesJugadas(jugadasList, usuario, DateTime.Now);
            return new JsonResult(jugadas);
        }
        public IActionResult CargarMisBanqueadas()
        {
            var banqueadas = _appServices.GetBanqueadasByUsuarioFecha(session.GetString("id"), DateTime.Now);

            return new JsonResult(banqueadas);
        }


        [HttpPost]
        public IActionResult CargarBanqueadaCardView(BanqueadaViewModel data)
        {
            //var rnd = new Random();

            //var nume = rnd.Next(1, 1000000);

            //var id = "TPJBAN_" + nume.ToString().PadLeft(7, '0');

            decimal montoAcept = data.Jugadas.Aceptaciones != null ? data.Jugadas.Aceptaciones.Sum(s => s.Monto) : 0;

            string status = montoAcept == data.Monto ? "COMPLETA" : "ENCURSO";

            var model = new CardViewModel
            {
                id = data.CajaNro,
                UserId = data.Usuario,
                HipodromoId = data.Jugadas.Carrera.HipodromoID,
                HipodromoName = _backOfficeServices.GetHipodromosByID(data.Jugadas.Carrera.HipodromoID).Nombre,
                CarreraId = data.Jugadas.CarreraID,
                JugadaId = data.JugadaID,
                BanquedaId = 0,
                CarreraNum = data.Jugadas.Carrera.NumeroCarrera.ToString(),
                EjemplarNum = data.NumeroEjemplar.ToString(),
                EjemplarName = _backOfficeServices.GetCarreraById(data.Jugadas.CarreraID).NombreEjemplar,
                TipoJugada = _backOfficeServices.GetTiposJugadasById(data.TipoJugada).Codigo,
                Monto = data.Monto,
                AceptacionesMonto = montoAcept,
                IdAgente = data.IdAgente,
                Agente = data.Agente,
                Moneda = data.Moneda,
                CajaNro = data.CajaNro,
                Status = status,
                FechaCarreraInfo = data.Jugadas.Carrera.FechaCarrera.ToString("yyyy/MM/dd") + " " + data.Jugadas.Carrera.HoraCarrera.ToString("hh:mm tt"),
                Aceptaciones = data.Jugadas.Aceptaciones
            };
            return PartialView("_banquearCardView", model);
        }

        [HttpPost]
        public IActionResult CargarJugadaCardView(JugadasViewModel data)
        {
            //var rnd = new Random();

            //var nume = rnd.Next(1, 1000000);

            //var id = "TPJJUG_" + nume.ToString().PadLeft(7, '0');

            decimal montoAcept = data.Aceptaciones != null ? data.Aceptaciones.Sum(s => s.Monto) : 0;

            string status = string.IsNullOrEmpty(data.Status) ? (montoAcept == data.Monto ? "COMPLETA" : "ENCURSO") : data.Status;
            var horaCarrera = (data.Carrera.HoraCarrera.Hours > 12 ? (data.Carrera.HoraCarrera.Hours - 12).ToString().PadLeft(2, '0') : data.Carrera.HoraCarrera.Hours.ToString().PadLeft(2, '0')) + ":" + data.Carrera.HoraCarrera.Minutes.ToString().PadLeft(2,'0') + " " + (data.Carrera.HoraCarrera.Hours >= 12 ? "PM": "AM"); 

            var model = new CardViewModel
            {
                id = data.CajaNro,
                UserId = data.Usuario,
                HipodromoId = data.Carrera.HipodromoID,
                HipodromoName = _backOfficeServices.GetHipodromosByID(data.Carrera.HipodromoID).Nombre,
                CarreraId = data.CarreraID,
                JugadaId = data.JugadaID,
                BanquedaId = 0,
                CarreraNum = data.Carrera.NumeroCarrera.ToString(),
                EjemplarNum = data.NumeroEjemplar.ToString(),
                EjemplarName = _appServices.GetEjemplarName(data.Carrera.HipodromoID, data.Carrera.NumeroCarrera, data.NumeroEjemplar),
                TipoJugada = _backOfficeServices.GetTiposJugadasById(data.TipoJugada).Codigo,
                Monto = data.Monto,
                AceptacionesMonto = montoAcept,
                IdAgente = data.IdAgente,
                Agente = data.Agente,
                Moneda = data.Moneda,
                CajaNro = data.CajaNro,
                Status = status,
                FechaCarreraInfo = data.Carrera.FechaCarrera.ToString("yyyy/MM/dd") + " " + horaCarrera,
                UsuarioApuesta = HttpContext.Session.GetString("id"),
                Aceptaciones = data.Aceptaciones
            };

            if (status == "ENCURSO")
                return PartialView("_jugarCardView", model);
            else if (status == "COMPLETA")
                return PartialView("_tomadoCardView", model);
            else
                return NoContent();

        }

        public IActionResult CargarLocacion()
        {
            var locacion = _backOfficeServices.GetLocacionSinResultados(DateTime.Now);

            return new JsonResult(locacion);
        }
        public IActionResult CargarHipodromos(string location)
        {
            var hipodromos = _backOfficeServices.GetHipodromosByLocacionSinResultados(DateTime.Now, location.Trim());

            return new JsonResult(hipodromos);
        }

        public IActionResult CargarHipodromosFilter()
        {
            var hipodromos = _appServices.GetHipodromosSinResultados(DateTime.Now);

            return new JsonResult(hipodromos);
        }

        public IActionResult CargarCarreras(string hipodromoId)
        {
            var carreras = _backOfficeServices.GetCarrerasSelect(DateTime.Now, hipodromoId.Trim(), false);

            return new JsonResult(carreras);
        }

        public IActionResult CargarEjemplares(string hipodromoId, int numCarrera)
        {
            var ejemplares = _backOfficeServices.GetEjemplaresActivos(hipodromoId.Trim(), numCarrera);

            return new JsonResult(ejemplares);
        }

        //public IActionResult CargarTipoJugadas()
        //{
        //    var tp = _backOfficeServices.GetTiposJugadas();

        //    return new JsonResult(tp);
        //}

        public IActionResult CargarTipoJugadas(string locacion)
        {
            var tp = _backOfficeServices.GetTiposJugadas();

            List<string> codigos = new List<string>();
            codigos.Add("5P");
            codigos.Add("5N");
            codigos.Add("5P5N");

            if (locacion.ToUpper() == "AMERICANA")
                tp = tp.Where(w => !codigos.Contains(w.Codigo)).ToList();

            return new JsonResult(tp);
        }

        [HttpPost]
        public async Task<IActionResult> Jugar(CardViewModel model)
        {

            //validar que la carrera este vigente, que la hora de la carrera sea mayor a la hora actual y no tenga fecha de cierre aun

            var carrerasList = _backOfficeServices.GetCarrerasByHipodromoNumeroFecha(DateTime.Now, model.HipodromoId, int.Parse(model.CarreraNum));


            var carrera = carrerasList.SingleOrDefault(s => s.CarreraID == model.CarreraId);//  _backOfficeServices.GetCarreraById(model.CarreraId);

            if (carrera == null)
                throw new Exception("La carrera no existe");

            if (carrera.FechaCierreCarrera != null || carrera.HoraCierreCarrera != null)
                throw new Exception("La carrera ya ha finalizado");

            if (carrera.FechaCarrera.Date != DateTime.Now.Date)
                throw new Exception("La carrera ya no es valida");

            if (carrera.HoraCarrera <= DateTime.Now.TimeOfDay)
                throw new Exception("La carrera ya ha comenzado");

            if (carrera.LlegadaEjemplar != null)
                throw new Exception("La carrera ya ha finalizado");

            int numEjemplar = int.Parse(model.EjemplarNum);

            var ejemplarValido = carrerasList.SingleOrDefault(s => s.NumeroEjemplar == numEjemplar)?.EstatusEjemplar;

            if (ejemplarValido == null || !ejemplarValido.Value)
                throw new Exception("El ejemplar ha sido retirado");

            //TODO:guardar en bd tabla Jugadas
            var data = new JugadasViewModel
            {
                CarreraID = model.CarreraId,
                NumeroEjemplar = int.Parse(model.EjemplarNum),
                NumeroCarrera = int.Parse(model.CarreraNum),
                TipoJugada = _backOfficeServices.GetTiposJugadasByCodigo(model.TipoJugada).TipoJugadaID,
                Monto = model.Monto,
                FechaJugada = DateTime.Now,
                Usuario = model.UserId,
                Agente = model.Agente,
                IdAgente = model.IdAgente,
                Moneda = model.Moneda,
                CajaNro = model.CajaNro,
                Status = model.Status
            };

            var json = new JavaScriptSerializer().Serialize(model);

            var result = await _backOfficeServices.GuardarJugada(data);

            if (result.Status == "OK")
            {
                model.AceptacionesMonto = _appServices.TotalAceptacionesPorJugada(model.UserId, model.CarreraId);
                model.JugadaId = Convert.ToInt64(result.Msg.Split('|')[0]);
                ViewBag.saldo = result.Msg.Split('|')[1];

                decimal balance;

                decimal.TryParse(result.Msg.Split('|')[1].Replace(".", ""), out balance);

                var tran = new TransaccionesViewModel
                {
                    Usuario = session.GetString("id"),
                    IdAgente = session.GetInt32("idagente").Value,
                    Agente = session.GetString("agente"),
                    Moneda = session.GetString("currency"),
                    IP = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    Token = null,
                    CarreraID = model.CarreraId,
                    JugadaID = model.JugadaId,
                    BanqueadaID = null,
                    NotificacionID = null,
                    AceptacionID = null,
                    MontoJugada = model.Monto,
                    MontoBanqueada = null,
                    MontoAceptacion = null,
                    Balance = balance,
                    Accion = "JUGAR",
                    Descripcion = "Registro de jugada exitoso",
                    ErrorMensaje = "",
                    JsonRequest = json,
                    Fecha = DateTime.Now
                };

                _backOfficeServices.GuardarTransaccion(tran);

                var jugadasList = session.GetObject<Dictionary<string, List<string>>>("JugadasList_" + session.GetString("id"));
                jugadasList[session.GetString("id")].Add(model.CajaNro);
                session.SetObject("JugadasList_" + session.GetString("id"), jugadasList);

                return PartialView("_jugarCardView", model);
            }
            else
            {
                ViewBag.Error = result.Msg;
                //return new ContentResult { Content = result.Msg, ContentType = "text/html" }; //NoContent();
                //return NoContent();
                var tran = new TransaccionesViewModel
                {
                    Usuario = session.GetString("id"),
                    IdAgente = session.GetInt32("idagente").Value,
                    Agente = session.GetString("agente"),
                    Moneda = session.GetString("currency"),
                    IP = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    Token = null,
                    CarreraID = model.CarreraId,
                    JugadaID = model.JugadaId,
                    BanqueadaID = null,
                    NotificacionID = null,
                    AceptacionID = null,
                    MontoJugada = model.Monto,
                    MontoBanqueada = null,
                    MontoAceptacion = null,
                    Balance = null,
                    Accion = "JUGAR",
                    Descripcion = "ERROR",
                    ErrorMensaje = result.Msg,
                    JsonRequest = json,
                    Fecha = DateTime.Now
                };

                _backOfficeServices.GuardarTransaccion(tran);

                throw new Exception(result.Msg);
            }
        }

        [HttpPost]
        public IActionResult Banquear(CardViewModel model)
        {
            //TODO:guardar en bd tabla Jugadas            
            var data = new BanqueadaViewModel
            {
                JugadaID = model.JugadaId,
                NumeroEjemplar = int.Parse(model.EjemplarNum),
                TipoJugada = _backOfficeServices.GetTiposJugadasByCodigo(model.TipoJugada).TipoJugadaID,
                Monto = model.Monto,
                FechaBanqueada = DateTime.Now,
                Usuario = model.UserId,
                Agente = model.Agente,
                IdAgente = model.IdAgente,
                Moneda = model.Moneda,
                CajaNro = model.CajaNro,
                Status = model.Status
            };

            var result = _backOfficeServices.GuardarBanqueada(data);

            if (result.Status == "OK")
            {
                model.AceptacionesMonto = _appServices.TotalAceptacionesPorBanqueda(model.UserId, model.BanquedaId);
                model.BanquedaId = Convert.ToInt64(result.Msg);
                return PartialView("_banquearCardView", model);
            }
            else
                return NoContent();
        }

        [HttpPost]
        public IActionResult Reporte(string usuario)
        {
            var model = _backOfficeServices.ReporteGradeo(usuario, DateTime.Now);

            return PartialView("_reporteView", model);
        }

        [HttpPost]
        public IActionResult MensajeResumen(string usuario)
        {
            var data = _appServices.GetJugadasByUsuarioFechaSinFiltro(usuario/*HttpContext.Session.GetString("id")*/, DateTime.Now)
                .Select(s => new MensajeViewModel
                {
                    TicketNro = s.JugadaID.ToString(),
                     CarreraId = s.CarreraID,
                    JugadaId = s.JugadaID,
                    Fecha = s.FechaJugada,
                    Status = s.Status,
                    HipodromoId = s.Carrera.HipodromoID,
                    Monto = s.Monto,
                    TipoJugada = _backOfficeServices.GetTiposJugadasById(s.TipoJugada).Codigo,
                    Aceptaciones = s.Aceptaciones
                })
                .Union(_appServices.GetAceptacionesByUserFecha(usuario/*HttpContext.Session.GetString("id")*/, DateTime.Now).Select(s => new MensajeViewModel
                {
                    TicketNro = s.JugadaID.ToString() + "-" + s.AceptacionID.ToString(),
                    CarreraId = s.CarreraId,
                    JugadaId = s.JugadaID.Value,
                    Fecha = s.Fecha,
                    Status = s.Status,
                    HipodromoId = s.HipodromoId,
                    Monto = s.Monto,
                    TipoJugada = _backOfficeServices.GetTiposJugadasById(_appServices.GetJugadaById(s.JugadaID.Value).TipoJugada).Codigo,
                    Aceptaciones = null
                })).ToList();

            for (int i = 0; i < data.Count(); i++)
            {
                if (data[i].HipodromoId == null)
                    data[i].HipodromoId = _appServices.GetCarreraInfoByJugadaId(data[i].JugadaId).HipodromoID;
            }

            return PartialView("_mensajesView", data);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarAceptacion(AceptacionesViewModels model)
        {
            if (model.Usuario != session.GetString("id"))
            {

                var carrerasList = _backOfficeServices.GetCarrerasByHipodromoNumeroFecha(DateTime.Now, model.HipodromoId, int.Parse(model.CarreraNum));
                //validar que la carrera este vigente, que la hora de la carrera sea mayor a la hora actual y no tenga fecha de cierre aun
                var carrera = carrerasList.SingleOrDefault(s => s.CarreraID == model.CarreraId);//_backOfficeServices.GetCarreraById(model.CarreraId);

                if (carrera == null)
                    throw new Exception("La carrera no existe");

                if (carrera.FechaCierreCarrera != null || carrera.HoraCierreCarrera != null)
                    throw new Exception("La carrera ya ha finalizado");

                if (carrera.FechaCarrera.Date != DateTime.Now.Date)
                    throw new Exception("La carrera ya no es valida");

                if (carrera.HoraCarrera <= DateTime.Now.TimeOfDay)
                    throw new Exception("La carrera ya ha comenzado");

                if (carrera.LlegadaEjemplar != null)
                    throw new Exception("La carrera ya ha finalizado");

                int numEjemplar = int.Parse(model.EjemplarNum);

                var ejemplarValido = carrerasList.SingleOrDefault(s => s.NumeroEjemplar == numEjemplar)?.EstatusEjemplar;

                if (ejemplarValido == null || !ejemplarValido.Value)
                    throw new Exception("El ejemplar ha sido retirado");

                var jugadaPrev = _appServices.GetJugadaById(model.JugadaID.Value);
                //validar si ya fue tomada y el monto es menor al monto jugado

                if (jugadaPrev == null)
                {
                    throw new Exception("No existe Jugada");
                }

                string statusJugadaPrev = jugadaPrev.Status;
                decimal factor = 1;
                decimal montoAceptJugadaPrev = jugadaPrev.Aceptaciones != null ? (jugadaPrev.Aceptaciones.Sum(x => x.Monto)) : 0;

                montoAceptJugadaPrev += model.Monto;

                switch (model.TipoJugada)
                {
                    case "10A2":
                        factor = 0.2M;
                        break;
                    case "10A3":
                        factor = 0.3M;
                        break;
                    case "10A4":
                        factor = 0.4M;
                        break;
                    case "10A5":
                        factor = 0.5M;
                        break;
                    case "10A6":
                        factor = 0.6M;
                        break;
                    case "10A7":
                        factor = 0.7M;
                        break;
                    case "10A8":
                        factor = 0.8M;
                        break;
                    case "10A9":
                        factor = 0.9M;
                        break;
                    default:
                        break;
                }

                string msg = "";

                if (statusJugadaPrev == "COMPLETA")
                {
                    msg = "La Jugada ya ha sido tomada previamente";
                    throw new Exception(msg);
                }
                else if (statusJugadaPrev.StartsWith("GANADOR") || statusJugadaPrev.StartsWith("PERDEDOR") || statusJugadaPrev.StartsWith("PUSH") || statusJugadaPrev.StartsWith("ANULADA"))
                {
                    msg = "La Jugada ya ha sido GRADEADA previamente";
                    throw new Exception(msg);
                }
                else if (montoAceptJugadaPrev > (model.MontoJugada * factor))
                {
                    msg = "La Jugada ha superado el monto maximo permitido, coloque un monto menor o igual a " + (montoAceptJugadaPrev - (model.MontoJugada * factor)).ToString();
                    throw new Exception(msg);
                }
                else if (factor != 1 && montoAceptJugadaPrev > (model.MontoJugada * factor))
                {
                    msg = "La Jugada ha superado el monto maximo permitido, coloque un monto menor o igual a " + (model.Monto - (model.MontoJugada * factor)).ToString();
                    throw new Exception(msg);
                }

                var result = await _appServices.GuardarAceptacion(model);

                var json = new JavaScriptSerializer().Serialize(model);

                if (result.Status == "OK")
                {
                    var horaCarrera = (carrera.HoraCarrera.Hours > 12 ? (carrera.HoraCarrera.Hours - 12).ToString().PadLeft(2, '0') : carrera.HoraCarrera.Hours.ToString().PadLeft(2, '0')) + ":" + carrera.HoraCarrera.Minutes.ToString().PadLeft(2, '0') + " " + (carrera.HoraCarrera.Hours >= 12 ? "PM" : "AM");

                    var data = new CardViewModel
                    {
                        id = model.CajaNro,
                        UserId = model.Usuario,
                        HipodromoId = model.HipodromoId,
                        HipodromoName = _backOfficeServices.GetHipodromosByID(model.HipodromoId).Nombre,
                        CarreraId = model.CarreraId,
                        JugadaId = model.JugadaID.Value,
                        BanquedaId = 0,
                        CarreraNum = model.CarreraNum,
                        EjemplarNum = model.EjemplarNum,
                        EjemplarName = _appServices.GetEjemplarName(model.HipodromoId, int.Parse(model.CarreraNum), int.Parse(model.EjemplarNum)), 
                        TipoJugada = model.TipoJugada,
                        Monto = model.MontoJugada,
                        AceptacionesMonto = _appServices.TotalAceptacionesPorJugada(model.UsuarioApuesta, model.JugadaID.Value),
                        IdAgente = model.IdAgente,
                        Agente = model.Agente,
                        Moneda = model.Moneda,
                        CajaNro = model.CajaNro,
                        UsuarioApuesta = model.UsuarioApuesta,
                        FechaCarreraInfo = carrera.FechaCarrera.ToString("yyyy/MM/dd") + " " + horaCarrera,
                        Aceptaciones = _appServices.GetAceptacionesByJugadas(model.JugadaID.Value)
                    };

                    decimal balance;

                    decimal.TryParse(result.Msg.Split('|')[2].Replace(".", ""), out balance);

                    long aceptacionId;

                    long.TryParse(result.Msg.Split('|')[0], out aceptacionId);

                    var tran = new TransaccionesViewModel
                    {
                        Usuario = session.GetString("id"),
                        IdAgente = session.GetInt32("idagente").Value,
                        Agente = session.GetString("agente"),
                        Moneda = session.GetString("currency"),
                        IP = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                        Token = null,
                        CarreraID = model.CarreraId,
                        JugadaID = model.JugadaID,
                        BanqueadaID = null,
                        NotificacionID = null,
                        AceptacionID = aceptacionId,
                        MontoJugada = model.MontoJugada,
                        MontoBanqueada = null,
                        MontoAceptacion = model.Monto,
                        Balance = balance,
                        Accion = "ACEPTACION",
                        Descripcion = "Registro de aceptacion exitoso",
                        ErrorMensaje = "",
                        JsonRequest = json,
                        Fecha = DateTime.Now
                    };

                    _backOfficeServices.GuardarTransaccion(tran);

                    string statusJugada = result.Msg.Split('|')[1];
                    ViewBag.saldo = result.Msg.Split('|')[2];
                    if (statusJugada == "COMPLETA")
                        return PartialView("_tomadoCardView", data);
                    else if (statusJugada == "ENCURSO")
                        return PartialView("_jugarCardView", data);
                    else
                        return NoContent();
                }
                else
                {
                    ViewBag.Error = result.Msg;

                    var tran = new TransaccionesViewModel
                    {
                        Usuario = session.GetString("id"),
                        IdAgente = session.GetInt32("idagente").Value,
                        Agente = session.GetString("agente"),
                        Moneda = session.GetString("currency"),
                        IP = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                        Token = null,
                        CarreraID = model.CarreraId,
                        JugadaID = model.JugadaID,
                        BanqueadaID = null,
                        NotificacionID = null,
                        AceptacionID = null,
                        MontoJugada = model.MontoJugada,
                        MontoBanqueada = null,
                        MontoAceptacion = model.Monto,
                        Balance = null,
                        Accion = "ACEPTACION",
                        Descripcion = "ERROR",
                        ErrorMensaje = result.Msg,
                        JsonRequest = json,
                        Fecha = DateTime.Now
                    };

                    _backOfficeServices.GuardarTransaccion(tran);

                    //return new ContentResult { Content = result.Msg, ContentType = "text/html" }; //NoContent();
                    throw new Exception(result.Msg);
                }
            }

            return NoContent();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logout()
        {

            var tran = new TransaccionesViewModel
            {
                Usuario = session.GetString("id"),
                IdAgente = session.GetInt32("idagente").Value,
                Agente = session.GetString("agente"),
                Moneda = session.GetString("currency"),
                IP = _accessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                Token = null,
                CarreraID = -1,
                JugadaID = null,
                BanqueadaID = null,
                NotificacionID = null,
                AceptacionID = null,
                MontoJugada = null,
                MontoBanqueada = null,
                MontoAceptacion = null,
                Balance = null,
                Accion = "LOGOUT",
                Descripcion = "usuario logout",
                ErrorMensaje = "",
                JsonRequest = "",
                Fecha = DateTime.Now
            };

            HttpContext.SignOutAsync();

            session.Clear();



            _backOfficeServices.GuardarTransaccion(tran);

            return View("Inicio");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
