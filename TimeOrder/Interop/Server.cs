using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Interop
{
   public sealed class Server<T>(string ipAddress, ushort port) : IDisposable where T : new()
   {
      public static string IpAddress
         => Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? string.Empty;

      public ushort Port { get; private set; } = port;

      public string Url => $"tcp://{ipAddress}:{Port}/";

      private TcpListener? _listener;
      private CancellationTokenSource? _cts;
      private readonly object _stateLock = new();
      private T _state = new();

      public async void Run()
      {
         _listener = new TcpListener(IPAddress.Any, Port);
         _listener.Start();

         _cts = new CancellationTokenSource();

         await Task.Run(() => _runListenerAsync(_cts.Token));
      }

      private async Task _runListenerAsync(CancellationToken ct)
      {
         while (!ct.IsCancellationRequested)
         {
            try
            {
               TcpClient client = await _listener!.AcceptTcpClientAsync(ct);
               _ = _handleClientAsync(client, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
               break;
            }
            catch (ObjectDisposedException) when (ct.IsCancellationRequested)
            {
               break;
            }
            catch (SocketException) when (ct.IsCancellationRequested)
            {
               break;
            }
            catch (Exception ex)
            {
               throw new ListenerErrorExeption(ex);
            }
         }
      }

      private async Task _handleClientAsync(TcpClient client, CancellationToken ct)
      {
         try
         {
            using (client)
            {
               await using NetworkStream stream = client.GetStream();
               using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
               using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true) { AutoFlush = true };

               string? json = await reader.ReadLineAsync(ct);

               if (!string.IsNullOrEmpty(json))
               {
                  var state = JsonSerializer.Deserialize<T>(json);

                  if (state is not null)
                  {
                     lock (_stateLock)
                     {
                        _state = state;
                     }
                  }
               }

               T snapshot;
               lock (_stateLock)
               {
                  snapshot = _state;
               }

               await writer.WriteLineAsync(JsonSerializer.Serialize(snapshot));
            }
         }
         catch (OperationCanceledException) when (ct.IsCancellationRequested)
         {
         }
         catch (IOException)
         {
         }
         catch (SocketException)
         {
         }
      }

      public void Dispose()
      {
         _cts?.Cancel();
         _listener?.Stop();
      }
   }

   public class ListenerErrorExeption(Exception ex) : Exception(ex.Message, ex) { }
}
