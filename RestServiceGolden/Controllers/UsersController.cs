using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Cors;
using RestServiceGolden.Models;

namespace RestServiceGolden.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UsersController : ApiController
    {
        [ResponseType(typeof(Usuario))]
        [Route("api/users/authenticate")]
        public IHttpActionResult Authenticate([FromBody]Usuario user)
        {
            var usuario = this.validation(user.n_usuario, user.password);

            if (usuario == null)
                return Unauthorized();

            // return basic user info (without password)
            return Ok(usuario);
        }

        public Usuario validation(string n_usuario, string password)
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

            Usuario userDto = new Usuario();
            userDto.n_usuario = usuario.n_usuario;
            userDto.password = usuario.password;
            return userDto;
        }
    }
}

