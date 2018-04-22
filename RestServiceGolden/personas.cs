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
    
    public partial class personas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public personas()
        {
            this.arbitros = new HashSet<arbitros>();
            this.jugadores = new HashSet<jugadores>();
            this.veedores = new HashSet<veedores>();
        }
    
        public int id_persona { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public long nro_documento { get; set; }
        public int id_tipo_documento { get; set; }
        public System.DateTime fecha_nacimiento { get; set; }
        public Nullable<int> id_domicilio { get; set; }
        public Nullable<int> id_contacto { get; set; }
        public Nullable<int> id_estado_civil { get; set; }
        public Nullable<int> id_nacionalidad { get; set; }
        public System.DateTime fecha_alta { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_baja { get; set; }
        public string ocupacion { get; set; }
        public Nullable<int> id_foto { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<arbitros> arbitros { get; set; }
        public virtual contactos contactos { get; set; }
        public virtual domicilios domicilios { get; set; }
        public virtual estados_civil estados_civil { get; set; }
        public virtual files files { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<jugadores> jugadores { get; set; }
        public virtual nacionalidades nacionalidades { get; set; }
        public virtual tipos_documento tipos_documento { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<veedores> veedores { get; set; }
    }
}
