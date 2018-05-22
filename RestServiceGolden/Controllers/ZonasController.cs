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
    public class ZonasController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/zona/todos/{id}")]
        public IHttpActionResult GetAll(int id)
        {
            List<Zona> lsZonas = new List<Zona>();
            try
            {
                var zonas = db.zonas.ToList().OrderBy(z => z.descripcion);
                var torneoIdFase = db.torneos.Where(x => x.id_torneo == id).FirstOrDefault();
                foreach (var tZona in zonas)
                {
                    if (torneoIdFase != null && tZona.id_torneo == id && tZona.id_fase == torneoIdFase.id_fase)
                    {
                        Zona zona = new Zona();
                        Torneo torneo = new Torneo();
                        List<Equipo> lsEquipos = new List<Equipo>();
                        zona.id_zona = tZona.id_zona;
                        zona.descripcion = tZona.descripcion;
                        zona.torneo = torneo;
                        zona.torneo.id_torneo = tZona.id_torneo;

                        var equipos = (from tEquipos in db.equipos
                                       join tEquiposZona in db.equipos_zona on tEquipos.id_equipo equals tEquiposZona.id_equipo
                                       join tZonas in db.zonas on tEquiposZona.id_zona equals tZonas.id_zona
                                       where tZonas.id_zona == zona.id_zona
                                       select new
                                       {
                                           id_equipo = tEquipos.id_equipo,
                                           nombre = tEquipos.nombre,
                                           id_torneo = tEquipos.id_torneo,
                                           logo = tEquipos.logo,
                                           id_zona = tZonas.id_zona,
                                           camisetalogo = tEquipos.camisetalogo
                                       }).OrderBy(s => s.nombre);


                        foreach (var equipoZona in equipos)
                        {
                            Equipo equipo = new Equipo();
                            Torneo torneoEquipo = new Torneo();
                            int? idZona = equipoZona.id_zona;
                            if (idZona != null)
                            {
                                equipo.id_equipo = equipoZona.id_equipo;
                                equipo.nombre = equipoZona.nombre;
                                equipo.logo = (equipoZona.logo != null) ? (int)equipoZona.logo : 0;
                                equipo.camisetalogo = (equipoZona.camisetalogo != null) ? (int)equipoZona.camisetalogo : 0;
                                equipo.torneo = torneo;
                                equipo.torneo.id_torneo = equipoZona.id_torneo;
                                lsEquipos.Add(equipo);
                            }
                        }

                        zona.lsEquipos = lsEquipos;
                        lsZonas.Add(zona);
                    }
                }
                return Ok(lsZonas);
            }
            catch (Exception e)
            {
                var logger = new Logger("ZonasController");
                logger.AgregarMensaje("api/zona/todos/" + " Parametros de entrada: " +
                  id, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/zona/tienePlayoff/{id_torneo}")]
        public IHttpActionResult getTienePlayOff(int id_torneo)
        {
            try
            {
                var lsZonas = db.zonas.Where(x => x.id_torneo == id_torneo).ToList();
                foreach (var zona in lsZonas)
                {
                    if (zona.id_fase == 3)
                    {
                        return Ok(true);
                    }
                }
                return Ok(false);
            }
            catch (Exception e)
            {
                var logger = new Logger("ZonasController");
                logger.AgregarMensaje("api/zona/tienePlayoff/" + " Parametros de entrada: " +
                  id_torneo, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/zona/registrar")]
        public IHttpActionResult registrar([FromBody]List<Zona> lsZonas)
        {

            int id_zona = 0;
            List<Equipo> lsEquipos = new List<Equipo>();
            try
            {
                foreach (Zona z in lsZonas)
                {
                    int zonaCheck = db.zonas.Where(x => x.descripcion == z.descripcion && x.id_torneo == z.torneo.id_torneo && x.id_fase == z.torneo.fase.id_fase).Count();
                    if (zonaCheck != 0)
                    {
                        return BadRequest("Alguna de las zonas con ese nombre ya ha sido creada para ese torneo, para esta fase");
                    }
                }

                foreach (Zona zona in lsZonas)
                {
                    zonas zonaDto = new zonas();
                    lsEquipos = zona.lsEquipos;
                    zonaDto.id_torneo = zona.torneo.id_torneo;
                    zonaDto.id_fase = zona.torneo.fase.id_fase;
                    zonaDto.descripcion = zona.descripcion;
                    db.zonas.Add(zonaDto);
                    db.SaveChanges();
                    id_zona = zonaDto.id_zona;
                    foreach (var equiposZona in lsEquipos)
                    {
                        equipos_zona equiposZonaDto = db.equipos_zona.SingleOrDefault(x => x.id_equipo == equiposZona.id_equipo);
                        if (equiposZonaDto != null)
                        {
                            equiposZonaDto.id_zona = id_zona;
                            db.SaveChanges();

                        }
                        else
                        {
                            equipos_zona equipoZonaInsert = new equipos_zona();
                            equipoZonaInsert.id_equipo = equiposZona.id_equipo;
                            equipoZonaInsert.id_zona = id_zona;
                            equipoZonaInsert.id_torneo = zona.torneo.id_torneo;
                            db.equipos_zona.Add(equipoZonaInsert);
                            db.SaveChanges();
                        }

                    }

                    fixture_zona fixture_zona = new fixture_zona();
                    fixture_zona.id_tipo = 1;
                    fixture_zona.id_torneo = zona.torneo.id_torneo;
                    fixture_zona.id_zona = id_zona;
                    db.fixture_zona.Add(fixture_zona);
                    db.SaveChanges();

                    var fixtureInterzonal = db.fixture_zona.Where(x => x.id_torneo == zona.torneo.id_torneo && x.id_zona == null).FirstOrDefault();
                    if (fixtureInterzonal == null)
                    {
                        fixture_zona fixture_zona_interzonal = new fixture_zona();
                        fixture_zona_interzonal.id_tipo = 1;
                        fixture_zona_interzonal.id_torneo = zona.torneo.id_torneo;
                        db.fixture_zona.Add(fixture_zona_interzonal);
                        db.SaveChanges();
                    }
                }
                return Ok();

            }
            catch (Exception e)
            {
                var logger = new Logger("ZonasController");
                logger.AgregarMensaje("api/zona/registrar/" + " Parametros de entrada: " +
                  JsonConvert.SerializeObject(lsZonas, Formatting.None), " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/zona/eliminar")]
        public IHttpActionResult eliminar([FromBody]Zona zona)
        {
            List<Equipo> lsEquipos = new List<Equipo>();
            try
            {
                foreach (var equiposZona in zona.lsEquipos)
                {
                    equipos_zona equiposZonaDto = db.equipos_zona.SingleOrDefault(x => x.id_equipo == equiposZona.id_equipo);
                    equiposZonaDto.id_zona = null;
                }

                fixture_zona fixture_zona = db.fixture_zona.SingleOrDefault(x => x.id_zona == zona.id_zona);
                int id_fixture_zona = fixture_zona.id_fixture;

                var fechas = db.fechas.Where(x => x.id_fixture_zona == id_fixture_zona).ToList();

                foreach (var fecha in fechas)
                {
                    db.fechas.Attach(fecha);
                    db.fechas.Remove(fecha);

                    var partidos = db.partidos.Where(x => x.id_fecha == fecha.id_fecha).ToList();

                    foreach (var partido in partidos)
                    {
                        var resultado = db.resultados.SingleOrDefault(x => x.id_resultado == partido.id_resultado);
                        var resultado_zona = db.resultados_zona.SingleOrDefault(x => x.id_resultado == partido.id_resultados_zona);

                        if (resultado != null)
                        {
                            db.resultados.Attach(resultado);
                            db.resultados.Remove(resultado);
                        }

                        if (resultado_zona != null)
                        {
                            db.resultados_zona.Attach(resultado_zona);
                            db.resultados_zona.Remove(resultado_zona);
                        }
                        db.partidos.Attach(partido);
                        db.partidos.Remove(partido);
                    }

                }

                db.fixture_zona.Attach(fixture_zona);
                db.fixture_zona.Remove(fixture_zona);

                zonas zonaDto = new zonas();
                lsEquipos = zona.lsEquipos;

                foreach (var equipo in lsEquipos)
                {
                    var posicion = db.posiciones.Where(x => x.id_equipo == equipo.id_equipo && x.id_torneo == equipo.torneo.id_torneo).FirstOrDefault();
                    var posicion_zona = db.posiciones_zona.Where(x => x.id_equipo == equipo.id_equipo && x.id_torneo == equipo.torneo.id_torneo).FirstOrDefault();

                    if (posicion != null)
                    {
                        db.posiciones.Attach(posicion);
                        db.posiciones.Remove(posicion);
                    }

                    if (posicion_zona != null)
                    {
                        db.posiciones_zona.Attach(posicion_zona);
                        db.posiciones_zona.Remove(posicion_zona);
                    }
                }

                zonaDto.id_torneo = zona.torneo.id_torneo;
                zonaDto.descripcion = zona.descripcion;
                zonaDto.id_zona = (int)zona.id_zona;
                db.zonas.Attach(zonaDto);
                db.zonas.Remove(zonaDto);
                db.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                var logger = new Logger("ZonasController");
                logger.AgregarMensaje("api/zona/eliminar/" + " Parametros de entrada: " +
                  JsonConvert.SerializeObject(zona, Formatting.None), " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/zona/update")]
        public IHttpActionResult update([FromBody]List<Zona> lsZonas)
        {

            List<Equipo> lsEquipos = new List<Equipo>();
            try
            {

                foreach (Zona zona in lsZonas)
                {
                    var totalEquipos = db.equipos_zona.Where(x => x.id_zona == zona.id_zona).ToList();
                    lsEquipos = zona.lsEquipos;

                    var result = totalEquipos.Where(p => !lsEquipos.Any(l => p.id_equipo == l.id_equipo)).ToList();

                    foreach (var r in result)
                    {
                        equipos_zona equiposZonaDto = db.equipos_zona.SingleOrDefault(x => x.id_equipo == r.id_equipo);
                        equiposZonaDto.id_zona = null;
                        db.SaveChanges();
                    }


                    foreach (var equiposZona in lsEquipos)
                    {
                        equipos_zona equiposZonaDto = db.equipos_zona.SingleOrDefault(x => x.id_equipo == equiposZona.id_equipo);
                        equiposZonaDto.id_zona = zona.id_zona;
                        db.SaveChanges();

                    }
                }
                return Ok();

            }
            catch (Exception e)
            {
                var logger = new Logger("ZonasController");
                logger.AgregarMensaje("api/zona/update/" + " Parametros de entrada: " +
                  JsonConvert.SerializeObject(lsZonas, Formatting.None), " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }
    }
}