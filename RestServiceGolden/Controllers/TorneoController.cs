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
    public class TorneoController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [Route("api/torneo/registrar")]
        public IHttpActionResult registrar([FromBody]Torneo torneo)
        {
            torneos torneoDto = new torneos();
            modalidades modalidad = new modalidades();
            reglas_torneo regla = new reglas_torneo();
            categorias categoria = new categorias();
            tipos_torneos tipoTorneo = new tipos_torneos();
            int id_torneo = 0;
            Boolean transaccion = false;

            try
            {
                torneoDto.nombre = torneo.nombre;
                torneoDto.descripcion = torneo.descripcion;
                torneoDto.fecha_inicio = torneo.fecha_inicio;
                torneoDto.fecha_fin = torneo.fecha_fin;
                torneoDto.id_modalidad = torneo.modalidad.id_modalidad;
                torneoDto.id_categoria = torneo.categoria.id_categoria;
                torneoDto.id_tipo = torneo.tipoTorneo.id_tipo;
                torneoDto.id_regla = torneo.regla.id_regla;

                torneos torneoCheck = db.torneos.Where(x => x.nombre.ToUpper().Equals(torneoDto.nombre.ToUpper())).FirstOrDefault();

                if (torneoCheck == null)
                {
                    db.torneos.Add(torneoDto);
                    db.SaveChanges();
                    id_torneo = torneoDto.id_torneo;
                    transaccion = true;
                }
                foreach (Equipo e in torneo.lsEquipos)
                {
                    if (transaccion)
                    {
                        equipos equipoToUpdate = db.equipos.Where(x => x.id_equipo == e.id_equipo).FirstOrDefault();
                        equipoToUpdate.id_torneo = id_torneo;
                    }
                }
                db.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/torneo/todos")]
        public IHttpActionResult GetAll()
        {
            List<Torneo> lsTorneos = new List<Torneo>();

            var torneos = db.torneos.ToList();

            foreach (var t in torneos)
            {
                Torneo torneo = new Torneo();
                torneo.id_torneo = t.id_torneo;
                torneo.nombre = t.nombre;
                torneo.descripcion = t.descripcion;
                lsTorneos.Add(torneo);
            }

            return Ok(lsTorneos);
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/torneo/{nombre}")]
        public IHttpActionResult GetById(String nombre)
        {
            var torneos = db.torneos.Where(x => x.nombre == nombre).FirstOrDefault();
            Torneo torneo = new Torneo();
            torneo.id_torneo = torneos.id_torneo;
            torneo.nombre = torneos.nombre;
            torneo.descripcion = torneos.descripcion;

            return Ok(torneo);
        }
    }
}