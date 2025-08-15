using Banking.DAL.Entities.Enums;

namespace Banking.DAL.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата створення

        public int AccountId { get; set; }
        public virtual Account? Account { get; set; }

        public int? TargetAccountId { get; set; }
        public virtual Account? TargetAccount { get; set; }
    }


}
