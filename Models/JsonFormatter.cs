using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SignalMan.Models
{
    class JsonFormatter
    {
        private readonly JsonWriterOptions _writerOptions;

        public JsonFormatter()
        {
            _writerOptions = new JsonWriterOptions()
            {
                Indented = true
            };
        }

        public async Task<string> ConvertToStringAsync(JsonElement element)
        {
            await using var ms = new MemoryStream();
            await using var writer = new Utf8JsonWriter(ms, _writerOptions);

            if (element.ValueKind == JsonValueKind.Object)
            {
                writer.WriteStartObject();
            }
            else
            {
                return string.Empty;
            }

            foreach (var property in element.EnumerateObject())
            {
                property.WriteTo(writer);
            }

            writer.WriteEndObject();
            await writer.FlushAsync();
            ms.Position = 0;

            using var reader = new StreamReader(ms, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
    }
}