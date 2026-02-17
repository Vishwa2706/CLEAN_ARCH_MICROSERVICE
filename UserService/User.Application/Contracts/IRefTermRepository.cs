using System.Linq;
using User.Domain.Models;

namespace User.Application.Contracts;

public interface IRefTermRepository
{
    Task<List<string>> GetTermsAsync(string termType);
}
