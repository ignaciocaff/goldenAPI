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
    public class NoticiasController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/noticia/registrar")]
        public IHttpActionResult registrar([FromBody]Noticia noticia)
        {
            try
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
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(Noticia))]
        [Route("api/noticia/principales/{id}")]
        public IHttpActionResult getPrincipales(int id)
        {
            List<Noticia> lsNoticiasPrincipales = new List<Noticia>();
            try
            {
                var noticias = db.noticias.OrderByDescending(x => x.id_noticia).Where(x => x.id_categoria_noticia == 1 && (x.id_torneo == id || x.id_torneo == null)).Take(1);

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
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseType(typeof(Noticia))]
        [Route("api/noticia/secundarias/{id}")]
        public IHttpActionResult getSecundarias(int id)
        {
            try
            {
                List<Noticia> lsNoticiasSecundarias = new List<Noticia>();

                var noticias = db.noticias.OrderByDescending(x => x.id_noticia).Where(x => x.id_categoria_noticia == 2 && (x.id_torneo == id || x.id_torneo == null)).Take(3);

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
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseType(typeof(Noticia))]
        [Route("api/noticia/{id}")]
        public IHttpActionResult getById(int id)
        {
            try
            {
                var noticias = db.noticias.Where(x => x.id_noticia == id).FirstOrDefault();
                var cat_not = db.categorias_noticias.Where(x => x.id_categoria_noticia == noticias.categorias_noticias.id_categoria_noticia).First();

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
                if (noticias.id_torneo != null)
                {
                    noticia.torneo.nombre = noticias.torneos.nombre;
                }
                else
                {
                    noticia.torneo.nombre = null;
                }
                noticia.club.id_club = noticias.id_club;
                noticia.categoriaNoticia.id_categoria_noticia = noticias.id_categoria_noticia;
                noticia.categoriaNoticia.descripcion = noticias.categorias_noticias.descripcion;
                noticia.tags = noticias.tags;
                noticia.id_thumbnail = noticias.id_thumbnail.Value;

                return Ok(noticia);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/noticias/categorias")]
        public IHttpActionResult GetAll()
        {
            List<CategoriaNoticia> lsCategoriasNoticias = new List<CategoriaNoticia>();

            try
            {
                var categoriasnoticias = db.categorias_noticias.ToList();

                foreach (var cn in categoriasnoticias)
                {
                    CategoriaNoticia categoria = new CategoriaNoticia();
                    categoria.id_categoria_noticia = cn.id_categoria_noticia;
                    categoria.descripcion = cn.descripcion;
                    lsCategoriasNoticias.Add(categoria);
                }
                return Ok(lsCategoriasNoticias);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [Route("api/noticia/update")]
        public IHttpActionResult update([FromBody]Noticia noticia)
        {

            noticias noticiaDto = new noticias();

            try
            {
                noticiaDto.id_noticia = noticia.id_noticia.Value;
                noticiaDto.titulo = noticia.titulo;
                noticiaDto.descripcion = noticia.descripcion;
                noticiaDto.id_torneo = noticia.torneo.id_torneo;
                noticiaDto.id_club = noticia.club.id_club;
                noticiaDto.id_categoria_noticia = noticia.categoriaNoticia.id_categoria_noticia;
                noticiaDto.tags = noticia.tags;
                noticiaDto.id_thumbnail = noticia.id_thumbnail;


                var result = db.noticias.SingleOrDefault(n => n.id_noticia == noticiaDto.id_noticia);

                if (result != null)
                {
                    result.id_noticia = noticiaDto.id_noticia;
                    result.titulo = noticiaDto.titulo;
                    result.descripcion = noticiaDto.descripcion;
                    result.id_torneo = noticiaDto.id_torneo;
                    result.id_club = noticiaDto.id_club;
                    result.id_categoria_noticia = noticiaDto.id_categoria_noticia;
                    result.tags = noticiaDto.tags;
                    result.id_thumbnail = noticiaDto.id_thumbnail;
                    db.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseType(typeof(Noticia))]
        [Route("api/noticia/historicas/{id}")]
        public IHttpActionResult getHistoricas(int id)
        {
            try
            {
                List<Noticia> lsNoticiasHistoricas = new List<Noticia>();

                var noticias = db.noticias.OrderByDescending(x => x.id_noticia).Where(x => x.id_torneo == id || x.id_torneo == null).Take(10);

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
                    lsNoticiasHistoricas.Add(noticia);
                }
                return Ok(lsNoticiasHistoricas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseType(typeof(Noticia))]
        [Route("api/noticia/borrarNoticia/{id}")]
        public IHttpActionResult getBorrarNoticia(int id)
        {
            try
            {
                noticias noticia = db.noticias.Where(x => x.id_noticia == id).FirstOrDefault();

                db.noticias.Attach(noticia);
                db.noticias.Remove(noticia);
                db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
