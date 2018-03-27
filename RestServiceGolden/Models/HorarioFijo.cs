using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class HorarioFijo
    {
        public int? id_horario { get; set; }
        public Turno turno { get; set; }
        public String inicio { get; set; }
        public String fin { get; set; }
    }
}