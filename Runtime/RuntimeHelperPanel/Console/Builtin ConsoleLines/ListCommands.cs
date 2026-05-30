using System;
using System.Linq;

namespace ZeroGame.RHP
{
    public class ListCommands : RuntimeConsoleCommand
    {
        public override string Description => "List all commands and search with first parameter.";

        public override string Execute(string[] parameters)
        {
            var listOfBs = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .SelectMany(domainAssembly => domainAssembly.GetExportedTypes())
                .Where(type => type.IsSubclassOf(typeof(RuntimeConsoleCommand))
                && type != typeof(RuntimeConsoleCommand)
                && ! type.IsAbstract
                ).ToArray();

            string ret = $"Total {listOfBs.Length} command.\n";
            foreach (var type in listOfBs)
            {
                var instance = (RuntimeConsoleCommand)Activator.CreateInstance(type);
                string description = instance.Description;
                ret += $"-{type.Name}: {description}\n";
            }
            return ret;
        }
    }
}