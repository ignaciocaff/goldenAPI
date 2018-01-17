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
    public class TipoTorneoController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/torneo/tipoTorneo")]
        public IHttpActionResult GetAll()
        {
            List<TipoTorneo> lsTiposTorneo = new List<TipoTorneo>();

            var tiposTorneo = db.tipos_torneos.ToList();

            foreach (var tTorneo in tiposTorneo)
            {
                TipoTorneo tipoTorneo = new TipoTorneo();
                tipoTorneo.descripcion = tTorneo.descripcion;
                tipoTorneo.id_tipo = tTorneo.id_tipo;
                lsTiposTorneo.Add(tipoTorneo);
            }

            return Ok(lsTiposTorneo);
        }
    }
}