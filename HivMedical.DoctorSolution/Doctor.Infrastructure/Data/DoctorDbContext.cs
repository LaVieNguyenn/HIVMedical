using Doctor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Infrastructure.Data
{
    public class DoctorDbContext : DbContext
    {
        public DbSet<Doctors> Doctors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<DoctorQualification> DoctorQualifications { get; set; }
        public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Specialization> Specializations { get; set; }

        public DoctorDbContext(DbContextOptions<DoctorDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).HasColumnName("user_id");
                entity.Property(u => u.FullName).HasColumnName("full_name");
                entity.Property(u => u.Phone).HasColumnName("phone");
                entity.Property(u => u.Email).HasColumnName("email");
            });

            // Doctor
            modelBuilder.Entity<Doctors>(entity =>
            {
                entity.ToTable("doctors");
                entity.HasKey(e => e.DoctorId);
                entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property<DateTime>("created_at");
                entity.Property<int>("created_by");
                entity.Property<DateTime?>("updated_at");
                entity.Property<int?>("updated_by");

                // Navigation tới User
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId);
            });

            // DoctorSchedule
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
                entity.Property<DateTime>("created_at");
                entity.Property<int>("created_by");
                entity.Property<DateTime?>("updated_at");
                entity.Property<int?>("updated_by");
            });

            // Qualification
            modelBuilder.Entity<Qualification>(entity =>
            {
                entity.ToTable("qualifications");
                entity.HasKey(q => q.QualificationId);
                entity.Property(q => q.QualificationId).HasColumnName("qualification_id");
                entity.Property(q => q.Name).HasColumnName("name");
            });

            // Specialization
            modelBuilder.Entity<Specialization>(entity =>
            {
                entity.ToTable("specializations");
                entity.HasKey(s => s.SpecializationId);
                entity.Property(s => s.SpecializationId).HasColumnName("specialization_id");
                entity.Property(s => s.Name).HasColumnName("name");
            });

            // DoctorQualification
            modelBuilder.Entity<DoctorQualification>(entity =>
            {
                entity.ToTable("doctor_qualifications");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
                entity.Property(e => e.QualificationId).HasColumnName("qualification_id");
                entity.Property<DateTime>("created_at");
                entity.Property<int>("created_by");
                entity.Property<DateTime?>("updated_at");
                entity.Property<int?>("updated_by");

                entity.HasOne(dq => dq.Doctor)
                      .WithMany(d => d.Qualifications)
                      .HasForeignKey(dq => dq.DoctorId);

                entity.HasOne(dq => dq.Qualification)
                      .WithMany()
                      .HasForeignKey(dq => dq.QualificationId);
            });

            // DoctorSpecialization
            modelBuilder.Entity<DoctorSpecialization>(entity =>
            {
                entity.ToTable("doctor_specializations");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
                entity.Property(e => e.SpecializationId).HasColumnName("specialization_id");
                entity.Property<DateTime>("created_at");
                entity.Property<int>("created_by");
                entity.Property<DateTime?>("updated_at");
                entity.Property<int?>("updated_by");

                entity.HasOne(ds => ds.Doctor)
                      .WithMany(d => d.Specializations)
                      .HasForeignKey(ds => ds.DoctorId);

                entity.HasOne(ds => ds.Specialization)
                      .WithMany()
                      .HasForeignKey(ds => ds.SpecializationId);
            });
        }
    }
}
