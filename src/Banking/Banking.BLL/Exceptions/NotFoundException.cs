namespace Banking.BLL.Exceptions
{
    public class NotFoundException(string message) : BusinessException(message, 404)
    {
    }
}
