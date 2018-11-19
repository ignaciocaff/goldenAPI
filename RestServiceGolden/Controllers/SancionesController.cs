using RestServiceGolden.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace RestServiceGolden.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SancionesController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/sanciones/tipos")]
        public IHttpActionResult GetAllTipos()
        {
            List<TipoSancion> lsTipoSanciones = new List<TipoSancion>();
            try
            {
                var tipos_sanciones = db.tipos_sanciones.ToList().OrderBy(z => z.id_tipo);

                foreach (var tSancion in tipos_sanciones)
                {
                    TipoSancion tipo_sancion = new TipoSancion(); ;
                    tipo_sancion.id_tipo = tSancion.id_tipo;
                    tipo_sancion.descripcion = tSancion.descripcion;
                    lsTipoSanciones.Add(tipo_sancion);
                }
                return Ok(lsTipoSanciones);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/sanciones/zonaPorEquipo/{id_equipo}")]
        public IHttpActionResult getZonaPorEquipo(int id_equipo)
        {
            Zona zona = new Zona();
            try
            {
                var equipo_zona = db.equipos_zona.Where(x => x.id_equipo == id_equipo).FirstOrDefault();
                zona.id_zona = equipo_zona.id_zona;
                return Ok(zona);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/sanciones/ultimaSancion/{id_jugador}")]
        public IHttpActionResult getUltimaSancion(int id_jugador)
        {
            Sancion sancionDto = new Sancion();
            try
            {
                var sancion = db.sanciones.Where(x => x.id_jugador == id_jugador).OrderByDescending(y => y.id_sancion).FirstOrDefault();
                Fecha fechaInicio = new Fecha();
                Fecha fechaFin = new Fecha();
                TipoSancion tipoSancion = new TipoSancion();
                Jugador jugador = new Jugador();
                Zona zona = new Zona();

                if (sancion != null)
                {
                    sancionDto.id_sancion = sancion.id_sancion;
                    sancionDto.jugador = jugador;
                    sancionDto.jugador.id_jugador = id_jugador;
                    sancionDto.tipo_sancion = tipoSancion;
                    sancionDto.tipo_sancion.id_tipo = sancion.id_tipo;
                    sancionDto.tipo_sancion.descripcion = db.tipos_sanciones.Where(x => x.id_tipo == sancion.id_tipo).FirstOrDefault().descripcion;
                    sancionDto.fecha_inicio = fechaInicio;
                    sancionDto.fecha_fin = fechaFin;
                    sancionDto.fecha_inicio.fecha = (DateTime)db.fechas.Where(x => x.id_fecha == sancion.fecha_inicio).FirstOrDefault().fecha;
                    sancionDto.fecha_fin.fecha = (DateTime)db.fechas.Where(x => x.id_fecha == sancion.fecha_fin).FirstOrDefault().fecha;
                    sancionDto.detalle = sancion.detalle;
                }
                return Ok(sancionDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/sanciones/modificarUltimaSancion")]
        public IHttpActionResult modificarUltimaSancion([FromBody]Sancion sancion)
        {
            try
            {
                var sancionUpdate = db.sanciones.SingleOrDefault(x => x.id_sancion == sancion.id_sancion);
                sancionUpdate.id_tipo = sancion.tipo_sancion.id_tipo;
                sancionUpdate.fecha_fin = sancion.fecha_fin.id_fecha;
                sancionUpdate.detalle = sancion.detalle;
                db.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/sanciones/acumuladoJugador/{id_torneo}/{id_jugador}")]
        public IHttpActionResult getAcumuladoJugador(int id_torneo, int id_jugador)
        {
            List<Sancion> lsSanciones = new List<Sancion>();
            try
            {
                var lsSancion = db.sanciones.Where(x => x.id_jugador == id_jugador && x.id_torneo == id_torneo).ToList();


                foreach (var sancionD in lsSancion)
                {
                    Sancion sancion = new Sancion();
                    Jugador jugador = new Jugador();
                    Zona zona = new Zona();
                    TipoSancion tipo_sancion = new TipoSancion();
                    sancion.jugador = jugador;
                    sancion.jugador.id_jugador = sancionD.id_jugador;
                    sancion.zona = zona;
                    sancion.zona.id_zona = sancionD.id_zona;
                    sancion.tipo_sancion = tipo_sancion;
                    sancion.tipo_sancion.id_tipo = sancionD.id_tipo;
                    lsSanciones.Add(sancion);

                }
                return Ok(lsSanciones);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}