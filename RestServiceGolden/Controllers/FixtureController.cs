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
        [Route("api/fecha/registrar/{id_zona}")]
        public IHttpActionResult registrar([FromBody]List<Partido> partidos, int id_zona)
        {
            try
            {
                int id_fixture_zona = db.fixture_zona.SingleOrDefault(x => x.id_zona == id_zona).id_fixture;

                if (id_fixture_zona != 0)
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

                }
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
            return Ok();
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


    }
}