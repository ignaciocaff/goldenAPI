using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Equipo
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public DateTime fecha_alta { get; set; }
        public Image logo { get; set; }
        public Categoria categoria { get; set; }
        public int? id_equipo { get; set; }
        public Club club { get; set; }
    }
}