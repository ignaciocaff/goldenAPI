using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Perfil
    {
        public int id_perfil { get; set; }
        public string n_perfil { get; set; }
        public List<Permiso> lsPermisos { get; set; }
    }
}