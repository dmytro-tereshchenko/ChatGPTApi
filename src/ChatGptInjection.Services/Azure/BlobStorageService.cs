using ChatGptInjection.Abstractions;
using ChatGptInjection.Abstractions.Models;
using ChatGptInjection.Abstractions.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ChatGptInjection.Services.Azure;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient? _containerClient;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(IOptions<AppSettings> appSettingsOptions, ILogger<BlobStorageService> logger)
    {
        _logger = logger;
        var azureConnectionString = Environment.GetEnvironmentVariable(appSettingsOptions.Value.AzureConnectionStringName);
        if (azureConnectionString is not null)
        {
            _containerClient = new BlobContainerClient(azureConnectionString, appSettingsOptions.Value.ChatGptResponsesBlobContainer);
        }
        else
        {
            _logger.LogError("Azure connection string to Storage account is not found");
        }
    }

    public async Task<string?> UploadMessage(OpenAiChatResponse messageContext)
    {
        if (_containerClient is null) return null;

        var blobName = $"{messageContext.ChatId}-{messageContext.Created}";
        BlobClient blobClient = _containerClient.GetBlobClient(blobName);
        string blobContents = JsonConvert.SerializeObject(messageContext);

        BlobUploadOptions options = new BlobUploadOptions();
        options.Tags = new Dictionary<string, string>
        {
            { "ChatId", messageContext.ChatId },
            { "Created", messageContext.Created.ToString() }
        };

        await blobClient.UploadAsync(BinaryData.FromString(blobContents), options);

        return blobName;
    }

    public async Task<OpenAiChatResponse?> DownloadMessage(string blobName)
    {
        if (_containerClient is null) return null;

        var blobClient = _containerClient.GetBlobClient(blobName);
        var downloadResult = await blobClient.DownloadContentAsync();

        if (downloadResult is null) return null;

        var openAiChatResponse = JsonConvert.DeserializeObject<OpenAiChatResponse>(downloadResult.Value.Content.ToString());

        return openAiChatResponse;
    }
}
