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
            //Database.SetInitializer<OOMetricsContext>(new DropCreateDatabaseAlways<OOMetricsContext>());
            Database.SetInitializer<OOMetricsContext>(null);
        }

        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Revision> Revisions { get; set; }
        public virtual DbSet<Namespace> Namespaces { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<Method> Methods { get; set; }
        public virtual DbSet<Metric> Metrics { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("OOM");

            modelBuilder.Entity<Revision>()
                .HasRequired(e => e.Project)
                .WithMany(e => e.Revisions)
                .HasForeignKey(e => e.ProjectId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Namespace>()
                .HasRequired(e => e.Revision)
                .WithMany(e => e.Namespaces)
                .HasForeignKey(e => e.RevisionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Class>()
                .HasRequired(e => e.Namespace)
                .WithMany(e => e.Classes)
                .HasForeignKey(e => e.NamespaceId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Class>()
                .HasOptional(e => e.BaseClass)
                .WithMany(e => e.ChildClasses)
                .HasForeignKey(e => e.BaseClassId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Field>()
                .HasRequired(e => e.Class)
                .WithMany(e => e.Fields)
                .HasForeignKey(e => e.ClassId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Method>()
                .HasRequired(e => e.Class)
                .WithMany(e => e.Methods)
                .HasForeignKey(e => e.ClassId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Method>()
                .HasMany(e => e.ReferencedFields)
                .WithMany(e => e.ReferencingMethods)
                .Map(m =>
                {
                    m.MapLeftKey("MethodId");
                    m.MapRightKey("FieldId");
                    m.ToTable("MethodField");
                });
        }
    }
}
