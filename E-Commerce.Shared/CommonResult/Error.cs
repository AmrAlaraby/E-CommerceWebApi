using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared.CommonResult
{
    public class Error
    {

        public string Code { get; }
        public string Description { get; }
        public ErrorType Type  { get; }
        private Error(string code, string description, ErrorType type)
        {
            Code = code;
            Description = description;
            Type = type;
        }
        public static Error Failure (string code = "General.Failure", string description = "A General Falure Has Occured")
        {
            return new Error(code, description, ErrorType.Failure);
        }
        public static Error Validation(string code = "General.Validation", string description = "Validation Error Has Occured")
        {
            return new Error(code, description , ErrorType.Validation);
        }
        public static Error NotFound(string code = "General.NotFound", string description = "The Requested Resorce Was Not Found")
        {
            return new Error(code , description , ErrorType.NotFound);
        }
        public static Error UnAuthorized(string code = "General.UnAuthorized", string description = "You Are Not Authorized To This EndPoint")
        {
            return new Error(code , description , ErrorType.UnAuthorized);
        }
        public static Error Forbidden(string code = "General.Forbidden", string description = "You Do Not Have Permission To Access That Resouurse")
        {
            return new Error(code , description , ErrorType.Forbidden);
        }
        public static Error InvalidCredentials(string code = "General.InvalidCredentials", string description = "The provided Credentials are Invalid")
        {
            return new Error(code , description , ErrorType.InvalidCredentials);
        }
    }
}
