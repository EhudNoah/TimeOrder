using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Interop_Test.Interop
{
   internal class Server<T>(string ipAddress, ushort port) : IDisposable where T : new()
   {
      public string IpAddress { get; private set; } = ipAddress;
      public ushort Port { get; private set; } = port;

      private HttpListener? _listener;
      private CancellationTokenSource? _cts;

      private T _state;

      public async void Run()
      {
         string prefix = $"http://{IpAddress}:{Port}/receive/";

         _listener = new HttpListener();
         _listener.Prefixes.Add(prefix);
         _listener.Start();

         _cts = new CancellationTokenSource();

         _state = new();

         await Task.Run(() => _runListenerAsync(_cts.Token));
      }

      private async Task _runListenerAsync(CancellationToken ct)
      {
         while (!ct.IsCancellationRequested)
         {
            try
            {
               var context = await _listener!.GetContextAsync();
               await _handleRequestAsync(context);
            }
            catch (HttpListenerException) when (ct.IsCancellationRequested)
            {
               break;
            }
            catch (Exception ex)
            {
               Debug.WriteLine($"Listener error: {ex}");
            }
         }
      }

      private async Task _handleRequestAsync(HttpListenerContext context)
      {
         var request = context.Request;
         var response = context.Response;

         if (request.HttpMethod != "POST")
         {
            response.StatusCode = 405;
            response.Close();
            return;
         }

         using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
         string json = await reader.ReadToEndAsync();

         if (!string.IsNullOrEmpty(json))
         {
            var state = JsonSerializer.Deserialize<T>(json);

            if (state is not null)
            {
               _state = state;
            }
         }

         byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_state));
         response.ContentLength64 = buffer.Length;
         await response.OutputStream.WriteAsync(buffer);
         response.Close();
      }

      public void Dispose()
      {
         _cts?.Cancel();
         _listener?.Stop();
         _listener?.Close();
      }
   }
}
