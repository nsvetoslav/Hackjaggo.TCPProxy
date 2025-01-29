﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hackjaggo.Proxy
{
    public class ProxyConfig
    {
        public ushort? localPort { get; set; }
        public string? localIp { get; set; }
        public string? forwardIp { get; set; }
        public ushort? forwardPort { get; set; }
        public bool FilterIPAddressRanges { get; set; }
        public List<string> IPAddressRanges { get; set; } = new();
    }
}
