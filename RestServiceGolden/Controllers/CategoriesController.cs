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
    public class CategoriesController: ApiController
    {
        [Route("api/torneo/categorias")]
        public Object[] getCategories()
        {
            goldenEntities db = new goldenEntities();
            Categoria[] lsCategorias = new Categoria[20];

            try
            {
                var categorias = (from tCategorias in db.categorias select new
                {
                    idCategoria = tCategorias.id_categoria,
                    descripcion = tCategorias.descripcion
                });

                int i = 0;

                foreach(var cat in categorias)
                {
                    lsCategorias[i] = new Categoria();
                    lsCategorias[i].id_categoria = cat.idCategoria;
                    lsCategorias[i].descripcion  = cat.descripcion;
                    i++;
                }
                return lsCategorias;
            }
            catch(Exception ex)
            {
                return lsCategorias;
            }
        }
    }
}