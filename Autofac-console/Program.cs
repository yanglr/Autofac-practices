using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.Indexed;

namespace Autofac_console_keyed
{
    public class Program
    {
        public static void Main()
        {
            var builder = new ContainerBuilder();

            // 注册键控索引类型的服务
            builder.RegisterType<ServiceA>().Keyed<IService>("ServiceA").WithParameter("type", "TypeA");      // ToDo: IService need rename
            builder.RegisterType<ServiceB>().Keyed<IService>("ServiceB").WithParameter("type", "TypeB");
            builder.RegisterType<ServiceConsumer>();

            var container = builder.Build();

            // 解析 ServiceConsumer，并注入 IIndex<string, IService>
            using (var scope = container.BeginLifetimeScope())
            {
                var consumer = scope.Resolve<ServiceConsumer>(new TypedParameter(typeof(Dictionary<string, string>), GetDictionary()));

                // 从其他地方获取 Service key
                string serviceKey = "ServiceA"; // ToDo: need replace

                // 使用变量作为参数调用 UseService 方法
                consumer.UseService(serviceKey);
                Console.ReadKey();
            }
        }

        private static Dictionary<string, string> GetDictionary()
        {
            // 创建类型字典并添加对应的类型字符串
            var typeDictionary = new Dictionary<string, string>
            {
                { "ServiceA", "TypeA" },
                { "ServiceB", "TypeB" }
            };

            return typeDictionary;
        }
    }
}

public interface IService
{
    void DoSomething();
}

public class ServiceA : IService
{
    public ServiceA(string type)
    {
        Console.WriteLine("ServiceA constructed with type: " + type);
    }

    public void DoSomething()
    {
        Console.WriteLine("ServiceA is doing something.");
    }
}

public class ServiceB : IService
{
    public ServiceB(string type)
    {
        Console.WriteLine("ServiceB constructed with type: " + type);
    }

    public void DoSomething()
    {
        Console.WriteLine("ServiceB is doing something.");
    }
}

public class ServiceConsumer
{
    private readonly IIndex<string, IService> _services;

    public ServiceConsumer(IIndex<string, IService> services)
    {
        _services = services;
    }

    public void UseService(string key)
    {
        if (_services.TryGetValue(key, out var service))
        {
            service.DoSomething();
        }
        else
        {
            Console.WriteLine("Unknown service key: " + key);
        }
    }
}
