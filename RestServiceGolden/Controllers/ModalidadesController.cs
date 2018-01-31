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
    public class ModalidadesController : ApiController
    {
        goldenEntities db = new goldenEntities();
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/modalidades")]
        public IHttpActionResult GetAll()
        {
            List<Modalidad> lsModalidades = new List<Modalidad>();

            var modalidades = db.modalidades.ToList();

            foreach (var m in modalidades)
            {
                Modalidad modalidad = new Modalidad();
                modalidad.descripcion = m.descripcion;
                modalidad.id_modalidad = m.id_modalidad;
                lsModalidades.Add(modalidad);
            }

            return Ok(lsModalidades);
        }
    }
}