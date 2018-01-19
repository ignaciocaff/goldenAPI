using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class SancionesTorneo
    {
        public int? id_sancion { get; set; }
        public Jugador jugador { get; set; }
        public int cantidad_amarillas { get; set; }
        public int cantidad_rojas { get; set; }
        public Torneo torneo { get; set; }
    }
}