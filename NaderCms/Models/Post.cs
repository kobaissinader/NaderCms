using NaderCms.InfraStructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace NaderCms.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public DateTime PostCreatoinDate { get; set; }
        public string PostDetials { get; set; }
        public string PostSummary { get; set; }
        public string? VideoUrl { get; set; }

        public int PostTypeId { get; set; }
        public PostType PostType { get; set; }
        public ICollection<PostTerm> PostTerms { get; set; }

        public string Image { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload { get; set; }
    }
}
