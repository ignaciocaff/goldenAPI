using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class ReglaTorneo
    {
        public int? id_regla { get; set; }
        public string descripcion { get; set; }
        public Torneo torneo { get; set; }
    }
}