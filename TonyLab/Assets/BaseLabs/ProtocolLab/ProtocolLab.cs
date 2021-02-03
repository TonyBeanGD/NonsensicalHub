using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class ProtocolManager
{
    private static ProtocolManager instance;

    public static ProtocolManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ProtocolManager();
                instance.Events = new Dictionary<Type, Action<object>>();

                instance.Protocols = new Dictionary<string, Type>();
                Type[] types = Assembly.GetExecutingAssembly().GetTypes();
                foreach (var item in types)
                {
                    var v = typeof(ReceivedMessageEvent<>).MakeGenericType(item);
                    TestSignalNameAttribute temp = (TestSignalNameAttribute)item.GetCustomAttribute(typeof(TestSignalNameAttribute));
                    if (temp != null)
                    {
                        instance.Protocols.Add(temp.PositionalString, item);
                    }
                }
            }

            return instance;
        }
    }

    internal class SignalInfo<T>
    {
        public Type t;

        public SignalInfo()
        {
            t = typeof(T);
        }
    }

    public new string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in Protocols)
        {
            sb.Append("(");
            sb.Append(item.Key);
            sb.Append(",");
            sb.Append(item.Value);
            sb.Append(")");
        }
        return sb.ToString();
    }

    public delegate void ReceivedMessageEvent<T>();

    public Dictionary<string, Type> Protocols;

    /// <summary>
    /// Action里应当使用泛型
    /// </summary>
    public Dictionary<Type, Action<object>> Events;

    private void OnReceivedMessage(Protocol dataProtocol)
    {
        //如果代码里没有写相关类直接返回
        if (!Protocols.ContainsKey(dataProtocol.signalName))
        {
            return;
        }

        Type protocol = Protocols[dataProtocol.signalName];
        Dictionary<string, object> dictionary = LitJson.JsonMapper.ToObject<Dictionary<string, object>>(dataProtocol.signalContent);

        Events[protocol]?.Invoke(CreateInstance(protocol, dictionary));
    }

    static object CreateInstance(Type type, Dictionary<string, object> dictionary)
    {
        var instance = Activator.CreateInstance(type);
        foreach (var property in type.GetProperties())
        {
            TestSignalKeyAttribute attribute = (TestSignalKeyAttribute)property.GetCustomAttribute(typeof(TestSignalKeyAttribute), false);

            if (dictionary.ContainsKey(attribute.PositionalString))
            {
                var value = dictionary[attribute.PositionalString];

                if (property.PropertyType.IsSubclassOf(typeof(Enum)))
                {
                    value = Enum.ToObject(property.PropertyType, value);
                }
                property.SetValue(instance, value);
            }
        }

        return instance;
    }
}

public class Protocol
{
    public string signalName { get; set; }
    public string signalContent { get; set; }
}

[TestSignalName(SignalName.TestSignal)]
public class TestSignalTemplate
{
    [TestSignalKey(SignalKey.TestKey)]
    public string id;
}

[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
sealed class TestSignalNameAttribute : Attribute
{
    // See the attribute guidelines at 
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    readonly string positionalString;

    // This is a positional argument
    public TestSignalNameAttribute(string positionalString)
    {
        this.positionalString = positionalString;

        // TODO: Implement code here
    }

    public string PositionalString
    {
        get { return positionalString; }
    }

    // This is a named argument
    public int NamedInt { get; set; }
}

[System.AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
sealed class TestSignalKeyAttribute : Attribute
{
    // See the attribute guidelines at 
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    readonly string positionalString;

    // This is a positional argument
    public TestSignalKeyAttribute(string positionalString)
    {
        this.positionalString = positionalString;

        // TODO: Implement code here
    }

    public string PositionalString
    {
        get { return positionalString; }
    }

    // This is a named argument
    public int NamedInt { get; set; }
}

public class SignalName
{
    public const string TestSignal = nameof(TestSignal);
}

public class SignalKey
{
    public const string TestKey = nameof(TestKey);
}


//public partial class CommunicationLogic
//{
//    static MethodInfo DeserializeObjectMethod { get; }

//    private static void OnReceivedMessage(IEventAggregator eventAggregator, SportDataProtocol dataProtocol)
//    {
//        if (!ProtocolParameter.Protocols.ContainsKey(dataProtocol.signalName)) return;
//        var protocol = ProtocolParameter.Protocols[dataProtocol.signalName];
//        var dictionary = (Dictionary<string, object>)DeserializeObjectMethod.MakeGenericMethod(typeof(Dictionary<string, object>)).Invoke(null, new object[]
//        {
//                dataProtocol.signalContent,
//                new JsonSerializerSettings()
//                {
//                    TypeNameHandling = TypeNameHandling.Auto
//                }
//        });
//        var receivedMessageEvent = typeof(ReceivedMessageEvent<>).MakeGenericType(protocol);
//        var pubSubEvent = GetGetEventMethod().MakeGenericMethod(receivedMessageEvent).Invoke(eventAggregator, null);
//        var publishMethod = GetPublishMethod(pubSubEvent.GetType(), protocol);
//        publishMethod.Invoke(pubSubEvent, new object[] { CreateInstance(protocol, dictionary) });
//    }

//    static CommunicationLogic()
//    {
//        DeserializeObjectMethod = GetDeserializeObjectMethod();
//    }

//    private static MethodInfo GetDeserializeObjectMethod()
//    {
//        var type = typeof(JsonConvert);
//        var methodName = nameof(JsonConvert.DeserializeObject);
//        var bindingAttr = BindingFlags.Static | BindingFlags.Public;
//        var parameters = new Type[] { typeof(string), typeof(JsonSerializerSettings) };
//        foreach (var method in type.GetMethods(bindingAttr).Where(p => p.Name == methodName && p.IsGenericMethod))
//        {
//            var methodParameters = method.GetParameters();
//            if (methodParameters.Length != parameters.Length) continue;
//            if (!methodParameters.All(p => parameters.Any(p1 => p1.FullName == p.ParameterType.FullName))) continue;
//            return method;
//        }
//        throw new Exception();
//    }

//    static MethodInfo GetGetEventMethod()
//    {
//        var methodName = nameof(IEventAggregator.GetEvent);
//        var bindingAttr = BindingFlags.Instance | BindingFlags.Public;
//        return typeof(IEventAggregator).GetMethod(methodName, bindingAttr);
//    }

//    static MethodInfo GetPublishMethod(Type type, params Type[] parameters)
//    {
//        var methodName = nameof(PubSubEvent.Publish);
//        return type.GetMethod(methodName, parameters);
//    }

//    static object CreateInstance(Type type, Dictionary<string, object> dictionary)
//    {
//        var instance = Activator.CreateInstance(type);
//        foreach (var property in type.GetProperties())
//        {
//            if (property.GetAttributes().OfType<JsonPropertyAttribute>().FirstOrDefault() is JsonPropertyAttribute attribute)
//            {
//                if (dictionary.ContainsKey(attribute.PropertyName))
//                {
//                    var value = dictionary[attribute.PropertyName];
//                    if (property.PropertyType.IsSubclassOf(typeof(Enum)))
//                    {
//                        value = Enum.ToObject(property.PropertyType, value);
//                    }
//                    property.SetValue(instance, value);
//                }
//            }
//        }

//        return instance;
//    }
//}

