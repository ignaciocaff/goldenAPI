﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Posiciones
    {
        public int? id_posicion { get; set; }
        public Equipo equipo { get; set; }
        public int puntos { get; set; }
        public int goles_favor { get; set; }
        public int goles_contra { get; set; }
        public int dif_gol { get; set; }
        public Torneo torneo { get; set; }
    }
}