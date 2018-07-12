using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Netling.Game;
using Picasso.Common.Balancing;
using Picasso.Common.Constants;
using Picasso.Common.Game;

namespace Netling.ConsoleClient
{
    public class GameSessionSequence : IRequestSequence
    {
        private IHttpClient _client;

        public void Register(IHttpClient client)
        {
            _client = client;
        }

        public async Task StartSequence()
        {
            _client = await _client.NewGuestAsync();
            var gameStartResponse = await _client.PostAsync<GameStartResponse>($"game/start", new GameStartRequest
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

            var gameStopResponse = await _client.PostAsync<EndGameResponse>($"game/stop", new GameEndRequest
            {
                GameId = gameStartResponse.Data.GameId,
                Level = 1
            });
        }
    }
}
