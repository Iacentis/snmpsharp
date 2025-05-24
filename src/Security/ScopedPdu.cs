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

using System;

namespace SnmpSharpNet;

/// <summary>SNMP Version 3 Scoped Protocol Data Unit.</summary>
/// <remarks>
///     ScopedPdu extends the <see cref="Pdu" /> class by adding SNMP version 3 specific Context Engine Id and Context Name
///     variables to the beginning of the packet.
///     Context engine id is retrieved from the agent using the SNMP version 3 standard defined discovery process. Context
///     name is used to define which subsection of the agents MIB user is allowed (or wants) to access.
///     When creating a new ScopedPdu, for a valid request, ContextEngineId value has to be set for a valid reply to be
///     received. Context value is implementation specific (depends on the configuration of the agent) and might not be
///     required for a successful request.
/// </remarks>
public class ScopedPdu : Pdu
{
    #region Private variables

    /// <summary>
    ///     Context Engine Id variable.
    /// </summary>
    protected readonly OctetString _contextEngineId;

    /// <summary>
    ///     Context name variable
    /// </summary>
    protected readonly OctetString _contextName;

    #endregion

    #region Properties

    /// <summary>
    ///     Get/Set context engine id value.
    /// </summary>
    public OctetString ContextEngineId => _contextEngineId;

    /// <summary>
    ///     Get/Set context name value
    /// </summary>
    public OctetString ContextName => _contextName;

    #endregion

    #region Constructor(s)

    /// <summary>
    ///     Standard constructor.
    /// </summary>
    /// <remarks>
    ///     Intializes the ScopedPdu type to SNMP-GET. For details see base
    ///     class <see cref="Pdu" /> standard constructor definition.
    ///     Context engine id and name variables are initialized to 0 length values.
    /// </remarks>
    public ScopedPdu()
    {
        _contextEngineId = new OctetString();
        _contextName = new OctetString();
    }

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="pduType">Pdu type value. For available types see <see cref="PduType" />.</param>
    public ScopedPdu(PduType pduType)
        : base(pduType)
    {
        _contextEngineId = new OctetString();
        _contextName = new OctetString();
    }

    /// <summary>Constructor.</summary>
    /// <remarks>
    ///     Standard constructor that sets ScopedPdu request type and request id. For valid types see <see cref="PduType" />.
    ///     By default, requestId value is set to a random value. Second argument allows caller to specify request
    ///     id for this packet
    /// </remarks>
    /// <param name="pduType">Pdu type value.</param>
    /// <param name="requestId">Request id for the ScopedPdu instance.</param>
    public ScopedPdu(PduType pduType, int requestId)
        : base(pduType)
    {
        _requestId.Value = requestId;
        _contextEngineId = new OctetString();
        _contextName = new OctetString();
    }

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="pdu">Initialize class from this <see cref="Pdu" /> class</param>
    public ScopedPdu(Pdu pdu)
        : base(pdu)
    {
        _contextEngineId = new OctetString();
        _contextName = new OctetString();
    }

    #endregion

    #region Encode and decode
    public override int Encode(Span<byte> buffer)
    {
        var offset = BuildHeader(buffer, SnmpConstants.SMI_SEQUENCE, MemberByteLength());
        offset += _contextEngineId.Encode(buffer[offset..]);
        offset += _contextName.Encode(buffer[offset..]);
        offset += base.Encode(buffer[offset..]);
        return offset;
    }

    public override int ByteLength
    {
        get
        {
            var length = MemberByteLength();
            length += HeaderSize(length);
            return length;
        }
    }

    private int MemberByteLength()
    {
        var length = 0;
        length += _contextEngineId.ByteLength;
        length += _contextName.ByteLength;
        length += base.ByteLength;
        return length;
    }

    /// <summary>
    ///     Decode BER encoded <see cref="ScopedPdu" /> values. This method does not perform SNMP v3 privacy operations
    ///     and is not aware of privacy requirements.
    ///     To decode a privacy protected SNMP v3 packet, you will need to a) extract <see cref="OctetString" /> value
    ///     holding encrypted <see cref="ScopedPdu" /> data, b) decrypt the encrypted ScopedPdu data into an unecrypted
    ///     byte array, c) pass unencrypted <see cref="ScopedPdu" /> and BER encoded byte array to this method for
    ///     final data conversion from BER into individual sequences and variables.
    /// </summary>
    /// <param name="buffer">Buffer holding BER encoded <see cref="ScopedPdu" /> data</param>
    /// <param name="offset">Offset within the buffer BER encoded <see cref="ScopedPdu" /> data begins.</param>
    /// <returns>Offset position after parsed ScopedPdu</returns>
    /// <exception cref="SnmpDecodingException">Error was encountered when decoding the PDU</exception>
    /// <exception cref="OverflowException">Thrown when buffer is too short to contain the PDU</exception>
    public override int Decode(ReadOnlySpan<byte> buffer, int offset)
    {
        var sequenceType = ParseHeader(buffer, ref offset, out var length);
        if (sequenceType != SnmpConstants.SMI_SEQUENCE)
            throw new SnmpDecodingException("Invalid ScopedPdu sequence detected. Invalid ScopedPdu encoding.");
        // verify packet can match parsed length
        if (length > buffer.Length - offset)
            throw new OverflowException("SNMP packet too short.");

        // Reset scoped pdu specific variables
        _contextEngineId.Reset();
        _contextName.Reset();
        // parse scoped pdu specific variables
        offset = _contextEngineId.Decode(buffer, offset);
        offset = _contextName.Decode(buffer, offset);
        // decode base pdu
        offset = base.Decode(buffer, offset);

        return offset;
    }

    #endregion Encode and decode
}