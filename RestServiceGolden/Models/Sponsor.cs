using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace RestServiceGolden.Models
{
    public class Sponsor
    {
        public int? id_sponsor { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public Image logo { get; set; }
        public Torneo torneo { get; set; }
        public Club club { get; set; }
    }
}