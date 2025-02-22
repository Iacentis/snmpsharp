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

public readonly ref struct SnmpHeader
{

    /// <summary>
    ///     Defines the BER extension "value" that is used to mark an extension type.
    /// </summary>
    private const byte EXTENSION_ID = 0x1F;
    public SnmpHeader(ReadOnlySpan<byte> encoded)
    {
        switch (encoded.Length)
        {
            case < 1:
                throw new OverflowException("Buffer is too short.");
        }

        // ASN.1 type
        AsnType = encoded[0];
        if ((AsnType & EXTENSION_ID) == EXTENSION_ID) throw new SnmpException("Invalid SNMP header type");

        // length
        Length = new SnmpLength(encoded[1..]);
    }
    public SnmpHeader(byte asnType, int length)
    {
        AsnType = asnType;
        Length = new SnmpLength(length);
    }
    public readonly int CopyTo(Span<byte> target)
    {
        target[0] = AsnType;
        Length.CopyTo(target[1..]);
        return ByteLength;
    }
    public readonly SnmpLength Length;
    public readonly byte AsnType;
    public readonly int ByteLength => Length.ByteLength + 1;
}
