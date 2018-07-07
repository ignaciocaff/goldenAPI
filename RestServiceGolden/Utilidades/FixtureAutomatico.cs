using RestServiceGolden.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Utilidades
{
    public class FixtureAutomatico
    {
        public Fixture generarFixtureAutomatico(ParametrosFixture parametros)
        {

            Fixture fixture;
            switch (parametros.tipoDeFixture)
            {
                case 1:
                    fixture = generarFixtureTorneoCompleto(parametros);
                    break;
                case 2:
                    fixture = generarFixturePorZona(parametros);
                    break;
                default:
                    fixture = null;
                    break;
            }

            return fixture;
        }

        public Fixture generarFixtureTorneoCompleto(ParametrosFixture parametros)
        {

            List<Fecha> listadoFechasFixture = new List<Fecha>();
            Fixture fixture = new Fixture();
            Fecha fechaInicio = new Fecha();
            fechaInicio.fecha = parametros.fechaInicioFixture;
            listadoFechasFixture.Add(fechaInicio);

            for (var i = 1; i < parametros.cantidadFechas; i++)
            {
                Fecha nuevaFecha = new Fecha();
                nuevaFecha.fecha = listadoFechasFixture[i - 1].fecha.AddDays(parametros.cantidadDiasEntrePartidos);
                listadoFechasFixture.Add(nuevaFecha);
            }

            fixture.fechas = listadoFechasFixture;

            combinacionEquipos(fixture, parametros);

            return fixture;
        }


        public Fixture combinacionEquipos(Fixture fixture, ParametrosFixture parametros)
        {
            List<Equipo> equiposFixture = generarEquiposPorTorneo(parametros);
            int cantidadPartidosPorFecha = equiposFixture.Count / 2;


            foreach (Fecha fecha in fixture.fechas)
            {
                fecha.partidos = new List<Partido>();
                List<Equipo> lsLocales = equiposFixture.GetRange(0, cantidadPartidosPorFecha).ToList();
                List<Equipo> lsVisitantes = equiposFixture.GetRange(cantidadPartidosPorFecha, equiposFixture.Count - cantidadPartidosPorFecha).ToList();

                for (var i = cantidadPartidosPorFecha - 1; i >= 0; i--)
                {
                    if (fixture.fechas.First().fecha == fecha.fecha)
                    {
                        Partido partidoDto = new Partido();
                        partidoDto.local = lsLocales[i];
                        partidoDto.visitante = lsVisitantes[i];
                        fecha.partidos.Add(partidoDto);
                        lsLocales.Remove(lsLocales[i]);
                        lsVisitantes.Remove(lsVisitantes[i]);
                    }
                    else
                    {
                        //Aca comienzo a mezclar los equipos a partir de la fecha 2.
                        foreach (var f in fixture.fechas)
                        {
                            f.partidos.SingleOrDefault(y => (y.local.id_equipo == lsLocales[i].id_equipo
                                                && y.visitante.id_equipo == lsVisitantes[i].id_equipo)
                                                || (y.visitante.id_equipo == lsVisitantes[i].id_equipo
                                                && y.local.id_equipo == lsLocales[i].id_equipo));
  
                        }
                    }

                }
            }

            return fixture;
        }

        public Fixture generarFixturePorZona(ParametrosFixture parametros)
        {
            return new Fixture();
        }

        public List<Equipo> generarEquiposPorZonaYTorneo(ParametrosFixture parametros)
        {
            goldenEntities db = new goldenEntities();


            var listadoCompletoEquipos = (from tEquipo in db.equipos
                                          join tEquipoZona in db.equipos_zona on tEquipo.id_equipo equals tEquipoZona.id_equipo
                                          where tEquipoZona.id_zona == parametros.id_zona && tEquipoZona.id_torneo == parametros.id_torneo
                                          select new
                                          {
                                              id_equipo = tEquipo.id_equipo,
                                              nombre = tEquipo.nombre,
                                          }).ToList();

            List<Equipo> listadoInterfazEquipo = new List<Equipo>();


            foreach (var equipo in listadoCompletoEquipos)
            {
                Equipo equipoDto = new Equipo();

                equipoDto.id_equipo = equipo.id_equipo;
                equipoDto.nombre = equipo.nombre;
                listadoInterfazEquipo.Add(equipoDto);
            }
            return listadoInterfazEquipo;
        }


        public List<Equipo> generarEquiposPorTorneo(ParametrosFixture parametros)
        {
            goldenEntities db = new goldenEntities();


            var listadoCompletoEquipos = (from tEquipo in db.equipos
                                          join tEquipoZona in db.equipos_zona on tEquipo.id_equipo equals tEquipoZona.id_equipo
                                          where tEquipoZona.id_torneo == parametros.id_torneo
                                          select new
                                          {
                                              id_equipo = tEquipo.id_equipo,
                                              nombre = tEquipo.nombre,
                                          }).ToList();

            List<Equipo> listadoInterfazEquipo = new List<Equipo>();


            foreach (var equipo in listadoCompletoEquipos)
            {
                Equipo equipoDto = new Equipo();

                equipoDto.id_equipo = equipo.id_equipo;
                equipoDto.nombre = equipo.nombre;
                listadoInterfazEquipo.Add(equipoDto);
            }
            return listadoInterfazEquipo;
        }
    }
}