using System;
using System.Threading.Tasks;

namespace Netling.Game
{
    public interface IRequestSequence
    {
        void Register(IHttpClient client);

        Task StartSequence();
    }
}
