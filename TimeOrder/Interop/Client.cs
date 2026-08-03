using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Interop
{
   public sealed class Client<T>(string ipAddress, int port) : IDisposable where T : new()
   {
      public string IpAddress { get; private set; } = ipAddress;
      public int Port { get; private set; } = port;

      public string Url => $"tcp://{IpAddress}:{Port}/";

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
         try
         {
            using var client = new TcpClient();
            await client.ConnectAsync(IpAddress, Port).ConfigureAwait(false);

            await using NetworkStream stream = client.GetStream();
            using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true) { AutoFlush = true };
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);

            await writer.WriteLineAsync(message).ConfigureAwait(false);
            string? json = await reader.ReadLineAsync().ConfigureAwait(false);

            if (json is null)
            {
               throw new NetworkErrorException("Connection closed by server.");
            }

            return JsonSerializer.Deserialize<T>(json) ?? new();
         }
         catch (SocketException ex)
         {
            throw new NetworkErrorException(ex.Message);
         }
         catch (IOException ex)
         {
            throw new NetworkErrorException(ex.Message);
         }
      }

      public void Dispose() { }
   }

   public class NetworkErrorException(string message) : Exception(message) { }
}
