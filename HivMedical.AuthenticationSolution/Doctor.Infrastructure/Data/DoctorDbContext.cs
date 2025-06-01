using Doctor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Data
{
    public class DoctorDbContext : DbContext
    {
        public DbSet<Doctors> Doctors => Set<Doctors>();
        public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
        public DbSet<DoctorQualification> DoctorQualifications => Set<DoctorQualification>();
        public DbSet<DoctorSpecialization> DoctorSpecializations => Set<DoctorSpecialization>();

        public DoctorDbContext(DbContextOptions<DoctorDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctors>(entity =>
            {
                entity.ToTable("doctors");
                entity.HasKey(e => e.DoctorId);

                entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.FullName).HasColumnName("full_name");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Email).HasColumnName("email");
            });

            modelBuilder.Entity<DoctorSchedule>(entity =>
            {
                entity.ToTable("doctor_schedules");
                entity.HasKey(e => e.ScheduleId);

                entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
                entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
                entity.Property(e => e.Weekday).HasColumnName("weekday");
                entity.Property(e => e.StartTime).HasColumnName("start_time");
                entity.Property(e => e.EndTime).HasColumnName("end_time");
                entity.Property(e => e.Location).HasColumnName("location");
                entity.Property(e => e.Note).HasColumnName("note");
            });

            modelBuilder.Entity<DoctorQualification>(entity =>
            {
                entity.ToTable("doctor_qualifications");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
                entity.Property(e => e.Name).HasColumnName("name");
            });

            modelBuilder.Entity<DoctorSpecialization>(entity =>
            {
                entity.ToTable("doctor_specializations");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
                entity.Property(e => e.Name).HasColumnName("name");
            });
        }
    }
}
