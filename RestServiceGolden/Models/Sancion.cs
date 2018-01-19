using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Sancion
    {
        public int? id_sancion { get; set; }
        public Jugador jugador { get; set; }
        public DateTime minuto { get; set; }
        public TipoSancion tipo_sancion { get; set; }
    }
}