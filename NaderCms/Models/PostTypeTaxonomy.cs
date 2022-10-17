using System.Collections.Generic;

namespace NaderCms.Models
{
    public class PostTypeTaxonomy
    {
        public int TaxanomyId { get; set; }
        public Taxonomy Taxonomy { get; set; }

        public int PostTypeId { get; set; }
        public PostType PostType { get; set; }

    }
}
