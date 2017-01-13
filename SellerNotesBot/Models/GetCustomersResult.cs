using SellerNotesBot.Model;
using System.Collections.Generic;
using System.Net;

namespace SellerNotesBot.Models
{
    public class GetCustomersResult
    {
        public bool IsSuccessStatusCode { get; set; }
        public List<Customer> Customers { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}