namespace SnmpSharpNet.Tests.AuthenticationTests;

public class AuthenticationSHA348Tests
{
    [Test]
    public async Task PasswordToKeyIsConsistent()
    {
        byte[] knownValue =
        [
            34, 83, 246, 66, 112, 242, 143, 0, 36, 57, 152, 177, 140, 125, 140, 48, 191, 57, 227, 38, 102, 59, 7, 135,
            67, 183, 51, 4, 188, 182, 144, 145, 44, 3, 95, 238, 232, 62, 158, 49, 41, 5, 165, 255, 136, 111, 183, 120
        ];
        var auth = new AuthenticationSHA384();
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