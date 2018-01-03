using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Partido
    {
        public String local { get; set; }
        public String visitante { get; set; }
        public String horario { get; set; }
        public String cancha { get; set; }
    }
}