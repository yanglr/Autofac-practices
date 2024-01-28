using Autofac;
using Autofac.Features.Indexed;

namespace Autofac_console_keyed_Module
{
    public interface IService
    {
        void DoSomething();
    }

    public class ServiceA : IService
    {
        private readonly string _type;

        public ServiceA(string type)
        {
            _type = type;
            Console.WriteLine("ServiceA constructed with type: " + type);
        }

        public void DoSomething()
        {
            Console.WriteLine("ServiceA is doing something.");
        }
    }

    public class ServiceB : IService
    {
        private readonly string _type;

        public ServiceB(string type)
        {
            _type = type;
            Console.WriteLine("ServiceB constructed with type: " + type);
        }

        public void DoSomething()
        {
            Console.WriteLine("ServiceB is doing something.");
        }
    }

    public class ServicesModule : Autofac.Module
    {
        private readonly Dictionary<string, string> _typeDictionary;

        public ServicesModule(Dictionary<string, string> typeDictionary)
        {
            _typeDictionary = typeDictionary;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceA>().Keyed<IService>("ServiceA").WithParameter((p, c) => p.Name == "type", (p, c) => _typeDictionary["ServiceA"]);
            builder.RegisterType<ServiceB>().Keyed<IService>("ServiceB").WithParameter((p, c) => p.Name == "type", (p, c) => _typeDictionary["ServiceB"]);
        }
    }

    public class Program
    {
        public static void Main()
        {
            var builder = new ContainerBuilder();

            var typeDictionary = new Dictionary<string, string>
            {
                { "ServiceA", "TypeA" },
                { "ServiceB", "TypeB" }
            };

            builder.RegisterModule(new ServicesModule(typeDictionary));
            builder.RegisterType<ServiceConsumer>();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var consumer = scope.Resolve<ServiceConsumer>();

                string serviceKey = "ServiceA";
                consumer.UseService(serviceKey);
            }
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
}
