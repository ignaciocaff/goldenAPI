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
    
    public partial class fixture_zona
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public fixture_zona()
        {
            this.fechas = new HashSet<fechas>();
        }
    
        public int id_fixture { get; set; }
        public Nullable<int> id_torneo { get; set; }
        public Nullable<int> id_tipo { get; set; }
        public Nullable<int> id_zona { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<fechas> fechas { get; set; }
        public virtual torneos torneos { get; set; }
        public virtual tipos_fixture tipos_fixture { get; set; }
        public virtual zonas zonas { get; set; }
    }
}
