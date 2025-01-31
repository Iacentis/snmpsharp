namespace SnmpSharpNet;

public class Snmp : UdpTransport
{
    /// <summary>
    ///     Internal storage for request target information.
    /// </summary>
    protected ITarget? _target;

    #region Constructor(s)

    /// <summary>
    ///     Constructor
    /// </summary>
    public Snmp()
        : base(false)
    {
        _target = null;
    }

    #endregion Constructor(s)

    /// <summary>
    ///     Internal event to send result of the async request to.
    /// </summary>
    protected event SnmpAsyncResponse? _response;

    protected virtual void OnResponse(AsyncRequestResult result, SnmpPacket packet)
    {
        _response?.Invoke(result, packet);
    }
}