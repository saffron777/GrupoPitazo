using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace GP.BackOffice.Web.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class HomeController : Controller
    {
        private readonly IBackOfficeServices _backOfficeServices;

        public HomeController(IBackOfficeServices backOfficeServices)
        {
            _backOfficeServices = backOfficeServices;
        }
        public IActionResult Index()
        {
            //Log.Info("Prueba logs: lista de hipodromos");
            //var hipodromos = _backOfficeServices.GetHipodromos();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult MessageBox(string titulo , string mensaje)
        {
            ViewBag.titulo = titulo;
            ViewBag.mensaje = mensaje;

            return PartialView("_Confirm");
        }
    }
}
