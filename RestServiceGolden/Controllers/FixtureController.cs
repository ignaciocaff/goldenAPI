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
                if (id_fixture_zona != 0 && fechaCheck == null)
                {
                    fechas fecha = new fechas();
                    fecha.fecha = partidos.FirstOrDefault().fecha.fecha;
                    fecha.id_estado = 1;
                    fecha.id_fixture_zona = id_fixture_zona;
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
        [Route("api/fecha/obtener/{id_zona}/{id_torneo}")]
        public IHttpActionResult obtener([FromBody]Fecha fecha, int id_zona, int id_torneo)
        {
            List<IPartido> lsPartidos = new List<IPartido>();

            try
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

                            var objLocal = (from tEquipos in db.equipos
                                            join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                            where tEquipos.id_equipo == partido.local
                                            select new
                                            {
                                                id_equipo = tEquipos.id_equipo,
                                                nombre = tEquipos.nombre,
                                                imagePath = tArchivos.ImagePath,
                                                logo = tEquipos.logo
                                            }).SingleOrDefault();

                            var objVisitante = (from tEquipos in db.equipos
                                                join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                where tEquipos.id_equipo == partido.visitante
                                                select new
                                                {
                                                    id_equipo = tEquipos.id_equipo,
                                                    nombre = tEquipos.nombre,
                                                    imagePath = tArchivos.ImagePath,
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
                            lsPartidos.Add(iPartido);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
            return Ok(lsPartidos);
        }
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fecha/obtenerPartidos")]
        public IHttpActionResult obtenerPartidos([FromBody]IPartido iPartido)
        {
            try
            {
                var fechas = db.fechas.Where(x => x.fecha == iPartido.fecha).ToList();
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
                                                        imagePath = tArchivos.ImagePath,
                                                        logo = tEquipos.logo
                                                    }).SingleOrDefault();

                                    var objVisitante = (from tEquipos in db.equipos
                                                        join tArchivos in db.files on tEquipos.logo equals tArchivos.Id
                                                        where tEquipos.id_equipo == partido.visitante
                                                        select new
                                                        {
                                                            id_equipo = tEquipos.id_equipo,
                                                            nombre = tEquipos.nombre,
                                                            imagePath = tArchivos.ImagePath,
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
                return BadRequest(e.ToString());
            }
        }
    }
}