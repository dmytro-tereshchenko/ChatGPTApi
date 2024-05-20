using ChatGptInjection.Abstractions.Models;

namespace ChatGptInjection.Abstractions.Services;

public interface IBlobStorageService
{
    Task<string> UploadMessage(OpenAiChatResponse messageContext);
    Task<OpenAiChatResponse?> DownloadMessage(string blobName);
}
