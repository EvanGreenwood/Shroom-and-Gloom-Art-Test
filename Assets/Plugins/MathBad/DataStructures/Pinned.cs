//  ___  _                      _                                                                      //
// | _ \(_) _ _   _ _   ___  __| |                                                                     //
// |  _/| || ' \ | ' \ / -_)/ _` |                                                                     //
// |_|  |_||_||_||_||_|\___|\__,_|                                                                     //
//                                                                                                     //
//----------------------------------------------------------------------------------------------------

#region
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
#endregion

namespace MathBad
{
internal static class PinnedUtil
{
  internal static void DisposedCheck<T>(this IPinned<T> pinned) where T : unmanaged, IDisposable
  {
    if(pinned.IsDisposed)
      throw new ObjectDisposedException($"{typeof(T)}> has been disposed.");
  }
}

internal interface IPinned<T> where T : unmanaged, IDisposable
{
  public bool IsDisposed { get; }
  public void Dispose();
}

// Pinned<T>
//----------------------------------------------------------------------------------------------------
public unsafe struct Pinned<T> : IPinned<Pinned<T>>, IDisposable where T : unmanaged
{
  IntPtr _memoryPtr;
  T* _ptr;
  bool _isDisposed;

  public Pinned(T source)
  {
    _isDisposed = false;

    // Allocate memory
    _memoryPtr = Marshal.AllocHGlobal(sizeof(T));
    _ptr = (T*)_memoryPtr;
    *_ptr = source;
  }

  public T* Ptr
  {
    get
    {
      this.DisposedCheck();
      return _ptr;
    }
  }

  public T Value
  {
    get
    {
      this.DisposedCheck();
      return *_ptr;
    }
  }

  public bool IsDisposed => _isDisposed;

  // Dispose
  //----------------------------------------------------------------------------------------------------
  public void Dispose()
  {
    if(_isDisposed)
      return;

    if(typeof(T) is IDisposable)
    {
      if(*_ptr is IDisposable disposable)
        disposable.Dispose();
    }
    Marshal.FreeHGlobal(_memoryPtr);
    _ptr = null;
    _isDisposed = true;
  }
}

// PinnedArray<T>
//----------------------------------------------------------------------------------------------------
public unsafe struct PinnedArray<T> : IDisposable, IPinned<Pinned<T>>, IEnumerable<T> where T : unmanaged
{
  IntPtr _memoryPtr;
  T* _ptr;
  readonly int _length;
  bool _isDisposed;

  public T* Ptr
  {
    get
    {
      this.DisposedCheck();
      return _ptr;
    }
  }

  public readonly int Length => _length;
  public bool IsDisposed => _isDisposed;

  public PinnedArray(T[] source)
  {
    if(source.IsNullOrEmpty())
      throw new ArgumentException($"{typeof(PinnedArray<T>)}>: Invalid source array.");
    _length = source.Length;
    _isDisposed = false;

    // Allocate memory
    _memoryPtr = Marshal.AllocHGlobal(sizeof(T) * _length);
    _ptr = (T*)_memoryPtr;

    for(int i = 0; i < _length; i++)
      _ptr[i] = source[i];
  }

  // Dispose
  //----------------------------------------------------------------------------------------------------
  public void Dispose()
  {
    if(!IsDisposed)
    {
      if(typeof(T) is IDisposable)
      {
        for(int i = 0; i < _length; i++)
          ((IDisposable)_ptr[i]).Dispose();
      }
      Marshal.FreeHGlobal(_memoryPtr);
      _memoryPtr = IntPtr.Zero;
      _ptr = null;
      _isDisposed = true;
    }
  }

  public T this[int index]
  {
    get
    {
      this.DisposedCheck();
      if(index < 0 || index >= _length)
        throw new IndexOutOfRangeException();

      return _ptr[index];
    }
    set
    {
      this.DisposedCheck();
      if(index < 0 || index >= _length)
        throw new IndexOutOfRangeException();

      _ptr[index] = value;
    }
  }

  public T[] Unpack()
  {
    this.DisposedCheck();

    T[] unpackedArray = new T[Length];
    fixed(T* unpackedArrayPtr = unpackedArray)
    {
      byte* destinationBytePtr = (byte*)unpackedArrayPtr;
      byte* sourceBytePtr = (byte*)_ptr;
      int byteCount = Length * sizeof(T);

      for(int i = 0; i < byteCount; i++) destinationBytePtr[i] = sourceBytePtr[i];
    }

    return unpackedArray;
  }

  public IEnumerator<T> GetEnumerator()
  {
    for(int i = 0; i < _length; i++)
      yield return this[i];
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

// PinnedArrayBuffer<T>
//----------------------------------------------------------------------------------------------------
public unsafe struct PinnedArrayBuffer<T> : IDisposable, IPinned<Pinned<T>>, IEnumerable<T> where T : unmanaged
{
  IntPtr _readMemoryPtr;
  IntPtr _writeMemoryPtr;

  T* _readPtr;
  T* _writePtr;

  readonly int _length;

  bool _isDisposed;

  public PinnedArrayBuffer(int length)
  {
    _length = math.max(0, length);
    _isDisposed = false;

    // Allocate memory
    _readMemoryPtr = Marshal.AllocHGlobal(sizeof(T) * _length);
    _writeMemoryPtr = Marshal.AllocHGlobal(sizeof(T) * _length);
    _readPtr = (T*)_readMemoryPtr;
    _writePtr = (T*)_writeMemoryPtr;
  }

  public T* ReadPtr
  {
    get
    {
      this.DisposedCheck();
      return _readPtr;
    }
  }
  public T* WritePtr
  {
    get
    {
      this.DisposedCheck();
      return _writePtr;
    }
  }

  public readonly int Length => _length;
  public bool IsDisposed => _isDisposed;

  // Dispose
  //----------------------------------------------------------------------------------------------------
  public void Dispose()
  {
    if(!_isDisposed)
    {
      if(typeof(T) is IDisposable)
      {
        for(int i = 0; i < _length; i++) ((IDisposable)_readPtr[i]).Dispose();
        for(int i = 0; i < _length; i++) ((IDisposable)_writePtr[i]).Dispose();
      }

      // Free unmanaged memory
      Marshal.FreeHGlobal(_readMemoryPtr);
      Marshal.FreeHGlobal(_writeMemoryPtr);
      _readMemoryPtr = _writeMemoryPtr = IntPtr.Zero;
      _readPtr = _writePtr = null;
      _isDisposed = true;
    }
  }

  // Read / Write
  //----------------------------------------------------------------------------------------------------
  public T ReadAt(int index)
  {
    this.DisposedCheck();
    if(index < 0 || index >= Length) throw new IndexOutOfRangeException();
    return _readPtr[index];
  }

  public void WriteAt(int index, T value)
  {
    this.DisposedCheck();
    if(index < 0 || index >= Length) throw new IndexOutOfRangeException();
    _writePtr[index] = value;
  }

  // Copy
  //----------------------------------------------------------------------------------------------------
  public void CopyTo(T[] destination, int destinationIndex, int length)
  {
    this.DisposedCheck();

    if(destination == null)
      throw new ArgumentNullException(nameof(destination));

    if(destinationIndex < 0 || destinationIndex >= destination.Length)
      throw new ArgumentOutOfRangeException(nameof(destinationIndex));

    if(length < 0 || length > Length || destination.Length - destinationIndex < length)
      throw new ArgumentOutOfRangeException(nameof(length), "Invalid range.");

    fixed(T* destinationPtr = destination)
    {
      byte* destinationBytePtr = (byte*)destinationPtr;
      byte* sourceBytePtr = (byte*)ReadPtr;
      int byteCount = length * sizeof(T);

      for(int i = 0; i < byteCount; i++)
        destinationBytePtr[destinationIndex * sizeof(T) + i] = sourceBytePtr[i];
    }
  }

  public T[] Unpack()
  {
    this.DisposedCheck();

    T[] unpackedArray = new T[Length];
    fixed(T* unpackedArrayPtr = unpackedArray)
    {
      byte* destinationBytePtr = (byte*)unpackedArrayPtr;
      byte* sourceBytePtr = (byte*)ReadPtr;
      int byteCount = Length * sizeof(T);

      for(int i = 0; i < byteCount; i++) destinationBytePtr[i] = sourceBytePtr[i];
    }

    return unpackedArray;
  }

  // Get Enumerator
  //----------------------------------------------------------------------------------------------------
  public IEnumerator<T> GetEnumerator()
  {
    for(int i = 0; i < Length; i++)
      yield return ReadAt(i);
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  // Double Buffer
  //----------------------------------------------------------------------------------------------------
  /// <summary>
  /// Schedules a job to copy contents from the back buffer into the front buffer and returns the resulting JobHandle.
  /// </summary>
  public JobHandle DoubleBuffer(int batchSize)
  {
    this.DisposedCheck();

    DoubleBufferJobs.DoubleBuffer<T> db
      = new DoubleBufferJobs.DoubleBuffer<T>
        {
          read = ReadPtr,
          write = WritePtr
        };

    return db.Schedule(Length, batchSize);
  }

  /// <summary>
  /// Schedules a job to copy contents from the back buffer into the front buffer, waits for the specified dependency,
  /// then returns the resulting JobHandle.
  /// </summary>
  public JobHandle DoubleBuffer(int batchSize, JobHandle dependency)
  {
    this.DisposedCheck();

    DoubleBufferJobs.DoubleBuffer<T> db
      = new DoubleBufferJobs.DoubleBuffer<T>
        {
          read = ReadPtr,
          write = WritePtr
        };

    return db.Schedule(Length, batchSize, dependency);
  }

  /// <summary>
  /// Schedules a job to copy contents from the back buffer into the front buffer, for the selected indices,
  /// waits for the specified dependency, then returns the resulting JobHandle.
  /// </summary>
  public JobHandle DoubleBufferSelect(NativeArray<int> bufferSelect, int batchSize, JobHandle dependency)
  {
    this.DisposedCheck();

    DoubleBufferJobs.DoubleBuffer<T> db
      = new DoubleBufferJobs.DoubleBuffer<T>
        {
          read = ReadPtr,
          write = WritePtr
        };

    return db.Schedule(bufferSelect.Length, batchSize, dependency);
  }
}
// pinned grid 2
//----------------------------------------------------------------------------------------------------//
public unsafe struct PinnedGrid2<T> : IDisposable, IPinned<Pinned<T>>, IEnumerable<T> where T : unmanaged
{
  T* _ptr;
  IntPtr _memoryPtr;
  AABB2f _aabb;
  int2 _res;
  int _length;
  float2 _voxelSize;

  public AABB2f AABB => _aabb;
  public int2 Res => _res;
  public int Length => _length;
  public bool IsDisposed { get; set; }

  public PinnedGrid2(T[] voxels, AABB2f aabb, int2 res)
  {
    if(voxels.IsNullOrEmpty())
      throw new ArgumentException($"PinnedGrid2<{typeof(T)}>: Invalid source array.");

    _res = res;
    _aabb = aabb;
    _length = _res.x * _res.y;
    _voxelSize = _aabb.size / _res;

    // Allocate memory
    _memoryPtr = Marshal.AllocHGlobal(sizeof(T) * _length);
    _ptr = (T*)_memoryPtr;

    for(int i = 0; i < _length; i++)
      _ptr[i] = voxels[i];

    IsDisposed = false;
  }

  public void Dispose()
  {
    if(!IsDisposed)
    {
      if(typeof(T) is IDisposable)
      {
        for(int i = 0; i < _length; i++)
          ((IDisposable)_ptr[i]).Dispose();
      }
      Marshal.FreeHGlobal(_memoryPtr);
      _memoryPtr = IntPtr.Zero;
      _ptr = null;
      IsDisposed = true;
    }
  }

  public T this[int i]
  {
    get
    {
      this.DisposedCheck();
      if(!IsValidIndex(i))
        throw new IndexOutOfRangeException();
      return _ptr[i];
    }
    set
    {
      this.DisposedCheck();
      if(!IsValidIndex(i))
        throw new IndexOutOfRangeException();
      _ptr[i] = value;
    }
  }

  public T this[int x, int y]
  {
    get
    {
      this.DisposedCheck();
      if(!IsValidGridIndex(x, y))
        throw new IndexOutOfRangeException();
      int i = XY_I(x, y);
      return _ptr[i];
    }
    set
    {
      this.DisposedCheck();
      if(!IsValidGridIndex(x, y))
        throw new IndexOutOfRangeException();
      int i = XY_I(x, y);
      _ptr[i] = value;
    }
  }

  public T this[int2 xy]
  {
    get
    {
      this.DisposedCheck();
      if(!IsValidGridIndex(xy))
        throw new IndexOutOfRangeException();
      int i = XY_I(xy);
      return _ptr[i];
    }
    set
    {
      this.DisposedCheck();
      if(!IsValidGridIndex(xy))
        throw new IndexOutOfRangeException();
      int i = XY_I(xy);
      _ptr[i] = value;
    }
  }

  public bool IsValidIndex(int i) => i >= 0 && i < _length;

  public bool IsValidGridIndex(int2 xy) => IsValidGridIndex(xy.x, xy.y);
  public bool IsValidGridIndex(int x, int y) => x >= 0 && x < _res.x && y >= 0 && y < _res.y;

  public bool IsValidPosition(float2 worldPos) => _aabb.Contains(worldPos);

  public float2 ClampInBounds(float2 worldPos) => math.clamp(worldPos, _aabb.min, _aabb.max);

  public int XY_I(int x, int y) => x + y * _res.y;
  public int XY_I(int2 xyz) => xyz.x + xyz.y * _res.y;
  public int2 I_XYZ(int i)
  {
    int z = i / (_res.x * _res.y);
    i -= z * _res.x * _res.y;
    int y = i / _res.x;
    int x = i % _res.x;
    return new int2(x, y);
  }

  public float2 IndexToWorldPos(int i) => GridToWorldPos(I_XYZ(i));
  public float2 GridToWorldPos(int2 i2) => GridToWorldPos(i2.x, i2.y) + _voxelSize.Half();
  public float2 GridToWorldPos(int x, int y) => math.remap(float2.zero, _res, _aabb.min, _aabb.max, new float2(x, y));

  public int WorldToIndex(float2 worldPos) => XY_I(WorldToGrid(worldPos));
  public int2 WorldToGrid(float2 worldPos) => (int2)math.floor(math.remap(_aabb.min, _aabb.max, new float2(0f), _res, ClampInBounds(worldPos)));

  public bool TryGetVoxelAtPosition(float2 worldPos, out T voxel)
  {
    if(IsValidPosition(worldPos))
    {
      int i = WorldToIndex(worldPos);
      voxel = _ptr[i];
      return true;
    }
    else
    {
      voxel = default;
      return false;
    }
  }

  // enumerator
  //----------------------------------------------------------------------------------------------------//
  public IEnumerator<T> GetEnumerator()
  {
    for(int i = 0; i < Length; i++)
      yield return this[i];
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
// Jobs
//--------------------------------------------------------------------------------------
public static class DoubleBufferJobs
{
  [BurstCompile]
  public unsafe struct DoubleBufferSelect<T> : IJobParallelFor where T : unmanaged
  {
    [WriteOnly, NativeDisableUnsafePtrRestriction]
    public T* front;
    [ReadOnly, NativeDisableUnsafePtrRestriction]
    public T* back;

    [ReadOnly, NativeDisableUnsafePtrRestriction] NativeArray<int> _bufferSelect;

    [BurstCompile]
    public void Execute(int index)
    {
      front[_bufferSelect[index]] = back[_bufferSelect[index]];
    }
  }

  [BurstCompile]
  public unsafe struct DoubleBuffer<T> : IJobParallelFor where T : unmanaged
  {
    [WriteOnly, NativeDisableUnsafePtrRestriction]
    public T* read;
    [ReadOnly, NativeDisableUnsafePtrRestriction]
    public T* write;

    [BurstCompile]
    public void Execute(int index)
    {
      read[index] = write[index];
    }
  }
}
}
