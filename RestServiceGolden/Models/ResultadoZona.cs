using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class ResultadoZona
    {
        public int id_resultado { get; set; }
        public Equipo ganador { get; set; }
        public Equipo perdedor { get; set; }
        public int? empate { get; set; }
        public Zona zona { get; set; }
    }
}