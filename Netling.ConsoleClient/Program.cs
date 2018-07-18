using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine.Options;
using Netling.Core;
using Netling.Core.HttpClientWorker;
using Netling.Core.Models;
using Netling.Core.SocketWorker;
using Netling.Game;

namespace Netling.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleClient client = new ConsoleClient(new FakeRequestSequence(), args);
            client.Start();

        }

    }

    class FakeRequestSequence : IRequestSequence
    {
        public void Register(IHttpClient client)
        {
            throw new NotImplementedException();
        }

        public Task StartSequence()
        {
            throw new NotImplementedException();
        }
    }
}
