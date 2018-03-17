using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Cancha
    {
        public int id_cancha { get; set; }
        public string nombre { get; set; }
        public int capacidad { get; set; }
        public Domicilio domicilio { get; set; }
        public Club club { get; set; }
    }
}