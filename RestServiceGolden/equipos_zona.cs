//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RestServiceGolden
{
    using System;
    using System.Collections.Generic;
    
    public partial class equipos_zona
    {
        public int id_equipos_zona { get; set; }
        public Nullable<int> id_equipo { get; set; }
        public Nullable<int> id_zona { get; set; }
        public Nullable<int> id_torneo { get; set; }
    
        public virtual equipos equipos { get; set; }
        public virtual torneos torneos { get; set; }
        public virtual zonas zonas { get; set; }
    }
}
