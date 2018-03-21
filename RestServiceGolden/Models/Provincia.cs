using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Provincia
    {
        public int? id_provincia { get; set; }
        public string n_provincia { get; set; }
        public DateTime fecha_alta { get; set; }
        public DateTime fecha_modificacion { get; set; }
        public DateTime fecha_baja { get; set; }
        public List<Localidad> lsLocalidades { get; set; }

        public Provincia(int? id_provincia, string n_provincia)
        {
            this.id_provincia = id_provincia;
            this.n_provincia = n_provincia;
        }

        public Provincia()
        {
        }
    }
}