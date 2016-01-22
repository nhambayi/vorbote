using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    [Serializable]
    public class EmailHeader
    {
        //
        // Summary:
        //     Gets the name of the header field.
        //
        // Remarks:
        //     Represents the field name of the header.
        public string Field { get; set; }

        //
        // Summary:
        //     Gets the header identifier.
        //
        // Remarks:
        //     This property is mainly used for switch-statements for performance reasons.
        public string Id { get; set; }
        //
        // Summary:
        //     Gets the stream offset of the beginning of the header.
        //
        // Remarks:
        //     If the offset is set, it refers to the byte offset where it was found in the
        //     stream it was parsed from.
        public long? Offset { get; set; }
        //
        // Summary:
        //     Gets the raw field name of the header.
        //
        // Remarks:
        //     Contains the raw field name of the header.
        public byte[] RawField { get; set; }
        //
        // Summary:
        //     Gets the raw value of the header.
        //
        // Remarks:
        //     Contains the raw value of the header, before any decoding or charset conversion.
        public byte[] RawValue { get; set; }
        //
        // Summary:
        //     Gets or sets the header value.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     value is null.
        //
        // Remarks:
        //     Represents the decoded header value and is suitable for displaying to the user.
        public string Value { get; set; }
    }
}
