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
    public class ReglasController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/reglas")]
        public IHttpActionResult GetAll()
        {
            List<Regla> lsReglas = new List<Regla>();

            var reglas = db.reglas_torneo.ToList();

            foreach (var r in reglas)
            {
                Regla regla = new Regla();
                regla.descripcion = r.descripcion;
                regla.id_regla = r.id_regla;
                lsReglas.Add(regla);
            }

            return Ok(lsReglas);
        }
    }
}