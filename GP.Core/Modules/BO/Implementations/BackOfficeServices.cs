using GP.Core.Logging;
using GP.Core.Models;
using GP.Core.Utilities;
using GP.Core.Entities;
using GP.Core.Modules.BO.Interface;
using GP.Core.Repository.Contracts;
using GP.Core.Models.ViewModels;
//using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Globalization;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace GP.Core.Modules.BO.Implementations
{
    public class BackOfficeServices : IBackOfficeServices
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
        private readonly IGradeoRepository _gradeoRepository;
        private readonly IConfiguration _configuration;

        public BackOfficeServices(IConfiguration configuration, IBanqueadasRepository banqueadasRepository,
            ICarrerasRepository carrerasRepository, IHipodromosRepository hipodromosRepository,
            IJugadasRepository jugadasRepository, INotificacionesRepository notificacionesRepository,
            ITipoJugadasRepository tipoJugadasRepository, ITokensRepository tokensRepository,
            ITransaccionesRepository transaccionesRepository, IAceptacionesRepository aceptacionesRepository,
            IGradeoRepository gradeoRepository)
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
            _gradeoRepository = gradeoRepository;

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

        public List<LocacionViewModel> GetLocacionSinResultados(DateTime fecha)
        {            

            //var locacion = _hipodromosRepository.AllNoTracking()
            //    .Join(_carrerasRepository.AllNoTracking(), hip => hip.HipodromoID, carr => carr.HipodromoID, (hip, carr) => new { Hipodromos = hip, Carreras = carr })
            //    .Where(w => w.Carreras.LlegadaEjemplar == null && w.Carreras.FechaCarrera.Date == fecha.Date /*&& w.Carreras.HoraCarrera > DateTime.Now.TimeOfDay*/)
            //    .Select(s => new LocacionViewModel
            //    {
            //        Locacion = s.Hipodromos.Locacion
            //    }).Distinct();


            List<LocacionViewModel> locations = new List<LocacionViewModel>();

            string sql = "select distinct hip.Locacion ";
            sql += "from  Hipodromos hip ";
            sql += "inner join Carreras carr on hip.HipodromoID = carr.HipodromoID ";
            sql += "where carr.LlegadaEjemplar is null ";
            sql += "and cast(carr.[FechaCarrera] as date) = cast(getdate() as date) ";
            sql += "and cast(carr.HoraCarrera as time) > cast(GETDATE() as time) ";

            using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("AppContext")))
            {
                locations = db.Query<LocacionViewModel>(sql).ToList();
            }

            //var result = locacion.ToList();
            return locations; // result;
        }

        public List<HipodromosViewModels> GetHipodromos()
        {
            var hipodromos = _hipodromosRepository.AllNoTracking().Select(s => new HipodromosViewModels
            {
                HipodromoID = s.HipodromoID,
                Nombre = s.Nombre,
                Clasificacion = s.Clasificacion,
                Locacion = s.Locacion
            }).ToList();

            return hipodromos;
        }

        public List<HipodromosViewModels> GetHipodromosConResultados(DateTime fecha)
        {
            var hipodromos = _hipodromosRepository.AllNoTracking()
                .Join(_carrerasRepository.AllNoTracking(), hip => hip.HipodromoID, carr => carr.HipodromoID, (hip, carr) => new { Hipodromos = hip, Carreras = carr })
                .Where(w => w.Carreras.LlegadaEjemplar >= 0 && w.Carreras.FechaCarrera.Date == fecha.Date)
                .Select(s => new HipodromosViewModels
                {
                    HipodromoID = s.Hipodromos.HipodromoID,
                    Nombre = s.Hipodromos.Nombre,
                    Clasificacion = s.Hipodromos.Clasificacion,
                    Locacion = s.Hipodromos.Locacion
                }).Distinct();



            return hipodromos.ToList();
        }

        public List<HipodromosViewModels> GetHipodromosSinResultados(DateTime fecha)
        {

            var hipodromos = _hipodromosRepository.AllNoTracking()
                .Join(_carrerasRepository.AllNoTracking(), hip => hip.HipodromoID, carr => carr.HipodromoID, (hip, carr) => new { Hipodromos = hip, Carreras = carr })
                .Where(w => w.Carreras.LlegadaEjemplar == null && w.Carreras.FechaCarrera.Date == fecha.Date)
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

        public List<HipodromosViewModels> GetHipodromosByLocacionSinResultados(DateTime fecha, string locacion)
        {

            var hipodromos = _hipodromosRepository.AllNoTracking()
                .Join(_carrerasRepository.AllNoTracking(), hip => hip.HipodromoID, carr => carr.HipodromoID, (hip, carr) => new { Hipodromos = hip, Carreras = carr })
                .Where(w => w.Carreras.LlegadaEjemplar == null && w.Carreras.FechaCarrera.Date == fecha.Date && w.Carreras.HoraCarrera > DateTime.Now.TimeOfDay && w.Hipodromos.Locacion == locacion)
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

        public List<HipodromosViewModels> GetHipodromosByLocacion(string locacion)
        {
            var hipodromos = _hipodromosRepository.AllNoTracking(w => w.Locacion == locacion).Select(s => new HipodromosViewModels
            {
                HipodromoID = s.HipodromoID.Trim(),
                Nombre = s.Nombre.Trim(),
                Clasificacion = s.Clasificacion.Trim(),
                Locacion = s.Locacion.Trim()
            }).ToList();

            return hipodromos;
        }

        public List<HipodromosViewModels> GetHipodromosByLocacionClasificacion(string locacion, string clasif)
        {
            var hipodromos = _hipodromosRepository.AllNoTracking(w => w.Locacion == locacion && w.Clasificacion == clasif).Select(s => new HipodromosViewModels
            {
                HipodromoID = s.HipodromoID.Trim(),
                Nombre = s.Nombre.Trim(),
                Clasificacion = s.Clasificacion.Trim(),
                Locacion = s.Locacion.Trim()
            }).ToList();

            return hipodromos;
        }

        public List<CarrerasViewModel> GetCarreras(string hipodromoId)
        {
            Expression<Func<Carreras, bool>> expression = exp => exp.HipodromoID == hipodromoId && exp.FechaCarrera.Date == DateTime.Now.Date && exp.FechaCierreCarrera == null && exp.HoraCarrera > DateTime.Now.TimeOfDay;

            var carreras = _carrerasRepository.Get(expression);

            if (carreras != null)
            {
                var result = carreras.Select(s => new CarrerasViewModel
                {
                    CarreraID = s.CarreraID,
                    HipodromoID = s.HipodromoID.Trim(),
                    NumeroCarrera = s.NumeroCarrera,
                    NombreEjemplar = s.NombreEjemplar,
                    NumeroEjemplar = s.NumeroEjemplar,
                    LlegadaEjemplar = s.LlegadaEjemplar,
                    EstatusEjemplar = s.EstatusEjemplar,
                    FechaCarrera = s.FechaCarrera,
                    FechaCierreCarrera = s.FechaCierreCarrera,
                    HoraCarrera = s.HoraCarrera,
                    HoraCierreCarrera = s.HoraCierreCarrera
                })
                .ToList();
                return result;
            }

            return null;
        }

        public CarrerasViewModel GetCarreraById(long carreraId)
        {
            var carrera = _carrerasRepository.GetById(carreraId);

            if (carrera != null)
            {
                var result = new CarrerasViewModel
                {
                    CarreraID = carrera.CarreraID,
                    HipodromoID = carrera.HipodromoID.Trim(),
                    NumeroCarrera = carrera.NumeroCarrera,
                    NumeroEjemplar = carrera.NumeroEjemplar,
                    LlegadaEjemplar = carrera.LlegadaEjemplar,
                    NombreEjemplar = carrera.NombreEjemplar,
                    EstatusEjemplar = carrera.EstatusEjemplar,
                    FechaCarrera = carrera.FechaCarrera,
                    FechaCierreCarrera = carrera.FechaCierreCarrera,
                    HoraCarrera = carrera.HoraCarrera,
                    HoraCierreCarrera = carrera.HoraCierreCarrera
                };
                return result;
            }

            return null;
        }

        public List<CarrerasViewModel> GetCarrerasByHipodromoNumeroFecha(DateTime fecha, string hipodromoId, int numeroCarrera)
        {
            var carreras = _carrerasRepository.AllNoTracking(c => c.HipodromoID == hipodromoId && c.NumeroCarrera == numeroCarrera && c.FechaCarrera.Date == fecha.Date);

            if (carreras != null)
            {
                var result = carreras.Select(s => new CarrerasViewModel
                {
                    CarreraID = s.CarreraID,
                    HipodromoID = s.HipodromoID.Trim(),
                    NumeroCarrera = s.NumeroCarrera,
                    NombreEjemplar = s.NombreEjemplar,
                    NumeroEjemplar = s.NumeroEjemplar,
                    LlegadaEjemplar = s.LlegadaEjemplar,
                    EstatusEjemplar = s.EstatusEjemplar,
                    FechaCarrera = s.FechaCarrera,
                    FechaCierreCarrera = s.FechaCierreCarrera,
                    HoraCarrera = s.HoraCarrera,
                    HoraCierreCarrera = s.HoraCierreCarrera
                })
                .ToList();
                return result;
            }

            return null;
        }

        public List<CarrerasSelectModel> GetCarrerasSelect(DateTime fecha, string hipodromoId, bool tieneResult)
        {
            //Expression<Func<Carreras, bool>> expression = exp => exp.HipodromoID == hipodromoId;
            IQueryable<Carreras> carreras = null;
            if (!tieneResult)
                carreras = _carrerasRepository.AllNoTracking().Where(w => w.HipodromoID == hipodromoId && w.LlegadaEjemplar == null && w.FechaCarrera.Date == fecha.Date && w.HoraCarrera > DateTime.Now.TimeOfDay);
            else
                carreras = _carrerasRepository.AllNoTracking().Where(w => w.HipodromoID == hipodromoId && w.LlegadaEjemplar != null && w.FechaCarrera.Date == fecha.Date);

            if (carreras != null)
            {
                var result = carreras.Select(s => new CarrerasSelectModel { CarreraID = s.CarreraID, HipodromoID = s.HipodromoID.Trim(), NumeroCarrera = s.NumeroCarrera, Info = s.FechaCarrera.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " " + s.HoraCarrera.toHour12() }).ToList();

                CarrerasSelectModel ant = null;
                List<CarrerasSelectModel> data = new List<CarrerasSelectModel>();
                foreach (var item in result)
                {
                    if (ant == null || (item.HipodromoID != ant.HipodromoID || item.NumeroCarrera != ant.NumeroCarrera))
                    {
                        data.Add(item);
                    }

                    ant = item;
                }

                return data;
            }

            return null;
        }

        private string toHour12(TimeSpan t)
        {
            DateTime time = DateTime.Today.Add(t);
            string displayTime = time.ToString("hh:mm tt", CultureInfo.InvariantCulture);

            return displayTime;
        }
        private IEnumerable<T> RemoveDuplicates<T>(IEnumerable<T> source, Func<T, T, bool> equater)
        {
            // copy the source array 
            List<T> result = new List<T>();

            foreach (T item in source)
            {
                if (result.All(t => !equater(item, t)))
                {
                    // Doesn't exist already: Add it 
                    result.Add(item);
                }
            }

            return result;
        }

        public DatosHipodromoViewModel GetInfoCarrera(string hipodromoId, long idCarrera)
        {
            Expression<Func<Carreras, bool>> expression = exp => exp.HipodromoID == hipodromoId && exp.CarreraID == idCarrera;

            var carreras = _carrerasRepository.Get(expression);

            if (carreras != null)
            {



            }

            return null;
        }

        public DatosHipodromoViewModel GetInfoCarrera()
        {
            DatosHipodromoViewModel result = null;

            result = new DatosHipodromoViewModel
            {
                HipodromoID = "",
                NumeroCarrera = 0,
                CantidadEjemplar = 0,
                Clasificacion = "",
                Locacion = "",
                FechaCarrera = DateTime.Now,
                HoraCarrera = DateTime.Now.TimeOfDay,
                Ejemplares = new List<DatosEjemplarViewModel>()
            };

            return result;
        }

        public DatosHipodromoViewModel GetInfoCarrera(string hipodromoId, int numCarrera)
        {
            Expression<Func<Carreras, bool>> expression = exp => exp.HipodromoID == hipodromoId && exp.NumeroCarrera == numCarrera;

            DatosHipodromoViewModel result = null;

            var carreras = _carrerasRepository.Get(expression);

            if (carreras != null && carreras.Count() > 0)
            {
                Expression<Func<Hipodromos, bool>> find = exp => exp.HipodromoID == hipodromoId;

                var hipodromo = _hipodromosRepository.Get(find);

                if (hipodromo != null)
                {
                    var hip = hipodromo.Single();

                    var data = carreras.ToList();

                    result = new DatosHipodromoViewModel
                    {
                        HipodromoID = data.First().HipodromoID.Trim(),
                        NumeroCarrera = data.First().NumeroCarrera,
                        CantidadEjemplar = data.Count(),
                        Clasificacion = hip.Clasificacion,
                        Locacion = hip.Locacion,
                        FechaCarrera = data.First().FechaCarrera,
                        HoraCarrera = data.First().HoraCarrera,
                        FechaCierreCarrera = data.First().FechaCierreCarrera,
                        HoraCierreCarrera = data.First().HoraCierreCarrera,
                        Ejemplares = new List<DatosEjemplarViewModel>()
                    };

                }

            }

            return result;
        }

        public DatosHipodromoViewModel GetInfoCarrera(string hipodromoId, int numCarrera, DateTime fecha, TimeSpan hora)
        {
            Expression<Func<Carreras, bool>> expression = exp => exp.HipodromoID == hipodromoId && exp.NumeroCarrera == numCarrera && exp.FechaCarrera == fecha && exp.HoraCarrera == hora;

            DatosHipodromoViewModel result = null;

            var carreras = _carrerasRepository.Get(expression);

            if (carreras != null && carreras.Count() > 0)
            {
                Expression<Func<Hipodromos, bool>> find = exp => exp.HipodromoID == hipodromoId;

                var hipodromo = _hipodromosRepository.Get(find);

                if (hipodromo != null)
                {
                    var hip = hipodromo.Single();

                    var data = carreras.ToList();

                    result = new DatosHipodromoViewModel
                    {
                        HipodromoID = data.First().HipodromoID.Trim(),
                        NumeroCarrera = data.First().NumeroCarrera,
                        CantidadEjemplar = data.Count(),
                        Clasificacion = hip.Clasificacion,
                        Locacion = hip.Locacion,
                        FechaCarrera = data.First().FechaCarrera,
                        HoraCarrera = data.First().HoraCarrera,
                        FechaCierreCarrera = data.First().FechaCierreCarrera,
                        HoraCierreCarrera = data.First().HoraCierreCarrera,
                        Ejemplares = new List<DatosEjemplarViewModel>()
                    };

                }

            }

            return result;
        }

        public List<DatosEjemplarViewModel> GetEjemplaresActivos(string hipodromoId, int numCarrera)
        {
            Expression<Func<Carreras, bool>> expression = exp => exp.HipodromoID == hipodromoId 
            && exp.NumeroCarrera == numCarrera 
            && exp.FechaCarrera.Date == DateTime.Now.Date 
            && exp.HoraCarrera > DateTime.Now.TimeOfDay && exp.EstatusEjemplar == true && exp.LlegadaEjemplar == null && exp.FechaCierreCarrera == null;

            List<DatosEjemplarViewModel> result = null;

            var carreras = _carrerasRepository.Get(expression);

            if (carreras != null)
            {
                result = new List<DatosEjemplarViewModel>();

                var data = carreras.ToList();

                foreach (var item in data)
                {
                    result.Add(new DatosEjemplarViewModel
                    {
                        CarreraId = item.CarreraID,
                        NumeroEjemplar = item.NumeroEjemplar,
                        NombreEjemplar = item.NombreEjemplar,
                        LlegadaEjemplar = item.LlegadaEjemplar,
                        Estatusejemplar = item.StatusCarrera

                    });
                }
            }

            return result;
        }

        public List<DatosEjemplarViewModel> GetEjemplares(string hipodromoId, int numCarrera)
        {
            Expression<Func<Carreras, bool>> expression = exp => exp.HipodromoID == hipodromoId && exp.NumeroCarrera == numCarrera;

            List<DatosEjemplarViewModel> result = null;

            var carreras = _carrerasRepository.Get(expression);

            if (carreras != null)
            {
                result = new List<DatosEjemplarViewModel>();

                var data = carreras.ToList();

                foreach (var item in data)
                {
                    result.Add(new DatosEjemplarViewModel
                    {
                        CarreraId = item.CarreraID,
                        NumeroEjemplar = item.NumeroEjemplar,
                        NombreEjemplar = item.NombreEjemplar,
                        LlegadaEjemplar = item.LlegadaEjemplar,
                        Estatusejemplar = item.StatusCarrera

                    });
                }
            }

            return result;
        }

        public List<DatosEjemplarViewModel> GetEjemplares(string hipodromoId, int numCarrera, DateTime fecha, TimeSpan hora)
        {
            Expression<Func<Carreras, bool>> expression = exp => exp.HipodromoID == hipodromoId && exp.NumeroCarrera == numCarrera && exp.FechaCarrera == fecha && exp.HoraCarrera == hora;

            List<DatosEjemplarViewModel> result = null;

            var carreras = _carrerasRepository.Get(expression);

            if (carreras != null)
            {
                result = new List<DatosEjemplarViewModel>();

                var data = carreras.ToList();

                foreach (var item in data)
                {
                    result.Add(new DatosEjemplarViewModel
                    {
                        CarreraId = item.CarreraID,
                        NumeroEjemplar = item.NumeroEjemplar,
                        NombreEjemplar = item.NombreEjemplar,
                        LlegadaEjemplar = item.LlegadaEjemplar,
                        Estatusejemplar = item.EstatusEjemplar

                    });
                }
            }

            return result;
        }

        public bool ExisteCarrera(string HipodromoID, int NumeroCarrera, DateTime FechaCarrera, TimeSpan HoraCarrera)
        {
            try
            {
                var dataExist = _carrerasRepository.Filter(f => f.NumeroCarrera == NumeroCarrera && f.HipodromoID == HipodromoID && f.FechaCarrera == FechaCarrera && f.HoraCarrera == HoraCarrera).Any();

                return dataExist;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public bool ExisteEjemplarEnCarrera(string HipodromoID, int numeroEjemplar, DateTime FechaCarrera, TimeSpan HoraCarrera)
        {
            try
            {
                var dataExist = _carrerasRepository.Filter(f => f.HipodromoID == HipodromoID && f.NumeroEjemplar == numeroEjemplar && f.FechaCarrera == FechaCarrera && f.HoraCarrera == HoraCarrera && !string.IsNullOrEmpty(f.NombreEjemplar)).Any();


                return dataExist;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public bool ExisteEjemplarEnCarrera(string HipodromoID, int NumeroCarrera, int numeroEjemplar, DateTime FechaCarrera, TimeSpan HoraCarrera)
        {
            try
            {
                var dataExist = _carrerasRepository.Filter(f => f.NumeroCarrera == NumeroCarrera && f.HipodromoID == HipodromoID && f.NumeroEjemplar == numeroEjemplar && f.FechaCarrera == FechaCarrera && f.HoraCarrera == HoraCarrera && !string.IsNullOrEmpty(f.NombreEjemplar)).Any();


                return dataExist;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public bool ExisteEjemplarEnCarrera(string HipodromoID, int NumeroCarrera, int numeroEjemplar, DateTime FechaCarrera)
        {
            try
            {
                var dataExist = _carrerasRepository.Filter(f => f.NumeroCarrera == NumeroCarrera && f.HipodromoID == HipodromoID && f.NumeroEjemplar == numeroEjemplar && f.FechaCarrera == FechaCarrera && !string.IsNullOrEmpty(f.NombreEjemplar)).Any();

                return dataExist;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Result Guardar(DatosHipodromoViewForm data)
        {
            Result res = null;

            Colores color = new Colores();

            try
            {
                TimeSpan horacarrera;
                TimeSpan HoraServer = DateTime.Now.TimeOfDay;

                //TimeSpan.TryParse(data.HoraCarrera, out horacarrera);
                horacarrera = DateTime.ParseExact(data.HoraCarrera, "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture).TimeOfDay;


                if (data.TipoForm == "CARGA")//nuevo registro
                {

                    if (horacarrera < HoraServer)
                        throw new Exception("Hora de la carrera no es valida, debe ingresar una hora mayor a la del servidor.");

                    res = new Result { Ejemplares = data.Ejemplares != null && data.Ejemplares.Count() > 0 ? data.Ejemplares.ToList() : new List<DatosEjemplarViewModel>() };

                    if (data.Ejemplares != null)
                    {

                        foreach (var item in data.Ejemplares)
                        {
                            var dataExist = _carrerasRepository.Filter(f => f.NumeroCarrera == data.NumeroCarrera && f.NumeroEjemplar == item.NumeroEjemplar && f.HipodromoID == data.HipodromoID && f.FechaCarrera == data.FechaCarrera && f.HoraCarrera == horacarrera).SingleOrDefault();

                            if (dataExist == null)
                            {
                                if (item.NumeroEjemplar == 0)
                                    item.NumeroEjemplar = 1;

                                var carrerasNew = new Carreras
                                {
                                    ColorEjemplar = color.GetColores()[item.NumeroEjemplar - 1].Color,
                                    NumeroEjemplar = item.NumeroEjemplar,
                                    NombreEjemplar = item.NombreEjemplar,
                                    EstatusEjemplar = item.Estatusejemplar,
                                    HipodromoID = data.HipodromoID.Trim(),
                                    FechaCarrera = data.FechaCarrera,
                                    HoraCarrera = horacarrera, //data.HoraCarrera,
                                    NumeroCarrera = data.NumeroCarrera,
                                    LlegadaEjemplar = item.LlegadaEjemplar,
                                    StatusCarrera = false,
                                    FechaCierreCarrera = null,
                                    HoraCierreCarrera = null
                                };

                                _carrerasRepository.Create(carrerasNew);

                            }
                            else
                            {
                                if (dataExist.FechaCierreCarrera == null && dataExist.HoraCierreCarrera == null)
                                {
                                    dataExist.EstatusEjemplar = data.Ejemplares.Single(s => s.NumeroEjemplar == item.NumeroEjemplar).Estatusejemplar;
                                    dataExist.NombreEjemplar = data.Ejemplares.Single(s => s.NumeroEjemplar == item.NumeroEjemplar).NombreEjemplar;
                                    dataExist.HipodromoID = data.HipodromoID.Trim();
                                    dataExist.FechaCarrera = data.FechaCarrera;
                                    dataExist.HoraCarrera = horacarrera; //data.HoraCarrera;
                                    dataExist.NumeroCarrera = data.NumeroCarrera;
                                    _carrerasRepository.Update(dataExist);

                                }
                            }

                            if (dataExist != null)
                            {
                                res.Codigo = 200;
                                res.Msg = "";
                                res.Status = "OK";

                            }
                            else
                            {
                                res.Codigo = 200;
                                res.Msg = "Carrera Agregada con exito!";
                                res.Status = "OK";
                                res.Ejemplares.Add(item);
                            }

                        }
                    }
                    else
                    {
                        var carrerasNew = new Carreras
                        {
                            ColorEjemplar = color.GetColores()[0].Color,
                            NumeroEjemplar = data.CantidadEjemplar,
                            NombreEjemplar = "",
                            EstatusEjemplar = false,
                            HipodromoID = data.HipodromoID.Trim(),
                            FechaCarrera = data.FechaCarrera,
                            HoraCarrera = horacarrera, //data.HoraCarrera,
                            NumeroCarrera = data.NumeroCarrera,
                            LlegadaEjemplar = null,
                            StatusCarrera = false,
                            FechaCierreCarrera = null,
                            HoraCierreCarrera = null
                        };



                        _carrerasRepository.Create(carrerasNew);
                        res.Codigo = 200;
                        res.Msg = "";
                        res.Status = "OK";
                        res.Ejemplares.Add(new DatosEjemplarViewModel { NumeroEjemplar = data.CantidadEjemplar, NombreEjemplar = "", Estatusejemplar = false, LlegadaEjemplar = null });
                    }


                }
                else if (data.TipoForm == "MODIFICAR") //update
                {

                    var dataUpdate = _carrerasRepository.Filter(f => f.NumeroCarrera == data.NumeroCarrera && f.HipodromoID == data.HipodromoID && f.FechaCarrera == data.FechaCarrera && f.HoraCarrera == horacarrera);

                    if (dataUpdate != null)
                    {
                        var carreasList = dataUpdate.ToList();

                        foreach (var item in carreasList)
                        {
                            if (item.FechaCierreCarrera == null && item.HoraCierreCarrera == null)
                            {
                                item.EstatusEjemplar = data.Ejemplares.Single(s => s.NumeroEjemplar == item.NumeroEjemplar).Estatusejemplar;
                                item.NombreEjemplar = data.Ejemplares.Single(s => s.NumeroEjemplar == item.NumeroEjemplar).NombreEjemplar;
                                item.HipodromoID = data.HipodromoID.Trim();
                                item.FechaCarrera = data.FechaCarrera;
                                item.HoraCarrera = horacarrera; //data.HoraCarrera;
                                item.NumeroCarrera = data.NumeroCarrera;
                                _carrerasRepository.Update(item);

                            }

                        }


                        if (carreasList.Count < data.Ejemplares.Count())
                        {
                            var ejemplarnew = data.Ejemplares[data.Ejemplares.Length - 1];

                            var carrerasNew = new Carreras
                            {
                                ColorEjemplar = color.GetColores()[0].Color,
                                NumeroEjemplar = ejemplarnew.NumeroEjemplar,
                                NombreEjemplar = ejemplarnew.NombreEjemplar,
                                EstatusEjemplar = ejemplarnew.Estatusejemplar,
                                HipodromoID = data.HipodromoID.Trim(),
                                FechaCarrera = data.FechaCarrera,
                                HoraCarrera = horacarrera, //data.HoraCarrera,
                                NumeroCarrera = data.NumeroCarrera,
                                LlegadaEjemplar = null,
                                StatusCarrera = false,
                                FechaCierreCarrera = null,
                                HoraCierreCarrera = null
                            };

                            _carrerasRepository.Create(carrerasNew);
                        }


                        res = new Result { Codigo = 200, Msg = "Carrera modificada con exito!", Status = "OK" };
                    }

                }
                else //update solo resultados
                {
                    var dataUpdate = _carrerasRepository.Filter(f => f.NumeroCarrera == data.NumeroCarrera && f.HipodromoID == data.HipodromoID && f.FechaCarrera == data.FechaCarrera && f.HoraCarrera == horacarrera);

                    if (dataUpdate != null)
                    {

                        foreach (var item in dataUpdate.ToList())
                        {
                            if (item.FechaCierreCarrera == null && item.HoraCierreCarrera == null)
                            {
                                item.LlegadaEjemplar = data.Ejemplares.Single(s => s.NumeroEjemplar == item.NumeroEjemplar).LlegadaEjemplar;
                                item.FechaCierreCarrera = DateTime.Now;
                                item.HoraCierreCarrera = DateTime.Now.TimeOfDay;
                                _carrerasRepository.Update(item);
                            }
                        }

                        res = new Result { Codigo = 200, Msg = "Resultado agregado con exito!", Status = "OK" };
                    }

                }

                _carrerasRepository.Commit();
            }
            catch (Exception ex)
            {
                res = new Result { Codigo = 400, Msg = ex.Message, Status = "ERROR" };
            }

            if (res == null)
            {
                res = new Result { Codigo = 300, Msg = "", Status = "NULO" };
            }

            return res;
        }

        public Result EliminarEjemplar(string hipodromoID, int numeroCarrera, int numeroEjemplar, DateTime fecha, TimeSpan hora)
        {
            Result res = null;

            int codigo = 400;
            try
            {
                //validar si existe mas de una carrera si solo existe una no deberia dejar eliminarla
                var totalCarreras = _carrerasRepository.Filter(f => f.NumeroCarrera == numeroCarrera && f.HipodromoID == hipodromoID && f.FechaCarrera == fecha && f.HoraCarrera == hora).Count();

                if (totalCarreras == 1)
                {
                    codigo = 100;
                    throw new Exception("No se puede eliminar el ejemplar, debido a que debe haber al menos uno por carrera");
                }

                var dataExist = _carrerasRepository.Filter(f => f.NumeroCarrera == numeroCarrera && f.NumeroEjemplar == numeroEjemplar && f.HipodromoID == hipodromoID && f.FechaCarrera == fecha && f.HoraCarrera == hora).SingleOrDefault();

                if (dataExist != null)
                {
                    if (dataExist.FechaCierreCarrera != null || dataExist.HoraCierreCarrera != null)
                    {
                        throw new Exception("No se puede eliminar un ejemplar cuando existe fecha de cierre de carrera");
                    }

                    _carrerasRepository.Delete(dataExist);
                    _carrerasRepository.Commit();

                    //reasignar numeros a los ejemplares

                    var data = _carrerasRepository.Filter(f => f.NumeroCarrera == numeroCarrera && f.HipodromoID == hipodromoID && f.FechaCarrera == fecha && f.HoraCarrera == hora).ToList();


                    if (data != null)
                    {
                        var i = 1;
                        foreach (var item in data)
                        {
                            item.NumeroEjemplar = i;
                            _carrerasRepository.Update(item);
                            i++;
                        }

                        _carrerasRepository.Commit();
                    }
                    res = new Result { Codigo = 200, Msg = "Ejemplar eliminado con exito!", Status = "OK" };
                }
                else
                {
                    throw new Exception("Ejemplar no existe");
                }

            }
            catch (Exception ex)
            {
                res = new Result { Codigo = codigo, Msg = ex.Message, Status = "ERROR" };
            }

            return res;
        }

        public List<TipoJugadasViewModel> GetTiposJugadas()
        {
            var tp = _tipoJugadasRepository.AllNoTracking(w => w.Activo).Select(
                s => new TipoJugadasViewModel
                {
                    Codigo = s.Codigo,
                    Descripcion = s.Descripcion,
                    Activo = s.Activo
                }
                ).ToList();

            return tp;
        }

        public TipoJugadasViewModel GetTiposJugadasByCodigo(string codigo)
        {
            var tp = _tipoJugadasRepository.Get(w => w.Activo && w.Codigo == codigo).Select(s => new TipoJugadasViewModel
            {
                TipoJugadaID = s.TipoJugadaID,
                Codigo = s.Codigo,
                Descripcion = s.Descripcion,
                Activo = s.Activo
            }).SingleOrDefault();

            return tp;
        }

        public TipoJugadasViewModel GetTiposJugadasById(int id)
        {
            var tp = _tipoJugadasRepository.Get(w => w.Activo && w.TipoJugadaID == id).Select(s => new TipoJugadasViewModel
            {
                TipoJugadaID = s.TipoJugadaID,
                Codigo = s.Codigo,
                Descripcion = s.Descripcion,
                Activo = s.Activo
            }).SingleOrDefault();

            return tp;
        }

        public HipodromosViewModels GetHipodromosByID(string codigo)
        {
            var tp = _hipodromosRepository.Get(w => w.HipodromoID == codigo).Select(s => new HipodromosViewModels
            {
                HipodromoID = s.HipodromoID,
                Nombre = s.Nombre,
                Locacion = s.Locacion,
                Clasificacion = s.Clasificacion
            }).SingleOrDefault();

            return tp;
        }

        void IBackOfficeServices.GuardarTransaccion(TransaccionesViewModel data)
        {
            try
            {
                var transaccionNew = new Transacciones
                {
                    Usuario = data.Usuario,
                    IdAgente = data.IdAgente,
                    Agente = data.Agente,
                    Moneda = data.Moneda,
                    IP = data.IP,
                    Token = data.Token,
                    CarreraID = data.CarreraID,
                    JugadaID = data.JugadaID,
                    BanqueadaID = data.BanqueadaID,
                    NotificacionID = data.NotificacionID,
                    AceptacionID = data.AceptacionID,
                    MontoJugada = data.MontoJugada,
                    MontoBanqueada = data.MontoBanqueada,
                    MontoAceptacion = data.MontoAceptacion,
                    Balance = data.Balance,
                    Accion = data.Accion,
                    Descripcion = data.Descripcion,
                    ErrorMensaje = data.ErrorMensaje,
                    JsonRequest = data.JsonRequest,
                    Fecha = data.Fecha,
                };

                _transaccionesRepository.Create(transaccionNew);
                _transaccionesRepository.Commit();

            }
            catch (Exception ex)
            {

                //throw;
            }

        }

        public async Task<Result> GuardarJugada(JugadasViewModel data)
        {
            Result res = null;
            try
            {
                //var newJugada = new Jugadas
                //{
                //    CarreraID = data.CarreraID,
                //    NumeroEjemplar = data.NumeroEjemplar,
                //    TipoJugadaID = data.TipoJugada,
                //    Monto = data.Monto,
                //    FechaJugada = data.FechaJugada,
                //    Usuario = data.Usuario,
                //    Agente = data.Agente,
                //    IdAgente = data.IdAgente,
                //    Moneda = data.Moneda
                //};

                //_jugadasRepository.Create(newJugada);
                //_jugadasRepository.Commit();
                var saldo = await Utils.SaldoDisponibleAsync(data.Usuario);

                decimal saldoDisp;

                decimal.TryParse(saldo, out saldoDisp);

                if (saldoDisp <= 0)
                {
                    throw new Exception("El cliente no dispone de saldo suficiente");
                }

                string sql = "SP_INSERT_JUGADA @CarreraID, @NumeroEjemplar, @NumeroCarrera, @TipoJugadaID, @Monto ,@FechaJugada, @Usuario ,@Agente ,@IdAgente, @Moneda, @CajaNro, @Status, @id output";
                //string sql = "INSERT INTO [dbo].[Jugadas] ([CarreraID] ,[NumeroEjemplar] ,[NumeroCarrera] ,[TipoJugadaID],[Monto] ,[FechaJugada],[Usuario] ,[Agente] ,[IdAgente] ,[Moneda], [CajaNro],[Status])";
                //sql += "VALUES (@CarreraID,@NumeroEjemplar,@NumeroCarrera,@TipoJugadaID,@Monto ,@FechaJugada,@Usuario ,@Agente ,@IdAgente,@Moneda,@CajaNro, @Status)";
                //sql += "SELECT @id = SCOPE_IDENTITY()";

                //List<SqlParameter> parameters = new List<SqlParameter>();
                //parameters.Add(new SqlParameter { ParameterName = "@CarreraID", SqlDbType = System.Data.SqlDbType.BigInt, Value = data.CarreraID });
                //parameters.Add(new SqlParameter { ParameterName = "@NumeroEjemplar", SqlDbType = System.Data.SqlDbType.Int, Value = data.NumeroEjemplar });
                //parameters.Add(new SqlParameter { ParameterName = "@NumeroCarrera", SqlDbType = System.Data.SqlDbType.Int, Value = data.NumeroCarrera });
                //parameters.Add(new SqlParameter { ParameterName = "@TipoJugadaID", SqlDbType = System.Data.SqlDbType.Int, Value = data.TipoJugada });
                //parameters.Add(new SqlParameter { ParameterName = "@Monto", SqlDbType = System.Data.SqlDbType.Decimal, Precision = 18, Scale = 2, Value = data.Monto });
                //parameters.Add(new SqlParameter { ParameterName = "@FechaJugada", SqlDbType = System.Data.SqlDbType.DateTime, Value = data.FechaJugada });
                //parameters.Add(new SqlParameter { ParameterName = "@Usuario", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = data.Usuario });
                //parameters.Add(new SqlParameter { ParameterName = "@Agente", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = data.Agente });
                //parameters.Add(new SqlParameter { ParameterName = "@IdAgente", SqlDbType = System.Data.SqlDbType.Int, Value = data.IdAgente });
                //parameters.Add(new SqlParameter { ParameterName = "@Moneda", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = data.Moneda });
                //parameters.Add(new SqlParameter { ParameterName = "@CajaNro", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = data.CajaNro });
                //parameters.Add(new SqlParameter { ParameterName = "@Status", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = data.Status });
                //parameters.Add(new SqlParameter { ParameterName = "@id", SqlDbType = System.Data.SqlDbType.BigInt, Direction = System.Data.ParameterDirection.Output });

                //var result = _jugadasRepository.ExecuteSQL(sql, parameters.ToArray());

                long jugadaId = 0;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("AppContext")))
                {
                    //var affectedRows = db.Execute(sql,
                    //new { 
                    //    CarreraID = data.CarreraID, 
                    //    NumeroEjemplar = data.NumeroEjemplar,
                    //    NumeroCarrera = data.NumeroCarrera,
                    //    TipoJugadaID = data.TipoJugada,
                    //    Monto = data.Monto,
                    //    FechaJugada = data.FechaJugada,
                    //    Usuario = data.Usuario,
                    //    Agente = data.Agente,
                    //    IdAgente = data.IdAgente,
                    //    Moneda = data.Moneda,
                    //    CajaNro = data.CajaNro,
                    //    Status = data.Status,
                    //    id = 0
                    //},
                    //commandType: CommandType.StoredProcedure);
                    var p = new { 
                        CarreraID = data.CarreraID, 
                        NumeroEjemplar = data.NumeroEjemplar,
                        NumeroCarrera = data.NumeroCarrera,
                        TipoJugadaID = data.TipoJugada,
                        Monto = data.Monto,
                        FechaJugada = data.FechaJugada,
                        Usuario = data.Usuario,
                        Agente = data.Agente,
                        IdAgente = data.IdAgente,
                        Moneda = data.Moneda,
                        CajaNro = data.CajaNro,
                        Status = data.Status
                    };

                    jugadaId = db.ExecuteOutputParam(sql, p);

                }




                    
                //jugadaId = Convert.ToInt64(parameters.Single(x => x.ParameterName == "@id").Value);//_jugadasRepository.ExecuteSQL("SELECT SCOPE_IDENTITY() AS id", null);

                int tipoTrans = 2;

                var trans = await Utils.RegistrarMontoAsync(data.Monto.ToString().Replace('.', ','), tipoTrans, data.Usuario, "Apuesta - Descuento");

                int.TryParse(trans.ToString(), out int numtransaction);

                if (numtransaction <= 0)
                    throw new Exception("No se pudo realizar la transaccion.");

                saldo = await Utils.SaldoDisponibleAsync(data.Usuario);
                decimal.TryParse(saldo, out saldoDisp);

                res = new Result { Codigo = 200, Msg = jugadaId.ToString() + "|" + saldoDisp.ToString("n"), Status = "OK" };

            }
            catch (Exception ex)
            {
                res = new Result { Codigo = 400, Msg = ex.Message, Status = "ERROR" };
            }

            return res;
        }

        public Result GuardarBanqueada(BanqueadaViewModel data)
        {
            Result res = null;
            try
            {
                //var newBanqueada = new Banquedas
                //{
                //    JugadaID = data.JugadaID,                     
                //    NumeroEjemplar = data.NumeroEjemplar,
                //    TipoJugadaID = data.TipoJugada,
                //    Monto = data.Monto,
                //    FechaBanqueada = data.FechaBanqueada,
                //    Usuario = data.Usuario,
                //    Agente = data.Agente,
                //    IdAgente = data.IdAgente,
                //    Moneda = data.Moneda
                //};

                //_banqueadasRepository.Create(newBanqueada);
                //_banqueadasRepository.Commit();

                string sql = "INSERT INTO [dbo].[Banquedas] ([JugadaID] ,[Monto] ,[NumeroEjemplar], [NumeroCarrera],[TipoJugadaID],[FechaBanqueada],[Usuario] ,[Agente],[IdAgente] ,[Moneda], [CajaNro])";
                sql += "VALUES (@JugadaID,@Monto,@NumeroEjemplar,@TipoJugadaID ,@FechaBanqueada,@Usuario ,@Agente ,@IdAgente,@Moneda)";
                sql += "SELECT @id = SCOPE_IDENTITY()";

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter { ParameterName = "@JugadaID", SqlDbType = System.Data.SqlDbType.BigInt, Value = data.JugadaID });
                parameters.Add(new SqlParameter { ParameterName = "@Monto", SqlDbType = System.Data.SqlDbType.Decimal, Precision = 18, Scale = 2, Value = data.Monto });
                parameters.Add(new SqlParameter { ParameterName = "@NumeroEjemplar", SqlDbType = System.Data.SqlDbType.Int, Value = data.NumeroEjemplar });
                //parameters.Add(new SqlParameter { ParameterName = "@NumeroCarrera", SqlDbType = System.Data.SqlDbType.Int, Value = data.nu });
                parameters.Add(new SqlParameter { ParameterName = "@TipoJugadaID", SqlDbType = System.Data.SqlDbType.Int, Value = data.TipoJugada });
                parameters.Add(new SqlParameter { ParameterName = "@FechaBanqueada", SqlDbType = System.Data.SqlDbType.DateTime, Value = data.FechaBanqueada });
                parameters.Add(new SqlParameter { ParameterName = "@Usuario", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = data.Usuario });
                parameters.Add(new SqlParameter { ParameterName = "@Agente", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = data.Agente });
                parameters.Add(new SqlParameter { ParameterName = "@IdAgente", SqlDbType = System.Data.SqlDbType.Int, Value = data.IdAgente });
                parameters.Add(new SqlParameter { ParameterName = "@Moneda", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = data.Moneda });
                parameters.Add(new SqlParameter { ParameterName = "@CajaNro", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 50, Value = data.CajaNro });
                parameters.Add(new SqlParameter { ParameterName = "@id", SqlDbType = System.Data.SqlDbType.BigInt, Direction = System.Data.ParameterDirection.Output });
                var result = _banqueadasRepository.ExecuteSQL(sql, parameters.ToArray());
                long banqueadaId = 0;
                banqueadaId = Convert.ToInt64(parameters.Single(x => x.ParameterName == "@id").Value); ;
                res = new Result { Codigo = 200, Msg = banqueadaId.ToString(), Status = "OK" };

            }
            catch (Exception ex)
            {
                res = new Result { Codigo = 400, Msg = ex.Message, Status = "ERROR" };
            }

            return res;
        }


        public List<ReporteViewModel> ReporteGradeo(string usuario, DateTime fecha)
        {
            try
            {
                return _gradeoRepository.AllNoTracking(x => x.Activo && x.Usuario == usuario && x.Fecha.Date == fecha.Date)
                    .Select(s => new ReporteViewModel
                    {
                        Ticket = s.Ticket,
                        Tipo = s.Tipo,
                        Detalle = s.Detalle,
                        Monto = s.Monto,
                        Comision = s.Comision,
                        Status = s.Status

                    }).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //private readonly IHttpContextAccessor _accessor;


    }

}
