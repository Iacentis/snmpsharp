// This file is part of SNMP#NET.
// 
// SNMP#NET is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SNMP#NET is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with SNMP#NET.  If not, see <http://www.gnu.org/licenses/>.
// 

namespace SnmpSharpNet;

/// <summary>
///     SNMP Agent specific values.
/// </summary>
/// <remarks>
///     This class stores values to access SNMP version 1 and version 2
///     agents.
///     Pass this class with your request data (Pdu) to the request method of the target class to make
///     a request.
/// </remarks>
public class AgentParameters : IAgentParameters
{
    /// <summary>
    ///     SNMP community name for SNMP v1 and v2 protocol versions
    /// </summary>
    protected readonly OctetString _community;

    /// <summary>
    ///     Flag that disables checking of host IP address and port number from which reply is received. If not disabled, only
    ///     replies from the host IP/port to which request was sent will be considered valid and all others will be ignored.
    ///     Default value is: false (reply source check is enabled)
    ///     Set to true if you wish to disable this check.
    /// </summary>
    protected bool _disableReplySourceCheck;

    /// <summary>
    ///     Agent protocol version
    /// </summary>
    protected readonly Integer32 _version;

    /// <summary>
    ///     Standard constructor
    /// </summary>
    public AgentParameters()
    {
        _version = new Integer32((int)SnmpVersion.Ver1);
        _community = new OctetString("public");
        _disableReplySourceCheck = false;
    }

    /// <summary>
    ///     Copy constructor. Initialize the class with the values of the parameter class values.
    /// </summary>
    /// <param name="second">Parameter class.</param>
    public AgentParameters(AgentParameters second)
    {
        _version = new Integer32(second._version);
        _community = new OctetString(second.Community);
        _disableReplySourceCheck = second.DisableReplySourceCheck;
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="version">
    ///     SNMP protocol version. Acceptable values are SnmpConstants.SNMPV1 and
    ///     SnmpConstants.SNMPV2
    /// </param>
    public AgentParameters(SnmpVersion version)
        : this()
    {
        _version.Value = (int)version;
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="community">Agent SNMP community name</param>
    public AgentParameters(OctetString community)
        : this()
    {
        _community = new OctetString(community);
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="version">SNMP Protocol version</param>
    /// <param name="community">SNMP community name</param>
    public AgentParameters(SnmpVersion version, OctetString community)
        : this(version)
    {
        _community = new(community);
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="version">SNMP Protocol version</param>
    /// <param name="community">SNMP community name</param>
    /// <param name="disableReplySourceCheck">Should reply source IP address/port number be checked on reply reception</param>
    public AgentParameters(SnmpVersion version, OctetString community, bool disableReplySourceCheck)
        : this(version, community)
    {
        _disableReplySourceCheck = disableReplySourceCheck;
    }

    /// <summary>
    ///     Get SNMP version 1 or 2 community name object
    /// </summary>
    public virtual OctetString Community => _community;

    /// <summary>
    ///     Get/Set flag that disables checking of host IP address and port number from which reply is received. If not
    ///     disabled, only
    ///     replies from the host IP/port to which request was sent will be considered valid and all others will be ignored.
    /// </summary>
    public bool DisableReplySourceCheck
    {
        get => _disableReplySourceCheck;
        set => _disableReplySourceCheck = value;
    }

    /// <summary>
    ///     Get/Set SNMP protocol version.
    /// </summary>
    /// <exception cref="SnmpInvalidVersionException">
    ///     Thrown when attempting to set protocol version
    ///     other then version 1 or 2c
    /// </exception>
    public virtual SnmpVersion Version
    {
        get => (SnmpVersion)_version.Value;
        set
        {
            if (value != SnmpVersion.Ver1 && value != SnmpVersion.Ver2)
                throw new SnmpInvalidVersionException("Valid SNMP versions are 1 or 2");
            _version.Value = (int)value;
        }
    }

    /// <summary>
    ///     Validate object.
    /// </summary>
    /// <returns>true if object is valid, otherwise false</returns>
    public bool Valid()
    {
        switch (_community.Length)
        {
            case <= 0:
                return false;
            default:
                return _version.Value is (int)SnmpVersion.Ver1 or (int)SnmpVersion.Ver2;
        }
    }

    /// <summary>
    ///     Initialize SNMP packet class with agent parameters. In this class, SNMP community name is
    ///     set in SNMPv1 and SNMPv2 packets.
    /// </summary>
    /// <param name="packet">Packet class to initialize</param>
    public void InitializePacket(SnmpPacket packet)
    {
        switch (packet)
        {
            case SnmpV1Packet v1Packet:
                v1Packet.Community.Set(_community);
                break;
            case SnmpV2Packet pkt:
                pkt.Community.Set(_community);
                break;
            default:
                throw new SnmpInvalidVersionException("Invalid SNMP version.");
        }
    }

    /// <summary>
    ///     Clone current object
    /// </summary>
    /// <returns>Duplicate object initialized with values from this class.</returns>
    public object Clone()
    {
        return new AgentParameters(Version, Community, DisableReplySourceCheck);
    }

    /// <summary>
    ///     Return SNMP version Integer32 object
    /// </summary>
    /// <returns>Integer32 object</returns>
    public Integer32 GetVersion()
    {
        return _version;
    }
}