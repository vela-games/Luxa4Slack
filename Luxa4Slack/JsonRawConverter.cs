﻿namespace CG.Luxa4Slack
{
  using System;
  using System.Diagnostics;
  using System.Reflection;

  using Castle.DynamicProxy;

  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;

  using SlackAPI;
  using SlackAPI.WebSocketMessages;

  public class JsonRawConverter : JsonConverter
  {
    private readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(ImMarked) || 
             objectType == typeof(ChannelMarked) || 
             objectType == typeof(GroupMarked) || 
             objectType == typeof(Message) ||
             objectType == typeof(NewMessage);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.Null)
      {
        string rawData = serializer.Context.Context as string;

        // Extract part of the raw data if needed
        if (string.IsNullOrEmpty(reader.Path) == false)
        {
          JObject context = JObject.Parse(rawData);
          JToken token = context.SelectToken(reader.Path);
          rawData = token.ToString();
        }

        object instance = this.proxyGenerator.CreateClassProxy(objectType, new[] { typeof(IRawMessage) }, new RawMessageInterceptor(rawData));

        JObject jsonObject = JObject.Load(reader);
        serializer.Populate(jsonObject.CreateReader(), instance);

        return instance;
      }

      return null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    private class RawMessageInterceptor : IInterceptor
    {
      private static readonly MethodInfo GetDataProperty = typeof(IRawMessage).GetProperty(nameof(IRawMessage.Data)).GetGetMethod();
      private readonly string raw;

      public RawMessageInterceptor(string raw)
      {
        this.raw = raw;
      }

      public void Intercept(IInvocation invocation)
      {
        if (invocation.Method == GetDataProperty)
        {
          invocation.ReturnValue = this.raw;
        }
        else
        {
          invocation.Proceed();
        }
      }
    }
  }
}
