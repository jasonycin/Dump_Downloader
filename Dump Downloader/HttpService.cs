﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dump_Downloader
{
    public class HttpService : IDisposable
    {
        private HttpClient _httpClient;
        private bool _isDisposed = false;

        public HttpService()
        {
            _httpClient = new HttpClient();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            _httpClient.Dispose();
        }

        public async Task SendRequest(string url, string saveLocation)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    // Send Request
                    using (Stream output = File.OpenWrite(saveLocation))
                    using (Stream input = await response.Content.ReadAsStreamAsync())
                    {
                        await input.CopyToAsync(output);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"ERROR: The request failed. {response.StatusCode} - {response.ReasonPhrase}");
                    Console.ResetColor();
                }
                // 5s Ratelimit
                await Task.Delay(5000);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nERROR: This file likely does not exist! Moving on...\n");
                Console.ResetColor();
            }
        }
    }
}