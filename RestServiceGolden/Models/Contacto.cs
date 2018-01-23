using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Contacto
    {
        public int? id_contacto { get; set; }
        public string telefono_fijo { get; set; }
        public string telefono_movil { get; set; }
        public string email { get; set; }
    }
}