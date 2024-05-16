using AppSec.AIPromtInjection.Abstractions.Models;

namespace AppSec.AIPromtInjection.Abstractions.Services;

public interface IChatGptService
{
    Task<string> SendMessage(ChatRequestDto messageContext);
}
