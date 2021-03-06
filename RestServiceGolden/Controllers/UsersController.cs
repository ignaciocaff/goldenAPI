﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Cors;
using RestServiceGolden.Models;

namespace RestServiceGolden.Controllers
{
    public class UsersController : ApiController
    {
        private static String representante = "REPRESENTANTE";

        [ResponseType(typeof(Usuario))]
        [Route("api/users/authenticate")]
        public IHttpActionResult Authenticate([FromBody]Usuario user)
        {
            var usuario = new Usuario();
            try
            {
                usuario = (Usuario)this.validation(user.n_usuario, user.password);
                if (usuario == null)
                {
                    return Unauthorized();
                }
                else if (usuario.perfil.n_perfil.ToUpper().Equals(representante))
                {
                    if (usuario.caducidad.Date < DateTime.Now)
                    {
                        return BadRequest("El usuario ha expirado");
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest("Excepción: La BD no responde, intente más tarde");
            }
            // return basic user info (without password)
            return Ok(usuario);
        }
        [ResponseType(typeof(IHttpActionResult))]
        [Route("api/users/logout")]
        public IHttpActionResult GetLogOut()
        {
            return Ok("Usuario deslogueado");
        }
        public Object validation(string n_usuario, string password)
        {
            goldenEntities db = new goldenEntities();

            usuarios usuario;

            if (string.IsNullOrEmpty(n_usuario) || string.IsNullOrEmpty(password))
                return null;
            usuario = db.usuarios.Where(x => x.n_usuario == n_usuario).ToList().FirstOrDefault();
            // check if username exists
            if (usuario == null)
                return null;

            // check if password is correct
            if (usuario.password == null || usuario.password != password)
                return null;

            try
            {
                var usuarios = (from tUsuarios in db.usuarios
                                join tPerfiles in db.perfiles on tUsuarios.id_perfil equals tPerfiles.id_perfil
                                select new
                                {
                                    idUsuario = tUsuarios.id_usuario,
                                    nUsuario = tUsuarios.n_usuario,
                                    passwordU = tUsuarios.password,
                                    nPerfil = tPerfiles.n_perfil,
                                    nCaducidad = tUsuarios.caducidad,
                                    id_perfil = tPerfiles.id_perfil
                                });


                foreach (var u in usuarios)
                {
                    if (u.idUsuario == usuario.id_usuario)
                    {
                        Usuario objUsuario = new Usuario();
                        Perfil objPerfil = new Perfil();

                        objUsuario.id_usuario = u.idUsuario;
                        objUsuario.n_usuario = u.nUsuario;
                        objUsuario.caducidad = Convert.ToDateTime(u.nCaducidad);
                        objPerfil.n_perfil = u.nPerfil;
                        objPerfil.id_perfil = u.id_perfil;
                        objUsuario.perfil = objPerfil;


                        return objUsuario;
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
            return null;
        }

        [ResponseType(typeof(Equipo))]
        [Route("api/user/representante/{id}")]
        public IHttpActionResult getEquipoRepresentante(int id)
        {
            goldenEntities db = new goldenEntities();
            var representante = db.representante_equipo.Where(x => x.id_usuario == id).First();
            var tEquipo = db.equipos.Where(x => x.id_equipo == representante.id_equipo).First();

            Equipo equipo = new Equipo();
            Categoria categoria = new Categoria();
            Torneo torneo = new Torneo();
            Club club = new Club();

            equipo.id_equipo = tEquipo.id_equipo;
            equipo.nombre = tEquipo.nombre;
            equipo.descripcion = tEquipo.descripcion;
            equipo.fecha_alta = Convert.ToDateTime(tEquipo.fecha_alta);
            equipo.logo = tEquipo.logo.Value;
            equipo.categoria = categoria;
            equipo.club = club;
            equipo.torneo = torneo;
            equipo.categoria.id_categoria = (int)tEquipo.id_categoria_equipo;
            equipo.club.id_club = tEquipo.id_club;
            equipo.torneo.id_torneo = tEquipo.id_torneo;

            return Ok(equipo);
        }
    }
}