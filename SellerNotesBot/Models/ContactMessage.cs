namespace SellerNotesBot.Models
{
    public class ContactMessage
    {
        public int CustomerId { get; set; }

        public long UtcDateTicks { get; set; }

        public string Text { get; set; }

        public string PersonOfCustomer { get; set; }

        public Channel Channel { get; set; }
    }

    public enum Channel
    {
        InPerson,
        Phone,
        ByEmail
    }
}