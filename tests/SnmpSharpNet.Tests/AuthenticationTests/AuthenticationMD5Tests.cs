using System.Text;

namespace SnmpSharpNet.Tests.AuthenticationTests;

public class AuthenticationMD5Tests
{
    [Test]
    public async Task PasswordToKeyIsConsistent()
    {
        byte[] knownValue = [249, 116, 116, 144, 203, 27, 252, 8, 52, 109, 51, 246, 15, 131, 23, 20];
        var auth = AuthenticationMD5.Instance;
        var password = "passwordu"u8.ToArray();
        var engineId = new byte[] { 0x80, 0x00, 0x13, 0x70, 0x02, 0x01 };
        var key = auth.PasswordToKey(password, engineId);
        await Assert.That(key.Length).IsEqualTo(knownValue.Length);
        Console.WriteLine(string.Join(" ", key));
        for (int i = 0; i < knownValue.Length; i++)
        {
            await Assert.That(key[i]).IsEqualTo(knownValue[i]);
        }
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(128)]
    [Arguments(256)]
    [Arguments(512)]
    [Arguments(1024)]
    [Arguments(2048)]
    public async Task AnAuthenticatedBufferIsVerifiedByAuthenticateIncomingMessage(int packetLength)
    {
        var auth = AuthenticationMD5.Instance;
        var password = "password"u8.ToArray();
        var engineId = new byte[] { 0x80, 0x00, 0x13, 0x70, 0x02, 0x01 };
        var packet = new byte[packetLength];
        Random.Shared.NextBytes(packet);
        var authenticate = auth.Authenticate(password, engineId, packet);
        var result = auth.AuthenticateIncomingMsg(password, engineId, authenticate, packet);
        await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(128)]
    [Arguments(256)]
    [Arguments(512)]
    [Arguments(1024)]
    [Arguments(2048)]
    public async Task AnAuthenticatedBufferWithTheWrongPasswordIsNotVerifiedByAuthenticateIncomingMessage(
        int packetLength)
    {
        var auth = AuthenticationMD5.Instance;
        var password = "password"u8.ToArray();
        var password2 = "password2"u8.ToArray();
        var engineId = new byte[] { 0x80, 0x00, 0x13, 0x70, 0x02, 0x01 };
        var packet = new byte[packetLength];
        Random.Shared.NextBytes(packet);
        var authenticate = auth.Authenticate(password, engineId, packet);
        var result = auth.AuthenticateIncomingMsg(password2, engineId, authenticate, packet);
        await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(128)]
    [Arguments(256)]
    [Arguments(512)]
    [Arguments(1024)]
    [Arguments(2048)]
    public async Task AnAuthenticatedBufferWithTheWrongEngineIdIsNotVerifiedByAuthenticateIncomingMessage(
        int packetLength)
    {
        var auth = AuthenticationMD5.Instance;
        var password = "password"u8.ToArray();
        var engineId = new byte[] { 0x80, 0x00, 0x13, 0x70, 0x02, 0x01 };
        var engineId2 = new byte[] { 0x80, 0x00, 0x13, 0x70, 0x02, 0x02 };
        var packet = new byte[packetLength];
        Random.Shared.NextBytes(packet);
        var authenticate = auth.Authenticate(password, engineId, packet);
        var result = auth.AuthenticateIncomingMsg(password, engineId2, authenticate, packet);
        await Assert.That(result).IsFalse();
    }
}