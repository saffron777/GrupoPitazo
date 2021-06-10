using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GP.Core.Entities;
using GP.Core.Models;
using GP.Core.Models.ViewModels;

namespace GP.Core.Modules.App.Interface
{
    public interface IAppServices
    {
        JugadasViewModel GetJugadaById(long jugadaId);
        List<AceptacionesViewModels> GetAceptacionesByUser(string usuario);
        List<AceptacionesViewModels> GetAceptacionesByUserFecha(string usuario, DateTime fecha);
        List<AceptacionesViewModels> GetAceptacionesByJugadas(long jugadaId);
        List<AceptacionesViewModels> GetAceptacionesByBanqueadas(long banqueadaId);
        List<AceptacionesViewModels> GetAceptacionesByUsuarioJugadas(string usuario, long jugadaId);
        List<AceptacionesViewModels> GetAceptacionesByUsuarioBanquedas(string usuario, long banqueadaId);
        decimal TotalAceptacionesPorJugada(string usuario, long jugadaId);
        decimal TotalAceptacionesPorBanqueda(string usuario, long banqueadaId);
        List<JugadasViewModel> GetNewJugadas(Dictionary<string, List<string>> jugadasList, string hipodromoId, string usuario, string status, string filtro, DateTime fecha);
        List<JugadasViewModel> GetNewAceptacionesJugadas(Dictionary<string, List<string>> jugadasList, string usuario, DateTime fecha);
        List<JugadasViewModel> GetAllJugadasByHipodromo(string hipodromoId, DateTime fecha);
        List<JugadasViewModel> GetAllJugadasByConcretadas(DateTime fecha);
        List<JugadasViewModel> GetAllJugadasByFecha(DateTime fecha);
        List<JugadasViewModel> GetJugadasByUsuarioFecha(string usuario, DateTime fecha);
        List<JugadasViewModel> GetJugadasByOtrosFecha(string usuario, DateTime fecha);
        List<BanqueadaViewModel> GetBanqueadasByUsuarioFecha(string usuario, DateTime fecha);
        List<HipodromosViewModels> GetHipodromosSinResultados(DateTime fecha);
        Task<Result> GuardarAceptacion(AceptacionesViewModels data);
        string GetEjemplarName(string hipodromoId, int numeroCarrera, int numeroEjemplar);
        ParametersViewModel GetParametersByTableIdSemantic(string tableId, string semantic);
        ParametersViewModel GetParametersByTableIdDescription(string tableId, string description);
        List<ParametersViewModel> GetParametersByTableId(string tableId);
        List<string> GetJugadasVencidas(Dictionary<string, List<string>> jugadasList, string hipodromoId, string usuario, string status, string filtro, DateTime fecha);
        CarrerasViewModel GetCarreraInfoByJugadaId(long jugadaId);
        List<JugadasViewModel> GetJugadasByUsuarioFechaSinFiltro(string usuario, DateTime fecha);
    }
}
