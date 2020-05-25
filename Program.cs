﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using XProxy.ServerList;
using XProxy.Proxy;
using Utf8Json;

namespace XProxy
{
    class Program
    {
        static ServerConsole sc;
        static void Main(string[] args)
        {
            sc = new ServerConsole();
            sc.RunServer();
            try
            {
                var configJson = System.IO.File.ReadAllText("config.json");

                var configs = JsonSerializer.Deserialize<Dictionary<string, ProxyConfig>>(configJson);


                Task.WhenAll(configs.Select(c =>
                {
                    if (c.Value.protocol == "udp")
                    {
                        try
                        {
                            var proxy = new UdpProxy();
                            return proxy.Start(c.Value.forwardIp, c.Value.forwardPort, c.Value.localPort, c.Value.localIp);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to start {c.Key} : {ex.Message}");
                            throw ex;
                        }
                    }
                    else if (c.Value.protocol == "tcp")
                    {
                        try
                        {
                            var proxy = new TcpProxy();
                            return proxy.Start(c.Value.forwardIp, c.Value.forwardPort, c.Value.localPort, c.Value.localIp);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to start {c.Key} : {ex.Message}");
                            throw ex;
                        }
                    }
                    else
                    {
                        return Task.FromException(new InvalidOperationException($"procotol not supported {c.Value.protocol}"));
                    }
                })).Wait();




            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured : {ex}");
            }
        }
    }

    public class ProxyConfig
    {
        public string protocol { get; set; }
        public ushort localPort { get; set; }
        public string localIp { get; set; }
        public string forwardIp { get; set; }
        public ushort forwardPort { get; set; }
    }
}