using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace JsonUserPlugin
{
    internal class JsonUser : IUser
    {
        [JsonProperty("first_name")]
        private string _firstname;
        public string FirstName => _firstname;

        [JsonProperty("last_name")]
        private string _lastname;
        public string LastName => _lastname;

        [JsonProperty("email")]
        private string _email;
        public string EmailAddress => _email;
    }

    public class JsonUserProvider : IUserProvider
    {
        private Dictionary<string, JsonUser> users = new Dictionary<string, JsonUser>();

        private static uint currentId = 1;

        public JsonUserProvider(string filePath)
        {
            foreach (var filename in Directory.GetFiles(filePath, "*.json"))
            {
                string filecontent = File.ReadAllText(filename);
                JsonConvert.DeserializeObject<List<JsonUser>>(filecontent);

                foreach(JsonUser user in JsonConvert.DeserializeObject<List<JsonUser>>(filecontent))
                {
                    users.Add((currentId++).ToString(), user);
                }
            }
        }

        public IEnumerable<IUser> GetAll()
        {
            return users.Values;
        }

        public IUser GetById(string id)
        {
            return users[id];
        }
    }
}
