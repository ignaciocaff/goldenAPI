using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Usuario
    {
        public int? id_usuario { get; set; }
        public string n_usuario { get; set; }
        public string password { get; set; }
        public Perfil perfil { get; set; }
    }
}