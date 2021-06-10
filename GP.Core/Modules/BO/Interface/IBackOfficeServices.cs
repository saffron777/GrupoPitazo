using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GP.Core.Models;
using GP.Core.Models.ViewModels;

namespace GP.Core.Modules.BO.Interface
{
    public interface IBackOfficeServices
    {
        List<HipodromosViewModels> GetHipodromos();
        List<LocacionViewModel> GetLocacionSinResultados(DateTime fecha);
        List<HipodromosViewModels> GetHipodromosByLocacion(string locacion);
        List<HipodromosViewModels> GetHipodromosByLocacionClasificacion(string locacion, string clasif);
        List<HipodromosViewModels> GetHipodromosByLocacionSinResultados(DateTime fecha, string locacion);
        List<HipodromosViewModels> GetHipodromosConResultados(DateTime fecha);
        List<HipodromosViewModels> GetHipodromosSinResultados(DateTime fecha);
        List<CarrerasViewModel> GetCarrerasByHipodromoNumeroFecha(DateTime fecha, string hipodromoId, int numeroCarrera);
        List<CarrerasViewModel> GetCarreras(string hipodromoId);
        CarrerasViewModel GetCarreraById(long carreraId);
        List<CarrerasSelectModel> GetCarrerasSelect(DateTime fecha, string hipodromoId, bool tieneResult);
        DatosHipodromoViewModel GetInfoCarrera();
        DatosHipodromoViewModel GetInfoCarrera(string hipodromoId, int numCarrera);
        DatosHipodromoViewModel GetInfoCarrera(string hipodromoId, int numCarrera, DateTime fecha, TimeSpan hora);
        List<DatosEjemplarViewModel> GetEjemplaresActivos(string hipodromoId, int numCarrera);
        List<DatosEjemplarViewModel> GetEjemplares(string hipodromoId, int numCarrera);
        List<DatosEjemplarViewModel> GetEjemplares(string hipodromoId, int numCarrera, DateTime fecha, TimeSpan hora);
        HipodromosViewModels GetHipodromosByID(string codigo);
        bool ExisteCarrera(string HipodromoID, int NumeroCarrera, DateTime FechaCarrera, TimeSpan HoraCarrera);
        bool ExisteEjemplarEnCarrera(string HipodromoID, int numeroEjemplar, DateTime FechaCarrera, TimeSpan HoraCarrera);
        bool ExisteEjemplarEnCarrera(string HipodromoID, int NumeroCarrera, int numeroEjemplar, DateTime FechaCarrera, TimeSpan HoraCarrera);
        bool ExisteEjemplarEnCarrera(string HipodromoID, int NumeroCarrera, int numeroEjemplar, DateTime FechaCarrera);
        Result Guardar(DatosHipodromoViewForm data);

        Result EliminarEjemplar(string hipodromoID, int numeroCarrera, int numeroEjemplar, DateTime fecha, TimeSpan hora);

        List<TipoJugadasViewModel> GetTiposJugadas();

        TipoJugadasViewModel GetTiposJugadasByCodigo(string codigo);
        TipoJugadasViewModel GetTiposJugadasById(int id);
        void GuardarTransaccion(TransaccionesViewModel data);

        Task<Result> GuardarJugada(JugadasViewModel data);

        Result GuardarBanqueada(BanqueadaViewModel data);

        List<ReporteViewModel> ReporteGradeo(string usuario, DateTime fecha);
    }
}
