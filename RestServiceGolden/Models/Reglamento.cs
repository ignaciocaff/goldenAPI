using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Reglamento
    {
        public int? id_reglamento {get; set;}
        public string descripcion { get; set; }
        public int id_torneo { get; set; }
    }
}