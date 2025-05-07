using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;

namespace LoginAPI.Attributes
{
    public class HasKeyAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _keyName;

        public HasKeyAttribute(string keyName)
        {
            _keyName = keyName;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var keyAccessClaim = context.HttpContext.User.FindFirst("Keys")?.Value;

            if (string.IsNullOrEmpty(keyAccessClaim))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            Dictionary<string, bool> keyDict;
            try
            {
                keyDict = JsonConvert.DeserializeObject<Dictionary<string, bool>>(keyAccessClaim);
            }
            catch
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!keyDict.TryGetValue(_keyName, out bool hasAccess) || !hasAccess)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
