using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class ParametrosFixture
    {
        public Zona zona { get; set; }
        public int id_torneo { get; set; }
        public int id_fase { get; set; }
        public int cantidadDiasEntrePartidos { get; set; }
        public TipoFixture tipoDeFixture { get; set; }
        public Boolean esInterzonal { get; set; }
        public Boolean intercalarLocalVisitante { get; set; }
        public DateTime fechaInicioFixture { get; set; }
    }
}