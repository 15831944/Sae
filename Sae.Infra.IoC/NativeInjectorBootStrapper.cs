using Autofac;
using Sae.Domain.Repository;
using Sae.Infra.Data;

namespace Sae.Infra.IoC
{
    public class NativeInjectorBootStrapper
    {
        public static IContainer Container()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TextRepository>().As<ITextRepository>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(BlockRepository<,>)).As(typeof(IBlockRepository<,>)).InstancePerLifetimeScope();
            return builder.Build();
        }
    }
}
