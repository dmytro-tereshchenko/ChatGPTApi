namespace ChatGptInjection.Abstractions.Services;

public interface IChatContentValidator
{
    bool ChatRequestValidate(string messageContent);

    bool ChatResponseValidate(string messageContent);
}
