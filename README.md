

1.  Le projet se divise en 4 projets:
    - `App`, un projet d'exécutable qui contient la fonction main.
    - `PluginManager`, un projet de bibliothèque qui contient le code du gestionnaire de plugins. 
    Ce projet est destiné à fournir un comportement générique et n'est pas spécifique à cette application.
    - `PluginBase`, un projet de bibliothèque qui contient les interfaces pour les communications entre l'application et les plugins. 
    - `JsonUserPlugin`, le plugin qui charge des utilisateurs avec des fichiers JSON.

2. Au lieu d'utiliser une classe, le système utilise une interface IUser présente dans le projet `PluginBase` à coté de
   l'interface `IUserProvider`.

3. Chaque plugin doit contenir une classe implémentant l'interface `IPlugin`. Cette interface contient deux méthodes
   `Startup()` et `Shutdown()`, appelé par la système quand le plugin est chargé dans l'application et quand le
   plugin est déchargé. 
   Lors de l'appel à la méthode `Startup`, le plugin doit enregistrer des "features". Une Feature est une instance de  
   classe que le plugin met à disposition de l'application. Les features sont enregistrés dans le gestionnaire de plugins et l'application peut récupérer les features via ce dernier.

   J'ai choisi d'utiliser une tel architecture afin de séparer les responsabilités de chaque systeme. 
   - L'application n'est responsable que de la classe implémentant `IPlugin`
   - Le plugin est chargé de l'instantiation et de la gestion des ses classes internes. 

   Cela se traduit par la classe `JsonUserPlugin` qui enregistre le `JsonUserProvider` en tant qu'implémentation de `IUserProvider`:
   ```csharp
   public class JsonUserPlugin : IPlugin
   {
       public void Startup(IPluginFeatureCollection features)
       {
           features.Register<IUserProvider, JsonUserProvider>();
       }

       public void Shutdown()
       {
       }
   }
   ```

   L'interface `IUserProvider` sert de contrat entre l'application et le plugin. Il s'agit d'une classe "feature".
   ```csharp
   public interface IUserProvider : IPluginFeature
   {
       IEnumerable<IUser> GetAll();

       IUser GetById(string id);
   }
   ```

   L'application peut ensuite récupérer l'instance via le gestionnaire de plugins.
   ```csharp
   var features = pm.GetPlugin("JsonUserPlugin");
   IUserProvider jsonUserProvider = features.GetFeature<IUserProvider>();
   ```
