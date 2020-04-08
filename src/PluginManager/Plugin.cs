
public interface IPlugin
{
    void Startup(IPluginFeatureCollection features);

    void Shutdown();
}

