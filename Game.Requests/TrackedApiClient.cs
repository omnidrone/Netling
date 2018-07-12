using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Netling.Core.Models;
using ProtobufSerializersGenerator;
using WithBuddies.ApiClient;
using WithBuddies.Common;

namespace Netling.Game
{
    public class TrackedApiClient : ApiClient, IHttpClient
    {
        private readonly int _index;
        private readonly WorkerThreadResult _workerThreadResult;
        private readonly string _baseAddress;
        private readonly Stopwatch _stopwatch;
        private readonly Stopwatch _localStopwatch;

        public TrackedApiClient(int index, WorkerThreadResult workerThreadResult, string baseAddress, ApiContext context = null, string defaultQueryString = null) : base(baseAddress, context, defaultQueryString)
        {
            _index = index;
            _workerThreadResult = workerThreadResult;
            _baseAddress = baseAddress;
            _stopwatch = new Stopwatch();
            //_stopwatch.Start();
            _localStopwatch = new Stopwatch();
        }

        public void StartTiming()
        {
            _stopwatch.Start();
        }

        public async Task<TrackedApiClient> NewGuestAsync()
        {
            return await NewGuestAsync(_index, _workerThreadResult, _baseAddress);
        }

        public static async Task<TrackedApiClient> NewGuestAsync(int index, WorkerThreadResult workerThreadResult, string baseAddress)
        {
            ApiClient.SerializationModel = Generator.Generate(new[]
            {
                typeof(UserLoginResponse).GetTypeInfo().Assembly,
                typeof(Picasso.Common.PicassoDto).GetTypeInfo().Assembly
            });
            var unauthenticated = new TrackedApiClient(index, workerThreadResult, baseAddress, null);

            var res = await unauthenticated.PostAsync<UserLoginResponse>("users", new UserCreateRequest
            {
                AutoGenerateName = true,
                SsoToken = Guid.NewGuid().ToString("n"),
            });
            res.AssertSuccessful();
            return new TrackedApiClient(index, workerThreadResult, baseAddress, new ApiContext
            {
                SessionToken = res.Data.SecretKey
            });
        }

        public new async Task<Response> PostAsync(string path, object request) 
        {
            _localStopwatch.Restart();
            Response response = await base.PostAsync(path, request);
            TrackResponse(response);
            return response;
        }

        public new async Task<Response> PutAsync(string path, object request)
        {
            _localStopwatch.Restart();
            Response response = await base.PutAsync(path, request);
            TrackResponse(response);
            return response;
        }

        public new async Task<Response> GetAsync(string path)
        {
            _localStopwatch.Restart();
            Response response = await base.GetAsync(path);
            TrackResponse(response);
            return response;
        }

        public new async Task<Response> GetAsync(string path, object request)
        {
            _localStopwatch.Restart();
            Response response = await base.GetAsync(path, request);
            TrackResponse(response);
            return response;
        }

        public new async Task<Response> DeleteAsync(string path, object request)
        {
            _localStopwatch.Restart();
            Response response = await base.DeleteAsync(path, request);
            TrackResponse(response);
            return response;
        }

        public new async Task<Response> DeleteAsync(string path)
        {
            _localStopwatch.Restart();
            Response response = await base.DeleteAsync(path);
            TrackResponse(response);
            return response;
        }

        public new async Task<Response<T>> PostAsync<T>(string path, object request)
        {
            _localStopwatch.Restart();
            Response<T> response = await base.PostAsync<T>(path, request);
            TrackResponse(response);
            return response;
        }

        public new async Task<Response<T>> PutAsync<T>(string path, object request)
        {
            _localStopwatch.Restart();
            Response<T> response = await base.PutAsync<T>(path, request);
            TrackResponse(response);
            return response;
        }

        public new async Task<Response<T>> GetAsync<T>(string path)
        {
            _localStopwatch.Restart();
            Response<T> response = await base.GetAsync<T>(path);
            TrackResponse(response);
            return response;
        }

        public new async Task<Response<T>> GetAsync<T>(string path, object request)
        {
            _localStopwatch.Restart();
            Response<T> response = await base.GetAsync<T>(path, request);
            TrackResponse(response);
            return response;
        }

        public new async Task<Response<T>> DeleteAsync<T>(string path, object request)
        {
            _localStopwatch.Restart();
            Response<T> response = await base.DeleteAsync<T>(path, request);
            TrackResponse(response);
            return response;
        }

        public new async Task<Response<T>> DeleteAsync<T>(string path)
        {
            _localStopwatch.Restart();
            Response<T> response = await base.DeleteAsync<T>(path);
            TrackResponse(response);
            return response;
        }

        private void TrackResponse(Response response)
        {
            var responseTime = (float)_localStopwatch.ElapsedTicks / Stopwatch.Frequency * 1000;
            var length = 0;//content.Length + response.Headers.ToString().Length + MissingHeaderLength;

            if (response.IsSuccess)
            {
                _workerThreadResult.Add((int)_stopwatch.ElapsedMilliseconds, length, responseTime, _index < 10);
            }

            else
            {
                _workerThreadResult.AddError((int)_stopwatch.ElapsedMilliseconds, responseTime, _index < 10);
            }
        }

        private void TrackResponse<T>(Response<T> response)
        {
            var responseTime = (float)_localStopwatch.ElapsedTicks / Stopwatch.Frequency * 1000;
            var length = 0;//content.Length + response.Headers.ToString().Length + MissingHeaderLength;
            if (response.Data != null)
            {
                //bad, I know :_(
                byte[] bytes = ProtobufUtil.ToProtobuf(response.Data);
                length = bytes.Length;
            }

            if (response.IsSuccess)
            {
                _workerThreadResult.Add((int)_stopwatch.ElapsedMilliseconds, length, responseTime, _index < 10);
            }

            else
            {
                _workerThreadResult.AddError((int)_stopwatch.ElapsedMilliseconds, responseTime, _index < 10);
            }
        }

    }
}
