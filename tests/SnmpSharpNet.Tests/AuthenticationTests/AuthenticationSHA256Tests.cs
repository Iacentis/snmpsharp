namespace SnmpSharpNet.Tests.AuthenticationTests;

public class AuthenticationSHA256Tests
{
    [Test]
    public async Task PasswordToKeyIsConsistent()
    {
        byte[] knownValue =
        [
            116, 9, 58, 147, 13, 7, 97, 62, 128, 14, 0, 188, 130, 201, 255, 236, 24, 230, 77, 226, 77, 255, 214, 16,
            214, 145, 145, 204, 157, 149, 43, 62
        ];
        var auth = new AuthenticationSHA256();
        var password = "password"u8.ToArray();
        var engineId = new byte[] { 0x80, 0x00, 0x13, 0x70, 0x02, 0x01 };
        var key = auth.PasswordToKey(password, engineId);
        await Assert.That(key.Length).IsEqualTo(knownValue.Length);
        for (int i = 0; i < knownValue.Length; i++)
        {
            await Assert.That(key[i]).IsEqualTo(knownValue[i]);
        }
    }
}