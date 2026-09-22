using System.Text.Json.Serialization;

namespace Interop
{
   public sealed class InteropResponse<T>
   {
      [JsonPropertyName("ok")]
      public bool Ok { get; init; }

      [JsonPropertyName("error")]
      public string? Error { get; init; }

      [JsonPropertyName("payload")]
      public T? Payload { get; init; }

      public static InteropResponse<T> Success(T payload) => new()
      {
         Ok = true,
         Payload = payload,
      };

      public static InteropResponse<T> Failure(string error) => new()
      {
         Ok = false,
         Error = error,
      };
   }

   public class ServerErrorException(string message) : Exception(message) { }
}
