using System.Text;

namespace SnmpSharpNet.Tests.AuthenticationTests;

public class AuthenticationSHA512Tests
{
    [Test]
    public async Task PasswordToKeyIsConsistent()
    {
        byte[] knownValue =
        [
            249, 78, 3, 182, 62, 41, 12, 233, 164, 6, 238, 143, 140, 147, 248, 80, 80, 121, 72, 227, 42, 117, 240, 64,
            250, 52, 115, 17, 185, 223, 179, 209, 33, 178, 95, 254, 231, 97, 4, 162, 188, 128, 173, 35, 41, 170, 168,
            84, 207, 17, 165, 108, 75, 132, 191, 202, 20, 136, 223, 157, 41, 146, 175, 150
        ];
        var auth = new AuthenticationSHA512();
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