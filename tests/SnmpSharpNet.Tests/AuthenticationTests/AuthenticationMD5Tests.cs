using System.Text;

namespace SnmpSharpNet.Tests.AuthenticationTests;

public class AuthenticationMD5Tests
{
    [Test]
    public async Task PasswordToKeyIsConsistent()
    {
        byte[] knownValue = [240, 215, 156, 98, 95, 151, 52, 129, 108, 91, 220, 215, 186, 232, 71, 162];
        var auth = new AuthenticationMD5();
        var password = "passwordu"u8.ToArray();
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