using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Sancion
    {
        public int? id_sancion { get; set; }
        public Fecha fecha_inicio { get; set; }
        public Fecha fecha_fin { get; set; }
        public Jugador jugador { get; set; }
        public Equipo equipo { get; set; }
        public Partido partido { get; set; }
        public TipoSancion tipo_sancion { get; set; }
        public String detalle { get; set; }
        public Zona zona { get; set; }
        public Fase fase { get; set; }
    }
}