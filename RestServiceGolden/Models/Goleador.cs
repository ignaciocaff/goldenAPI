using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Goleador
    {
        public int? id_goleador { get; set; }
        public Torneo torneo { get; set; }
        public Jugador jugador { get; set; }
        public int cantidad_goles { get; set; }
        public Equipo equipo { get; set; }
    }
}