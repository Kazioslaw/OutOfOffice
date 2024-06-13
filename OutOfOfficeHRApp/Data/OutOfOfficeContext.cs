using Microsoft.EntityFrameworkCore;
using OutOfOfficeHRApp.Models;

namespace OutOfOfficeHRApp.Data
{
    public class OutOfOfficeContext : DbContext
    {
        public OutOfOfficeContext(DbContextOptions<OutOfOfficeContext> options) : base(options) { }

        public DbSet<AbsenceReason> AbsenceReason { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequest { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<LeaveRequest> LeaveRequest { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectType> ProjectType { get; set; }
        public DbSet<Subdivision> Subdivision { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AbsenceReason>().HasData(new AbsenceReason { ID = 1, Name = "Illness" },
                                                         new AbsenceReason { ID = 2, Name = "Family Matter" },
                                                         new AbsenceReason { ID = 3, Name = "Official Matter" },
                                                         new AbsenceReason { ID = 4, Name = "Holiday" },
                                                         new AbsenceReason { ID = 5, Name = "Other" });

            modelBuilder.Entity<Position>().HasData(new Position { ID = 1, Name = "Employee" },
                                                    new Position { ID = 2, Name = "HR Manager" },
                                                    new Position { ID = 3, Name = "Project Manager" },
                                                    new Position { ID = 4, Name = "Administrator" });

            modelBuilder.Entity<Subdivision>().HasData(new Subdivision { ID = 1, Name = "Software Development" },
                                                       new Subdivision { ID = 2, Name = "Quality Assurance" },
                                                       new Subdivision { ID = 3, Name = "User Experience/User Interface" },
                                                       new Subdivision { ID = 4, Name = "Product Management" },
                                                       new Subdivision { ID = 5, Name = "Customer Support" },
                                                       new Subdivision { ID = 6, Name = "Buissness Analysis" },
                                                       new Subdivision { ID = 7, Name = "Information Security" },
                                                       new Subdivision { ID = 8, Name = "IT Infractructure" },
                                                       new Subdivision { ID = 9, Name = "Data Management" });

            modelBuilder.Entity<ProjectType>().HasData(new ProjectType { ID = 1, Name = "CMS" },
                                                       new ProjectType { ID = 2, Name = "Mobile E-Commerce" },
                                                       new ProjectType { ID = 3, Name = "CRM" },
                                                       new ProjectType { ID = 4, Name = "ERP" });
        }

    }
}
