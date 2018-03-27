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
    public class HorariosController : ApiController
    {
        goldenEntities db = new goldenEntities();
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/horario/turnos")]
        public IHttpActionResult getAll()
        {
            List<Turno> lsTurnos = new List<Turno>();

            var turnos = db.turnos.ToList();

            foreach (var t in turnos)
            {
                Turno turno = new Turno();
                turno.descripcion = t.descripcion;
                turno.id = t.id;
                lsTurnos.Add(turno);
            }

            return Ok(lsTurnos);
        }
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/horarios")]
        public IHttpActionResult getHorarios()
        {
            List<HorarioFijo> lsHorarios = new List<HorarioFijo>();

            var horarios = db.horarios_fijos.ToList();

            foreach (var h in horarios)
            {
                turnos turnoDto = db.turnos.Where(x => x.id == h.id_turno).SingleOrDefault();
                HorarioFijo horario = new HorarioFijo();
                Turno turno = new Turno();

                horario.id_horario = h.id_horario;
                horario.inicio = h.inicio;
                horario.fin = h.fin;
                horario.turno = turno;
                horario.turno.id = turnoDto.id;
                horario.turno.descripcion = turnoDto.descripcion;
                lsHorarios.Add(horario);
            }

            return Ok(lsHorarios);
        }


        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/horario/registrar")]
        public IHttpActionResult registrar([FromBody]HorarioFijo horario)
        {
            horarios_fijos horarioDto = new horarios_fijos();

            try
            {
                horarioDto.inicio = horario.inicio;
                horarioDto.fin = horario.fin;
                horarioDto.id_turno = (int)horario.turno.id;

                int horarioCheck = db.horarios_fijos.Where(x => x.id_turno == horarioDto.id_turno
                && x.inicio == horarioDto.inicio
                && x.fin == horarioDto.fin).Count();

                if (horarioCheck == 0)
                {
                    // Si no existe el horario exacto en ese turno, lo doy de alta
                    db.horarios_fijos.Add(horarioDto);
                    db.SaveChanges();
                    return Ok(true);
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/horario/update")]
        public IHttpActionResult update([FromBody]HorarioFijo horario)
        {
            horarios_fijos horarioDto = new horarios_fijos();

            try
            {
                horarioDto.inicio = horario.inicio;
                horarioDto.fin = horario.fin;
                horarioDto.id_turno = (int)horario.turno.id;
                horarioDto.id_horario = (int)horario.id_horario;

                int horarioCheck = db.horarios_fijos.Where(x => x.id_turno == horarioDto.id_turno
                && x.inicio == horarioDto.inicio
                && x.fin == horarioDto.fin).Count();

                if (horarioCheck == 0)
                {
                    var horarioUpdate = db.horarios_fijos.SingleOrDefault(b => b.id_horario == horarioDto.id_horario);
                    // Si no existe el horario exacto en ese turno, lo traigo y lo modifico
                    if (horarioUpdate != null)
                    {
                        horarioUpdate.inicio = horarioDto.inicio;
                        horarioUpdate.fin = horarioDto.fin;
                        horarioUpdate.id_turno = horarioDto.id_turno;
                        db.SaveChanges();
                    }
                    return Ok(true);
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