using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class IEquipo
    {

        public int? id_equipo { get; set; }
        public String nombre { get; set; }
        public String imagePath { get; set; }
        public int? logo { get; set; }

        public IEquipo(int? id_equipo)
        {
            this.id_equipo = id_equipo;
        }

        public IEquipo()
        {
        }
    }
}