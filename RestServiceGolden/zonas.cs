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
    
    public partial class zonas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public zonas()
        {
            this.equipos_zona = new HashSet<equipos_zona>();
            this.posiciones_zona = new HashSet<posiciones_zona>();
        }
    
        public int id_zona { get; set; }
        public string descripcion { get; set; }
        public Nullable<int> id_torneo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<equipos_zona> equipos_zona { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<posiciones_zona> posiciones_zona { get; set; }
        public virtual torneos torneos { get; set; }
    }
}
