using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Playoff
    {
        public int id_playoff { get; set; }
        public Llave llave { get; set; }
        public IEquipo local { get; set; }
        public IEquipo visitante { get; set; }
        public IEquipo ganador { get; set; }
        public Etapa etapa { get; set; }
        public Torneo torneo { get; set; }
        public Partido partido { get; set; }
    }
}