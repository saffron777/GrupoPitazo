using System;
using System.Collections.Generic;
using System.Text;

namespace GP.Core.Models
{
    public class UsersResponse
    {
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public DateTime FechaCreacion { get; set; }

        public long TokenID { get; set; }

        public bool Activo { get; set; }
    }
}
