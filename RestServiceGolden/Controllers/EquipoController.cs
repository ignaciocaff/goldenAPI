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
    public class EquipoController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/equipo/registrar")]
        public IHttpActionResult registrar([FromBody]Equipo equipo)
        {
            equipos equipoDto = new equipos();
            categorias categoria = new categorias();
            clubes club = new clubes();

            equipoDto.nombre = equipo.nombre;
            equipoDto.descripcion = equipo.descripcion;
            equipoDto.fecha_alta = DateTime.Now;
            equipoDto.logo = equipo.logo;
            equipoDto.camiseta = equipo.camiseta;
            equipoDto.camisetalogo = equipo.camisetalogo;
            equipoDto.id_club = equipo.club.id_club;
            equipoDto.id_categoria_equipo = equipo.categoria.id_categoria;
            equipoDto.id_torneo = equipo.torneo.id_torneo;

            int equiposCheck = db.equipos.Where(x => x.nombre.ToUpper().Equals(equipoDto.nombre.ToUpper()) && x.id_categoria_equipo == equipoDto.id_categoria_equipo).Count();

            if (equiposCheck == 0)
            {
                db.equipos.Add(equipoDto);
                db.SaveChanges();
                return Ok();
            }

            return BadRequest("Ya existe un equipo registrado para esta categoría con ese nombre.");
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/equipo/desvincular")]
        public IHttpActionResult desvincular([FromBody]List<Equipo> lsEquipos)
        {
            try
            {
                foreach (Equipo e in lsEquipos)
                {
                    equipos equipoToUpdate = db.equipos.Where(x => x.id_equipo == e.id_equipo).FirstOrDefault();
                    equipoToUpdate.id_torneo = null;
                    db.SaveChanges();
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("No se pudieron desvincular los equipos");
            }
        }

        [ResponseType(typeof(Equipo))]
        [Route("api/equipo/obtenerTodos")]
        public IHttpActionResult getAll()
        {
            List<Equipo> lsEquipos = new List<Equipo>();

            try
            {
                var equipos = db.equipos.ToList();

                foreach (var tEquipo in equipos)
                {
                    var torneoDb = db.torneos.Where(x => x.id_torneo == tEquipo.id_torneo).FirstOrDefault();
                    var categoriaDb = db.categorias.Where(x => x.id_categoria == tEquipo.id_categoria_equipo).FirstOrDefault();
                    Equipo equipo = new Equipo();
                    Categoria categoria = new Categoria();
                    Torneo torneo = new Torneo();
                    Club club = new Club();
                    equipo.id_equipo = tEquipo.id_equipo;
                    equipo.nombre = tEquipo.nombre;
                    equipo.descripcion = tEquipo.descripcion;
                    equipo.fecha_alta = Convert.ToDateTime(tEquipo.fecha_alta);
                    equipo.logo = (tEquipo.logo != null) ? tEquipo.logo.Value : 0;
                    equipo.camiseta = (tEquipo.camiseta != null) ? tEquipo.camiseta.Value : 0;
                    equipo.camisetalogo = (tEquipo.camisetalogo != null) ? tEquipo.camisetalogo.Value : 0;
                    equipo.categoria = categoria;
                    equipo.club = club;
                    equipo.torneo = torneo;
                    equipo.categoria.id_categoria = (int)tEquipo.id_categoria_equipo;
                    equipo.club.id_club = tEquipo.id_club;
                    equipo.torneo.id_torneo = tEquipo.id_torneo;
                    equipo.torneo.nombre = torneoDb.nombre;
                    equipo.categoria.descripcion = categoriaDb.descripcion;
                    lsEquipos.Add(equipo);
                }
                return Ok(lsEquipos);
            }
            catch (Exception e)
            {
                e.ToString();
                Console.WriteLine(e.ToString());
                return BadRequest(e.ToString());
            }
        }
    }
}