using System;
using System.IO;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            PluginManager pm = new PluginManager();
            pm.AddSearchPath(Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "plugins"));

            var features = pm.GetPlugin("JsonUserPlugin");
            if(features == null)
            {
                Console.Error.WriteLine("Cannot load JsonUserPlugin");
            }
            else
            {
                IUserProvider jsonUserProvider = features.GetFeature<IUserProvider>();
                foreach (IUser user in jsonUserProvider.GetAll())
                {
                    Console.WriteLine(user.FirstName + " " + user.LastName + ", " + user.EmailAddress);
                }
            }
        }
    }
}
