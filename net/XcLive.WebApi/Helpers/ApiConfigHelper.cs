using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using log4net.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchemaBuilder.Svc.Core;
using SchemaBuilder.Svc.Svc;

namespace SchemaBuilder.Web.Helpers;

public static class ApiConfigHelper
{
    public static void Init(WebApplicationBuilder builder)
    {
        InitLog();
        InitApiJsonOptions(builder);
        InitExceptions(builder);
        InitAuth(builder);
        AppHelper.Init(builder.Configuration);
    }

    private static void InitLog()
    {
        XmlConfigurator.Configure(new FileInfo("log4net.config"));
    }

    private static void InitApiJsonOptions(WebApplicationBuilder builder)
    {
        // Add services to the container.
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new LongToStringConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableLongToStringConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
            options.JsonSerializerOptions.Converters.Add(new JsonNullableDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
            options.JsonSerializerOptions.Converters.Add(new UniversalEnumConverterFactory());
        });
    }

    private static void InitAuth(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options => { options.Filters.Add<ApiAuthorizationFilter>(); });
    }

    private static void InitExceptions(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options => { options.Filters.Add(new BadRequestFilter()); });
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                // 这里可以自定义返回的错误信息
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(e => new { Field = e.Key, Message = e.Value!.Errors.First().ErrorMessage })
                    .ToArray();
                var errStr = string.Join(",", errors.Select(e => e.Field + ": " + e.Message));
                // 返回 HTTP 200，并包含错误信息
                return new OkObjectResult(R.Faild(errStr));
            };
        });
    }
}

public class BadRequestFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is BadHttpRequestException)
        {
            context.Result = new ObjectResult(new { error = "Bad Request", message = context.Exception.Message })
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
            context.ExceptionHandled = true;
        }
    }
}

public class UniversalEnumConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(UniversalEnumConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private class UniversalEnumConverter<T> : JsonConverter<T> where T : Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && int.TryParse(reader.GetString(), out int value))
            {
                return (T)Enum.ToObject(typeof(T), value);
            }
            else if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int intValue))
            {
                return (T)Enum.ToObject(typeof(T), intValue);
            }

            throw new JsonException($"Cannot convert {reader.GetString()} to {typeToConvert.Name}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // 使用默认的序列化行为
            writer.WriteNumberValue(Convert.ToInt32(value));
        }
    }
}