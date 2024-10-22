using Amazon.S3;
using DomainObjects.Pregnancy;


namespace BionicPlusAPI.Data.Interfaces
{
    public interface IImageService
    {
        Task<Image> UploadImageAsync(string name,
            Stream image);

        Task<bool> DeleteImageAsync(string cardId);
    }
}
