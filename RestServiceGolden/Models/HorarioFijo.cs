using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class HorarioFijo
    {
        public int? id_horario { get; set; }
        public string turno { get; set; }
        public DateTime inicio { get; set; }
        public DateTime fin { get; set; }
        public Torneo torneo { get; set; }
    }
}