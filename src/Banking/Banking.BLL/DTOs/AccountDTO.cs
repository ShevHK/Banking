namespace Banking.BLL.DTOs
{
    public class AccountDTO
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string OwnerName { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<TransactionSummaryDTO> Transactions { get; set; } = new();
    }
}
