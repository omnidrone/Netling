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
            IRequestSequence requestSequence = new GameSessionSequence();
            ConsoleClient consoleClient = new ConsoleClient(requestSequence, args);
            consoleClient.Start();
        }
    }
}
