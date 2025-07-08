namespace SnmpSharpNet.Tests;

public class SnmpV3PacketTests
{
    [Test]
    [MatrixDataSource]
    public async Task EncodedBytesDecodeToSamePacket(
        [Matrix(true, false)] bool @private,
        [Matrix(true, false)] bool auth,
        [Matrix(AuthenticationDigests.SHA1, AuthenticationDigests.SHA224, AuthenticationDigests.SHA256,
            AuthenticationDigests.SHA384, AuthenticationDigests.SHA512, AuthenticationDigests.MD5)]
        AuthenticationDigests digests,
        [Matrix(PrivacyProtocols.None, PrivacyProtocols.AES128, PrivacyProtocols.AES192, PrivacyProtocols.AES256,
            PrivacyProtocols.TripleDES, PrivacyProtocols.DES)]
        PrivacyProtocols protocols)
    {
        var pdu = new ScopedPdu(PduType.GetNext, 123);
        var parameters = new SecureAgentParameters();
        parameters.EngineId.Set($"{123}");
        parameters.EngineTime.Set($"{234}");
        parameters.EngineBoots.Set($"{457}");

        var packet = new SnmpV3Packet(parameters, pdu);
        VbCollection vbs =
        [
            new Vb(new Oid("1.3.2"), new Integer32(123)),
            new Vb(new Oid("1.3.3"), new Integer32(234)),
            new Vb(new Oid("1.3.4"), new Integer32(345))
        ];
        SetAuth(@private, auth, packet, digests, protocols);

        packet.Pdu.SetVbList(vbs);
        packet.Pdu.RequestId = 123;
        packet.Pdu.ErrorIndex = 567;
        packet.Pdu.ErrorStatus = 6879;
        var bytes = packet.Encode();
        var newPacket = new SnmpV3Packet();
        SetAuth(@private, auth, newPacket, digests, protocols);


        newPacket.Decode(bytes);
        await Assert.That(newPacket.ToString()).IsEqualTo(packet.ToString());
    }

    [Test]
    [MatrixDataSource]
    public async Task CanEncodeFromAgentParameters(
        [Matrix(true, false)] bool @private,
        [Matrix(true, false)] bool auth,
        [Matrix(AuthenticationDigests.SHA1, AuthenticationDigests.SHA224, AuthenticationDigests.SHA256,
            AuthenticationDigests.SHA384, AuthenticationDigests.SHA512, AuthenticationDigests.MD5)]
        AuthenticationDigests digests,
        [Matrix(PrivacyProtocols.None, PrivacyProtocols.AES128, PrivacyProtocols.AES192, PrivacyProtocols.AES256,
            PrivacyProtocols.TripleDES, PrivacyProtocols.DES)]
        PrivacyProtocols protocols,
        [Matrix(true, false)] bool cache)

    {
        var parameters = new SecureAgentParameters();
        SetAuth(@private, auth, parameters, digests, protocols, cache);
        VbCollection vbs =
        [
            new Vb(new Oid("1.3.2"), new Integer32(123)),
            new Vb(new Oid("1.3.3"), new Integer32(234)),
            new Vb(new Oid("1.3.4"), new Integer32(345))
        ];
        var pdu = new Pdu(vbs, PduType.GetBulk, 123);
        var outPdu = new ScopedPdu(pdu);
        var packet = new SnmpV3Packet(outPdu, parameters);
        Span<byte> bytes = stackalloc byte[packet.ByteLength];
        packet.MessageId = 123;
        var count = packet.Encode(bytes);
        var newPacket = new SnmpV3Packet(bytes[..count], parameters);
        await Assert.That(newPacket.ToString()).IsEqualTo(packet.ToString());
    }

    [Test]
    [MatrixDataSource]
    public async Task FromOld(
        [Matrix(true, false)] bool @private,
        [Matrix(true, false)] bool auth,
        [Matrix(AuthenticationDigests.SHA1, AuthenticationDigests.SHA256,
            AuthenticationDigests.SHA384, AuthenticationDigests.SHA512, AuthenticationDigests.MD5)]
        AuthenticationDigests digests,
        [Matrix(PrivacyProtocols.None, PrivacyProtocols.AES128, PrivacyProtocols.AES192, PrivacyProtocols.AES256,
            PrivacyProtocols.TripleDES, PrivacyProtocols.DES)]
        PrivacyProtocols protocols)
    {
        var dir = Directory.GetCurrentDirectory();
        if (dir is null) throw new Exception("Could not get current directory");
        var testsFolder = dir.IndexOf("tests", StringComparison.Ordinal);
        if (testsFolder == -1) throw new Exception("Could not find tests folder");
        var path = dir[..testsFolder];
        var file = Path.Combine(path, "tests", "resources", "old", "parameters", "private_" + @private, "auth_" + auth,
            "digest_" + digests, "protocol" + protocols, "packet");
        Console.WriteLine($"reading from {file}");
        var bytes = await File.ReadAllBytesAsync(file);
        var newPacket = new SnmpV3Packet();
        SetAuth(@private, auth, newPacket, digests, protocols);
        newPacket.Decode(bytes);
    }

    [Test]
    [MatrixDataSource]
    public async Task WriteNew(
        [Matrix(true, false)] bool @private,
        [Matrix(true, false)] bool auth,
        [Matrix(AuthenticationDigests.SHA1, AuthenticationDigests.SHA256,
            AuthenticationDigests.SHA384, AuthenticationDigests.SHA512, AuthenticationDigests.MD5)]
        AuthenticationDigests digests,
        [Matrix(PrivacyProtocols.None, PrivacyProtocols.AES128, PrivacyProtocols.AES192, PrivacyProtocols.AES256,
            PrivacyProtocols.TripleDES, PrivacyProtocols.DES)]
        PrivacyProtocols protocols)
    {
        var pdu = new ScopedPdu(PduType.GetNext, 123);
        var parameters = new SecureAgentParameters();
        parameters.EngineId.Set($"{123}");
        parameters.EngineTime.Set($"{234}");
        parameters.EngineBoots.Set($"{457}");

        var packet = new SnmpV3Packet(parameters, pdu);
        VbCollection vbs =
        [
            new Vb(new Oid("1.3.2"), new Integer32(123)),
            new Vb(new Oid("1.3.3"), new Integer32(234)),
            new Vb(new Oid("1.3.4"), new Integer32(345))
        ];
        SetAuth(@private, auth, packet, digests, protocols);

        packet.Pdu.SetVbList(vbs);
        packet.Pdu.RequestId = 123;
        packet.Pdu.ErrorIndex = 567;
        packet.Pdu.ErrorStatus = 6879;
        var bytes = packet.Encode();
        var dir = Directory.GetCurrentDirectory();
        if (dir is null) throw new Exception("Could not get current directory");
        var testsFolder = dir.IndexOf("tests", StringComparison.Ordinal);
        if (testsFolder == -1) throw new Exception("Could not find tests folder");
        var path = dir[..testsFolder];
        var file = Path.Combine(path, "tests", "resources", "new", "parameters", "private_" + @private, "auth_" + auth,
            "digest_" + digests, "protocol" + protocols, "packet");
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        Console.WriteLine($"writing to {file}");
        await File.WriteAllBytesAsync(file, bytes);
    }

    private void SetAuth(bool @private, bool auth, SnmpV3Packet packet, AuthenticationDigests digests,
        PrivacyProtocols protocols)
    {
        @private &= protocols != PrivacyProtocols.None;
        auth &= digests != AuthenticationDigests.None;
        var username = "admin"u8.ToArray();
        var authenticationPassword = "someFakePassword"u8.ToArray();
        var privacyPassword = "someFakePassword"u8.ToArray();
        var engineId = "TheEngineID"u8.ToArray();

        switch (auth)
        {
            case true when @private:
                packet.AuthPriv(username, authenticationPassword, digests, privacyPassword,
                    protocols);
                break;
            case true:
                packet.AuthNoPriv(username, authenticationPassword, digests);
                break;
            default:
                packet.NoAuthNoPriv(username);
                break;
        }

        packet.IsReportable = true;
        packet.SetEngineTime(123, 234);
        packet.SetEngineId(engineId);
    }

    private void SetAuth(bool @private, bool auth, SecureAgentParameters parameters, AuthenticationDigests digests,
        PrivacyProtocols protocols, bool cache)
    {
        @private &= protocols != PrivacyProtocols.None;
        auth &= digests != AuthenticationDigests.None;
        const string username = "admin";
        const string authenticationPassword = "someFakePassword";
        const string privacyPassword = "someFakePassword";
        const string engineId = "TheEngineID";

        switch (auth)
        {
            case true when @private:
                parameters.authPriv(username, digests, authenticationPassword, protocols, privacyPassword);
                break;
            case true:
                parameters.authNoPriv(username, digests, authenticationPassword);
                break;
            default:
                parameters.noAuthNoPriv(username);
                break;
        }

        parameters.EngineId.Set(engineId);
        parameters.EngineTime.Set("123");
        parameters.EngineBoots.Set("234");
        if (cache) parameters.BuildCachedSecurityKeys();
    }
}