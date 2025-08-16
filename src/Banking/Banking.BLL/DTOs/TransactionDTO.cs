using Banking.DAL.Entities;
using Banking.DAL.Entities.Enums;

namespace Banking.BLL.DTOs
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? TargetAccountId { get; set; }
        public string AccountOwnerName { get; set; }
        public string AccountNumber { get; set; }
        public string TargetAccountOwnerName { get; set; }
        public string TargetAccountNumber { get; set; }
    }
}
