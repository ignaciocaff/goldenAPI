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
    public class ClubController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/club/todos")]
        public IHttpActionResult GetAll()
        {
            List<Club> lsClubes = new List<Club>();

            var clubes = db.clubes.ToList();

            foreach (var c in clubes)
            {
                Club club= new Club();
                club.id_club = c.id_club;
                club.nombre = c.nombre;
                club.descripcion = c.descripcion;
                lsClubes.Add(club);
            }

            return Ok(lsClubes);
        }
    }
}