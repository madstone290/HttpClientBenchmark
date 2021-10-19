using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using HttpClientBenchmark.Api;
using HttpClientBenchmark.ConsoleApp.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientBenchmark.ConsoleApp
{
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    //[NativeMemoryProfiler]
    [RPlotExporter]
    [HtmlExporter]
    //[PlainExporter]
    public class HttpClientRun
    {
        private static readonly HttpClient httpClient;
        private const int N = 10;

        //[Params("api/values/long", "api/values/middle", "api/values/short")]
        [Params("api/values/short")]
        public string Route { get; set; }

        static HttpClientRun()
        {
            httpClient = new WebApplicationFactory<Startup>().CreateClient();
        }

        [Benchmark]
        public async Task GetStringBenchAsync()
        {
            for (int i = 0; i < N; i++)
            {
                await GetStringAsync();
            }
        }

        [Benchmark]
        public async Task ReadAsStringBenchAsync()
        {
            for (int i = 0; i < N; i++)
            {
                await ReadAsStringAsync();
            }
        }


        [Benchmark]
        public async Task ReadAsStreamBenchAsync()
        {
            for (int i = 0; i < N; i++)
            {
                await ReadAsStreamAsync();
            }
        }

        [Benchmark]
        public async Task ReadAsStreamOptimizedBenchAsync()
        {
            for (int i = 0; i < N; i++)
            {
                await ReadAsStreamOptimizedAsync();
            }
        }

        public async Task<List<Person>> GetStringAsync()
        {
            var content = await httpClient.GetStringAsync(Route);
            return JsonConvert.DeserializeObject<List<Person>>(content);
        }

        public async Task<List<Person>> ReadAsStringAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, Route))
            using (var response = await httpClient.SendAsync(request, CancellationToken.None))
            {
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                   return JsonConvert.DeserializeObject<List<Person>>(content);

                throw new Exception(new { StatusCode = (int)response.StatusCode, Content = content }.ToString());
            }
        }

        public async Task<List<Person>> ReadAsStreamAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, Route))
            using (var response = await httpClient.SendAsync(request, CancellationToken.None))
            {
                var stream = await response.Content.ReadAsStreamAsync();

                if (response.IsSuccessStatusCode)
                    return DeserializeJsonFromStream<List<Person>>(stream);

                var content = await StreamToStringAsync(stream);
                throw new Exception(new { StatusCode = (int)response.StatusCode, Content = content }.ToString());
            }
        }

        public async Task<List<Person>> ReadAsStreamOptimizedAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, Route))
            using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None))
            {
                var stream = await response.Content.ReadAsStreamAsync();

                if (response.IsSuccessStatusCode)
                    return DeserializeJsonFromStream<List<Person>>(stream);

                var content = await StreamToStringAsync(stream);
                throw new Exception(new { StatusCode = (int)response.StatusCode, Content = content }.ToString());
            }
        }

        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default(T);

            using (var sr = new StreamReader(stream))
            using (var jtr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();
                var searchResult = js.Deserialize<T>(jtr);
                return searchResult;
            }
        }

        private static async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = null;

            if (stream != null)
            {
                using (var sr = new StreamReader(stream))
                {
                    content = await sr.ReadToEndAsync();
                }
            }
            return content;
        }
    }
}
