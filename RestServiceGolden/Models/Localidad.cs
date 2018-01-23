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
        public Provincia provincia { get; set; }
    }
}