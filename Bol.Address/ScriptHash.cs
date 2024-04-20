using System;
using System.Linq;
using Bol.Cryptography;

namespace Bol.Address
{
    public class ScriptHash : IScriptHash, IEquatable<ScriptHash>
    {
        private readonly byte[] _bytes;
        private readonly IBase16Encoder _encoder;

        public ScriptHash(byte[] bytes, IBase16Encoder encoder)
        {
            _bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
            _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        }

        public byte[] GetBytes()
        {
            return _bytes.ToArray();
        }

        public string ToHexString()
        {
            return _encoder.Encode(_bytes);
        }

        public bool Equals(ScriptHash other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _bytes.SequenceEqual(other._bytes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((ScriptHash)obj);
        }

        public override int GetHashCode()
        {
            return (_bytes != null ? Convert.ToBase64String(_bytes).GetHashCode() : 0);
        }
    }
}
