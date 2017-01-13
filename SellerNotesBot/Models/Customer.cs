using System;

namespace SellerNotesBot.Model
{
    [Serializable]
    public class Customer
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }

        public string VAT { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}