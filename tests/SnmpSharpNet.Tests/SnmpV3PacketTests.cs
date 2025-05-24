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
        PrivacyProtocols protocols)

    {
        var parameters = new SecureAgentParameters();
        SetAuth(@private, auth, parameters, digests, protocols);
        VbCollection vbs =
        [
            new Vb(new Oid("1.3.2"), new Integer32(123)),
            new Vb(new Oid("1.3.3"), new Integer32(234)),
            new Vb(new Oid("1.3.4"), new Integer32(345))
        ];
        var pdu = new Pdu(vbs, PduType.GetBulk, 123);
        var outPdu = new ScopedPdu(pdu);
        var packet = new SnmpV3Packet(outPdu);
        parameters.InitializePacket(packet);
        var bytes = packet.Encode();
        var newPacket = new SnmpV3Packet();
        parameters.InitializePacket(newPacket);
        newPacket.Decode(bytes);
        await Assert.That(newPacket.ToString()).IsEqualTo(packet.ToString());
    }

    private void SetAuth(bool @private, bool auth, SnmpV3Packet packet, AuthenticationDigests digests,
        PrivacyProtocols protocols)
    {
        @private &= protocols != PrivacyProtocols.None;
        auth &= digests != AuthenticationDigests.None;
        var username = "admin"u8.ToArray();
        var authenticationPassword = "someFakePassword"u8.ToArray();
        var privacyPassword = "someFakePassword"u8.ToArray();
        var engineID = "TheEngineID"u8.ToArray();

        switch (auth)
        {
            case true when @private:
                packet.authPriv(username, authenticationPassword, digests, privacyPassword,
                    protocols);
                break;
            case true:
                packet.authNoPriv(username, authenticationPassword, digests);
                break;
            default:
                packet.NoAuthNoPriv(username);
                break;
        }

        packet.IsReportable = true;
        packet.SetEngineTime(123, 234);
        packet.SetEngineId(engineID);
    }

    private void SetAuth(bool @private, bool auth, SecureAgentParameters parameters, AuthenticationDigests digests,
        PrivacyProtocols protocols)
    {
        @private &= protocols != PrivacyProtocols.None;
        auth &= digests != AuthenticationDigests.None;
        var username = "admin";
        var authenticationPassword = "someFakePassword";
        var privacyPassword = "someFakePassword";
        var engineID = "TheEngineID";

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

        parameters.EngineId.Set(engineID);
        parameters.EngineTime.Set("123");
        parameters.EngineBoots.Set("234");
    }
}