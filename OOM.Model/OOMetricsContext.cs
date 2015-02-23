using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OOM.Model
{
    public partial class OOMetricsContext : DbContext
    {
        public OOMetricsContext()
            : this("name=OOMDB")
        {

        }

        public OOMetricsContext(string connectionString)
            : base(connectionString)
        {
            //Database.SetInitializer<OOMetricsContext>(new CreateDatabaseIfNotExists<OOMetricsContext>());
            Database.SetInitializer<OOMetricsContext>(new DropCreateDatabaseAlways<OOMetricsContext>());
        }

        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Revision> Revisions { get; set; }
        public virtual DbSet<Namespace> Namespaces { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Attribute> Attributes { get; set; }
        public virtual DbSet<Method> Methods { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("OOM");

            modelBuilder.Entity<Project>()
                .HasMany(e => e.Revisions)
                .WithRequired(e => e.Project)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Revision>()
                .HasMany(e => e.Namespaces)
                .WithRequired(e => e.Revision)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Namespace>()
                .HasMany(e => e.Classes)
                .WithRequired(e => e.Namespace)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.Attributes)
                .WithRequired(e => e.Class)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.Methods)
                .WithRequired(e => e.Class)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Method>()
                .HasMany(e => e.ReferencedAttributes)
                .WithMany(e => e.ReferencingMethods)
                .Map(m =>
                {
                    m.MapLeftKey("MethodId");
                    m.MapRightKey("AttributeId");
                    m.ToTable("MethodAttribute");
                });
        }
    }
}
