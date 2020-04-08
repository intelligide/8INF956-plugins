using System;

public interface IPluginFeatureProvider
{
    TFeature GetFeature<TFeature>() where TFeature: class, IPluginFeature;

    TFeature GetFeature<TFeature>(string name) where TFeature: class, IPluginFeature;
}
