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
    public class JugadorController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/jugador/registrar")]
        public IHttpActionResult registrar([FromBody]Jugador jugador)
        {
            personas persona = new personas();
            jugadores jugadorDto = new jugadores();
            equipos equipo = new equipos();
            try
            {
                //Si el id_jugador es distinto de null, entonces estoy haciendo un update y no es necesario verificar el limite.
                if (jugador.id_jugador == null)
                {
                    if (jugador.rol.Equals("jugador"))
                    {
                        string respuesta = verificarLimiteEquipo(jugador);

                        if (!respuesta.Equals(""))
                        {
                            return BadRequest(respuesta);
                        }
                    }
                    if (jugador.rol.Equals("director_tecnico"))
                    {
                        if (verificarDT((int)jugador.equipo.id_equipo))
                        {
                            return BadRequest("El equipo ya tiene un director técnico asociado.");
                        }
                    }
                    if (jugador.rol.Equals("representante"))
                    {
                        if (verificarRepresentante((int)jugador.equipo.id_equipo))
                        {
                            return BadRequest("El equipo ya tiene un representante asociado");
                        }
                    }
                }

                if (jugador.id_persona == null)
                {
                    jugadorDto.id_persona = registrarPersona(jugador);
                }
                else
                {
                    jugadorDto.id_persona = jugador.id_persona;
                    actualizarPersona(jugador);
                }

                if (jugador.id_jugador == null)
                {
                    jugadorDto.numero = jugador.numero;
                    jugadorDto.fecha_alta = DateTime.Now;
                    jugadorDto.id_equipo = jugador.equipo.id_equipo;
                    jugadorDto.rol = jugador.rol;

                    db.jugadores.Add(jugadorDto);
                    db.SaveChanges();
                    return Ok();
                }
                else
                {
                    actualizarJugador(jugador);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        public int registrarDomicilio(Jugador jugador)
        {
            try
            {
                domicilios domicilio = new domicilios();

                domicilio.calle = jugador.domicilio.calle;
                domicilio.numeracion = jugador.domicilio.numeracion;
                domicilio.piso = jugador.domicilio.piso;
                domicilio.dpto = jugador.domicilio.dpto;
                domicilio.torre = jugador.domicilio.torre;
                domicilio.id_localidad = jugador.domicilio.provincia.lsLocalidades[0].id_localidad;
                domicilio.barrio = jugador.domicilio.barrio;
                domicilio.observaciones = jugador.domicilio.observaciones;
                domicilio.fecha_alta = DateTime.Now;

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
            try
            {
                contactos contacto = new contactos();

                contacto.telefono_fijo = jugador.contacto.telefono_fijo;
                contacto.telefono_movil = jugador.contacto.telefono_movil;
                contacto.email = jugador.contacto.email;
                contacto.fecha_alta = DateTime.Now;

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
            try
            {
                personas persona = new personas();

                if (jugador.Equals(null))
                    return 0;

                persona.id_domicilio = registrarDomicilio(jugador);
                persona.id_contacto = registrarContacto(jugador);

                persona.nombre = jugador.nombre;
                persona.apellido = jugador.apellido;
                persona.fecha_nacimiento = jugador.fecha_nacimiento;
                persona.nro_documento = jugador.nro_documento;
                persona.id_tipo_documento = (int)jugador.tipoDocumento.id_tipo_documento;
                persona.id_foto = jugador.id_foto;
                persona.ocupacion = jugador.ocupacion;
                persona.fecha_alta = DateTime.Now;



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

        public void actualizarPersona(Jugador jugador)
        {
            personas personaDto = new personas();
            try
            {
                actualizarContacto(jugador);
                actualizarDomicilio(jugador);

                personaDto.id_persona = (int)jugador.id_persona;
                personaDto.nombre = jugador.nombre;
                personaDto.apellido = jugador.apellido;
                personaDto.nro_documento = jugador.nro_documento;
                personaDto.ocupacion = jugador.ocupacion;
                personaDto.fecha_nacimiento = jugador.fecha_nacimiento;
                personaDto.fecha_modificacion = DateTime.Now;
                personaDto.id_foto = jugador.id_foto;
                personaDto.id_tipo_documento = (int)jugador.tipoDocumento.id_tipo_documento;
                personaDto.id_domicilio = (int)jugador.domicilio.id_domicilio;
                personaDto.id_contacto = (int)jugador.contacto.id_contacto;

                var result = db.personas.SingleOrDefault(x => x.id_persona == personaDto.id_persona);

                if (result != null)
                {
                    result.id_persona = personaDto.id_persona;
                    result.nombre = personaDto.nombre;
                    result.apellido = personaDto.apellido;
                    result.nro_documento = personaDto.nro_documento;
                    result.ocupacion = personaDto.ocupacion;
                    result.fecha_nacimiento = personaDto.fecha_nacimiento;
                    result.fecha_modificacion = personaDto.fecha_modificacion;
                    result.id_foto = personaDto.id_foto;
                    result.id_domicilio = personaDto.id_domicilio;
                    result.id_tipo_documento = personaDto.id_tipo_documento;
                    result.id_contacto = personaDto.id_contacto;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public void actualizarDomicilio(Jugador jugador)
        {
            domicilios domicilio = new domicilios();

            try
            {
                domicilio.id_domicilio = (int)jugador.domicilio.id_domicilio;
                domicilio.calle = jugador.domicilio.calle;
                domicilio.numeracion = jugador.domicilio.numeracion;
                domicilio.piso = jugador.domicilio.piso;
                domicilio.dpto = jugador.domicilio.dpto;
                domicilio.torre = jugador.domicilio.torre;
                domicilio.id_localidad = (int)jugador.domicilio.provincia.lsLocalidades[0].id_localidad;
                domicilio.fecha_modificacion = DateTime.Now;
                domicilio.observaciones = jugador.domicilio.observaciones;
                domicilio.barrio = jugador.domicilio.barrio;

                var r = db.domicilios.SingleOrDefault(x => x.id_domicilio == domicilio.id_domicilio);

                if (r != null)
                {
                    r.id_domicilio = domicilio.id_domicilio;
                    r.calle = domicilio.calle;
                    r.numeracion = domicilio.numeracion;
                    r.piso = domicilio.piso;
                    r.dpto = domicilio.dpto;
                    r.torre = domicilio.torre;
                    r.id_localidad = domicilio.id_localidad;
                    r.fecha_modificacion = domicilio.fecha_modificacion;
                    r.observaciones = domicilio.observaciones;
                    r.barrio = domicilio.barrio;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public void actualizarContacto(Jugador jugador)
        {
            contactos c = new contactos();
            try
            {
                c.id_contacto = (int)jugador.contacto.id_contacto;
                c.telefono_fijo = jugador.contacto.telefono_fijo;
                c.telefono_movil = jugador.contacto.telefono_movil;
                c.email = jugador.contacto.email;
                c.fecha_modificacion = DateTime.Now;

                var r = db.contactos.SingleOrDefault(x => x.id_contacto == c.id_contacto);

                if (r != null)
                {
                    r.id_contacto = c.id_contacto;
                    r.telefono_fijo = c.telefono_fijo;
                    r.telefono_movil = c.telefono_movil;
                    r.email = c.email;
                    r.fecha_modificacion = c.fecha_modificacion;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        [ResponseType(typeof(Persona))]
        [Route("api/persona/{doc}")]
        public Jugador getByDoc(int doc)
        {
            Jugador per = new Jugador();
            try
            {

                var ObjPersona = (from tPersonas in db.personas
                                  join tTiposDocumento in db.tipos_documento on tPersonas.id_tipo_documento equals tTiposDocumento.id_tipo_documento
                                  join tDomicilio in db.domicilios on tPersonas.id_domicilio equals tDomicilio.id_domicilio
                                  join tContacto in db.contactos on tPersonas.id_contacto equals tContacto.id_contacto
                                  join tLocalidad in db.localidades on tDomicilio.id_localidad equals tLocalidad.id_localidad
                                  join tProvincias in db.provincias on tLocalidad.id_provincia equals tProvincias.id_provincia
                                  // join tJugador in db.jugadores on tPersonas.id_persona equals tJugador.id_persona
                                  where tPersonas.nro_documento == doc
                                  select new
                                  {
                                      id_persona = tPersonas.id_persona,
                                      nro_documento = tPersonas.nro_documento,
                                      tipo_documento = tTiposDocumento.id_tipo_documento,
                                      descr_documento = tTiposDocumento.descripcion,

                                      id_domicilio = tDomicilio.id_domicilio,
                                      calle = tDomicilio.calle,
                                      numeracion = tDomicilio.numeracion,
                                      piso = tDomicilio.piso,
                                      dpto = tDomicilio.dpto,
                                      torre = tDomicilio.torre,
                                      barrio = tDomicilio.barrio,
                                      obs = tDomicilio.observaciones,

                                      id_prov = tProvincias.id_provincia,
                                      n_prov = tProvincias.n_provincia,

                                      id_loc = tLocalidad.id_localidad,
                                      n_loc = tLocalidad.n_localidad,

                                      nombre = tPersonas.nombre,
                                      apellido = tPersonas.apellido,
                                      fecha_nacim = tPersonas.fecha_nacimiento,
                                      ocupacion = tPersonas.ocupacion,
                                      id_foto = tPersonas.id_foto,

                                      id_contacto = tContacto.id_contacto,
                                      email = tContacto.email,
                                      cel = tContacto.telefono_movil,
                                      fijo = tContacto.telefono_fijo
                                  });


                foreach (var p in ObjPersona)
                {
                    Jugador jugador = new Jugador();
                    TipoDocumento tipoDoc = new TipoDocumento();
                    Domicilio domicilio = new Domicilio();
                    Contacto contacto = new Contacto();
                    Provincia prov = new Provincia();
                    Localidad loc = new Localidad();
                    List<Localidad> listaLoc = new List<Localidad>();

                    jugador.id_persona = p.id_persona;
                    jugador.nombre = p.nombre;
                    jugador.apellido = p.apellido;
                    jugador.fecha_nacimiento = p.fecha_nacim;
                    jugador.ocupacion = p.ocupacion;
                    jugador.nro_documento = Convert.ToInt32(p.nro_documento);
                    jugador.id_foto = (int)p.id_foto;

                    jugador.tipoDocumento = tipoDoc;
                    jugador.tipoDocumento.id_tipo_documento = p.tipo_documento;
                    jugador.tipoDocumento.descripcion = p.descr_documento;

                    jugador.domicilio = domicilio;
                    jugador.domicilio.id_domicilio = p.id_domicilio;
                    jugador.domicilio.calle = p.calle;
                    jugador.domicilio.numeracion = p.numeracion;
                    jugador.domicilio.piso = p.piso;
                    jugador.domicilio.dpto = p.dpto;
                    jugador.domicilio.torre = p.torre;
                    jugador.domicilio.barrio = p.barrio;
                    jugador.domicilio.observaciones = p.obs;

                    jugador.domicilio.provincia = prov;
                    jugador.domicilio.provincia.id_provincia = p.id_prov;
                    jugador.domicilio.provincia.n_provincia = p.n_prov;

                    listaLoc.Add(new Localidad(p.id_loc, p.n_loc));
                    jugador.domicilio.provincia.lsLocalidades = listaLoc;

                    jugador.contacto = contacto;
                    jugador.contacto.id_contacto = p.id_contacto;
                    jugador.contacto.email = p.email;
                    jugador.contacto.telefono_fijo = p.fijo;
                    jugador.contacto.telefono_movil = p.cel;

                    return jugador;
                }
                return per;
            }
            catch (Exception ex)
            {
                return per;
            }
        }

        [ResponseType(typeof(Jugador))]
        [Route("api/jugador/obtenerJugador")]
        public IHttpActionResult obtenerJugador([FromBody]Jugador jugador)
        {
            try
            {
                Jugador jug = new Jugador();
                jug = getByDoc(jugador.nro_documento);
                var j = db.jugadores.Where(x => x.id_equipo == jugador.equipo.id_equipo && x.id_persona == jug.id_persona).FirstOrDefault();

                if (j != null)
                {
                    jug.rol = j.rol;
                    jug.id_jugador = j.id_jugador;

                }
                return Ok(jug);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        public void actualizarJugador(Jugador jugador)
        {
            jugadores j = new jugadores();
            try
            {
                j.id_jugador = (int)jugador.id_jugador;
                j.fecha_alta = jugador.fecha_alta;
                j.numero = jugador.numero;
                j.id_persona = jugador.id_persona;
                j.rol = jugador.rol;
                j.id_equipo = jugador.equipo.id_equipo;

                var r = db.jugadores.SingleOrDefault(x => x.id_jugador == jugador.id_jugador);

                if (r != null)
                {
                    r.id_jugador = j.id_jugador;
                    r.fecha_alta = j.fecha_alta;
                    r.numero = j.numero;
                    r.id_persona = j.id_persona;
                    r.rol = j.rol;
                    r.id_equipo = j.id_equipo;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public String verificarLimiteEquipo(Jugador jugador)
        {
            try
            {
                var juga = db.jugadores.Where(x => x.id_persona == jugador.id_persona && x.id_equipo == jugador.equipo.id_equipo).FirstOrDefault();

                if (juga != null)
                {
                    return "El jugador ya fue registrado previamente en este equipo.";
                }

                var cantidadJugadores = db.jugadores.Count(x => x.id_equipo == jugador.equipo.id_equipo && x.rol.Equals("jugador"));

                if (cantidadJugadores == 25)
                {
                    return "La cantidad de jugadores no puede ser superior a 25.";
                }

                if (jugador.equipo.categoria.descripcion.Equals("Libre"))
                {
                    return "";
                }

                if (jugador.equipo.categoria.descripcion.Equals("+30"))
                {
                    if (jugador.edad >= 30)
                    {
                        return "";
                    }
                    else
                    {
                        if (verificarCupos30((int)jugador.equipo.id_equipo))
                        {
                            return "";
                        }
                        else
                        {
                            return "No hay mas cupos en este equipo para jugadores menores de 30 años.";
                        }
                    }
                }

                if (jugador.equipo.categoria.descripcion.Equals("+35"))
                {
                    if (jugador.edad < 30)
                    {
                        return "El jugador debe tener al menos 30 años para formar parte de este equipo.";
                    }
                    else
                    {
                        if (jugador.edad >= 35)
                        {
                            return "";
                        }
                        else
                        {
                            if (verificarCupos35((int)jugador.equipo.id_equipo))
                            {
                                return "";
                            }
                            else
                            {
                                return "No hay mas cupos en este equipo para jugadores menores de 35 años.";
                            }
                        }
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return (ex.ToString());
            }
        }

        public Boolean verificarCupos30(int id_equipo)
        {
            try
            {
                var jugadores = db.jugadores.Where(x => x.id_equipo == id_equipo).ToList();
                int cantidad = 0;
                DateTime fecha = DateTime.Now;

                foreach (var j in jugadores)
                {
                    var persona = db.personas.Where(x => x.id_persona == j.id_persona).FirstOrDefault();
                    var edad = fecha.Year - persona.fecha_nacimiento.Year;

                    if (fecha.Month < persona.fecha_nacimiento.Month ||
                        (fecha.Month == persona.fecha_nacimiento.Month && fecha.Day < persona.fecha_nacimiento.Day))
                    {
                        edad--;
                    }

                    if (edad < 30)
                    {
                        cantidad++;
                    }
                }
                return (cantidad < 6);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Boolean verificarCupos35(int id_equipo)
        {
            try
            {
                var jugadores = db.jugadores.Where(x => x.id_equipo == id_equipo).ToList();
                int cantidad = 0;
                DateTime fecha = DateTime.Now;

                foreach (var j in jugadores)
                {
                    var persona = db.personas.Where(x => x.id_persona == j.id_persona).FirstOrDefault();
                    var edad = fecha.Year - persona.fecha_nacimiento.Year;

                    if (fecha.Month < persona.fecha_nacimiento.Month ||
                        (fecha.Month == persona.fecha_nacimiento.Month && fecha.Day < persona.fecha_nacimiento.Day))
                    {
                        edad--;
                    }

                    if (edad < 35)
                    {
                        cantidad++;
                    }
                }
                return (cantidad < 8);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Boolean verificarDT(int id_equipo)
        {
            try
            {
                var jugadores = db.jugadores.Where(x => x.id_equipo == id_equipo && x.rol.Equals("director_tecnico")).FirstOrDefault();
                return (jugadores != null);
            }
            catch
            {
                return false;
            }
        }

        public Boolean verificarRepresentante(int id_equipo)
        {
            try
            {
                var jugadores = db.jugadores.Where(x => x.id_equipo == id_equipo && x.rol.Equals("representante")).FirstOrDefault();
                return (jugadores != null);
            }
            catch
            {
                return false;
            }
        }
    }
}
