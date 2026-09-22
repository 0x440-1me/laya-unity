using System.Threading;
using System.Threading.Tasks;

namespace Laya.Unity.Transport
{
    public interface ILayaTransport
    {
        Task<LayaTransportResponse> SendAsync(
            LayaTransportRequest request,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
