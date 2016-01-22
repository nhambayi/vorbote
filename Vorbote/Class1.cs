using Microsoft.WindowsAzure.Storage.Queue;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// Alternative (not related to this code, but in case you want to serialize to XML
// instead): http://social.msdn.microsoft.com/Forums/en-US/csharplanguage/thread/5d08bc28-5b61-4c5a-8c4b-4665b1c929ea/

// Usage example
// NOTE: The Windows Azure CloudQueueMessage constructor accepts either a string
//       or a byte array. If a byte array is passed in, it is Base64 encoded.
//       Base64 encoding results in approx a 1/3 payload size penalty (so the
//       payload is 4/3 of size of binary). The maximum size of single message for a
//       Windows Azure Storage queue is 64 KB. (Versus 256 KB for ServiceBus queues.)
//
// The AppSpecificMessage class and the GetAppSpecificMessage and CreateCloudQueueMessage
//       helper functions serve to illustrate how to create simple helpers that use the
//       more generic classes shown below.

//public CloudQueueMessage CreateCloudQueueMessage(AppSpecificMessage msg)
//{
//    return new CloudQueueMessage(ByteArraySerializer<AppSpecificMessage>.Serialize(msg));
//}

//public AppSpecificMessage GetAppSpecificMessage(CloudQueueMessage cloudQueueMessage)
//{
//    return ByteArraySerializer<AppSpecificMessage>.
//       Deserialize(cloudQueueMessage.AsBytes);
//}

namespace Vorbote
{
    public static class ByteArraySerializer<T>
    {
        public static byte[] Serialize(T m)
        {
            var ms = new MemoryStream();
            try
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, m);
                return ms.ToArray();
            }
            finally
            {
                ms.Close();
            }
        }

        public static T Deserialize(byte[] byteArray)
        {
            var ms = new MemoryStream(byteArray);
            try
            {
                var formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(ms);
            }
            finally
            {
                ms.Close();
            }
        }
    }
}
