using RestServiceGolden.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestServiceGolden.Controllers
{
    public class PartidosController : ApiController
    {
        goldenEntities db = new goldenEntities();


        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/partidos/registrar/{id_fase}/{id_torneo}/{esInterzonal}")]
        public IHttpActionResult registrar([FromBody] List<Partido> lsPartidos, int id_fase, int id_torneo, int esInterzonal)
        {
            try

            {
                int id_resultado_zona = 0;
                int id_resultado = 0;
                foreach (var partido in lsPartidos)
                {
                    //Primer paso registro Goles
                    foreach (var golL in partido.lsGoleadoresLocales)
                    {
                        goles golDto = new goles();
                        golDto.id_jugador = golL.jugador.id_jugador;
                        golDto.id_partido = partido.id_partido;
                        golDto.id_equipo = golL.equipo.id_equipo;
                        golDto.id_torneo = id_torneo;
                        db.goles.Add(golDto);

                    }
                    foreach (var golV in partido.lsGoleadoresVisitantes)
                    {
                        goles golDtoV = new goles();
                        golDtoV.id_jugador = golV.jugador.id_jugador;
                        golDtoV.id_partido = partido.id_partido;
                        golDtoV.id_equipo = golV.equipo.id_equipo;
                        golDtoV.id_torneo = id_torneo;
                        db.goles.Add(golDtoV);

                    }
                    //Segundo paso registro Sanciones
                    foreach (var sancionL in partido.lsSancionesLocal)
                    {
                        sanciones sancionDtoL = new sanciones();
                        sancionDtoL.id_jugador = sancionL.jugador.id_jugador;
                        sancionDtoL.id_partido = sancionL.partido.id_partido;
                        sancionDtoL.id_equipo = sancionL.equipo.id_equipo;
                        sancionDtoL.id_tipo = sancionL.tipo_sancion.id_tipo;
                        if (sancionL.tipo_sancion.id_tipo == 1)
                        {
                            sancionDtoL.fecha_inicio = partido.fecha.id_fecha;
                            sancionDtoL.fecha_fin = partido.fecha.id_fecha;
                        }
                        else
                        {
                            sancionDtoL.fecha_inicio = sancionL.fecha_inicio.id_fecha;
                            sancionDtoL.fecha_fin = sancionL.fecha_fin.id_fecha;
                        }

                        sancionDtoL.detalle = sancionL.detalle;
                        sancionDtoL.id_fase = sancionL.fase.id_fase;
                        sancionDtoL.id_torneo = id_torneo;

                        if (esInterzonal == 1)
                        {
                            sancionDtoL.id_zona = null;
                        }
                        else
                        {
                            sancionDtoL.id_zona = sancionL.zona.id_zona;
                        }

                        db.sanciones.Add(sancionDtoL);
                    }

                    foreach (var sancionV in partido.lsSancionesVisitante)
                    {
                        sanciones sancionDtoV = new sanciones();
                        sancionDtoV.id_jugador = sancionV.jugador.id_jugador;
                        sancionDtoV.id_partido = sancionV.partido.id_partido;
                        sancionDtoV.id_tipo = sancionV.tipo_sancion.id_tipo;
                        sancionDtoV.id_equipo = sancionV.equipo.id_equipo;
                        if (sancionV.tipo_sancion.id_tipo == 1)
                        {
                            sancionDtoV.fecha_inicio = partido.fecha.id_fecha;
                            sancionDtoV.fecha_fin = partido.fecha.id_fecha;
                        }
                        else
                        {
                            sancionDtoV.fecha_inicio = sancionV.fecha_inicio.id_fecha;
                            sancionDtoV.fecha_fin = sancionV.fecha_fin.id_fecha;
                        }

                        sancionDtoV.detalle = sancionV.detalle;
                        sancionDtoV.id_fase = sancionV.fase.id_fase;
                        sancionDtoV.id_torneo = id_torneo;

                        if (esInterzonal == 1)
                        {
                            sancionDtoV.id_zona = null;
                        }
                        else
                        {
                            sancionDtoV.id_zona = sancionV.zona.id_zona;
                        }
                        db.sanciones.Add(sancionDtoV);
                    }

                    //Tercer paso registro resultados (Es partido fase 1 2 o 3)
                    if (esInterzonal != 0)
                    {
                        resultados resultadoDto = new resultados();
                        resultadoDto.id_ganador = partido.resultado.ganador.id_equipo;
                        resultadoDto.id_perdedor = partido.resultado.perdedor.id_equipo;

                        if (partido.resultado.empate == 0)
                        {
                            resultadoDto.empate = null;
                        }
                        else
                        {
                            resultadoDto.empate = 1;
                        }
                        db.resultados.Add(resultadoDto);
                        db.SaveChanges();
                        id_resultado = resultadoDto.id_resultado;

                    }
                    else
                    {
                        resultados_zona resultadoDto2 = new resultados_zona();
                        resultadoDto2.id_ganador = partido.resultado_zona.ganador.id_equipo;
                        resultadoDto2.id_perdedor = partido.resultado_zona.perdedor.id_equipo;
                        resultadoDto2.id_zona = partido.resultado_zona.zona.id_zona;
                        if (partido.resultado_zona.empate == 0)
                        {
                            resultadoDto2.empate = null;
                        }
                        else
                        {
                            resultadoDto2.empate = 1;
                        }
                        db.resultados_zona.Add(resultadoDto2);
                        db.SaveChanges();
                        id_resultado_zona = resultadoDto2.id_resultado;
                    }

                    //Cuarto paso actualizo partidos con el id_resultado o id_resultado_zona

                    if (esInterzonal != 0)
                    {
                        var partidoUpdate = db.partidos.SingleOrDefault(x => x.id_partido == partido.id_partido);
                        //Esto significa que es interzonal por lo tanto guardo en id_resultado
                        partidoUpdate.id_resultado = id_resultado;
                        partidoUpdate.esInterzonal = 1;
                        db.SaveChanges();

                    }
                    else
                    {
                        var partidoUpdate = db.partidos.SingleOrDefault(x => x.id_partido == partido.id_partido);
                        //Esto significa que es interzonal por lo tanto guardo en id_resultado
                        partidoUpdate.id_resultados_zona = id_resultado_zona;
                        db.SaveChanges();
                    }

                    //Actualizo o creo posiciones si no existe para fase 1
                    if (id_fase == 1)
                    {
                        posiciones posicionZonaLocal = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.local.id_equipo
                            && x.id_torneo == id_torneo);

                        if (posicionZonaLocal == null)
                        {
                            posiciones posicionesDtoLocal = new posiciones();
                            posicionesDtoLocal.id_torneo = partido.local.torneo.id_torneo;
                            posicionesDtoLocal.id_equipo = partido.local.id_equipo;
                            posicionesDtoLocal.goles_favor = partido.lsGoleadoresLocales.Count;
                            posicionesDtoLocal.goles_contra = partido.lsGoleadoresVisitantes.Count;
                            posicionesDtoLocal.dif_gol = partido.lsGoleadoresLocales.Count - partido.lsGoleadoresVisitantes.Count;


                            if (esInterzonal == 0)
                            {
                                if (partido.local.id_equipo == partido.resultado_zona.ganador.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 3;

                                }
                                else if (partido.local.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 0;
                                }
                                else
                                {
                                    posicionesDtoLocal.puntos = 1;
                                }
                            }
                            else
                            {
                                if (partido.local.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 3;

                                }
                                else if (partido.local.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 0;
                                }
                                else
                                {
                                    posicionesDtoLocal.puntos = 1;
                                }
                            }

                            db.posiciones.Add(posicionesDtoLocal);
                            db.SaveChanges();
                        }
                        else
                        {
                            if (esInterzonal == 0)
                            {
                                //Significa que ya existe hay q actualizar y no crear
                                if (partido.local.id_equipo == partido.resultado_zona.ganador.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 3;

                                }
                                else if (partido.local.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos;
                                }
                                else
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 1;
                                }
                            }
                            else
                            {
                                //Significa que ya existe hay q actualizar y no crear
                                if (partido.local.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 3;

                                }
                                else if (partido.local.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos;
                                }
                                else
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 1;
                                }
                            }


                            posicionZonaLocal.goles_favor = posicionZonaLocal.goles_favor + partido.lsGoleadoresLocales.Count;
                            posicionZonaLocal.goles_contra = posicionZonaLocal.goles_contra + partido.lsGoleadoresVisitantes.Count;
                            posicionZonaLocal.dif_gol = posicionZonaLocal.goles_favor - posicionZonaLocal.goles_contra;
                            db.SaveChanges();
                        }

                        //Posiciones zonas visitante
                        posiciones posicionZonaVisitante = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.visitante.id_equipo && x.id_torneo == id_torneo);

                        if (posicionZonaVisitante == null)
                        {
                            posiciones posicionesDtoVisitante = new posiciones();
                            posicionesDtoVisitante.id_torneo = partido.visitante.torneo.id_torneo;
                            posicionesDtoVisitante.id_equipo = partido.visitante.id_equipo;
                            posicionesDtoVisitante.goles_favor = partido.lsGoleadoresVisitantes.Count;
                            posicionesDtoVisitante.goles_contra = partido.lsGoleadoresLocales.Count;
                            posicionesDtoVisitante.dif_gol = partido.lsGoleadoresVisitantes.Count - partido.lsGoleadoresLocales.Count;

                            if (esInterzonal == 0)
                            {
                                if (partido.visitante.id_equipo == partido.resultado_zona.ganador.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 3;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 0;
                                }
                                else
                                {
                                    posicionesDtoVisitante.puntos = 1;
                                }
                                db.posiciones.Add(posicionesDtoVisitante);
                                db.SaveChanges();
                            }
                            else
                            {
                                if (partido.visitante.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 3;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 0;
                                }
                                else
                                {
                                    posicionesDtoVisitante.puntos = 1;
                                }
                                db.posiciones.Add(posicionesDtoVisitante);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            //Significa que ya existe hay q actualizar y no crear
                            if (esInterzonal == 0)
                            {
                                if (partido.visitante.id_equipo == partido.resultado_zona.ganador.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 3;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos;
                                }
                                else
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 1;
                                }

                                posicionZonaVisitante.goles_favor = posicionZonaVisitante.goles_favor + partido.lsGoleadoresVisitantes.Count;
                                posicionZonaVisitante.goles_contra = posicionZonaVisitante.goles_contra + partido.lsGoleadoresLocales.Count;
                                posicionZonaVisitante.dif_gol = posicionZonaVisitante.goles_favor - posicionZonaVisitante.goles_contra;
                                db.SaveChanges();
                            }
                            else
                            {
                                if (partido.visitante.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 3;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos;
                                }
                                else
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 1;
                                }

                                posicionZonaVisitante.goles_favor = posicionZonaVisitante.goles_favor + partido.lsGoleadoresVisitantes.Count;
                                posicionZonaVisitante.goles_contra = posicionZonaVisitante.goles_contra + partido.lsGoleadoresLocales.Count;
                                posicionZonaVisitante.dif_gol = posicionZonaVisitante.goles_favor - posicionZonaVisitante.goles_contra;
                                db.SaveChanges();
                            }
                        }
                    }

                    //Actualizo o creo posiciones si no existe para fase 2
                    if (id_fase == 2)
                    {
                        posiciones_zona posicionZonaLocal;
                        if (esInterzonal == 1)
                        {
                            posicionZonaLocal = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.local.id_equipo
                            && x.id_torneo == id_torneo);
                        }
                        else
                        {
                            posicionZonaLocal = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.local.id_equipo
                                && x.id_torneo == id_torneo && x.id_zona == partido.resultado_zona.zona.id_zona);

                        }

                        if (posicionZonaLocal == null)
                        {
                            posiciones_zona posicionesDtoLocal = new posiciones_zona();
                            posicionesDtoLocal.id_torneo = partido.local.torneo.id_torneo;
                            posicionesDtoLocal.id_equipo = partido.local.id_equipo;
                            posicionesDtoLocal.goles_favor = partido.lsGoleadoresLocales.Count;
                            posicionesDtoLocal.goles_contra = partido.lsGoleadoresVisitantes.Count;
                            posicionesDtoLocal.dif_gol = partido.lsGoleadoresLocales.Count - partido.lsGoleadoresVisitantes.Count;


                            if (esInterzonal == 0)
                            {
                                posicionesDtoLocal.id_zona = partido.resultado_zona.zona.id_zona;
                                if (partido.local.id_equipo == partido.resultado_zona.ganador.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 3;

                                }
                                else if (partido.local.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 0;
                                }
                                else
                                {
                                    posicionesDtoLocal.puntos = 1;
                                }
                            }
                            else
                            {
                                if (partido.local.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 3;

                                }
                                else if (partido.local.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 0;
                                }
                                else
                                {
                                    posicionesDtoLocal.puntos = 1;
                                }
                            }

                            db.posiciones_zona.Add(posicionesDtoLocal);
                            db.SaveChanges();
                        }
                        else
                        {
                            if (esInterzonal == 0)
                            {
                                //Significa que ya existe hay q actualizar y no crear
                                if (partido.local.id_equipo == partido.resultado_zona.ganador.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 3;

                                }
                                else if (partido.local.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos;
                                }
                                else
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 1;
                                }
                            }
                            else
                            {
                                //Significa que ya existe hay q actualizar y no crear
                                if (partido.local.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 3;

                                }
                                else if (partido.local.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos;
                                }
                                else
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 1;
                                }
                            }


                            posicionZonaLocal.goles_favor = posicionZonaLocal.goles_favor + partido.lsGoleadoresLocales.Count;
                            posicionZonaLocal.goles_contra = posicionZonaLocal.goles_contra + partido.lsGoleadoresVisitantes.Count;
                            posicionZonaLocal.dif_gol = posicionZonaLocal.goles_favor - posicionZonaLocal.goles_contra;
                            db.SaveChanges();
                        }

                        //Posiciones zonas visitante
                        posiciones_zona posicionZonaVisitante;
                        if (esInterzonal == 1)
                        {
                            posicionZonaVisitante = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.visitante.id_equipo
                            && x.id_torneo == id_torneo);
                        }
                        else
                        {
                            posicionZonaVisitante = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.visitante.id_equipo
                                && x.id_torneo == id_torneo && x.id_zona == partido.resultado_zona.zona.id_zona);
                        }

                        if (posicionZonaVisitante == null)
                        {
                            posiciones_zona posicionesDtoVisitante = new posiciones_zona();
                            posicionesDtoVisitante.id_torneo = partido.visitante.torneo.id_torneo;
                            posicionesDtoVisitante.id_equipo = partido.visitante.id_equipo;
                            posicionesDtoVisitante.goles_favor = partido.lsGoleadoresVisitantes.Count;
                            posicionesDtoVisitante.goles_contra = partido.lsGoleadoresLocales.Count;
                            posicionesDtoVisitante.dif_gol = partido.lsGoleadoresVisitantes.Count - partido.lsGoleadoresLocales.Count;

                            if (esInterzonal == 0)
                            {
                                posicionesDtoVisitante.id_zona = partido.resultado_zona.zona.id_zona;
                                if (partido.visitante.id_equipo == partido.resultado_zona.ganador.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 3;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 0;
                                }
                                else
                                {
                                    posicionesDtoVisitante.puntos = 1;
                                }
                                db.posiciones_zona.Add(posicionesDtoVisitante);
                                db.SaveChanges();
                            }
                            else
                            {
                                if (partido.visitante.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 3;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 0;
                                }
                                else
                                {
                                    posicionesDtoVisitante.puntos = 1;
                                }
                                db.posiciones_zona.Add(posicionesDtoVisitante);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            //Significa que ya existe hay q actualizar y no crear
                            if (esInterzonal == 0)
                            {
                                if (partido.visitante.id_equipo == partido.resultado_zona.ganador.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 3;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos;
                                }
                                else
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 1;
                                }

                                posicionZonaVisitante.goles_favor = posicionZonaVisitante.goles_favor + partido.lsGoleadoresVisitantes.Count;
                                posicionZonaVisitante.goles_contra = posicionZonaVisitante.goles_contra + partido.lsGoleadoresLocales.Count;
                                posicionZonaVisitante.dif_gol = posicionZonaVisitante.goles_favor - posicionZonaVisitante.goles_contra;
                                db.SaveChanges();
                            }
                            else
                            {
                                if (partido.visitante.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 3;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos;
                                }
                                else
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 1;
                                }

                                posicionZonaVisitante.goles_favor = posicionZonaVisitante.goles_favor + partido.lsGoleadoresVisitantes.Count;
                                posicionZonaVisitante.goles_contra = posicionZonaVisitante.goles_contra + partido.lsGoleadoresLocales.Count;
                                posicionZonaVisitante.dif_gol = posicionZonaVisitante.goles_favor - posicionZonaVisitante.goles_contra;
                                db.SaveChanges();
                            }
                        }
                    }

                    if (id_fase == 3)
                    {
                        // Pendiente Play Off
                    }
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