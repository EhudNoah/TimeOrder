using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace Interop_Test.Interop
{
   internal class Client<T>(string ipAddress, int port) : IDisposable where T : new()
   {
      public string IpAddress { get; private set; } = ipAddress;
      public int Port { get; private set; } = port;

      private readonly HttpClient _httpClient = new();

      public async Task<T?> Shake()
      {
         return await _send(string.Empty);
      }

      public async Task<T?> Send(T obj)
      {
         string message = obj is not null ? JsonSerializer.Serialize(obj) : string.Empty;

         return await _send(message);
      }

      private async Task<T?> _send(string message)
      {
         string url = $"http://{IpAddress}:{Port}/receive/";

         var content = new StringContent(message, Encoding.UTF8, "application/json");

         try
         {
            HttpResponseMessage response = await _httpClient.PostAsync(url, content).ConfigureAwait(false);
            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
               return JsonSerializer.Deserialize<T>(json) ?? new();
            }
            else
            {
               throw new Exception($"Server error: {response.StatusCode}");
            }
         }
         catch (HttpRequestException ex)
         {
            throw new Exception($"Network error: {ex.Message}");
         }
      }

      public void Dispose() => _httpClient.Dispose();
   }
}
