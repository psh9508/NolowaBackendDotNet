using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NolowaBackendDotNet.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace NolowaBackendDotNet.Core
{
    public abstract class NolowaController : ControllerBase
    {
        protected long GetLoggedInUserAccountIDFromToken()
        {
            bool tryResult = Request.Headers.TryGetValue("Authorization", out StringValues values);

            if (tryResult == false || values.Count == 0)
                throw new InvalidOperationException("Header에서 Token을 찾을 수 없습니다.");

            if (values.Count != 1)
                throw new InvalidOperationException("Header에 Token이 2개 이상 있습니다.");

            var token = values[0].Replace("Bearer ", "");

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            string accountIDFromToken = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            if (accountIDFromToken == null)
                throw new InvalidOperationException("Token에서 사용자(sub)를 찾을 수 없습니다.");

            return Convert.ToInt64(accountIDFromToken);
        }
    }
}
