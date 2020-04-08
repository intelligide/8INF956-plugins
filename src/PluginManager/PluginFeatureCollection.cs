using System;


public interface IPluginFeatureCollection
{
    void Register<TFeature>(TFeature feature) where TFeature: class, IPluginFeature;

    void Register<TFeature>(string name, TFeature feature) where TFeature: class, IPluginFeature;
}

public static class IPluginFeatureCollectionExtensions
{
    public static void Register<TFeature, TImplementation>(this IPluginFeatureCollection collection) where TFeature: class, IPluginFeature where TImplementation: TFeature
    {
        collection.Register<TFeature>((TImplementation) Activator.CreateInstance(typeof(TImplementation)));
    }

    public static void Register<TFeature, TImplementation>(this IPluginFeatureCollection collection, string name) where TFeature: class, IPluginFeature where TImplementation: TFeature
    {
        collection.Register<TFeature>(name, (TImplementation) Activator.CreateInstance(typeof(TImplementation)));
    }
}
