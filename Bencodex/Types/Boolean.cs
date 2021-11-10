using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace Bencodex.Types
{
    /// <summary>Represents a Bencodex Boolean true (i.e., <c>t</c>)
    /// or false (i.e., <c>f</c>).</summary>
    public struct Boolean :
        IValue,
        IEquatable<bool>,
        IEquatable<Boolean>,
        IComparable<bool>,
        IComparable<Boolean>,
        IComparable
    {
        private static readonly byte[] _true = new byte[1] { 0x74 };  // 't'

        private static readonly byte[] _false = new byte[1] { 0x66 };  // 'f'

#pragma warning disable SA1202
        public static readonly Fingerprint TrueFingerprint =
            new Fingerprint(ValueType.Boolean, 1, new byte[] { 1 });

        public static readonly Fingerprint FalseFingerprint =
            new Fingerprint(ValueType.Boolean, 1, new byte[] { 0 });
#pragma warning restore SA1202

        public Boolean(bool value)
        {
            Value = value;
        }

        public bool Value { get; }

        /// <inheritdoc cref="IValue.Type"/>
        [Pure]
        public ValueType Type => ValueType.Boolean;

        /// <inheritdoc cref="IValue.Fingerprint"/>
        [Pure]
        public Fingerprint Fingerprint => Value ? TrueFingerprint : FalseFingerprint;

        /// <inheritdoc cref="IValue.EncodingLength"/>
        [Pure]
        public int EncodingLength => 1;

        /// <inheritdoc cref="IValue.Inspection"/>
        [Pure]
        public string Inspection =>
            Value ? "true" : "false";

        public static implicit operator bool(Boolean boolean)
        {
            return boolean.Value;
        }

        public static implicit operator Boolean(bool b)
        {
            return new Boolean(b);
        }

        public int CompareTo(object obj)
        {
            if (obj is bool b)
            {
                return ((IComparable<bool>)this).CompareTo(b);
            }

            return Value.CompareTo(obj);
        }

        int IComparable<bool>.CompareTo(bool other) => Value.CompareTo(other);

        int IComparable<Boolean>.CompareTo(Boolean other)
        {
            return CompareTo(other.Value);
        }

        bool IEquatable<bool>.Equals(bool other)
        {
            return Value == other;
        }

        bool IEquatable<Boolean>.Equals(Boolean other)
        {
            return Value == other.Value;
        }

        bool IEquatable<IValue>.Equals(IValue other) =>
            other is Boolean o && ((IEquatable<Boolean>)this).Equals(o);

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case Boolean b:
                    return Value.Equals(b.Value);
                case bool b:
                    return Value.Equals(b);
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        [Pure]
        public IEnumerable<byte[]> EncodeIntoChunks()
        {
            if (Value)
            {
                yield return _true;
            }
            else
            {
                yield return _false;
            }
        }

        public void EncodeToStream(Stream stream)
        {
            var value = Value ? _true[0] : _false[0];
            stream.WriteByte(value);
        }

        [Pure]
        public override string ToString() =>
            $"{nameof(Bencodex)}.{nameof(Bencodex.Types)}.{nameof(Boolean)} {Inspection}";
    }
}
