using SellerNotesBot.Forms;
using SellerNotesBot.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SellerNotesBot.Model;

namespace SellerNotesBot.Services
{
    public class ContactService
    {
        private HttpClient _client;
        private string _baseUrl;

        public ContactService(string bearerToken)
        {
            _baseUrl = "https://service-url";
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
        public async Task<SaveContactResult> SaveContactAsync(ContactForm form)
        {
            return await SaveContactAsync(new ContactMessage
            {
                Channel = ((Channel)((int)form.CommunicationChannel) - 1),
                CustomerId = form.CustomerId,
                PersonOfCustomer = form.PersonName,
                Text = form.Details,
                UtcDateTicks = form.UtcDateTicks
            });
        }
        public async Task<SaveContactResult> SaveContactAsync(ContactMessage message)
        {
            // Call a web service to save contact to database, CRM or other service.
            // Check for errors and if there are any, modify the SaveContactResult accordingly.

            // This mock returns always true.
            return new SaveContactResult
            {
                IsSuccessStatusCode = true
            };
        }
        public async Task<GetCustomersResult> GetCustomersAsync(string namePart = null, int? code = null, string vat = null)
        {
            // Call a web service to get a list of Customers. Intentionally omitted.

            // Return the list.
            var result = new GetCustomersResult {
                IsSuccessStatusCode = true,
                StatusCode = System.Net.HttpStatusCode.OK // mock
            };

            // Mock data for this sample.
            result.Customers = new List<Customer>()
            {
                new Customer()
                {
                    Code = 1,
                    VAT = "ABCD",
                    Id = 1,
                    Name = "Test customer"
                }
            };

            return await Task.FromResult(result); // hack to use async for the mock
        }
    }
}