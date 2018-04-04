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
    public class ZonasController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/zona/todos/{id}")]
        public IHttpActionResult GetAll(int id)
        {
            List<Zona> lsZonas = new List<Zona>();
            try
            {
                var zonas = db.zonas.ToList().OrderBy(z => z.descripcion);

                foreach (var tZona in zonas)
                {
                    if (tZona.id_torneo == id)
                    {
                        Zona zona = new Zona();
                        Torneo torneo = new Torneo();
                        List<Equipo> lsEquipos = new List<Equipo>();
                        zona.id_zona = tZona.id_zona;
                        zona.descripcion = tZona.descripcion;
                        zona.torneo = torneo;
                        zona.torneo.id_torneo = tZona.id_torneo;

                        var equipos = (from tEquipos in db.equipos
                                       join tEquiposZona in db.equipos_zona on tEquipos.id_equipo equals tEquiposZona.id_equipo
                                       join tZonas in db.zonas on tEquiposZona.id_zona equals tZonas.id_zona
                                       where tZonas.id_zona == zona.id_zona
                                       select new
                                       {
                                           id_equipo = tEquipos.id_equipo,
                                           nombre = tEquipos.nombre,
                                           id_torneo = tEquipos.id_torneo,
                                           logo = tEquipos.logo,
                                           id_zona = tZonas.id_zona,
                                       }).OrderBy(s => s.nombre);


                        foreach (var equipoZona in equipos)
                        {
                            Equipo equipo = new Equipo();
                            Torneo torneoEquipo = new Torneo();
                            int? idZona = equipoZona.id_zona;
                            if (idZona != null)
                            {
                                equipo.id_equipo = equipoZona.id_equipo;
                                equipo.nombre = equipoZona.nombre;
                                equipo.logo = (equipoZona.logo != null) ? (int)equipoZona.logo : 0;
                                equipo.torneo = torneo;
                                equipo.torneo.id_torneo = equipoZona.id_torneo;
                                lsEquipos.Add(equipo);
                            }
                        }

                        zona.lsEquipos = lsEquipos;
                        lsZonas.Add(zona);
                    }
                }
                return Ok(lsZonas);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }


        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/zona/registrar")]
        public IHttpActionResult registrar([FromBody]List<Zona> lsZonas)
        {

            int id_zona = 0;
            List<Equipo> lsEquipos = new List<Equipo>();
            try
            {
                foreach (Zona z in lsZonas)
                {
                    int zonaCheck = db.zonas.Where(x => x.descripcion == z.descripcion && x.id_torneo == z.torneo.id_torneo).Count();
                    if (zonaCheck != 0)
                    {
                        return BadRequest("Alguna de las zonas con ese nombre ya ha sido creada para ese torneo");
                    }
                }

                foreach (Zona zona in lsZonas)
                {
                    zonas zonaDto = new zonas();
                    lsEquipos = zona.lsEquipos;
                    zonaDto.id_torneo = zona.torneo.id_torneo;
                    zonaDto.descripcion = zona.descripcion;
                    db.zonas.Add(zonaDto);
                    db.SaveChanges();
                    id_zona = zonaDto.id_zona;
                    foreach (var equiposZona in lsEquipos)
                    {
                        equipos_zona equiposZonaDto = db.equipos_zona.SingleOrDefault(x => x.id_equipo == equiposZona.id_equipo);
                        equiposZonaDto.id_zona = id_zona;
                        db.SaveChanges();
                    }

                    fixture_zona fixture_zona = new fixture_zona();
                    fixture_zona.id_tipo = 1;
                    fixture_zona.id_torneo = zona.torneo.id_torneo;
                    fixture_zona.id_zona = id_zona;
                    db.fixture_zona.Add(fixture_zona);
                    db.SaveChanges();

                }
                return Ok();

            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/zona/eliminar")]
        public IHttpActionResult eliminar([FromBody]Zona zona)
        {
            List<Equipo> lsEquipos = new List<Equipo>();
            try
            {
                foreach (var equiposZona in zona.lsEquipos)
                {
                    equipos_zona equiposZonaDto = db.equipos_zona.SingleOrDefault(x => x.id_equipo == equiposZona.id_equipo);
                    equiposZonaDto.id_zona = null;
                    db.SaveChanges();
                }

                fixture_zona fixture_zona = db.fixture_zona.SingleOrDefault(x => x.id_zona == zona.id_zona);
                db.fixture_zona.Attach(fixture_zona);
                db.fixture_zona.Remove(fixture_zona);
                db.SaveChanges();

                zonas zonaDto = new zonas();
                lsEquipos = zona.lsEquipos;
                zonaDto.id_torneo = zona.torneo.id_torneo;
                zonaDto.descripcion = zona.descripcion;
                zonaDto.id_zona = (int)zona.id_zona;
                db.zonas.Attach(zonaDto);
                db.zonas.Remove(zonaDto);
                db.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/zona/update")]
        public IHttpActionResult update([FromBody]List<Zona> lsZonas)
        {

            List<Equipo> lsEquipos = new List<Equipo>();
            try
            {

                foreach (Zona zona in lsZonas)
                {
                    var totalEquipos = db.equipos_zona.Where(x => x.id_zona == zona.id_zona).ToList();
                    lsEquipos = zona.lsEquipos;

                    var result = totalEquipos.Where(p => !lsEquipos.Any(l => p.id_equipo == l.id_equipo)).ToList();

                    foreach (var r in result)
                    {
                        equipos_zona equiposZonaDto = db.equipos_zona.SingleOrDefault(x => x.id_equipo == r.id_equipo);
                        equiposZonaDto.id_zona = null;
                        db.SaveChanges();
                    }


                    foreach (var equiposZona in lsEquipos)
                    {
                        equipos_zona equiposZonaDto = db.equipos_zona.SingleOrDefault(x => x.id_equipo == equiposZona.id_equipo);
                        equiposZonaDto.id_zona = zona.id_zona;
                        db.SaveChanges();

                    }
                }
                return Ok();

            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}