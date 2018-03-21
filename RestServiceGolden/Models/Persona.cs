using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Persona
    {
        public string nombre { get; set; }
        public string apellido { get; set; }
        public DateTime fecha_nacimiento  { get; set; }
        public int nro_documento { get; set; }
        public TipoDocumento tipoDocumento { get; set; }
        public Domicilio domicilio { get; set; }
        public Contacto contacto { get; set; }
        public int? id_persona { get; set; }
        public int edad { get; set; }
        public string ocupacion { get; set; }
        public int id_foto { get; set; }
    }
}