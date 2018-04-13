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

    public class PersonaController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(TipoDocumento))]
        [Route("api/personas/tiposdoc")]
        public IHttpActionResult GetTiposDocumento()
        {
            try { 
            List<TipoDocumento> lsTiposDocumento = new List<TipoDocumento>();

            var tiposDocumentos = db.tipos_documento.ToList().OrderBy(s => s.descripcion);

            foreach (var td in tiposDocumentos)
            {
                TipoDocumento tipoDocumento = new TipoDocumento();
                tipoDocumento.id_tipo_documento = td.id_tipo_documento;
                tipoDocumento.descripcion = td.descripcion;
                lsTiposDocumento.Add(tipoDocumento);
            }
            return Ok(lsTiposDocumento);
        }
                        
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseType(typeof(Provincia))]
        [Route("api/domicilio/provincias")]
        public IHttpActionResult getProvincias()
        {
            try
            {
                List<Provincia> lsProvincias = new List<Provincia>();
                var provincias = db.provincias.ToList().OrderBy(s => s.n_provincia);

                foreach (var p in provincias)
                {
                    Provincia provincia = new Provincia();
                    List<Localidad> lsLocalidades = new List<Localidad>();

                    var localidades = db.localidades.Where(x => x.id_provincia == p.id_provincia).OrderBy(s => s.n_localidad);

                    provincia.id_provincia = p.id_provincia;
                    provincia.n_provincia = p.n_provincia;

                    foreach (var l in localidades)
                    {
                        Localidad loc = new Localidad();
                        loc.id_localidad = l.id_localidad;
                        loc.n_localidad = l.n_localidad;
                        lsLocalidades.Add(loc);
                    }
                    provincia.lsLocalidades = lsLocalidades;
                    lsProvincias.Add(provincia);
                }
                return Ok(lsProvincias);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [ResponseType(typeof(Localidad))]
        [Route("api/domicilio/localidades")]
        public IHttpActionResult getLocalidades()
        {
            try
            {
                List<Localidad> lsLocalidades = new List<Localidad>();
                var localidades = db.localidades.ToList().OrderBy(s => s.n_localidad);

                foreach (var l in localidades)
                {
                    Localidad localidad = new Localidad();
                    localidad.id_localidad = l.id_localidad;
                    localidad.n_localidad = l.n_localidad;
                    lsLocalidades.Add(localidad);
                }
                return Ok(lsLocalidades);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        [ResponseType(typeof(Localidad))]
        [Route("api/domicilio/localidades/{id}")]
        public IHttpActionResult getLocalidadesPorProvincia(int id)
        {
            try
            {
                List<Localidad> lsLocalidades = new List<Localidad>();
                var localidades = db.localidades.Where(x => x.id_provincia == id).ToList().OrderBy(s => s.n_localidad);

                foreach (var l in localidades)
                {
                    Localidad localidad = new Localidad();
                    localidad.id_localidad = l.id_localidad;
                    localidad.n_localidad = l.n_localidad;
                    lsLocalidades.Add(localidad);
                }
                return Ok(lsLocalidades);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());

            }
        }

        [Route("api/provincia/localidad/{id_prov}")]
        public IHttpActionResult agregarLocalidad(int id_prov, [FromBody]Localidad loc)
        {
            try
            {
                var verif = db.localidades.Where(x => x.n_localidad == loc.n_localidad && x.id_provincia == id_prov).FirstOrDefault();

                if (verif == null)
                {
                    localidades localidad = new localidades();
                    localidad.n_localidad = loc.n_localidad;
                    localidad.fecha_alta = DateTime.Now;
                    localidad.id_provincia = id_prov;
                    db.localidades.Add(localidad);
                    db.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest("Ya existe una localidad con ese nombre para esa provincia.");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.ToString());
            }
        }
    }
}
