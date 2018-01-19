using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Representante
    {
        public int? id { get; set; }
        public Usuario usuario { get; set; }
        public Equipo equipo { get; set; }
    }
}