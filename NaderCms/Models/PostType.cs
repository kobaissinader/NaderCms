using System.Collections.Generic;

namespace NaderCms.Models
{
    public class PostType
    {
        public int PostTypeId { get; set; }
        public string PostTypeTitle { get; set; }
        public string PostTypeCode { get; set; }
        public ICollection<PostTypeTaxonomy> PostTypeTaxonomies { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
