using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Images
    {
        public int? Id { get; set; }
        public string FileName { get; set; }
        public string ImagePath { get; set; }
        public string ThumbPath { get; set; }
        public string ProjectId { get; set; }
        public string SectionId { get; set; }
        public int FileSize { get; set; }
    }
}