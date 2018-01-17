using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Permiso
    {
        public int id_permiso { get; set; }
        public string descripcion { get; set; }
        public DateTime fechaExpiracion { get; set; }
    }
}