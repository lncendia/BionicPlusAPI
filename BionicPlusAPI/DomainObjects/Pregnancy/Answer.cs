using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DomainObjects.Pregnancy
{
    public class Answer
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        [JsonPropertyName("title")]
        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("imageInfo")]
        public Image? ImageInfo { get; set; }

        [JsonPropertyName("relatedCard")]
        public Annotation? RelatedCard { get; set; }
    }
}