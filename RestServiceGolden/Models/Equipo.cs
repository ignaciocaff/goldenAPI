using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace RestServiceGolden.Models
{
    public class Equipo
    {
        public int? id_equipo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public DateTime fecha_alta { get; set; }
        public int logo { get; set; }
        public Categoria categoria { get; set; }
        public Club club { get; set; }
        public Torneo torneo { get; set; }
    }
}