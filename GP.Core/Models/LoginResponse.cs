using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GP.Core.Models
{
    [XmlRoot(ElementName = "Data")]
    public class LoginResponse
    {
        [XmlElement(ElementName = "Existe")]
        public string Existe { get; set; }
        [XmlElement(ElementName = "CuentaCliente")]
        public string CuentaCliente { get; set; }
        [XmlElement(ElementName = "IDdeCliente")]
        public string IDdeCliente { get; set; }
        [XmlElement(ElementName = "ClaveDeCliente")]
        public string ClaveDeCliente { get; set; }
        [XmlElement(ElementName = "CorreoDeCliente")]
        public string CorreoDeCliente { get; set; }
        [XmlElement(ElementName = "BalanceDeCliente")]
        public string BalanceDeCliente { get; set; }
        [XmlElement(ElementName = "BalancePendiente")]
        public string BalancePendiente { get; set; }
        [XmlElement(ElementName = "BalanceDisponible")]
        public string BalanceDisponible { get; set; }
        [XmlElement(ElementName = "BalanceDisponibleConMatchPlay")]
        public string BalanceDisponibleConMatchPlay { get; set; }
        [XmlElement(ElementName = "Credito")]
        public string Credito { get; set; }
        [XmlElement(ElementName = "CreditoTemporal")]
        public string CreditoTemporal { get; set; }
        [XmlElement(ElementName = "SaldoOCredito")]
        public string SaldoOCredito { get; set; }
        [XmlElement(ElementName = "UseKiosk")]
        public string UseKiosk { get; set; }
        [XmlElement(ElementName = "PostUp")]
        public string PostUp { get; set; }
        [XmlElement(ElementName = "Compania")]
        public string Compania { get; set; }
        [XmlElement(ElementName = "Paisdependencia")]
        public string Paisdependencia { get; set; }
        [XmlElement(ElementName = "idPaisdependencia")]
        public string IdPaisdependencia { get; set; }
        [XmlElement(ElementName = "Agente")]
        public string Agente { get; set; }
        [XmlElement(ElementName = "idAgente")]
        public string IdAgente { get; set; }
        [XmlElement(ElementName = "NombreUser")]
        public string NombreUser { get; set; }
        [XmlElement(ElementName = "Nombre")]
        public string Nombre { get; set; }
        [XmlElement(ElementName = "apellido")]
        public string Apellido { get; set; }
        [XmlElement(ElementName = "Cedula")]
        public string Cedula { get; set; }
        [XmlElement(ElementName = "FechaNacimiento")]
        public string FechaNacimiento { get; set; }
        [XmlElement(ElementName = "Direccion")]
        public string Direccion { get; set; }
        [XmlElement(ElementName = "Ciudad")]
        public string Ciudad { get; set; }
        [XmlElement(ElementName = "Ciudadcodigo")]
        public string Ciudadcodigo { get; set; }
        [XmlElement(ElementName = "idCiudad")]
        public string IdCiudad { get; set; }
        [XmlElement(ElementName = "Pais")]
        public string Pais { get; set; }
        [XmlElement(ElementName = "Moneda")]
        public string Moneda { get; set; }
        [XmlElement(ElementName = "Monedadef")]
        public string Monedadef { get; set; }
        [XmlElement(ElementName = "Telefono")]
        public string Telefono { get; set; }
        [XmlElement(ElementName = "pin")]
        public string Pin { get; set; }
        [XmlElement(ElementName = "Activo")]
        public string Activo { get; set; }
        [XmlElement(ElementName = "esdemo")]
        public string Esdemo { get; set; }
        [XmlElement(ElementName = "logeado")]
        public string Logeado { get; set; }
        [XmlElement(ElementName = "ConfigBotones")]
        public string ConfigBotones { get; set; }
        [XmlElement(ElementName = "statuslog")]
        public string Statuslog { get; set; }
    }
}
