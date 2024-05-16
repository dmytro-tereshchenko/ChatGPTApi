﻿using AppSec.AIPromtInjection.Abstractions.Models;
using AppSec.AIPromtInjection.Abstractions.Services;

namespace AppSec.AIPromtInjection.WebApi.Infrastructure.EndPoints;

public static class EndPointsGroup
{
    public static IResult SendMessage(ChatRequestDto bodyRequest, IChatGptService chatGptService)
    {
        try
        {
            if (string.IsNullOrEmpty(bodyRequest.Message))
                return TypedResults.BadRequest();

            if (string.IsNullOrEmpty(bodyRequest.ChatId))
                bodyRequest.ChatId = Guid.NewGuid().ToString();

            var chatResponse = chatGptService.SendMessage(bodyRequest);

            return TypedResults.Ok(chatResponse);
        }
        catch (Exception ex)
        {
            return Results.Json(new { error = ex.Message }, statusCode: 500);
        }
    }

    public static IResult GetHistory(string chatId = "")
    {
        try
        {
            if (!string.IsNullOrEmpty(chatId))
            {
                return TypedResults.Ok($"history for one chatId {chatId}");
            }

            return TypedResults.Ok("all history");
        }
        catch (Exception ex)
        {
            return Results.Json(new { error = ex.Message }, statusCode: 500);
        }
    }
}