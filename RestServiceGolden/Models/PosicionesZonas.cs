using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class PosicionesZonas
    {
        public int? id_posicion { get; set; }
        public IEquipo equipo { get; set; }
        public int puntos { get; set; }
        public int goles_favor { get; set; }
        public int goles_contra { get; set; }
        public int dif_gol { get; set; }
        public Torneo torneo { get; set; }
        public Zona zona { get; set; }
        public int partidos_jugados { get; set; }
        public int partidos_ganados { get; set; }
        public int partidos_empatados { get; set; }
        public int partidos_perdidos { get; set; }
    }
}