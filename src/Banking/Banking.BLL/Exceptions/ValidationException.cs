namespace Banking.BLL.Exceptions
{
    public class ValidationException(string message) : BusinessException(message, 400)
    {
    }
}
