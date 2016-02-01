namespace Vorbote
{
    using System;

    public interface IMailStorage
    {
        string Save(string containerName, string messageBody);
    }
}
