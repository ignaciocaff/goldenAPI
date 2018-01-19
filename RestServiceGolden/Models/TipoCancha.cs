using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class TipoCancha
    {
        public int? id_tipo { get; set; }
        public string descripcion { get; set; }
        public Cancha cancha { get; set; }
    }
}