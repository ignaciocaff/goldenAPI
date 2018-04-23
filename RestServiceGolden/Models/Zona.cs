using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Zona
    {
        public int? id_zona { get; set; }
        public string descripcion { get; set; }
        public Torneo torneo { get; set; }
        public List<Equipo> lsEquipos { get; set; }
        public Fase fase { get; set; }
    }
}