using Amazon.Runtime;
using Amazon.S3;

namespace BionicPlusAPI.Extensions;

/// <summary>
/// Статический класс для добавления сервисов Amazon S3 в коллекцию служб
/// </summary>
public static class S3Services
{
    /// <summary>
    /// Добавляет сервисы Amazon S3 в коллекцию служб
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public static void AddS3Services(this IServiceCollection services, IConfiguration configuration)
    {
        // Получаем настройки AWS из конфигурации
        var awsOption = configuration.GetAWSOptions();

        // Устанавливаем учетные данные AWS
        awsOption.Credentials = new BasicAWSCredentials(configuration["AWS:AccessKey"], configuration["AWS:SecretKey"]);

        // Добавляем настройки AWS по умолчанию в коллекцию служб
        services.AddDefaultAWSOptions(awsOption);

        // Добавляем сервис Amazon S3 в коллекцию служб
        services.AddAWSService<IAmazonS3>();
    }
}