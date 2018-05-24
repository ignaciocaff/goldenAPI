using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using RestServiceGolden.Models;
using RestServiceGolden.Utilidades;
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
    public class FixtureController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/registrar/{id_zona}/{id_torneo}")]
        public IHttpActionResult registrar([FromBody]List<Partido> partidos, int id_zona, int id_torneo)
        {
            try
            {
                int id_fixture_zona = db.fixture_zona.SingleOrDefault(x => x.id_zona == id_zona && x.id_torneo == id_torneo).id_fixture;
                var fechaDto = partidos.FirstOrDefault().fecha.fecha;
                var fechaCheck = db.fechas.Where(x => x.fecha == fechaDto && x.id_fixture_zona == id_fixture_zona).SingleOrDefault();
                var id_fase = db.torneos.Where(x => x.id_torneo == id_torneo).FirstOrDefault().id_fase;
                if (id_fixture_zona != 0 && fechaCheck == null)
                {
                    fechas fecha = new fechas();
                    fecha.fecha = partidos.FirstOrDefault().fecha.fecha;
                    fecha.id_estado = 1;
                    fecha.id_fixture_zona = id_fixture_zona;
                    fecha.id_fase = id_fase;
                    db.fechas.Add(fecha);
                    db.SaveChanges();

                    int id_fecha = fecha.id_fecha;

                    foreach (Partido p in partidos)
                    {
                        partidos partido = new partidos();
                        partido.local = p.local.id_equipo;
                        partido.visitante = p.visitante.id_equipo;
                        partido.id_estado_partido = p.estado.id_estado;
                        partido.id_cancha = p.cancha.id_cancha;
                        partido.id_horario_fijo = p.horario_fijo.id_horario;
                        partido.hora_inicio = p.horario_fijo.inicio;
                        partido.hora_fin = p.horario_fijo.fin;
                        partido.id_fecha = id_fecha;
                        db.partidos.Add(partido);
                        db.SaveChanges();
                    }
                    return Ok();
                }
                return BadRequest("Esa fecha ya fue creada, debe modificarla.");
            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/registrar/" + " Parametros de entrada: " +
                  JsonConvert.SerializeObject(partidos, Formatting.None) + id_zona + id_torneo, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/registrarInterzonal/{id_torneo}/{id_fase}")]
        public IHttpActionResult registrarInterzonal([FromBody]List<Partido> partidos, int id_torneo, int id_fase)
        {
            try
            {
                int id_fixture_zona = db.fixture_zona.SingleOrDefault(x => x.id_zona == null && x.id_torneo == id_torneo).id_fixture;
                var fechaDto = partidos.FirstOrDefault().fecha.fecha;
                fechas fecha = new fechas();
                fecha.fecha = partidos.FirstOrDefault().fecha.fecha;
                fecha.id_estado = 1;
                fecha.id_fixture_zona = id_fixture_zona;
                fecha.id_fase = id_fase;
                db.fechas.Add(fecha);
                db.SaveChanges();

                int id_fecha = fecha.id_fecha;

                foreach (Partido p in partidos)
                {
                    partidos partido = new partidos();
                    partido.local = p.local.id_equipo;
                    partido.visitante = p.visitante.id_equipo;
                    partido.id_estado_partido = p.estado.id_estado;
                    partido.id_cancha = p.cancha.id_cancha;
                    partido.id_horario_fijo = p.horario_fijo.id_horario;
                    partido.hora_inicio = p.horario_fijo.inicio;
                    partido.hora_fin = p.horario_fijo.fin;
                    partido.id_fecha = id_fecha;
                    partido.esInterzonal = 1;
                    db.partidos.Add(partido);
                    db.SaveChanges();
                }
                return Ok();
            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/registrarInterzonal/" + " Parametros de entrada: " +
                  JsonConvert.SerializeObject(partidos, Formatting.None) + id_torneo + id_fase, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/modificar/{id_zona}/{id_torneo}")]
        public IHttpActionResult modificar([FromBody]List<Partido> partidos, int id_zona, int id_torneo)
        {
            try
            {
                int id_fixture_zona = db.fixture_zona.SingleOrDefault(x => x.id_zona == id_zona && x.id_torneo == id_torneo).id_fixture;
                var fechaDto = partidos.FirstOrDefault().fecha.fecha;
                var fechaCheck = db.fechas.Where(x => x.fecha == fechaDto && x.id_fixture_zona == id_fixture_zona).SingleOrDefault();

                if (fechaCheck != null)
                {
                    foreach (Partido p in partidos)
                    {
                        if (p.id_partido == null)
                        {
                            //Damos de alta el partido entero.
                            partidos partido = new partidos();
                            partido.local = p.local.id_equipo;
                            partido.visitante = p.visitante.id_equipo;
                            partido.id_estado_partido = p.estado.id_estado;
                            partido.id_cancha = p.cancha.id_cancha;
                            partido.id_horario_fijo = p.horario_fijo.id_horario;
                            partido.hora_inicio = p.horario_fijo.inicio;
                            partido.hora_fin = p.horario_fijo.fin;
                            partido.id_fecha = fechaCheck.id_fecha;
                            db.partidos.Add(partido);
                            db.SaveChanges();
                        }
                        else
                        {
                            var partidoUpdate = db.partidos.SingleOrDefault(b => b.id_partido == p.id_partido);
                            if (partidoUpdate != null)
                            {
                                partidoUpdate.local = p.local.id_equipo;
                                partidoUpdate.visitante = p.visitante.id_equipo;
                                partidoUpdate.id_estado_partido = p.estado.id_estado;
                                partidoUpdate.id_cancha = p.cancha.id_cancha;
                                partidoUpdate.id_horario_fijo = p.horario_fijo.id_horario;
                                partidoUpdate.hora_inicio = p.horario_fijo.inicio;
                                partidoUpdate.hora_fin = p.horario_fijo.fin;
                                partidoUpdate.id_fecha = fechaCheck.id_fecha;
                                db.SaveChanges();
                            }
                        }
                    }
                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/modificar/" + " Parametros de entrada: " +
                  JsonConvert.SerializeObject(partidos, Formatting.None) + id_zona + id_torneo, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/verificar/{id_zona}/{id_torneo}")]
        public IHttpActionResult verificar([FromBody]Fecha fecha, int id_zona, int id_torneo)
        {
            List<fechas> lsFechas = db.fechas.Where(x => x.fecha == fecha.fecha).ToList();

            foreach (var fec in lsFechas)
            {
                if (fec != null)
                {
                    var fixtureDto = db.fixture_zona.SingleOrDefault(x => x.id_fixture == fec.id_fixture_zona);

                    if (fixtureDto != null && fixtureDto.id_zona == id_zona && fixtureDto.id_torneo == id_torneo)
                    {
                        return BadRequest();
                    }
                }
            }


            return Ok();
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/obtener/{id_torneo}/{id_zona:int?}/{esInterzonal:int?}")]
        public IHttpActionResult obtener([FromBody]Fecha fecha, int id_torneo, int? id_zona = 0, int? esInterzonal = 0)
        {
            List<IPartido> lsPartidos = new List<IPartido>();
            try
            {
                if (esInterzonal == 0)
                {
                    var fixture_zona = db.fixture_zona.SingleOrDefault(x => x.id_zona == id_zona && x.id_torneo == id_torneo);
                    if (fixture_zona != null)
                    {
                        var fechas = db.fechas.Where(x => x.id_fixture_zona == fixture_zona.id_fixture && x.fecha == fecha.fecha).ToList();


                        foreach (var f in fechas)
                        {
                            foreach (var partido in f.partidos)
                            {
                                IPartido iPartido = new IPartido();
                                Cancha cancha = new Cancha();
                                HorarioFijo horarioFijo = new HorarioFijo();
                                IEquipo iLocal = new IEquipo();
                                IEquipo iVisitante = new IEquipo();
                                Turno turno = new Turno();
                                Fecha fechaPartido = new Fecha();

                                var objLocal = (from tEquipos in db.equipos
                                                join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                where tEquipos.id_equipo == partido.local
                                                select new
                                                {
                                                    id_equipo = tEquipos.id_equipo,
                                                    nombre = tEquipos.nombre,
                                                    imagePath = tArchivos.ThumbPath,
                                                    logo = tEquipos.logo
                                                }).SingleOrDefault();

                                var objVisitante = (from tEquipos in db.equipos
                                                    join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                    where tEquipos.id_equipo == partido.visitante
                                                    select new
                                                    {
                                                        id_equipo = tEquipos.id_equipo,
                                                        nombre = tEquipos.nombre,
                                                        imagePath = tArchivos.ThumbPath,
                                                        logo = tEquipos.logo
                                                    }).SingleOrDefault();

                                iLocal.id_equipo = objLocal.id_equipo;
                                iLocal.nombre = objLocal.nombre;
                                iLocal.logo = objLocal.logo;
                                iLocal.imagePath = objLocal.imagePath;

                                iVisitante.id_equipo = objVisitante.id_equipo;
                                iVisitante.nombre = objVisitante.nombre;
                                iVisitante.logo = objVisitante.logo;
                                iVisitante.imagePath = objVisitante.imagePath;

                                iPartido.local = new List<IEquipo>();
                                iPartido.visitante = new List<IEquipo>();


                                iPartido.local.Add(iLocal);
                                iPartido.visitante.Add(iVisitante);

                                var canchaDto = db.canchas.SingleOrDefault(x => x.id_cancha == partido.id_cancha);

                                iPartido.cancha = cancha;
                                iPartido.cancha.id_cancha = (int)partido.id_cancha;
                                iPartido.cancha.nombre = canchaDto.nombre;

                                var horarioDto = db.horarios_fijos.SingleOrDefault(x => x.id_horario == partido.id_horario_fijo);
                                iPartido.horario = horarioFijo;
                                iPartido.horario.id_horario = partido.id_horario_fijo;
                                iPartido.horario.inicio = horarioDto.inicio;
                                iPartido.horario.fin = horarioDto.fin;
                                iPartido.horario.turno = turno;
                                iPartido.horario.turno.id = horarioDto.id_turno;
                                iPartido.id_partido = partido.id_partido;
                                iPartido.id_fixture = f.id_fixture_zona;
                                iPartido.fecha = fechaPartido;
                                iPartido.fecha.id_fecha = f.id_fecha;
                                iPartido.fecha.fecha = (DateTime)f.fecha;

                                if (partido.esInterzonal == null && partido.id_resultado == null && partido.id_resultados_zona == null)
                                {
                                    lsPartidos.Add(iPartido);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //Obtengo todos los partidos interzonales sin resultado
                    var fechas = db.fechas.Where(x => x.fecha == fecha.fecha).ToList();

                    foreach (var f in fechas)
                    {
                        foreach (var partido in f.partidos)
                        {
                            IPartido iPartido = new IPartido();
                            Cancha cancha = new Cancha();
                            HorarioFijo horarioFijo = new HorarioFijo();
                            IEquipo iLocal = new IEquipo();
                            IEquipo iVisitante = new IEquipo();
                            Turno turno = new Turno();
                            Fecha fechaPartido = new Fecha();
                            var objLocal = (from tEquipos in db.equipos
                                            join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                            where tEquipos.id_equipo == partido.local
                                            select new
                                            {
                                                id_equipo = tEquipos.id_equipo,
                                                nombre = tEquipos.nombre,
                                                imagePath = tArchivos.ThumbPath,
                                                logo = tEquipos.logo
                                            }).SingleOrDefault();

                            var objVisitante = (from tEquipos in db.equipos
                                                join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                where tEquipos.id_equipo == partido.visitante
                                                select new
                                                {
                                                    id_equipo = tEquipos.id_equipo,
                                                    nombre = tEquipos.nombre,
                                                    imagePath = tArchivos.ThumbPath,
                                                    logo = tEquipos.logo
                                                }).SingleOrDefault();

                            iLocal.id_equipo = objLocal.id_equipo;
                            iLocal.nombre = objLocal.nombre;
                            iLocal.logo = objLocal.logo;
                            iLocal.imagePath = objLocal.imagePath;

                            iVisitante.id_equipo = objVisitante.id_equipo;
                            iVisitante.nombre = objVisitante.nombre;
                            iVisitante.logo = objVisitante.logo;
                            iVisitante.imagePath = objVisitante.imagePath;

                            iPartido.local = new List<IEquipo>();
                            iPartido.visitante = new List<IEquipo>();


                            iPartido.local.Add(iLocal);
                            iPartido.visitante.Add(iVisitante);

                            var canchaDto = db.canchas.SingleOrDefault(x => x.id_cancha == partido.id_cancha);

                            iPartido.cancha = cancha;
                            iPartido.cancha.id_cancha = (int)partido.id_cancha;
                            iPartido.cancha.nombre = canchaDto.nombre;

                            var horarioDto = db.horarios_fijos.SingleOrDefault(x => x.id_horario == partido.id_horario_fijo);
                            iPartido.horario = horarioFijo;
                            iPartido.horario.id_horario = partido.id_horario_fijo;
                            iPartido.horario.inicio = horarioDto.inicio;
                            iPartido.horario.fin = horarioDto.fin;
                            iPartido.horario.turno = turno;
                            iPartido.horario.turno.id = horarioDto.id_turno;
                            iPartido.id_partido = partido.id_partido;
                            iPartido.fecha = fechaPartido;
                            iPartido.fecha.id_fecha = f.id_fecha;
                            iPartido.fecha.fecha = (DateTime)f.fecha;

                            if (partido.esInterzonal == 1 && partido.id_resultado == null && partido.id_resultados_zona == null)
                            {
                                lsPartidos.Add(iPartido);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/obtener/" + " Parametros de entrada: " +
                JsonConvert.SerializeObject(fecha, Formatting.None) + id_torneo + id_zona + esInterzonal, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
            return Ok(lsPartidos);
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/obtenerPartido/{id_equipo}/{id_torneo}/{id_zona:int?}/{esInterzonal:int?}")]
        public IHttpActionResult obtenerPartido([FromBody]Fecha fecha, int id_equipo, int id_torneo, int? id_zona = 0, int? esInterzonal = 0)
        {
            try
            {
                IPartido iPartido = new IPartido();

                if (esInterzonal == 0)
                {
                    var fixture_zona = db.fixture_zona.SingleOrDefault(x => x.id_zona == id_zona && x.id_torneo == id_torneo);
                    if (fixture_zona != null)
                    {
                        var partidos = db.fechas.Where(x => x.id_fixture_zona == fixture_zona.id_fixture && x.fecha == fecha.fecha).SingleOrDefault().partidos;
                        if (partidos != null)
                        {
                            var partido = partidos.Where(x => (x.local == id_equipo || x.visitante == id_equipo) && x.esInterzonal == null && x.id_resultados_zona != null).SingleOrDefault();
                            if (partido != null)
                            {

                                Cancha cancha = new Cancha();
                                HorarioFijo horarioFijo = new HorarioFijo();
                                IEquipo iLocal = new IEquipo();
                                IEquipo iVisitante = new IEquipo();
                                Turno turno = new Turno();
                                Fecha fechaPartido = new Fecha();
                                ResultadoZona resultado_zona = new ResultadoZona();
                                List<Sancion> lsSancionesLocal = new List<Sancion>();
                                List<Sancion> lsSancionesVisitante = new List<Sancion>();
                                List<Gol> lsGolesLocal = new List<Gol>();
                                List<Gol> lsGolesVisitante = new List<Gol>();
                                List<Jugador> lsJugadoresLocal = new List<Jugador>();
                                List<Jugador> lsJugadoresVisitante = new List<Jugador>();

                                var objLocal = (from tEquipos in db.equipos
                                                join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                where tEquipos.id_equipo == partido.local
                                                select new
                                                {
                                                    id_equipo = tEquipos.id_equipo,
                                                    nombre = tEquipos.nombre,
                                                    imagePath = tArchivos.ThumbPath,
                                                    logo = tEquipos.logo
                                                }).SingleOrDefault();


                                var jugLocales = (from tJugador in db.jugadores
                                                  join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                                  where tJugador.id_equipo == partido.local
                                                  select new
                                                  {
                                                      id_jugador = tJugador.id_jugador,
                                                      nombre = tPersona.nombre,
                                                      apellido = tPersona.apellido,
                                                      id_equipo = tJugador.id_jugador
                                                  }).ToList();

                                var objVisitante = (from tEquipos in db.equipos
                                                    join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                    where tEquipos.id_equipo == partido.visitante
                                                    select new
                                                    {
                                                        id_equipo = tEquipos.id_equipo,
                                                        nombre = tEquipos.nombre,
                                                        imagePath = tArchivos.ThumbPath,
                                                        logo = tEquipos.logo
                                                    }).SingleOrDefault();

                                var jugVisitantes = (from tJugador in db.jugadores
                                                     join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                                     where tJugador.id_equipo == partido.visitante
                                                     select new
                                                     {
                                                         id_jugador = tJugador.id_jugador,
                                                         nombre = tPersona.nombre,
                                                         apellido = tPersona.apellido,
                                                         id_equipo = tJugador.id_jugador
                                                     }).ToList();

                                iLocal.id_equipo = objLocal.id_equipo;
                                iLocal.nombre = objLocal.nombre;
                                iLocal.logo = objLocal.logo;
                                iLocal.imagePath = objLocal.imagePath;

                                iVisitante.id_equipo = objVisitante.id_equipo;
                                iVisitante.nombre = objVisitante.nombre;
                                iVisitante.logo = objVisitante.logo;
                                iVisitante.imagePath = objVisitante.imagePath;

                                foreach (var jL in jugLocales)
                                {
                                    Jugador jugador = new Jugador();
                                    Equipo equipo = new Equipo();
                                    jugador.id_jugador = jL.id_jugador;
                                    jugador.nombre = jL.nombre;
                                    jugador.apellido = jL.apellido;
                                    jugador.equipo = equipo;
                                    jugador.equipo.id_equipo = jL.id_equipo;
                                    lsJugadoresLocal.Add(jugador);
                                }

                                foreach (var jV in jugVisitantes)
                                {
                                    Jugador jugador = new Jugador();
                                    Equipo equipo = new Equipo();
                                    jugador.id_jugador = jV.id_jugador;
                                    jugador.nombre = jV.nombre;
                                    jugador.apellido = jV.apellido;
                                    jugador.equipo = equipo;
                                    jugador.equipo.id_equipo = jV.id_equipo;
                                    lsJugadoresVisitante.Add(jugador);
                                }

                                iPartido.local = new List<IEquipo>();
                                iPartido.visitante = new List<IEquipo>();

                                iLocal.lsJugadores = lsJugadoresLocal;
                                iVisitante.lsJugadores = lsJugadoresVisitante;
                                iPartido.local.Add(iLocal);
                                iPartido.visitante.Add(iVisitante);

                                var canchaDto = db.canchas.SingleOrDefault(x => x.id_cancha == partido.id_cancha);

                                iPartido.cancha = cancha;
                                iPartido.cancha.id_cancha = (int)partido.id_cancha;
                                iPartido.cancha.nombre = canchaDto.nombre;

                                var horarioDto = db.horarios_fijos.SingleOrDefault(x => x.id_horario == partido.id_horario_fijo);
                                iPartido.horario = horarioFijo;
                                iPartido.horario.id_horario = partido.id_horario_fijo;
                                iPartido.horario.inicio = horarioDto.inicio;
                                iPartido.horario.fin = horarioDto.fin;
                                iPartido.horario.turno = turno;
                                iPartido.horario.turno.id = horarioDto.id_turno;
                                iPartido.id_partido = partido.id_partido;
                                iPartido.fecha = fechaPartido;
                                iPartido.fecha.id_fecha = (int)partido.id_fecha;

                                var sLocales = (from tSancion in db.sanciones
                                                join tJugador in db.jugadores on tSancion.id_jugador equals tJugador.id_jugador
                                                join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                                where tSancion.id_partido == partido.id_partido && tSancion.id_equipo == partido.local
                                                select new
                                                {
                                                    id_partido = tSancion.id_partido,
                                                    id_jugador = tSancion.id_jugador,
                                                    id_sancion = tSancion.id_sancion,
                                                    nombre = tPersona.nombre,
                                                    apellido = tPersona.apellido,
                                                    id_tipo = tSancion.id_tipo
                                                }).ToList();

                                var gLocales = (from tGoles in db.goles
                                                join tJugador in db.jugadores on tGoles.id_jugador equals tJugador.id_jugador
                                                join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                                where tGoles.id_partido == partido.id_partido && tGoles.id_equipo == partido.local
                                                select new
                                                {
                                                    id_partido = tGoles.id_partido,
                                                    id_jugador = tGoles.id_jugador,
                                                    id_gol = tGoles.id_gol,
                                                    nombre = tPersona.nombre,
                                                    apellido = tPersona.apellido,
                                                }).ToList();


                                var sVisitantes = (from tSancion in db.sanciones
                                                   join tJugador in db.jugadores on tSancion.id_jugador equals tJugador.id_jugador
                                                   join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                                   where tSancion.id_partido == partido.id_partido && tSancion.id_equipo == partido.visitante
                                                   select new
                                                   {
                                                       id_partido = tSancion.id_partido,
                                                       id_jugador = tSancion.id_jugador,
                                                       id_sancion = tSancion.id_sancion,
                                                       nombre = tPersona.nombre,
                                                       apellido = tPersona.apellido,
                                                       id_tipo = tSancion.id_tipo
                                                   }).ToList();

                                var gVisitante = (from tGoles in db.goles
                                                  join tJugador in db.jugadores on tGoles.id_jugador equals tJugador.id_jugador
                                                  join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                                  where tGoles.id_partido == partido.id_partido && tGoles.id_equipo == partido.visitante
                                                  select new
                                                  {
                                                      id_partido = tGoles.id_partido,
                                                      id_jugador = tGoles.id_jugador,
                                                      id_gol = tGoles.id_gol,
                                                      nombre = tPersona.nombre,
                                                      apellido = tPersona.apellido,
                                                  }).ToList();

                                foreach (var sL in sLocales)
                                {
                                    Sancion sancion = new Sancion();
                                    TipoSancion tipoSancion = new TipoSancion();
                                    Partido partidoSancion = new Partido();
                                    Jugador jugador = new Jugador();

                                    sancion.partido = partidoSancion;
                                    sancion.partido.id_partido = sL.id_partido;
                                    sancion.jugador = jugador;
                                    sancion.jugador.id_jugador = sL.id_jugador;
                                    sancion.jugador.nombre = sL.nombre;
                                    sancion.jugador.apellido = sL.apellido;
                                    sancion.id_sancion = sL.id_sancion;
                                    sancion.tipo_sancion = tipoSancion;
                                    sancion.tipo_sancion.id_tipo = sL.id_tipo;
                                    sancion.tipo_sancion.descripcion = db.tipos_sanciones.Where(x => x.id_tipo == sL.id_tipo).FirstOrDefault().descripcion;
                                    lsSancionesLocal.Add(sancion);
                                }

                                foreach (var sV in sVisitantes)
                                {
                                    Sancion sancion = new Sancion();
                                    TipoSancion tipoSancion = new TipoSancion();
                                    Partido partidoSancion = new Partido();
                                    Jugador jugador = new Jugador();

                                    sancion.partido = partidoSancion;
                                    sancion.partido.id_partido = sV.id_partido;
                                    sancion.jugador = jugador;
                                    sancion.jugador.id_jugador = sV.id_jugador;
                                    sancion.jugador.nombre = sV.nombre;
                                    sancion.jugador.apellido = sV.apellido;
                                    sancion.id_sancion = sV.id_sancion;
                                    sancion.tipo_sancion = tipoSancion;
                                    sancion.tipo_sancion.id_tipo = sV.id_tipo;
                                    sancion.tipo_sancion.descripcion = db.tipos_sanciones.Where(x => x.id_tipo == sV.id_tipo).FirstOrDefault().descripcion;
                                    lsSancionesVisitante.Add(sancion);
                                }

                                foreach (var gL in gLocales)
                                {
                                    Gol gol = new Gol();
                                    Partido partidoGol = new Partido();
                                    Jugador jugador = new Jugador();

                                    gol.partido = partidoGol;
                                    gol.partido.id_partido = gL.id_partido;
                                    gol.jugador = jugador;
                                    gol.jugador.id_jugador = gL.id_jugador;
                                    gol.jugador.nombre = gL.nombre;
                                    gol.jugador.apellido = gL.apellido;
                                    gol.id_gol = gL.id_gol;
                                    lsGolesLocal.Add(gol);
                                }

                                foreach (var gV in gVisitante)
                                {
                                    Gol gol = new Gol();
                                    Partido partidoGol = new Partido();
                                    Jugador jugador = new Jugador();

                                    gol.partido = partidoGol;
                                    gol.partido.id_partido = gV.id_partido;
                                    gol.jugador = jugador;
                                    gol.jugador.id_jugador = gV.id_jugador;
                                    gol.jugador.nombre = gV.nombre;
                                    gol.jugador.apellido = gV.apellido;
                                    gol.id_gol = gV.id_gol;
                                    lsGolesVisitante.Add(gol);
                                }

                                iPartido.lsSancionesLocal = lsSancionesLocal;
                                iPartido.lsSancionesVisitante = lsSancionesVisitante;
                                iPartido.lsGolesLocal = lsGolesLocal;
                                iPartido.lsGolesVisitante = lsGolesVisitante;
                                iPartido.resultado_zona = resultado_zona;
                                iPartido.resultado_zona.id_resultado = (int)partido.id_resultados_zona;
                            }
                        }
                    }
                }
                else
                {

                    var fixture_zona = db.fixture_zona.SingleOrDefault(x => x.id_zona == null && x.id_torneo == id_torneo);
                    if (fixture_zona != null)
                    {
                        var partidos = db.fechas.Where(x => x.id_fixture_zona == fixture_zona.id_fixture && x.fecha == fecha.fecha).SingleOrDefault().partidos;
                        if (partidos != null)
                        {
                            var partido = partidos.Where(x => (x.local == id_equipo || x.visitante == id_equipo) && x.esInterzonal != null && x.id_resultado != null).SingleOrDefault();
                            if (partido != null)
                            {


                                Cancha cancha = new Cancha();
                                HorarioFijo horarioFijo = new HorarioFijo();
                                IEquipo iLocal = new IEquipo();
                                IEquipo iVisitante = new IEquipo();
                                Turno turno = new Turno();
                                Fecha fechaPartido = new Fecha();
                                Resultado resultado = new Resultado();
                                List<Sancion> lsSancionesLocal = new List<Sancion>();
                                List<Sancion> lsSancionesVisitante = new List<Sancion>();
                                List<Gol> lsGolesLocal = new List<Gol>();
                                List<Gol> lsGolesVisitante = new List<Gol>();
                                List<Jugador> lsJugadoresLocal = new List<Jugador>();
                                List<Jugador> lsJugadoresVisitante = new List<Jugador>();

                                var objLocal = (from tEquipos in db.equipos
                                                join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                where tEquipos.id_equipo == partido.local
                                                select new
                                                {
                                                    id_equipo = tEquipos.id_equipo,
                                                    nombre = tEquipos.nombre,
                                                    imagePath = tArchivos.ThumbPath,
                                                    logo = tEquipos.logo
                                                }).SingleOrDefault();


                                var jugLocales = (from tJugador in db.jugadores
                                                  join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                                  where tJugador.id_equipo == partido.local
                                                  select new
                                                  {
                                                      id_jugador = tJugador.id_jugador,
                                                      nombre = tPersona.nombre,
                                                      apellido = tPersona.apellido,
                                                      id_equipo = tJugador.id_jugador
                                                  }).ToList();

                                var objVisitante = (from tEquipos in db.equipos
                                                    join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                    where tEquipos.id_equipo == partido.visitante
                                                    select new
                                                    {
                                                        id_equipo = tEquipos.id_equipo,
                                                        nombre = tEquipos.nombre,
                                                        imagePath = tArchivos.ThumbPath,
                                                        logo = tEquipos.logo
                                                    }).SingleOrDefault();

                                var jugVisitantes = (from tJugador in db.jugadores
                                                     join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                                     where tJugador.id_equipo == partido.visitante
                                                     select new
                                                     {
                                                         id_jugador = tJugador.id_jugador,
                                                         nombre = tPersona.nombre,
                                                         apellido = tPersona.apellido,
                                                         id_equipo = tJugador.id_jugador
                                                     }).ToList();

                                iLocal.id_equipo = objLocal.id_equipo;
                                iLocal.nombre = objLocal.nombre;
                                iLocal.logo = objLocal.logo;
                                iLocal.imagePath = objLocal.imagePath;

                                iVisitante.id_equipo = objVisitante.id_equipo;
                                iVisitante.nombre = objVisitante.nombre;
                                iVisitante.logo = objVisitante.logo;
                                iVisitante.imagePath = objVisitante.imagePath;

                                foreach (var jL in jugLocales)
                                {
                                    Jugador jugador = new Jugador();
                                    Equipo equipo = new Equipo();
                                    jugador.id_jugador = jL.id_jugador;
                                    jugador.nombre = jL.nombre;
                                    jugador.apellido = jL.apellido;
                                    jugador.equipo = equipo;
                                    jugador.equipo.id_equipo = jL.id_equipo;
                                    lsJugadoresLocal.Add(jugador);
                                }

                                foreach (var jV in jugVisitantes)
                                {
                                    Jugador jugador = new Jugador();
                                    Equipo equipo = new Equipo();
                                    jugador.id_jugador = jV.id_jugador;
                                    jugador.nombre = jV.nombre;
                                    jugador.apellido = jV.apellido;
                                    jugador.equipo = equipo;
                                    jugador.equipo.id_equipo = jV.id_equipo;
                                    lsJugadoresVisitante.Add(jugador);
                                }

                                iPartido.local = new List<IEquipo>();
                                iPartido.visitante = new List<IEquipo>();

                                iLocal.lsJugadores = lsJugadoresLocal;
                                iVisitante.lsJugadores = lsJugadoresVisitante;
                                iPartido.local.Add(iLocal);
                                iPartido.visitante.Add(iVisitante);

                                var canchaDto = db.canchas.SingleOrDefault(x => x.id_cancha == partido.id_cancha);

                                iPartido.cancha = cancha;
                                iPartido.cancha.id_cancha = (int)partido.id_cancha;
                                iPartido.cancha.nombre = canchaDto.nombre;

                                var horarioDto = db.horarios_fijos.SingleOrDefault(x => x.id_horario == partido.id_horario_fijo);
                                iPartido.horario = horarioFijo;
                                iPartido.horario.id_horario = partido.id_horario_fijo;
                                iPartido.horario.inicio = horarioDto.inicio;
                                iPartido.horario.fin = horarioDto.fin;
                                iPartido.horario.turno = turno;
                                iPartido.horario.turno.id = horarioDto.id_turno;
                                iPartido.id_partido = partido.id_partido;

                                iPartido.fecha = fechaPartido;
                                iPartido.fecha.id_fecha = (int)partido.id_fecha;

                                var sLocales = (from tSancion in db.sanciones
                                                join tJugador in db.jugadores on tSancion.id_jugador equals tJugador.id_jugador
                                                join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                                where tSancion.id_partido == partido.id_partido && tSancion.id_equipo == partido.local
                                                select new
                                                {
                                                    id_partido = tSancion.id_partido,
                                                    id_jugador = tSancion.id_jugador,
                                                    id_sancion = tSancion.id_sancion,
                                                    nombre = tPersona.nombre,
                                                    apellido = tPersona.apellido,
                                                    id_tipo = tSancion.id_tipo
                                                }).ToList();

                                var gLocales = (from tGoles in db.goles
                                                join tJugador in db.jugadores on tGoles.id_jugador equals tJugador.id_jugador
                                                join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                                where tGoles.id_partido == partido.id_partido && tGoles.id_equipo == partido.local
                                                select new
                                                {
                                                    id_partido = tGoles.id_partido,
                                                    id_jugador = tGoles.id_jugador,
                                                    id_gol = tGoles.id_gol,
                                                    nombre = tPersona.nombre,
                                                    apellido = tPersona.apellido,
                                                }).ToList();


                                var sVisitantes = (from tSancion in db.sanciones
                                                   join tJugador in db.jugadores on tSancion.id_jugador equals tJugador.id_jugador
                                                   join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                                   where tSancion.id_partido == partido.id_partido && tSancion.id_equipo == partido.visitante
                                                   select new
                                                   {
                                                       id_partido = tSancion.id_partido,
                                                       id_jugador = tSancion.id_jugador,
                                                       id_sancion = tSancion.id_sancion,
                                                       nombre = tPersona.nombre,
                                                       apellido = tPersona.apellido,
                                                       id_tipo = tSancion.id_tipo
                                                   }).ToList();

                                var gVisitante = (from tGoles in db.goles
                                                  join tJugador in db.jugadores on tGoles.id_jugador equals tJugador.id_jugador
                                                  join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                                  where tGoles.id_partido == partido.id_partido && tGoles.id_equipo == partido.visitante
                                                  select new
                                                  {
                                                      id_partido = tGoles.id_partido,
                                                      id_jugador = tGoles.id_jugador,
                                                      id_gol = tGoles.id_gol,
                                                      nombre = tPersona.nombre,
                                                      apellido = tPersona.apellido,
                                                  }).ToList();

                                foreach (var sL in sLocales)
                                {
                                    Sancion sancion = new Sancion();
                                    TipoSancion tipoSancion = new TipoSancion();
                                    Partido partidoSancion = new Partido();
                                    Jugador jugador = new Jugador();

                                    sancion.partido = partidoSancion;
                                    sancion.partido.id_partido = sL.id_partido;
                                    sancion.jugador = jugador;
                                    sancion.jugador.id_jugador = sL.id_jugador;
                                    sancion.jugador.nombre = sL.nombre;
                                    sancion.jugador.apellido = sL.apellido;
                                    sancion.id_sancion = sL.id_sancion;
                                    sancion.tipo_sancion = tipoSancion;
                                    sancion.tipo_sancion.id_tipo = sL.id_tipo;
                                    sancion.tipo_sancion.descripcion = db.tipos_sanciones.Where(x => x.id_tipo == sL.id_tipo).FirstOrDefault().descripcion;
                                    lsSancionesLocal.Add(sancion);
                                }

                                foreach (var sV in sVisitantes)
                                {
                                    Sancion sancion = new Sancion();
                                    TipoSancion tipoSancion = new TipoSancion();
                                    Partido partidoSancion = new Partido();
                                    Jugador jugador = new Jugador();

                                    sancion.partido = partidoSancion;
                                    sancion.partido.id_partido = sV.id_partido;
                                    sancion.jugador = jugador;
                                    sancion.jugador.id_jugador = sV.id_jugador;
                                    sancion.jugador.nombre = sV.nombre;
                                    sancion.jugador.apellido = sV.apellido;
                                    sancion.id_sancion = sV.id_sancion;
                                    sancion.tipo_sancion = tipoSancion;
                                    sancion.tipo_sancion.id_tipo = sV.id_tipo;
                                    sancion.tipo_sancion.descripcion = db.tipos_sanciones.Where(x => x.id_tipo == sV.id_tipo).FirstOrDefault().descripcion;
                                    lsSancionesVisitante.Add(sancion);
                                }

                                foreach (var gL in gLocales)
                                {
                                    Gol gol = new Gol();
                                    Partido partidoGol = new Partido();
                                    Jugador jugador = new Jugador();

                                    gol.partido = partidoGol;
                                    gol.partido.id_partido = gL.id_partido;
                                    gol.jugador = jugador;
                                    gol.jugador.id_jugador = gL.id_jugador;
                                    gol.jugador.nombre = gL.nombre;
                                    gol.jugador.apellido = gL.apellido;
                                    gol.id_gol = gL.id_gol;
                                    lsGolesLocal.Add(gol);
                                }

                                foreach (var gV in gVisitante)
                                {
                                    Gol gol = new Gol();
                                    Partido partidoGol = new Partido();
                                    Jugador jugador = new Jugador();

                                    gol.partido = partidoGol;
                                    gol.partido.id_partido = gV.id_partido;
                                    gol.jugador = jugador;
                                    gol.jugador.id_jugador = gV.id_jugador;
                                    gol.jugador.nombre = gV.nombre;
                                    gol.jugador.apellido = gV.apellido;
                                    gol.id_gol = gV.id_gol;
                                    lsGolesVisitante.Add(gol);
                                }

                                iPartido.lsSancionesLocal = lsSancionesLocal;
                                iPartido.lsSancionesVisitante = lsSancionesVisitante;
                                iPartido.lsGolesLocal = lsGolesLocal;
                                iPartido.lsGolesVisitante = lsGolesVisitante;
                                iPartido.resultado = resultado;
                                iPartido.resultado.id_resultado = (int)partido.id_resultado;
                            }
                        }
                    }
                }
                return Ok(iPartido);
            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/obtenerPartido/" + " Parametros de entrada: " +
                JsonConvert.SerializeObject(fecha, Formatting.None) + id_equipo + id_torneo + id_zona + esInterzonal, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/obtenerPartidos")]
        public IHttpActionResult obtenerPartidos([FromBody]IPartido iPartido)
        {
            try
            {
                var fechas = db.fechas.Where(x => x.fecha == iPartido.fecha.fecha).ToList();
                if (fechas.Count > 0)
                {
                    foreach (var fecha in fechas)
                    {
                        var partidos = db.partidos.Where(x => x.id_fecha == fecha.id_fecha).ToList();
                        if (partidos.Count > 0)
                        {
                            foreach (var partido in partidos)
                            {
                                var horarioDto = db.horarios_fijos.Where(x => x.inicio == iPartido.horario.inicio && x.fin == iPartido.horario.fin).SingleOrDefault();
                                if (horarioDto != null && partido.id_horario_fijo == horarioDto.id_horario
                                    && partido.id_cancha == iPartido.cancha.id_cancha)
                                {
                                    IPartido iPartidoExistente = new IPartido();
                                    Cancha cancha = new Cancha();
                                    HorarioFijo horarioFijo = new HorarioFijo();
                                    IEquipo iLocal = new IEquipo(partido.local);
                                    IEquipo iVisitante = new IEquipo(partido.visitante);
                                    Turno turno = new Turno();

                                    var objLocal = (from tEquipos in db.equipos
                                                    join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                    where tEquipos.id_equipo == partido.local
                                                    select new
                                                    {
                                                        id_equipo = tEquipos.id_equipo,
                                                        nombre = tEquipos.nombre,
                                                        imagePath = tArchivos.ThumbPath,
                                                        logo = tEquipos.logo
                                                    }).SingleOrDefault();

                                    var objVisitante = (from tEquipos in db.equipos
                                                        join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                        where tEquipos.id_equipo == partido.visitante
                                                        select new
                                                        {
                                                            id_equipo = tEquipos.id_equipo,
                                                            nombre = tEquipos.nombre,
                                                            imagePath = tArchivos.ThumbPath,
                                                            logo = tEquipos.logo
                                                        }).SingleOrDefault();

                                    iLocal.id_equipo = objLocal.id_equipo;
                                    iLocal.nombre = objLocal.nombre;
                                    iLocal.logo = objLocal.logo;
                                    iLocal.imagePath = objLocal.imagePath;

                                    iVisitante.id_equipo = objVisitante.id_equipo;
                                    iVisitante.nombre = objVisitante.nombre;
                                    iVisitante.logo = objVisitante.logo;
                                    iVisitante.imagePath = objVisitante.imagePath;

                                    iPartidoExistente.local = new List<IEquipo>();
                                    iPartidoExistente.visitante = new List<IEquipo>();

                                    iPartidoExistente.local.Add(iLocal);
                                    iPartidoExistente.visitante.Add(iVisitante);

                                    var canchaDto = db.canchas.SingleOrDefault(x => x.id_cancha == partido.id_cancha);

                                    iPartidoExistente.cancha = cancha;
                                    iPartidoExistente.cancha.id_cancha = (int)partido.id_cancha;
                                    iPartidoExistente.cancha.nombre = canchaDto.nombre;

                                    var horarioDtoExistente = db.horarios_fijos.SingleOrDefault(x => x.id_horario == partido.id_horario_fijo);
                                    iPartidoExistente.horario = horarioFijo;
                                    iPartidoExistente.horario.id_horario = partido.id_horario_fijo;
                                    iPartidoExistente.horario.inicio = horarioDtoExistente.inicio;
                                    iPartidoExistente.horario.fin = horarioDtoExistente.fin;
                                    iPartidoExistente.horario.turno = turno;
                                    iPartidoExistente.horario.turno.id = horarioDtoExistente.id_turno;

                                    iPartidoExistente.id_partido = partido.id_partido;

                                    return Ok(iPartidoExistente);
                                }
                            }
                        }
                    }
                }
                return Ok(new IPartido());
            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/obtenerPartidos/" + " Parametros de entrada: " +
                JsonConvert.SerializeObject(iPartido, Formatting.None), " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/eliminarPartido")]
        public IHttpActionResult eliminarPartido([FromBody]IPartido iPartido)
        {
            try
            {
                var partido = db.partidos.Where(x => x.id_estado_partido == 1 && x.id_resultado == null && x.id_resultados_zona == null
                && x.id_partido == iPartido.id_partido).SingleOrDefault();

                if (partido != null)
                {
                    db.partidos.Remove(partido);
                    db.SaveChanges();

                    return Ok(new IPartido());
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/eliminarPartido/" + " Parametros de entrada: " +
                  JsonConvert.SerializeObject(iPartido, Formatting.None), " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/obtenerTodas/{id_zona}/{id_torneo}")]
        public IHttpActionResult getObtenerTodas(int id_zona, int id_torneo)
        {
            try
            {
                var fechas = (from tFixtureZona in db.fixture_zona
                              join tFecha in db.fechas on tFixtureZona.id_fixture equals tFecha.id_fixture_zona
                              where tFixtureZona.id_torneo == id_torneo && tFixtureZona.id_zona == id_zona
                              select new
                              {
                                  id_fixture = tFixtureZona.id_fixture,
                                  id_torneo = tFixtureZona.id_torneo,
                                  id_fecha = tFecha.id_fecha,
                                  estado = tFecha.id_estado,
                                  fecha = tFecha.fecha
                              }).ToList();

                if (fechas.Count > 0)
                {
                    Fixture fixture = new Fixture();
                    Torneo torneo = new Torneo();
                    List<Fecha> lsFechas = new List<Fecha>();

                    fixture.fechas = lsFechas;
                    fixture.torneo = torneo;

                    foreach (var f in fechas)
                    {
                        fixture.id_fixture = f.id_fixture;
                        fixture.torneo.id_torneo = f.id_torneo;

                        Fecha fecha = new Fecha();
                        EstadoFecha estado = new EstadoFecha();

                        fecha.id_fecha = f.id_fecha;
                        fecha.fecha = (DateTime)f.fecha;
                        fecha.estado = estado;
                        fecha.estado.id_estado = f.estado;


                        var partidos = db.partidos.Where(x => x.id_fecha == fecha.id_fecha).ToList();
                        if (partidos.Count > 0)
                        {
                            List<IPartido> lsPartidos = new List<IPartido>();
                            fecha.iPartidos = lsPartidos;

                            foreach (var partido in partidos)
                            {
                                IPartido iPartidoExistente = new IPartido();
                                Cancha cancha = new Cancha();
                                HorarioFijo horarioFijo = new HorarioFijo();
                                IEquipo iLocal = new IEquipo(partido.local);
                                IEquipo iVisitante = new IEquipo(partido.visitante);
                                Turno turno = new Turno();

                                var objLocal = (from tEquipos in db.equipos
                                                join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                where tEquipos.id_equipo == partido.local
                                                select new
                                                {
                                                    id_equipo = tEquipos.id_equipo,
                                                    nombre = tEquipos.nombre,
                                                    imagePath = tArchivos.ThumbPath,
                                                    logo = tEquipos.logo
                                                }).SingleOrDefault();

                                var objVisitante = (from tEquipos in db.equipos
                                                    join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                    where tEquipos.id_equipo == partido.visitante
                                                    select new
                                                    {
                                                        id_equipo = tEquipos.id_equipo,
                                                        nombre = tEquipos.nombre,
                                                        imagePath = tArchivos.ThumbPath,
                                                        logo = tEquipos.logo
                                                    }).SingleOrDefault();

                                iLocal.id_equipo = objLocal.id_equipo;
                                iLocal.nombre = objLocal.nombre;
                                iLocal.logo = objLocal.logo;
                                iLocal.imagePath = objLocal.imagePath;

                                iVisitante.id_equipo = objVisitante.id_equipo;
                                iVisitante.nombre = objVisitante.nombre;
                                iVisitante.logo = objVisitante.logo;
                                iVisitante.imagePath = objVisitante.imagePath;

                                iPartidoExistente.local = new List<IEquipo>();
                                iPartidoExistente.visitante = new List<IEquipo>();

                                iPartidoExistente.local.Add(iLocal);
                                iPartidoExistente.visitante.Add(iVisitante);

                                var canchaDto = db.canchas.SingleOrDefault(x => x.id_cancha == partido.id_cancha);

                                iPartidoExistente.cancha = cancha;
                                iPartidoExistente.cancha.id_cancha = (int)partido.id_cancha;
                                iPartidoExistente.cancha.nombre = canchaDto.nombre;

                                var horarioDtoExistente = db.horarios_fijos.SingleOrDefault(x => x.id_horario == partido.id_horario_fijo);
                                iPartidoExistente.horario = horarioFijo;
                                iPartidoExistente.horario.id_horario = partido.id_horario_fijo;
                                iPartidoExistente.horario.inicio = horarioDtoExistente.inicio;
                                iPartidoExistente.horario.fin = horarioDtoExistente.fin;
                                iPartidoExistente.horario.turno = turno;
                                iPartidoExistente.horario.turno.id = horarioDtoExistente.id_turno;

                                iPartidoExistente.id_partido = partido.id_partido;

                                fecha.iPartidos.Add(iPartidoExistente);

                            }
                        }

                        fixture.fechas.Add(fecha);
                    }

                    return Ok(fixture);
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/obtenerTodas/" + " Parametros de entrada: " +
                id_zona + id_torneo, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/modificarFecha")]
        public IHttpActionResult modificarFecha([FromBody]Fixture fixture)
        {
            try
            {
                var fechas = db.fechas.Where(x => x.id_fixture_zona == fixture.id_fixture).ToList();

                foreach (var f in fechas)
                {

                    foreach (var fix in fixture.fechas)
                    {
                        if (f.id_fecha == fix.id_fecha)
                        {
                            var fecha = db.fechas.SingleOrDefault(x => x.id_fecha == f.id_fecha);
                            fecha.fecha = fix.fecha;
                            db.SaveChanges();
                        }
                    }
                }
                return Ok(true);
            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/modificarFecha/" + " Parametros de entrada: " +
                 JsonConvert.SerializeObject(fixture, Formatting.None), " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/obtenerFInterzonales/{id_torneo}")]
        public IHttpActionResult getObtenerFInterzonales(int id_torneo)
        {
            try
            {
                List<Fecha> lsFechas = new List<Fecha>();
                var fixture = db.fixture_zona.Where(x => x.id_torneo == id_torneo).ToList();
                var id_fase = db.torneos.Where(x => x.id_torneo == id_torneo).FirstOrDefault().id_fase;
                foreach (var fix in fixture)
                {

                    var fechas = db.fechas.Where(x => x.id_fixture_zona == fix.id_fixture && x.id_fase == id_fase).OrderBy(x => x.fecha).ToList();

                    foreach (var f in fechas)
                    {
                        Fecha fechaDto = new Fecha();
                        fechaDto.id_fecha = f.id_fecha;
                        fechaDto.fecha = (System.DateTime)f.fecha;
                        lsFechas.Add(fechaDto);

                    }
                }

                return Ok(lsFechas.OrderBy(x => x.fecha));
            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/obtenerFInterzonales/" + " Parametros de entrada: " +
                 id_torneo, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/obtenerPartidosVisualizacionFixture/{id_torneo}")]
        public IHttpActionResult obtenerPartidosPorFechaFixture([FromBody]Fecha fechaDto, int id_torneo)
        {
            List<IPartido> lsPartidosPrueba = new List<IPartido>();
            try
            {
                var fechas = (from tFixtureZona in db.fixture_zona
                              join tFecha in db.fechas on tFixtureZona.id_fixture equals tFecha.id_fixture_zona
                              where tFixtureZona.id_torneo == id_torneo && tFecha.fecha == fechaDto.fecha
                              select new
                              {
                                  id_fixture = tFixtureZona.id_fixture,
                                  id_torneo = tFixtureZona.id_torneo,
                                  id_fecha = tFecha.id_fecha,
                                  estado = tFecha.id_estado,
                                  fecha = tFecha.fecha
                              }).ToList();

                if (fechas.Count > 0)
                {
                    Fixture fixture = new Fixture();
                    Torneo torneo = new Torneo();
                    List<Fecha> lsFechas = new List<Fecha>();

                    fixture.fechas = lsFechas;
                    fixture.torneo = torneo;

                    foreach (var f in fechas)
                    {
                        fixture.id_fixture = f.id_fixture;
                        fixture.torneo.id_torneo = f.id_torneo;

                        Fecha fecha = new Fecha();
                        EstadoFecha estado = new EstadoFecha();

                        fecha.id_fecha = f.id_fecha;
                        fecha.fecha = (DateTime)f.fecha;
                        fecha.estado = estado;
                        fecha.estado.id_estado = f.estado;


                        var partidos = db.partidos.Where(x => x.id_fecha == f.id_fecha).ToList();
                        if (partidos.Count > 0)
                        {
                            List<IPartido> lsPartidos = new List<IPartido>();
                            fecha.iPartidos = lsPartidos;

                            foreach (var partido in partidos)
                            {
                                IPartido iPartidoExistente = new IPartido();
                                Cancha cancha = new Cancha();
                                HorarioFijo horarioFijo = new HorarioFijo();
                                IEquipo iLocal = new IEquipo(partido.local);
                                IEquipo iVisitante = new IEquipo(partido.visitante);
                                Turno turno = new Turno();

                                var objLocal = (from tEquipos in db.equipos
                                                join tArchivos in db.files on tEquipos.camisetalogo equals tArchivos.Id
                                                where tEquipos.id_equipo == partido.local
                                                select new
                                                {
                                                    id_equipo = tEquipos.id_equipo,
                                                    nombre = tEquipos.nombre,
                                                    imagePath = tArchivos.ThumbPath,
                                                    logo = tEquipos.camisetalogo
                                                }).SingleOrDefault();

                                var objVisitante = (from tEquipos in db.equipos
                                                    join tArchivos in db.files on tEquipos.camisetalogo equals tArchivos.Id
                                                    where tEquipos.id_equipo == partido.visitante
                                                    select new
                                                    {
                                                        id_equipo = tEquipos.id_equipo,
                                                        nombre = tEquipos.nombre,
                                                        imagePath = tArchivos.ThumbPath,
                                                        logo = tEquipos.camisetalogo
                                                    }).SingleOrDefault();

                                iLocal.id_equipo = objLocal.id_equipo;
                                iLocal.nombre = objLocal.nombre;
                                iLocal.logo = objLocal.logo;
                                iLocal.imagePath = objLocal.imagePath;

                                iVisitante.id_equipo = objVisitante.id_equipo;
                                iVisitante.nombre = objVisitante.nombre;
                                iVisitante.logo = objVisitante.logo;
                                iVisitante.imagePath = objVisitante.imagePath;

                                iPartidoExistente.local = new List<IEquipo>();
                                iPartidoExistente.visitante = new List<IEquipo>();

                                iPartidoExistente.local.Add(iLocal);
                                iPartidoExistente.visitante.Add(iVisitante);

                                var canchaDto = db.canchas.SingleOrDefault(x => x.id_cancha == partido.id_cancha);

                                iPartidoExistente.cancha = cancha;
                                iPartidoExistente.cancha.id_cancha = (int)partido.id_cancha;
                                iPartidoExistente.cancha.nombre = canchaDto.nombre;

                                var horarioDtoExistente = db.horarios_fijos.SingleOrDefault(x => x.id_horario == partido.id_horario_fijo);
                                iPartidoExistente.horario = horarioFijo;
                                iPartidoExistente.horario.id_horario = partido.id_horario_fijo;
                                iPartidoExistente.horario.inicio = horarioDtoExistente.inicio;
                                iPartidoExistente.horario.fin = horarioDtoExistente.fin;
                                iPartidoExistente.horario.turno = turno;
                                iPartidoExistente.horario.turno.id = horarioDtoExistente.id_turno;

                                iPartidoExistente.id_partido = partido.id_partido;

                                fecha.iPartidos.Add(iPartidoExistente);
                                lsPartidosPrueba.Add(iPartidoExistente);

                            }
                        }

                        fixture.fechas.Add(fecha);
                    }

                    return Ok(lsPartidosPrueba);
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/obtenerPartidosVisualizacionFixture/" + " Parametros de entrada: " +
                JsonConvert.SerializeObject(fechaDto, Formatting.None) + id_torneo, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }


        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/obtenerResultadosVisualizacionFecha/{id_torneo}")]
        public IHttpActionResult obtenerResultadosFecha([FromBody]Fecha fecha, int id_torneo)
        {
            List<IPartido> lsPartidos = new List<IPartido>();
            try
            {
                var fixture_zona = db.fixture_zona.Where(x => x.id_torneo == id_torneo).ToList();
                foreach (var fix in fixture_zona)
                {
                    var fechas = db.fechas.Where(x => x.id_fixture_zona == fix.id_fixture && x.fecha == fecha.fecha).ToList();


                    foreach (var f in fechas)
                    {
                        foreach (var partido in f.partidos)
                        {
                            IPartido iPartido = new IPartido();
                            Cancha cancha = new Cancha();
                            HorarioFijo horarioFijo = new HorarioFijo();
                            IEquipo iLocal = new IEquipo();
                            IEquipo iVisitante = new IEquipo();
                            Turno turno = new Turno();
                            Fecha fechaPartido = new Fecha();
                            List<Gol> lsGolesLocal = new List<Gol>();
                            List<Gol> lsGolesVisitante = new List<Gol>();
                            List<Sancion> lsSancionesLocal = new List<Sancion>();
                            List<Sancion> lsSancionesVisitante = new List<Sancion>();


                            iPartido.lsGolesLocal = lsGolesLocal;
                            iPartido.lsGolesVisitante = lsGolesVisitante;
                            iPartido.lsSancionesLocal = lsSancionesLocal;
                            iPartido.lsSancionesVisitante = lsSancionesVisitante;


                            var objLocal = (from tEquipos in db.equipos
                                            join tArchivos in db.files on tEquipos.camisetalogo equals tArchivos.Id
                                            where tEquipos.id_equipo == partido.local
                                            select new
                                            {
                                                id_equipo = tEquipos.id_equipo,
                                                nombre = tEquipos.nombre,
                                                imagePath = tArchivos.ThumbPath,
                                                logo = tEquipos.logo
                                            }).SingleOrDefault();

                            var objVisitante = (from tEquipos in db.equipos
                                                join tArchivos in db.files on tEquipos.camisetalogo equals tArchivos.Id
                                                where tEquipos.id_equipo == partido.visitante
                                                select new
                                                {
                                                    id_equipo = tEquipos.id_equipo,
                                                    nombre = tEquipos.nombre,
                                                    imagePath = tArchivos.ThumbPath,
                                                    logo = tEquipos.logo
                                                }).SingleOrDefault();

                            iLocal.id_equipo = objLocal.id_equipo;
                            iLocal.nombre = objLocal.nombre;
                            iLocal.logo = objLocal.logo;
                            iLocal.imagePath = objLocal.imagePath;

                            iVisitante.id_equipo = objVisitante.id_equipo;
                            iVisitante.nombre = objVisitante.nombre;
                            iVisitante.logo = objVisitante.logo;
                            iVisitante.imagePath = objVisitante.imagePath;

                            iPartido.local = new List<IEquipo>();
                            iPartido.visitante = new List<IEquipo>();


                            iPartido.local.Add(iLocal);
                            iPartido.visitante.Add(iVisitante);

                            var canchaDto = db.canchas.SingleOrDefault(x => x.id_cancha == partido.id_cancha);

                            iPartido.cancha = cancha;
                            iPartido.cancha.id_cancha = (int)partido.id_cancha;
                            iPartido.cancha.nombre = canchaDto.nombre;

                            var horarioDto = db.horarios_fijos.SingleOrDefault(x => x.id_horario == partido.id_horario_fijo);
                            iPartido.horario = horarioFijo;
                            iPartido.horario.id_horario = partido.id_horario_fijo;
                            iPartido.horario.inicio = horarioDto.inicio;
                            iPartido.horario.fin = horarioDto.fin;
                            iPartido.horario.turno = turno;
                            iPartido.horario.turno.id = horarioDto.id_turno;
                            iPartido.id_partido = partido.id_partido;
                            iPartido.id_fixture = f.id_fixture_zona;
                            iPartido.fecha = fechaPartido;
                            iPartido.fecha.id_fecha = f.id_fecha;
                            iPartido.fecha.fecha = (DateTime)f.fecha;

                            var sLocales = (from tSancion in db.sanciones
                                            join tJugador in db.jugadores on tSancion.id_jugador equals tJugador.id_jugador
                                            join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                            where tSancion.id_partido == partido.id_partido && tSancion.id_equipo == partido.local
                                            select new
                                            {
                                                id_partido = tSancion.id_partido,
                                                id_jugador = tSancion.id_jugador,
                                                id_sancion = tSancion.id_sancion,
                                                nombre = tPersona.nombre,
                                                apellido = tPersona.apellido,
                                                id_tipo = tSancion.id_tipo
                                            }).ToList();

                            var gLocales = (from tGoles in db.goles
                                            join tJugador in db.jugadores on tGoles.id_jugador equals tJugador.id_jugador
                                            join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                            where tGoles.id_partido == partido.id_partido && tGoles.id_equipo == partido.local
                                            select new
                                            {
                                                id_partido = tGoles.id_partido,
                                                id_jugador = tGoles.id_jugador,
                                                id_gol = tGoles.id_gol,
                                                nombre = tPersona.nombre,
                                                apellido = tPersona.apellido,
                                            }).ToList();


                            var sVisitantes = (from tSancion in db.sanciones
                                               join tJugador in db.jugadores on tSancion.id_jugador equals tJugador.id_jugador
                                               join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                               where tSancion.id_partido == partido.id_partido && tSancion.id_equipo == partido.visitante
                                               select new
                                               {
                                                   id_partido = tSancion.id_partido,
                                                   id_jugador = tSancion.id_jugador,
                                                   id_sancion = tSancion.id_sancion,
                                                   nombre = tPersona.nombre,
                                                   apellido = tPersona.apellido,
                                                   id_tipo = tSancion.id_tipo
                                               }).ToList();

                            var gVisitante = (from tGoles in db.goles
                                              join tJugador in db.jugadores on tGoles.id_jugador equals tJugador.id_jugador
                                              join tPersona in db.personas on tJugador.id_persona equals tPersona.id_persona
                                              where tGoles.id_partido == partido.id_partido && tGoles.id_equipo == partido.visitante
                                              select new
                                              {
                                                  id_partido = tGoles.id_partido,
                                                  id_jugador = tGoles.id_jugador,
                                                  id_gol = tGoles.id_gol,
                                                  nombre = tPersona.nombre,
                                                  apellido = tPersona.apellido,
                                              }).ToList();

                            foreach (var sL in sLocales)
                            {
                                Sancion sancion = new Sancion();
                                TipoSancion tipoSancion = new TipoSancion();
                                Partido partidoSancion = new Partido();
                                Jugador jugador = new Jugador();

                                sancion.partido = partidoSancion;
                                sancion.partido.id_partido = sL.id_partido;
                                sancion.jugador = jugador;
                                sancion.jugador.id_jugador = sL.id_jugador;
                                sancion.jugador.nombre = sL.nombre;
                                sancion.jugador.apellido = sL.apellido;
                                sancion.id_sancion = sL.id_sancion;
                                sancion.tipo_sancion = tipoSancion;
                                sancion.tipo_sancion.id_tipo = sL.id_tipo;
                                sancion.tipo_sancion.descripcion = db.tipos_sanciones.Where(x => x.id_tipo == sL.id_tipo).FirstOrDefault().descripcion;
                                iPartido.lsSancionesLocal.Add(sancion);
                            }

                            foreach (var sV in sVisitantes)
                            {
                                Sancion sancion = new Sancion();
                                TipoSancion tipoSancion = new TipoSancion();
                                Partido partidoSancion = new Partido();
                                Jugador jugador = new Jugador();

                                sancion.partido = partidoSancion;
                                sancion.partido.id_partido = sV.id_partido;
                                sancion.jugador = jugador;
                                sancion.jugador.id_jugador = sV.id_jugador;
                                sancion.jugador.nombre = sV.nombre;
                                sancion.jugador.apellido = sV.apellido;
                                sancion.id_sancion = sV.id_sancion;
                                sancion.tipo_sancion = tipoSancion;
                                sancion.tipo_sancion.id_tipo = sV.id_tipo;
                                sancion.tipo_sancion.descripcion = db.tipos_sanciones.Where(x => x.id_tipo == sV.id_tipo).FirstOrDefault().descripcion;
                                iPartido.lsSancionesVisitante.Add(sancion);
                            }

                            foreach (var gL in gLocales)
                            {
                                Gol gol = new Gol();
                                Partido partidoGol = new Partido();
                                Jugador jugador = new Jugador();

                                gol.partido = partidoGol;
                                gol.partido.id_partido = gL.id_partido;
                                gol.jugador = jugador;
                                gol.jugador.id_jugador = gL.id_jugador;
                                gol.jugador.nombre = gL.nombre;
                                gol.jugador.apellido = gL.apellido;
                                gol.id_gol = gL.id_gol;
                                iPartido.lsGolesLocal.Add(gol);
                            }

                            foreach (var gV in gVisitante)
                            {
                                Gol gol = new Gol();
                                Partido partidoGol = new Partido();
                                Jugador jugador = new Jugador();

                                gol.partido = partidoGol;
                                gol.partido.id_partido = gV.id_partido;
                                gol.jugador = jugador;
                                gol.jugador.id_jugador = gV.id_jugador;
                                gol.jugador.nombre = gV.nombre;
                                gol.jugador.apellido = gV.apellido;
                                gol.id_gol = gV.id_gol;
                                iPartido.lsGolesVisitante.Add(gol);
                            }

                            if (partido.id_resultado != null || partido.id_resultados_zona != null)
                            {
                                lsPartidos.Add(iPartido);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/obtenerResultadosVisualizacionFecha/" + " Parametros de entrada: " +
                JsonConvert.SerializeObject(fecha, Formatting.None) + id_torneo, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
            return Ok(lsPartidos);
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/obtenerFechasJugadas/{id_torneo}")]
        public IHttpActionResult getObtenerFechasJugadas(int id_torneo)
        {
            try
            {
                List<Fecha> lsFechas = new List<Fecha>();
                var fixture = db.fixture_zona.Where(x => x.id_torneo == id_torneo).ToList();
                var id_fase = db.torneos.Where(x => x.id_torneo == id_torneo).FirstOrDefault().id_fase;
                foreach (var fix in fixture)
                {
                    var fechas = db.fechas.Where(x => x.id_fixture_zona == fix.id_fixture && x.id_fase == id_fase).OrderBy(x => x.fecha).ToList();

                    foreach (var f in fechas)
                    {
                        Fecha fechaDto = new Fecha();
                        fechaDto.id_fecha = f.id_fecha;
                        fechaDto.fecha = (System.DateTime)f.fecha;

                        var partidos = db.partidos.Where(x => x.id_fecha == f.id_fecha && (x.id_resultado != null || x.id_resultados_zona != null)).FirstOrDefault();

                        if (partidos != null)
                        {
                            lsFechas.Add(fechaDto);
                        }

                    }
                }

                return Ok(lsFechas);
            }
            catch (Exception e)
            {
                var logger = new Logger("FixtureController");
                logger.AgregarMensaje("api/fecha/obtenerFechasJugadas/" + " Parametros de entrada: " +
                id_torneo, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }
    }
}