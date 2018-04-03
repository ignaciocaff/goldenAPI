using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Fixture
    {
        public int id_fixture { get; set; }
        public Torneo torneo { get; set; }
        public List<Fecha> fechas { get; set; }
        public TipoFixture tipo_fixture { get; set; }
    }
}