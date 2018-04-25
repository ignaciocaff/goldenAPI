using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class IPartido
    {
        public int? id_partido { get; set; }
        public List<IEquipo> local { get; set; }
        public List<IEquipo> visitante { get; set; }
        public Cancha cancha { get; set; }
        public HorarioFijo horario { get; set; }
        public Fecha fecha { get; set; }
        public int? id_fixture { get; set; }

        public List<Gol> lsGolesLocal { get; set; }
        public List<Gol> lsGolesVisitante { get; set; }
        public List<Sancion> lsSancionesLocal { get; set; }
        public List<Sancion> lsSancionesVisitante { get; set; }
    }
}