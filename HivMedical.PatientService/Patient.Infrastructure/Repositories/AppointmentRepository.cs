using Microsoft.EntityFrameworkCore;
using Patient.Domain.Entities;
using Patient.Infrastructure.Data;
using SharedLibrary.Repository;

namespace Patient.Infrastructure.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(PatientDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Set<Appointment>()
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Set<Appointment>()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Set<Appointment>()
                .Include(a => a.Patient)
                .Where(a => a.AppointmentDate >= fromDate.Date && a.AppointmentDate <= toDate.Date)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByStatusAsync(string status)
        {
            return await _context.Set<Appointment>()
                .Include(a => a.Patient)
                .Where(a => a.Status == status)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetFilteredAppointmentsAsync(
            int? patientId = null,
            int? doctorId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? status = null,
            string? appointmentType = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Set<Appointment>()
                .Include(a => a.Patient)
                .AsQueryable();

            if (patientId.HasValue)
                query = query.Where(a => a.PatientId == patientId.Value);

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            if (fromDate.HasValue)
                query = query.Where(a => a.AppointmentDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(a => a.AppointmentDate <= toDate.Value.Date);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrEmpty(appointmentType))
                query = query.Where(a => a.AppointmentType == appointmentType);

            return await query
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalFilteredCountAsync(
            int? patientId = null,
            int? doctorId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? status = null,
            string? appointmentType = null)
        {
            var query = _context.Set<Appointment>().AsQueryable();

            if (patientId.HasValue)
                query = query.Where(a => a.PatientId == patientId.Value);

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            if (fromDate.HasValue)
                query = query.Where(a => a.AppointmentDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(a => a.AppointmentDate <= toDate.Value.Date);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrEmpty(appointmentType))
                query = query.Where(a => a.AppointmentType == appointmentType);

            return await query.CountAsync();
        }

        public async Task<Appointment?> GetNextAppointmentByPatientIdAsync(int patientId)
        {
            var now = DateTime.Now;
            return await _context.Set<Appointment>()
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patientId && 
                           a.Status == "Scheduled" &&
                           (a.AppointmentDate > now.Date || 
                            (a.AppointmentDate == now.Date && a.AppointmentTime > now.TimeOfDay)))
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsByPatientIdAsync(int patientId, int days = 7)
        {
            var fromDate = DateTime.Now.Date;
            var toDate = fromDate.AddDays(days);

            return await _context.Set<Appointment>()
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patientId &&
                           a.Status == "Scheduled" &&
                           a.AppointmentDate >= fromDate &&
                           a.AppointmentDate <= toDate)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<bool> HasConflictingAppointmentAsync(int doctorId, DateTime appointmentDate, TimeSpan appointmentTime, int? excludeAppointmentId = null)
        {
            var query = _context.Set<Appointment>()
                .Where(a => a.DoctorId == doctorId &&
                           a.AppointmentDate == appointmentDate.Date &&
                           a.AppointmentTime == appointmentTime &&
                           a.Status != "Cancelled");

            if (excludeAppointmentId.HasValue)
                query = query.Where(a => a.Id != excludeAppointmentId.Value);

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAndStatusAsync(int patientId, string status)
        {
            return await _context.Set<Appointment>()
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patientId && a.Status == status)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetTodayAppointmentsByDoctorAsync(int doctorId)
        {
            var today = DateTime.Now.Date;
            return await _context.Set<Appointment>()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && 
                           a.AppointmentDate == today &&
                           a.Status != "Cancelled")
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }
    }
}
