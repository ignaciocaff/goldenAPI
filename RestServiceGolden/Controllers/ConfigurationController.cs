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
    public class ConfigurationController: ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/reglasTorneo/registrar")]
        public IHttpActionResult registrar([FromBody]ReglaTorneo regla)
        {
            reglas_torneo reglaDto = new reglas_torneo();
                        
            try
            {
                reglaDto.descripcion = regla.descripcion;
                reglaDto.id_torneo = regla.torneo.id_torneo;

                db.reglas_torneo.Add(reglaDto);
                db.SaveChanges();
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/reglasTorneo")]
        public IHttpActionResult GetAll()
        {
            List<ReglaTorneo> lsReglas = new List<ReglaTorneo>();

            try
            {
                var reglas = db.reglas_torneo.ToList();

                foreach (var r in reglas)
                {
                    var tor = db.torneos.Where(x => x.id_torneo == r.id_torneo).FirstOrDefault();
                    ReglaTorneo regla = new ReglaTorneo();
                    Torneo torneo = new Torneo();
                    regla.id_regla = r.id_regla;
                    regla.descripcion = r.descripcion;
                    regla.torneo = torneo;
                    regla.torneo.id_torneo = (int)tor.id_torneo;
                    regla.torneo.nombre = tor.nombre;
                    lsReglas.Add(regla);
                }
                return Ok(lsReglas);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [Route("api/reglasTorneo/update")]
        public IHttpActionResult updateReglaTorneo([FromBody]ReglaTorneo regla)
        {
            reglas_torneo reglaDto = new reglas_torneo();

            try
            {
                reglaDto.id_regla = (int)regla.id_regla;
                reglaDto.descripcion = regla.descripcion;
                reglaDto.id_torneo = (int)regla.torneo.id_torneo;

                reglas_torneo reglas = db.reglas_torneo.Where(x => x.id_regla == regla.id_regla).FirstOrDefault();

                if(reglas != null)
                {
                    reglas.id_regla = reglaDto.id_regla;
                    reglas.descripcion = reglaDto.descripcion;
                    reglas.id_torneo = reglaDto.id_torneo;
                    db.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [Route("api/sancion_equipo/registrar")]
        public IHttpActionResult registrarSancionEquipo([FromBody]SancionEquipo sancion)
        {
            sanciones_equipo sancionDto = new sanciones_equipo();

            try
            {
                var zonaEquipo = db.equipos_zona.Where(x => x.id_equipo == sancion.equipo.id_equipo).FirstOrDefault();

                var torneos = db.torneos.Where(x => x.id_torneo == sancion.torneo.id_torneo).FirstOrDefault();
                if (torneos.id_fase == 1)
                {
                    var posiciones = db.posiciones.Where(x => x.id_equipo == sancion.equipo.id_equipo && x.id_torneo == sancion.torneo.id_torneo).FirstOrDefault();

                    if(posiciones != null)
                    {
                        posiciones.puntos = posiciones.puntos - sancion.puntos_restados;
                    }
                    else
                    {
                        posiciones posicion = new posiciones();
                        posicion.id_equipo = sancion.equipo.id_equipo;
                        posicion.puntos = 0 - sancion.puntos_restados;
                        posicion.goles_favor = 0;
                        posicion.goles_contra = 0;
                        posicion.dif_gol = 0;
                        posicion.id_torneo = sancion.torneo.id_torneo;
                        db.posiciones.Add(posicion);
                    }
                }
                if (torneos.id_fase == 2)
                {
                    var posiciones_zona = db.posiciones_zona.Where(x => x.id_equipo == sancion.equipo.id_equipo && x.id_zona == zonaEquipo.id_zona).FirstOrDefault();

                    if (posiciones_zona != null)
                    {
                        posiciones_zona.puntos = posiciones_zona.puntos - sancion.puntos_restados;
                    }
                    else
                    {
                        posiciones_zona posicion = new posiciones_zona();
                        posicion.id_equipo = sancion.equipo.id_equipo;
                        posicion.puntos = 0 - sancion.puntos_restados;
                        posicion.goles_favor = 0;
                        posicion.goles_contra = 0;
                        posicion.dif_gol = 0;
                        posicion.id_torneo = sancion.torneo.id_torneo;
                        posicion.id_zona = zonaEquipo.id_zona;
                        db.posiciones_zona.Add(posicion);
                    }
                }


                sancionDto.descripcion = sancion.descripcion;
                sancionDto.puntos_restados = sancion.puntos_restados;
                sancionDto.id_equipo = sancion.equipo.id_equipo;
                sancionDto.id_zona = zonaEquipo.id_zona;
                sancionDto.id_torneo = sancion.torneo.id_torneo;

                db.sanciones_equipo.Add(sancionDto);
                db.SaveChanges();
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [Route("api/sancion_equipo/sanciones/{id_equipo}")]
        public IHttpActionResult getSancionesEquipo(int id_equipo)
        {
            try
            {
                List<SancionEquipo> lsSanciones = new List<SancionEquipo>();

                var equipo_zona = db.equipos_zona.Where(x => x.id_equipo == id_equipo).FirstOrDefault();
                if (equipo_zona != null)
                {
                    var sanciones_equipo = db.sanciones_equipo.Where(x => x.id_equipo == id_equipo && x.id_zona == equipo_zona.id_zona && x.id_torneo == equipo_zona.id_torneo).ToList();
                    var zonas = db.zonas.Where(x => x.id_zona == equipo_zona.id_zona).FirstOrDefault();

                    if(sanciones_equipo != null)
                    {
                        foreach(var san in sanciones_equipo) {
                            SancionEquipo sancion = new SancionEquipo();
                            Equipo equipo = new Equipo();
                            Torneo torneo = new Torneo();
                            Zona zona = new Zona();

                            sancion.id_sancion_equipo = san.id_sancion_equipo;
                            sancion.descripcion = san.descripcion;
                            sancion.puntos_restados = (int)san.puntos_restados;

                            sancion.equipo = equipo;
                            sancion.equipo.id_equipo = san.id_equipo;

                            sancion.torneo = torneo;
                            sancion.torneo.id_torneo = san.id_torneo;

                            sancion.zona = zona;
                            sancion.zona.id_zona = san.id_zona;
                            sancion.zona.descripcion = zonas.descripcion;

                            lsSanciones.Add(sancion);
                        }
                        return Ok(lsSanciones);
                    }
                    return BadRequest("No existen sanciones para el equipo seleccionado");
                }
                return BadRequest("El equipo no pertenece a una zona");
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [Route("api/sancion_equipo/borrar")]
        public IHttpActionResult borrarSancionEquipo(SancionEquipo sancion)
        {
            try
            {
                var zona = db.zonas.Where(x => x.id_zona == sancion.zona.id_zona).FirstOrDefault();

                if (zona.id_fase == 1)
                {
                    posiciones posiciones = db.posiciones.Where(x => x.id_equipo == sancion.equipo.id_equipo && x.id_torneo == sancion.torneo.id_torneo).FirstOrDefault();

                    if (posiciones != null)
                    {
                        posiciones.puntos = posiciones.puntos + sancion.puntos_restados;
                    }
                } 
                else if (zona.id_fase == 2)
                {
                    posiciones_zona posiciones = db.posiciones_zona.Where(x => x.id_equipo == sancion.equipo.id_equipo && x.id_torneo == sancion.torneo.id_torneo && x.id_zona == sancion.zona.id_zona).FirstOrDefault();

                    if (posiciones != null)
                    {
                        posiciones.puntos = posiciones.puntos + sancion.puntos_restados;
                    }
                }
                sanciones_equipo sancionDto = db.sanciones_equipo.Where(x => x.id_sancion_equipo == sancion.id_sancion_equipo).FirstOrDefault();

                db.sanciones_equipo.Attach(sancionDto);
                db.sanciones_equipo.Remove(sancionDto);
                db.SaveChanges();

                return Ok();                
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}