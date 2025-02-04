namespace SnmpSharpNet.Tests;

public class SnmpV3PacketTests
{
    [Test]
    [Arguments(true, true, AuthenticationDigests.SHA512, PrivacyProtocols.AES128)]
    [Arguments(true, true, AuthenticationDigests.SHA384, PrivacyProtocols.AES128)]
    [Arguments(true, true, AuthenticationDigests.SHA256, PrivacyProtocols.AES128)]
    [Arguments(true, true, AuthenticationDigests.SHA1, PrivacyProtocols.AES128)]
    [Arguments(true, true, AuthenticationDigests.MD5, PrivacyProtocols.AES128)]
    [Arguments(true, true, AuthenticationDigests.SHA512, PrivacyProtocols.AES192)]
    [Arguments(true, true, AuthenticationDigests.SHA384, PrivacyProtocols.AES192)]
    [Arguments(true, true, AuthenticationDigests.SHA256, PrivacyProtocols.AES192)]
    [Arguments(true, true, AuthenticationDigests.SHA1, PrivacyProtocols.AES192)]
    [Arguments(true, true, AuthenticationDigests.MD5, PrivacyProtocols.AES192)]
    [Arguments(true, true, AuthenticationDigests.SHA512, PrivacyProtocols.AES256)]
    [Arguments(true, true, AuthenticationDigests.SHA384, PrivacyProtocols.AES256)]
    [Arguments(true, true, AuthenticationDigests.SHA256, PrivacyProtocols.AES256)]
    [Arguments(true, true, AuthenticationDigests.SHA1, PrivacyProtocols.AES256)]
    [Arguments(true, true, AuthenticationDigests.MD5, PrivacyProtocols.AES256)]
    [Arguments(false, true, AuthenticationDigests.SHA512, PrivacyProtocols.None)]
    [Arguments(false, true, AuthenticationDigests.SHA384, PrivacyProtocols.None)]
    [Arguments(false, true, AuthenticationDigests.SHA256, PrivacyProtocols.None)]
    [Arguments(false, true, AuthenticationDigests.SHA1, PrivacyProtocols.None)]
    [Arguments(false, true, AuthenticationDigests.MD5, PrivacyProtocols.None)]
    [Arguments(false, false, AuthenticationDigests.None, PrivacyProtocols.None)]
    public async Task EncodedBytesDecodeToSamePacket(bool @private, bool auth, AuthenticationDigests digests,
        PrivacyProtocols protocols)
    {
        var pdu = new ScopedPdu(PduType.GetNext, 123);
        var parameters = new SecureAgentParameters();
        parameters.EngineId.Set($"{123}");
        parameters.EngineTime.Set($"{234}");
        parameters.EngineBoots.Set($"{457}");
        Span<byte> someAuthKey = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];

        var packet = new SnmpV3Packet(parameters, pdu);
        VbCollection vbs =
        [
            new Vb(new Oid("1.3.2"), new Integer32(123)),
            new Vb(new Oid("1.3.3"), new Integer32(234)),
            new Vb(new Oid("1.3.4"), new Integer32(345))
        ];
        SetAuth(@private, auth, packet, digests, protocols);
        var authProto = SnmpSharpNet.Authentication.GetInstance(digests);

        packet.Pdu.SetVbList(vbs);
        packet.Pdu.RequestId = 123;
        packet.Pdu.ErrorIndex = 567;
        packet.Pdu.ErrorStatus = 6879;
        var bytes = packet.encode();

        if (authProto is not null)
        {
            var authBytes = authProto.authenticate(someAuthKey, bytes);
            var success = authProto.authenticateIncomingMsg(someAuthKey, authBytes, bytes);
            await Assert.That(success).IsTrue();
        }

        var newPacket = new SnmpV3Packet();
        SetAuth(@private, auth, newPacket, digests, protocols);


        newPacket.decode(bytes, bytes.Length);
        await Assert.That(newPacket.ToString()).IsEqualTo(packet.ToString());
    }

    private void SetAuth(bool @private, bool auth, SnmpV3Packet packet, AuthenticationDigests digests,
        PrivacyProtocols protocols)
    {
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
}