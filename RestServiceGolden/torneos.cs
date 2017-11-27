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
    
    public partial class torneos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public torneos()
        {
            this.equipos = new HashSet<equipos>();
            this.fotos = new HashSet<fotos>();
            this.goleadores = new HashSet<goleadores>();
            this.horarios_fijos = new HashSet<horarios_fijos>();
            this.noticias = new HashSet<noticias>();
            this.sanciones_torneo = new HashSet<sanciones_torneo>();
            this.sponsors = new HashSet<sponsors>();
            this.equipos_zona = new HashSet<equipos_zona>();
            this.fixture = new HashSet<fixture>();
        }
    
        public int id_torneo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public Nullable<System.DateTime> fecha_inicio { get; set; }
        public Nullable<System.DateTime> fecha_fin { get; set; }
        public Nullable<int> id_tipo { get; set; }
        public Nullable<int> id_categoria { get; set; }
        public Nullable<int> id_regla { get; set; }
        public Nullable<int> id_modalidad { get; set; }
    
        public virtual categorias categorias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<equipos> equipos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<fotos> fotos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<goleadores> goleadores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<horarios_fijos> horarios_fijos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<noticias> noticias { get; set; }
        public virtual reglas_torneo reglas_torneo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sanciones_torneo> sanciones_torneo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sponsors> sponsors { get; set; }
        public virtual tipos_torneos tipos_torneos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<equipos_zona> equipos_zona { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<fixture> fixture { get; set; }
        public virtual modalidades modalidades { get; set; }
    }
}
