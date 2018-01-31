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
    public class NoticiasController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/noticias/categorias")]
        public IHttpActionResult GetAll()
        {
            List<CategoriaNoticia> lsCategoriasNoticias= new List<CategoriaNoticia>();

            var categoriasnoticias = db.categorias_noticias.ToList();

            foreach (var cn in categoriasnoticias)
            {
                CategoriaNoticia categoria= new CategoriaNoticia();
                categoria.id_categoria_noticia = cn.id_categoria_noticia;
                categoria.descripcion = cn.descripcion;
                lsCategoriasNoticias.Add(categoria);
            }
            return Ok(lsCategoriasNoticias);
        }
    }
}