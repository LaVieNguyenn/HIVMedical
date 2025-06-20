using Patient.Domain.Entities;
using SharedLibrary.Interfaces;

namespace Patient.Infrastructure.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<Appointment>> GetByStatusAsync(string status);
        Task<IEnumerable<Appointment>> GetFilteredAppointmentsAsync(
            int? patientId = null,
            int? doctorId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? status = null,
            string? appointmentType = null,
            int pageNumber = 1,
            int pageSize = 10);
        Task<int> GetTotalFilteredCountAsync(
            int? patientId = null,
            int? doctorId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? status = null,
            string? appointmentType = null);
        Task<Appointment?> GetNextAppointmentByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsByPatientIdAsync(int patientId, int days = 7);
        Task<bool> HasConflictingAppointmentAsync(int doctorId, DateTime appointmentDate, TimeSpan appointmentTime, int? excludeAppointmentId = null);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientAndStatusAsync(int patientId, string status);
        Task<IEnumerable<Appointment>> GetTodayAppointmentsByDoctorAsync(int doctorId);
    }
}
