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
    
    public partial class sanciones_torneo
    {
        public int id_sancion { get; set; }
        public Nullable<int> id_jugador { get; set; }
        public Nullable<int> cantidad_amarillas { get; set; }
        public Nullable<int> cantidad_rojas { get; set; }
        public Nullable<int> id_torneo { get; set; }
    
        public virtual jugadores jugadores { get; set; }
        public virtual torneos torneos { get; set; }
    }
}
