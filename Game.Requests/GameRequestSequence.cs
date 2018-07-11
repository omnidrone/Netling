using System;
using System.Collections.Generic;
using System.Net.Http;
using Netling.Core;
using Picasso.Common.Game;

namespace Game.Requests
{
    public class GameRequestSequence : IRequestSequence
    {
        private readonly List<IGameRequest> _requests;
        private int _idx = 0;
        private Request _currentRequest;

        public GameRequestSequence()
        {
            _requests = CreateRequestSequence();    
        }
        private List<IGameRequest> CreateRequestSequence()
        {
            return new List<IGameRequest>
            {
                new CreateUser(),
                new StartGame(),
                new StopGame()
            };
        }

        public Request Next() {
            if (_idx >= _requests.Count)
                return null;
            IGameRequest gameRequest = _requests[_idx];
            _idx++;
            Request req;
            if (_currentRequest != null)
            {
                req = gameRequest.CreateRequest(_currentRequest.response);
            }
            else
            {
                req = gameRequest.CreateRequest(null);
            }
            _currentRequest = req;

            return req;    
        }

        private class CreateUser : IGameRequest
        {
            public Request CreateRequest(byte[] previousResponse)
            {
                return new Request
                {

                    url = "http://localhost:5470/users?BypassSignature=true",
                    httpMethod = HttpMethod.Get
                };
            }
        }

        private class StartGame : IGameRequest
        {
            public Request CreateRequest(byte[] previousResponse)
            {
                
                WithBuddies.Common.UserLoginResponse userLoginResponse = ProtobufUtil.ObjectFromProtobuf<WithBuddies.Common.UserLoginResponse>(previousResponse);
                string sessionToken = userLoginResponse.SecretKey;
                return new Request
                {
                    //http://localhost:5470/game/start?BypassSignature=true&SessionToken=2ed1addd-691c-49c4-8205-c0021d64bf40
                    url = $"http://localhost:5470/game/start?BypassSignature=true&SessionToken={sessionToken}",
                    httpMethod = HttpMethod.Post
                };
            }
        }

        private class StopGame : IGameRequest
        {
            public Request CreateRequest(byte[] previousResponse)
            {
                GameStartResponse gameStartResponse = ProtobufUtil.ObjectFromProtobuf<GameStartResponse>(previousResponse);

                GameEndRequest gameEndRequest = new GameEndRequest
                {
                    GameId = gameStartResponse.GameId,
                    Level = 0
                };

                return new Request
                {
                    //http://localhost:5470/game/start?BypassSignature=true&SessionToken=2ed1addd-691c-49c4-8205-c0021d64bf40

                    url = "http://localhost:5470/game/stop",
                    httpMethod = HttpMethod.Post,
                    data = ProtobufUtil.ToProtobuf(gameEndRequest)
                };
            }
        }
    }
}
