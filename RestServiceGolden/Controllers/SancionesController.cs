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
    public class SancionesController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/sanciones/tipos")]
        public IHttpActionResult GetAllTipos()
        {
            List<TipoSancion> lsTipoSanciones = new List<TipoSancion>();
            try
            {
                var tipos_sanciones = db.tipos_sanciones.ToList().OrderBy(z => z.id_tipo);

                foreach (var tSancion in tipos_sanciones)
                {
                    TipoSancion tipo_sancion = new TipoSancion(); ;
                    tipo_sancion.id_tipo = tSancion.id_tipo;
                    tipo_sancion.descripcion = tSancion.descripcion;
                    lsTipoSanciones.Add(tipo_sancion);
                }
                return Ok(lsTipoSanciones);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}