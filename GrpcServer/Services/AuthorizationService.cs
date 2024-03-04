using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;

namespace GrpcServer.Services;

public class AuthorizationService: Jwt.JwtBase
{
    public async override Task<JwtResponse> Authorize(JwtRequest request, ServerCallContext context)
    {
        var identity = GetIdentity(request.User);
        
        var jwt = new JwtSecurityToken(
            issuer: "MyAuthServer",
            audience: "MyAuthClient",
            claims: identity.Claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.Add(TimeSpan.FromHours(1)),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes("mysupersecret_secretkey!123sdfdsdsfsdfdsfsfdfds")), 
                SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new JwtResponse()
        {
            Token = encodedJwt
        };
    }

    private ClaimsIdentity GetIdentity(string username)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, username)
        };
        ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }
}