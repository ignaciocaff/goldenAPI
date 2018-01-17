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
    public class JugadorController: ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/jugador/registrar")]
        public IHttpActionResult registrar([FromBody]Jugador jugador)
        {
            personas persona = new personas();
            jugadores jugadorDto = new jugadores();
            equipos equipo = new equipos();

            personas personaInexistente = db.personas.Where(x => x.nro_documento == jugador.nro_documento).FirstOrDefault();

            if (!personaInexistente.Equals(null))
                return BadRequest("El jugador ya existe");


            jugadorDto.id_persona = registrarPersona(jugador);
            
            jugadorDto.numero = jugador.numero;
            jugadorDto.fecha_alta = jugador.fecha_alta;
            jugadorDto.id_equipo = jugador.equipo.id_equipo;

            db.jugadores.Add(jugadorDto);
            db.SaveChanges();
            return Ok();
        }


        public int registrarDomicilio(Jugador jugador)
        {
            goldenEntities db = new goldenEntities();

            domicilios domicilio = new domicilios();

            domicilio.calle = jugador.domicilio.calle;
            domicilio.numeracion = jugador.domicilio.numeracion;
            domicilio.piso = jugador.domicilio.piso;
            domicilio.dpto = jugador.domicilio.dpto;
            domicilio.torre = jugador.domicilio.torre;
            domicilio.id_localidad = jugador.domicilio.localidad.id_localidad;
            domicilio.barrio = jugador.domicilio.barrio;
            domicilio.observaciones = jugador.domicilio.observaciones;

            try
            {
                db.domicilios.Add(domicilio);
                db.SaveChanges();

                domicilios domicilioGuardado = db.domicilios.Where(x => x.calle.ToUpper().Equals(domicilio.calle.ToUpper()) && x.numeracion == domicilio.numeracion).FirstOrDefault();
                return domicilioGuardado.id_domicilio;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public int registrarContacto(Jugador jugador)
        {
            goldenEntities db = new goldenEntities();
            contactos contacto = new contactos();

            contacto.telefono_fijo = jugador.contacto.telefono_fijo;
            contacto.telefono_movil = jugador.contacto.telefono_movil;
            contacto.email = jugador.contacto.email;

            try
            {
                db.contactos.Add(contacto);
                db.SaveChanges();

                contactos contactoGuardado = db.contactos.Where(x => x.telefono_movil == contacto.telefono_movil).FirstOrDefault();
                return contactoGuardado.id_contacto;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public int registrarPersona(Jugador jugador)
        {
            goldenEntities db = new goldenEntities();
            personas persona = new personas();

            if (jugador.Equals(null))
                return 0;

            persona.id_domicilio = registrarDomicilio(jugador);
            persona.id_contacto = registrarContacto(jugador);

            persona.nombre = jugador.nombre;
            persona.apellido = jugador.apellido;
            persona.fecha_nacimiento = jugador.fecha_nacimiento;
            persona.nro_documento = jugador.nro_documento;
            persona.id_tipo_documento= jugador.tipoDocumento.id_tipo_documento;

            try { 

                db.personas.Add(persona);
                db.SaveChanges();

                personas personaGuardada = db.personas.Where(x => x.nro_documento == persona.nro_documento).FirstOrDefault();
                return personaGuardada.id_persona;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

    }
}
