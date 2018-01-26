using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Torneo
    {
        public int? id_torneo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public DateTime fecha_inicio { get; set; }
        public DateTime fecha_fin { get; set; }
        public Categoria categoria { get; set; }
        public Regla regla { get; set; }
        public TipoTorneo tipoTorneo { get; set; }
        public Modalidad modalidad { get; set; }
        public List<Equipo> lsEquipos { get; set; }
    }
}