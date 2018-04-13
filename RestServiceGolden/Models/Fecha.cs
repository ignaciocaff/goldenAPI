using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Fecha
    {
        public int id_fecha { get; set; }
        public DateTime fecha { get; set; }
        public EstadoFecha estado { get; set; }
        public List<Partido> partidos { get; set; }
        public List<IPartido> iPartidos { get; set; }
        public Fixture fixture { get; set; }

    }
}