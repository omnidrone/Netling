using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Netling.Core.Models;

namespace Netling.Core.HttpClientWorker
{
    public class HttpClientWorkerJob : IWorkerJob
    {
        private readonly int _index;
        private readonly Uri _uri;
        private readonly Stopwatch _stopwatch;
        private readonly Stopwatch _localStopwatch;
        private readonly WorkerThreadResult _workerThreadResult;
        private readonly HttpClient _httpClient;
        private readonly IRequestSequence _requestSequence;

        // Used to approximately calculate bandwidth
        private static readonly int MissingHeaderLength = "HTTP/1.1 200 OK\r\nContent-Length: 123\r\nContent-Type: text/plain\r\n\r\n".Length; 

        public HttpClientWorkerJob(Uri uri, IRequestSequence requestSequence)
        {
            _uri = uri;
            _requestSequence = requestSequence;
        }

        private HttpClientWorkerJob(int index, Uri uri, IRequestSequence requestSequence, WorkerThreadResult workerThreadResult)
        {
            _index = index;
            _uri = uri;
            _requestSequence = requestSequence;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            _localStopwatch = new Stopwatch();
            _workerThreadResult = workerThreadResult;
            _httpClient = new HttpClient();
        }

        public async Task DoWork()
        {
            var req = _requestSequence.Next();
            while (req != null)
            {
                Uri uri = null;

                if (!Uri.TryCreate(req.url, UriKind.Absolute, out uri))
                    Console.WriteLine("Failed to parse URL");

                _localStopwatch.Restart();

                using (var response = await _httpClient.GetAsync(uri))
                {
                    var content = await response.Content.ReadAsByteArrayAsync();//.Content.ReadAsStreamAsync();
                    var length = content.Length + response.Headers.ToString().Length + MissingHeaderLength;
                    var responseTime = (float)_localStopwatch.ElapsedTicks / Stopwatch.Frequency * 1000;

                    if ((int)response.StatusCode < 400)
                    {
                        _workerThreadResult.Add((int)_stopwatch.ElapsedMilliseconds, length, responseTime, _index < 10);
                        req.response = content;
                        //string resp = Encoding.UTF8.GetString(req.response);
                        req = _requestSequence.Next();
                    }

                    else
                    {
                        _workerThreadResult.AddError((int)_stopwatch.ElapsedMilliseconds, responseTime, _index < 10);
                        req = null;
                    }
                }
            }
        }

        public WorkerThreadResult GetResults()
        {
            return _workerThreadResult;
        }

        public Task<IWorkerJob> Init(int index, WorkerThreadResult workerThreadResult)
        {
            return Task.FromResult<IWorkerJob>(new HttpClientWorkerJob(index, _uri, _requestSequence, workerThreadResult));
        }
    }
}