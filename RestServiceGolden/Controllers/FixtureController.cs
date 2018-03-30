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
        [Route("api/fecha/registrar")]
        public IHttpActionResult registrar([FromBody]List<Partido> partidos)
        {
            foreach (Partido p in partidos)
            {

            }
            return Ok();
        }
    }
}