using Amazon.S3.Model;
using Amazon.S3;
using BionicPlusAPI.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using BionicPlusAPI.Models.Settings;
using Microsoft.Extensions.Options;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.AspNetCore.Builder;
using DomainObjects.Pregnancy;

namespace BionicPlusAPI.Data.Concrete
{
    public class ImageService : IImageService
    {
        private readonly IAmazonS3 _client;
        private readonly S3Settings _s3Settings;
        private readonly AWSOptions _awsOptions;

        public ImageService(IAmazonS3 client, IOptions<S3Settings> s3Settings, AWSOptions awsOptions)
        {
            _client = client;
            _s3Settings = s3Settings.Value;
            _awsOptions = awsOptions;
        }

        public async Task<bool> DeleteImageAsync(string cardId)
        {
            await _client.DeleteAsync(_s3Settings.BucketName, cardId, additionalProperties: null);
            return true;
        }

        public async Task<Image> UploadImageAsync(string name,
            Stream image)
        {
            var imageGuid = Guid.NewGuid();
            var extension = Path.GetExtension(name);
            var fileName = imageGuid + extension;

            var request = new PutObjectRequest
            {
                InputStream = image,
                BucketName = _s3Settings.BucketName,
                Key = fileName,
                CannedACL = S3CannedACL.PublicRead,
            };
            

            var response = await _client.PutObjectAsync(request);
            image.Close();
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return new Image { ImageUrl = $"https://s3.{_awsOptions.Region.SystemName}.amazonaws.com/{_s3Settings.BucketName}/{fileName}",
                    ImageGuid = imageGuid,
                    ImageType = extension,
                    ImageVersion = response.VersionId,
                };
            }
            else
            {
                throw new FormatException($"Image not loaded cause: AWS HttpStatusCode = {response.HttpStatusCode}");
            }
           
        }

    }
}
