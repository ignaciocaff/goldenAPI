using RestServiceGolden.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace RestServiceGolden.Utilidades
{
    public class FixtureAutomatico
    {
        public Fixture listadoPartidos(List<Equipo> lsquipos, int cantidadEquipos, ParametrosFixture parametros)
        {
            goldenEntities db = new goldenEntities();
            List<turnos_fixture> turnos = db.turnos_fixture.ToList();

            if (lsquipos.Count % 2 != 0)
            {
                Equipo e = new Equipo();
                e.id_equipo = -1;
                lsquipos.Add(e);
            }

            int cantidadDias = (cantidadEquipos - 1);

            int mitadTamaño = cantidadEquipos / 2;

            if (parametros.tipoDeFixture.id_tipo == 2)
            {
                mitadTamaño = lsquipos.Count / 2;
            }

            List<Equipo> equipos = new List<Equipo>();

            equipos.AddRange(lsquipos); // Copiamos todos los elementos
            equipos.RemoveAt(0); // Excluimos el primer equipo

            int equiposTamaño = equipos.Count;
            Fixture fixture = new Fixture();
            List<Fecha> lsFechas = new List<Fecha>();
            for (int dia = 0; dia < cantidadDias; dia++)
            {
                Fecha fecha = new Fecha();
                fecha.partidos = new List<Partido>();
                fecha.fecha = parametros.fechaInicioFixture.AddDays(parametros.cantidadDiasEntrePartidos * dia);
                int equipoIndice = dia % equiposTamaño;


                Partido partido = new Partido();
                partido.local = equipos[equipoIndice];
                partido.visitante = lsquipos[0];
                asignacionTurnos(turnos, partido, fecha);
                fecha.partidos.Add(partido);

                for (int i = 1; i < mitadTamaño; i++)
                {
                    Partido p2 = new Partido();
                    int primerEquipo = (dia + i) % equiposTamaño;
                    int segundoEquipo = (dia + equiposTamaño - i) % equiposTamaño;
                    p2.local = equipos[primerEquipo];
                    p2.visitante = equipos[segundoEquipo];
                    asignacionTurnos(turnos, p2, fecha);
                    fecha.partidos.Add(p2);
                }

                lsFechas.Add(fecha);
            }
            fixture.fechas = lsFechas;
            completarDatosParaVisualizacion(fixture);
            return fixture;
        }

        public Partido asignacionTurnos(List<turnos_fixture> turnos, Partido partido, Fecha fecha)
        {
            goldenEntities db = new goldenEntities();


            HorarioFijo horario = new HorarioFijo();
            Cancha cancha = new Cancha();
            Random random = new Random();
            int turno = 0;
            fechas comprobacion;
            do
            {
                turno = random.Next(0, turnos.Count);
                comprobacion = (from fechaDto in db.fechas
                                from partidoDto in db.partidos
                                where fechaDto.fecha == fecha.fecha && partidoDto.id_horario_fijo == horario.id_horario
                                && partidoDto.id_cancha == cancha.id_cancha
                                select fechaDto).SingleOrDefault();
                if (comprobacion != null)
                {
                    turnos.RemoveAt(turno);
                }
            } while (comprobacion != null);

            cancha.id_cancha = (int)turnos[turno].id_cancha;
            horario.id_horario = turnos[turno].id_horario;
            turnos.RemoveAt(turno);

            return partido;
        }

        public Fixture generarFixtureAutomatico(ParametrosFixture parametros)
        {
            goldenEntities db = new goldenEntities();
            Fixture fixture;
            List<Equipo> listadoEquipos = new List<Equipo>();
            switch (parametros.tipoDeFixture.id_tipo)
            {
                case 1:
                    //Fixture zona con equipos par o todos contra todos.
                    //fixture = generarFixturePorZona(parametros);
                    //persistenciaDatosZonas(fixture, parametros);
                    listadoEquipos = generarEquiposPorZonaYTorneo(parametros);
                    fixture = listadoPartidos(listadoEquipos, listadoEquipos.Count, parametros);
                    break;
                case 2:
                    //Fixture completo con interzonal, numero de zonas par, numero de equipos impar
                    var zonas = db.zonas.Where(x => x.id_torneo == parametros.id_torneo).ToList();

                    Fixture fixtureDto = new Fixture();
                    List<Fecha> lsFechas = new List<Fecha>();
                    fixtureDto.fechas = lsFechas;
                    foreach (var zona in zonas)
                    {
                        parametros.zona.id_zona = zona.id_zona;
                        listadoEquipos = generarEquiposPorZonaYTorneo(parametros);

                        foreach (var fecha in listadoPartidos(listadoEquipos, listadoEquipos.Count, parametros).fechas)
                        {

                            int? indice = lsFechas.FindIndex(x => x.fecha == fecha.fecha);
                            if (indice != -1)
                            {
                                lsFechas.ElementAt((int)indice).partidos.AddRange(fecha.partidos);
                                lsFechas.ElementAt((int)indice).iPartidos.AddRange(fecha.iPartidos);
                            }
                            else
                            {
                                lsFechas.Add(fecha);
                            }
                        }
                    }

                    foreach (var fechaInterzonal in lsFechas)
                    {
                        List<Equipo> equiposInterzonales = new List<Equipo>();
                        List<IEquipo> iEquiposInterzonales = new List<IEquipo>();
                        Partido partidoInterzonal = new Partido();
                        IPartido iPartidoInterzonal = new IPartido();

                        for (var i = fechaInterzonal.partidos.Count - 1; i >= 0; i--)
                        {
                            if (fechaInterzonal.partidos[i].local.id_equipo == -1)
                            {
                                equiposInterzonales.Add(fechaInterzonal.partidos[i].visitante);
                                iEquiposInterzonales.Add(fechaInterzonal.iPartidos[i].visitante[0]);
                                partidoInterzonal.cancha = fechaInterzonal.partidos[i].cancha;
                                partidoInterzonal.horario_fijo = fechaInterzonal.partidos[i].horario_fijo;
                                iPartidoInterzonal.cancha = fechaInterzonal.iPartidos[i].cancha;
                                iPartidoInterzonal.horario = fechaInterzonal.iPartidos[i].horario;
                                fechaInterzonal.partidos.RemoveAt(i);
                                fechaInterzonal.iPartidos.RemoveAt(i);
                            }
                            else if (fechaInterzonal.partidos[i].visitante.id_equipo == -1)
                            {
                                equiposInterzonales.Add(fechaInterzonal.partidos[i].local);
                                iEquiposInterzonales.Add(fechaInterzonal.iPartidos[i].local[0]);
                                partidoInterzonal.cancha = fechaInterzonal.partidos[i].cancha;
                                partidoInterzonal.horario_fijo = fechaInterzonal.partidos[i].horario_fijo;
                                iPartidoInterzonal.cancha = fechaInterzonal.iPartidos[i].cancha;
                                iPartidoInterzonal.horario = fechaInterzonal.iPartidos[i].horario;
                                fechaInterzonal.partidos.RemoveAt(i);
                                fechaInterzonal.iPartidos.RemoveAt(i);
                            }
                        }
                        Random randomEquiposInterzonales = new Random();

                        equiposInterzonales = equiposInterzonales.OrderBy(a => randomEquiposInterzonales.Next()).ToList();

                        for (var j = 0; j < equiposInterzonales.Count / 2; j += 2)
                        {
                            List<IEquipo> lsLocal = new List<IEquipo>();
                            List<IEquipo> lsVisitante = new List<IEquipo>();
                            iPartidoInterzonal.local = lsLocal;
                            iPartidoInterzonal.visitante = lsVisitante;
                            partidoInterzonal.local = equiposInterzonales[j];
                            partidoInterzonal.visitante = equiposInterzonales[j + 1];
                            iPartidoInterzonal.local.Add(iEquiposInterzonales.Find(x => x.id_equipo == equiposInterzonales[j].id_equipo));
                            iPartidoInterzonal.visitante.Add(iEquiposInterzonales.Find(x => x.id_equipo == equiposInterzonales[j + 1].id_equipo));
                            fechaInterzonal.partidos.Add(partidoInterzonal);
                            fechaInterzonal.iPartidos.Add(iPartidoInterzonal);
                        }
                    }

                    fixtureDto.fechas = lsFechas;
                    fixture = fixtureDto;

                    //persistenciaDatos(fixture, parametros);
                    break;
                case 3:
                    //Fixture completo sin interzonal, numero de zonas par o impar, numero de equipos par
                    var zonasTipo3 = db.zonas.Where(x => x.id_torneo == parametros.id_torneo).ToList();

                    Fixture fixtureDtoTipo3 = new Fixture();
                    List<Fecha> lsFechasTipo3 = new List<Fecha>();
                    fixtureDtoTipo3.fechas = lsFechasTipo3;
                    foreach (var zona in zonasTipo3)
                    {
                        parametros.zona.id_zona = zona.id_zona;
                        listadoEquipos = generarEquiposPorZonaYTorneo(parametros);

                        foreach (var fecha in listadoPartidos(listadoEquipos, listadoEquipos.Count, parametros).fechas)
                        {

                            int? indice = lsFechasTipo3.FindIndex(x => x.fecha == fecha.fecha);
                            if (indice != -1)
                            {
                                lsFechasTipo3.ElementAt((int)indice).partidos.AddRange(fecha.partidos);
                                lsFechasTipo3.ElementAt((int)indice).iPartidos.AddRange(fecha.iPartidos);
                            }
                            else
                            {
                                lsFechasTipo3.Add(fecha);
                            }
                        }
                    }
                    fixtureDtoTipo3.fechas = lsFechasTipo3;
                    fixture = fixtureDtoTipo3;
                    break;
                default:
                    fixture = null;
                    break;
            }

            return fixture;
        }

        public Fixture generarFixtureTorneoCompleto(ParametrosFixture parametros)
        {
            goldenEntities db = new goldenEntities();

            List<Equipo> lsEquipos = generarEquiposPorTorneo(parametros) != null ? generarEquiposPorTorneo(parametros) : null;
            int cantidadFechas = lsEquipos.Count;
            List<Fecha> listadoFechasFixture = new List<Fecha>();
            EstadoFecha estadoFecha = new EstadoFecha();
            Fixture fixture = new Fixture();
            Fecha fechaInicio = new Fecha();
            fechaInicio.fecha = parametros.fechaInicioFixture;
            fechaInicio.estado = estadoFecha;
            fechaInicio.estado.id_estado = 1;
            listadoFechasFixture.Add(fechaInicio);

            if (parametros.esInterzonal)
            {
                foreach (Equipo e in lsEquipos)
                {
                    cantidadFechas = db.equipos_zona.Where(x => x.id_zona == e.id_zona).ToList().Count;
                    break;
                }

                for (var i = 1; i < cantidadFechas; i++)
                {
                    List<Partido> listadoPartidos = new List<Partido>();
                    Fecha nuevaFecha = new Fecha();
                    nuevaFecha.estado = estadoFecha;
                    nuevaFecha.estado.id_estado = 1;
                    nuevaFecha.fecha = listadoFechasFixture[i - 1].fecha.AddDays(parametros.cantidadDiasEntrePartidos);
                    nuevaFecha.partidos = listadoPartidos;
                    listadoFechasFixture.Add(nuevaFecha);
                }

                fixture.fechas = listadoFechasFixture;
                combinacionEquipos(fixture, parametros);
                completarDatosParaVisualizacion(fixture);
            }
            else
            {
                for (var i = 1; i < cantidadFechas - 1; i++)
                {
                    List<Partido> listadoPartidos = new List<Partido>();
                    Fecha nuevaFecha = new Fecha();
                    nuevaFecha.estado = estadoFecha;
                    nuevaFecha.estado.id_estado = 1;
                    nuevaFecha.fecha = listadoFechasFixture[i - 1].fecha.AddDays(parametros.cantidadDiasEntrePartidos);
                    nuevaFecha.partidos = listadoPartidos;
                    listadoFechasFixture.Add(nuevaFecha);
                }

                fixture.fechas = listadoFechasFixture;
                combinacionEquipos(fixture, parametros);
                completarDatosParaVisualizacion(fixture);
            }

            return fixture;
        }

        public Fixture generarFixturePorZona(ParametrosFixture parametros)
        {
            int cantidadFechas = generarEquiposPorZonaYTorneo(parametros) != null ? generarEquiposPorZonaYTorneo(parametros).Count : 0;
            List<Fecha> listadoFechasFixture = new List<Fecha>();
            EstadoFecha estadoFecha = new EstadoFecha();
            Fixture fixture = new Fixture();
            Fecha fechaInicio = new Fecha();
            fechaInicio.fecha = parametros.fechaInicioFixture;
            fechaInicio.estado = estadoFecha;
            fechaInicio.estado.id_estado = 1;
            listadoFechasFixture.Add(fechaInicio);

            for (var i = 1; i < cantidadFechas - 1; i++)
            {
                List<Partido> listadoPartidos = new List<Partido>();
                Fecha nuevaFecha = new Fecha();
                nuevaFecha.estado = estadoFecha;
                nuevaFecha.estado.id_estado = 1;
                nuevaFecha.fecha = listadoFechasFixture[i - 1].fecha.AddDays(parametros.cantidadDiasEntrePartidos);
                nuevaFecha.partidos = listadoPartidos;
                listadoFechasFixture.Add(nuevaFecha);
            }

            fixture.fechas = listadoFechasFixture;

            combinacionEquipos(fixture, parametros);
            completarDatosParaVisualizacion(fixture);
            return fixture;
        }

        public Fixture completarDatosParaVisualizacion(Fixture fixture)
        {
            goldenEntities db = new goldenEntities();
            var horarios = db.horarios_fijos.ToList();
            var canchas = db.canchas.ToList();
            var archivos = db.files.ToList();
            foreach (var fecha in fixture.fechas)
            {
                List<IPartido> lsPartidos = new List<IPartido>();

                foreach (var partido in fecha.partidos)
                {
                    IPartido interfazPartido = new IPartido();
                    IEquipo interfazLocal = new IEquipo();
                    IEquipo interfazVisitante = new IEquipo();
                    HorarioFijo horarioFijo = new HorarioFijo();
                    Cancha cancha = new Cancha();
                    List<IEquipo> listaLocal = new List<IEquipo>();
                    List<IEquipo> listaVisitante = new List<IEquipo>();

                    interfazPartido.cancha = cancha;
                    interfazPartido.cancha.id_cancha = partido.cancha.id_cancha;
                    interfazPartido.cancha.nombre = canchas.Where(x => x.id_cancha == interfazPartido.cancha.id_cancha).SingleOrDefault().nombre;
                    interfazPartido.horario = horarioFijo;
                    interfazPartido.horario.id_horario = partido.horario_fijo.id_horario;
                    interfazPartido.horario.inicio = horarios.Where(x => x.id_horario == interfazPartido.horario.id_horario).SingleOrDefault().inicio;
                    interfazPartido.horario.fin = horarios.Where(x => x.id_horario == interfazPartido.horario.id_horario).SingleOrDefault().fin;
                    interfazLocal.id_equipo = partido.local.id_equipo;
                    interfazLocal.nombre = partido.local.nombre;
                    interfazLocal.imagePath = partido.local.camisetalogo != 0 ? archivos.Where(x => x.Id == partido.local.camisetalogo).SingleOrDefault().ImagePath : null;
                    interfazVisitante.id_equipo = partido.visitante.id_equipo;
                    interfazVisitante.nombre = partido.visitante.nombre;
                    interfazVisitante.imagePath = partido.visitante.camisetalogo != 0 ? archivos.Where(x => x.Id == partido.visitante.camisetalogo).SingleOrDefault().ImagePath : null;
                    interfazPartido.local = listaLocal;
                    interfazPartido.visitante = listaVisitante;
                    interfazPartido.local.Add(interfazLocal);
                    interfazPartido.visitante.Add(interfazVisitante);

                    lsPartidos.Add(interfazPartido);
                }
                fecha.iPartidos = lsPartidos;
            }
            return fixture;
        }

        public Fixture combinacionEquipos(Fixture fixture, ParametrosFixture parametros)
        {
            goldenEntities db = new goldenEntities();

            List<Equipo> equiposFixture = (parametros.zona != null && parametros.zona.id_zona != null) ? generarEquiposPorZonaYTorneo(parametros) : generarEquiposPorTorneo(parametros);
            int cantidadPartidosPorFecha = equiposFixture.Count / 2;
            List<Equipo> lsLocales = equiposFixture.GetRange(0, cantidadPartidosPorFecha).ToList();
            List<Equipo> lsVisitantes = equiposFixture.GetRange(cantidadPartidosPorFecha, equiposFixture.Count - cantidadPartidosPorFecha).ToList();
            List<Partido> cruces = new List<Partido>();
            Random randomLocal = new Random();
            Random randomVisitante = new Random();

            lsLocales = lsLocales.OrderBy(a => randomLocal.Next()).ToList();
            lsVisitantes = lsVisitantes.OrderBy(a => randomVisitante.Next()).ToList();

            foreach (var local in lsLocales)
            {
                foreach (var visitante in lsVisitantes)
                {
                    Partido partido = new Partido();
                    partido.local = local;
                    partido.visitante = visitante;

                    if (local.id_zona != visitante.id_zona)
                    {
                        partido.esInterzonal = true;
                    }
                    cruces.Add(partido);
                }
            }

            for (var l = 0; l < lsLocales.Count; l++)
            {
                for (var k = l; k < lsLocales.Count; k++)
                {
                    if (l != k)
                    {
                        Partido partido = new Partido();
                        partido.local = lsLocales[l];
                        partido.visitante = lsLocales[k];
                        if (lsLocales[l].id_zona != lsLocales[k].id_zona)
                        {
                            partido.esInterzonal = true;
                        }
                        cruces.Add(partido);
                    }
                }
            }
            for (var l = 0; l < lsVisitantes.Count; l++)
            {
                for (var k = l; k < lsVisitantes.Count; k++)
                {
                    if (l != k)
                    {
                        Partido partido = new Partido();
                        partido.local = lsVisitantes[l];
                        partido.visitante = lsVisitantes[k];

                        if (lsVisitantes[l].id_zona != lsVisitantes[k].id_zona)
                        {
                            partido.esInterzonal = true;
                        }
                        cruces.Add(partido);
                    }
                }
            }
            String lms = "";
            foreach (var cruce in cruces)
            {

                lms = lms + ("Local: " + cruce.local.id_equipo.ToString() + " Zona: " + cruce.local.id_zona.ToString() + " vs " + " Visitante: " + cruce.visitante.id_equipo.ToString() + " Zona: " + cruce.visitante.id_zona.ToString()) + "\n";
            }
            //Para fixture con interzonal
            if (parametros.esInterzonal)
            {
                String cls = "";

                var crucesDefinitivos = cruces.GroupBy(x => x.esInterzonal).Select(grp => grp.ToList()).ToList();
                var crucesZonales = crucesDefinitivos[1];
                var crucesInterzonales = crucesDefinitivos[0];


                for (var i = 0; i < fixture.fechas.Count; i++)
                {
                    List<Partido> partidosFecha = new List<Partido>();
                    List<turnos_fixture> turnos = db.turnos_fixture.ToList();


                    for (var j = crucesZonales.Count - 1; j >= 0; j--)
                    {

                        if (partidosFecha.Where(x => (x.local.id_equipo == crucesZonales[j].local.id_equipo
                        || x.visitante.id_equipo == crucesZonales[j].local.id_equipo)
                        || (x.local.id_equipo == crucesZonales[j].visitante.id_equipo
                        || x.visitante.id_equipo == crucesZonales[j].visitante.id_equipo)).ToList().Count > 0)
                        {

                        }
                        else
                        {
                            HorarioFijo horario = new HorarioFijo();
                            Cancha cancha = new Cancha();
                            Random random = new Random();

                            int turno = random.Next(0, turnos.Count);
                            cancha.id_cancha = (int)turnos[turno].id_cancha;
                            horario.id_horario = turnos[turno].id_horario;

                            DateTime fechaActual = fixture.fechas[i].fecha;

                            var comprobacionFecha = db.fechas.Where(x => x.fecha == fechaActual).ToList();

                            foreach (var cFecha in comprobacionFecha)
                            {
                                while (db.partidos.Where(x => x.id_cancha == cancha.id_cancha
                                && x.id_horario_fijo == horario.id_horario && x.id_fecha == cFecha.id_fecha).SingleOrDefault() != null)
                                {
                                    turno = random.Next(0, turnos.Count);
                                    cancha.id_cancha = (int)turnos[turno].id_cancha;
                                    horario.id_horario = turnos[turno].id_horario;
                                }
                            }

                            crucesZonales[j].horario_fijo = horario;
                            crucesZonales[j].cancha = cancha;
                            turnos.RemoveAt(turno);
                            partidosFecha.Add(crucesZonales[j]);
                            crucesZonales.RemoveAt(j);
                        }
                    }

                    int cantidadBorrada = 0;
                    for (var k = crucesInterzonales.Count - 1; k >= 0; k -= cantidadBorrada)
                    {

                        if (partidosFecha.Where(x => (x.local.id_equipo == crucesInterzonales[k].local.id_equipo
                         || x.visitante.id_equipo == crucesInterzonales[k].local.id_equipo)
                         || (x.local.id_equipo == crucesInterzonales[k].visitante.id_equipo
                         || x.visitante.id_equipo == crucesInterzonales[k].visitante.id_equipo)).ToList().Count > 0)
                        {
                            cantidadBorrada = 1;
                        }
                        else
                        {
                            HorarioFijo horario = new HorarioFijo();
                            Cancha cancha = new Cancha();
                            Random random = new Random();

                            int turno = random.Next(0, turnos.Count);
                            cancha.id_cancha = (int)turnos[turno].id_cancha;
                            horario.id_horario = turnos[turno].id_horario;

                            DateTime fechaActual = fixture.fechas[i].fecha;

                            var comprobacionFecha = db.fechas.Where(x => x.fecha == fechaActual).ToList();

                            foreach (var cFecha in comprobacionFecha)
                            {
                                while (db.partidos.Where(x => x.id_cancha == cancha.id_cancha
                                && x.id_horario_fijo == horario.id_horario && x.id_fecha == cFecha.id_fecha).SingleOrDefault() != null)
                                {
                                    turno = random.Next(0, turnos.Count);
                                    cancha.id_cancha = (int)turnos[turno].id_cancha;
                                    horario.id_horario = turnos[turno].id_horario;
                                }
                            }

                            crucesInterzonales[k].horario_fijo = horario;
                            crucesInterzonales[k].cancha = cancha;
                            turnos.RemoveAt(turno);

                            partidosFecha.Add(crucesInterzonales[k]);
                            cantidadBorrada = crucesInterzonales.RemoveAll(x => (x.local.id_equipo == crucesInterzonales[k].local.id_equipo
                        || x.visitante.id_equipo == crucesInterzonales[k].local.id_equipo)
                        || (x.local.id_equipo == crucesInterzonales[k].visitante.id_equipo
                        || x.visitante.id_equipo == crucesInterzonales[k].visitante.id_equipo));


                        }
                    }



                    fixture.fechas[i].partidos = partidosFecha;
                    cls = cls + fixture.fechas[i].fecha + "\n";
                    foreach (var c in partidosFecha)
                    {
                        if (c.local.id_zona != c.visitante.id_zona)
                        {
                            cls = cls + ("Local: " + c.local.id_equipo.ToString() + " Zona: " + c.local.id_zona.ToString() + " vs " + " Visitante: " + c.visitante.id_equipo.ToString() + " Zona: " + c.visitante.id_zona.ToString()) + " PARTIDO INTERZONAL" + "\n";
                        }
                        else
                        {
                            cls = cls + ("Local: " + c.local.id_equipo.ToString() + " Zona: " + c.local.id_zona.ToString() + " vs " + " Visitante: " + c.visitante.id_equipo.ToString() + " Zona: " + c.visitante.id_zona.ToString()) + "\n";
                        }
                    }
                }
                return fixture;
            }

            //Para fixture sin Interzonal
            for (var i = 0; i < fixture.fechas.Count; i++)
            {
                List<Partido> partidosFecha = new List<Partido>();
                List<turnos_fixture> turnos = db.turnos_fixture.ToList();
                for (var j = cruces.Count - 1; j >= 0; j--)
                {
                    if (partidosFecha.Where(x => (x.local.id_equipo == cruces[j].local.id_equipo
                        || x.visitante.id_equipo == cruces[j].local.id_equipo)
                        || (x.local.id_equipo == cruces[j].visitante.id_equipo
                        || x.visitante.id_equipo == cruces[j].visitante.id_equipo)).ToList().Count > 0)
                    {

                    }
                    else
                    {
                        HorarioFijo horario = new HorarioFijo();
                        Cancha cancha = new Cancha();
                        Random random = new Random();

                        int turno = random.Next(0, turnos.Count);
                        cancha.id_cancha = (int)turnos[turno].id_cancha;
                        horario.id_horario = turnos[turno].id_horario;

                        DateTime fechaActual = fixture.fechas[i].fecha;

                        var comprobacionFecha = db.fechas.Where(x => x.fecha == fechaActual).ToList();

                        foreach (var cFecha in comprobacionFecha)
                        {
                            while (db.partidos.Where(x => x.id_cancha == cancha.id_cancha
                            && x.id_horario_fijo == horario.id_horario && x.id_fecha == cFecha.id_fecha).SingleOrDefault() != null)
                            {
                                turno = random.Next(0, turnos.Count);
                                cancha.id_cancha = (int)turnos[turno].id_cancha;
                                horario.id_horario = turnos[turno].id_horario;
                            }
                        }

                        cruces[j].horario_fijo = horario;
                        cruces[j].cancha = cancha;
                        turnos.RemoveAt(turno);
                        partidosFecha.Add(cruces[j]);
                        cruces.RemoveAt(j);
                    }
                }

                fixture.fechas[i].partidos = partidosFecha;
            }



            return fixture;
        }

        public List<Equipo> generarEquiposPorZonaYTorneo(ParametrosFixture parametros)
        {
            goldenEntities db = new goldenEntities();


            var listadoCompletoEquipos = (from tEquipo in db.equipos
                                          join tEquipoZona in db.equipos_zona on tEquipo.id_equipo equals tEquipoZona.id_equipo
                                          where tEquipoZona.id_zona == parametros.zona.id_zona && tEquipoZona.id_torneo == parametros.id_torneo
                                          select new
                                          {
                                              id_equipo = tEquipo.id_equipo,
                                              nombre = tEquipo.nombre,
                                              id_zona = tEquipoZona.id_zona,
                                              camisetalogo = tEquipo.camisetalogo
                                          }).ToList();

            List<Equipo> listadoInterfazEquipo = new List<Equipo>();


            foreach (var equipo in listadoCompletoEquipos)
            {
                Equipo equipoDto = new Equipo();

                equipoDto.id_equipo = equipo.id_equipo;
                equipoDto.nombre = equipo.nombre;
                equipoDto.camisetalogo = (int)equipo.camisetalogo;
                equipoDto.id_zona = (int)equipo.id_zona;

                listadoInterfazEquipo.Add(equipoDto);
            }
            return listadoInterfazEquipo;
        }

        public List<Equipo> generarEquiposPorTorneo(ParametrosFixture parametros)
        {
            goldenEntities db = new goldenEntities();


            var listadoCompletoEquipos = (from tEquipo in db.equipos
                                          where tEquipo.id_torneo == parametros.id_torneo
                                          select new
                                          {
                                              id_equipo = tEquipo.id_equipo,
                                              nombre = tEquipo.nombre,
                                              camisetalogo = tEquipo.camisetalogo
                                          }).ToList();

            List<Equipo> listadoInterfazEquipo = new List<Equipo>();


            foreach (var equipo in listadoCompletoEquipos)
            {
                Equipo equipoDto = new Equipo();
                var zona = db.equipos_zona.SingleOrDefault(x => x.id_equipo == equipo.id_equipo);
                if (zona != null)
                {
                    equipoDto.id_zona = (int)zona.id_zona;
                }
                equipoDto.camisetalogo = (int)equipo.camisetalogo;
                equipoDto.id_equipo = equipo.id_equipo;
                equipoDto.nombre = equipo.nombre;

                listadoInterfazEquipo.Add(equipoDto);
            }
            return listadoInterfazEquipo;
        }

        public bool persistenciaDatos(Fixture fixture, ParametrosFixture parametros)
        {
            try
            {
                goldenEntities db = new goldenEntities();
                var existeFixture = db.fixture_zona.Where(x => x.id_torneo == parametros.id_torneo).FirstOrDefault();

                if (existeFixture == null)
                {
                    fixture_zona fixtureDto = new fixture_zona();
                    fixtureDto.id_tipo = parametros.tipoDeFixture.id_tipo;
                    fixtureDto.id_torneo = parametros.id_torneo;
                    db.fixture_zona.Add(fixtureDto);

                    int id_fixture = fixtureDto.id_fixture;


                    foreach (var fecha in fixture.fechas)
                    {
                        fechas fechaDto = new fechas();
                        fechaDto.fecha = fecha.fecha;
                        fechaDto.id_estado = fecha.estado.id_estado;
                        fechaDto.id_fixture_zona = id_fixture;
                        fechaDto.id_fase = parametros.id_fase;

                        db.fechas.Add(fechaDto);
                        db.SaveChanges();
                        int id_fecha = fechaDto.id_fecha;

                        foreach (var partido in fecha.partidos)
                        {
                            partidos partidoDto = new partidos();
                            partidoDto.local = partido.local.id_equipo;
                            partidoDto.visitante = partido.visitante.id_equipo;
                            partidoDto.id_fecha = id_fecha;
                            partidoDto.id_estado_partido = 1;
                            partidoDto.id_cancha = partido.cancha.id_cancha;
                            partidoDto.id_horario_fijo = partido.horario_fijo.id_horario;

                            db.partidos.Add(partidoDto);
                            db.SaveChanges();
                        }
                    }
                    return true;
                }
                else
                {
                    throw new Exception("Fixture existente para ese torneo.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Ocurrió un error. Intente nuevamente más tarde.");
            }
        }

        public bool persistenciaDatosZonas(Fixture fixture, ParametrosFixture parametros)
        {
            try
            {
                goldenEntities db = new goldenEntities();
                int id_fixture = db.fixture_zona.Where(x => x.id_torneo == parametros.id_torneo && x.id_zona == parametros.zona.id_zona).FirstOrDefault().id_fixture;
                var existenFechasFixtureZona = db.fechas.Where(x => x.id_fixture_zona == id_fixture).ToList();
                if (existenFechasFixtureZona.Count == 0)
                {
                    foreach (var fecha in fixture.fechas)
                    {
                        fechas fechaDto = new fechas();
                        fechaDto.fecha = fecha.fecha;
                        fechaDto.id_estado = fecha.estado.id_estado;
                        fechaDto.id_fixture_zona = id_fixture;
                        fechaDto.id_fase = parametros.id_fase;

                        db.fechas.Add(fechaDto);
                        db.SaveChanges();
                        int id_fecha = fechaDto.id_fecha;

                        foreach (var partido in fecha.partidos)
                        {
                            partidos partidoDto = new partidos();
                            partidoDto.local = partido.local.id_equipo;
                            partidoDto.visitante = partido.visitante.id_equipo;
                            partidoDto.id_fecha = id_fecha;
                            partidoDto.id_estado_partido = 1;
                            partidoDto.id_cancha = partido.cancha.id_cancha;
                            partidoDto.id_horario_fijo = partido.horario_fijo.id_horario;

                            db.partidos.Add(partidoDto);
                            db.SaveChanges();
                        }
                    }
                    return true;
                }
                else
                {
                    throw new Exception("Fixture existente para ese torneo.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Ocurrió un error. Intente nuevamente más tarde.");
            }
        }
    }
}