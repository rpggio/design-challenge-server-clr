using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Args;
using Args.Help;
using Args.Help.Formatters;

namespace DCS.Console
{
    public static class ArgsUtil
    {
        public static void BindModel(object model, string[] args)
        {
            var modelType = model.GetType();
            var definition = CreateDefinitionForType(modelType);
            var bindMethod = definition.GetType().GetMethod("BindModel");
            try
            {
                bindMethod.Invoke(definition, new[] {model, args});
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                throw;
            }
        }

        public static object CreateDefinitionForType(Type type)
        {
            var bindToMethod = typeof (Configuration).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m => m.Name == "Configure" && !m.GetParameters().Any());
            try
            {
                return bindToMethod.MakeGenericMethod(type).Invoke(null, null);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                throw;
            }
        }

        public static void WriteHelp(Type modelType, TextWriter writer)
        {
            var definition = CreateDefinitionForType(modelType);
            var helpProvider = new HelpProvider();
            var generateHelpMethod = typeof (HelpProvider).GetMethod("GenerateModelHelp").MakeGenericMethod(modelType);
            var modelHelp = (ModelHelp) generateHelpMethod.Invoke(helpProvider, new[] {definition});
            var formatter = new ConsoleHelpFormatter(80, 1, 5);
            formatter.WriteHelp(modelHelp, writer);
        }
    }
}