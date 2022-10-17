using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace NaderCms.Models
{
    public class Taxonomy
    {
        public int TaxonomyId { get; set; }
        public string TaxonomyName { get; set; }
        public string TaxonomyCode { get; set; }
        public ICollection<Term> Terms { get; set; }
        public ICollection<PostTypeTaxonomy> PostTypeTaxonomies { get; set; }

    }
}