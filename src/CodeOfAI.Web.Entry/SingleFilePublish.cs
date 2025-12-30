using System.Reflection;
using Furion;

namespace CodeOfAI.Web.Entry
{
    public class SingleFilePublish : ISingleFilePublish
    {
        public Assembly[] IncludeAssemblies()
        {
            return Array.Empty<Assembly>();
        }

        public string[] IncludeAssemblyNames()
        {
            return new[]
            {
                "CodeOfAI.Application",
                "CodeOfAI.Core",
                "CodeOfAI.Web.Core"
            };
        }
    }
}