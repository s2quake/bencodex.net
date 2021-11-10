using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

namespace Bencodex.Types
{
    /// <summary>Represents a Bencodex null value (i.e., <c>n</c>).</summary>
    public readonly struct Null :
        IValue,
        IEquatable<Null>,
        IComparable<Null>,
        IComparable
    {
        /// <summary>
        /// Represents a <see cref="Null"/> instance.  Recommends to prefer this over using
        /// the default constructor or a <c>default</c> keyword.  This field is read-only.
        /// </summary>
        public static readonly Null Value =
#pragma warning disable SA1129
            new Null();
#pragma warning restore SA1129

        /// <summary>
        /// The singleton fingerprint for the <see cref="Null"/> value.
        /// </summary>
        public static readonly Fingerprint SingletonFingerprint =
            new Fingerprint(ValueType.Null, 1L);

        /// <inheritdoc cref="IValue.Type"/>
        [Pure]
        public ValueType Type => ValueType.Null;

        /// <inheritdoc cref="IValue.Fingerprint"/>
        [Pure]
        public Fingerprint Fingerprint => SingletonFingerprint;

        /// <inheritdoc cref="IValue.EncodingLength"/>
        [Pure]
        public long EncodingLength => 1L;

        /// <inheritdoc cref="IValue.Inspection"/>
        [Pure]
        public string Inspection => "null";

        public override int GetHashCode() => 0;

        int IComparable.CompareTo(object obj) => obj is Null ? 0 : -1;

        int IComparable<Null>.CompareTo(Null other) => 0;

        public override bool Equals(object obj)
        {
            return ReferenceEquals(null, obj) || obj is Null;
        }

        bool IEquatable<IValue>.Equals(IValue other) =>
            other is Null;

        bool IEquatable<Null>.Equals(Null other) => true;

        [Pure]
        public IEnumerable<byte[]> EncodeIntoChunks()
        {
            yield return new byte[1] { 0x6e }; // 'n'
        }

        public void EncodeToStream(Stream stream)
        {
            stream.WriteByte(0x6e); // 'n'
        }

        [Pure]
        public override string ToString() =>
            $"{nameof(Bencodex)}.{nameof(Bencodex.Types)}.{nameof(Null)}";
    }
}
