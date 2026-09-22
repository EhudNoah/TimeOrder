using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace Interop
{
   public sealed class Server<T> : IDisposable where T : new()
   {
      public static string IpAddress
         => Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? string.Empty;
      public ushort Port { get; }

      public int MaxMessageSize { get; }
      public TimeSpan Timeout { get; }

      private TcpListener? _listener;
      private CancellationTokenSource? _cts;
      private Task? _listenTask;
      private readonly object _stateLock = new();
      private T _state = new();

      public Server(ushort port, int maxMessageSize = TcpMessageIO.DefaultMaxMessageSize, TimeSpan? timeout = null)
      {
         if (maxMessageSize <= 0)
         {
            throw new ArgumentOutOfRangeException(nameof(maxMessageSize), "Max message size must be greater than zero.");
         }

         Port = port;
         MaxMessageSize = maxMessageSize;
         Timeout = timeout ?? TcpMessageIO.DefaultTimeout;
      }

      public Task RunAsync(CancellationToken cancellationToken = default)
      {
         if (_listenTask is not null)
         {
            throw new InvalidOperationException("Server is already running.");
         }

         _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
         _listener = new TcpListener(IPAddress.Any, Port);
         _listener.Start();

         _listenTask = _runListenerAsync(_cts.Token);
         return _listenTask;
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
         using var timeoutCts = TcpMessageIO.CreateTimeoutCts(Timeout, ct);

         try
         {
            using (client)
            {
               await using NetworkStream stream = client.GetStream();

               string? json = await TcpMessageIO.ReadLineAsync(stream, MaxMessageSize, timeoutCts.Token);
               InteropResponse<T> response = _processRequest(json);

               await TcpMessageIO.WriteLineAsync(
                  stream,
                  JsonSerializer.Serialize(response),
                  MaxMessageSize,
                  timeoutCts.Token);
            }
         }
         catch (OperationCanceledException) when (ct.IsCancellationRequested)
         {
         }
         catch (OperationCanceledException)
         {
            // Per-request timeout: drop the connection.
         }
         catch (MessageTooLargeException)
         {
         }
         catch (IOException)
         {
         }
         catch (SocketException)
         {
         }
      }

      private InteropResponse<T> _processRequest(string? json)
      {
         if (string.IsNullOrEmpty(json))
         {
            return InteropResponse<T>.Success(_snapshot());
         }

         try
         {
            var state = JsonSerializer.Deserialize<T>(json);

            if (state is null)
            {
               return InteropResponse<T>.Failure("Request payload deserialized to null.");
            }

            lock (_stateLock)
            {
               _state = state;
            }

            return InteropResponse<T>.Success(_snapshot());
         }
         catch (JsonException ex)
         {
            return InteropResponse<T>.Failure($"Invalid JSON payload: {ex.Message}");
         }
      }

      private T _snapshot()
      {
         lock (_stateLock)
         {
            return _state;
         }
      }

      public void Dispose()
      {
         _cts?.Cancel();
         _listener?.Stop();
         _cts?.Dispose();
         _cts = null;
      }
   }

   public class ListenerErrorExeption(Exception ex) : Exception(ex.Message, ex) { }
}
