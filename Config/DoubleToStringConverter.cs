using Newtonsoft.Json;
using System.Globalization;

namespace WSOptimizerGallinas.Config
{
    public class DoubleToStringConverter : JsonConverter<double>
    {
        public override void WriteJson(JsonWriter writer, double value, JsonSerializer serializer)
        {
            writer.WriteRawValue(value.ToString("F2", CultureInfo.InvariantCulture));

        }

        public override double ReadJson(JsonReader reader, Type objectType, double existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return Convert.ToDouble(reader.Value);
        }
    }

}
