namespace OOM.Model.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OOM.Model.OOMetricsContext>
    {
        private readonly string[] _readOnlyTablesAccess = new string[] { "Attribute", "Class", "Method", "Namespace", "Project", "Revision" };

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(OOMetricsContext context)
        {
            if (!IsReadOnlyUserCreated(context))
                throw new Exception("The read-only user [raf_ro] could not be found in the database. Configure this user before proceed.");

            foreach (var table in _readOnlyTablesAccess)
                context.Database.ExecuteSqlCommand(String.Format("GRANT SELECT ON [{0}] TO [raf_ro]", table));

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }

        private bool IsReadOnlyUserCreated(OOMetricsContext context)
        {
            var readOnlyUser = context.Database.SqlQuery<string>("SELECT Name FROM sys.database_principals WHERE Name = 'raf_ro'").FirstOrDefault();
            return !String.IsNullOrEmpty(readOnlyUser);
        }
    }
}
