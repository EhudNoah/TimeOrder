using System.Text;

namespace Interop
{
   internal static class TcpMessageIO
   {
      public const int DefaultMaxMessageSize = 65_536;
      public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

      public static async Task<string?> ReadLineAsync(Stream stream, int maxMessageSize, CancellationToken ct)
      {
         var buffer = new byte[1];
         var bytes = new List<byte>(Math.Min(256, maxMessageSize));

         while (true)
         {
            int read = await stream.ReadAsync(buffer.AsMemory(0, 1), ct).ConfigureAwait(false);
            if (read == 0)
            {
               return bytes.Count == 0 ? null : Encoding.UTF8.GetString(bytes.ToArray());
            }

            byte b = buffer[0];
            if (b == (byte)'\n')
            {
               break;
            }

            if (b == (byte)'\r')
            {
               continue;
            }

            if (bytes.Count >= maxMessageSize)
            {
               throw new MessageTooLargeException(maxMessageSize);
            }

            bytes.Add(b);
         }

         return Encoding.UTF8.GetString(bytes.ToArray());
      }

      public static async Task WriteLineAsync(Stream stream, string message, int maxMessageSize, CancellationToken ct)
      {
         int byteCount = Encoding.UTF8.GetByteCount(message);
         if (byteCount > maxMessageSize)
         {
            throw new MessageTooLargeException(maxMessageSize);
         }

         byte[] payload = Encoding.UTF8.GetBytes(message + "\n");
         await stream.WriteAsync(payload.AsMemory(0, payload.Length), ct).ConfigureAwait(false);
         await stream.FlushAsync(ct).ConfigureAwait(false);
      }

      public static CancellationTokenSource CreateTimeoutCts(TimeSpan timeout, CancellationToken outer = default)
      {
         var cts = CancellationTokenSource.CreateLinkedTokenSource(outer);
         if (timeout > TimeSpan.Zero)
         {
            cts.CancelAfter(timeout);
         }

         return cts;
      }
   }

   public class MessageTooLargeException(int maxMessageSize)
      : Exception($"Message exceeds the maximum allowed size of {maxMessageSize} bytes.")
   {
      public int MaxMessageSize { get; } = maxMessageSize;
   }
}
