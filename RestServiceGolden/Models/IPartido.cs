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
        public IEquipo equipoLocal { get; set; }
        public IEquipo equipoVisitante { get; set; }
        public HorarioFijo horario_fijo { get; set; }
        public Cancha cancha { get; set; }
        public HorarioFijo horario { get; set; }
        public Fecha fecha { get; set; }
        public int? id_fixture { get; set; }
        public Resultado resultado { get; set; }
        public ResultadoZona resultado_zona { get; set; }
        public List<Gol> lsGolesLocal { get; set; }
        public List<Gol> lsGolesVisitante { get; set; }
        public List<Sancion> lsSancionesLocal { get; set; }
        public List<Sancion> lsSancionesVisitante { get; set; }
        public Llave llave { get; set; }
        public Etapa etapa { get; set; }
        public IEquipo ganadorPlayoff { get; set; }
        public Boolean penales { get; set; }
        public String detallePenales { get; set; }
    }
}