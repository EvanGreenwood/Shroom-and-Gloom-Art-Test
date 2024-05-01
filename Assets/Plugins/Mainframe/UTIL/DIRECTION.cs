#region Usings
using UnityEngine;
#endregion

namespace Mainframe
{
public class DIRECTION
{
  /// <summary>
  /// Generates evenly distributed directions on a sphere based on the given number of view directions.
  /// The generated directions aim to minimize the energy potential between them, effectively scattering them uniformly.
  /// </summary>
  /// <param name="numViewDirections">The total number of directions to generate on the sphere.</param>
  /// <returns>An array of Vector3, each representing a direction.</returns>
  public static Vector3[] GetSphericalDirections(int numViewDirections)
  {
    Vector3[] directions = new Vector3[numViewDirections];

    float goldenRatio = (1f + Mathf.Sqrt(5f)) / 2f;
    float angleIncrement = Mathf.PI * 2f * goldenRatio;

    for(int i = 0; i < numViewDirections; i++)
    {
      float t = (float)i / numViewDirections;
      float inclination = Mathf.Acos(1f - 2f * t);
      float azimuth = angleIncrement * i;

      float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
      float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
      float z = Mathf.Cos(inclination);
      directions[i] = new Vector3(x, y, z);
    }

    return directions;
  }

  /// <summary>
  /// Generates a fan of directions radiating out from the given direction. The fan spread is defined by the spread angle.
  /// This method evenly spaces the directions within the fan based on the given count.
  /// </summary>
  /// <param name="origin">The origin from which the fan directions are generated.</param>
  /// <param name="direction">The central direction of the fan.</param>
  /// <param name="up">The up vector to use as the rotation axis.</param>
  /// <param name="spread">The total angle of spread for the fan, in degrees.</param>
  /// <param name="count">The number of directions to generate within the spread angle.</param>
  /// <returns>An array of Vector3, each representing a direction within the fan.</returns>
  public static Vector3[] GetFanDirections(Vector3 origin, Vector3 direction, Vector3 up, float spread, int count)
  {
    if(count == 0)
      return null;

    Vector3[] directions = new Vector3[count];
    float increment = (count > 1) ? spread / (count - 1) : 0;

    for(int i = 0; i < count; i++)
    {
      float rotationAngle;

      if(count.IsEven())
      {
        int incrementsAwayFromCenter = i / 2 + 1;
        rotationAngle = incrementsAwayFromCenter * increment;
        rotationAngle = (i % 2 == 0) ? -rotationAngle + (increment / 2) : rotationAngle - (increment / 2);
      }
      else
      {
        if(i == 0)
        {
          rotationAngle = 0f;
        }
        else
        {
          int incrementsAwayFromCenter = (i + 1) / 2;
          rotationAngle = incrementsAwayFromCenter * increment;
          if(i % 2 == 1) rotationAngle = -rotationAngle;
        }
      }

      directions[i] = Quaternion.AngleAxis(rotationAngle, up) * direction;
    }

    return directions;
  }

  public static Vector2[] GetFanDirections(Vector2 origin, Vector2 direction, float spread, int count)
  {
    if(count == 0)
      return null;

    Vector2[] directions = new Vector2[count];
    float increment = count > 1 ? spread / (count - 1) : 0;
    if(spread.Approx(360f))
    {
      increment = spread / count;
    }

    for(int i = 0; i < count; i++)
    {
      float rotationAngle;

      if(count.IsEven())
      {
        int incrementsAwayFromCenter = i / 2 + 1;
        rotationAngle = incrementsAwayFromCenter * increment;
        rotationAngle = (i % 2 == 0) ? -rotationAngle + (increment / 2) : rotationAngle - (increment / 2);
      }
      else
      {
        if(i == 0)
        {
          rotationAngle = 0f;
        }
        else
        {
          int incrementsAwayFromCenter = (i + 1) / 2;
          rotationAngle = incrementsAwayFromCenter * increment;
          if(i % 2 == 1) rotationAngle = -rotationAngle;
        }
      }

      directions[i] = Quaternion.Euler(0f, 0f, rotationAngle) * direction;
    }

    return directions;
  }
}
}
