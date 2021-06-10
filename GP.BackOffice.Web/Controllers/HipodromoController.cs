using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GP.BackOffice.Web.Models;
using GP.Core.Logging;
using GP.Core.Utilities;
using GP.Core.Models;
using GP.Core.Models.ViewModels;
using GP.Core.Modules.BO.Interface;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace GP.BackOffice.Web.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class HipodromoController : Controller
    {
        private readonly IBackOfficeServices _backOfficeServices;

        public HipodromoController(IBackOfficeServices backOfficeServices)
        {
            _backOfficeServices = backOfficeServices;
        }
        public IActionResult CargarHipodromo()
        {
            ViewBag.TipoForm = "CARGA";
            return View();
        }

        public IActionResult ModificarCarrera()
        {
            ViewBag.TipoForm = "MODIFICAR";
            return View("CargarHipodromo");
        }

        public IActionResult ResultadoCarrera()
        {
            ViewBag.TipoForm = "RESULTADO";
            return View("CargarHipodromo");
        }

        public IActionResult BuscarCarrerasPartialView(string tipoForm)
        {
            //var hipodromos = _backOfficeServices.GetHipodromos();


            //List<HipodromosViewModels> hipodromos = null;

            //if (tipoForm == "MODIFICAR")
            //    hipodromos = _backOfficeServices.GetHipodromosSinResultados();
            //else if (tipoForm == "RESULTADO")
            //    hipodromos = _backOfficeServices.GetHipodromosConResultados();
            //else
            //    hipodromos = _backOfficeServices.GetHipodromos();

            //return PartialView("_BuscarCarreras",hipodromos);


            return PartialView("_BuscarCarreras");
        }

        public JsonResult GetHipodromosPorFecha(DateTime fecha, string tipoForm)
        {
            List<HipodromosViewModels> hipodromos = null;

            if (tipoForm == "MODIFICAR" || tipoForm == "RESULTADO")
                hipodromos = _backOfficeServices.GetHipodromosSinResultados(fecha);
            //else if (tipoForm == "RESULTADO")
            //    hipodromos = _backOfficeServices.GetHipodromosConResultados(fecha);
            else
                hipodromos = _backOfficeServices.GetHipodromos();

            return new JsonResult(hipodromos);
        }


        public JsonResult GetCarreras(DateTime fecha, string hipodromoId, string tipoForm)
        {
            var carreras = _backOfficeServices.GetCarrerasSelect(fecha,hipodromoId, false);//  _backOfficeServices.GetCarreras(hipodromoId);

            return new JsonResult(carreras);
        }

        public IActionResult BuscarInfoVaciaPartialView()
        {
            var info = _backOfficeServices.GetInfoCarrera();

            info.Hipodromos = _backOfficeServices.GetHipodromos();

            info.TipoForm = "CARGA";
            return PartialView("_DatosHipodromo", info);
        }

        public IActionResult CargarHipodromos(string locacion, string clasif)
        {
            var hipodromos = _backOfficeServices.GetHipodromosByLocacionClasificacion(locacion, clasif);

            return new JsonResult(hipodromos);
        }
        public IActionResult BuscarInfoCarrerasPartialView(string tipoForm, string hipodromoId, int numCarrera, DateTime fecha, TimeSpan hora)
        {
            var info = _backOfficeServices.GetInfoCarrera(hipodromoId, numCarrera, fecha, hora);

            if (info != null)
            {               
                info.Hipodromos = _backOfficeServices.GetHipodromos();

                info.TipoForm = tipoForm;                
            }
            else
            {
                if(tipoForm == "MODIFICAR")
                    RedirectToAction("ModificarCarrera");
                else if(tipoForm == "RESULTADO")                
                    RedirectToAction("ResultadoCarrera");                
            }

            return PartialView("_DatosHipodromo", info);
        }

        [HttpPost]
        public IActionResult Guardar(DatosHipodromoViewForm data)
        {

            if (ModelState.IsValid)
            {
                if (data.Ejemplares != null)
                {
                    foreach (var item in data.Ejemplares)
                    {
                        if (item.NombreEjemplar == "_")
                        {
                            item.NombreEjemplar = "";
                        }
                    }
                }

                if(data.TipoForm=="CARGA")
                {
                    bool existeCarrera = false;
                   
                    string msg = "";
                    TimeSpan horacarrera;                   
                    horacarrera = DateTime.ParseExact(data.HoraCarrera, "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture).TimeOfDay;

                    existeCarrera = _backOfficeServices.ExisteEjemplarEnCarrera(data.HipodromoID, data.NumeroNuevoEjemplar, data.FechaCarrera, horacarrera);

                    bool existeCarreraMimsoDia = _backOfficeServices.ExisteEjemplarEnCarrera(data.HipodromoID, data.NumeroCarrera, data.NumeroNuevoEjemplar, data.FechaCarrera);
                                       

                    if (existeCarrera)
                    {
                        if (data.Operacion == "EDICION"){
                            
                            msg = "No se puede cargar una nueva carrera, debido a que existe ya una previamente otra cargada a esa misma fecha y hora.";
                        }
                        else if (data.Operacion == "NUEVOEJEMPLAR")
                        {                            
                            msg = "No se puede cargar un nuevo ejemplar, debido a que existe  previamente otro cargado en otra carrera a esa misma fecha y hora.";
                        }

                        return new JsonResult(new { status = "ERROR", msg = msg });
                    }
                    else if (existeCarreraMimsoDia)
                    {
                        if (data.Operacion == "EDICION")
                        {

                            msg = "No se puede cargar una nueva carrera, debido a que existe ya una previamente cargada a esa misma fecha.";
                        }
                        else if (data.Operacion == "NUEVOEJEMPLAR")
                        {
                            msg = "No se puede cargar un nuevo ejemplar, debido a que existe  previamente cargado en una carrera a esa misma fecha.";
                        }

                        return new JsonResult(new { status = "ERROR", msg = msg });
                    }


                }

                var result = _backOfficeServices.Guardar(data);

                return new JsonResult(new { status = result.Status, msg = result.Msg });
            }
            else
            {
                if(data.Ejemplares==null || data.Ejemplares.Count() == 0)
                {
                    return new JsonResult(new { status = "ERROR", msg = "Debe agregrar al menos un ejemplar." });
                }
                else
                return new JsonResult(new { status = "ERROR", msg = "No se pudo realizar la operacion." });
            }
        }

        [HttpPost]
        public IActionResult EliminarEjemplar(string hipodromoID, int numeroCarrera, int numeroEjemplar, DateTime fecha, TimeSpan hora)
        {

            var result = _backOfficeServices.EliminarEjemplar(hipodromoID, numeroCarrera, numeroEjemplar, fecha, hora);

            return new JsonResult(new {codigo = result.Codigo, status = result.Status, msg = result.Msg });
        }

        public IActionResult AjaxHandler(JQueryDataTableParamModel param)
        {

            if (string.IsNullOrEmpty(Request.Query["hipodromoId"]))
            {
                return new JsonResult(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = 1,
                    iTotalDisplayRecords = 1,
                    aaData = (new List<DatosEjemplarViewModel>()).Select(u => new
                    {
                        u.NumeroEjemplar,
                        u.NombreEjemplar,
                        u.Estatusejemplar,
                        u.LlegadaEjemplar
                    }).ToList()
                });
            }

            string hipodromoId = Request.Query["hipodromoId"];
            int numCarreram =int.Parse(Request.Query["numCarreram"]);
            string numeroEjemplar = Request.Query["numeroEjemplar"];
           string fechaCarrera = Request.Query["fechaCarrera"];
            string horaCarrera = Request.Query["horaCarrera"];

            DateTime fecha;
            TimeSpan hora;

            DateTime.TryParse(fechaCarrera, out fecha);
            //TimeSpan.TryParse(horaCarrera, out hora);

            hora = DateTime.ParseExact(horaCarrera, "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture).TimeOfDay;

            List<DatosEjemplarViewModel> data = null;

            if(fechaCarrera==null || horaCarrera ==null)
            data = _backOfficeServices.GetEjemplares(hipodromoId, numCarreram).Where(w => w.NumeroEjemplar > 0).ToList();
            else
            data = _backOfficeServices.GetEjemplares(hipodromoId, numCarreram, fecha, hora).Where(w => w.NumeroEjemplar > 0).ToList();

            if ((!string.IsNullOrEmpty(numeroEjemplar) && int.Parse(numeroEjemplar) > 1) || (data.Count == 0 && !string.IsNullOrEmpty(numeroEjemplar)))
            {
              
                data.Add(new DatosEjemplarViewModel
                {
                    NumeroEjemplar =int.Parse(numeroEjemplar),
                    Estatusejemplar = false,
                    LlegadaEjemplar = null,
                    NombreEjemplar = ""
                });

            }

            return new JsonResult(new
            {
                sEcho = param.sEcho,
                iTotalRecords = data.Count(),
                iTotalDisplayRecords = data.Count(),
                aaData = data.Select(u => new
                {
                    u.NumeroEjemplar,
                    u.NombreEjemplar,
                    u.Estatusejemplar,
                    u.LlegadaEjemplar
                }).ToList()
            });
        }
    }
}