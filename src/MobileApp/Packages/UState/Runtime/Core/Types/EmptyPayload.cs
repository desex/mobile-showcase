using System;

namespace UState
{
    public struct EmptyPayload : IEquatable<EmptyPayload>
    {
        public static EmptyPayload Default = new();
        
        public static bool operator ==(EmptyPayload first, EmptyPayload second) => true;
        public static bool operator !=(EmptyPayload first, EmptyPayload second) => false;

        public bool Equals(EmptyPayload other) => true;

        public override bool Equals(object obj) => 
            obj is EmptyPayload other && Equals(other);

        public override int GetHashCode() => 0;
    }
}