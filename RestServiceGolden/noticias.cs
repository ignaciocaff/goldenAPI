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
    
    public partial class noticias
    {
        public int id_noticia { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<int> id_torneo { get; set; }
        public Nullable<int> id_club { get; set; }
        public Nullable<int> id_fecha { get; set; }
    
        public virtual clubes clubes { get; set; }
        public virtual fechas fechas { get; set; }
        public virtual torneos torneos { get; set; }
    }
}
