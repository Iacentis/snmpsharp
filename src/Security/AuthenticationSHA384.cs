// This file is part of SNMP#NET.
// 
// SNMP#NET is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SNMP#NET is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with SNMP#NET.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Security.Cryptography;

namespace SnmpSharpNet;

/// <summary>
///     SHA-384 Authentication class.
/// </summary>
public class AuthenticationSHA384 : IAuthenticationDigest
{
    private const int authenticationLength = 32;
    private const int digestLength = 48;

    /// <summary>
    ///     Standard constructor.
    /// </summary>
    public AuthenticationSHA384()
    {
    }

    /// <summary>
    ///     Authenticate packet and return authentication parameters value to the caller
    /// </summary>
    /// <param name="authenticationSecret">User authentication secret</param>
    /// <param name="engineId">SNMP agent authoritative engine id</param>
    /// <param name="wholeMessage">Message to authenticate</param>
    /// <returns>Authentication parameters value</returns>
    public byte[] authenticate(Span<byte> authenticationSecret, Span<byte> engineId, Span<byte> wholeMessage)
    {
        var result = new byte[authenticationLength];
        var authKey = PasswordToKey(authenticationSecret, engineId);
        using var sha = new HMACSHA384(authKey);
        sha.WithHashed(wholeMessage, (span, _) =>
        {
            span[..authenticationLength].CopyTo(result);
            return true;
        });
        sha.Clear(); // release resources
        return result;
    }

    /// <summary>
    ///     Authenticate packet and return authentication parameters value to the caller
    /// </summary>
    /// <param name="authKey">Authentication key (not password)</param>
    /// <param name="wholeMessage">Message to authenticate</param>
    /// <returns>Authentication parameters value</returns>
    public byte[] authenticate(Span<byte> authKey, Span<byte> wholeMessage)
    {
        var result = new byte[authenticationLength];

        using var sha = new HMACSHA384(authKey.ToArray());
        sha.WithHashed(wholeMessage, (span, _) =>
        {
            span[..authenticationLength].CopyTo(result);
            return true;
        });
        sha.Clear(); // release resources
        return result;
    }

    /// <summary>
    ///     Verifies correct SHA-384 authentication of the frame. Prior to calling this method, you have to extract
    ///     authentication
    ///     parameters from the wholeMessage and reset authenticationParameters field in the USM information block to 12 0x00
    ///     values.
    /// </summary>
    /// <param name="userPassword">User password</param>
    /// <param name="engineId">Authoritative engine id</param>
    /// <param name="authenticationParameters">Extracted USM authentication parameters</param>
    /// <param name="wholeMessage">Whole message with authentication parameters zeroed (0x00) out</param>
    /// <returns>True if message authentication has passed the check, otherwise false</returns>
    public bool authenticateIncomingMsg(Span<byte> userPassword, Span<byte> engineId,
        Span<byte> authenticationParameters, Span<byte> wholeMessage)
    {
        var authKey = PasswordToKey(userPassword, engineId);
        return authenticateIncomingMsg(authKey, authenticationParameters, wholeMessage);
    }

    /// <summary>
    ///     Verify SHA-384 authentication of a packet.
    /// </summary>
    /// <param name="authKey">Authentication key (not password)</param>
    /// <param name="authenticationParameters">Authentication parameters extracted from the packet being authenticated</param>
    /// <param name="wholeMessage">Entire packet being authenticated</param>
    /// <returns>True on authentication success, otherwise false</returns>
    public bool authenticateIncomingMsg(Span<byte> authKey, Span<byte> authenticationParameters,
        Span<byte> wholeMessage)
    {
        using var sha = new HMACSHA384(authKey.ToArray());
        var auth = authenticationParameters.ToArray();
        var result = sha.WithHashed(wholeMessage, (span, _) => span[..authenticationLength].SequenceEqual(auth));
        sha.Clear(); // release resources
        return result;
    }

    /// <summary>
    ///     Convert user password to acceptable authentication key.
    /// </summary>
    /// <param name="userPassword">User password</param>
    /// <param name="engineID">Authoritative engine id</param>
    /// <returns>Localized authentication key</returns>
    /// <exception cref="SnmpAuthenticationException">Thrown when key length is less then 8 bytes</exception>
    public byte[] PasswordToKey(Span<byte> userPassword, Span<byte> engineID)
    {
        // key length has to be at least 8 bytes long (RFC3414)
        if (userPassword.Length < 8)
            throw new SnmpAuthenticationException("Secret key is too short.");

        var password_index = 0;
        var count = 0;
        using var sha = SHA384.Create();

        /* Use while loop until we've done 1 Megabyte */
        var sourceBuffer = new byte[1048576];
        var buf = new byte[64];
        while (count < 1048576)
        {
            for (var i = 0; i < 64; ++i)
                // Take the next octet of the password, wrapping
                // to the beginning of the password as necessary.
                buf[i] = userPassword[password_index++ % userPassword.Length];
            Buffer.BlockCopy(buf, 0, sourceBuffer, count, buf.Length);
            count += 64;
        }

        var digest = sha.ComputeHash(sourceBuffer);
        var res = sha.ComputeHash([..digest, ..engineID, ..digest]);
        sha.Clear(); // release resources
        return res;
    }

    /// <summary>
    ///     Length of the digest generated by the authentication protocol
    /// </summary>
    public int DigestLength => digestLength;

    /// <summary>
    ///     Length of the authentification header generated by the authentication protocol
    /// </summary>
    public int AuthentificationHeaderLength => authenticationLength;

    /// <summary>
    ///     Return authentication protocol name
    /// </summary>
    public string Name => "HMAC-SHA384";

    /// <summary>
    ///     Compute hash using authentication protocol.
    /// </summary>
    /// <param name="data">Data to hash</param>
    /// <param name="offset">Compute hash from the source buffer offset</param>
    /// <param name="count">Compute hash for source data length</param>
    /// <returns>Hash value</returns>
    public byte[] ComputeHash(Span<byte> data, int offset, int count)
    {
        using var sha = SHA384.Create();
        var res = sha.ComputeHash(data.ToArray(), offset, count);
        sha.Clear();
        return res;
    }
}