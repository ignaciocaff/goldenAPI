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
    public class PosicionesController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/posiciones/{id_torneo}")]
        public IHttpActionResult getPosiciones(int id_torneo)
        {
            try
            {
                var torneos = db.torneos.Where(x => x.id_torneo == id_torneo).FirstOrDefault();

                if (torneos.id_fase == 1)
                {
                    List<Posiciones> lsPosiciones = new List<Posiciones>();
                    var posiciones = db.posiciones.Where(x => x.id_torneo == id_torneo).OrderByDescending(x => x.puntos).ThenByDescending(x => x.dif_gol).ThenByDescending(x => x.goles_favor).ToList();

                    foreach (var pos in posiciones)
                    {
                        var eq = db.equipos.Where(x => x.id_equipo == pos.id_equipo).FirstOrDefault();
                        var escudo = db.files.Where(x => x.Id == eq.logo).FirstOrDefault();

                        Posiciones posicion = new Posiciones();
                        IEquipo equipo = new IEquipo();
                        Torneo torneo = new Torneo();

                        posicion.id_posicion = pos.id_posicion;
                        posicion.puntos = (int)pos.puntos;
                        posicion.goles_contra = (int)pos.goles_contra;
                        posicion.goles_favor = (int)pos.goles_favor;
                        posicion.dif_gol = (int)pos.dif_gol;
                        posicion.partidos_jugados = (int)pos.partidos_jugados;
                        posicion.partidos_ganados = (int)pos.partidos_ganados;
                        posicion.partidos_empatados = (int)pos.partidos_empatados;
                        posicion.partidos_perdidos = (int)pos.partidos_perdidos;

                        posicion.equipo = equipo;
                        posicion.equipo.id_equipo = eq.id_equipo;
                        posicion.equipo.nombre = eq.nombre;
                        posicion.equipo.logo = (int)eq.logo;
                        posicion.equipo.imagePath = escudo.ThumbPath;

                        posicion.torneo = torneo;

                        lsPosiciones.Add(posicion);
                    }
                    return Ok(lsPosiciones);
                }
                else
                {
                    var lsZonas = new List<List<PosicionesZonas>>();
                    //var lsIdZonas = db.posiciones_zona.Where(x => x.id_torneo == id_torneo).Select(x => x.id_zona).Distinct().ToList();

                    var lsIdZonas = (from tPosZona in db.posiciones_zona
                                     join tZona in db.zonas on tPosZona.id_zona equals tZona.id_zona
                                     where tPosZona.id_torneo == id_torneo
                                     select new
                                     {
                                         id = tPosZona.id_zona,
                                         descripcion = tZona.descripcion,
                                     }).Distinct().ToList().OrderBy(x => x.descripcion);

                    foreach (var singleZona in lsIdZonas)
                    {
                        var posiciones = db.posiciones_zona.Where(x => x.id_zona == singleZona.id).OrderByDescending(x => x.puntos).ThenByDescending(x => x.dif_gol).ThenByDescending(x => x.goles_favor).ToList();
                        List<PosicionesZonas> lsPosiciones = new List<PosicionesZonas>();

                        foreach (var pos in posiciones)
                        {
                            var eq = db.equipos.Where(x => x.id_equipo == pos.id_equipo).FirstOrDefault();
                            var z = db.zonas.Where(x => x.id_zona == singleZona.id).FirstOrDefault();
                            var escudo = db.files.Where(x => x.Id == eq.logo).FirstOrDefault();
                            PosicionesZonas posicion = new PosicionesZonas();
                            IEquipo equipo = new IEquipo();
                            Torneo torneo = new Torneo();
                            Zona zona = new Zona();

                            posicion.id_posicion = pos.id_posicion;
                            posicion.puntos = (int)pos.puntos;
                            posicion.goles_contra = (int)pos.goles_contra;
                            posicion.goles_favor = (int)pos.goles_favor;
                            posicion.dif_gol = (int)pos.dif_gol;
                            posicion.partidos_jugados = (int)pos.partidos_jugados;
                            posicion.partidos_ganados = (int)pos.partidos_ganados;
                            posicion.partidos_empatados = (int)pos.partidos_empatados;
                            posicion.partidos_perdidos = (int)pos.partidos_perdidos;

                            posicion.equipo = equipo;
                            posicion.equipo.id_equipo = eq.id_equipo;
                            posicion.equipo.nombre = eq.nombre;
                            posicion.equipo.logo = (int)eq.logo;
                            posicion.equipo.imagePath = escudo.ThumbPath;

                            posicion.torneo = torneo;
                            posicion.torneo.id_torneo = id_torneo;

                            posicion.zona = zona;
                            posicion.zona.id_zona = singleZona.id;
                            posicion.zona.descripcion = z.descripcion;

                            lsPosiciones.Add(posicion);
                        }
                        lsZonas.Add(lsPosiciones);
                    }
                    return Ok(lsZonas);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/goleadores/{id_torneo}")]
        public IHttpActionResult getGoleadores(int id_torneo)
        {
            try
            {
                var goleadores = db.goleadores.Where(x => x.id_torneo == id_torneo).OrderByDescending(x => x.cantidad_goles).Take(20).ToList();
                List<Goleador> lsGoleadores = new List<Goleador>();

                foreach (var goleador in goleadores)
                {
                    Goleador goleadorTorneo = new Goleador();
                    Torneo torneo = new Torneo();
                    IEquipo equipo = new IEquipo();
                    Jugador jugador = new Jugador();

                    var eq = db.equipos.Where(x => x.id_equipo == goleador.id_equipo).FirstOrDefault();
                    var escudo = db.files.Where(x => x.Id == eq.logo).FirstOrDefault();
                    var jug = db.jugadores.Where(x => x.id_jugador == goleador.id_jugador).FirstOrDefault();
                    var per = db.personas.Where(x => x.id_persona == jug.id_persona).FirstOrDefault();

                    goleadorTorneo.cantidad_goles = (int)goleador.cantidad_goles;

                    goleadorTorneo.torneo = torneo;
                    goleador.torneos.id_torneo = id_torneo;

                    goleadorTorneo.equipo = equipo;
                    goleadorTorneo.equipo.nombre = eq.nombre;
                    goleadorTorneo.equipo.id_equipo = eq.id_equipo;
                    goleadorTorneo.equipo.imagePath = escudo.ThumbPath;

                    goleadorTorneo.jugador = jugador;
                    goleadorTorneo.jugador.id_jugador = jug.id_jugador;
                    goleadorTorneo.jugador.nombre = per.nombre;
                    goleadorTorneo.jugador.apellido = per.apellido;

                    lsGoleadores.Add(goleadorTorneo);
                }

                return Ok(lsGoleadores);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


    }
}