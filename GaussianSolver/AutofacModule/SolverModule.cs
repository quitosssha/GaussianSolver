using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace GaussianSolver.AutofacModule;

public class SolverModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.GetDeclaredPublicConstructors().Length != 0)
            .AsSelf()
            .AsImplementedInterfaces()
            .InstancePerDependency();
    }
}