using System;
using System.Collections.Generic;
using System.Text;
using GP.Core.Logging;
using GP.Core.Models;
using GP.Core.Utilities;
using GP.Core.Entities;
using GP.Core.Modules.App.Interface;
using GP.Core.Repository.Contracts;
using GP.Core.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Globalization;
using System.Data.SqlClient;

namespace GP.Core.Modules.App.Implementations
{
    public class AppServices : IAppServices
    {
        private readonly IBanqueadasRepository _banqueadasRepository;
        private readonly ICarrerasRepository _carrerasRepository;
        private readonly IHipodromosRepository _hipodromosRepository;
        private readonly IJugadasRepository _jugadasRepository;
        private readonly INotificacionesRepository _notificacionesRepository;
        private readonly ITipoJugadasRepository _tipoJugadasRepository;
        private readonly ITokensRepository _tokensRepository;
        private readonly ITransaccionesRepository _transaccionesRepository;
        private readonly IAceptacionesRepository _aceptacionesRepository;
        private readonly IParametersRepository _parametersRepository;

        private readonly IConfiguration _configuration;

        public AppServices(IConfiguration configuration, IBanqueadasRepository banqueadasRepository,
            ICarrerasRepository carrerasRepository, IHipodromosRepository hipodromosRepository,
            IJugadasRepository jugadasRepository, INotificacionesRepository notificacionesRepository,
            ITipoJugadasRepository tipoJugadasRepository, ITokensRepository tokensRepository, ITransaccionesRepository transaccionesRepository,
            IAceptacionesRepository aceptacionesRepository, IParametersRepository parametersRepository)
        {
            _configuration = configuration;
            _banqueadasRepository = banqueadasRepository;
            _carrerasRepository = carrerasRepository;
            _hipodromosRepository = hipodromosRepository;
            _jugadasRepository = jugadasRepository;
            _notificacionesRepository = notificacionesRepository;
            _tipoJugadasRepository = tipoJugadasRepository;
            _tokensRepository = tokensRepository;
            _transaccionesRepository = transaccionesRepository;
            _aceptacionesRepository = aceptacionesRepository;
            _parametersRepository = parametersRepository;

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

        public JugadasViewModel GetJugadaById(long jugadaId)
        {
            var jugada = _jugadasRepository.GetById(jugadaId);

            if (jugada != null)
            {
                return new JugadasViewModel
                {
                    JugadaID = jugada.JugadaID,
                    CarreraID = jugada.CarreraID,
                    CajaNro = jugada.CajaNro,
                    FechaJugada = jugada.FechaJugada,
                    Status = jugada.Status,
                    IdAgente = jugada.IdAgente,
                    Moneda = jugada.Moneda,
                    NumeroEjemplar = jugada.NumeroEjemplar,
                    Monto = jugada.Monto,
                    TipoJugada = jugada.TipoJugadaID,
                    Agente = jugada.Agente,
                    Usuario = jugada.Usuario,
                    Aceptaciones = GetAceptacionesByJugadas(jugada.JugadaID)
                };
            }

            return null;
        }

        public List<AceptacionesViewModels> GetAceptacionesByBanqueadas(long banqueadaId)
        {
            return _aceptacionesRepository.AllNoTracking(x => x.BanqueadaID == banqueadaId && x.Activo).Select(
                s => new AceptacionesViewModels
                {
                    AceptacionID = s.AceptacionID,
                    JugadaID = s.JugadaID,
                    //BanqueadaID = s.BanqueadaID,
                    Usuario = s.Usuario,
                    CajaNro = s.CajaNro,
                    Monto = s.Monto,
                    Fecha = s.Fecha,
                    Status = s.Status,

                }
                ).ToList();
        }

        public List<AceptacionesViewModels> GetAceptacionesByJugadas(long jugadaId)
        {
            return _aceptacionesRepository.AllNoTracking(x => x.JugadaID == jugadaId && x.Activo).Select(
                s => new AceptacionesViewModels
                {
                    AceptacionID = s.AceptacionID,
                    JugadaID = s.JugadaID,
                    //BanqueadaID = s.BanqueadaID,
                    Usuario = s.Usuario,
                    CajaNro = s.CajaNro,
                    Monto = s.Monto,
                    Fecha = s.Fecha,
                    Status = s.Status,

                }
                ).ToList();
        }

        public List<AceptacionesViewModels> GetAceptacionesByUser(string usuario)
        {
            return _aceptacionesRepository.AllNoTracking(x => x.Usuario == usuario && x.Activo).Select(
                s => new AceptacionesViewModels
                {
                    AceptacionID = s.AceptacionID,
                    JugadaID = s.JugadaID,
                    //BanqueadaID = s.BanqueadaID,
                    Usuario = s.Usuario,
                    CajaNro = s.CajaNro,
                    Monto = s.Monto,
                    Fecha = s.Fecha,
                    Status = s.Status,

                }
                ).ToList();
        }

        public CarrerasViewModel GetCarreraInfoByJugadaId(long jugadaId)
        {
            var data = _jugadasRepository.AllNoTracking(x => x.JugadaID == jugadaId)
                .Join(_carrerasRepository.AllNoTracking(), j => j.CarreraID, c => c.CarreraID, (j, c) => new { jugada = j, carrera = c })
                .Select(s => new CarrerasViewModel
                {
                    CarreraID = s.carrera.CarreraID,
                    HipodromoID = s.carrera.HipodromoID,
                    EstatusEjemplar = s.carrera.EstatusEjemplar,
                    FechaCarrera = s.carrera.FechaCarrera,
                    FechaCierreCarrera = s.carrera.FechaCierreCarrera,
                    HoraCarrera = s.carrera.HoraCarrera,
                    HoraCierreCarrera = s.carrera.HoraCierreCarrera,
                    LlegadaEjemplar = s.carrera.LlegadaEjemplar,
                    //NombreEjemplar = GetEjemplarName(s.carrera.HipodromoID, s.jugada.NumeroCarrera.Value, s.jugada.NumeroEjemplar),
                    NumeroCarrera = s.jugada.NumeroCarrera.Value,
                    NumeroEjemplar = s.jugada.NumeroEjemplar
                }).FirstOrDefault();

            return data;
        }

        public List<AceptacionesViewModels> GetAceptacionesByUserFecha(string usuario, DateTime fecha)
        {
            return _aceptacionesRepository.AllNoTracking(x => x.Usuario == usuario && x.Fecha.Date == fecha.Date && x.Activo).Select(
                s => new AceptacionesViewModels
                {
                    AceptacionID = s.AceptacionID,
                    JugadaID = s.JugadaID,                    
                    //HipodromoId = GetJugadaById(s.JugadaID.Value).Carrera.HipodromoID,
                    //BanqueadaID = s.BanqueadaID,
                    Usuario = s.Usuario,
                    CajaNro = s.CajaNro,
                    Monto = s.Monto,
                    Fecha = s.Fecha,
                    Status = s.Status,

                }
                ).ToList();
        }

        public List<AceptacionesViewModels> GetAceptacionesByUsuarioBanquedas(string usuario, long banqueadaId)
        {
            return _aceptacionesRepository.AllNoTracking(x => x.Usuario == usuario && x.BanqueadaID == banqueadaId && x.Activo).Select(
                s => new AceptacionesViewModels
                {
                    AceptacionID = s.AceptacionID,
                    JugadaID = s.JugadaID,
                    //BanqueadaID = s.BanqueadaID,
                    Usuario = s.Usuario,
                    CajaNro = s.CajaNro,
                    Monto = s.Monto,
                    Fecha = s.Fecha,
                    Status = s.Status,

                }
                ).ToList();
        }

        public List<AceptacionesViewModels> GetAceptacionesByUsuarioJugadas(string usuario, long jugadaId)
        {
            return _aceptacionesRepository.AllNoTracking(x => x.Usuario == usuario && x.JugadaID == jugadaId && x.Activo).Select(
                s => new AceptacionesViewModels
                {
                    AceptacionID = s.AceptacionID,
                    JugadaID = s.JugadaID,
                    //BanqueadaID = s.BanqueadaID,
                    Usuario = s.Usuario,
                    CajaNro = s.CajaNro,
                    Monto = s.Monto,
                    Fecha = s.Fecha,
                    Status = s.Status,

                }
                ).ToList();
        }

        public List<HipodromosViewModels> GetHipodromosSinResultados(DateTime fecha)
        {

            var hipodromos = _hipodromosRepository.AllNoTracking()
                .Join(_carrerasRepository.AllNoTracking(), hip => hip.HipodromoID, carr => carr.HipodromoID, (hip, carr) => new { Hipodromos = hip, Carreras = carr })
                .Where(w => w.Carreras.LlegadaEjemplar == null && w.Carreras.FechaCarrera.Date == fecha.Date && w.Carreras.HoraCarrera > DateTime.Now.TimeOfDay)
                .Select(s => new HipodromosViewModels
                {
                    HipodromoID = s.Hipodromos.HipodromoID,
                    Nombre = s.Hipodromos.Nombre,
                    Clasificacion = s.Hipodromos.Clasificacion,
                    Locacion = s.Hipodromos.Locacion
                }).Distinct();

            var result = hipodromos.ToList();

            return result;
        }

        public string GetEjemplarName(string hipodromoId, int numeroCarrera, int numeroEjemplar)
        {
            return _carrerasRepository.AllNoTracking(exp => exp.HipodromoID == hipodromoId
            && exp.NumeroCarrera == numeroCarrera && exp.NumeroEjemplar == numeroEjemplar && exp.FechaCarrera.Date == DateTime.Now.Date
            && exp.HoraCarrera > DateTime.Now.TimeOfDay && exp.EstatusEjemplar == true && exp.LlegadaEjemplar == null && exp.FechaCierreCarrera == null).SingleOrDefault()?.NombreEjemplar;
            
        }
        public decimal TotalAceptacionesPorJugada(string usuario, long jugadaId)
        {
            return _aceptacionesRepository.AllNoTracking(x => x.Usuario == usuario && x.JugadaID == jugadaId && x.Activo).Sum(s => s.Monto);
        }

        public decimal TotalAceptacionesPorBanqueda(string usuario, long banqueadaId)
        {
            return _aceptacionesRepository.AllNoTracking(x => x.Usuario == usuario && x.BanqueadaID == banqueadaId && x.Activo).Sum(s => s.Monto);
        }

        public List<JugadasViewModel> GetNewAceptacionesJugadas(Dictionary<string, List<string>> jugadasList, string usuario, DateTime fecha)
        {
            List<string> m_status = new List<string>();

            //if (filtro == "ME")
            //{
            //    m_status = status.Split(',').ToList();
            //}

            var jugadasCargadas = jugadasList[usuario];

            //Expression<Func<Jugadas, bool>> expression = exp =>
            //(!string.IsNullOrEmpty(hipodromoId) ? exp.Carreras.HipodromoID == hipodromoId : true)
            //&& (filtro == "ACT" || filtro == "ME") ? (!string.IsNullOrEmpty(usuario) ? (filtro == "ACT" ? exp.Usuario != usuario : exp.Usuario == usuario) : true) : true
            //&& (!string.IsNullOrEmpty(status) ? (filtro == "ME" ? m_status.Contains(exp.Status) : exp.Status == status) : true)
            //&& jugadasCargadas.Contains(exp.CajaNro)
            //&& exp.FechaJugada.Date == fecha.Date;

            var jugadas = _jugadasRepository.AllNoTracking(exp => exp.FechaJugada.Date == fecha.Date && exp.Carreras.HoraCarrera > DateTime.Now.TimeOfDay && exp.Carreras.LlegadaEjemplar == null);
            var aceptaciones = _aceptacionesRepository.AllNoTracking(x => jugadasCargadas.Contains(x.CajaNro));

            var data = from j in jugadas
                       join a in aceptaciones on j.JugadaID equals a.JugadaID
                       orderby j.JugadaID descending
                       select new JugadasViewModel
                       {
                           JugadaID = j.JugadaID,
                           CarreraID = j.CarreraID,
                           NumeroEjemplar = j.NumeroEjemplar,
                           TipoJugada = j.TipoJugadaID,
                           Monto = j.Monto,
                           FechaJugada = j.FechaJugada,
                           Agente = j.Agente,
                           Usuario = j.Usuario,
                           IdAgente = j.IdAgente,
                           Moneda = j.Moneda,
                           CajaNro = j.CajaNro,
                           Status = j.Status,
                           Carrera = new CarrerasViewModel
                           {
                               CarreraID = j.Carreras.CarreraID,
                               HipodromoID = j.Carreras.HipodromoID,
                               NumeroCarrera = j.Carreras.NumeroCarrera,
                               NombreEjemplar = j.Carreras.NombreEjemplar,
                               NumeroEjemplar = j.Carreras.NumeroCarrera,
                               FechaCarrera = j.Carreras.FechaCarrera,
                               HoraCarrera = j.Carreras.HoraCarrera
                           },
                           Aceptaciones = j.Aceptaciones.Select(x => new AceptacionesViewModels
                           {
                               AceptacionID = x.AceptacionID,
                               JugadaID = x.JugadaID,
                               TipoJugada = j.TipoJugadas.Codigo,
                               //BanqueadaID = x.BanqueadaID,
                               Usuario = x.Usuario,
                               Monto = x.Monto,
                               Fecha = x.Fecha,
                               CajaNro = x.CajaNro,
                               Status = x.Status,
                               Activo = x.Activo
                           }).ToList()
                       };
            //.Join(_aceptacionesRepository.AllNoTracking().ToList(), jug => jug.JugadaID, acep => acep;


            var result = data.ToList();
            return result;
        }

        public List<string> GetJugadasVencidas(Dictionary<string, List<string>> jugadasList, string hipodromoId, string usuario, string status, string filtro, DateTime fecha)
        {
            var jugadasCargadas = jugadasList[usuario];

            var data = _jugadasRepository.AllNoTracking(exp =>            
            jugadasCargadas.Contains(exp.CajaNro.Substring(0, 36))
            && exp.FechaJugada.Date >= fecha.Date
            && exp.Carreras.HoraCierreCarrera <= DateTime.Now.TimeOfDay 
            && exp.Carreras.LlegadaEjemplar != null)
            .Select (s => s.CajaNro.Substring(0,36))
            .ToList();

            return data;
        }

        public List<JugadasViewModel> GetNewJugadas(Dictionary<string, List<string>> jugadasList, string hipodromoId, string usuario, string status, string filtro, DateTime fecha)
        {
            List<string> m_status = new List<string>();

            if (filtro == "ME")
            {
                m_status = status.Split(',').ToList();
            }

            var jugadasCargadas = jugadasList[usuario];
           

            var data = _jugadasRepository.AllNoTracking(exp =>
            //(!string.IsNullOrEmpty(hipodromoId) ? exp.Carreras.HipodromoID == hipodromoId : true)
            // && (filtro == "ACT" || filtro == "ME") ? (!string.IsNullOrEmpty(usuario) ? (filtro == "ACT" ? exp.Usuario != usuario : exp.Usuario == usuario) : true) : true
            //&& (!string.IsNullOrEmpty(status) ? (filtro == "ME" ? m_status.Contains(exp.Status) : exp.Status == status) : true)            
            !jugadasCargadas.Contains(exp.CajaNro)
            && exp.FechaJugada.Date == fecha.Date
            && exp.Carreras.HoraCarrera > DateTime.Now.TimeOfDay && exp.Carreras.LlegadaEjemplar == null)
                .Select(s => new JugadasViewModel
                {
                    JugadaID = s.JugadaID,
                    CarreraID = s.CarreraID,
                    NumeroEjemplar = s.NumeroEjemplar,
                    TipoJugada = s.TipoJugadaID,
                    Monto = s.Monto,
                    FechaJugada = s.FechaJugada,
                    Agente = s.Agente,
                    Usuario = s.Usuario,
                    IdAgente = s.IdAgente,
                    Moneda = s.Moneda,
                    CajaNro = s.CajaNro,
                    Status = s.Status,
                    Carrera = new CarrerasViewModel
                    {
                        CarreraID = s.Carreras.CarreraID,
                        HipodromoID = s.Carreras.HipodromoID,
                        NumeroCarrera = s.Carreras.NumeroCarrera,
                        NombreEjemplar = s.Carreras.NombreEjemplar,
                        NumeroEjemplar = s.Carreras.NumeroCarrera,
                        FechaCarrera = s.Carreras.FechaCarrera,
                        HoraCarrera = s.Carreras.HoraCarrera
                    },
                    Aceptaciones = s.Aceptaciones.Select(x => new AceptacionesViewModels
                    {
                        AceptacionID = x.AceptacionID,
                        JugadaID = x.JugadaID,
                        TipoJugada = s.TipoJugadas.Codigo,
                        //BanqueadaID = x.BanqueadaID,
                        Usuario = x.Usuario,
                        Monto = x.Monto,
                        Fecha = x.Fecha,
                        CajaNro = x.CajaNro,
                        Status = x.Status,
                        Activo = x.Activo
                    }).ToList()
                })
                .OrderByDescending(o => o.JugadaID);         

            var result = data.ToList();
            return result;


        }


        public List<JugadasViewModel> GetAllJugadasByHipodromo(string hipodromoId, DateTime fecha)
        {
            var data = _jugadasRepository.AllNoTracking(x => x.FechaJugada.Date == fecha.Date && x.Carreras.HipodromoID == hipodromoId && x.Carreras.HoraCarrera > DateTime.Now.TimeOfDay && x.Carreras.LlegadaEjemplar == null && (x.Status == "ENCURSO"))
                .Select(s => new JugadasViewModel
                {
                    JugadaID = s.JugadaID,
                    CarreraID = s.CarreraID,
                    NumeroEjemplar = s.NumeroEjemplar,
                    TipoJugada = s.TipoJugadaID,
                    Monto = s.Monto,
                    FechaJugada = s.FechaJugada,
                    Agente = s.Agente,
                    Usuario = s.Usuario,
                    IdAgente = s.IdAgente,
                    Moneda = s.Moneda,
                    CajaNro = s.CajaNro,
                    Status = s.Status,
                    Carrera = new CarrerasViewModel
                    {
                        CarreraID = s.Carreras.CarreraID,
                        HipodromoID = s.Carreras.HipodromoID,
                        NumeroCarrera = s.Carreras.NumeroCarrera,
                        NombreEjemplar = s.Carreras.NombreEjemplar,
                        NumeroEjemplar = s.Carreras.NumeroCarrera,
                        FechaCarrera = s.Carreras.FechaCarrera,
                        HoraCarrera = s.Carreras.HoraCarrera
                    },
                    Aceptaciones = s.Aceptaciones.Select(x => new AceptacionesViewModels
                    {
                        AceptacionID = x.AceptacionID,
                        JugadaID = x.JugadaID,
                        TipoJugada = s.TipoJugadas.Codigo,
                        //BanqueadaID = x.BanqueadaID,
                        Usuario = x.Usuario,
                        Monto = x.Monto,
                        Fecha = x.Fecha,
                        CajaNro = x.CajaNro,
                        Status = x.Status,
                        Activo = x.Activo
                    }).ToList()
                }).OrderByDescending(o => o.JugadaID);

            return data.ToList();
        }

        public List<JugadasViewModel> GetAllJugadasByConcretadas(DateTime fecha)
        {
            var data = _jugadasRepository.AllNoTracking(x => x.FechaJugada.Date == fecha.Date && x.Carreras.HoraCarrera > DateTime.Now.TimeOfDay && x.Carreras.LlegadaEjemplar == null && (x.Status == "COMPLETA"))
                .Select(s => new JugadasViewModel
                {
                    JugadaID = s.JugadaID,
                    CarreraID = s.CarreraID,
                    NumeroEjemplar = s.NumeroEjemplar,
                    TipoJugada = s.TipoJugadaID,
                    Monto = s.Monto,
                    FechaJugada = s.FechaJugada,
                    Agente = s.Agente,
                    Usuario = s.Usuario,
                    IdAgente = s.IdAgente,
                    Moneda = s.Moneda,
                    CajaNro = s.CajaNro,
                    Status = s.Status,
                    Carrera = new CarrerasViewModel
                    {
                        CarreraID = s.Carreras.CarreraID,
                        HipodromoID = s.Carreras.HipodromoID,
                        NumeroCarrera = s.Carreras.NumeroCarrera,
                        NombreEjemplar = s.Carreras.NombreEjemplar,
                        NumeroEjemplar = s.Carreras.NumeroCarrera,
                        FechaCarrera = s.Carreras.FechaCarrera,
                        HoraCarrera = s.Carreras.HoraCarrera
                    },
                    Aceptaciones = s.Aceptaciones.Select(x => new AceptacionesViewModels
                    {
                        AceptacionID = x.AceptacionID,
                        JugadaID = x.JugadaID,
                        TipoJugada = s.TipoJugadas.Codigo,
                        //BanqueadaID = x.BanqueadaID,
                        Usuario = x.Usuario,
                        Monto = x.Monto,
                        Fecha = x.Fecha,
                        CajaNro = x.CajaNro,
                        Status = x.Status,
                        Activo = x.Activo
                    }).ToList()
                }).OrderByDescending(o => o.JugadaID);

            return data.ToList();
        }


        public List<JugadasViewModel> GetAllJugadasByFecha(DateTime fecha)
        {

            var data = _jugadasRepository.AllNoTracking(x => x.FechaJugada.Date == fecha.Date && x.Carreras.HoraCarrera > DateTime.Now.TimeOfDay && x.Carreras.LlegadaEjemplar == null && (x.Status == "ENCURSO"))
                .Select(s => new JugadasViewModel
                {
                    JugadaID = s.JugadaID,
                    CarreraID = s.CarreraID,
                    NumeroEjemplar = s.NumeroEjemplar,
                    TipoJugada = s.TipoJugadaID,
                    Monto = s.Monto,
                    FechaJugada = s.FechaJugada,
                    Agente = s.Agente,
                    Usuario = s.Usuario,
                    IdAgente = s.IdAgente,
                    Moneda = s.Moneda,
                    CajaNro = s.CajaNro,
                    Status = s.Status,
                    Carrera = new CarrerasViewModel
                    {
                        CarreraID = s.Carreras.CarreraID,
                        HipodromoID = s.Carreras.HipodromoID,
                        NumeroCarrera = s.Carreras.NumeroCarrera,
                        NombreEjemplar = s.Carreras.NombreEjemplar,
                        NumeroEjemplar = s.Carreras.NumeroCarrera,
                        FechaCarrera = s.Carreras.FechaCarrera,
                        HoraCarrera = s.Carreras.HoraCarrera
                    },
                    Aceptaciones = s.Aceptaciones.Select(x => new AceptacionesViewModels
                    {
                        AceptacionID = x.AceptacionID,
                        JugadaID = x.JugadaID,
                        TipoJugada = s.TipoJugadas.Codigo,
                        //BanqueadaID = x.BanqueadaID,
                        Usuario = x.Usuario,
                        Monto = x.Monto,
                        Fecha = x.Fecha,
                        CajaNro = x.CajaNro,
                        Status = x.Status,
                        Activo = x.Activo
                    }).ToList()
                }).OrderByDescending(o => o.JugadaID);

            return data.ToList();
        }

        public List<JugadasViewModel> GetJugadasByUsuarioFechaSinFiltro(string usuario, DateTime fecha)
        {

            var data = _jugadasRepository.AllNoTracking(x => x.Usuario == usuario && x.FechaJugada.Date == fecha.Date )
                .Select(s => new JugadasViewModel
                {
                    JugadaID = s.JugadaID,
                    CarreraID = s.CarreraID,
                    NumeroEjemplar = s.NumeroEjemplar,
                    TipoJugada = s.TipoJugadaID,
                    Monto = s.Monto,
                    FechaJugada = s.FechaJugada,
                    Agente = s.Agente,
                    Usuario = s.Usuario,
                    IdAgente = s.IdAgente,
                    Moneda = s.Moneda,
                    CajaNro = s.CajaNro,
                    Status = s.Status,
                    Carrera = new CarrerasViewModel
                    {
                        CarreraID = s.Carreras.CarreraID,
                        HipodromoID = s.Carreras.HipodromoID,
                        NumeroCarrera = s.Carreras.NumeroCarrera,
                        NombreEjemplar = s.Carreras.NombreEjemplar,
                        NumeroEjemplar = s.Carreras.NumeroCarrera,
                        FechaCarrera = s.Carreras.FechaCarrera,
                        HoraCarrera = s.Carreras.HoraCarrera
                    },
                    Aceptaciones = s.Aceptaciones.Select(x => new AceptacionesViewModels
                    {
                        AceptacionID = x.AceptacionID,
                        JugadaID = x.JugadaID,
                        TipoJugada = s.TipoJugadas.Codigo,
                        //BanqueadaID = x.BanqueadaID,
                        Usuario = x.Usuario,
                        Monto = x.Monto,
                        Fecha = x.Fecha,
                        CajaNro = x.CajaNro,
                        Status = x.Status,
                        Activo = x.Activo
                    }).ToList()
                }).OrderByDescending(o => o.JugadaID);

            return data.ToList();
        }

        public List<JugadasViewModel> GetJugadasByUsuarioFecha(string usuario, DateTime fecha)
        {

            var data = _jugadasRepository.AllNoTracking(x => x.Usuario == usuario && x.FechaJugada.Date == fecha.Date && x.Carreras.HoraCarrera > DateTime.Now.TimeOfDay && x.Carreras.LlegadaEjemplar == null && (x.Status == "ENCURSO" || x.Status == "COMPLETA"))
                .Select(s => new JugadasViewModel
                {
                    JugadaID = s.JugadaID,
                    CarreraID = s.CarreraID,
                    NumeroEjemplar = s.NumeroEjemplar,
                    TipoJugada = s.TipoJugadaID,
                    Monto = s.Monto,
                    FechaJugada = s.FechaJugada,
                    Agente = s.Agente,
                    Usuario = s.Usuario,
                    IdAgente = s.IdAgente,
                    Moneda = s.Moneda,
                    CajaNro = s.CajaNro,
                    Status = s.Status,
                    Carrera = new CarrerasViewModel
                    {
                        CarreraID = s.Carreras.CarreraID,
                        HipodromoID = s.Carreras.HipodromoID,
                        NumeroCarrera = s.Carreras.NumeroCarrera,
                        NombreEjemplar = s.Carreras.NombreEjemplar,
                        NumeroEjemplar = s.Carreras.NumeroCarrera,
                        FechaCarrera = s.Carreras.FechaCarrera,
                        HoraCarrera = s.Carreras.HoraCarrera
                    },
                    Aceptaciones = s.Aceptaciones.Select(x => new AceptacionesViewModels
                    {
                        AceptacionID = x.AceptacionID,
                        JugadaID = x.JugadaID,
                        TipoJugada = s.TipoJugadas.Codigo,
                        //BanqueadaID = x.BanqueadaID,
                        Usuario = x.Usuario,
                        Monto = x.Monto,
                        Fecha = x.Fecha,
                        CajaNro = x.CajaNro,
                        Status = x.Status,
                        Activo = x.Activo
                    }).ToList()
                }).OrderByDescending(o => o.JugadaID);

            return data.ToList();
        }

        public List<JugadasViewModel> GetJugadasByOtrosFecha(string usuario, DateTime fecha)
        {

            var data = _jugadasRepository.AllNoTracking(x => x.Usuario != usuario && x.FechaJugada.Date == fecha.Date && x.Carreras.HoraCarrera > DateTime.Now.TimeOfDay && x.Carreras.LlegadaEjemplar == null && (x.Status == "ENCURSO"))
                .Select(s => new JugadasViewModel
                {
                    JugadaID = s.JugadaID,
                    CarreraID = s.CarreraID,
                    NumeroEjemplar = s.NumeroEjemplar,
                    TipoJugada = s.TipoJugadaID,
                    Monto = s.Monto,
                    FechaJugada = s.FechaJugada,
                    Agente = s.Agente,
                    Usuario = s.Usuario,
                    IdAgente = s.IdAgente,
                    Moneda = s.Moneda,
                    CajaNro = s.CajaNro,
                    Status = s.Status,
                    Carrera = new CarrerasViewModel
                    {
                        CarreraID = s.Carreras.CarreraID,
                        HipodromoID = s.Carreras.HipodromoID,
                        NumeroCarrera = s.Carreras.NumeroCarrera,
                        NombreEjemplar = s.Carreras.NombreEjemplar,
                        NumeroEjemplar = s.Carreras.NumeroCarrera,
                        FechaCarrera = s.Carreras.FechaCarrera,
                        HoraCarrera = s.Carreras.HoraCarrera
                    },
                    Aceptaciones = s.Aceptaciones.Select(x => new AceptacionesViewModels
                    {
                        AceptacionID = x.AceptacionID,
                        JugadaID = x.JugadaID,
                        //BanqueadaID = x.BanqueadaID,
                        Usuario = x.Usuario,
                        Monto = x.Monto,
                        Fecha = x.Fecha,
                        CajaNro = x.CajaNro,
                        Status = x.Status,
                        Activo = x.Activo
                    }).ToList()
                })
                .OrderByDescending(o => o.JugadaID)
                .ToList();

            return data;
        }

        public List<BanqueadaViewModel> GetBanqueadasByUsuarioFecha(string usuario, DateTime fecha)
        {

            var data = _banqueadasRepository.AllNoTracking(x => x.Usuario == usuario && x.FechaBanqueada.Date == fecha.Date)
                .Select(s => new BanqueadaViewModel
                {
                    JugadaID = s.JugadaID,
                    BanqueadaID = s.BanquedaID,
                    NumeroEjemplar = s.NumeroEjemplar,
                    TipoJugada = s.TipoJugadaID,
                    Monto = s.Monto,
                    FechaBanqueada = s.FechaBanqueada,
                    Agente = s.Agente,
                    Usuario = s.Usuario,
                    IdAgente = s.IdAgente,
                    Moneda = s.Moneda,
                    CajaNro = s.CajaNro,
                    Status = s.Status,
                    Jugadas = new JugadasViewModel
                    {
                        JugadaID = s.Jugadas.JugadaID,
                        CarreraID = s.Jugadas.CarreraID,
                        NumeroEjemplar = s.Jugadas.NumeroEjemplar,
                        TipoJugada = s.Jugadas.TipoJugadaID,
                        Monto = s.Jugadas.Monto,
                        FechaJugada = s.Jugadas.FechaJugada,
                        Agente = s.Jugadas.Agente,
                        Usuario = s.Jugadas.Usuario,
                        IdAgente = s.Jugadas.IdAgente,
                        Moneda = s.Jugadas.Moneda,
                        CajaNro = s.CajaNro,
                        Carrera = new CarrerasViewModel
                        {
                            CarreraID = s.Jugadas.Carreras.CarreraID,
                            HipodromoID = s.Jugadas.Carreras.HipodromoID,
                            NumeroCarrera = s.Jugadas.Carreras.NumeroCarrera,
                            NombreEjemplar = s.Jugadas.Carreras.NombreEjemplar,
                            NumeroEjemplar = s.Jugadas.Carreras.NumeroCarrera,
                            FechaCarrera = s.Jugadas.Carreras.FechaCarrera,
                            HoraCarrera = s.Jugadas.Carreras.HoraCarrera
                        },

                        Aceptaciones = s.Aceptaciones.Select(x => new AceptacionesViewModels
                        {
                            AceptacionID = x.AceptacionID,
                            JugadaID = x.JugadaID,
                            //BanqueadaID = x.BanqueadaID,
                            Usuario = x.Usuario,
                            Monto = x.Monto,
                            Fecha = x.Fecha,
                            CajaNro = x.CajaNro,
                            Status = x.Status,
                            Activo = x.Activo
                        }).ToList()

                    },

                })
                .ToList();

            return data;
        }

        public async Task<Result> GuardarAceptacion(AceptacionesViewModels data)
        {
            Result res = null;

            try
            {
                //var aceptacionesNew = new Aceptaciones
                //{
                //    JugadaID = data.JugadaID,
                //    BanqueadaID = data.BanqueadaID,
                //    Usuario = data.Usuario,
                //    CajaNro = data.CajaNro,
                //    Monto = data.Monto,
                //    Fecha = data.Fecha,
                //    Status = data.Status,
                //    Activo = data.Activo
                //};

                //_aceptacionesRepository.Create(aceptacionesNew);
                //_aceptacionesRepository.Commit();

                //long aceptacionId = _aceptacionesRepository.AllNoTracking().Max(x => x.AceptacionID);

                //verificamos el saldo dispnible
                var saldo = await Utils.SaldoDisponibleAsync(data.UsuarioApuesta);

                decimal saldoDisp;
                decimal factor = 1;

                decimal.TryParse(saldo, out saldoDisp);

                if (saldoDisp <= 0)
                {
                    throw new Exception("El cliente no dispone de saldo suficiente");
                }


                string sql = "INSERT INTO [dbo].[Aceptaciones] ([JugadaID],[BanquedaID],[Usuario],[Monto],[Status],[Fecha],[CajaNro],[Activo]) ";
                sql += "VALUES (@JugadaID,@BanquedaID,@Usuario,@Monto,@Status,@Fecha,@CajaNro,@Activo) ";
                sql += "SELECT @id = SCOPE_IDENTITY()";

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter { ParameterName = "@JugadaID", SqlDbType = System.Data.SqlDbType.BigInt, Value = data.JugadaID });
                parameters.Add(new SqlParameter { ParameterName = "@BanquedaID", SqlDbType = System.Data.SqlDbType.BigInt, Value = (data.BanqueadaID == 0 ? (object)DBNull.Value : data.BanqueadaID) });
                parameters.Add(new SqlParameter { ParameterName = "@Usuario", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = data.UsuarioApuesta });
                parameters.Add(new SqlParameter { ParameterName = "@Monto", SqlDbType = System.Data.SqlDbType.Decimal, Precision = 18, Scale = 2, Value = data.Monto });
                parameters.Add(new SqlParameter { ParameterName = "@Status", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = data.Status });
                parameters.Add(new SqlParameter { ParameterName = "@Fecha", SqlDbType = System.Data.SqlDbType.DateTime, Value = DateTime.Now });
                parameters.Add(new SqlParameter { ParameterName = "@CajaNro", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = data.CajaNro });
                parameters.Add(new SqlParameter { ParameterName = "@Activo", SqlDbType = System.Data.SqlDbType.Bit, Value = data.Activo });
                parameters.Add(new SqlParameter { ParameterName = "@id", SqlDbType = System.Data.SqlDbType.BigInt, Direction = System.Data.ParameterDirection.Output });

                var result = _aceptacionesRepository.ExecuteSQL(sql, parameters.ToArray());
                long aceptacionId = 0;
                aceptacionId = Convert.ToInt64(parameters.Single(x => x.ParameterName == "@id").Value);

                var jugada = _jugadasRepository.GetById(data.JugadaID);

                switch (data.TipoJugada)
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

                var montoAcumulado = GetAceptacionesByJugadas(data.JugadaID.Value).Sum(x => x.Monto);

                if (montoAcumulado == (data.MontoJugada * factor))
                {
                    if (jugada != null)
                    {
                        jugada.Status = "COMPLETA";
                        _jugadasRepository.Update(jugada);
                        _jugadasRepository.Commit();
                    }
                }

                int tipoTrans = 2;

                var trans = await Utils.RegistrarMontoAsync(data.Monto.ToString().Replace('.', ','), tipoTrans, data.UsuarioApuesta, "Apuesta - Descuento");

                int.TryParse(trans.ToString(), out int numtransaction);

                if (numtransaction <= 0)
                    throw new Exception("No se pudo realizar la transaccion.");


                saldo = await Utils.SaldoDisponibleAsync(data.UsuarioApuesta);
                decimal.TryParse(saldo, out saldoDisp);

                res = new Result { Codigo = 200, Msg = aceptacionId.ToString() + "|" + jugada.Status + "|" + saldoDisp.ToString("n"), Status = "OK" };
            }
            catch (Exception ex)
            {
                res = new Result { Codigo = 400, Msg = ex.Message, Status = "ERROR" };
            }


            //
            return res;
        }


        #region Parametros
        public ParametersViewModel GetParametersByTableIdSemantic(string tableId, string semantic)
        {
            return _parametersRepository.AllNoTracking(exp => exp.TableId == tableId && exp.Semantic == semantic)
                .Select(s => new ParametersViewModel
                {
                    ParameterId = s.ParameterId,
                    TableId = s.TableId,
                    ParameterCode = s.ParameterCode,
                    Description = s.Description,
                    SortOrder = s.SortOrder,
                    Semantic = s.Semantic
                })
                .SingleOrDefault();
        }

        public ParametersViewModel GetParametersByTableIdDescription(string tableId, string description)
        {
            return _parametersRepository.AllNoTracking(exp => exp.TableId == tableId && exp.Description == description)
                .Select(s => new ParametersViewModel
                {
                    ParameterId = s.ParameterId,
                    TableId = s.TableId,
                    ParameterCode = s.ParameterCode,
                    Description = s.Description,
                    SortOrder = s.SortOrder,
                    Semantic = s.Semantic
                })
                .SingleOrDefault();
        }

        public List<ParametersViewModel> GetParametersByTableId(string tableId)
        {
            return _parametersRepository.AllNoTracking(exp => exp.TableId == tableId)
                .Select(s => new ParametersViewModel
                {
                    ParameterId = s.ParameterId,
                    TableId = s.TableId,
                    ParameterCode = s.ParameterCode,
                    Description = s.Description,
                    SortOrder = s.SortOrder,
                    Semantic = s.Semantic
                }).ToList();
        }



        #endregion
    }
}
