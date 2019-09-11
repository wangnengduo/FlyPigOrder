using MeiTuan.Response;
using Newtonsoft.Json;

namespace MeiTuan.Request
{
    public interface IBaseRequest<TResponse> where TResponse : IBaseResponse
    {
        [JsonIgnore]
        string Method { get; }
    }
}
