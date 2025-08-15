namespace Banking.BLL.Models.Account
{
    public class CreateAccountRequest
    {
        public required string OwnerName { get; set; }
        public required string Email { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
