using Banking.DAL.Entities.Enums;

namespace Banking.BLL.DTOs
{
    public class TransactionSummaryDTO
    {
        public int Id { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? TargetAccountId { get; set; }
        public string TargetAccountOwnerName { get; set; }
    }
}
