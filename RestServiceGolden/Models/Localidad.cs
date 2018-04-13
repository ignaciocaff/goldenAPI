using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Localidad
    {
        public int? id_localidad { get; set; }
        public string n_localidad { get; set; }
        public DateTime fecha_alta { get; set; }
        public DateTime fecha_modificacion { get; set; }
        public DateTime fecha_baja { get; set; }

        public Localidad(int? id_localidad, string n_localidad)
        {
            this.id_localidad = id_localidad;
            this.n_localidad = n_localidad;
        }

        public Localidad()
        {
        }
    }
}