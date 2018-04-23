using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class SancionEquipo
    {
        public int? id_sancion_equipo { get; set; }
        public string descripcion { get; set; }
        public int puntos_restados { get; set; }
        public Equipo equipo { get; set; }
        public Zona zona { get; set; }
        public Torneo torneo { get; set; }
    }
}