using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class EstadoCivil
    {
        public int? id_estado_civil { get; set; }
        public string n_estado_civil { get; set; }
        public DateTime fecha_alta { get; set; }
        public DateTime fecha_modificacion { get; set; }
        public DateTime fecha_baja { get; set; }
    }
}