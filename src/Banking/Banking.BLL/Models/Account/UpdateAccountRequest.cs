namespace Banking.BLL.Models.Account
{
    public class UpdateAccountRequest
    {
        public int Id { get; set; }
        public required string OwnerName { get; set; }
        public required string Email { get; set; }
    }
}
