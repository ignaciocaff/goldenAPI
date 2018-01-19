using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Arbitro: Persona
    {
        public int? id_arbitro { get; set; }
        public int matricula { get; set; }
    }
}