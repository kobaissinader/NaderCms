using NaderCms.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace NaderCms.Infrastructure
{
    public class NaderCmsContext : IdentityDbContext<AppUser>
    {
        public NaderCmsContext(DbContextOptions<NaderCmsContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<PostTypeTaxonomy>()
                .HasKey(x => new { x.TaxanomyId, x.PostTypeId });

            builder.Entity<PostTypeTaxonomy>()
                .HasOne(x => x.Taxonomy)
                .WithMany(x => x.PostTypeTaxonomies)
                .HasForeignKey(x => x.TaxanomyId);

            builder.Entity<PostTypeTaxonomy>()
                .HasOne(x => x.PostType)
                .WithMany(x => x.PostTypeTaxonomies)
                .HasForeignKey(x => x.PostTypeId);

            builder.Entity<PostTerm>()
               .HasKey(x => new { x.PostId, x.TermId });

            builder.Entity<PostTerm>()
                .HasOne(x => x.Post)
                .WithMany(x => x.PostTerms)
                .HasForeignKey(x => x.PostId);

            builder.Entity<PostTerm>()
                .HasOne(x => x.Term)
                .WithMany(x => x.PostTerms)
                .HasForeignKey(x => x.TermId);

            builder.Entity<Taxonomy>()
                .HasIndex(x => x.TaxonomyName)
                .IsUnique();
        }

        public DbSet<Taxonomy> Taxonomies { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<PostType> PostTypes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostTypeTaxonomy> PostTypeTaxonomies { get; set; }
        public DbSet<PostTerm> PostTerms { get; set; }
    }
}
