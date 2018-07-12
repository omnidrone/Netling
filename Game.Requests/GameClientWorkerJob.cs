using System;
using System.Threading.Tasks;
using Netling.Core;
using Netling.Core.Models;

namespace Netling.Game
{
    public class GameClientWorkerJob : IWorkerJob
    {
        private readonly int _index;
        private readonly string _baseAddress;
        private readonly WorkerThreadResult _workerThreadResult;
        private readonly IRequestSequence _requestSequence;
        private TrackedApiClient _client;

        public GameClientWorkerJob(string baseAddress, IRequestSequence requestSequence)
        {
            _baseAddress = baseAddress;
            _requestSequence = requestSequence;
        }

        private GameClientWorkerJob(string baseAddress, int index, WorkerThreadResult workerThreadResult, IRequestSequence requestSequence, TrackedApiClient client)
        {
            _baseAddress = baseAddress;
            _index = index;
            _workerThreadResult = workerThreadResult;
            _requestSequence = requestSequence;
            _client = client;
            //_gameRequestSequence = new GameRequestSequence(_baseAddress, _index, _workerThreadResult);

        }

        public async Task DoWork()
        {
            _client.StartTiming();
            await _requestSequence.StartSequence();
            //await _gameRequestSequence.StartSequence();
        }

        public WorkerThreadResult GetResults()
        {
            return _workerThreadResult;
        }

        public Task<IWorkerJob> Init(int index, WorkerThreadResult workerThreadResult)
        {
            _client = new TrackedApiClient(index, workerThreadResult, _baseAddress);
            _requestSequence.Register(_client);
            return Task.FromResult<IWorkerJob>(new GameClientWorkerJob(_baseAddress, index, workerThreadResult, _requestSequence, _client));
        }
    }
}
