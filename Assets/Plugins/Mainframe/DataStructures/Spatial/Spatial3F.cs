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
public struct AABB3F
{
  float3 _position, _size, _min, _max, _extents;

  public AABB3F(float3 position, float3 size)
  {
    _position = position;
    _size = math.abs(size);
    _extents = _size * 0.5f;

    _min = _position - (size * 0.5f);
    _max = _position + (size * 0.5f);
  }

  /// <summary>
  /// The position of the aabb in world space.
  /// </summary>
  public float3 position
  {
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
  public float3 extents => _extents;

  /// <summary>
  /// The size of the AABB.
  /// </summary>
  public float3 size
  {
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
  public float3 min
  {
    get => _min;
    private set
    {
      _min = value;
      RecalculateVolume();
    }
  }

  /// <summary>
  /// The maximum point of the aabb's bounds.
  /// </summary>
  public float3 max
  {
    get => _max;
    private set
    {
      _max = value;
      RecalculateVolume();
    }
  }

  /// <summary>
  /// Moves the aabb and recalculates the min and max
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Translate(float3 p)
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

//   ___   ___  ___                                                                                   
//  / _ \ | _ )| _ )                                                                                  
// | (_) || _ \| _ \                                                                                  
//  \___/ |___/|___/                                                                                  
//                                                                                                    
//----------------------------------------------------------------------------------------------------
/// <summary>
/// OBB (Oriented Bounding Box)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct OBB3F
{
  float3 _origin, _size;
  quaternion _rotation;

  public OBB3F(float3 origin, float3 size, quaternion rotation)
  {
    _origin = origin;
    _size = math.abs(size);
    extents = _size / 2f;
    _rotation = rotation;

    vertices = new float3[8]; // Initialize the vertices array

    RecalculateVolume();
  }

  /// <summary>
  /// The position of the obb in world space.
  /// </summary>
  public float3 position
  {
    get => _origin;
    set => Translate(value);
  }

  /// <summary>
  /// The rotation of the obb.
  /// </summary>
  public quaternion rotation
  {
    get => _rotation;
    set
    {
      _rotation = value;
      RecalculateVolume();
    }
  }

  /// <summary>
  /// The extents (half-size) of the obb.
  /// </summary>
  public float3 extents { get; }

  /// <summary>
  /// The size of the obb.
  /// </summary>
  public float3 size
  {
    get => _size;
    set
    {
      _size = value;
      RecalculateVolume();
    }
  }

  /// <summary>
  /// The vertices of the obb in world space.
  /// </summary>
  public float3[] vertices { get; set; }

  /// <summary>
  /// Moves the obb and recalculates the min and max.
  /// </summary>
  public void Translate(float3 position)
  {
    _origin = position;
    RecalculateVolume();
  }

  /// <summary>
  /// Recalculates the vertices for the obb.
  /// </summary>
  public void RecalculateVolume()
  {
    float3 rotatedExtents = math.rotate(_rotation, extents);
    float3 min = _origin - rotatedExtents;
    float3 max = _origin + rotatedExtents;

    // Update vertices
    vertices[0] = min;
    vertices[1] = new float3(min.x, min.y, max.z);
    vertices[2] = new float3(min.x, max.y, min.z);
    vertices[3] = new float3(min.x, max.y, max.z);
    vertices[4] = new float3(max.x, min.y, min.z);
    vertices[5] = new float3(max.x, min.y, max.z);
    vertices[6] = new float3(max.x, max.y, min.z);
    vertices[7] = max;
  }
}

//  ___        _                                                                                      
// / __| _ __ | |_   ___  _ _  ___                                                                    
// \__ \| '_ \| ' \ / -_)| '_|/ -_)                                                                   
// |___/| .__/|_||_|\___||_|  \___|                                                                   
//      |_|                                                                                           
//----------------------------------------------------------------------------------------------------
[StructLayout(LayoutKind.Sequential)]
public struct Sphere
{
  float _radius;
  float3 _size;

  public Sphere(float3 origin, float radius)
  {
    position = origin;
    _radius = radius;
    _size = new float3(radius * 2f);
    extents = new float3(_radius);
  }

  /// <summary>
  /// The position of the sphere in world space.
  /// </summary>
  public float3 position { get; set; }

  /// <summary>
  /// The radius of the sphere.
  /// </summary>
  public float radius
  {
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
  public float3 size
  {
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
  public float3 extents { get; private set; }

  public void Translate(float3 position)
  {
    this.position = position;
    RecalculateVolume();
  }

  /// <summary>
  /// Recalculates bounds of the sphere.
  /// </summary>
  public void RecalculateVolume()
  {
    _size = new float3(radius * 2f);
    extents = new float3(_radius);
  }
}

//  _  _  _  _                                                                                        
// | || |(_)| |_                                                                                      
// | __ || ||  _|                                                                                     
// |_||_||_| \__|                                                                                     
//                                                                                                    
//----------------------------------------------------------------------------------------------------
[StructLayout(LayoutKind.Sequential)]
public struct Hit3F
{
  public Hit3F(float3 point, float3 normal, float3 delta, float distance)
  {
    this.point = point;
    this.normal = normal;
    this.delta = delta;
    this.distance = distance;
  }

  /// <summary>
  /// The position of the hit point in world space.
  /// </summary>
  public float3 point { get; set; }

  /// <summary>
  /// The normal vector of the hit surface.
  /// </summary>
  public float3 normal { get; set; }

  /// <summary>
  /// The delta vector between the start and end positions of the hit ray.
  /// </summary>
  public float3 delta { get; set; }

  /// <summary>
  /// The distance from the ray origin to the hit point.
  /// </summary>
  public float distance { get; set; }
}

//  _     _                                                                                           
// | |   (_) _ _   ___                                                                                
// | |__ | || ' \ / -_)                                                                               
// |____||_||_||_|\___|                                                                               
//                                                                                                    
//----------------------------------------------------------------------------------------------------
[StructLayout(LayoutKind.Sequential)]
public struct Line3F
{
  public Line3F(float3 origin, float3 direction, float distance)
  {
    _origin = origin;
    _direction = direction;
    _distance = distance;

    directionInv = new float3(1f / _direction.x, 1f / _direction.y, 1f / _direction.z);

    _end = origin + _direction * distance;
    delta = _end - _origin;
  }

  float3 _origin, _end;
  float3 _direction;
  float _distance;

  /// <summary>
  /// The origin point of the ray.
  /// </summary>
  public float3 origin
  {
    get => _origin;
    set
    {
      _origin = value;
      _distance = math.length(_end - _origin);
      directionInv = 1f / _direction;
      delta = _end - _origin;
    }
  }

  /// <summary>
  /// The end point of the ray.
  /// </summary>
  public float3 end
  {
    get => _end;
    set
    {
      _end = value;
      _distance = math.length(_end - _origin);
      directionInv = 1f / _direction;
      delta = _end - _origin;
    }
  }

  /// <summary>
  /// The direction vector of the ray.
  /// </summary>
  public float3 direction => _direction;

  /// <summary>
  /// The normalized direction vector of the ray
  /// </summary>
  public float3 delta { get; private set; }

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
  public float3 directionInv { get; private set; }
}
}
