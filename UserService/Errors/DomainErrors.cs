using FluentResults;

namespace UserService.Errors
{
    public class DomainError : Error
    {
        public DomainError(string code, string message) : base(message)
        {
            Metadata.Add("code", code);
            Code = code;
        }

        public string Code { get; private set; }

    }
}
