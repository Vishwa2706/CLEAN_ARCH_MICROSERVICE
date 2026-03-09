using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Expense.Domain.Models;
using Expense.Application.Contracts;

namespace Expense.Infrastructure.Clients
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDto?> GetUser(int id)
        {
            var response = await _httpClient.GetAsync($"/api/users/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UserDto>();
        }
    }
}
