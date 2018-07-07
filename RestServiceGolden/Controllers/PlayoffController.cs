using RestServiceGolden.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestServiceGolden.Controllers
{
    public class PlayoffController : ApiController
    {
        goldenEntities db = new goldenEntities();
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/etapas/todas")]
        public IHttpActionResult getEtapasTodas()
        {
            List<Etapa> lsEtapas = new List<Etapa>();

            var etapas = db.etapa_playoff.ToList();

            foreach (var e in etapas)
            {
                Etapa etapa = new Etapa();
                etapa.descripcion = e.descripcion;
                etapa.id_etapa = e.id_etapa;
                lsEtapas.Add(etapa);
            }

            return Ok(lsEtapas);
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/llaves/todas")]
        public IHttpActionResult getLlavesTodas()
        {
            List<Llave> lsLlaves = new List<Llave>();

            var llaves = db.llaves.ToList();

            foreach (var l in llaves)
            {
                Llave llave = new Llave();
                llave.descripcion = l.descripcion;
                llave.id_llave = l.id_llave;
                lsLlaves.Add(llave);
            }

            return Ok(lsLlaves);
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/playoff/{id_torneo}")]
        public IHttpActionResult getPlayoffsPorTorneo(int id_torneo)
        {
            List<IPartido> lsPartidos = new List<IPartido>();
            try
            {
                var lsPartidosPlayoff = db.playoff.Where(x => x.id_torneo == id_torneo).ToList();

                foreach (var partido in lsPartidosPlayoff)
                {
                    IPartido iPartido = new IPartido();
                    Cancha cancha = new Cancha();
                    HorarioFijo horarioFijo = new HorarioFijo();
                    IEquipo iLocal = new IEquipo();
                    IEquipo iVisitante = new IEquipo();
                    Turno turno = new Turno();
                    Fecha fechaPartido = new Fecha();
                    Llave llave = new Llave();
                    Etapa etapa = new Etapa();
                    List<Gol> lsGolesLocal = new List<Gol>();
                    List<Gol> lsGolesVisitante = new List<Gol>();
                    IEquipo iGanador = new IEquipo();

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

                    iPartido.lsGolesLocal = lsGolesLocal;
                    iPartido.lsGolesVisitante = lsGolesVisitante;
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

                    if (partido.ganador == iLocal.id_equipo)
                    {
                        iGanador = iLocal;
                    }
                    if(partido.ganador == iVisitante.id_equipo)
                    {
                        iGanador = iVisitante;
                    }

                    iPartido.ganadorPlayoff = iGanador;
                    iPartido.cancha = cancha;
                    iPartido.horario = horarioFijo;
                    iPartido.fecha = fechaPartido;
                    iPartido.horario.turno = turno;

                    iPartido.llave = llave;
                    iPartido.llave.id_llave = partido.llave;
                    var llaveDto = db.llaves.SingleOrDefault(x => x.id_llave == partido.llave);
                    iPartido.llave.descripcion = llaveDto.descripcion;

                    iPartido.etapa = etapa;
                    var etapaDto = db.etapa_playoff.SingleOrDefault(x => x.id_etapa == partido.id_etapa);
                    iPartido.etapa.id_etapa = partido.id_etapa;
                    iPartido.etapa.descripcion = etapaDto.descripcion;

                    lsPartidos.Add(iPartido);
                }
                return Ok(lsPartidos);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }

        }
    }
}