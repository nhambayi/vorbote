namespace Vorbote
{
    using System;
    using System.Threading.Tasks;

    public interface IMessageStore
    {
        Task WriteAsync(string data);
        Task WriteLineAsync(string data);
    }
}
