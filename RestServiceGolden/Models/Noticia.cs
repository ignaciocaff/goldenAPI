using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Noticia
    {
        public int? id_noticia { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public DateTime fecha { get; set; }
        public Torneo torneo { get; set; }
        public Club club { get; set; }
        //public Fecha fecha { get; set; }
        public CategoriaNoticia categoriaNoticia { get; set; }
        public string tags { get; set; }
        public int id_thumbnail { get; set; }
    }
}