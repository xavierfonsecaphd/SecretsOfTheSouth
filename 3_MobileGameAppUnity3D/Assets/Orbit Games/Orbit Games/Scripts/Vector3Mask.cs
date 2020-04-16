using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Vector3Mask {

    public bool x;
    public bool y;
    public bool z;
    public bool w;

    public Vector3Mask(bool x, bool y = false, bool z = false, bool w = false)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public bool this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return x;
                case 1: return y;
                case 2: return z;
                case 3: return w;
            }
            return false;
        }
        set
        {
            switch (index)
            {
                case 0: x = value; break;
                case 1: y = value; break;
                case 2: z = value; break;
                case 3: w = value; break;
            }
        }
    }

    public bool HasTrue
    {
        get
        {
            return x || y || z || w;
        }
    }

    public bool HasFalse
    {
        get
        {
            return !x || !y || !z || !w;
        }
    }

    public int TrueCount
    {
        get
        {
            return 0 + (x ? 1 : 0) + (y ? 1 : 0) + (z ? 1 : 0) + (w ? 1 : 0);
        }
    }

    public int FalseCount
    {
        get
        {
            return 0 + (!x ? 1 : 0) + (!y ? 1 : 0) + (!z ? 1 : 0) + (!w ? 1 : 0);
        }
    }

    public Vector3 CopyValues(Vector3 valuesToSet, Vector3 vectorToUpdate)
    {
        if (w)
        {
            return valuesToSet;
        }
        else
        {
            return new Vector3(x ? valuesToSet.x : vectorToUpdate.x, y ? valuesToSet.y : vectorToUpdate.y, z ? valuesToSet.z : vectorToUpdate.z);
        }
    }

    public static Vector3Mask False { get { return new Vector3Mask(false); } }
    public static Vector3Mask True { get { return new Vector3Mask(true, true, true, true); } }
    public static Vector3Mask X { get { return new Vector3Mask(true, false, false, false); } }
    public static Vector3Mask Y { get { return new Vector3Mask(false, true, false, false); } }
    public static Vector3Mask Z { get { return new Vector3Mask(false, false, true, false); } }
    public static Vector3Mask W { get { return new Vector3Mask(false, false, false, true); } }

    public static Vector3Mask Or(Vector3Mask lhs, Vector3Mask rhs)
    {
        return new Vector3Mask(lhs.x || rhs.x, lhs.y || rhs.y, lhs.z || rhs.z, lhs.w || rhs.w);
    }

    public static Vector3Mask And(Vector3Mask lhs, Vector3Mask rhs)
    {
        return new Vector3Mask(lhs.x && rhs.x, lhs.y && rhs.y, lhs.z && rhs.z, lhs.w && rhs.w);
    }
    
    public override bool Equals(object other)
    {
        if (other.GetType() != GetType()) return false;
        var otherVec = (Vector3Mask)other;
        return otherVec.x == x && otherVec.y == y && otherVec.z == z && otherVec.w == w;
    }

    public override int GetHashCode()
    {
        return (x ? 1 : 0) + (y ? 2 : 0) + (z ? 4 : 0) + (w ? 8 : 0);
    }

    public static Vector3Mask operator +(Vector3Mask a, Vector3Mask b)
    {
        return Or(a, b);
    }
    public static Vector3Mask operator -(Vector3Mask a, Vector3Mask b)
    {
        return new Vector3Mask(b.x ? false : a.x, b.y ? false : a.y, b.z ? false : a.z, b.w ? false : a.w);
    }
    public static Vector3 operator *(Vector3Mask a, Vector3 b)
    {
        if (a.w)
        {
            return b;
        }
        else
        {
            return new Vector3(a.x ? b.x : 0, a.y ? b.y : 0, a.z ? b.z : 0);
        }
    }
    public static Vector3 operator *(Vector3 b, Vector3Mask a)
    {
        if (a.w)
        {
            return b;
        }
        else
        {
            return new Vector3(a.x ? b.x : 0, a.y ? b.y : 0, a.z ? b.z : 0);
        }
    }
    public static Vector4 operator *(Vector3Mask a, Vector4 b)
    {
        return new Vector4(a.x ? b.x : 0, a.y ? b.y : 0, a.z ? b.z : 0, a.w ? b.w : 0);
    }
    public static bool operator ==(Vector3Mask lhs, Vector3Mask rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;
    }
    public static bool operator !=(Vector3Mask lhs, Vector3Mask rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z || lhs.w != rhs.w;
    }
    public static implicit operator bool(Vector3Mask d)
    {
        return d.w;
    }
    public static implicit operator Vector3Mask(bool d)
    {
        return d ? True : False;
    }
}
