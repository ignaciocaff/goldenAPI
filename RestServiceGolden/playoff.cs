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
    
    public partial class playoff
    {
        public int id_playoff { get; set; }
        public Nullable<int> llave { get; set; }
        public Nullable<int> local { get; set; }
        public Nullable<int> visitante { get; set; }
        public Nullable<int> ganador { get; set; }
        public Nullable<int> id_etapa { get; set; }
        public Nullable<int> id_torneo { get; set; }
    
        public virtual equipos equipos { get; set; }
        public virtual equipos equipos1 { get; set; }
        public virtual equipos equipos2 { get; set; }
        public virtual etapa_playoff etapa_playoff { get; set; }
        public virtual llaves llaves { get; set; }
        public virtual torneos torneos { get; set; }
    }
}