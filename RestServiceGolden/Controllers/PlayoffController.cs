using RestServiceGolden.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestServiceGolden.Controllers
{
    public class PlayoffController : ApiController
    {
        goldenEntities db = new goldenEntities();
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/etapas/todas")]
        public IHttpActionResult getEtapasTodas()
        {
            List<Etapa> lsEtapas = new List<Etapa>();

            var etapas = db.etapa_playoff.ToList();

            foreach (var e in etapas)
            {
                Etapa etapa = new Etapa();
                etapa.descripcion = e.descripcion;
                etapa.id_etapa = e.id_etapa;
                lsEtapas.Add(etapa);
            }

            return Ok(lsEtapas);
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/llaves/todas")]
        public IHttpActionResult getLlavesTodas()
        {
            List<Llave> lsLlaves = new List<Llave>();

            var llaves = db.llaves.ToList();

            foreach (var l in llaves)
            {
                Llave llave = new Llave();
                llave.descripcion = l.descripcion;
                llave.id_llave = l.id_llave;
                lsLlaves.Add(llave);
            }

            return Ok(lsLlaves);
        }
    }
}