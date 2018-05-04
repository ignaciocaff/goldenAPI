using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class IJugador
    {
        public String nombre { get; set; }
        public String apellido { get; set; }
        public int id_persona { get; set; }
        public int id_equipo { get; set; }
        public int nro_doc { get; set; }
        public String imagePath { get; set; }
        public String rol { get; set; }
        public int edad { get; set; }
        public int id_jugador { get; set; }
        public int goles { get; set; }
        public int partidos_jugados { get; set; }
        public int tarjetas_amarillas { get; set; }
        public int tarjetas_rojas { get; set; }
        public string ultima_roja { get; set; }
    }
}