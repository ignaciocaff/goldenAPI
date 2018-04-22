using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Partido
    {
        public int? id_partido { get; set; }
        public String duracion { get; set; }
        public Fecha fecha { get; set; }
        public String inicio { get; set; }
        public String fin { get; set; }
        public Equipo local { get; set; }
        public Equipo visitante { get; set; }
        public Arbitro arbitro { get; set; }
        public EstadoPartido estado { get; set; }
        public Cancha cancha { get; set; }
        public Veedor veedor { get; set; }
        public Resultado resultado { get; set; }
        public ResultadoZona resultado_zona { get; set; }
        public HorarioFijo horario_fijo { get; set; }
        public List<Gol> lsGoleadoresVisitantes { get; set; }
        public List<Gol> lsGoleadoresLocales { get; set; }
        public List<Sancion> lsSancionesLocal { get; set; }
        public List<Sancion> lsSancionesVisitante { get; set; }
        public Llave llave { get; set; }
    }
}