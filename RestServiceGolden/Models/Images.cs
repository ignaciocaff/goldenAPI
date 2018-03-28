using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestServiceGolden.Models
{
    public class Images
    {
        public int? Id { get; set; }
        public String FileName { get; set; }
        public String ImagePath { get; set; }
        public String ThumbPath { get; set; }
        public String ProjectId { get; set; }
        public String SectionId { get; set; }
        public int FileSize { get; set; }
    }
}