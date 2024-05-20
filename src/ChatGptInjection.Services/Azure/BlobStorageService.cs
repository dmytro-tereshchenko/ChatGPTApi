using ChatGptInjection.Abstractions;
using ChatGptInjection.Abstractions.Models;
using ChatGptInjection.Abstractions.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ChatGptInjection.Services.Azure;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IOptions<AppSettings> appSettingsOptions, 
        IConfigService configService)
    {
        var azureConnectionString = configService.GetAzureConnectionString();
        _containerClient = new BlobContainerClient(azureConnectionString, appSettingsOptions.Value.ChatGptResponsesBlobContainer);
    }

    public async Task<string> UploadMessage(OpenAiChatResponse messageContext)
    {
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
        var blobClient = _containerClient.GetBlobClient(blobName);
        var downloadResult = await blobClient.DownloadContentAsync();

        if (downloadResult is null) return null;

        var openAiChatResponse = JsonConvert.DeserializeObject<OpenAiChatResponse>(downloadResult.Value.Content.ToString());

        return openAiChatResponse;
    }
}
