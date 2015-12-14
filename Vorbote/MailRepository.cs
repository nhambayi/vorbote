namespace Vorbote
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Microsoft.Azure.Documents.Linq;
    using System.IO;

    public class DocumentDbMailStore
    {
        private  string _endpointUrl;
        private  string _authorizationKey;
        private string _defaultDatabaseName = "moqmail";
        private string _defaultCollectionName = "moqmail";

        private DocumentClient _client;
        private Database _database;

        public DocumentDbMailStore()
        {
            Init();
        }

        private void Init()
        {
            _endpointUrl = ConfigurationManager.AppSettings["db-uri"];
            _authorizationKey = ConfigurationManager.AppSettings["db-key"];
            _client = new DocumentClient(new Uri(_endpointUrl), _authorizationKey);
            _database = _client.CreateDatabaseQuery() 
                .Where(db => db.Id == _defaultDatabaseName).AsEnumerable().FirstOrDefault();
        }

        public Task SaveMessageAsync(IMailMessage messageHeader)
        {
            return Task.Factory.StartNew(() => 
           {
                SaveMessage(message, messageHeader);
            });
        }

        public void SaveMessage(IMailMessage message, IMailMessage messageHeader)
        {
            var collectionUri = string.Format("dbs/{0}", _database.Id);
            DocumentCollection documentCollection = _client.CreateDocumentCollectionQuery(collectionUri).Where(c =>
            c.Id == _defaultCollectionName).AsEnumerable().FirstOrDefault();

            var documentUri = string.Format("dbs/{0}/colls/{1}", _database.Id, documentCollection.Id);

            _client.CreateDocumentAsync(documentUri, messageHeader).ContinueWith((response) =>
            {
                FileStream fileStream = File.Open("", FileMode.Open);
                CreateDocumentAttachment(response.Result.Resource, fileStream);
            });
        }

        private void CreateDocumentAttachment(Document document, Stream messageStream)
        {
            var mediaOptions = new MediaOptions
            {
                ContentType = "application/pdf",
                Slug = "something.pdf"
            };

            _client.CreateAttachmentAsync(document.SelfLink, messageStream, mediaOptions);
        }
    }
}
