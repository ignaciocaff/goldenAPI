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
    public class CanchasController : ApiController
    {
        goldenEntities db = new goldenEntities();


        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/canchas")]
        public IHttpActionResult GetAll()
        {
            List<Cancha> lsCanchas = new List<Cancha>();

            try
            {
                var canchas = db.canchas.ToList();
                foreach (var c in canchas)
                {
                    var dom = db.domicilios.Where(x => x.id_domicilio == c.id_domicilio).FirstOrDefault();
                    var cl = db.clubes.Where(x => x.id_club == c.id_club).FirstOrDefault();
                    Cancha cancha = new Cancha();
                    Domicilio domicilio = new Domicilio();
                    Club club = new Club();
                    cancha.nombre = c.nombre;
                    cancha.id_cancha = c.id_cancha;
                    cancha.capacidad = (int)c.capacidad;
                    cancha.domicilio = domicilio;
                    cancha.domicilio.id_domicilio = dom.id_domicilio;
                    cancha.domicilio.barrio = dom.barrio;
                    cancha.domicilio.calle = dom.calle;
                    cancha.domicilio.observaciones = dom.observaciones;
                    cancha.domicilio.numeracion = dom.numeracion;
                    cancha.club = club;
                    cancha.club.id_club = cl.id_club;
                    cancha.club.nombre = cl.nombre;
                    lsCanchas.Add(cancha);
                }
                return Ok(lsCanchas);
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/canchas/registrar")]
        public IHttpActionResult registrar([FromBody]Cancha cancha)
        {
            canchas canchaDto = new canchas();

            try
            {
                canchaDto.nombre = cancha.nombre;
                canchaDto.capacidad = cancha.capacidad;
                canchaDto.id_club = cancha.club.id_club;

                int canchaCheck = db.canchas.Where(x => x.nombre.ToUpper().Equals(canchaDto.nombre.ToUpper())).Count();

                if (canchaCheck == 0)
                {
                    // Si no existe el nombre de la cancha, doy de alta el domicilio que trae antes de dar de alta la cancha
                    // El club no es necesario porque va a ser siempre 1, de Golden Club.
                    domicilios domicilioDto = new domicilios();
                    domicilioDto.barrio = cancha.domicilio.barrio;
                    domicilioDto.observaciones = cancha.domicilio.observaciones;
                    domicilioDto.calle = cancha.domicilio.calle;
                    domicilioDto.numeracion = cancha.domicilio.numeracion;

                    db.domicilios.Add(domicilioDto);
                    db.SaveChanges();

                    int id_domicilio = domicilioDto.id_domicilio;
                    canchaDto.id_domicilio = id_domicilio;

                    db.canchas.Add(canchaDto);
                    db.SaveChanges();
                    return Ok(true);
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }

        [Route("api/canchas/update")]
        public IHttpActionResult update([FromBody]Cancha cancha)
        {
            canchas canchaDto = new canchas();
            domicilios domicilio = new domicilios();
            Boolean transaccion = false;

            try
            {
                canchaDto.nombre = cancha.nombre;
                canchaDto.capacidad = cancha.capacidad;
                canchaDto.id_cancha = cancha.id_cancha;
                domicilio.id_domicilio = (int)cancha.domicilio.id_domicilio;
                domicilio.barrio = cancha.domicilio.barrio;
                domicilio.calle = cancha.domicilio.calle;
                domicilio.numeracion = cancha.domicilio.numeracion;
                domicilio.observaciones = cancha.domicilio.observaciones;

                canchas canchaCheck = db.canchas.Where(x => x.nombre.ToUpper().Equals(canchaDto.nombre.ToUpper())).FirstOrDefault();

                if ((canchaCheck != null && canchaCheck.id_cancha == cancha.id_cancha) || canchaCheck == null)
                {
                    var result = db.canchas.SingleOrDefault(b => b.id_cancha == canchaDto.id_cancha);
                    var resultDom = db.domicilios.SingleOrDefault(b => b.id_domicilio == domicilio.id_domicilio);
                    if (resultDom != null)
                    {
                        resultDom.id_domicilio = domicilio.id_domicilio;
                        resultDom.calle = domicilio.calle;
                        resultDom.numeracion = domicilio.numeracion;
                        resultDom.observaciones = domicilio.observaciones;
                        resultDom.barrio = domicilio.barrio;
                        db.SaveChanges();
                        transaccion = true;
                    }

                    if (transaccion && result != null)
                    {
                        result.id_cancha = canchaDto.id_cancha;
                        result.nombre = canchaDto.nombre;
                        result.capacidad = canchaDto.capacidad;
                        result.id_club = 1;
                        db.SaveChanges();
                        return Ok();
                    }
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}