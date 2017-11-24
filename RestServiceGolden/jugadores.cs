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
    
    public partial class jugadores
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public jugadores()
        {
            this.goleadores = new HashSet<goleadores>();
            this.goles = new HashSet<goles>();
            this.sanciones = new HashSet<sanciones>();
            this.sanciones_torneo = new HashSet<sanciones_torneo>();
        }
    
        public int id_jugador { get; set; }
        public Nullable<int> numero { get; set; }
        public Nullable<System.DateTime> fecha_alta { get; set; }
        public Nullable<int> id_persona { get; set; }
        public Nullable<int> id_equipo { get; set; }
    
        public virtual equipos equipos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<goleadores> goleadores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<goles> goles { get; set; }
        public virtual personas personas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sanciones> sanciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sanciones_torneo> sanciones_torneo { get; set; }
    }
}
