using System.Linq;
using Microsoft.EntityFrameworkCore;
using User.Application.Contracts;
using User.Domain.Models;
using User.Infrastructure.Repository;

namespace User.Infrastructure.Service;

public class RefTermRepository : IRefTermRepository
{
    private readonly UserRepository _context;

    public RefTermRepository(UserRepository context)
    {
        _context = context;
    }

    public async Task<List<string>> GetTermsAsync(string termType)
    {
        return await _context
            .RefTerms.Where(x => x.TermType == termType && x.IsActive)
            .Select(x => x.TermValue)
            .ToListAsync();
    }
}
