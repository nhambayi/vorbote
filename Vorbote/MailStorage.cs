namespace Vorbote
{
    using System.IO;
    using Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using Microsoft.WindowsAzure.Storage.Queue;
    using System.Diagnostics;
    using Newtonsoft.Json;

    public class MailStorage 
    {
        private static CloudStorageAccount _storageAccount;

        public static CloudStorageAccount StorageAccount
        {
            get
            {
                if (_storageAccount == null)
                {
                    string accountName = SmtpServerConfiguration.StorageAccountName;
                    string accountKey = SmtpServerConfiguration.StorageAccountKey;
                    StorageCredentials creds = new StorageCredentials(accountName, accountKey);
                    _storageAccount = new CloudStorageAccount(creds, useHttps: true); 
                }

                return _storageAccount;
            }
        }

        public static string Save(string containerName, string messageBody)
        {
            try
            {
                string filekey = GenerateFileKey();
                

                CloudBlobClient client = StorageAccount.CreateCloudBlobClient();

                CloudBlobContainer sampleContainer = client.GetContainerReference(containerName);
                sampleContainer.CreateIfNotExists();

                CloudBlockBlob blob = sampleContainer.GetBlockBlobReference(filekey);


                using (Stream file = GetStream(messageBody))
                {
                    blob.UploadFromStream(file);
                }

                return filekey;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                throw;
            }
        }

        public async static void QueueMessage(string id, string account, string headers)
        {
            var queueClient = StorageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(SmtpServerConfiguration.QueueName);

            try
            {
                queue.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }


            var newMessage = new EmailReceivedMessage()
            {
                Account = account,
                MessageKey = id,
                RawHeaders = headers
            };

            string data = JsonConvert.SerializeObject(newMessage);

            CloudQueueMessage message = new CloudQueueMessage(data);
            try
            {
                await queue.AddMessageAsync(message);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                //throw;
            }
        }

        public static Stream GetStream(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private static string GenerateFileKey()
        {
            var guid = Guid.NewGuid();
            string key = guid.ToString().Replace("-", "");
            return key;
        }
    }
}
