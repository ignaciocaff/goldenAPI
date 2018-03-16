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
    public class EquipoController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/equipo/registrar")]
        public IHttpActionResult registrar([FromBody]Equipo equipo)
        {
            equipos equipoDto = new equipos();
            categorias categoria = new categorias();
            clubes club = new clubes();

            equipoDto.nombre = equipo.nombre;
            equipoDto.descripcion = equipo.descripcion;
            equipoDto.fecha_alta = DateTime.Now;
            equipoDto.logo = equipo.logo;
            equipoDto.camiseta = equipo.camiseta;
            equipoDto.camisetalogo = equipo.camisetalogo;
            equipoDto.id_club = equipo.club.id_club;
            equipoDto.id_categoria_equipo = equipo.categoria.id_categoria;
            equipoDto.id_torneo = equipo.torneo.id_torneo;

            int equiposCheck = db.equipos.Where(x => x.nombre.ToUpper().Equals(equipoDto.nombre.ToUpper()) && x.id_categoria_equipo == equipoDto.id_categoria_equipo).Count();

            if (equiposCheck == 0)
            {
                db.equipos.Add(equipoDto);
                db.SaveChanges();
                return Ok();
            }

            return BadRequest("Ya existe un equipo registrado para esta categoría con ese nombre.");
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/equipo/desvincular")]
        public IHttpActionResult desvincular([FromBody]List<Equipo> lsEquipos)
        {
            try
            {
                foreach (Equipo e in lsEquipos)
                {
                    equipos equipoToUpdate = db.equipos.Where(x => x.id_equipo == e.id_equipo).FirstOrDefault();
                    equipoToUpdate.id_torneo = null;
                    db.SaveChanges();
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("No se pudieron desvincular los equipos");
            }
        }

        [ResponseType(typeof(Equipo))]
        [Route("api/equipo/obtenerTodos")]
        public IHttpActionResult getAll()
        {
            List<Equipo> lsEquipos = new List<Equipo>();

            try
            {
                var equipos = db.equipos.ToList();

                foreach (var tEquipo in equipos)
                {
                    var torneoDb = db.torneos.Where(x => x.id_torneo == tEquipo.id_torneo).FirstOrDefault();
                    var categoriaDb = db.categorias.Where(x => x.id_categoria == tEquipo.id_categoria_equipo).FirstOrDefault();
                    Equipo equipo = new Equipo();
                    Categoria categoria = new Categoria();
                    Torneo torneo = new Torneo();
                    Club club = new Club();
                    equipo.id_equipo = tEquipo.id_equipo;
                    equipo.nombre = tEquipo.nombre;
                    equipo.descripcion = tEquipo.descripcion;
                    equipo.fecha_alta = Convert.ToDateTime(tEquipo.fecha_alta);
                    equipo.logo = (tEquipo.logo != null) ? tEquipo.logo.Value : 0;
                    equipo.camiseta = (tEquipo.camiseta != null) ? tEquipo.camiseta.Value : 0;
                    equipo.camisetalogo = (tEquipo.camisetalogo != null) ? tEquipo.camisetalogo.Value : 0;
                    equipo.categoria = categoria;
                    equipo.club = club;
                    equipo.torneo = torneo;
                    equipo.categoria.id_categoria = (int)tEquipo.id_categoria_equipo;
                    equipo.club.id_club = tEquipo.id_club;
                    equipo.torneo.id_torneo = tEquipo.id_torneo;
                    equipo.torneo.nombre = (torneoDb != null) ? torneoDb.nombre : null;
                    equipo.categoria.descripcion = categoriaDb.descripcion;
                    lsEquipos.Add(equipo);
                }
                return Ok(lsEquipos);
            }
            catch (Exception e)
            {
                e.ToString();
                Console.WriteLine(e.ToString());
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/equipo/jugadores/{id}")]
        public IHttpActionResult GetJugadoresByIdEquipo(int id)
        {
            try
            {
                var personas = (from tPersonas in db.personas
                                join tTiposDocumento in db.tipos_documento on tPersonas.id_tipo_documento equals tTiposDocumento.id_tipo_documento
                                join tDomicilio in db.domicilios on tPersonas.id_domicilio equals tDomicilio.id_domicilio
                                join tContacto in db.contactos on tPersonas.id_contacto equals tContacto.id_contacto
                                join tLocalidad in db.localidades on tDomicilio.id_localidad equals tLocalidad.id_localidad
                                join tProvincias in db.provincias on tLocalidad.id_provincia equals tProvincias.id_provincia
                                join tJugador in db.jugadores on tPersonas.id_persona equals tJugador.id_persona
                                where tJugador.id_equipo == id
                                select new
                                {
                                    nombre = tPersonas.nombre,
                                    apellido = tPersonas.apellido,
                                    documento = tPersonas.nro_documento,
                                    id_equipo = tJugador.id_equipo,
                                    id_jugador = tJugador.id_jugador,
                                });

                List<Jugador> lsJugadores = new List<Jugador>();
                foreach (var p in personas)
                {
                    Jugador jugador = new Jugador();
                    Equipo equipo = new Equipo();
                    jugador.nro_documento = Convert.ToInt32(p.documento);
                    jugador.nombre = p.nombre;
                    jugador.apellido = p.apellido;
                    jugador.equipo = equipo;
                    jugador.equipo.id_equipo = p.id_equipo;
                    jugador.id_jugador = p.id_jugador;

                    lsJugadores.Add(jugador);
                }
                return Ok(lsJugadores);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/equipo/desvincular/jugadores/{id_jugador}")]
        public IHttpActionResult getEliminarJugadorPorEquipo(int id_jugador)
        {
            try
            {
                var result = db.jugadores.SingleOrDefault(b => b.id_jugador == id_jugador);
                if (result != null)
                {
                    result.id_equipo = null;
                    db.SaveChanges();
                }
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [Route("api/equipo/update")]
        public IHttpActionResult update([FromBody]Equipo equipo)
        {
            equipos equipoDto = new equipos();

            try
            {
                equipoDto.nombre = equipo.nombre;
                equipoDto.descripcion = equipo.descripcion;
                equipoDto.logo = equipo.logo;
                equipoDto.camiseta = equipo.camiseta;
                equipoDto.camisetalogo = equipo.camisetalogo;
                equipoDto.id_torneo = equipo.torneo.id_torneo;
                equipoDto.id_categoria_equipo = equipo.categoria.id_categoria;
                equipoDto.id_equipo = (int)equipo.id_equipo;

                var result = db.equipos.SingleOrDefault(b => b.id_equipo == equipoDto.id_equipo);
                if (result != null)
                {
                    result.nombre = equipoDto.nombre;
                    result.descripcion = equipoDto.descripcion;
                    result.logo = equipoDto.logo;
                    result.camiseta = equipoDto.camiseta;
                    result.camisetalogo = equipoDto.camisetalogo;
                    result.id_torneo = equipoDto.id_torneo;
                    result.id_categoria_equipo = equipoDto.id_categoria_equipo;
                    result.id_equipo = equipoDto.id_equipo;
                    db.SaveChanges();
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}