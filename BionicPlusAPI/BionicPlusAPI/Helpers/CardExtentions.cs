using BionicPlusAPI.Models;
using DomainObjects.Pregnancy;

namespace BionicPlusAPI.Helpers
{
    public static class CardExtentions
    {
        public static AnnotationResponse ToAnnotation(this List<Card> cards)
        {
            if (cards == null || !cards.Any())
            {
                return new AnnotationResponse { Annotations = new List<Annotation>() };
            }
            var annotations = new List<Annotation>();
            foreach (var card in cards)
            {
                annotations.Add(new Annotation { Description = card.Description.GetFirstCharacters(50), Id = card.Id, Title = card.Title, Category = card.Category, ImageUrl = card.ImgInfo?.ImageUrl });
            }
            return new AnnotationResponse { Annotations = annotations };
        }

        public static Annotation ToAnnotation(this Card card)
        {
            if (card == null)
            {
                return new Annotation();
            }
            return new Annotation { Description = card.Description.GetFirstCharacters(50), Id = card.Id, Title = card.Title, Category = card.Category, ImageUrl = card.ImgInfo?.ImageUrl };
        }
    }
}
