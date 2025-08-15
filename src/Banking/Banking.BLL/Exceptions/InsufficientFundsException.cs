namespace Banking.BLL.Exceptions
{
    public class InsufficientFundsException(string message) : BusinessException(message, 400)
    {
    }
}
