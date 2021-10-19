using BenchmarkDotNet.Running;
using HttpClientBenchmark.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System;

namespace HttpClientBenchmark.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<HttpClientRun>();
        }
    }
}
