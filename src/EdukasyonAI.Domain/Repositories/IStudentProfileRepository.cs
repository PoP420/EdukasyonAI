using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Shared.Enums;

namespace EdukasyonAI.Domain.Repositories;

public interface IStudentProfileRepository : IRepository<StudentProfile>
{
    Task<StudentProfile?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentProfile>> GetBySchoolAsync(int schoolId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentProfile>> GetPendingSyncAsync(CancellationToken cancellationToken = default);
}
