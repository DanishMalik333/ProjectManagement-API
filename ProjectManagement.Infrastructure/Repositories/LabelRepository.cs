using Microsoft.EntityFrameworkCore;
using ProjectManagement.Core.Entities;
using ProjectManagement.Core.Interfaces;
using ProjectManagement.Infrastructure.Persistence;

namespace ProjectManagement.Infrastructure.Repositories;

public class LabelRepository : GenericRepository<Label>, ILabelRepository
{
    public LabelRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Label>> GetByProjectAsync(Guid projectId, CancellationToken ct = default) =>
        await _context.Labels.Where(l => l.ProjectId == projectId).ToListAsync(ct);

    public async Task<Label?> GetByIdAndProjectAsync(Guid id, Guid projectId, CancellationToken ct = default) =>
        await _context.Labels.FirstOrDefaultAsync(l => l.Id == id && l.ProjectId == projectId, ct);
}
