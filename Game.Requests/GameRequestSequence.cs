using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Netling.Core;
using Netling.Core.Models;
using Picasso.Common.Balancing;
using Picasso.Common.Constants;
using Picasso.Common.Game;
using WithBuddies.ApiClient;
using WithBuddies.Common;

namespace Netling.Game
{
    public abstract class GameRequestSequence
    {
        private readonly int _index;
        private readonly WorkerThreadResult _workerThreadResult;
        private readonly string _baseAddress;

        protected GameRequestSequence(string baseAddress, int index, WorkerThreadResult workerThreadResult)
        {
            _baseAddress = baseAddress;
            _index = index;
            _workerThreadResult = workerThreadResult;
        }

        public async Task StartSequence()
        {
            var client = await TrackedApiClient.NewGuestAsync(_index, _workerThreadResult, _baseAddress);

            var gameStartResponse = await client.PostAsync<GameStartResponse>($"game/start", new GameStartRequest
            {
                Mode = ControlMode.Smart,
                Goals = new List<GoalDto>
                {
                    new GoalDto
                    {
                        Name = "Destroy",
                        Required = 10
                    }
                }
            });

            var gameStopResponse = await client.PostAsync<EndGameResponse>($"game/stop", new GameEndRequest
            {
                GameId = gameStartResponse.Data.GameId,
                Level = 1
            });

        }

    }
}
