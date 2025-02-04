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
using System.Linq;

namespace SnmpSharpNet;

/// <summary>
///     Collection of static helper methods making operations with SMI data types simpler and easier.
/// </summary>
public sealed class SMIDataType
{
    private SMIDataType()
    {
    }

    /// <summary>
    ///     Get class instance for the SMI value type with the specific TLV encoding type code.
    /// </summary>
    /// <param name="asnType">SMI type code</param>
    /// <returns>Correct SMI type class instance for the data type or null if type is not recognized</returns>
    public static AsnType? GetSyntaxObject(byte asnType)
    {
        return !IsValidType(asnType) ? null : GetSyntaxObject((SMIDataTypeCode)asnType);
    }

    /// <summary>Used to create correct variable type object for the specified encoded type</summary>
    /// <param name="asnType">ASN.1 type code</param>
    /// <returns>A new object matching type supplied or null if type was not recognized.</returns>
    public static AsnType? GetSyntaxObject(SMIDataTypeCode asnType)
    {
        AsnType? obj = asnType switch
        {
            SMIDataTypeCode.Integer => new Integer32(),
            SMIDataTypeCode.Counter32 => new Counter32(),
            SMIDataTypeCode.Gauge32 => new Gauge32(),
            SMIDataTypeCode.Counter64 => new Counter64(),
            SMIDataTypeCode.TimeTicks => new TimeTicks(),
            SMIDataTypeCode.OctetString => new OctetString(),
            SMIDataTypeCode.Opaque => new Opaque(),
            SMIDataTypeCode.IPAddress => new IpAddress(),
            SMIDataTypeCode.ObjectId => new Oid(),
            SMIDataTypeCode.PartyClock => new V2PartyClock(),
            SMIDataTypeCode.NoSuchInstance => new NoSuchInstance(),
            SMIDataTypeCode.NoSuchObject => new NoSuchObject(),
            SMIDataTypeCode.EndOfMibView => new EndOfMibView(),
            SMIDataTypeCode.Null => new Null(),
            _ => null
        };

        return obj;
    }

    /// <summary>
    ///     Return SNMP type object of the type specified by name. Supported variable types are:
    ///     <see cref="Integer32" />, <see cref="Counter32" />, <see cref="Gauge32" />, <see cref="Counter64" />,
    ///     <see cref="TimeTicks" />, <see cref="OctetString" />, <see cref="IpAddress" />, <see cref="Oid" />, and
    ///     <see cref="Null" />.
    ///     Type names are the same as support class names compared without case sensitivity (e.g. Integer == INTEGER).
    /// </summary>
    /// <param name="name">Name of the object type (not case sensitive)</param>
    /// <returns>New <see cref="AsnType" /> object.</returns>
    public static AsnType GetSyntaxObject(string name)
    {
        AsnType obj = name.ToUpper() switch
        {
            "INTEGER32" or "INTEGER" => new Integer32(),
            "COUNTER32" => new Counter32(),
            "GAUGE32" => new Gauge32(),
            "COUNTER64" => new Counter64(),
            "TIMETICKS" => new TimeTicks(),
            "OCTETSTRING" => new OctetString(),
            "IPADDRESS" => new IpAddress(),
            "OID" => new Oid(),
            "NULL" => new Null(),
            _ => throw new ArgumentException("Invalid value type name")
        };

        return obj;
    }

    /// <summary>
    ///     Return string representation of the SMI value type.
    /// </summary>
    /// <param name="type">AsnType class Type member function value.</param>
    /// <returns>String formatted name of the SMI type.</returns>
    public static string? GetTypeName(SMIDataTypeCode type)
    {
        return Enum.GetName(type);
    }

    /// <summary>
    ///     Check if byte code is a valid SMI data type code
    /// </summary>
    /// <param name="smiType">SMI data type code to test</param>
    /// <returns>true if valid SMI data type, otherwise false</returns>
    public static bool IsValidType(byte smiType)
    {
        var validSMITypes = Enum.GetValues<SMIDataTypeCode>();
        return validSMITypes.Cast<byte>().Any(type => type == smiType);
    }
}