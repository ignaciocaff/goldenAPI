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
    public class CategoriasController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/categorias")]
        public IHttpActionResult GetAll()
        {
            List<Categoria> lsCategorias = new List<Categoria>();

            var categorias = db.categorias.ToList();

            foreach (var c in categorias)
            {
                Categoria categoria = new Categoria();
                categoria.descripcion = c.descripcion;
                categoria.id_categoria = c.id_categoria;
                lsCategorias.Add(categoria);
            }

            return Ok(lsCategorias);
        }
        //Categorias de equipos(GOLDEN, MASTER, ETC)
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/categorias_equipos")]
        public IHttpActionResult GetCatEquipos()
        {
            List<Torneo> lsTorneos = new List<Torneo>();

            var categorias = db.torneos.ToList();

            foreach (var c in categorias)
            {
                Torneo torneo = new Torneo();
                torneo.descripcion = c.descripcion;
                torneo.id_torneo = c.id_torneo;
                lsTorneos.Add(torneo);
            }

            return Ok(lsTorneos);
        }
    }
}