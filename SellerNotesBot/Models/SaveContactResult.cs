using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SellerNotesBot.Models
{
    public class SaveContactResult
    {
        public bool IsSuccessStatusCode { get; set; }
        public bool HasErrors { get { return !String.IsNullOrEmpty(ErrorMessage); } }
        public string ErrorMessage { get; set; }
    }
}