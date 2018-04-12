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
    public class ConfigurationController: ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/reglasTorneo/registrar")]
        public IHttpActionResult registrar([FromBody]ReglaTorneo regla)
        {
            reglas_torneo reglaDto = new reglas_torneo();
                        
            try
            {
                reglaDto.descripcion = regla.descripcion;
                reglaDto.id_torneo = regla.torneo.id_torneo;

                db.reglas_torneo.Add(reglaDto);
                db.SaveChanges();
                return Ok(true);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/reglasTorneo")]
        public IHttpActionResult GetAll()
        {
            List<ReglaTorneo> lsReglas = new List<ReglaTorneo>();

            try
            {
                var reglas = db.reglas_torneo.ToList();

                foreach (var r in reglas)
                {
                    var tor = db.torneos.Where(x => x.id_torneo == r.id_torneo).FirstOrDefault();
                    ReglaTorneo regla = new ReglaTorneo();
                    Torneo torneo = new Torneo();
                    regla.id_regla = r.id_regla;
                    regla.descripcion = r.descripcion;
                    regla.torneo = torneo;
                    regla.torneo.id_torneo = (int)tor.id_torneo;
                    regla.torneo.nombre = tor.nombre;
                    lsReglas.Add(regla);
                }
                return Ok(lsReglas);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [Route("api/reglasTorneo/update")]
        public IHttpActionResult update([FromBody]ReglaTorneo regla)
        {
            reglas_torneo reglaDto = new reglas_torneo();

            try
            {
                reglaDto.id_regla = (int)regla.id_regla;
                reglaDto.descripcion = regla.descripcion;
                reglaDto.id_torneo = (int)regla.torneo.id_torneo;

                reglas_torneo reglas = db.reglas_torneo.Where(x => x.id_regla == regla.id_regla).FirstOrDefault();

                if(reglas != null)
                {
                    reglas.id_regla = reglaDto.id_regla;
                    reglas.descripcion = reglaDto.descripcion;
                    reglas.id_torneo = reglaDto.id_torneo;
                    db.SaveChanges();
                    return Ok();
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