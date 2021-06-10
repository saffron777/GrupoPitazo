using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GP.Core.Entities
{
    [Table("Notificaciones")]
    public class Notificaciones
    {
        [Key]
        public long NotificacionID { get; set; }
        [StringLength(50)]
        public string Usuario { get; set; }
        [StringLength(50)]
        public string UsuarioDestino { get; set; }
        [StringLength(4000)]
        public string Mensaje { get; set; }

        public DateTime Fecha { get; set; }

    }
}
