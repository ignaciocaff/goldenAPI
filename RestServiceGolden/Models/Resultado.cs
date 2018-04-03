﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Resultado
    {
        public int id_resultado { get; set; }
        public Equipo local { get; set; }
        public Equipo visitante { get; set; }
        public int empate { get; set; }

    }
}