//  ___              _    _        _                                                                  
// / __| _ __  __ _ | |_ (_) __ _ | |                                                                 
// \__ \| '_ \/ _` ||  _|| |/ _` || |                                                                 
// |___/| .__/\__,_| \__||_|\__,_||_|                                                                 
//      |_|                                                                                           
//----------------------------------------------------------------------------------------------------
// Data structures for physics                                                                        
//----------------------------------------------------------------------------------------------------

#region
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;
#endregion

namespace Mainframe
{
//    _      _    ___  ___                                                                            
//   /_\    /_\  | _ )| _ )                                                                           
//  / _ \  / _ \ | _ \| _ \                                                                           
// /_/ \_\/_/ \_\|___/|___/                                                                           
//                                                                                                    
//----------------------------------------------------------------------------------------------------
/// <summary>
/// AABB (Axis Aligned Bounding Box)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct AABB2f
{
  float2 _position, _size, _min, _max, _extents;

  public AABB2f(float2 position, float2 size)
  {
    _position = position;
    _size = math.abs(size);
    _extents = _size * 0.5f;

    _min = _position - (size * 0.5f);
    _max = _position + (size * 0.5f);
  }

  public static AABB2f FromMinMax(float2 min, float2 max)
  {
    float2 pos = (min + max) * 0.5f, size = min - max;
    return new AABB2f(pos, size);
  }

  /// <summary>
  /// The position of the aabb in world space.
  /// </summary>
  public float2 position
  {
    [MethodImpl(256)]
    get => _position;
    set
    {
      _position = value;
      RecalculateVolume();
    }
  }

  /// <summary>
  /// The half-size of the AABB.
  /// </summary>
  public float2 extents => _extents;

  /// <summary>
  /// The size of the AABB.
  /// </summary>
  public float2 size
  {
    [MethodImpl(256)]
    get => _size;
    set
    {
      _size = value;
      RecalculateVolume();
    }
  }

  /// <summary>
  /// The minimum point of the aabb's bounds.
  /// </summary>
  public float2 min
  {
    [MethodImpl(256)]
    get => _min;
    set
    {
      _min = value;
      _size = _max - _min;
      _extents = _size * 0.5f;
      _position = (_min + _max) * 0.5f;
    }
  }

  /// <summary>
  /// The maximum point of the aabb's bounds.
  /// </summary>
  public float2 max
  {
    [MethodImpl(256)]
    get => _max;
    set
    {
      _max = value;
      _size = _max - _min;
      _extents = _size * 0.5f;
      _position = (_min + _max) * 0.5f;
    }
  }

  /// <summary>
  /// Moves the aabb and recalculates the min and max
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Translate(float2 p)
  {
    _position = p;
    RecalculateVolume();
  }

  /// <summary>
  /// Recalculates bounds of the aabb.
  /// </summary>
  public void RecalculateVolume()
  {
    _extents = _size * 0.5f;
    _min = _position - (_size * 0.5f);
    _max = _position + (_size * 0.5f);
  }
}

[StructLayout(LayoutKind.Sequential)]
public struct Circle2F
{
  float _radius;
  float2 _size;
  float2 _position;
  float2 _extents;

  public Circle2F(float2 origin, float radius)
  {
    _position = origin;
    _radius = radius;
    _size = new float2(radius * 2f);
    _extents = new float2(_radius);
  }

  /// <summary>
  /// The position of the sphere in world space.
  /// </summary>
  public float2 position { [MethodImpl(256)] get => _position; set => _position = value; }

  /// <summary>
  /// The radius of the sphere.
  /// </summary>
  public float radius
  {
    [MethodImpl(256)]
    get => _radius;
    set
    {
      _radius = value;
      RecalculateVolume();
    }
  }

  /// <summary>
  /// The size of the bounds of the sphere.
  /// </summary>
  public float2 size
  {
    [MethodImpl(256)]
    get => _size;
    set
    {
      _size = value;
      RecalculateVolume();
    }
  }

  /// <summary>
  /// The extents (half-size) of the sphere.
  /// </summary>
  public float2 extents
  {
    [MethodImpl(256)]
    get => _extents;
    private set => _extents = value;
  }

  public void Translate(float2 position)
  {
    this.position = position;
    RecalculateVolume();
  }

  /// <summary>
  /// Recalculates bounds of the sphere.
  /// </summary>
  public void RecalculateVolume()
  {
    _size = new float2(radius * 2f);
    extents = new float2(_radius);
  }
}

//  _  _  _  _                                                                                        
// | || |(_)| |_                                                                                      
// | __ || ||  _|                                                                                     
// |_||_||_| \__|                                                                                     
//                                                                                                    
//----------------------------------------------------------------------------------------------------
[StructLayout(LayoutKind.Sequential)]
public struct Hit2F
{
  float2 _point;
  float2 _normal;
  float2 _delta;
  float _distance;

  public Hit2F(float2 point, float2 normal, float2 delta, float distance)
  {
    _point = point;
    _normal = normal;
    _delta = delta;
    _distance = distance;
  }

  /// <summary>
  /// The position of the hit point in world space.
  /// </summary>
  public float2 point
  {
    [MethodImpl(256)]
    get => _point;
  }

  /// <summary>
  /// The normal vector of the hit surface.
  /// </summary>
  public float2 normal
  {
    [MethodImpl(256)]
    get => _normal;
  }

  /// <summary>
  /// The delta vector between the start and end positions of the hit ray.
  /// </summary>
  public float2 delta
  {
    [MethodImpl(256)]
    get => _delta;
  }

  /// <summary>
  /// The distance from the ray origin to the hit point.
  /// </summary>
  public float distance
  {
    [MethodImpl(256)]
    get => _distance;
  }
}

//  _     _                                                                                           
// | |   (_) _ _   ___                                                                                
// | |__ | || ' \ / -_)                                                                               
// |____||_||_||_|\___|                                                                               
//                                                                                                    
//----------------------------------------------------------------------------------------------------
[StructLayout(LayoutKind.Sequential)]
public struct Line2F
{
  float2 _origin, _end;
  float2 _direction;
  float _distance;
  float2 _directionInv;
  float2 _delta;

  public Line2F(float2 origin, float2 direction, float distance)
  {
    _origin = origin;
    _direction = direction;
    _distance = distance;

    _directionInv = new float2(1f / _direction.x, 1f / _direction.y);

    _end = origin + _direction * distance;
    _delta = _end - _origin;
  }

  /// <summary>
  /// The origin point of the ray.
  /// </summary>
  public float2 origin
  {
    [MethodImpl(256)]
    get => _origin;
    set
    {
      _origin = value;
      _distance = math.length(_end - _origin);
      _directionInv = 1f / _direction;
      _delta = _end - _origin;
    }
  }

  /// <summary>
  /// The end point of the ray.
  /// </summary>
  public float2 end
  {
    [MethodImpl(256)]
    get => _end;
    set
    {
      _end = value;
      _distance = math.length(_end - _origin);
      _directionInv = 1f / _direction;
      _delta = _end - _origin;
    }
  }

  /// <summary>
  /// The direction vector of the ray.
  /// </summary>
  public float2 direction
  {
    [MethodImpl(256)]
    get => _direction;
  }

  /// <summary>
  /// The normalized direction vector of the ray
  /// </summary>
  public float2 delta
  {
    [MethodImpl(256)]
    get => _delta;
  }

  /// <summary>
  /// The distance from the ray origin to the end point
  /// </summary>
  public float distance
  {
    get => _distance;
    set
    {
      _distance = value;
      end = origin + _direction * _distance;
    }
  }

  /// <summary>
  /// The reciprocal of the direction vector components, used for ray-box intersection calculations.
  /// </summary>
  public float2 directionInv { [MethodImpl(256)] get => _directionInv; }
}
}
