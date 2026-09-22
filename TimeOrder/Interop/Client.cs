using System.Net.Sockets;
using System.Text.Json;

namespace Interop
{
   public sealed class Client<T> : IDisposable where T : new()
   {
      public string IpAddress { get; }
      public int Port { get; }
      public int MaxMessageSize { get; }
      public TimeSpan Timeout { get; }

      public string Url => $"tcp://{IpAddress}:{Port}/";

      public Client(string ipAddress, int port, int maxMessageSize = TcpMessageIO.DefaultMaxMessageSize, TimeSpan? timeout = null)
      {
         if (maxMessageSize <= 0)
         {
            throw new ArgumentOutOfRangeException(nameof(maxMessageSize), "Max message size must be greater than zero.");
         }

         IpAddress = ipAddress;
         Port = port;
         MaxMessageSize = maxMessageSize;
         Timeout = timeout ?? TcpMessageIO.DefaultTimeout;
      }

      public async Task<T?> Shake(CancellationToken cancellationToken = default)
      {
         return await _send(string.Empty, cancellationToken).ConfigureAwait(false);
      }

      public async Task<T?> Send(T obj, CancellationToken cancellationToken = default)
      {
         string message = obj is not null ? JsonSerializer.Serialize(obj) : string.Empty;

         return await _send(message, cancellationToken).ConfigureAwait(false);
      }

      private async Task<T?> _send(string message, CancellationToken cancellationToken)
      {
         using var timeoutCts = TcpMessageIO.CreateTimeoutCts(Timeout, cancellationToken);

         try
         {
            using var client = new TcpClient();
            await client.ConnectAsync(IpAddress, Port, timeoutCts.Token).ConfigureAwait(false);

            await using NetworkStream stream = client.GetStream();

            await TcpMessageIO.WriteLineAsync(stream, message, MaxMessageSize, timeoutCts.Token).ConfigureAwait(false);
            string? json = await TcpMessageIO.ReadLineAsync(stream, MaxMessageSize, timeoutCts.Token).ConfigureAwait(false);

            if (json is null)
            {
               throw new NetworkErrorException("Connection closed by server.");
            }

            InteropResponse<T>? response;
            try
            {
               response = JsonSerializer.Deserialize<InteropResponse<T>>(json);
            }
            catch (JsonException ex)
            {
               throw new NetworkErrorException("Invalid response from server.", ex);
            }

            if (response is null)
            {
               throw new NetworkErrorException("Invalid response from server.");
            }

            if (!response.Ok)
            {
               throw new ServerErrorException(response.Error ?? "Unknown server error.");
            }

            return response.Payload ?? new();
         }
         catch (ServerErrorException)
         {
            throw;
         }
         catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
         {
            throw;
         }
         catch (OperationCanceledException ex)
         {
            throw new NetworkErrorException("The operation timed out.", ex);
         }
         catch (MessageTooLargeException ex)
         {
            throw new NetworkErrorException(ex.Message, ex);
         }
         catch (SocketException ex)
         {
            throw new NetworkErrorException(ex.Message, ex);
         }
         catch (IOException ex)
         {
            throw new NetworkErrorException(ex.Message, ex);
         }
      }

      public void Dispose() { }
   }

   public class NetworkErrorException : Exception
   {
      public NetworkErrorException(string message) : base(message) { }
      public NetworkErrorException(string message, Exception innerException) : base(message, innerException) { }
   }
}
