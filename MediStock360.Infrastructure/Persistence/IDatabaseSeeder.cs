using System.Threading.Tasks;

namespace MediStock360.Infrastructure.Persistence
{
    public interface IDatabaseSeeder
    {
        Task SeedAsync();
    }
}

