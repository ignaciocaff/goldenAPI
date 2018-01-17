using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Domicilio
    {
        public int? id_domicilio { get; set; }
        public string calle { get; set; }
        public int numeracion { get; set; }
        public int? piso { get; set; }
        public string dpto { get; set; }
        public int? torre { get; set; }
        public Localidad localidad { get; set; }
        public string barrio { get; set; }
        public string observaciones { get; set; }
    }
}