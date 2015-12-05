using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vorbote.Models;
using Microsoft.Azure.Documents.Linq;

namespace Vorbote
{
    public class MailRepository
    {
        private  string _endpointUrl = "<your endpoint URI>";
        private  string _authorizationKey = "<your key>";
        private DocumentClient _client;
        private string _defaultDatabaseName  = "moqmail";
        private string _defaultCollectionName = "moqmail";
        private Database _database;

        public MailRepository()
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

        public void SaveMessage(IMailMessage message)
        {
            DocumentCollection documentCollection = _client.CreateDocumentCollectionQuery("dbs/" 
                + _database.Id).Where(c => c.Id == _defaultCollectionName).AsEnumerable().FirstOrDefault();
            _client.CreateDocumentAsync("dbs/" + _database.Id + "/colls/" + documentCollection.Id, message);
        }
    }
}
