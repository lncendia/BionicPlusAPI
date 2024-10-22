using DomainObjects.Pregnancy;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuthService.Infrastructure.Converters
{
    public class CategoryListConverter : JsonConverter<List<Category>>
    {
        public override List<Category> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var categoryList = new List<Category>();

                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.String && Enum.TryParse<Category>(reader.GetString(), out var category))
                    {
                        categoryList.Add(category);
                    }
                }

                return categoryList;
            }

            return null; // Обработка ошибки в случае некорректного формата JSON
        }

        public override void Write(Utf8JsonWriter writer, List<Category> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var category in value)
            {
                writer.WriteStringValue(category.ToString());
            }

            writer.WriteEndArray();
        }
    }
}
