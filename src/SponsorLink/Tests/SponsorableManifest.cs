using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace Devlooped.Sponsors;

/// <summary>
/// The serializable manifest of a sponsorable user, as persisted 
/// in the .github/sponsorlink.jwt file.
/// </summary>
public class SponsorableManifest
{
    /// <summary>
    /// Overall manifest status.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// SponsorLink manifest is invalid.
        /// </summary>
        Invalid,
        /// <summary>
        /// The manifest has an audience that doesn't match the sponsorable account.
        /// </summary>
        AccountMismatch,
        /// <summary>
        /// SponsorLink manifest not found for the given account, so it's not supported.
        /// </summary>
        NotFound,
        /// <summary>
        /// Manifest was successfully fetched and validated.
        /// </summary>
        OK,
    }

    /// <summary>
    /// Creates a new manifest with a new RSA key pair.
    /// </summary>
    public static SponsorableManifest Create(Uri issuer, Uri[] audience, string clientId)
    {
        var rsa = RSA.Create(3072);
        var pub = Convert.ToBase64String(rsa.ExportRSAPublicKey());

        return new SponsorableManifest(issuer, audience, clientId, new RsaSecurityKey(rsa), pub);
    }

    public static async Task<(Status, SponsorableManifest?)> FetchAsync(string sponsorable, string? branch, HttpClient? http = default)
    {
        // Try to detect sponsorlink manifest in the sponsorable .github repo
        var url = $"https://github.com/{sponsorable}/.github/raw/{branch ?? "main"}/sponsorlink.jwt";

        // Manifest should be public, so no need for any special HTTP client.
        using (http ??= new HttpClient())
        {
            var response = await http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return (Status.NotFound, default);

            var jwt = await response.Content.ReadAsStringAsync();
            if (!TryRead(jwt, out var manifest, out var missingClaim))
                return (Status.Invalid, default);

            // Manifest audience should match the sponsorable account to avoid weird issues?
            if (sponsorable != manifest.Sponsorable)
                return (Status.AccountMismatch, default);

            return (Status.OK, manifest);
        }
    }

    /// <summary>
    /// Parses a JWT into a <see cref="SponsorableManifest"/>.
    /// </summary>
    /// <param name="jwt">The JWT containing the sponsorable information.</param>
    /// <param name="manifest">The parsed manifest, if not required claims are missing.</param>
    /// <param name="missingClaim">The missing required claim, if any.</param>
    /// <returns>A validated manifest.</returns>
    public static bool TryRead(string jwt, [NotNullWhen(true)] out SponsorableManifest? manifest, out string? missingClaim)
    {
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        missingClaim = null;
        manifest = default;

        if (!handler.CanReadToken(jwt))
            return false;

        var token = handler.ReadJwtToken(jwt);
        var issuer = token.Issuer;

        if (token.Audiences.FirstOrDefault(x => x.StartsWith("https://github.com/")) is null)
        {
            missingClaim = "aud";
            return false;
        }

        if (token.Claims.FirstOrDefault(c => c.Type == "client_id")?.Value is not string clientId)
        {
            missingClaim = "client_id";
            return false;
        }

        if (token.Claims.FirstOrDefault(c => c.Type == "pub")?.Value is not string pub)
        {
            missingClaim = "pub";
            return false;
        }

        if (token.Claims.FirstOrDefault(c => c.Type == "sub_jwk")?.Value is not string jwk)
        {
            missingClaim = "sub_jwk";
            return false;
        }

        var key = new JsonWebKeySet { Keys = { JsonWebKey.Create(jwk) } }.GetSigningKeys().First();
        manifest = new SponsorableManifest(new Uri(issuer), token.Audiences.Select(x => new Uri(x)).ToArray(), clientId, key, pub);

        return true;
    }

    public SponsorableManifest(Uri issuer, Uri[] audience, string clientId, SecurityKey publicKey, string publicRsaKey)
    {
        Issuer = issuer.AbsoluteUri;
        Audience = audience.Select(a => a.AbsoluteUri.TrimEnd('/')).ToArray();
        ClientId = clientId;
        SecurityKey = publicKey;
        PublicKey = publicRsaKey;
        Sponsorable = audience.Where(x => x.Host == "github.com").Select(x => x.Segments.LastOrDefault()?.TrimEnd('/')).FirstOrDefault() ??
            throw new ArgumentException("At least one of the intended audience must be a GitHub sponsors URL.");
    }

    /// <summary>
    /// Converts (and optionally signs) the manifest into a JWT. Never exports the private key.
    /// </summary>
    /// <param name="signing">Optional credentials when signing the resulting manifest. Defaults to the <see cref="SecurityKey"/> if it has a private key.</param>
    /// <returns>The JWT manifest.</returns>
    public string ToJwt(SigningCredentials? signing = default)
    {
        var jwk = JsonWebKeyConverter.ConvertFromSecurityKey(SecurityKey);

        // Automatically sign if the manifest was created with a private key
        if (SecurityKey is RsaSecurityKey rsa && rsa.PrivateKeyStatus == PrivateKeyStatus.Exists)
        {
            signing ??= new SigningCredentials(rsa, SecurityAlgorithms.RsaSha256);

            // Ensure we never serialize the private key
            jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(rsa.Rsa.ExportParameters(false)));
        }

        var token = new JwtSecurityToken(
            claims:
                new[] { new Claim(JwtRegisteredClaimNames.Iss, Issuer) }
                .Concat(Audience.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)))
                .Concat(
                [
                    // See https://www.rfc-editor.org/rfc/rfc7519.html#section-4.1.6
                    new(JwtRegisteredClaimNames.Iat, Math.Truncate((DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds).ToString()),
                    new("client_id", ClientId),
                    // non-standard claim containing the base64-encoded public key
                    new("pub", PublicKey),
                    // standard claim, serialized as a JSON string, not an encoded JSON object
                    new("sub_jwk", JsonSerializer.Serialize(jwk, JsonOptions.JsonWebKey), JsonClaimValueTypes.Json),
                ]),
            signingCredentials: signing);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Sign the JWT claims with the provided RSA key.
    /// </summary>
    public string Sign(IEnumerable<Claim> claims, RSA rsa, TimeSpan? expiration = default)
        => Sign(claims, new RsaSecurityKey(rsa), expiration);

    public string Sign(IEnumerable<Claim> claims, RsaSecurityKey? key = default, TimeSpan? expiration = default)
    {
        var rsa = key ?? SecurityKey as RsaSecurityKey;
        if (rsa?.PrivateKeyStatus != PrivateKeyStatus.Exists)
            throw new NotSupportedException("No private key found to sign the manifest.");

        var signing = new SigningCredentials(rsa, SecurityAlgorithms.RsaSha256);

        var expirationDate = expiration != null ?
            DateTime.UtcNow.Add(expiration.Value) :
            // Expire the first day of the next month
            new DateTime(
                DateTime.UtcNow.AddMonths(1).Year,
                DateTime.UtcNow.AddMonths(1).Month, 1,
                // Use current time so they don't expire all at the same time
                DateTime.UtcNow.Hour,
                DateTime.UtcNow.Minute,
                DateTime.UtcNow.Second,
                DateTime.UtcNow.Millisecond,
                DateTimeKind.Utc);

        var tokenClaims = claims.Where(x => x.Type != JwtRegisteredClaimNames.Iat && x.Type != JwtRegisteredClaimNames.Exp).ToList();

        // See https://www.rfc-editor.org/rfc/rfc7519.html#section-4.1.6
        tokenClaims.Add(new(JwtRegisteredClaimNames.Iat, Math.Truncate((DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds).ToString()));

        if (tokenClaims.Find(c => c.Type == JwtRegisteredClaimNames.Iss) is { } issuer)
        {
            if (issuer.Value != Issuer)
                throw new ArgumentException($"The received claims contain an incompatible 'iss' claim. If present, the claim must contain the value '{Issuer}' but was '{issuer.Value}'.");
        }
        else
        {
            tokenClaims.Insert(0, new(JwtRegisteredClaimNames.Iss, Issuer));
        }

        if (tokenClaims.Find(c => c.Type == "client_id") is { } clientId)
        {
            if (clientId.Value != ClientId)
                throw new ArgumentException($"The received claims contain an incompatible 'client_id' claim. If present, the claim must contain the value '{ClientId}' but was '{clientId.Value}'.");
        }
        else
        {
            tokenClaims.Add(new("client_id", ClientId));
        }

        // Avoid duplicating audience claims
        foreach (var audience in Audience)
        {
            // Always compare ignoring trailing /
            if (tokenClaims.Find(c => c.Type == JwtRegisteredClaimNames.Aud && c.Value.TrimEnd('/') == audience.TrimEnd('/')) == null)
                tokenClaims.Insert(1, new(JwtRegisteredClaimNames.Aud, audience));
        }

        // The other claims (client_id, pub, sub_jwk) claims are mostly for the SL manifest itself,
        // not for the user, so for now we don't add them. 

        // Don't allow mismatches of public manifest key and the one used to sign, to avoid 
        // weird run-time errors verifiying manifests that were signed with a different key.
        var pubKey = Convert.ToBase64String(rsa.Rsa.ExportRSAPublicKey());
        if (pubKey != PublicKey)
            throw new ArgumentException($"Cannot sign with a private key that does not match the manifest public key.");

        var jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            claims: tokenClaims,
            expires: expirationDate,
            signingCredentials: signing
        ));

        return jwt;
    }

    public ClaimsPrincipal Validate(string jwt, out SecurityToken? token) => new JwtSecurityTokenHandler().ValidateToken(jwt, new TokenValidationParameters
    {
        RequireExpirationTime = true,
        // NOTE: setting this to false allows checking sponsorships even when the manifest is expired. 
        // This might be useful if package authors want to extend the manifest lifetime beyond the default 
        // 30 days and issue a warning on expiration, rather than an error and a forced sync.
        // If this is not set (or true), a SecurityTokenExpiredException exception will be thrown.
        ValidateLifetime = false,
        RequireAudience = true,
        // At least one of the audiences must match the manifest audiences
        AudienceValidator = (audiences, _, _) => Audience.Intersect(audiences.Select(x => x.TrimEnd('/'))).Any(),
        ValidIssuer = Issuer,
        IssuerSigningKey = SecurityKey,
    }, out token);

    /// <summary>
    /// Gets the GitHub sponsorable account.
    /// </summary>
    public string Sponsorable { get; }

    /// <summary>
    /// The web endpoint that issues signed JWT to authenticated users.
    /// </summary>
    /// <remarks>
    /// See https://www.rfc-editor.org/rfc/rfc7519.html#section-4.1.1
    /// </remarks>
    public string Issuer { get; }

    /// <summary>
    /// The audience for the JWT, which includes the sponsorable account and potentially other sponsoring platforms.
    /// </summary>
    /// <remarks>
    /// See https://www.rfc-editor.org/rfc/rfc7519.html#section-4.1.3
    /// </remarks>
    public string[] Audience { get; }

    /// <summary>
    /// The OAuth client ID (i.e. GitHub OAuth App ID) that is used to 
    /// authenticate the user.
    /// </summary>
    /// <remarks>
    /// See https://www.rfc-editor.org/rfc/rfc8693.html#name-client_id-client-identifier
    /// </remarks>
    public string ClientId { get; internal set; }

    /// <summary>
    /// Public key that can be used to verify JWT signatures.
    /// </summary>
    public string PublicKey { get; }

    /// <summary>
    /// Public key in a format that can be used to verify JWT signatures.
    /// </summary>
    public SecurityKey SecurityKey { get; }

    /// <inheritdoc/>
    public override int GetHashCode() => new HashCode().Add(Issuer, ClientId, PublicKey).AddRange(Audience).ToHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SponsorableManifest other && GetHashCode() == other.GetHashCode();
}
