namespace Banking.DAL.Entities
{
    public class Account
    {
        public Account() => AccountNumber = Guid.NewGuid().ToString();

        public int Id { get; set; }
        public string AccountNumber { get; private set; }
        public required string OwnerName { get; set; }
        public required string Email { get; set; }
        public decimal InitialBalance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
