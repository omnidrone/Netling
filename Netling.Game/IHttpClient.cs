using System;
using System.Threading.Tasks;
using WithBuddies.Common;

namespace Netling.Game
{
    public interface IHttpClient
    {
        Task<TrackedApiClient> NewGuestAsync();

        Task<Response> PostAsync(string path, object request);

        Task<Response> PutAsync(string path, object request);

        Task<Response> GetAsync(string path);

        Task<Response> GetAsync(string path, object request);

        Task<Response> DeleteAsync(string path, object request);

        Task<Response> DeleteAsync(string path);

        Task<Response<T>> PostAsync<T>(string path, object request);

        Task<Response<T>> PutAsync<T>(string path, object request);

        Task<Response<T>> GetAsync<T>(string path);

        Task<Response<T>> GetAsync<T>(string path, object request);

        Task<Response<T>> DeleteAsync<T>(string path, object request);

        Task<Response<T>> DeleteAsync<T>(string path);
    }
}
