using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Jugador : Persona
    {
        public int? id_jugador { get; set; }
        public int numero { get; set; }
        public DateTime fecha_alta { get; set; }
        public Equipo equipo { get; set; }
        public string rol { get; set; }
        public int acumAmarillas { get; set; }
        public int acumRojas { get; set; }
        public Boolean tieneUltSancion { get; set; }
    }
}