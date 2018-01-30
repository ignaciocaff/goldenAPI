﻿using RestServiceGolden.Models;
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

    public class PersonaController: ApiController
    {
        goldenEntities db = new goldenEntities();

        [ResponseType(typeof(TipoDocumento))]
        [Route("api/personas/tiposdoc")]
        public IHttpActionResult GetTiposDocumento()
        {
            List<TipoDocumento> lsTiposDocumento= new List<TipoDocumento>();

            var tiposDocumentos = db.tipos_documento.ToList();
            
            foreach (var td in tiposDocumentos)
            {
                TipoDocumento tipoDocumento = new TipoDocumento();
                tipoDocumento.id_tipo_documento = td.id_tipo_documento;
                tipoDocumento.descripcion = td.descripcion;
                lsTiposDocumento.Add(tipoDocumento);
            }
            return Ok(lsTiposDocumento);
        }

        [ResponseType(typeof(Provincia))]
        [Route("api/domicilio/provincias")]
        public IHttpActionResult getProvincias()
        {
            List<Provincia> lsProvincias = new List<Provincia>();
            var provincias = db.provincias.ToList();

            foreach (var p in provincias)
            {
                Provincia provincia = new Provincia();
                provincia.id_provincia = p.id_provincia;
                provincia.n_provincia = p.n_provincia;
                lsProvincias.Add(provincia);
            }
            return Ok(lsProvincias);
        }

        [ResponseType(typeof(Localidad))]
        [Route("api/domicilio/localidades")]
        public IHttpActionResult getLocalidades()
        {
            List<Localidad> lsLocalidades = new List<Localidad>();
            var localidades = db.localidades.ToList();

            foreach (var l in localidades)
            {
                Localidad localidad = new Localidad();
                localidad.id_localidad = l.id_localidad;
                localidad.n_localidad = l.n_localidad;
                lsLocalidades.Add(localidad);
            }
            return Ok(lsLocalidades);
        }


        [ResponseType(typeof(Localidad))]
        [Route("api/domicilio/localidades/{id}")]
        public IHttpActionResult getLocalidadesPorProvincia(int id)
        {
            List<Localidad> lsLocalidades = new List<Localidad>();
            var localidades = db.localidades.Where(x => x.id_provincia == id).ToList();

            foreach (var l in localidades)
            {
                Localidad localidad = new Localidad();
                localidad.id_localidad = l.id_localidad;
                localidad.n_localidad = l.n_localidad;
                lsLocalidades.Add(localidad);
            }
            return Ok(lsLocalidades);
        }
    }
}
