using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

internal class PluginFeatureCollection : IPluginFeatureCollection, IPluginFeatureProvider
{
    private class Temp
    {
        public object Default;

        public Dictionary<string, object> Named { get; set; } = new Dictionary<string, object>();
    }  

    private Dictionary<Type, Temp> FeatureMap = new Dictionary<Type, Temp>();

    public void Register<TFeature>(TFeature feature) where TFeature : class, IPluginFeature
    {
        Type featureType = typeof(TFeature);
        if(FeatureMap.ContainsKey(featureType))
        {
            FeatureMap[featureType].Default = feature; // override ???
        }
        else
        {
            FeatureMap.Add(featureType, new Temp 
            {
                Default = feature,
            });
        }
    }

    public void Register<TFeature>(string name, TFeature feature) where TFeature : class, IPluginFeature
    {
        Type featureType = typeof(TFeature);
        if(FeatureMap.ContainsKey(featureType))
        {
            FeatureMap[featureType].Named.Add(name, feature); // override ???
        }
        else
        {
            FeatureMap.Add(featureType, new Temp 
            {
                Named = {
                    { name, feature },
                }
            });
        }


        throw new NotImplementedException();
    }

    public TFeature GetFeature<TFeature>() where TFeature : class, IPluginFeature
    {

        Type featureType = typeof(TFeature);
        if(FeatureMap.ContainsKey(featureType))
        {
            return (TFeature) FeatureMap[featureType].Default;
        }
        else
        {
            return null;
        }
    }

    public TFeature GetFeature<TFeature>(string name) where TFeature : class, IPluginFeature
    {
        Type featureType = typeof(TFeature);
        if(FeatureMap.ContainsKey(featureType))
        {
            if(FeatureMap[featureType].Named.ContainsKey(name))
            {
                return (TFeature) FeatureMap[featureType].Named[name];
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}

internal class PluginLoadContext : AssemblyLoadContext
{
    private AssemblyDependencyResolver resolver;

    public PluginLoadContext(string pluginPath)
    {
        this.resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        string assemblyPath = this.resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }

        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string libraryPath = this.resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }
}

public class PluginManager
{
    private const string FileSuffix = ".dll";

    private List<string> searchPaths = new List<string>();

    private class PluginInfo
    {
        public Assembly Assembly;

        public string Path;

        public PluginFeatureCollection Features;

        public AssemblyLoadContext Context;

        public IPlugin PluginInstance;
    }

    private Dictionary<string, PluginInfo> loadedPlugins = new Dictionary<string, PluginInfo>();

    public void AddSearchPath(string searchPath)
    {
        searchPaths.Add(Path.GetFullPath(searchPath));
    }

    private PluginFeatureCollection LoadPlugin(string name)
    {
        foreach (string path in searchPaths)
        {
            var filepath = Path.Combine(path, name);
            PluginLoadContext loadContext = new PluginLoadContext(filepath + FileSuffix);
            Assembly asm = loadContext.LoadFromAssemblyName(new AssemblyName(name));

            if(asm != null)
            {
                var types = asm.GetTypes().Where(type => type.GetInterfaces().Contains(typeof(IPlugin)) && !type.IsAbstract && !type.IsInterface);

                if(types.Count() > 1)
                {
                    // error: multiple plugin classes
                }

                if(types.Count() == 1)
                {
                    var pluginInstanceType = types.First();

                    IPlugin pluginInstance = Activator.CreateInstance(pluginInstanceType) as IPlugin;

                    PluginFeatureCollection features = new PluginFeatureCollection();

                    pluginInstance.Startup(features);

                    loadedPlugins.Add(name, new PluginInfo
                    {
                        Assembly = asm,
                        Context = loadContext,
                        Path = asm.Location,
                        Features = features,
                        PluginInstance = pluginInstance,
                    });

                    return features;
                }
            }
        }

        return null;
    }

    public IPluginFeatureProvider GetPlugin(string name)
    {
        if(loadedPlugins.ContainsKey(name))
        {
            return loadedPlugins[name].Features;
        }
        else
        {
            return LoadPlugin(name);
        }
    }

    public void Unload(string name)
    {
        if(loadedPlugins.ContainsKey(name))
        {
            var info = loadedPlugins[name];
            info.PluginInstance.Shutdown();
            info.Context.Unload();
            loadedPlugins.Remove(name);
        }
    }

    public void UnloadAll()
    {
        foreach (var item in loadedPlugins)
        {
            item.Value.PluginInstance.Shutdown();
            item.Value.Context.Unload();
        }
        loadedPlugins.Clear();
    }
}
