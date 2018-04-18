using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using System.Drawing;
using System.Web.Http.Cors;
using System.Web.Hosting;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using RestServiceGolden.Models;

namespace RestServiceGolden.Controllers
{
    public class ArchivosController : ApiController
    {
        goldenEntities db = new goldenEntities();

        [HttpPost]
        public async Task<HttpResponseMessage> Upload(string projectId, string sectionId)
        {
            var status = new MyReponse();
            try
            {
                var context = HttpContext.Current.Request;
                if (context.Files.Count > 0)
                {
                    var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
                    var index = 0;
                    foreach (var streamContent in filesReadToProvider.Contents)
                    {
                        var fileBytes = await streamContent.ReadAsByteArrayAsync();
                        var file = new files();
                        file.ProjectId = projectId;
                        file.SectionId = sectionId;
                        file.FileName = context.Files[index].FileName;
                        file.FileSize = fileBytes.Length;
                        file.ImagePath = String.Format("/UploadedFiles/{0}_{1}_{2}", projectId, sectionId, file.FileName);
                        file.ThumbPath = String.Format("/UploadedFiles/{0}_{1}_th_{2}", projectId, sectionId, file.FileName);
                        var img = Image.FromStream(new System.IO.MemoryStream(fileBytes));
                        status.subidas.Add(await SaveFiles(file, img));
                        index++;
                    }
                    status.Status = true;
                    status.Message = "File uploaded successfully";
                    return Request.CreateResponse(HttpStatusCode.OK, status);
                }
            }
            catch (Exception ex)
            {
                status.Message = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, status);
        }

        private async Task<int> SaveFiles(files file, Image img)
        {
            // save thumb
            SaveToFolder(img, new Size(160, 160), file.ThumbPath);
            // save image of size max 600 x 600
            if (file.SectionId.Equals("NOTICIAS")) {
                SaveToFolder(img, new Size((int)img.PhysicalDimension.Width, (int)img.PhysicalDimension.Height), file.ImagePath);
            }
            else { 
                SaveToFolder(img, new Size(650, 650), file.ImagePath);
            }
            // Save  to database
            return await Save(file);
        }
        [Route("api/archivos/getimages")]
        [HttpPost]
        public IHttpActionResult getimages([FromBody] string[] subidas)
        {
            List<files> lsMostrar = new List<files>();
            var response = new MyReponse();
            try
            {
                foreach (var id in subidas)
                {
                    int idConvertido = Convert.ToInt32(id);
                    files f = db.files.Where(x => x.Id == idConvertido).FirstOrDefault();
                    lsMostrar.Add(f);
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message, e.InnerException);
            }
            return Ok(lsMostrar);
        }

        [Route("api/archivos/getbytorneo/{id}")]
        [HttpGet]
        public IHttpActionResult getbytorneo(int id)
        {
            var response = new MyReponse();
            List<Images> lsImages = new List<Images>();
            try
            {
                var imgs = (from tImages in db.files
                            join tEquipos in db.equipos on tImages.Id equals tEquipos.logo
                            where tEquipos.id_torneo == id
                            select new
                            {
                                idImg = tImages.Id,
                                fileName = tImages.FileName,
                                thumbPath = tImages.ThumbPath,
                                imagePath = tImages.ImagePath,
                                sectionId = tImages.SectionId
                            });

                foreach (var i in imgs)
                {

                    Images img = new Images();
                    img.Id = i.idImg;
                    img.FileName = i.fileName;
                    img.ThumbPath = i.thumbPath;
                    img.ImagePath = i.imagePath;
                    img.SectionId = i.sectionId;
                    lsImages.Add(img);
                }
                return Ok(lsImages);
            }
            catch (Exception e)
            {
                Console.Write(e.Message, e.InnerException);
                return BadRequest();
            }
        }

        [Route("api/archivos/getbynoticia/{id}")]
        [HttpGet]
        public IHttpActionResult getByNoticia(int id)
        {
            var response = new MyReponse();
            List<Images> lsImages = new List<Images>();
            try
            {
                var imgs = (from tImages in db.files
                            join tNoticias in db.noticias on tImages.Id equals tNoticias.id_thumbnail
                            where tNoticias.id_noticia == id
                            select new
                            {
                                idImg = tImages.Id,
                                fileName = tImages.FileName,
                                thumbPath = tImages.ThumbPath,
                                imagePath = tImages.ImagePath,
                                sectionId = tImages.SectionId
                            });

                foreach (var i in imgs)
                {
                   Images img = new Images();
                   img.Id = i.idImg;
                   img.FileName = i.fileName;
                   img.ThumbPath = i.thumbPath;
                   img.ImagePath = i.imagePath;
                   img.SectionId = i.sectionId;
                   lsImages.Add(img);
                }
                return Ok(lsImages);
            }
            catch (Exception e)
            {
                Console.Write(e.Message, e.InnerException);
                return BadRequest(e.ToString());
            }
        }


        [Route("api/archivos/getbyjugador/{id}")]
        [HttpGet]
        public IHttpActionResult getByJugador(int id)
        {
            var response = new MyReponse();
            List<Images> lsImages = new List<Images>();
            try
            {
                var imgs = (from tImages in db.files
                            join tPersona in db.personas on tImages.Id equals tPersona.id_foto
                            where tPersona.id_persona == id
                            select new
                            {
                                idImg = tImages.Id,
                                fileName = tImages.FileName,
                                thumbPath = tImages.ThumbPath,
                                imagePath = tImages.ImagePath,
                                sectionId = tImages.SectionId
                            });

                foreach (var i in imgs)
                {

                    Images img = new Images();
                    img.Id = i.idImg;
                    img.FileName = i.fileName;
                    img.ThumbPath = i.thumbPath;
                    img.ImagePath = i.imagePath;
                    img.SectionId = i.sectionId;
                    lsImages.Add(img);
                }
                return Ok(lsImages);
            }
            catch (Exception e)
            {
                Console.Write(e.Message, e.InnerException);
                return BadRequest();
            }
        }

        [Route("api/archivos/getbyequipo/{id}")]
        [HttpGet]
        public IHttpActionResult getByEquipo(int id)
        {
            var response = new MyReponse();
            try
            {
                var img = db.files.Where(x => x.Id == id).FirstOrDefault();

                Images image = new Images();
                image.Id = img.Id;
                image.FileName = img.FileName;
                image.ThumbPath = img.ThumbPath;
                image.ImagePath = img.ImagePath;
                image.SectionId = img.SectionId;
                return Ok(image);
            }
            catch (Exception e)
            {
                Console.Write(e.Message, e.InnerException);
                return BadRequest();
            }
        }


        private async Task<int> Save(files file)
        {
            db.files.Add(file);
            await db.SaveChangesAsync();
            int id = file.Id;
            return id;
        }

        private Size NewImageSize(Size imageSize, Size newSize)
        {
            Size finalSize;
            double tempval;
            if (imageSize.Height > newSize.Height || imageSize.Width > newSize.Width)
            {
                if (imageSize.Height > imageSize.Width)
                    tempval = newSize.Height / (imageSize.Height * 1.0);
                else
                    tempval = newSize.Width / (imageSize.Width * 1.0);

                finalSize = new Size((int)(tempval * imageSize.Width), (int)(tempval * imageSize.Height));
            }
            else
                finalSize = imageSize; // image is already small size

            return finalSize;
        }

        private void SaveToFolder(Image img, Size newSize, string pathToSave)
        {
            // Get new resolution
            Size imgSize = NewImageSize(img.Size, newSize);
            using (System.Drawing.Image newImg = new Bitmap(img, imgSize.Width, imgSize.Height))
            {
                // Remove image if already exist and save again
                String currentPath = HttpContext.Current.Server.MapPath(pathToSave);
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(pathToSave)))
                    System.IO.File.Delete(HttpContext.Current.Server.MapPath(pathToSave));

                newImg.Save(HttpContext.Current.Server.MapPath(pathToSave), img.RawFormat);
            }
        }

    }

    public class MyReponse
    {
        public Boolean Status { get; set; }
        public String Message { get; set; }
        public List<int> subidas { get; set; }

        public MyReponse()
        {
            this.subidas = new List<int>();
            this.Status = false;
            this.Message = "Some internal error";
        }
    }
}