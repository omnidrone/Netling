using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Netling.Core
{
    public class Request
    {
        public string url { get; set; }
        public HttpMethod httpMethod { get; set; }
        public Dictionary<string, string> headers { get; set; }
        public byte[] data { get; set; }
        public byte[] response { get; set; }
    }
}
