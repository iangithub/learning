using cosmosdb.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace cosmosdb.Controllers
{
    public class ItemController : Controller
    {
        private const string EndpointUrl = "https://myaicosmosdb.documents.azure.com:443/";
        private const string PrimaryKey = "NQh9eR5aOJvW5GYQtxWi7Au7zHjjU0jeGzXVkSYw2XFFIScznVcp4dUIdNCOuYXQqSTH4WQWEbMfA9HbLWdgiw==";
        private DocumentClient Client;
        private string DatabaseId = "myai";
        private string CollectionId = "todoitems";


        // GET: Item
        public async Task<ActionResult> Index()
        {
            var items = await GetItemsAsync();
            return View(items);
        }

        public async Task<ActionResult> Item(string id)
        {
            var item = await GetItemAsync(id);
            return View(item);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Completed,Category")] Models.Item item)
        {
            if (ModelState.IsValid)
            {
                item.Id = Guid.NewGuid().ToString();
                item.IsComplete = false;
                var items = await CreateItemAsync(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public async Task<ActionResult> Delete(string id)
        {
            await DelItemAsync(id);
            return RedirectToAction("Index");
        }

        public async Task DelItemAsync(string id)
        {
            Client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            
            await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri
                (DatabaseId, CollectionId, id));
        }

        public async Task<Document> CreateItemAsync(Models.Item item)
        {
            Client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            return await Client.CreateDocumentAsync(UriFactory
                .CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
        }

        public async Task<ActionResult> UpdateItem(Models.Item item)
        {
            await UpdateItemAsync(item);
            return RedirectToAction("Index");
        }

        public async Task<Document> UpdateItemAsync(Models.Item item)
        {
            Client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            item.IsComplete = true;
            return await Client.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(DatabaseId, CollectionId, item.Id), item);
        }

        public async Task<Models.Item> GetItemAsync(string id)
        {
            Client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            Document result = await Client.ReadDocumentAsync(
                UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
            return (Models.Item)(dynamic)result;
        }

        public async Task<List<Models.Item>> GetItemsAsync()
        {
            List<Models.Item> result = new List<Models.Item>();
            Client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            var query = Client.CreateDocumentQuery(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                .AsDocumentQuery();
            while (query.HasMoreResults)
            {
                result.AddRange(await query.ExecuteNextAsync<Models.Item>());
            }
            return result;
        }


    }
}