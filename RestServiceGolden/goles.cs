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
    
    public partial class goles
    {
        public int id_gol { get; set; }
        public Nullable<System.TimeSpan> minuto { get; set; }
        public Nullable<int> id_jugador { get; set; }
        public Nullable<int> id_partido { get; set; }
    
        public virtual jugadores jugadores { get; set; }
        public virtual partidos partidos { get; set; }
    }
}
