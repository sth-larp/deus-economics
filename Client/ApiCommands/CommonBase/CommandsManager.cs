using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WispCloudClient.ApiCommands
{
    public static class CommandsManager
    {
        public static List<BaseCommand> Commands { get; }

        static CommandsManager()
        {
            var presetBaseType = typeof(BaseCommand);
            var serviceTypes = new HashSet<Type>(new[] {
                typeof(InputCommand<>),
                typeof(OutputCommand<>),
                typeof(InputOutputCommand<,>),
            });

            Commands = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x != presetBaseType && !serviceTypes.Contains(x) && presetBaseType.IsAssignableFrom(x))
                .Select(x => (BaseCommand)Activator.CreateInstance(x))
                .OrderBy(x => x.SortIndex)
                .ThenBy(x => x.Name)
                .ToList();
        }

    }

}
