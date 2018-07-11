using System;
using Netling.Core;

namespace Game.Requests
{
    public interface IGameRequest
    {
        Request CreateRequest(byte[] previousContent);
    }
}
