using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class ParametrosFixture
    {
        public int? id_zona { get; set; }
        public int id_torneo { get; set; }
        public int cantidadDiasEntrePartidos { get; set; }
        public int tipoDeFixture { get; set; }
        public int cantidadFechas { get; set; }
        public Boolean intercalarLocalVisitante { get; set; }
        public DateTime fechaInicioFixture { get; set; }
    }
}