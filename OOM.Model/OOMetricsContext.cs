using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OOM.Model
{
    public partial class OOMetricsContext : DbContext
    {
        public OOMetricsContext()
            : base("name=OOMetricsContext")
        {
        }

        public virtual DbSet<Node> Nodes { get; set; }
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
                .HasMany(e => e.Nodes)
                .WithRequired(e => e.Revision)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Node>()
                .HasMany(e => e.Classes)
                .WithRequired(e => e.Node)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Revision>()
                .HasMany(e => e.Namespaces)
                .WithRequired(e => e.Revision)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Namespace>()
                .HasMany(e => e.Classes)
                .WithRequired(e => e.Namespace)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.Attributes)
                .WithRequired(e => e.Class)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Class>()
                .HasMany(e => e.Methods)
                .WithRequired(e => e.Class)
                .WillCascadeOnDelete(true);
        }
    }
}
