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
    }
}