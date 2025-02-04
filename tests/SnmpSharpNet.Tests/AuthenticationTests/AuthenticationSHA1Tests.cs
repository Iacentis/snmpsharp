using System.Text;

namespace SnmpSharpNet.Tests.AuthenticationTests;

public class AuthenticationSHA1Tests
{
    [Test]
    public async Task PasswordToKeyIsConsistent()
    {
        byte[] knownValue = [244, 32, 233, 153, 218, 11, 16, 149, 144, 15, 37, 54, 199, 4, 195, 123, 70, 5, 174, 226];
        var auth = new AuthenticationSHA1();
        var password = "password"u8.ToArray();
        var engineId = new byte[] { 0x80, 0x00, 0x13, 0x70, 0x02, 0x01 };
        var key = auth.PasswordToKey(password, engineId);
        Console.WriteLine(ArrayString(key));
        await Assert.That(key.Length).IsEqualTo(knownValue.Length);
        for (int i = 0; i < knownValue.Length; i++)
        {
            await Assert.That(key[i]).IsEqualTo(knownValue[i]);
        }
    }

    private string ArrayString(byte[] bytes)
    {
        StringBuilder builder = new();
        builder.Append('[');
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i]);
            if (i < bytes.Length - 1)
            {
                builder.Append(", ");
            }
        }

        builder.Append(']');
        return builder.ToString();
    }
}