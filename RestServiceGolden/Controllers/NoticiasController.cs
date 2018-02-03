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
        [Route("api/noticia/registrar")]
        public IHttpActionResult registrar([FromBody]Noticia noticia)
        {
            noticias noticiaDto = new noticias();
            torneos torneo = new torneos();
            categorias_noticias categoriaNoticia = new categorias_noticias();
            clubes club = new clubes();

            noticiaDto.titulo = noticia.titulo;
            noticiaDto.descripcion = noticia.descripcion;
            noticiaDto.fecha = DateTime.Now;
            noticiaDto.id_torneo = noticia.torneo.id_torneo;
            noticiaDto.id_club = noticia.club.id_club;
            noticiaDto.id_categoria_noticia = noticia.categoriaNoticia.id_categoria_noticia;
            noticiaDto.tags = noticia.tags;
            noticiaDto.id_thumbnail = noticia.id_thumbnail;

            db.noticias.Add(noticiaDto);
            db.SaveChanges();
            return Ok();
        }

        [ResponseType(typeof(Noticia))]
        [Route("api/noticia/principales")]
        public IHttpActionResult getPrincipales()
        {
            List<Noticia> lsNoticiasPrincipales= new List<Noticia>();

            var noticias = db.noticias.Where(x => x.id_categoria_noticia == 1).ToList();

            foreach (var n in noticias)
            {
                Noticia noticia = new Noticia();
                Torneo torneo = new Torneo();
                Club club = new Club();
                CategoriaNoticia categoriaNoticia = new CategoriaNoticia();
                noticia.torneo = torneo;
                noticia.club = club;
                noticia.categoriaNoticia = categoriaNoticia;

                noticia.id_noticia = n.id_noticia;
                noticia.titulo = n.titulo;
                noticia.descripcion = n.descripcion;
                noticia.fecha = Convert.ToDateTime(n.fecha);
                noticia.torneo.id_torneo = n.id_torneo;
                noticia.club.id_club = n.id_club;
                noticia.categoriaNoticia.id_categoria_noticia = n.id_categoria_noticia;
                noticia.tags = n.tags;
                noticia.id_thumbnail = n.id_thumbnail.Value;
                lsNoticiasPrincipales.Add(noticia);
            }
            return Ok(lsNoticiasPrincipales);
        }

        [ResponseType(typeof(Noticia))]
        [Route("api/noticia/secundarias")]
        public IHttpActionResult getSecundarias()
        {
            List<Noticia> lsNoticiasSecundarias = new List<Noticia>();

            var noticias = db.noticias.Where(x => x.id_categoria_noticia == 2).ToList();

            foreach (var n in noticias)
            {
                Noticia noticia = new Noticia();
                Torneo torneo = new Torneo();
                Club club = new Club();
                CategoriaNoticia categoriaNoticia = new CategoriaNoticia();
                noticia.torneo = torneo;
                noticia.club = club;
                noticia.categoriaNoticia = categoriaNoticia;

                noticia.id_noticia = n.id_noticia;
                noticia.titulo = n.titulo;
                noticia.descripcion = n.descripcion;
                noticia.fecha = Convert.ToDateTime(n.fecha);
                noticia.torneo.id_torneo = n.id_torneo;
                noticia.club.id_club = n.id_club;
                noticia.categoriaNoticia.id_categoria_noticia = n.id_categoria_noticia;
                noticia.tags = n.tags;
                noticia.id_thumbnail = n.id_thumbnail.Value;
                lsNoticiasSecundarias.Add(noticia);
            }
            return Ok(lsNoticiasSecundarias);
        }

        [ResponseType(typeof(Noticia))]
        [Route("api/noticia/{id}")]
        public IHttpActionResult getById(int id)
        {
            var noticias = db.noticias.Where(x => x.id_noticia == id).FirstOrDefault();

            Noticia noticia = new Noticia();
            Torneo torneo = new Torneo();
            Club club = new Club();
            CategoriaNoticia categoriaNoticia = new CategoriaNoticia();
            noticia.torneo = torneo;
            noticia.club = club;
            noticia.categoriaNoticia = categoriaNoticia;

            noticia.id_noticia = noticias.id_noticia;
            noticia.titulo = noticias.titulo;
            noticia.descripcion = noticias.descripcion;
            noticia.fecha = Convert.ToDateTime(noticias.fecha);
            noticia.torneo.id_torneo = noticias.id_torneo;
            noticia.club.id_club = noticias.id_club;
            noticia.categoriaNoticia.id_categoria_noticia = noticias.id_categoria_noticia;
            noticia.tags = noticias.tags;
            noticia.id_thumbnail = noticias.id_thumbnail.Value;

            return Ok(noticia);
        }

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