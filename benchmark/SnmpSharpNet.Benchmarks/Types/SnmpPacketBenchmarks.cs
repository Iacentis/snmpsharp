using BenchmarkDotNet.Attributes;

namespace SnmpSharpNet.Tests;

[MemoryDiagnoser]
public class SnmpPacketBenchmarks
{
    private SnmpV1Packet _1Initial = new();
    private SnmpV1Packet _1New = new();
    private byte[] _1Bytes = [];
    private SnmpV2Packet _2Initial = new();
    private SnmpV2Packet _2New = new();
    private byte[] _2Bytes = [];
    private SnmpV3Packet _3Initial = new();
    private SnmpV3Packet _3New = new();
    private byte[] _3Bytes = [];

    [Params(false)] public bool Private { get; set; }
    [Params(false)] public bool Auth { get; set; }

    [Params(AuthenticationDigests.None)] public AuthenticationDigests Digest { get; set; }

    [Params(PrivacyProtocols.None)] public PrivacyProtocols Protocol { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        SetupV1();
        SetupV2();
        SetupV3();
    }

    private void SetupV1()
    {
        var a = 123;
        var b = 234;
        var c = 345;
        var packet = new SnmpV1Packet("public");
        VbCollection vbs =
        [
            new Vb(new Oid("1.3.2"), new Integer32(a)),
            new Vb(new Oid("1.3.3"), new Integer32(b)),
            new Vb(new Oid("1.3.4"), new Integer32(c))
        ];
        packet.Pdu.SetVbList(vbs);
        packet.Pdu.RequestId = a * b * c;
        packet.Pdu.ErrorIndex = a + b + c;
        packet.Pdu.ErrorStatus = a - b + c;
        _1Initial = packet;
        _1Bytes = packet.Encode();
        _1New = new SnmpV1Packet();
    }

    private void SetupV2()
    {
        var a = 123;
        var b = 234;
        var c = 345;
        var packet = new SnmpV2Packet("public");
        VbCollection vbs =
        [
            new Vb(new Oid("1.3.2"), new Integer32(a)),
            new Vb(new Oid("1.3.3"), new Integer32(b)),
            new Vb(new Oid("1.3.4"), new Integer32(c))
        ];
        packet.Pdu.SetVbList(vbs);
        packet.Pdu.RequestId = a * b * c;
        packet.Pdu.ErrorIndex = a + b + c;
        packet.Pdu.ErrorStatus = a - b + c;
        _2Initial = packet;
        _2Bytes = packet.Encode();
        _2New = new SnmpV2Packet();
    }

    private void SetupV3()
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
        SetAuth(Private, Auth, packet, Digest, Protocol);

        packet.Pdu.SetVbList(vbs);
        packet.Pdu.RequestId = 123;
        packet.Pdu.ErrorIndex = 567;
        packet.Pdu.ErrorStatus = 6879;
        _3Initial = packet;
        _3Bytes = packet.Encode();
        _3New = new SnmpV3Packet();
        SetAuth(Private, Auth, _3New, Digest, Protocol);
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
        packet.SetEngineId(engineId);
    }

    private void SetAuth(bool @private, bool auth, SecureAgentParameters parameters, AuthenticationDigests digests,
        PrivacyProtocols protocols)
    {
        @private &= protocols != PrivacyProtocols.None;
        auth &= digests != AuthenticationDigests.None;
        var username = "admin";
        var authenticationPassword = "someFakePassword";
        var privacyPassword = "someFakePassword";
        var engineId = "TheEngineID";

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
    }

    [Benchmark]
    public byte[] EncodeV1()
    {
        return _1Initial.Encode();
    }

    [Benchmark]
    public SnmpV1Packet DecodeV1()
    {
        _1New.Decode(_1Bytes);
        return _1New;
    }

    [Benchmark]
    public byte[] EncodeV2()
    {
        return _2Initial.Encode();
    }

    [Benchmark]
    public SnmpV2Packet DecodeV2()
    {
        _2New.Decode(_2Bytes);
        return _2New;
    }

    [Benchmark]
    public byte[] EncodeV3()
    {
        return _3Initial.Encode();
    }

    [Benchmark]
    public SnmpV3Packet DecodeV3()
    {
        _3New.Decode(_3Bytes);
        return _3New;
    }
}