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