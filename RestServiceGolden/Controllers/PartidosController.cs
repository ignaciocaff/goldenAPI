using Newtonsoft.Json;
using RestServiceGolden.Models;
using RestServiceGolden.Utilidades;
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

                        if (partido.resultado.empate == null)
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
                        if (partido.resultado_zona.empate == null)
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
                            posicionesDtoLocal.partidos_jugados = 1;


                            if (esInterzonal == 0)
                            {
                                if (partido.local.id_equipo == partido.resultado_zona.ganador.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 3;
                                    posicionesDtoLocal.partidos_ganados = 1;
                                    posicionesDtoLocal.partidos_empatados = 0;
                                    posicionesDtoLocal.partidos_perdidos = 0;

                                }
                                else if (partido.local.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 0;
                                    posicionesDtoLocal.partidos_ganados = 0;
                                    posicionesDtoLocal.partidos_empatados = 0;
                                    posicionesDtoLocal.partidos_perdidos = 1;
                                }
                                else
                                {
                                    posicionesDtoLocal.puntos = 1;
                                    posicionesDtoLocal.partidos_ganados = 0;
                                    posicionesDtoLocal.partidos_empatados = 1;
                                    posicionesDtoLocal.partidos_perdidos = 0;
                                }
                            }
                            else
                            {
                                if (partido.local.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 3;
                                    posicionesDtoLocal.partidos_ganados = 1;
                                    posicionesDtoLocal.partidos_empatados = 0;
                                    posicionesDtoLocal.partidos_perdidos = 0;

                                }
                                else if (partido.local.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 0;
                                    posicionesDtoLocal.partidos_ganados = 0;
                                    posicionesDtoLocal.partidos_empatados = 0;
                                    posicionesDtoLocal.partidos_perdidos = 1;
                                }
                                else
                                {
                                    posicionesDtoLocal.puntos = 1;
                                    posicionesDtoLocal.partidos_ganados = 0;
                                    posicionesDtoLocal.partidos_empatados = 1;
                                    posicionesDtoLocal.partidos_perdidos = 0;
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
                                    posicionZonaLocal.partidos_ganados++;

                                }
                                else if (partido.local.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos;
                                    posicionZonaLocal.partidos_perdidos++;
                                }
                                else
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 1;
                                    posicionZonaLocal.partidos_empatados++;
                                }
                            }
                            else
                            {
                                //Significa que ya existe hay q actualizar y no crear
                                if (partido.local.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 3;
                                    posicionZonaLocal.partidos_ganados++;
                                }
                                else if (partido.local.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos;
                                    posicionZonaLocal.partidos_perdidos++;
                                }
                                else
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 1;
                                    posicionZonaLocal.partidos_empatados++;
                                }
                            }


                            posicionZonaLocal.goles_favor = posicionZonaLocal.goles_favor + partido.lsGoleadoresLocales.Count;
                            posicionZonaLocal.goles_contra = posicionZonaLocal.goles_contra + partido.lsGoleadoresVisitantes.Count;
                            posicionZonaLocal.dif_gol = posicionZonaLocal.goles_favor - posicionZonaLocal.goles_contra;
                            posicionZonaLocal.partidos_jugados++;
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
                            posicionesDtoVisitante.partidos_jugados = 1;

                            if (esInterzonal == 0)
                            {
                                if (partido.visitante.id_equipo == partido.resultado_zona.ganador.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 3;
                                    posicionesDtoVisitante.partidos_ganados = 1;
                                    posicionesDtoVisitante.partidos_empatados = 0;
                                    posicionesDtoVisitante.partidos_perdidos = 0;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 0;
                                    posicionesDtoVisitante.partidos_ganados = 0;
                                    posicionesDtoVisitante.partidos_empatados = 0;
                                    posicionesDtoVisitante.partidos_perdidos = 1;
                                }
                                else
                                {
                                    posicionesDtoVisitante.puntos = 1;
                                    posicionesDtoVisitante.partidos_ganados = 0;
                                    posicionesDtoVisitante.partidos_empatados = 1;
                                    posicionesDtoVisitante.partidos_perdidos = 0;
                                }
                                db.posiciones.Add(posicionesDtoVisitante);
                                db.SaveChanges();
                            }
                            else
                            {
                                if (partido.visitante.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 3;
                                    posicionesDtoVisitante.partidos_ganados = 1;
                                    posicionesDtoVisitante.partidos_empatados = 0;
                                    posicionesDtoVisitante.partidos_perdidos = 0;
                                }
                                else if (partido.visitante.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 0;
                                    posicionesDtoVisitante.partidos_ganados = 0;
                                    posicionesDtoVisitante.partidos_empatados = 0;
                                    posicionesDtoVisitante.partidos_perdidos = 1;
                                }
                                else
                                {
                                    posicionesDtoVisitante.puntos = 1;
                                    posicionesDtoVisitante.partidos_ganados = 0;
                                    posicionesDtoVisitante.partidos_empatados = 1;
                                    posicionesDtoVisitante.partidos_perdidos = 0;
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
                                    posicionZonaVisitante.partidos_ganados++;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos;
                                    posicionZonaVisitante.partidos_perdidos++;
                                }
                                else
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 1;
                                    posicionZonaVisitante.partidos_empatados++;
                                }

                                posicionZonaVisitante.goles_favor = posicionZonaVisitante.goles_favor + partido.lsGoleadoresVisitantes.Count;
                                posicionZonaVisitante.goles_contra = posicionZonaVisitante.goles_contra + partido.lsGoleadoresLocales.Count;
                                posicionZonaVisitante.dif_gol = posicionZonaVisitante.goles_favor - posicionZonaVisitante.goles_contra;
                                posicionZonaVisitante.partidos_jugados++;
                                db.SaveChanges();
                            }
                            else
                            {
                                if (partido.visitante.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 3;
                                    posicionZonaVisitante.partidos_ganados++;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos;
                                    posicionZonaVisitante.partidos_perdidos++;
                                }
                                else
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 1;
                                    posicionZonaVisitante.partidos_empatados++;
                                }

                                posicionZonaVisitante.goles_favor = posicionZonaVisitante.goles_favor + partido.lsGoleadoresVisitantes.Count;
                                posicionZonaVisitante.goles_contra = posicionZonaVisitante.goles_contra + partido.lsGoleadoresLocales.Count;
                                posicionZonaVisitante.dif_gol = posicionZonaVisitante.goles_favor - posicionZonaVisitante.goles_contra;
                                posicionZonaVisitante.partidos_jugados++;
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
                            posicionesDtoLocal.partidos_jugados = 1;


                            if (esInterzonal == 0)
                            {
                                posicionesDtoLocal.id_zona = partido.resultado_zona.zona.id_zona;
                                if (partido.local.id_equipo == partido.resultado_zona.ganador.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 3;
                                    posicionesDtoLocal.partidos_ganados = 1;
                                    posicionesDtoLocal.partidos_empatados = 0;
                                    posicionesDtoLocal.partidos_perdidos = 0;

                                }
                                else if (partido.local.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 0;
                                    posicionesDtoLocal.partidos_ganados = 0;
                                    posicionesDtoLocal.partidos_empatados = 0;
                                    posicionesDtoLocal.partidos_perdidos = 1;
                                }
                                else
                                {
                                    posicionesDtoLocal.puntos = 1;
                                    posicionesDtoLocal.partidos_ganados = 0;
                                    posicionesDtoLocal.partidos_empatados = 1;
                                    posicionesDtoLocal.partidos_perdidos = 0;
                                }
                            }
                            else
                            {
                                posicionesDtoLocal.id_zona = db.equipos_zona.SingleOrDefault(x => x.id_equipo == partido.resultado.ganador.id_equipo).id_zona;
                                if (partido.local.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 3;
                                    posicionesDtoLocal.partidos_ganados = 1;
                                    posicionesDtoLocal.partidos_empatados = 0;
                                    posicionesDtoLocal.partidos_perdidos = 0;

                                }
                                else if (partido.local.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoLocal.puntos = 0;
                                    posicionesDtoLocal.partidos_ganados = 0;
                                    posicionesDtoLocal.partidos_empatados = 0;
                                    posicionesDtoLocal.partidos_perdidos = 1;
                                }
                                else
                                {
                                    posicionesDtoLocal.puntos = 1;
                                    posicionesDtoLocal.partidos_ganados = 0;
                                    posicionesDtoLocal.partidos_empatados = 1;
                                    posicionesDtoLocal.partidos_perdidos = 0;
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
                                    posicionZonaLocal.partidos_ganados++;

                                }
                                else if (partido.local.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos;
                                    posicionZonaLocal.partidos_perdidos++;
                                }
                                else
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 1;
                                    posicionZonaLocal.partidos_empatados++;
                                }
                            }
                            else
                            {
                                //Significa que ya existe hay q actualizar y no crear
                                if (partido.local.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 3;
                                    posicionZonaLocal.partidos_ganados++;

                                }
                                else if (partido.local.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos;
                                    posicionZonaLocal.partidos_perdidos++;

                                }
                                else
                                {
                                    posicionZonaLocal.puntos = posicionZonaLocal.puntos + 1;
                                    posicionZonaLocal.partidos_empatados++;

                                }
                            }


                            posicionZonaLocal.goles_favor = posicionZonaLocal.goles_favor + partido.lsGoleadoresLocales.Count;
                            posicionZonaLocal.goles_contra = posicionZonaLocal.goles_contra + partido.lsGoleadoresVisitantes.Count;
                            posicionZonaLocal.dif_gol = posicionZonaLocal.goles_favor - posicionZonaLocal.goles_contra;
                            posicionZonaLocal.partidos_jugados++;
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
                            posicionesDtoVisitante.partidos_jugados = 1;

                            if (esInterzonal == 0)
                            {
                                posicionesDtoVisitante.id_zona = partido.resultado_zona.zona.id_zona;
                                if (partido.visitante.id_equipo == partido.resultado_zona.ganador.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 3;
                                    posicionesDtoVisitante.partidos_ganados = 1;
                                    posicionesDtoVisitante.partidos_empatados = 0;
                                    posicionesDtoVisitante.partidos_perdidos = 0;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 0;
                                    posicionesDtoVisitante.partidos_ganados = 0;
                                    posicionesDtoVisitante.partidos_empatados = 0;
                                    posicionesDtoVisitante.partidos_perdidos = 1;
                                }
                                else
                                {
                                    posicionesDtoVisitante.puntos = 1;
                                    posicionesDtoVisitante.partidos_ganados = 0;
                                    posicionesDtoVisitante.partidos_empatados = 1;
                                    posicionesDtoVisitante.partidos_perdidos = 0;
                                }
                                db.posiciones_zona.Add(posicionesDtoVisitante);
                                db.SaveChanges();
                            }
                            else
                            {
                                posicionesDtoVisitante.id_zona = db.equipos_zona.SingleOrDefault(x => x.id_equipo == partido.resultado.ganador.id_equipo).id_zona;
                                if (partido.visitante.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 3;
                                    posicionesDtoVisitante.partidos_ganados = 1;
                                    posicionesDtoVisitante.partidos_empatados = 0;
                                    posicionesDtoVisitante.partidos_perdidos = 0;

                                }
                                else if (partido.visitante.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionesDtoVisitante.puntos = 0;
                                    posicionesDtoVisitante.partidos_ganados = 0;
                                    posicionesDtoVisitante.partidos_empatados = 0;
                                    posicionesDtoVisitante.partidos_perdidos = 1;
                                }
                                else
                                {
                                    posicionesDtoVisitante.puntos = 1;
                                    posicionesDtoVisitante.partidos_ganados = 0;
                                    posicionesDtoVisitante.partidos_empatados = 1;
                                    posicionesDtoVisitante.partidos_perdidos = 0;
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
                                    posicionZonaVisitante.partidos_ganados++;
                                }
                                else if (partido.visitante.id_equipo == partido.resultado_zona.perdedor.id_equipo && partido.resultado_zona.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos;
                                    posicionZonaVisitante.partidos_perdidos++;
                                }
                                else
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 1;
                                    posicionZonaVisitante.partidos_empatados++;
                                }

                                posicionZonaVisitante.goles_favor = posicionZonaVisitante.goles_favor + partido.lsGoleadoresVisitantes.Count;
                                posicionZonaVisitante.goles_contra = posicionZonaVisitante.goles_contra + partido.lsGoleadoresLocales.Count;
                                posicionZonaVisitante.dif_gol = posicionZonaVisitante.goles_favor - posicionZonaVisitante.goles_contra;
                                posicionZonaVisitante.partidos_jugados++;
                                db.SaveChanges();
                            }
                            else
                            {
                                if (partido.visitante.id_equipo == partido.resultado.ganador.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 3;
                                    posicionZonaVisitante.partidos_ganados++;
                                }
                                else if (partido.visitante.id_equipo == partido.resultado.perdedor.id_equipo && partido.resultado.empate != 1)
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos;
                                    posicionZonaVisitante.partidos_perdidos++;
                                }
                                else
                                {
                                    posicionZonaVisitante.puntos = posicionZonaVisitante.puntos + 1;
                                    posicionZonaVisitante.partidos_empatados++;
                                }

                                posicionZonaVisitante.goles_favor = posicionZonaVisitante.goles_favor + partido.lsGoleadoresVisitantes.Count;
                                posicionZonaVisitante.goles_contra = posicionZonaVisitante.goles_contra + partido.lsGoleadoresLocales.Count;
                                posicionZonaVisitante.dif_gol = posicionZonaVisitante.goles_favor - posicionZonaVisitante.goles_contra;
                                posicionZonaVisitante.partidos_jugados++;
                                db.SaveChanges();
                            }
                        }
                    }

                    if (id_fase == 3)
                    {
                        // Pendiente Play Off
                    }

                    actualizarGoleadores(partido, id_torneo);
                }
                return Ok();
            }
            catch (Exception e)
            {
                var logger = new Logger("PartidosController");
                logger.AgregarMensaje("api/partidos/registrar/" + " Parametros de entrada: " +
                JsonConvert.SerializeObject(lsPartidos, Formatting.None) + id_fase + id_torneo + esInterzonal, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }


        public void actualizarGoleadores(Partido partido, int id_torneo)
        {
            try
            {

                foreach (var gol in partido.lsGoleadoresLocales)
                {
                    goleadores goleador = db.goleadores.Where(x => x.id_jugador == gol.jugador.id_jugador && x.id_equipo == gol.equipo.id_equipo && x.id_torneo == id_torneo).SingleOrDefault();

                    if (goleador != null)
                    {
                        goleador.cantidad_goles = goleador.cantidad_goles + 1;
                    }
                    else
                    {
                        goleadores goleadorLocal = new goleadores();
                        goleadorLocal.id_equipo = gol.equipo.id_equipo;
                        goleadorLocal.id_jugador = gol.jugador.id_jugador;
                        goleadorLocal.id_torneo = id_torneo;
                        goleadorLocal.cantidad_goles = 1;
                        db.goleadores.Add(goleadorLocal);
                    }
                    db.SaveChanges();
                }

                foreach (var gol in partido.lsGoleadoresVisitantes)
                {
                    goleadores goleador = db.goleadores.Where(x => x.id_jugador == gol.jugador.id_jugador && x.id_equipo == gol.equipo.id_equipo && x.id_torneo == id_torneo).SingleOrDefault();

                    if (goleador != null)
                    {
                        goleador.cantidad_goles = goleador.cantidad_goles + 1;
                    }
                    else
                    {
                        goleadores goleadorVisitante = new goleadores();
                        goleadorVisitante.id_equipo = gol.equipo.id_equipo;
                        goleadorVisitante.id_jugador = gol.jugador.id_jugador;
                        goleadorVisitante.id_torneo = id_torneo;
                        goleadorVisitante.cantidad_goles = 1;
                        db.goleadores.Add(goleadorVisitante);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public void modificarGoleadores(Partido partido, int id_torneo)
        {
            try
            {

                foreach (var gol in partido.lsGoleadoresLocales)
                {
                    goleadores goleador = db.goleadores.Where(x => x.id_jugador == gol.jugador.id_jugador && x.id_equipo == gol.equipo.id_equipo && x.id_torneo == id_torneo).SingleOrDefault();

                    if (goleador != null)
                    {
                        goleador.cantidad_goles = goleador.cantidad_goles + 1;
                    }
                    else
                    {
                        goleadores goleadorLocal = new goleadores();
                        goleadorLocal.id_equipo = gol.equipo.id_equipo;
                        goleadorLocal.id_jugador = gol.jugador.id_jugador;
                        goleadorLocal.id_torneo = id_torneo;
                        goleadorLocal.cantidad_goles = 1;
                        db.goleadores.Add(goleadorLocal);
                    }
                    db.SaveChanges();
                }

                foreach (var gol in partido.lsGoleadoresVisitantes)
                {
                    goleadores goleador = db.goleadores.Where(x => x.id_jugador == gol.jugador.id_jugador && x.id_equipo == gol.equipo.id_equipo && x.id_torneo == id_torneo).SingleOrDefault();

                    if (goleador != null)
                    {
                        goleador.cantidad_goles = goleador.cantidad_goles + 1;
                    }
                    else
                    {
                        goleadores goleadorVisitante = new goleadores();
                        goleadorVisitante.id_equipo = gol.equipo.id_equipo;
                        goleadorVisitante.id_jugador = gol.jugador.id_jugador;
                        goleadorVisitante.id_torneo = id_torneo;
                        goleadorVisitante.cantidad_goles = 1;
                        db.goleadores.Add(goleadorVisitante);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/partido/eliminar/sancion/{id_sancion}")]
        public IHttpActionResult getEliminarSancion(int id_sancion)
        {
            try
            {
                goldenEntities db = new goldenEntities();
                var sancion = db.sanciones.SingleOrDefault(x => x.id_sancion == id_sancion);
                db.sanciones.Remove(sancion);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                var logger = new Logger("PartidosController");
                logger.AgregarMensaje("api/partido/eliminar/sancion/" + " Parametros de entrada: " +
                id_sancion, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest("No es posible eliminar la sanción" + e.Message);
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/partido/eliminar/gol/{id_gol}/{id_fase}/{id_zona}")]
        public IHttpActionResult getEliminargol(int id_gol, int id_fase, int id_zona)
        {
            try
            {
                goldenEntities db = new goldenEntities();
                var gol = db.goles.SingleOrDefault(x => x.id_gol == id_gol);

                var goleador = db.goleadores.SingleOrDefault(x => x.id_jugador == gol.id_jugador && x.id_equipo == gol.id_equipo && x.id_torneo == gol.id_torneo);

                goleador.cantidad_goles = goleador.cantidad_goles - 1;

                var partido = db.partidos.SingleOrDefault(x => x.id_partido == gol.id_partido);

                var posicionEquipoContrario = new posiciones();
                var posicionesEquipoContrarioPorZona = new posiciones_zona();
                if (partido.local != gol.id_equipo)
                {
                    if (id_fase == 1)
                    {
                        //Significa que a este equipo le tengo q restar un gol en contra.
                        posicionEquipoContrario = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.local && x.id_torneo == gol.id_torneo);
                        posicionEquipoContrario.goles_contra = posicionEquipoContrario.goles_contra - 1;
                        posicionEquipoContrario.dif_gol = posicionEquipoContrario.goles_favor - posicionEquipoContrario.goles_contra;
                    }
                    else if (id_fase == 2)
                    {
                        if (id_zona != 0)
                        {
                            var equipoContrarioInterzonal = db.equipos_zona.SingleOrDefault(x => x.id_equipo == partido.local);

                            posicionesEquipoContrarioPorZona = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.local && x.id_torneo == gol.id_torneo
                                && x.id_zona == equipoContrarioInterzonal.id_zona);
                            posicionesEquipoContrarioPorZona.goles_contra = posicionesEquipoContrarioPorZona.goles_contra - 1;
                            posicionesEquipoContrarioPorZona.dif_gol = posicionesEquipoContrarioPorZona.goles_favor - posicionesEquipoContrarioPorZona.goles_contra;
                        }
                        else
                        {
                            //Es interzonal
                            var equipoInterzonal = db.equipos_zona.SingleOrDefault(x => x.id_equipo == partido.local);

                            var posicionEquipo = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.local && x.id_torneo == gol.id_torneo && x.id_zona == equipoInterzonal.id_zona);
                            posicionEquipo.goles_contra = posicionEquipo.goles_contra - 1;
                            posicionEquipo.dif_gol = posicionEquipo.goles_favor - posicionEquipo.goles_contra;
                        }
                    }
                    else
                    {
                        //Es fase 3, PlayOff
                    }
                }
                else
                {
                    //Significa que el otro es el contrario
                    posicionEquipoContrario = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.visitante && x.id_torneo == gol.id_torneo);

                    if (id_fase == 1)
                    {
                        //Significa que a este equipo le tengo q restar un gol en contra.
                        posicionEquipoContrario = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.visitante && x.id_torneo == gol.id_torneo);
                        posicionEquipoContrario.goles_contra = posicionEquipoContrario.goles_contra - 1;
                        posicionEquipoContrario.dif_gol = posicionEquipoContrario.goles_favor - posicionEquipoContrario.goles_contra;
                    }
                    else if (id_fase == 2)
                    {
                        if (id_zona != 0)
                        {
                            var equipoContrarioInterzonal = db.equipos_zona.SingleOrDefault(x => x.id_equipo == partido.visitante);

                            posicionesEquipoContrarioPorZona = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.visitante && x.id_torneo == gol.id_torneo
                                && x.id_zona == equipoContrarioInterzonal.id_zona);
                            posicionesEquipoContrarioPorZona.goles_contra = posicionesEquipoContrarioPorZona.goles_contra - 1;
                            posicionesEquipoContrarioPorZona.dif_gol = posicionesEquipoContrarioPorZona.goles_favor - posicionesEquipoContrarioPorZona.goles_contra;
                        }
                        else
                        {
                            //Es interzonal
                            var equipoInterzonal = db.equipos_zona.SingleOrDefault(x => x.id_equipo == partido.visitante);

                            var posicionEquipo = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.visitante && x.id_torneo == gol.id_torneo && x.id_zona == equipoInterzonal.id_zona);
                            posicionEquipo.goles_contra = posicionEquipo.goles_contra - 1;
                            posicionEquipo.dif_gol = posicionEquipo.goles_favor - posicionEquipo.goles_contra;
                        }
                    }
                    else
                    {
                        //Es fase 3, PlayOff
                    }
                }

                if (id_fase == 1)
                {
                    var posicionEquipo = db.posiciones.SingleOrDefault(x => x.id_equipo == gol.id_equipo && x.id_torneo == gol.id_torneo);

                    posicionEquipo.goles_favor = posicionEquipo.goles_favor - 1;
                    posicionEquipo.dif_gol = posicionEquipo.goles_favor - posicionEquipo.goles_contra;
                }
                else if (id_fase == 2)
                {
                    if (id_zona != 0)
                    {
                        var posicionEquipo = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == gol.id_equipo && x.id_torneo == gol.id_torneo && x.id_zona == id_zona);
                        posicionEquipo.goles_favor = posicionEquipo.goles_favor - 1;
                        posicionEquipo.dif_gol = posicionEquipo.goles_favor - posicionEquipo.goles_contra;
                    }
                    else
                    {
                        //Es interzonal
                        var equipoInterzonal = db.equipos_zona.SingleOrDefault(x => x.id_equipo == gol.id_equipo);

                        var posicionEquipo = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == gol.id_equipo && x.id_torneo == gol.id_torneo && x.id_zona == equipoInterzonal.id_zona);
                        posicionEquipo.goles_favor = posicionEquipo.goles_favor - 1;
                        posicionEquipo.dif_gol = posicionEquipo.goles_favor - posicionEquipo.goles_contra;
                    }
                }
                else
                {
                    //Fase 3, pendiente
                }

                db.goles.Remove(gol);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                var logger = new Logger("PartidosController");
                logger.AgregarMensaje("api/partido/eliminar/gol" + " Parametros de entrada: " +
                id_gol + id_fase + id_zona, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest("No es posible eliminar el gol" + e.Message);
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/partidos/modificar/{id_fase}/{id_torneo}/{esInterzonal}")]
        public IHttpActionResult modificar([FromBody] Partido partido, int id_fase, int id_torneo, int esInterzonal)
        {
            try
            {
                List<Gol> lsGolesLocal = new List<Gol>();
                List<Gol> lsGolesVisitante = new List<Gol>();
                //Primer paso registro Goles
                foreach (var golL in partido.lsGoleadoresLocales)
                {
                    if (golL.id_gol == null)
                    {
                        goles golDto = new goles();
                        golDto.id_jugador = golL.jugador.id_jugador;
                        golDto.id_partido = partido.id_partido;
                        golDto.id_equipo = golL.equipo.id_equipo;
                        golDto.id_torneo = id_torneo;
                        db.goles.Add(golDto);
                        lsGolesLocal.Add(golL);
                    }
                }
                foreach (var golV in partido.lsGoleadoresVisitantes)
                {
                    if (golV.id_gol == null)
                    {
                        goles golDtoV = new goles();
                        golDtoV.id_jugador = golV.jugador.id_jugador;
                        golDtoV.id_partido = partido.id_partido;
                        golDtoV.id_equipo = golV.equipo.id_equipo;
                        golDtoV.id_torneo = id_torneo;
                        db.goles.Add(golDtoV);
                        lsGolesVisitante.Add(golV);
                    }
                }
                partido.lsGoleadoresLocales = lsGolesLocal;
                partido.lsGoleadoresVisitantes = lsGolesVisitante;
                modificarGoleadores(partido, id_torneo);
                //Segundo paso registro Sanciones
                foreach (var sancionL in partido.lsSancionesLocal)
                {
                    if (sancionL.id_sancion == null)
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
                }

                foreach (var sancionV in partido.lsSancionesVisitante)
                {
                    if (sancionV.id_sancion == null)
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
                }
                db.SaveChanges();
                //Tercer paso registro resultados (Es partido fase 1 2 o 3)
                if (esInterzonal != 0)
                {
                    var resultadoUpdate = db.resultados.SingleOrDefault(x => x.id_resultado == partido.resultado.id_resultado);

                    if (resultadoUpdate.empate == 1 && resultadoUpdate.empate != partido.resultado.empate)
                    {
                        /* Habian empatado y ya no es un empate tengo que poner el nuevo ganador y el nuevo perdedor.*/

                        /* Tengo que sumarle 2 puntos al resultado.ganador.id_equipo y restarle 1 punto al
                          resultado.perdedor.id_perdedor perdedor*/


                        /*Dependiendo la fase voy a hacer lo descripto en una u otra tabla*/

                        if (id_fase == 1)
                        {
                            posiciones posicionesGanador = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.resultado.ganador.id_equipo
                                 && x.id_torneo == id_torneo);
                            posiciones posicionesPerdedor = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.resultado.perdedor.id_equipo
                            && x.id_torneo == id_torneo);

                            posicionesGanador.puntos = posicionesGanador.puntos + 2;
                            posicionesPerdedor.puntos = posicionesPerdedor.puntos - 1;
                            posicionesGanador.partidos_empatados = posicionesGanador.partidos_empatados - 1;
                            posicionesGanador.partidos_ganados = posicionesGanador.partidos_ganados + 1;
                            posicionesPerdedor.partidos_empatados = posicionesPerdedor.partidos_empatados - 1;
                            posicionesPerdedor.partidos_perdidos = posicionesPerdedor.partidos_perdidos + 1;

                            if (partido.resultado.ganador.id_equipo == partido.local.id_equipo)
                            {
                                posicionesGanador.goles_favor = posicionesGanador.goles_favor + lsGolesLocal.Count;
                                posicionesPerdedor.goles_favor = posicionesPerdedor.goles_favor + lsGolesVisitante.Count;
                                posicionesGanador.goles_contra = posicionesGanador.goles_contra + lsGolesVisitante.Count;
                                posicionesPerdedor.goles_contra = posicionesPerdedor.goles_contra + lsGolesLocal.Count;
                                posicionesGanador.dif_gol = posicionesGanador.goles_favor - posicionesGanador.goles_contra;
                                posicionesPerdedor.dif_gol = posicionesPerdedor.goles_favor - posicionesPerdedor.goles_contra;
                            }
                            else
                            {
                                posicionesGanador.goles_favor = posicionesGanador.goles_favor + lsGolesVisitante.Count;
                                posicionesPerdedor.goles_favor = posicionesPerdedor.goles_favor + lsGolesLocal.Count;
                                posicionesGanador.goles_contra = posicionesGanador.goles_contra + lsGolesLocal.Count;
                                posicionesPerdedor.goles_contra = posicionesPerdedor.goles_contra + lsGolesVisitante.Count;
                                posicionesGanador.dif_gol = posicionesGanador.goles_favor - posicionesGanador.goles_contra;
                                posicionesPerdedor.dif_gol = posicionesPerdedor.goles_favor - posicionesPerdedor.goles_contra;
                            }

                        }
                        else if (id_fase == 2)
                        {
                            posiciones_zona posicionesZonaGanador = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.resultado.ganador.id_equipo
                                    && x.id_torneo == id_torneo);
                            posiciones_zona posicionesZonaPerdedor = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.resultado.perdedor.id_equipo
                                  && x.id_torneo == id_torneo);

                            posicionesZonaGanador.puntos = posicionesZonaGanador.puntos + 2;
                            posicionesZonaPerdedor.puntos = posicionesZonaPerdedor.puntos - 1;

                            posicionesZonaGanador.partidos_empatados = posicionesZonaGanador.partidos_empatados - 1;
                            posicionesZonaGanador.partidos_ganados = posicionesZonaGanador.partidos_ganados + 1;
                            posicionesZonaPerdedor.partidos_empatados = posicionesZonaPerdedor.partidos_empatados - 1;
                            posicionesZonaPerdedor.partidos_perdidos = posicionesZonaPerdedor.partidos_perdidos + 1;


                            if (partido.resultado.ganador.id_equipo == partido.local.id_equipo)
                            {
                                posicionesZonaGanador.goles_favor = posicionesZonaGanador.goles_favor + lsGolesLocal.Count;
                                posicionesZonaPerdedor.goles_favor = posicionesZonaPerdedor.goles_favor + lsGolesVisitante.Count;
                                posicionesZonaGanador.goles_contra = posicionesZonaGanador.goles_contra + lsGolesVisitante.Count;
                                posicionesZonaPerdedor.goles_contra = posicionesZonaPerdedor.goles_contra + lsGolesLocal.Count;
                                posicionesZonaGanador.dif_gol = posicionesZonaGanador.goles_favor - posicionesZonaGanador.goles_contra;
                                posicionesZonaPerdedor.dif_gol = posicionesZonaPerdedor.goles_favor - posicionesZonaPerdedor.goles_contra;
                            }
                            else
                            {
                                posicionesZonaGanador.goles_favor = posicionesZonaGanador.goles_favor + lsGolesVisitante.Count;
                                posicionesZonaPerdedor.goles_favor = posicionesZonaPerdedor.goles_favor + lsGolesLocal.Count;
                                posicionesZonaGanador.goles_contra = posicionesZonaGanador.goles_contra + lsGolesLocal.Count;
                                posicionesZonaPerdedor.goles_contra = posicionesZonaPerdedor.goles_contra + lsGolesVisitante.Count;
                                posicionesZonaGanador.dif_gol = posicionesZonaGanador.goles_favor - posicionesZonaGanador.goles_contra;
                                posicionesZonaPerdedor.dif_gol = posicionesZonaPerdedor.goles_favor - posicionesZonaPerdedor.goles_contra;
                            }

                        }
                        else
                        {
                            //Aca iría el comportamiento para PlayOff.
                        }


                        /* Ya no es un empate, por tanto registro como ganador y perdedor a los nuevos equipos*/
                        resultadoUpdate.id_ganador = partido.resultado.ganador.id_equipo;
                        resultadoUpdate.id_perdedor = partido.resultado.perdedor.id_equipo;
                        resultadoUpdate.empate = null;

                        db.SaveChanges();
                    }
                    else if (resultadoUpdate.empate == null && resultadoUpdate.empate != partido.resultado.empate)
                    {
                        /* Tengo que buscar el que habia sumado 3 puntos que lo voy a sacar de 
                         resultadoUpdate.id_ganador, y a ese restarle 2 puntos. Por otro lado, al resultadoUpdate.id_perdedor
                         Tengo que sumarle 1 punto.
                         */

                        /*Dependiendo la fase voy a hacer lo descripto en una u otra tabla*/

                        if (id_fase == 1)
                        {
                            posiciones posicionesGanador = db.posiciones.SingleOrDefault(x => x.id_equipo == resultadoUpdate.id_ganador
                                 && x.id_torneo == id_torneo);
                            posiciones posicionesPerdedor = db.posiciones.SingleOrDefault(x => x.id_equipo == resultadoUpdate.id_perdedor
                            && x.id_torneo == id_torneo);

                            posicionesGanador.puntos = posicionesGanador.puntos - 2;
                            posicionesPerdedor.puntos = posicionesPerdedor.puntos + 1;
                            posicionesGanador.partidos_empatados = posicionesGanador.partidos_empatados + 1;
                            posicionesGanador.partidos_ganados = posicionesGanador.partidos_ganados - 1;
                            posicionesPerdedor.partidos_empatados = posicionesPerdedor.partidos_empatados + 1;
                            posicionesPerdedor.partidos_perdidos = posicionesPerdedor.partidos_perdidos - 1;

                            if (resultadoUpdate.id_ganador == partido.local.id_equipo)
                            {
                                posicionesGanador.goles_favor = posicionesGanador.goles_favor + lsGolesLocal.Count;
                                posicionesPerdedor.goles_favor = posicionesPerdedor.goles_favor + lsGolesVisitante.Count;
                                posicionesGanador.goles_contra = posicionesGanador.goles_contra + lsGolesVisitante.Count;
                                posicionesPerdedor.goles_contra = posicionesPerdedor.goles_contra + lsGolesLocal.Count;
                                posicionesGanador.dif_gol = posicionesGanador.goles_favor - posicionesGanador.goles_contra;
                                posicionesPerdedor.dif_gol = posicionesPerdedor.goles_favor - posicionesPerdedor.goles_contra;
                            }
                            else
                            {
                                posicionesGanador.goles_favor = posicionesGanador.goles_favor + lsGolesVisitante.Count;
                                posicionesPerdedor.goles_favor = posicionesPerdedor.goles_favor + lsGolesLocal.Count;
                                posicionesGanador.goles_contra = posicionesGanador.goles_contra + lsGolesLocal.Count;
                                posicionesPerdedor.goles_contra = posicionesPerdedor.goles_contra + lsGolesVisitante.Count;
                                posicionesGanador.dif_gol = posicionesGanador.goles_favor - posicionesGanador.goles_contra;
                                posicionesPerdedor.dif_gol = posicionesPerdedor.goles_favor - posicionesPerdedor.goles_contra;
                            }

                        }
                        else if (id_fase == 2)
                        {
                            posiciones_zona posicionesZonaGanador = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == resultadoUpdate.id_ganador
                                    && x.id_torneo == id_torneo);
                            posiciones_zona posicionesZonaPerdedor = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == resultadoUpdate.id_perdedor
                                  && x.id_torneo == id_torneo);

                            posicionesZonaGanador.puntos = posicionesZonaGanador.puntos - 2;
                            posicionesZonaPerdedor.puntos = posicionesZonaPerdedor.puntos + 1;
                            posicionesZonaGanador.partidos_empatados = posicionesZonaGanador.partidos_empatados + 1;
                            posicionesZonaGanador.partidos_ganados = posicionesZonaGanador.partidos_ganados - 1;
                            posicionesZonaPerdedor.partidos_empatados = posicionesZonaPerdedor.partidos_empatados + 1;
                            posicionesZonaPerdedor.partidos_perdidos = posicionesZonaPerdedor.partidos_perdidos - 1;

                            if (resultadoUpdate.id_ganador == partido.local.id_equipo)
                            {
                                posicionesZonaGanador.goles_favor = posicionesZonaGanador.goles_favor + lsGolesLocal.Count;
                                posicionesZonaPerdedor.goles_favor = posicionesZonaPerdedor.goles_favor + lsGolesVisitante.Count;
                                posicionesZonaGanador.goles_contra = posicionesZonaGanador.goles_contra + lsGolesVisitante.Count;
                                posicionesZonaPerdedor.goles_contra = posicionesZonaPerdedor.goles_contra + lsGolesLocal.Count;
                                posicionesZonaGanador.dif_gol = posicionesZonaGanador.goles_favor - posicionesZonaGanador.goles_contra;
                                posicionesZonaPerdedor.dif_gol = posicionesZonaPerdedor.goles_favor - posicionesZonaPerdedor.goles_contra;
                            }
                            else
                            {
                                posicionesZonaGanador.goles_favor = posicionesZonaGanador.goles_favor + lsGolesVisitante.Count;
                                posicionesZonaPerdedor.goles_favor = posicionesZonaPerdedor.goles_favor + lsGolesLocal.Count;
                                posicionesZonaGanador.goles_contra = posicionesZonaGanador.goles_contra + lsGolesLocal.Count;
                                posicionesZonaPerdedor.goles_contra = posicionesZonaPerdedor.goles_contra + lsGolesVisitante.Count;
                                posicionesZonaGanador.dif_gol = posicionesZonaGanador.goles_favor - posicionesZonaGanador.goles_contra;
                                posicionesZonaPerdedor.dif_gol = posicionesZonaPerdedor.goles_favor - posicionesZonaPerdedor.goles_contra;
                            }
                        }
                        else
                        {
                            //Aca iría el comportamiento para PlayOff.
                        }

                        /* No era un empate y ahora es un empate pongo el ganador, el perdedor y empate = 1 */
                        resultadoUpdate.id_ganador = partido.resultado.ganador.id_equipo;
                        resultadoUpdate.id_perdedor = partido.resultado.perdedor.id_equipo;
                        resultadoUpdate.empate = 1;

                        db.SaveChanges();
                    }
                    else
                    {
                        if (resultadoUpdate.id_ganador != partido.resultado.ganador.id_equipo)
                        {

                            /* Tengo que sumarle 3 puntos al nuevo ganador y restarle 3 puntos al nuevo perdedor.
                             El ganador anterior = nuevo perdedor lo saco de resultadoUpdate.id_ganador y el nuevo ganador
                              = anterior perdedor lo saco de resultadoUpdate.id_perdedor*/

                            /*Dependiendo la fase voy a hacer lo descripto en una u otra tabla*/

                            if (id_fase == 1)
                            {
                                posiciones posicionesGanador = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.resultado.ganador.id_equipo
                                     && x.id_torneo == id_torneo);
                                posiciones posicionesPerdedor = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.resultado.perdedor.id_equipo
                                && x.id_torneo == id_torneo);

                                posicionesGanador.puntos = posicionesGanador.puntos + 3;
                                posicionesPerdedor.puntos = posicionesPerdedor.puntos - 3;
                                posicionesGanador.partidos_ganados = posicionesGanador.partidos_ganados + 1;
                                posicionesGanador.partidos_perdidos = posicionesGanador.partidos_perdidos - 1;
                                posicionesPerdedor.partidos_ganados = posicionesPerdedor.partidos_ganados - 1;
                                posicionesPerdedor.partidos_perdidos = posicionesPerdedor.partidos_perdidos + 1;

                                if (partido.resultado.ganador.id_equipo == partido.local.id_equipo)
                                {
                                    posicionesGanador.goles_favor = posicionesGanador.goles_favor + lsGolesLocal.Count;
                                    posicionesPerdedor.goles_favor = posicionesPerdedor.goles_favor + lsGolesVisitante.Count;
                                    posicionesGanador.goles_contra = posicionesGanador.goles_contra + lsGolesVisitante.Count;
                                    posicionesPerdedor.goles_contra = posicionesPerdedor.goles_contra + lsGolesLocal.Count;
                                    posicionesGanador.dif_gol = posicionesGanador.goles_favor - posicionesGanador.goles_contra;
                                    posicionesPerdedor.dif_gol = posicionesPerdedor.goles_favor - posicionesPerdedor.goles_contra;
                                }
                                else
                                {
                                    posicionesGanador.goles_favor = posicionesGanador.goles_favor + lsGolesVisitante.Count;
                                    posicionesPerdedor.goles_favor = posicionesPerdedor.goles_favor + lsGolesLocal.Count;
                                    posicionesGanador.goles_contra = posicionesGanador.goles_contra + lsGolesLocal.Count;
                                    posicionesPerdedor.goles_contra = posicionesPerdedor.goles_contra + lsGolesVisitante.Count;
                                    posicionesGanador.dif_gol = posicionesGanador.goles_favor - posicionesGanador.goles_contra;
                                    posicionesPerdedor.dif_gol = posicionesPerdedor.goles_favor - posicionesPerdedor.goles_contra;
                                }

                            }
                            else if (id_fase == 2)
                            {
                                posiciones_zona posicionesZonaGanador = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.resultado.ganador.id_equipo
                                        && x.id_torneo == id_torneo);
                                posiciones_zona posicionesZonaPerdedor = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.resultado.perdedor.id_equipo
                                      && x.id_torneo == id_torneo);

                                posicionesZonaGanador.puntos = posicionesZonaGanador.puntos + 3;
                                posicionesZonaPerdedor.puntos = posicionesZonaPerdedor.puntos - 3;
                                posicionesZonaGanador.partidos_ganados = posicionesZonaGanador.partidos_ganados + 1;
                                posicionesZonaGanador.partidos_perdidos = posicionesZonaGanador.partidos_perdidos - 1;
                                posicionesZonaPerdedor.partidos_ganados = posicionesZonaPerdedor.partidos_ganados - 1;
                                posicionesZonaPerdedor.partidos_perdidos = posicionesZonaPerdedor.partidos_perdidos + 1;

                                if (partido.resultado.ganador.id_equipo == partido.local.id_equipo)
                                {
                                    posicionesZonaGanador.goles_favor = posicionesZonaGanador.goles_favor + lsGolesLocal.Count;
                                    posicionesZonaPerdedor.goles_favor = posicionesZonaPerdedor.goles_favor + lsGolesVisitante.Count;
                                    posicionesZonaGanador.goles_contra = posicionesZonaGanador.goles_contra + lsGolesVisitante.Count;
                                    posicionesZonaPerdedor.goles_contra = posicionesZonaPerdedor.goles_contra + lsGolesLocal.Count;
                                    posicionesZonaGanador.dif_gol = posicionesZonaGanador.goles_favor - posicionesZonaGanador.goles_contra;
                                    posicionesZonaPerdedor.dif_gol = posicionesZonaPerdedor.goles_favor - posicionesZonaPerdedor.goles_contra;
                                }
                                else
                                {
                                    posicionesZonaGanador.goles_favor = posicionesZonaGanador.goles_favor + lsGolesVisitante.Count;
                                    posicionesZonaPerdedor.goles_favor = posicionesZonaPerdedor.goles_favor + lsGolesLocal.Count;
                                    posicionesZonaGanador.goles_contra = posicionesZonaGanador.goles_contra + lsGolesLocal.Count;
                                    posicionesZonaPerdedor.goles_contra = posicionesZonaPerdedor.goles_contra + lsGolesVisitante.Count;
                                    posicionesZonaGanador.dif_gol = posicionesZonaGanador.goles_favor - posicionesZonaGanador.goles_contra;
                                    posicionesZonaPerdedor.dif_gol = posicionesZonaPerdedor.goles_favor - posicionesZonaPerdedor.goles_contra;
                                }
                            }
                            else
                            {
                                //Aca iría el comportamiento para PlayOff.
                            }


                            /* El ganador anterior es distinto del nuevo ganador por lo tanto
                             el perdedor anterior tambien es distinto con lo cual es indistinto chequear por esa condicion*/
                            resultadoUpdate.id_ganador = partido.resultado.ganador.id_equipo;
                            resultadoUpdate.id_perdedor = partido.resultado.perdedor.id_equipo;

                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    //No es interzonal
                    var resultadoUpdate = db.resultados_zona.SingleOrDefault(x => x.id_resultado == partido.resultado_zona.id_resultado);
                    if (resultadoUpdate.empate == 1 && resultadoUpdate.empate != partido.resultado_zona.empate)
                    {
                        /* Habian empatado y ya no es un empate tengo que poner el nuevo ganador y el nuevo perdedor.*/

                        /* Tengo que sumarle 2 puntos al resultado.ganador.id_equipo y restarle 1 punto al
                          resultado.perdedor.id_perdedor perdedor*/


                        /*Dependiendo la fase voy a hacer lo descripto en una u otra tabla*/

                        if (id_fase == 1)
                        {
                            posiciones posicionesGanador = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.resultado_zona.ganador.id_equipo
                                 && x.id_torneo == id_torneo);
                            posiciones posicionesPerdedor = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.resultado_zona.perdedor.id_equipo
                            && x.id_torneo == id_torneo);

                            posicionesGanador.puntos = posicionesGanador.puntos + 2;
                            posicionesPerdedor.puntos = posicionesPerdedor.puntos - 1;
                            posicionesGanador.partidos_empatados = posicionesGanador.partidos_empatados - 1;
                            posicionesGanador.partidos_ganados = posicionesGanador.partidos_ganados + 1;
                            posicionesPerdedor.partidos_empatados = posicionesPerdedor.partidos_empatados - 1;
                            posicionesPerdedor.partidos_perdidos = posicionesPerdedor.partidos_perdidos + 1;

                            if (partido.resultado_zona.ganador.id_equipo == partido.local.id_equipo)
                            {
                                posicionesGanador.goles_favor = posicionesGanador.goles_favor + lsGolesLocal.Count;
                                posicionesPerdedor.goles_favor = posicionesPerdedor.goles_favor + lsGolesVisitante.Count;
                                posicionesGanador.goles_contra = posicionesGanador.goles_contra + lsGolesVisitante.Count;
                                posicionesPerdedor.goles_contra = posicionesPerdedor.goles_contra + lsGolesLocal.Count;
                                posicionesGanador.dif_gol = posicionesGanador.goles_favor - posicionesGanador.goles_contra;
                                posicionesPerdedor.dif_gol = posicionesPerdedor.goles_favor - posicionesPerdedor.goles_contra;
                            }
                            else
                            {
                                posicionesGanador.goles_favor = posicionesGanador.goles_favor + lsGolesVisitante.Count;
                                posicionesPerdedor.goles_favor = posicionesPerdedor.goles_favor + lsGolesLocal.Count;
                                posicionesGanador.goles_contra = posicionesGanador.goles_contra + lsGolesLocal.Count;
                                posicionesPerdedor.goles_contra = posicionesPerdedor.goles_contra + lsGolesVisitante.Count;
                                posicionesGanador.dif_gol = posicionesGanador.goles_favor - posicionesGanador.goles_contra;
                                posicionesPerdedor.dif_gol = posicionesPerdedor.goles_favor - posicionesPerdedor.goles_contra;
                            }

                        }
                        else if (id_fase == 2)
                        {
                            posiciones_zona posicionesZonaGanador = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.resultado_zona.ganador.id_equipo
                                    && x.id_torneo == id_torneo);
                            posiciones_zona posicionesZonaPerdedor = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.resultado_zona.perdedor.id_equipo
                                  && x.id_torneo == id_torneo);

                            posicionesZonaGanador.puntos = posicionesZonaGanador.puntos + 2;
                            posicionesZonaPerdedor.puntos = posicionesZonaPerdedor.puntos - 1;

                            posicionesZonaGanador.partidos_empatados = posicionesZonaGanador.partidos_empatados - 1;
                            posicionesZonaGanador.partidos_ganados = posicionesZonaGanador.partidos_ganados + 1;
                            posicionesZonaPerdedor.partidos_empatados = posicionesZonaPerdedor.partidos_empatados - 1;
                            posicionesZonaPerdedor.partidos_perdidos = posicionesZonaPerdedor.partidos_perdidos + 1;


                            if (partido.resultado_zona.ganador.id_equipo == partido.local.id_equipo)
                            {
                                posicionesZonaGanador.goles_favor = posicionesZonaGanador.goles_favor + lsGolesLocal.Count;
                                posicionesZonaPerdedor.goles_favor = posicionesZonaPerdedor.goles_favor + lsGolesVisitante.Count;
                                posicionesZonaGanador.goles_contra = posicionesZonaGanador.goles_contra + lsGolesVisitante.Count;
                                posicionesZonaPerdedor.goles_contra = posicionesZonaPerdedor.goles_contra + lsGolesLocal.Count;
                                posicionesZonaGanador.dif_gol = posicionesZonaGanador.goles_favor - posicionesZonaGanador.goles_contra;
                                posicionesZonaPerdedor.dif_gol = posicionesZonaPerdedor.goles_favor - posicionesZonaPerdedor.goles_contra;
                            }
                            else
                            {
                                posicionesZonaGanador.goles_favor = posicionesZonaGanador.goles_favor + lsGolesVisitante.Count;
                                posicionesZonaPerdedor.goles_favor = posicionesZonaPerdedor.goles_favor + lsGolesLocal.Count;
                                posicionesZonaGanador.goles_contra = posicionesZonaGanador.goles_contra + lsGolesLocal.Count;
                                posicionesZonaPerdedor.goles_contra = posicionesZonaPerdedor.goles_contra + lsGolesVisitante.Count;
                                posicionesZonaGanador.dif_gol = posicionesZonaGanador.goles_favor - posicionesZonaGanador.goles_contra;
                                posicionesZonaPerdedor.dif_gol = posicionesZonaPerdedor.goles_favor - posicionesZonaPerdedor.goles_contra;
                            }
                        }
                        else
                        {
                            //Aca iría el comportamiento para PlayOff.
                        }


                        /* Ya no es un empate, por tanto registro como ganador y perdedor a los nuevos equipos*/
                        resultadoUpdate.id_ganador = partido.resultado_zona.ganador.id_equipo;
                        resultadoUpdate.id_perdedor = partido.resultado_zona.perdedor.id_equipo;
                        resultadoUpdate.empate = null;

                        db.SaveChanges();
                    }
                    else if (resultadoUpdate.empate == null && resultadoUpdate.empate != partido.resultado_zona.empate)
                    {
                        /* Tengo que buscar el que habia sumado 3 puntos que lo voy a sacar de 
                         resultadoUpdate.id_ganador, y a ese restarle 2 puntos. Por otro lado, al resultadoUpdate.id_perdedor
                         Tengo que sumarle 1 punto.
                         */

                        /*Dependiendo la fase voy a hacer lo descripto en una u otra tabla*/

                        if (id_fase == 1)
                        {
                            posiciones posicionesGanador = db.posiciones.SingleOrDefault(x => x.id_equipo == resultadoUpdate.id_ganador
                                 && x.id_torneo == id_torneo);
                            posiciones posicionesPerdedor = db.posiciones.SingleOrDefault(x => x.id_equipo == resultadoUpdate.id_perdedor
                            && x.id_torneo == id_torneo);

                            posicionesGanador.puntos = posicionesGanador.puntos - 2;
                            posicionesPerdedor.puntos = posicionesPerdedor.puntos + 1;
                            posicionesGanador.partidos_empatados = posicionesGanador.partidos_empatados + 1;
                            posicionesGanador.partidos_ganados = posicionesGanador.partidos_ganados - 1;
                            posicionesPerdedor.partidos_empatados = posicionesPerdedor.partidos_empatados + 1;
                            posicionesPerdedor.partidos_perdidos = posicionesPerdedor.partidos_perdidos - 1;

                            if (resultadoUpdate.id_ganador == partido.local.id_equipo)
                            {
                                posicionesGanador.goles_favor = posicionesGanador.goles_favor + lsGolesLocal.Count;
                                posicionesPerdedor.goles_favor = posicionesPerdedor.goles_favor + lsGolesVisitante.Count;
                                posicionesGanador.goles_contra = posicionesGanador.goles_contra + lsGolesVisitante.Count;
                                posicionesPerdedor.goles_contra = posicionesPerdedor.goles_contra + lsGolesLocal.Count;
                                posicionesGanador.dif_gol = posicionesGanador.goles_favor - posicionesGanador.goles_contra;
                                posicionesPerdedor.dif_gol = posicionesPerdedor.goles_favor - posicionesPerdedor.goles_contra;
                            }
                            else
                            {
                                posicionesGanador.goles_favor = posicionesGanador.goles_favor + lsGolesVisitante.Count;
                                posicionesPerdedor.goles_favor = posicionesPerdedor.goles_favor + lsGolesLocal.Count;
                                posicionesGanador.goles_contra = posicionesGanador.goles_contra + lsGolesLocal.Count;
                                posicionesPerdedor.goles_contra = posicionesPerdedor.goles_contra + lsGolesVisitante.Count;
                                posicionesGanador.dif_gol = posicionesGanador.goles_favor - posicionesGanador.goles_contra;
                                posicionesPerdedor.dif_gol = posicionesPerdedor.goles_favor - posicionesPerdedor.goles_contra;
                            }

                        }
                        else if (id_fase == 2)
                        {
                            posiciones_zona posicionesZonaGanador = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == resultadoUpdate.id_ganador
                                    && x.id_torneo == id_torneo);
                            posiciones_zona posicionesZonaPerdedor = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == resultadoUpdate.id_perdedor
                                  && x.id_torneo == id_torneo);

                            posicionesZonaGanador.puntos = posicionesZonaGanador.puntos - 2;
                            posicionesZonaPerdedor.puntos = posicionesZonaPerdedor.puntos + 1;
                            posicionesZonaGanador.partidos_empatados = posicionesZonaGanador.partidos_empatados + 1;
                            posicionesZonaGanador.partidos_ganados = posicionesZonaGanador.partidos_ganados - 1;
                            posicionesZonaPerdedor.partidos_empatados = posicionesZonaPerdedor.partidos_empatados + 1;
                            posicionesZonaPerdedor.partidos_perdidos = posicionesZonaPerdedor.partidos_perdidos - 1;


                            if (resultadoUpdate.id_ganador == partido.local.id_equipo)
                            {
                                posicionesZonaGanador.goles_favor = posicionesZonaGanador.goles_favor + lsGolesLocal.Count;
                                posicionesZonaPerdedor.goles_favor = posicionesZonaPerdedor.goles_favor + lsGolesVisitante.Count;
                                posicionesZonaGanador.goles_contra = posicionesZonaGanador.goles_contra + lsGolesVisitante.Count;
                                posicionesZonaPerdedor.goles_contra = posicionesZonaPerdedor.goles_contra + lsGolesLocal.Count;
                                posicionesZonaGanador.dif_gol = posicionesZonaGanador.goles_favor - posicionesZonaGanador.goles_contra;
                                posicionesZonaPerdedor.dif_gol = posicionesZonaPerdedor.goles_favor - posicionesZonaPerdedor.goles_contra;
                            }
                            else
                            {
                                posicionesZonaGanador.goles_favor = posicionesZonaGanador.goles_favor + lsGolesVisitante.Count;
                                posicionesZonaPerdedor.goles_favor = posicionesZonaPerdedor.goles_favor + lsGolesLocal.Count;
                                posicionesZonaGanador.goles_contra = posicionesZonaGanador.goles_contra + lsGolesLocal.Count;
                                posicionesZonaPerdedor.goles_contra = posicionesZonaPerdedor.goles_contra + lsGolesVisitante.Count;
                                posicionesZonaGanador.dif_gol = posicionesZonaGanador.goles_favor - posicionesZonaGanador.goles_contra;
                                posicionesZonaPerdedor.dif_gol = posicionesZonaPerdedor.goles_favor - posicionesZonaPerdedor.goles_contra;
                            }

                        }
                        else
                        {
                            //Aca iría el comportamiento para PlayOff.
                        }

                        /* No era un empate y ahora es un empate pongo el ganador, el perdedor y empate = 1 */
                        resultadoUpdate.id_ganador = partido.resultado_zona.ganador.id_equipo;
                        resultadoUpdate.id_perdedor = partido.resultado_zona.perdedor.id_equipo;
                        resultadoUpdate.empate = 1;

                        db.SaveChanges();
                    }
                    else
                    {
                        if (resultadoUpdate.id_ganador != partido.resultado_zona.ganador.id_equipo)
                        {

                            /* Tengo que sumarle 3 puntos al nuevo ganador y restarle 3 puntos al nuevo perdedor.
                             El ganador anterior = nuevo perdedor lo saco de resultadoUpdate.id_ganador y el nuevo ganador
                              = anterior perdedor lo saco de resultadoUpdate.id_perdedor*/

                            /*Dependiendo la fase voy a hacer lo descripto en una u otra tabla*/

                            if (id_fase == 1)
                            {
                                posiciones posicionesGanador = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.resultado_zona.ganador.id_equipo
                                     && x.id_torneo == id_torneo);
                                posiciones posicionesPerdedor = db.posiciones.SingleOrDefault(x => x.id_equipo == partido.resultado_zona.perdedor.id_equipo
                                && x.id_torneo == id_torneo);

                                posicionesGanador.puntos = posicionesGanador.puntos + 3;
                                posicionesPerdedor.puntos = posicionesPerdedor.puntos - 3;
                                posicionesGanador.partidos_ganados = posicionesGanador.partidos_ganados + 1;
                                posicionesGanador.partidos_perdidos = posicionesGanador.partidos_perdidos - 1;
                                posicionesPerdedor.partidos_ganados = posicionesPerdedor.partidos_ganados - 1;
                                posicionesPerdedor.partidos_perdidos = posicionesPerdedor.partidos_perdidos + 1;

                                if (partido.resultado_zona.ganador.id_equipo == partido.local.id_equipo)
                                {
                                    posicionesGanador.goles_favor = posicionesGanador.goles_favor + lsGolesLocal.Count;
                                    posicionesPerdedor.goles_favor = posicionesPerdedor.goles_favor + lsGolesVisitante.Count;
                                    posicionesGanador.goles_contra = posicionesGanador.goles_contra + lsGolesVisitante.Count;
                                    posicionesPerdedor.goles_contra = posicionesPerdedor.goles_contra + lsGolesLocal.Count;
                                    posicionesGanador.dif_gol = posicionesGanador.goles_favor - posicionesGanador.goles_contra;
                                    posicionesPerdedor.dif_gol = posicionesPerdedor.goles_favor - posicionesPerdedor.goles_contra;
                                }
                                else
                                {
                                    posicionesGanador.goles_favor = posicionesGanador.goles_favor + lsGolesVisitante.Count;
                                    posicionesPerdedor.goles_favor = posicionesPerdedor.goles_favor + lsGolesLocal.Count;
                                    posicionesGanador.goles_contra = posicionesGanador.goles_contra + lsGolesLocal.Count;
                                    posicionesPerdedor.goles_contra = posicionesPerdedor.goles_contra + lsGolesVisitante.Count;
                                    posicionesGanador.dif_gol = posicionesGanador.goles_favor - posicionesGanador.goles_contra;
                                    posicionesPerdedor.dif_gol = posicionesPerdedor.goles_favor - posicionesPerdedor.goles_contra;
                                }

                            }
                            else if (id_fase == 2)
                            {
                                posiciones_zona posicionesZonaGanador = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.resultado_zona.ganador.id_equipo
                                        && x.id_torneo == id_torneo);
                                posiciones_zona posicionesZonaPerdedor = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == partido.resultado_zona.perdedor.id_equipo
                                      && x.id_torneo == id_torneo);

                                posicionesZonaGanador.puntos = posicionesZonaGanador.puntos + 3;
                                posicionesZonaPerdedor.puntos = posicionesZonaPerdedor.puntos - 3;
                                posicionesZonaGanador.partidos_ganados = posicionesZonaGanador.partidos_ganados + 1;
                                posicionesZonaGanador.partidos_perdidos = posicionesZonaGanador.partidos_perdidos - 1;
                                posicionesZonaPerdedor.partidos_ganados = posicionesZonaPerdedor.partidos_ganados - 1;
                                posicionesZonaPerdedor.partidos_perdidos = posicionesZonaPerdedor.partidos_perdidos + 1;

                                if (partido.resultado_zona.ganador.id_equipo == partido.local.id_equipo)
                                {
                                    posicionesZonaGanador.goles_favor = posicionesZonaGanador.goles_favor + lsGolesLocal.Count;
                                    posicionesZonaPerdedor.goles_favor = posicionesZonaPerdedor.goles_favor + lsGolesVisitante.Count;
                                    posicionesZonaGanador.goles_contra = posicionesZonaGanador.goles_contra + lsGolesVisitante.Count;
                                    posicionesZonaPerdedor.goles_contra = posicionesZonaPerdedor.goles_contra + lsGolesLocal.Count;
                                    posicionesZonaGanador.dif_gol = posicionesZonaGanador.goles_favor - posicionesZonaGanador.goles_contra;
                                    posicionesZonaPerdedor.dif_gol = posicionesZonaPerdedor.goles_favor - posicionesZonaPerdedor.goles_contra;
                                }
                                else
                                {
                                    posicionesZonaGanador.goles_favor = posicionesZonaGanador.goles_favor + lsGolesVisitante.Count;
                                    posicionesZonaPerdedor.goles_favor = posicionesZonaPerdedor.goles_favor + lsGolesLocal.Count;
                                    posicionesZonaGanador.goles_contra = posicionesZonaGanador.goles_contra + lsGolesLocal.Count;
                                    posicionesZonaPerdedor.goles_contra = posicionesZonaPerdedor.goles_contra + lsGolesVisitante.Count;
                                    posicionesZonaGanador.dif_gol = posicionesZonaGanador.goles_favor - posicionesZonaGanador.goles_contra;
                                    posicionesZonaPerdedor.dif_gol = posicionesZonaPerdedor.goles_favor - posicionesZonaPerdedor.goles_contra;
                                }


                            }
                            else
                            {
                                //Aca iría el comportamiento para PlayOff.
                            }


                            /* El ganador anterior es distinto del nuevo ganador por lo tanto
                             el perdedor anterior tambien es distinto con lo cual es indistinto chequear por esa condicion*/
                            resultadoUpdate.id_ganador = partido.resultado_zona.ganador.id_equipo;
                            resultadoUpdate.id_perdedor = partido.resultado_zona.perdedor.id_equipo;
                            db.SaveChanges();
                        }
                    }
                }

                if (id_fase == 3)
                {
                    // Pendiente Play Off
                }

                return Ok();
            }
            catch (Exception e)
            {

                var logger = new Logger("PartidosController");
                logger.AgregarMensaje("api/partidos/modificar/" + " Parametros de entrada: " +
                JsonConvert.SerializeObject(partido, Formatting.None) + id_fase + id_torneo + esInterzonal, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }
        }
    }
}