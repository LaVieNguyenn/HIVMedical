using Microsoft.EntityFrameworkCore;
using Patient.Domain.Entities;

namespace Patient.Infrastructure.Data
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
        {
        }

        public DbSet<Domain.Entities.Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<PatientMedication> PatientMedications { get; set; }
        public DbSet<MedicationSchedule> MedicationSchedules { get; set; }
        public DbSet<MedicationAdherence> MedicationAdherences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Patient configuration
            modelBuilder.Entity<Domain.Entities.Patient>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PatientCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.EmergencyContact).HasMaxLength(200);
                entity.Property(e => e.EmergencyPhone).HasMaxLength(20);
                entity.Property(e => e.HIVStatus).HasMaxLength(50);
                entity.Property(e => e.TreatmentStatus).HasMaxLength(100);

                entity.HasIndex(e => e.PatientCode).IsUnique();
                entity.HasIndex(e => e.AuthUserId).IsUnique();
            });

            // MedicalRecord configuration
            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RecordType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();

                entity.HasOne(e => e.Patient)
                      .WithMany(p => p.MedicalRecords)
                      .HasForeignKey(e => e.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Appointment configuration
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AppointmentType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.HasOne(e => e.Patient)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(e => e.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Medication configuration
            modelBuilder.Entity<Medication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.GenericName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.BrandName).HasMaxLength(200);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MedicationType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Strength).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Form).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.SideEffects).HasMaxLength(2000);
                entity.Property(e => e.Contraindications).HasMaxLength(2000);
                entity.Property(e => e.DrugInteractions).HasMaxLength(2000);
                entity.Property(e => e.StorageInstructions).HasMaxLength(1000);

                entity.HasIndex(e => e.Name).IsUnique();
            });

            // PatientMedication configuration
            modelBuilder.Entity<PatientMedication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Dosage).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Frequency).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Instructions).HasMaxLength(1000);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DiscontinuationReason).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.AdherencePercentage).HasColumnType("decimal(5,2)");

                entity.HasOne(e => e.Patient)
                      .WithMany(p => p.PatientMedications)
                      .HasForeignKey(e => e.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Medication)
                      .WithMany(m => m.PatientMedications)
                      .HasForeignKey(e => e.MedicationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // MedicationSchedule configuration
            modelBuilder.Entity<MedicationSchedule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DayOfWeek).IsRequired().HasMaxLength(20);
                entity.Property(e => e.SpecialInstructions).HasMaxLength(500);

                entity.HasOne(e => e.PatientMedication)
                      .WithMany(pm => pm.MedicationSchedules)
                      .HasForeignKey(e => e.PatientMedicationId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Medication)
                      .WithMany(m => m.MedicationSchedules)
                      .HasForeignKey(e => e.MedicationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Patient)
                      .WithMany(p => p.MedicationSchedules)
                      .HasForeignKey(e => e.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // MedicationAdherence configuration
            modelBuilder.Entity<MedicationAdherence>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.SideEffectsReported).HasMaxLength(1000);

                entity.HasOne(e => e.PatientMedication)
                      .WithMany(pm => pm.MedicationAdherences)
                      .HasForeignKey(e => e.PatientMedicationId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Patient)
                      .WithMany(p => p.MedicationAdherences)
                      .HasForeignKey(e => e.PatientId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Medication)
                      .WithMany()
                      .HasForeignKey(e => e.MedicationId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
