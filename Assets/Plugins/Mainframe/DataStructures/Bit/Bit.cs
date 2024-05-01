#region
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
#endregion

namespace Mainframe
{
public static partial class MATH
{
  [MethodImpl(256)]
  public static Bit bit(byte v) => new Bit((byte)(v & 1));

  [MethodImpl(256)]
  public static Bit bit(bool v) => new Bit(v ? (byte)1 : (byte)0);

  [MethodImpl(256)]
  public static Bit bit(Bit v) => new Bit(v);

  [MethodImpl(256)]
  public static Bit bit(int n) => new Bit((byte)(n & 1));

  [MethodImpl(256)]
  public static Bit bit(float v) => new Bit((byte)((int)v & 1));

  [MethodImpl(256)]
  public static Bit bit(double v) => new Bit((byte)((int)v & 1));

  [MethodImpl(256)]
  public static uint hash(Bit v) => (uint)v * 0x745ED837u + 0x816EFB5Du;
}

//  ___  _  _                                                                                         
// | _ )(_)| |_                                                                                       
// | _ \| ||  _|                                                                                      
// |___/|_| \__|                                                                                      
//                                                                                                    
//----------------------------------------------------------------------------------------------------
/// <summary>
/// Represents a single binary digit with a value of either 0 or 1.
/// </summary>
[Serializable]
[StructLayout(LayoutKind.Sequential)]
public struct Bit : IEquatable<Bit>,
                    IComparable<Bit>
{
  [SerializeField]
  internal byte _value;

  // byte constructor
  [MethodImpl(256)]
  public Bit(byte v) => _value = (byte)(v & 1);

  // bool constructor
  [MethodImpl(256)]
  public Bit(bool v) => _value = v ? (byte)1 : (byte)0;

  // bit constructor
  [MethodImpl(256)]
  public Bit(Bit v) => _value = v._value;

  // integer constructor
  [MethodImpl(256)]
  public Bit(int v) => _value = (byte)(v & 1);

  // unsigned-integer constructor
  [MethodImpl(256)]
  public Bit(uint v) => _value = (byte)(v & 1);

  // float constructor
  [MethodImpl(256)]
  public Bit(float v) => _value = (byte)((int)v & 1);

  // double constructor
  [MethodImpl(256)]
  public Bit(double v) => _value = (byte)((int)v & 1);

  /// <summary>0 value bit.</summary>
  public static readonly Bit zero = new Bit(0);

  /// <summary>1 value bit.</summary>
  public static readonly Bit one = new Bit(1);

  /// <summary>Returns a hash code for the bit.</summary>
  /// <returns>The computed hash code of the bit.</returns>
  [MethodImpl(256)]
  public override int GetHashCode() => (int)MATH.hash(this);

  [MethodImpl(256)]
  public int CompareTo(Bit other) => _value.CompareTo(other._value);

  [MethodImpl(256)]
  public bool Equals(Bit other) => _value == other._value;

  [MethodImpl(256)]
  public override bool Equals(object obj) => obj is Bit b && _value == b._value;

  // string conversions
  //----------------------------------------------------------------------------------------------------
  /// <summary>Returns a string representation of the bit.</summary>
  /// <returns>The string representation of the bit.</returns>
  [MethodImpl(256)]
  public override string ToString() => $"{_value.ToString()} : {(_value == 1 ? "true" : "false")}";

  /// <summary>Returns a string representation of the bit using a specified format and culture-specific format information.</summary>
  /// <param name="format">The format string to use during string formatting.</param>
  /// <param name="formatProvider">The format provider to use during string formatting.</param>
  /// <returns>The string representation of the bit.</returns>
  [MethodImpl(256)]
  public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);

  // logical operators
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(256)]
  public static bool operator true(Bit b) => b._value == 1;
  [MethodImpl(256)]
  public static bool operator false(Bit b) => b._value == 0;

  // bitwise operators
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(256)]
  public static Bit operator &(Bit a, Bit b) => new Bit((byte)(a._value & b._value));
  [MethodImpl(256)]
  public static Bit operator |(Bit a, Bit b) => new Bit((byte)(a._value | b._value));
  [MethodImpl(256)]
  public static Bit operator ^(Bit a, Bit b) => new Bit((byte)(a._value ^ b._value));
  [MethodImpl(256)]
  public static Bit operator ~(Bit a) => new Bit((a._value == 0 ? (byte)1 : (byte)0));

  // equality operators
  //----------------------------------------------------------------------------------------------------
  [MethodImpl(256)]
  public static bool operator ==(Bit a, Bit b) => a._value == b._value;

  [MethodImpl(256)]
  public static bool operator !=(Bit a, Bit b) => a._value != b._value;

  // explicit conversions
  //----------------------------------------------------------------------------------------------------
  /// <summary>Explicitly converts a byte to a bit.</summary>
  /// <param name="v">The byte value to convert to bit.</param>
  /// <returns>The converted byte value.</returns>
  [MethodImpl(256)]
  public static implicit operator Bit(byte v) => new Bit(v);

  /// <summary>Explicitly converts a bool to a bit.</summary>
  /// <param name="v">The bool value to convert to bit.</param>
  /// <returns>The converted bit value.</returns>
  [MethodImpl(256)]
  public static implicit operator Bit(bool v) => new Bit(v ? (byte)1 : (byte)0);

  /// <summary>Explicitly converts a float value to a bit value.</summary>
  /// <param name="v">The single precision float value to convert to bit.</param>
  /// <returns>The converted bit value.</returns>
  [MethodImpl(256)]
  public static explicit operator Bit(float v) => new Bit(v);

  /// <summary>Explicitly converts a double value to a bit value.</summary>
  /// <param name="v">The double precision float value to convert to bit.</param>
  /// <returns>The converted bit value.</returns>
  [MethodImpl(256)]
  public static explicit operator Bit(double v) => new Bit(v);

  // implicit conversions
  //----------------------------------------------------------------------------------------------------
  /// <summary>Implicitly converts a bit to a byte.</summary>
  /// <param name="v">The bit value to convert to byte.</param>
  /// <returns>The converted byte value.</returns>
  [MethodImpl(256)]
  public static implicit operator byte(Bit v) => v._value;

  /// <summary>Implicitly converts a bit to a bool.</summary>
  /// <param name="v">The binary bit value to convert to bool.</param>
  /// <returns>The converted bool value.</returns>
  [MethodImpl(256)]
  public static implicit operator bool(Bit v) => v._value == 1;

  /// <summary>Implicitly converts a bit value to a float value.</summary>
  /// <param name="v">The bit value to convert to a single precision float.</param>
  /// <returns>The converted single precision float value.</returns>
  [MethodImpl(256)]
  public static implicit operator float(Bit v) => v._value;

  /// <summary>Implicitly converts a bit value to a double value.</summary>
  /// <param name="v">The bit value to convert to double precision float.</param>
  /// <returns>The converted double precision float value.</returns>
  [MethodImpl(256)]
  public static implicit operator double(Bit v) => v._value;
}

//  ___  _  _    ___                                                                                  
// | _ )(_)| |_ |_  )                                                                                 
// | _ \| ||  _| / /                                                                                  
// |___/|_| \__|/___|                                                                                 
//                                                                                                    
//----------------------------------------------------------------------------------------------------
/// <summary>
/// Represents a 2 dimensional binary digit's with a value of either 0 or 1.
/// </summary>
[Serializable, StructLayout(LayoutKind.Sequential)]
public struct Bit2
{
  [SerializeField] private Bit _x;
  [SerializeField] private Bit _y;

  public Bit2(Bit xy)
  {
    _x = xy;
    _y = xy;
  }
  public Bit2(Bit x, Bit y)
  {
    _x = x;
    _y = y;
  }

  public Bit x { [MethodImpl(256)] get => _x; set => _x = value; }
  public Bit y { [MethodImpl(256)] get => _y; set => _y = value; }
}

//  ___  _  _    ____                                                                                 
// | _ )(_)| |_ |__ /                                                                                 
// | _ \| ||  _| |_ \                                                                                 
// |___/|_| \__||___/                                                                                 
//                                                                                                    
//----------------------------------------------------------------------------------------------------
/// <summary>
/// Represents a 3 dimensional binary digit's with a value of either 0 or 1.
/// </summary>
[Serializable, StructLayout(LayoutKind.Sequential)]
public struct Bit3
{
  [SerializeField] private Bit _x;
  [SerializeField] private Bit _y;
  [SerializeField] private Bit _z;

  public Bit3(Bit xyz)
  {
    _x = xyz;
    _y = xyz;
    _z = xyz;
  }
  public Bit3(Bit x, Bit y, Bit z)
  {
    _x = x;
    _y = y;
    _z = z;
  }

  public Bit x { get => _x; set => _x = value; }
  public Bit y { get => _y; set => _y = value; }
  public Bit z { get => _z; set => _z = value; }
}
}
