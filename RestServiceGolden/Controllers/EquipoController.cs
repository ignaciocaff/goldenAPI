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
    public class EquipoController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/equipo/registrar")]
        public IHttpActionResult registrar([FromBody]Equipo equipo)
        {
            try
            {
                int id_equipo;
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
                    id_equipo = equipoDto.id_equipo;

                    equipos_zona equipo_zona = new equipos_zona();
                    equipo_zona.id_equipo = id_equipo;
                    equipo_zona.id_torneo = equipoDto.id_torneo;
                    equipo_zona.id_zona = null;
                    db.equipos_zona.Add(equipo_zona);
                    db.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
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

        [ResponseType(typeof(Equipo))]
        [Route("api/equipo/equiposPorZona/{id_zona}")]
        public IHttpActionResult GetJugadoresByZona(int id_zona)
        {
            List<Equipo> lsEquipos = new List<Equipo>();

            try
            {
                var equipos = (from tEquipo in db.equipos
                               join tEquipoZona in db.equipos_zona on tEquipo.id_equipo equals tEquipoZona.id_equipo
                               where tEquipoZona.id_zona == id_zona
                               select new
                               {
                                   id_torneo = tEquipo.id_torneo,
                                   id_categoria_equipo = tEquipo.id_categoria_equipo,
                                   id_equipo = tEquipo.id_equipo,
                                   nombre = tEquipo.nombre,

                                   descripcion = tEquipo.descripcion,
                                   fecha_alta = tEquipo.fecha_alta,
                                   logo = tEquipo.logo,
                                   camiseta = tEquipo.camiseta,
                                   camisetalogo = tEquipo.camisetalogo,
                                   id_club = tEquipo.id_club
                               }).ToList();


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

        [ResponseType(typeof(Equipo))]
        [Route("api/equipo/equiposSinZona/{id_torneo}")]
        public IHttpActionResult GetEquiposSinZona(int id_torneo)
        {
            List<Equipo> lsEquipos = new List<Equipo>();

            try
            {
                var equiposGrla = db.equipos.Where(x => x.id_torneo == id_torneo).ToList();
                var equipos = new List<equipos>();

                foreach (var eGrla in equiposGrla)
                {
                    var eZona = db.equipos_zona.Where(x => x.id_equipo == eGrla.id_equipo).FirstOrDefault();
                    if (eZona != null)
                    {
                        if (eZona.id_zona == null)
                        {
                            equipos.Add(eGrla);
                        }
                    }
                    else
                    {
                        equipos.Add(eGrla);
                    }
                }

                if (equipos.Count != 0)
                {
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
                }
                else
                {

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


        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/equipo/iJugadores/{id}/{id_torneo:int?}")]
        public IHttpActionResult GetiJugadoresByIdEquipo(int id, int? id_torneo = 0)
        {
            DateTime fecha = DateTime.Now;
            try
            {
                var personas = (from tPersonas in db.personas
                                join tJugador in db.jugadores on tPersonas.id_persona equals tJugador.id_persona
                                join tFiles in db.files on tPersonas.id_foto equals tFiles.Id
                                where tJugador.id_equipo == id
                                select new
                                {
                                    nombre = tPersonas.nombre,
                                    apellido = tPersonas.apellido,
                                    id_persona = tJugador.id_jugador,
                                    id_equipo = tJugador.id_equipo,
                                    imagePath = tFiles.ThumbPath,
                                    rol = tJugador.rol,
                                    nro_doc = tPersonas.nro_documento,
                                    fecha_nacimiento = tPersonas.fecha_nacimiento,
                                    id_jugador = tJugador.id_jugador

                                }).OrderBy(s => s.apellido).ToList();

                List<IJugador> lsJugadores = new List<IJugador>();
                foreach (var p in personas)
                {
                    var gol = db.goleadores.Where(x => x.id_jugador == p.id_jugador && x.id_torneo == id_torneo).FirstOrDefault();
                    var pos = db.posiciones.Where(x => x.id_equipo == p.id_equipo && x.id_torneo == id_torneo).FirstOrDefault();
                    var posZ = db.posiciones_zona.Where(x => x.id_equipo == p.id_equipo && x.id_torneo == id_torneo).FirstOrDefault();
                    var ama = db.sanciones.Where(x => x.id_jugador == p.id_jugador && x.id_torneo == id_torneo && x.id_tipo == 1).Count();
                    var roja = db.sanciones.Where(x => x.id_jugador == p.id_jugador && x.id_torneo == id_torneo && x.id_tipo != 1).Count();

                    IJugador jugador = new IJugador();
                    jugador.nombre = p.nombre;
                    jugador.apellido = p.apellido;
                    jugador.id_equipo = (int)p.id_equipo;
                    jugador.id_persona = p.id_persona;
                    jugador.nro_doc = Convert.ToInt32(p.nro_doc);
                    jugador.imagePath = p.imagePath;
                    jugador.rol = p.rol;
                    jugador.id_jugador = p.id_jugador;
                    if (gol != null)
                    {
                        jugador.goles = (int)gol.cantidad_goles;
                    }
                    else
                    {
                        jugador.goles = 0;
                    }

                    if (pos != null)
                    {
                        jugador.partidos_jugados = (int)pos.partidos_jugados;
                    }
                    else
                    {
                        jugador.partidos_jugados = 0;
                    }

                    if (posZ != null)
                    {
                        jugador.partidos_jugados = jugador.partidos_jugados + (int)posZ.partidos_jugados;
                    }

                    jugador.tarjetas_amarillas = ama;
                    jugador.tarjetas_rojas = roja;

                    jugador.edad = fecha.Year - p.fecha_nacimiento.Year;

                    if (fecha.Month < p.fecha_nacimiento.Month ||
                        (fecha.Month == p.fecha_nacimiento.Month && fecha.Day < p.fecha_nacimiento.Day))
                    {
                        jugador.edad--;
                    }

                    if (jugador.edad > 1800)
                    {
                        jugador.edad = jugador.edad - 1900;
                    }

                    lsJugadores.Add(jugador);
                }
                return Ok(lsJugadores);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(Equipo))]
        [Route("api/torneo/equipos/todos/{id}")]
        public IHttpActionResult getEquiposPorTorneo(int id)
        {
            List<Equipo> lsEquipos = new List<Equipo>();

            try
            {
                var equipos = db.equipos.Where(x => x.id_torneo == id).ToList();

                foreach (var tEquipo in equipos)
                {
                    Equipo equipo = new Equipo();
                    equipo.id_equipo = tEquipo.id_equipo;
                    equipo.nombre = tEquipo.nombre;
                    equipo.logo = (tEquipo.logo != null) ? tEquipo.logo.Value : 0;
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

        [ResponseType(typeof(Equipo))]
        [Route("api/torneo/equipo/{id}")]
        public IHttpActionResult getEquipo(int id)
        {
            try
            {
                var tEquipo = db.equipos.Where(x => x.id_equipo == id).FirstOrDefault();

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
                return Ok(equipo);
            }
            catch (Exception e)
            {
                e.ToString();
                Console.WriteLine(e.ToString());
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/equipo/planilla/{id_torneo}")]
        public IHttpActionResult getIJugadoresPlanilla(int id_torneo)
        {
            DateTime fecha = DateTime.Now;
            var lsIEquipos = new List<IEquipoPlanilla>();
                        
            try
            {
                var iequipos = db.equipos.OrderBy(x => x.nombre).Where(x => x.id_torneo == id_torneo).ToList();
                
                foreach(var eq in iequipos) {
                    IEquipoPlanilla equipo = new IEquipoPlanilla();
                    List<IJugador> lsJugadores = new List<IJugador>();

                    equipo.id_equipo = eq.id_equipo;
                    equipo.nombre = eq.nombre;

                    var personas = (from tPersonas in db.personas
                                    join tJugador in db.jugadores on tPersonas.id_persona equals tJugador.id_persona
                                    join tFiles in db.files on tPersonas.id_foto equals tFiles.Id
                                    where tJugador.id_equipo == eq.id_equipo
                                    select new
                                    {
                                        nombre = tPersonas.nombre,
                                        apellido = tPersonas.apellido,
                                        id_persona = tJugador.id_jugador,
                                        id_equipo = tJugador.id_equipo,
                                        imagePath = tFiles.ThumbPath,
                                        rol = tJugador.rol,
                                        nro_doc = tPersonas.nro_documento,
                                        fecha_nacimiento = tPersonas.fecha_nacimiento,
                                        id_jugador = tJugador.id_jugador

                                    }).OrderBy(s => s.apellido).ToList();

                    foreach (var p in personas)
                    {
                        var gol = db.goleadores.Where(x => x.id_jugador == p.id_jugador && x.id_torneo == id_torneo).FirstOrDefault();
                        var pos = db.posiciones.Where(x => x.id_equipo == p.id_equipo && x.id_torneo == id_torneo).FirstOrDefault();
                        var posZ = db.posiciones_zona.Where(x => x.id_equipo == p.id_equipo && x.id_torneo == id_torneo).FirstOrDefault();
                        var ama = db.sanciones.Where(x => x.id_jugador == p.id_jugador && x.id_torneo == id_torneo && x.id_tipo == 1).Count();
                        var roja = db.sanciones.Where(x => x.id_jugador == p.id_jugador && x.id_torneo == id_torneo && x.id_tipo != 1).Count();

                        IJugador jugador = new IJugador();
                        jugador.nombre = p.nombre;
                        jugador.apellido = p.apellido;
                        jugador.id_equipo = (int)p.id_equipo;
                        jugador.id_persona = p.id_persona;
                        jugador.nro_doc = Convert.ToInt32(p.nro_doc);
                        jugador.imagePath = p.imagePath;
                        jugador.rol = p.rol;
                        jugador.id_jugador = p.id_jugador;
                        if (gol != null)
                        {
                            jugador.goles = (int)gol.cantidad_goles;
                        }
                        else
                        {
                            jugador.goles = 0;
                        }

                        if (pos != null)
                        {
                            jugador.partidos_jugados = (int)pos.partidos_jugados;
                        }
                        else
                        {
                            jugador.partidos_jugados = 0;
                        }

                        if (posZ != null)
                        {
                            jugador.partidos_jugados = jugador.partidos_jugados + (int)posZ.partidos_jugados;
                        }

                        jugador.tarjetas_amarillas = ama;
                        jugador.tarjetas_rojas = roja;

                        jugador.edad = fecha.Year - p.fecha_nacimiento.Year;

                        if (fecha.Month < p.fecha_nacimiento.Month ||
                            (fecha.Month == p.fecha_nacimiento.Month && fecha.Day < p.fecha_nacimiento.Day))
                        {
                            jugador.edad--;
                        }

                        if (jugador.edad > 1800)
                        {
                            jugador.edad = jugador.edad - 1900;
                        }

                        lsJugadores.Add(jugador);
                    }
                    equipo.lsJugadores = lsJugadores;
                    lsIEquipos.Add(equipo);
                }
                return Ok(lsIEquipos);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}