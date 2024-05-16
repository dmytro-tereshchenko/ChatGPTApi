using AppSec.AIPromtInjection.Abstractions.Services;

namespace AppSec.AIPromtInjection.Services.Validators;

public class ChatContentValidator : IChatContentValidator
{
    public bool ChatRequestValidate(string messageContent)
    {
        return true;
    }

    public bool ChatResponseValidate(string messageContent)
    {
        return true;
    }
}
