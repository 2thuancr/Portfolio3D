using System.Threading.Tasks;

namespace Portfolio3D.Data;

public interface IPortfolio3DDbSchemaMigrator
{
    Task MigrateAsync();
}
