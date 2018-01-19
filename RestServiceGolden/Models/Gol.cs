using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Gol
    {
        public int? id_gol { get; set; }
        public DateTime minuto { get; set; }
        public Jugador jugador { get; set; }
        public Partido partido { get; set; }
        public Equipo equipo { get; set; }
    }
}