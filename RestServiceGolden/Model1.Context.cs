﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class goldenEntities : DbContext
    {
        public goldenEntities()
            : base("name=goldenEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<arbitros> arbitros { get; set; }
        public virtual DbSet<canchas> canchas { get; set; }
        public virtual DbSet<categoria_equipos> categoria_equipos { get; set; }
        public virtual DbSet<categorias> categorias { get; set; }
        public virtual DbSet<categorias_noticias> categorias_noticias { get; set; }
        public virtual DbSet<clubes> clubes { get; set; }
        public virtual DbSet<contactos> contactos { get; set; }
        public virtual DbSet<domicilios> domicilios { get; set; }
        public virtual DbSet<equipos> equipos { get; set; }
        public virtual DbSet<equipos_zona> equipos_zona { get; set; }
        public virtual DbSet<estado_fecha> estado_fecha { get; set; }
        public virtual DbSet<estados_civil> estados_civil { get; set; }
        public virtual DbSet<estados_partidos> estados_partidos { get; set; }
        public virtual DbSet<fechas> fechas { get; set; }
        public virtual DbSet<files> files { get; set; }
        public virtual DbSet<fixture> fixture { get; set; }
        public virtual DbSet<fixture_zona> fixture_zona { get; set; }
        public virtual DbSet<fotos> fotos { get; set; }
        public virtual DbSet<goleadores> goleadores { get; set; }
        public virtual DbSet<goles> goles { get; set; }
        public virtual DbSet<horarios_fijos> horarios_fijos { get; set; }
        public virtual DbSet<jugadores> jugadores { get; set; }
        public virtual DbSet<localidades> localidades { get; set; }
        public virtual DbSet<logs> logs { get; set; }
        public virtual DbSet<modalidades> modalidades { get; set; }
        public virtual DbSet<nacionalidades> nacionalidades { get; set; }
        public virtual DbSet<noticias> noticias { get; set; }
        public virtual DbSet<partidos> partidos { get; set; }
        public virtual DbSet<perfiles> perfiles { get; set; }
        public virtual DbSet<permisos> permisos { get; set; }
        public virtual DbSet<personas> personas { get; set; }
        public virtual DbSet<posiciones> posiciones { get; set; }
        public virtual DbSet<posiciones_zona> posiciones_zona { get; set; }
        public virtual DbSet<provincias> provincias { get; set; }
        public virtual DbSet<reglamentos> reglamentos { get; set; }
        public virtual DbSet<reglas_torneo> reglas_torneo { get; set; }
        public virtual DbSet<representante_equipo> representante_equipo { get; set; }
        public virtual DbSet<restricciones> restricciones { get; set; }
        public virtual DbSet<resultados> resultados { get; set; }
        public virtual DbSet<resultados_zona> resultados_zona { get; set; }
        public virtual DbSet<sanciones> sanciones { get; set; }
        public virtual DbSet<sanciones_torneo> sanciones_torneo { get; set; }
        public virtual DbSet<sponsors> sponsors { get; set; }
        public virtual DbSet<tipos_cancha> tipos_cancha { get; set; }
        public virtual DbSet<tipos_documento> tipos_documento { get; set; }
        public virtual DbSet<tipos_fixture> tipos_fixture { get; set; }
        public virtual DbSet<tipos_sanciones> tipos_sanciones { get; set; }
        public virtual DbSet<tipos_torneos> tipos_torneos { get; set; }
        public virtual DbSet<torneos> torneos { get; set; }
        public virtual DbSet<turnos> turnos { get; set; }
        public virtual DbSet<usuarios> usuarios { get; set; }
        public virtual DbSet<veedores> veedores { get; set; }
        public virtual DbSet<zonas> zonas { get; set; }
        public virtual DbSet<sanciones_equipo> sanciones_equipo { get; set; }
        public virtual DbSet<fases> fases { get; set; }
    }
}
