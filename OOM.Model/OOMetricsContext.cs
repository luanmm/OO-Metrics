using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OOM.Model
{
    public partial class OOMetricsContext : DbContext
    {
        public OOMetricsContext()
            : this(OOMetricsDBAccessType.FullAccess)
        {

        }

        public OOMetricsContext(OOMetricsDBAccessType accessType)
            : base(accessType == OOMetricsDBAccessType.FullAccess ? "name=OOMDBFullAccess" : "name=OOMDBReadOnly")
        {
            if (!Enum.IsDefined(typeof(OOMetricsDBAccessType), accessType))
                throw new ArgumentOutOfRangeException("accessType", "The informed access type is invalid in this context.");
        }

        public OOMetricsContext(string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer<OOMetricsContext>(new DropCreateDatabaseIfModelChanges<OOMetricsContext>());
            //Database.SetInitializer<OOMetricsContext>(new DropCreateDatabaseAlways<OOMetricsContext>());
        }

        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Revision> Revisions { get; set; }
        public virtual DbSet<Namespace> Namespaces { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Attribute> Attributes { get; set; }
        public virtual DbSet<Method> Methods { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
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
        }
    }

    public enum OOMetricsDBAccessType
    {
        FullAccess,
        ReadOnly
    }
}
