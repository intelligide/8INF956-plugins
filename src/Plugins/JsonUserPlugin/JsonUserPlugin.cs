using System;
using System.IO;
using System.Reflection;

namespace JsonUserPlugin
{
    public class JsonUserPlugin : IPlugin
    {
        public void Startup(IPluginFeatureCollection features)
        {
            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), "users");
            features.Register<IUserProvider>(new JsonUserProvider(filePath));
        }

        public void Shutdown()
        {
        }
    }
}
