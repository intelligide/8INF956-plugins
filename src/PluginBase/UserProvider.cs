using System.Collections.Generic;

public interface IUser
{
    string FirstName { get; }

    string LastName { get; }

    string EmailAddress { get; }
}

public interface IUserProvider : IPluginFeature
{
    IEnumerable<IUser> GetAll();

    IUser GetById(string id);
}
