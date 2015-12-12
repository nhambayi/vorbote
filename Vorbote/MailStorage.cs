using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public class MailStorage
    {
        public string Storage(string containerName, string messageBody)
        {
            string accountName = "vorbotestoragedev";
            string accountKey = "Cc9pujEsETK4kP25F3T4lE1Kw8yAFxVCaD8xkrXrXtaLJUhCcY+9wA4BdQneqaF3omnmhsVowNCpfQQ1tq4+8w==";

            try
            {
                string filekey = GenerateFileKey();
                StorageCredentials creds = new StorageCredentials(accountName, accountKey);
                CloudStorageAccount account = new CloudStorageAccount(creds, useHttps: true);

                CloudBlobClient client = account.CreateCloudBlobClient();

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
                throw;
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
