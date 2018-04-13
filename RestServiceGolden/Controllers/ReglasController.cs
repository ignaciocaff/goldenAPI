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
    public class ReglasController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/reglas")]
        public IHttpActionResult GetAll()
        {
            List<Regla> lsReglas = new List<Regla>();
            try { 

            var reglas = db.reglas_torneo.ToList();

            foreach (var r in reglas)
            {
                Regla regla = new Regla();
                regla.descripcion = r.descripcion;
                regla.id_regla = r.id_regla;
                lsReglas.Add(regla);
            }

            return Ok(lsReglas);
            } catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/reglamento/registrar")]
        public IHttpActionResult registrar([FromBody]Reglamento reglamento)
        {
            try { 
                var reglamentos= db.reglamentos.Where(x => x.id_torneo == reglamento.id_torneo).FirstOrDefault();

                if (reglamentos != null)
                {
                    return BadRequest("Ya existe un reglamento para este torneo");
                }
                else
                {
                    reglamentos regla = new reglamentos();
                    regla.descripcion = reglamento.descripcion;
                    regla.id_torneo = reglamento.id_torneo;

                    db.reglamentos.Add(regla);
                    db.SaveChanges();
                    return Ok();
                }
            } catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/reglamento/{id}")]
        public IHttpActionResult getReglamento(int id)
        {
            try
            {
                var reglamentos = db.reglamentos.Where(x => x.id_torneo == id).FirstOrDefault();

                if (reglamentos != null)
                {
                    Reglamento reglamento = new Reglamento();
                    reglamento.id_reglamento = reglamentos.id_reglamento;
                    reglamento.descripcion = reglamentos.descripcion;
                    reglamento.id_torneo = (int)reglamentos.id_torneo;
                    return Ok(reglamento);
                }
                else
                {
                    return BadRequest("No existe un reglamento con ese id.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        [Route("api/reglamento/update")]
        public IHttpActionResult updateReglamento([FromBody]Reglamento reglamento)
        {
            reglamentos reglamentoDto = new reglamentos();

            try
            {
                reglamentoDto.id_reglamento = (int) reglamento.id_reglamento;
                reglamentoDto.descripcion = reglamento.descripcion;
                reglamentoDto.id_torneo = reglamento.id_torneo;

                var result = db.reglamentos.SingleOrDefault(x => x.id_reglamento == reglamento.id_reglamento);

                if (result != null)
                {
                    result.id_reglamento = reglamentoDto.id_reglamento;
                    result.descripcion = reglamentoDto.descripcion;
                    result.id_torneo = reglamentoDto.id_torneo;
                    db.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}