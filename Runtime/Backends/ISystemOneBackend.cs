using System.Threading;
using System.Threading.Tasks;
using Laya.Unity.Protocol;

namespace Laya.Unity.Backends
{
    public interface ISystemOneBackend
    {
        Task<SystemOneResult> SystemOneAsync(
            SystemOneRequest request,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
