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
    public class FixtureAutomaticoController : ApiController
    {
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fixtureAutomatico/torneo")]
        public IHttpActionResult registrar([FromBody]ParametrosFixture parametros)
        {
            FixtureAutomatico service = new FixtureAutomatico();
            Fixture fixture = service.generarFixtureAutomatico(parametros);
            return Ok(fixture);
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fixtureAutomatico/generarTurnos")]
        public IHttpActionResult getGenerarTurnos()
        {
            goldenEntities db = new goldenEntities();
            try
            {
                var canchas = db.canchas.ToList();
                var horarios = db.horarios_fijos.ToList();

                foreach(var cancha in canchas)
                {
                    foreach(var horario in horarios)
                    {
                        var turnoFixture = new turnos_fixture();
                        turnoFixture.id_cancha = cancha.id_cancha;
                        turnoFixture.id_horario = horario.id_horario;
                        db.turnos_fixture.Add(turnoFixture);
                        db.SaveChanges();
                    }
                }
                return Ok();
            }catch(Exception e)
            {
                return BadRequest();
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/fixtureAutomatico/obtenerTipos")]
        public IHttpActionResult getObtenerTiposFixture()
        {
            goldenEntities db = new goldenEntities();
            try
            {
                var lsTiposFixture = db.tipos_fixture.ToList();
                List<TipoFixture> lsTiposFixtureDto = new List<TipoFixture>();
                foreach (var tipo in lsTiposFixture)
                {
                    TipoFixture tipoDto = new TipoFixture();
                    tipoDto.id_tipo = tipo.id_tipo;
                    tipoDto.descripcion = tipo.descripcion;
                    lsTiposFixtureDto.Add(tipoDto);
                }
                return Ok(lsTiposFixtureDto);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}
