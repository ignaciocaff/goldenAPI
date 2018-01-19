using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class TipoSancion
    {
        public int? id_tipo { get; set; }
        public string descripcion { get; set; }
        public Restriccion restriccion { get; set; }
    }
}