using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace NaderCms.Models
{
    public class Term
    {
        public int TermId { get; set; }
        public string TermName { get; set; }
        public string TermCode { get; set; }
        [DisplayName("Taxonomy Name")]
        public int TaxonomyId { get; set; }
        public Taxonomy Taxonomy { get; set; }
        public ICollection<PostTerm> PostTerms { get; set; }


    }
}
