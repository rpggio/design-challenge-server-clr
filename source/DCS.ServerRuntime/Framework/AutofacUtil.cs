using System;
using System.Reflection;
using Autofac;
using DCS.Core.Core;

namespace DCS.ServerRuntime.Framework
{
    public static class AutofacUtil
    {
        public static ContainerBuilder RegisterDeclaredComponentsFrom(
            this ContainerBuilder builder, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var registerAttribute = type.GetCustomAttribute<RegisterComponentAttribute>();
                if (registerAttribute != null)
                {
                    var registration = builder.RegisterType(type);
                    if (registerAttribute.Flags.HasFlagValue(RegisterWith.InstancePerDependency))
                    {
                        registration = registration.InstancePerDependency();
                    }
                    if (registerAttribute.Flags.HasFlagValue(RegisterWith.InstancePerLifetimeScope))
                    {
                        registration = registration.InstancePerLifetimeScope();
                    }
                    if (registerAttribute.Flags.HasFlagValue(RegisterWith.InstancePerOwned))
                    {
                        Require.PropertyIsNotNull("OwningType", registerAttribute.OwningType);
                        registration = registration.InstancePerOwned(registerAttribute.OwningType);
                    }
                    if (registerAttribute.Flags.HasFlagValue(RegisterWith.SingleInstance))
                    {
                        registration = registration.SingleInstance();
                    }
                    if (registerAttribute.Flags.HasFlagValue(RegisterWith.AsSelf))
                    {
                        registration = registration.AsSelf();
                    }
                    if (registerAttribute.Flags.HasFlagValue(RegisterWith.AsImplementedInterfaces))
                    {
                        registration = registration.AsImplementedInterfaces();
                    }
                    if (registerAttribute.Flags.HasFlagValue(RegisterWith.OwnedByLifetimeScope))
                    {
                        registration = registration.OwnedByLifetimeScope();
                    }

                    if (registerAttribute.AsService != null)
                    {
                        registration.As(registerAttribute.AsService);
                    }
                }
            }

            return builder;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterComponentAttribute : Attribute
    {
        public RegisterComponentAttribute(RegisterWith flags = RegisterWith.None)
        {
            Flags = flags;
        }

        public RegisterWith Flags { get; private set; }

        public Type OwningType { get; set; }

        public Type AsService { get; set; }
    }

    [Flags]
    public enum RegisterWith
    {
        None = 0,

        InstancePerDependency =     1 << 1,
        InstancePerLifetimeScope =  1 << 2,
        InstancePerOwned =          1 << 3,
        SingleInstance =            1 << 4,

        AsSelf =                    1 << 7,
        AsImplementedInterfaces =   1 << 8,
        OwnedByLifetimeScope =      1 << 9,
    }
}
