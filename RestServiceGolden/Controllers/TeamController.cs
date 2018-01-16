using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Cors;
using RestServiceGolden.Models;

namespace RestServiceGolden.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TeamController: ApiController
    {
        [ResponseType(typeof(Equipo))]
        [Route("api/users/authenticate")]
    }
}