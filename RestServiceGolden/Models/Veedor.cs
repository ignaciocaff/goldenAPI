using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Veedor: Persona
    {
        public int? id_veedor { get; set; }
        public DateTime fecha_alta { get; set; }
    }
}