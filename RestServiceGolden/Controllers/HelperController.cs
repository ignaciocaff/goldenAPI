using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestServiceGolden.Controllers
{
    public class HelperController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/helperTablas")]
        public IHttpActionResult GetAll()
        {
            var posiciones = db.posiciones_zona.Where(x => x.id_torneo == 27 && x.id_zona == 19).ToList().OrderBy(x => x.id_equipo);


            foreach (var posicion in posiciones)
            {
                var puntos = 0;
                var partidos_jugados = 0;
                var partidos_ganados = 0;
                var partidos_empatados = 0;
                var partidos_perdidos = 0;
                var goles_favor = 0;
                var listaGolesFavor = new List<goles>();
                var goles_contra = 0;
                var dif_gol = 0;
                var listaCompletaGolesEnContra = new List<goles>();
                var lsPartidos = new List<partidos>();

                /* var resultados = db.resultados.Where(x => x.id_ganador == posicion.id_equipo ||
                 x.id_perdedor == posicion.id_equipo).ToList();*/

                var fixture_zona = db.fixture_zona.Where(x => x.id_torneo == 27 && x.id_zona == 19).SingleOrDefault();

                var fechas = db.fechas.Where(x => x.id_fixture_zona == fixture_zona.id_fixture).ToList();
                foreach (var f in fechas)
                {
                    var lsPartidoDe = db.partidos.Where(x => x.id_fecha == f.id_fecha).ToList();
                    foreach (var p in lsPartidoDe)
                    {
                        lsPartidos.Add(p);
                    }
                }

                /*var resultados_zona = db.resultados_zona.Where(x => (x.id_ganador == posicion.id_equipo ||
                                x.id_perdedor == posicion.id_equipo) && x.id_zona == 19).ToList();*/

                foreach (var p in lsPartidos)
                {
                    var listadoGolesContra = db.goles.Where(x => x.id_partido == p.id_partido && x.id_equipo != posicion.id_equipo).ToList();
                    var listGolesFavor = db.goles.Where(x => x.id_partido == p.id_partido && x.id_equipo == posicion.id_equipo).ToList();
                    foreach (var golContra in listadoGolesContra)
                    {
                        listaCompletaGolesEnContra.Add(golContra);
                    }

                    foreach (var golAFavor in listGolesFavor)
                    {
                        listaGolesFavor.Add(golAFavor);
                    }
                }

                if (listaCompletaGolesEnContra.Count > 0)
                {
                    goles_contra = listaCompletaGolesEnContra.Count;
                }

                if (listaGolesFavor != null)
                {
                    goles_favor = listaGolesFavor.Count;
                }

                /* foreach (var resultado in resultados)
                 {
                     // Significa que empato ese partido
                     if ((posicion.id_equipo == resultado.id_ganador || posicion.id_equipo == resultado.id_perdedor)
                         && resultado.empate == 1)
                     {
                         puntos = puntos + 1;
                         partidos_jugados = partidos_jugados + 1;
                         partidos_empatados = partidos_empatados + 1;

                     }
                     else if (posicion.id_equipo == resultado.id_ganador)
                     {
                         //Gano el partido
                         puntos = puntos + 3;
                         partidos_jugados = partidos_jugados + 1;
                         partidos_ganados = partidos_ganados + 1;
                     }
                     else
                     {
                         //Perdio el partid
                         partidos_jugados = partidos_jugados + 1;
                         partidos_perdidos = partidos_perdidos + 1;
                     }

                 }*/

                /*foreach (var resultado_zona in resultados_zona)
                {
                    // Significa que empato ese partido
                    if ((posicion.id_equipo == resultado_zona.id_ganador || posicion.id_equipo == resultado_zona.id_perdedor)
                        && resultado_zona.empate == 1)
                    {
                        puntos = puntos + 1;
                        partidos_jugados = partidos_jugados + 1;
                        partidos_empatados = partidos_empatados + 1;

                    }
                    else if (posicion.id_equipo == resultado_zona.id_ganador)
                    {
                        //Gano el partido
                        puntos = puntos + 3;
                        partidos_jugados = partidos_jugados + 1;
                        partidos_ganados = partidos_ganados + 1;
                    }
                    else
                    {
                        //Perdio el partid
                        partidos_jugados = partidos_jugados + 1;
                        partidos_perdidos = partidos_perdidos + 1;
                    }
                }*/
                dif_gol = goles_favor - goles_contra;

                Console.Write(posicion.id_equipo + puntos + partidos_jugados + partidos_ganados + partidos_perdidos + partidos_empatados + goles_favor + goles_contra + dif_gol);

                posiciones_zona posicionParaActualizar = db.posiciones_zona.SingleOrDefault(x => x.id_equipo == posicion.id_equipo
                    && x.id_torneo == 27 && x.id_zona == 19);

                posicionParaActualizar.puntos = puntos;
                posicionParaActualizar.partidos_jugados = partidos_jugados;
                posicionParaActualizar.partidos_ganados = partidos_ganados;
                posicionParaActualizar.partidos_empatados = partidos_empatados;
                posicionParaActualizar.partidos_perdidos = partidos_perdidos;
                posicionParaActualizar.goles_favor = goles_favor;
                posicionParaActualizar.goles_contra = goles_contra;
                posicionParaActualizar.dif_gol = dif_gol;
                db.SaveChanges();
            }

            return Ok();
        }


        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/camelCase")]
        public IHttpActionResult getCamelCase()
        {

            var jugadores = db.jugadores.ToList();

            foreach (var jug in jugadores)
            {
                var jugadorUpdate = db.personas.SingleOrDefault(x => x.id_persona == jug.id_persona);

                var nombre = jugadorUpdate.nombre;
                var apellido = jugadorUpdate.apellido;
                for (var i = 0; i < nombre.Length; i++)
                {
                    if (i == 0)
                    {
                        nombre = nombre[0].ToString().ToUpper() + nombre.Substring(1).ToLower();
                    }

                    if (Char.IsLetter(nombre[i]))
                    {
                        if (i != 0)
                        {
                            if (Char.IsWhiteSpace(nombre[i - 1]))
                            {
                                nombre = nombre.Substring(0, i - 1) + ' ' + nombre[i].ToString().ToUpper() + nombre.Substring(i + 1, (nombre.Length - (i + 1))).ToLower();
                            }
                        }
                    }

                }

                for (var i = 0; i < apellido.Length; i++)
                {
                    if (i == 0)
                    {
                        apellido = apellido[0].ToString().ToUpper() + apellido.Substring(1).ToLower();
                    }

                    if (Char.IsLetter(apellido[i]))
                    {
                        if (i != 0)
                        {
                            if (Char.IsWhiteSpace(apellido[i - 1]))
                            {
                                apellido = apellido.Substring(0, i - 1) + ' ' + apellido[i].ToString().ToUpper() + apellido.Substring(i + 1, (apellido.Length - (i + 1))).ToLower();
                            }
                        }
                    }

                }

                jugadorUpdate.nombre = nombre;
                jugadorUpdate.apellido = apellido;
                db.SaveChanges();
            }

            return Ok();
        }
    }
}