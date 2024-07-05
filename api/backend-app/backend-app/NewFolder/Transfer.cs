namespace backend_app.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime TransferDate { get; set; }
    }
}
