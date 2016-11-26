using UnityEngine;

public static class RotateAroundPivotExtensions
{
    /// <summary>
    /// Returns the rotated Vector3 using a Quaterion
    /// </summary>
    /// <param name="Point">Point to rotate</param>
    /// <param name="Pivot">Pivot point</param>
    /// <param name="Angle">Rotation angle</param>
    /// <returns></returns>
    public static Vector3 RotateAroundPivot(this Vector3 Point, Vector3 Pivot, Quaternion Angle)
    {
        return Angle * (Point - Pivot) + Pivot;
    }

    /// <summary>
    /// Returns the rotated Vector3 using Euler
    /// </summary>
    /// <param name="Point">Point to rotate</param>
    /// <param name="Pivot">Pivot point</param>
    /// <param name="Euler"></param>
    /// <returns></returns>
    public static Vector3 RotateAroundPivot(this Vector3 Point, Vector3 Pivot, Vector3 Euler)
    {
        return RotateAroundPivot(Point, Pivot, Quaternion.Euler(Euler));
    }

    /// <summary>
    /// Rotates the Transform's position using a Quaterion
    /// </summary>
    /// <param name="Me"></param>
    /// <param name="Pivot"></param>
    /// <param name="Angle"></param>
    public static void RotateAroundPivot(this Transform Me, Vector3 Pivot, Quaternion Angle)
    {
        Me.position = Me.position.RotateAroundPivot(Pivot, Angle);
    }

    /// <summary>
    /// Rotates the Transform's position using Euler
    /// </summary>
    /// <param name="Me"></param>
    /// <param name="Pivot"></param>
    /// <param name="Euler"></param>
    public static void RotateAroundPivot(this Transform Me, Vector3 Pivot, Vector3 Euler)
    {
        Me.position = Me.position.RotateAroundPivot(Pivot, Quaternion.Euler(Euler));
    }
}

/*USAGE:
*
* myGameObject.transform.RotateAroundPivot(CenterAsVector3, AngleAsVector3); //Modifies the Transform (immediate rotation)
* myGameObject.transform.position.RotateAroundPivot(CenterAsVector3, AngleAsVector3); //Returns the rotated position (preview rotation)
*
*/
