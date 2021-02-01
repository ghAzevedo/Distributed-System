using Shared.Data;
using System.Threading.Tasks;

namespace RetailInMotion.Data.Repository
{
    public class RepositoryBase
    {
        protected readonly TransactionalWrapper _connectionnWrapper;

        public RepositoryBase(TransactionalWrapper connection)
        {
            _connectionnWrapper = connection;
        }

        public async Task CommitTransaction()
        {
            await _connectionnWrapper.CommitTransactionAsync();
        }

    }
}
