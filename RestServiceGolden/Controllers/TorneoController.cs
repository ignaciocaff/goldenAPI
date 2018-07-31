﻿using Newtonsoft.Json;
using RestServiceGolden.App_Start;
using RestServiceGolden.Models;
using RestServiceGolden.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace RestServiceGolden.Controllers
{
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
                torneoDto.id_fase = torneo.fase.id_fase;

                torneos torneoCheck = db.torneos.Where(x => x.nombre.ToUpper().Equals(torneoDto.nombre.ToUpper())).FirstOrDefault();

                if (torneoCheck == null)
                {
                    db.torneos.Add(torneoDto);
                    db.SaveChanges();
                    id_torneo = torneoDto.id_torneo;
                    transaccion = true;

                    categoria_equipos categoriaEquipo = new categoria_equipos();
                    categoriaEquipo.id = id_torneo;
                    categoriaEquipo.descripcion = torneo.nombre;
                    db.categoria_equipos.Add(categoriaEquipo);

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
                return BadRequest("El torneo ya existe.");
            }
            catch (Exception e)
            {
                var logger = new Logger("TorneoControllerException");
                logger.AgregarMensaje("api/torneo/registrar" + " Parametros de entrada: " +
                JsonConvert.SerializeObject(torneo, Formatting.None), " Excepción: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                throw new Exception(e.Message, e.InnerException);
            }
        }

        [Route("api/torneo/update")]
        public IHttpActionResult update([FromBody]Torneo torneo)
        {
            torneos torneoDto = new torneos();
            modalidades modalidad = new modalidades();
            reglas_torneo regla = new reglas_torneo();
            categorias categoria = new categorias();
            tipos_torneos tipoTorneo = new tipos_torneos();
            Boolean transaccion = false;

            try
            {
                torneoDto.nombre = torneo.nombre;
                torneoDto.id_torneo = (int)torneo.id_torneo;
                torneoDto.descripcion = torneo.descripcion;
                torneoDto.fecha_inicio = torneo.fecha_inicio;
                torneoDto.fecha_fin = torneo.fecha_fin;
                torneoDto.id_modalidad = torneo.modalidad.id_modalidad;
                torneoDto.id_categoria = torneo.categoria.id_categoria;
                torneoDto.id_tipo = torneo.tipoTorneo.id_tipo;
                torneoDto.id_regla = torneo.regla.id_regla;

                var result = db.torneos.SingleOrDefault(b => b.id_torneo == torneoDto.id_torneo);
                if (result != null)
                {
                    result.nombre = torneoDto.nombre;
                    result.id_torneo = torneoDto.id_torneo;
                    result.descripcion = torneoDto.descripcion;
                    result.fecha_inicio = torneoDto.fecha_inicio;
                    result.fecha_fin = torneoDto.fecha_fin;
                    result.id_modalidad = torneoDto.id_modalidad;
                    result.id_categoria = torneoDto.id_categoria;
                    result.id_tipo = torneoDto.id_tipo;
                    result.id_regla = torneoDto.id_regla;
                    db.SaveChanges();
                    transaccion = true;
                }
                int id_torneo = torneoDto.id_torneo;
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
                var logger = new Logger("TorneoControllerException");
                logger.AgregarMensaje("api/torneo/update" + " Parametros de entrada: " +
                   JsonConvert.SerializeObject(torneo, Formatting.None), " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest();
            }
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/torneo/todos")]
        public IHttpActionResult GetAll()
        {
            List<Torneo> lsTorneos = new List<Torneo>();
            try
            {
                var torneos = db.torneos.ToList();
                var categorias = db.categorias.ToList();
                var modalidades = db.modalidades.ToList();
                var tipos_torneos = db.tipos_torneos.ToList();
                var reglas = db.reglas_torneo.ToList();

                foreach (var t in torneos)
                {
                    Torneo torneo = new Torneo();
                    Modalidad modalidad = new Modalidad();
                    Categoria categoria = new Categoria();
                    TipoTorneo tTorneo = new TipoTorneo();
                    Regla regla = new Regla();
                    List<Equipo> lsEquipos = new List<Equipo>();
                    EstadoTorneo estado = new EstadoTorneo();
                    Fase fase = new Fase();
                    var equipos = db.equipos.Where(x => x.id_torneo == t.id_torneo).ToList();

                    torneo.id_torneo = t.id_torneo;
                    torneo.nombre = t.nombre;
                    torneo.descripcion = t.descripcion;
                    torneo.modalidad = modalidad;
                    torneo.modalidad.id_modalidad = (int)t.id_modalidad;
                    torneo.modalidad.descripcion = modalidades.Where(x => x.id_modalidad == t.id_modalidad).FirstOrDefault().descripcion;
                    torneo.categoria = categoria;
                    torneo.categoria.id_categoria = (int)t.id_categoria;
                    torneo.categoria.descripcion = categorias.Where(x => x.id_categoria == t.id_categoria).FirstOrDefault().descripcion;
                    torneo.regla = regla;
                    torneo.regla.id_regla = (int)t.id_regla;
                    torneo.regla.descripcion = reglas.Where(x => x.id_regla == t.id_regla).FirstOrDefault().descripcion;
                    torneo.tipoTorneo = tTorneo;
                    torneo.tipoTorneo.id_tipo = (int)t.id_tipo;
                    torneo.tipoTorneo.descripcion = tipos_torneos.Where(x => x.id_tipo == t.id_tipo).FirstOrDefault().descripcion;
                    torneo.fecha_fin = t.fecha_fin.Value.Date;
                    torneo.fecha_inicio = t.fecha_inicio.Value.Date;
                    torneo.fase = fase;
                    torneo.fase.id_fase = t.id_fase;
                    torneo.estado = estado;
                    torneo.estado.id_estado = t.id_estado;

                    foreach (var e in equipos)
                    {

                        Equipo equipo = new Equipo();
                        Torneo torneoEquipo = new Torneo();
                        Categoria categoriaEquipo = new Categoria();
                        equipo.id_equipo = e.id_equipo;
                        equipo.nombre = e.nombre;
                        equipo.torneo = torneoEquipo;
                        equipo.torneo.id_torneo = e.id_torneo;
                        equipo.categoria = categoriaEquipo;
                        equipo.categoria.id_categoria = (int)e.id_categoria_equipo;
                        lsEquipos.Add(equipo);
                    }
                    torneo.lsEquipos = lsEquipos;
                    if (torneo.estado.id_estado != 3)
                    {
                        lsTorneos.Add(torneo);
                    }
                }

            }
            catch (Exception e)
            {
                var logger = new Logger("TorneoControllerException");
                logger.AgregarMensaje("api/torneo/todos", "Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest();
            }
            return Ok(lsTorneos);
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/torneo/vigentes")]
        public IHttpActionResult GetAllSoloTorneo()
        {
            List<Torneo> lsTorneos = new List<Torneo>();
            try
            {
                var torneos = db.torneos.ToList();

                foreach (var t in torneos)
                {
                    Torneo torneo = new Torneo();
                    Fase fase = new Fase();
                    EstadoTorneo estado = new EstadoTorneo();
                    torneo.id_torneo = t.id_torneo;
                    torneo.nombre = t.nombre;
                    torneo.descripcion = t.descripcion;
                    torneo.fecha_fin = t.fecha_fin.Value.Date;
                    torneo.fecha_inicio = t.fecha_inicio.Value.Date;
                    torneo.fase = fase;
                    torneo.fase.id_fase = t.id_fase;
                    torneo.estado = estado;
                    torneo.estado.id_estado = t.id_estado;

                    if (torneo.estado.id_estado != 3)
                    {
                        lsTorneos.Add(torneo);
                    }
                }

            }
            catch (Exception e)
            {
                var logger = new Logger("TorneoControllerException");
                logger.AgregarMensaje("api/torneo/todos", "Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest();
            }
            return Ok(lsTorneos);
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/torneo/{nombre}")]
        public IHttpActionResult GetById(String nombre)
        {
            var torneos = db.torneos.Where(x => x.nombre == nombre && x.id_estado != 3).FirstOrDefault();
            Torneo torneo = new Torneo();
            Fase fase = new Fase();
            EstadoTorneo estado = new EstadoTorneo();
            torneo.id_torneo = torneos.id_torneo;
            torneo.nombre = torneos.nombre;
            torneo.descripcion = torneos.descripcion;
            torneo.fase = fase;
            torneo.fase.id_fase = torneos.id_fase;
            torneo.estado = estado;
            torneo.estado.id_estado = torneos.id_estado;
            return Ok(torneo);
        }

        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/torneo/cambioFase/{id_torneo}")]
        public IHttpActionResult cambioFase([FromBody] Fase fase, int id_torneo)
        {
            try
            {

                var torneo = db.torneos.SingleOrDefault(x => x.id_torneo == id_torneo);
                var equipos = db.equipos_zona.Where(x => x.id_torneo == id_torneo).ToList();

                torneo.id_fase = fase.id_fase;

                foreach (var equipo in equipos)
                {
                    var equipoUpdate = db.equipos_zona.SingleOrDefault(x => x.id_equipo == equipo.id_equipo);
                    equipoUpdate.id_zona = null;
                }

                db.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                var logger = new Logger("TorneoControllerException");
                logger.AgregarMensaje("api/torneo/cambioFase" + " Parametros de entrada: " +
                   JsonConvert.SerializeObject(fase, Formatting.None) + id_torneo, " Excepcion: " + e.Message + e.StackTrace);
                logger.EscribirLog();

                return BadRequest();
            }
        }

        [ResponseType(typeof(IEquipoPlanilla))]
        [Route("api/torneo/iequiposPorTorneo/{id}")]
        public IHttpActionResult getEquiposPorTorneo(int id)
        {
            List<IEquipoPlanilla> lsEquipos = new List<IEquipoPlanilla>();

            try
            {
                var equipos = db.equipos.OrderByDescending(x => x.nombre).Where(x => x.id_torneo == id);

                foreach (var tEquipo in equipos)
                {
                    IEquipoPlanilla equipo = new IEquipoPlanilla();
                    List<IJugador> lsJugadores = new List<IJugador>();

                    equipo.id_equipo = tEquipo.id_equipo;
                    equipo.nombre = tEquipo.nombre;
                    equipo.lsJugadores = lsJugadores;

                    lsEquipos.Add(equipo);
                }
                return Ok(lsEquipos);
            }
            catch (Exception e)
            {
                var logger = new Logger("TorneoControllerException");
                logger.AgregarMensaje("api/torneo/iequiposPorTorneo" + " Parametros de entrada: " +
                id, " Excepción: " + e.Message + e.StackTrace);
                logger.EscribirLog();
                return BadRequest(e.ToString());
            }

        }
    }
}