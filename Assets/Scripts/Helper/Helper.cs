// Standalone helper functions that can be called from anywhere

using UnityEngine;

public static class Helper
{
    /// <summary>
    /// Separates out the individual digits in an integer into a list
    /// Requires a positive number
    /// </summary>
    public static System.Collections.Generic.List<int> SeparateDigits(int number)
    {
        if (number <= 0 )
        {
            Debug.LogError("Helper.SeparateDigits not giving a postive number");
            return null;
        }

        var digitsList = new System.Collections.Generic.List<int>();

        while (number > 0)
        {
            digitsList.Add(number % 10);
            number = number / 10;
        }
        digitsList.Reverse();

        return digitsList;
    }

    /// <summary>
    /// Returns either -1.0f or 1.0f
    /// </summary>
    public static float RandomSign()
    {
        return Random.value < .5 ? 1.0f : -1.0f;
    }

    /// <summary>
    /// Returns whether the number is positive or negative
    /// </summary>
    /// <returns>1 or -1</returns>
    public static int GetSign(float number)
    {
        return number >= 0 ? 1 : -1;
    }

    /// <summary>
    /// Returns a random point on a sphere within the defined range given
    /// </summary>
    /// <param name="targetDirection">The centre point of the defined range</param>
    /// <param name="angle">How large the defined range is in degrees. Note 180 degrees is a full sphere</param>    
    public static Vector3 GetPointOnSphere(Quaternion targetDirection, float angle)
    {
        // Convert the angle given to radians
        float angleInRad = Random.Range(0.0f, angle) * Mathf.Deg2Rad;

        // Find a random point relative to how large the defined area is        
        Vector3 pointOnCircle = Random.insideUnitCircle.normalized * Mathf.Sin(angleInRad);
        Vector3 v = new Vector3(pointOnCircle.x, pointOnCircle.y, Mathf.Cos(angleInRad));

        // Rotate this point around to the target direction
        return targetDirection * v;
    }

    /// <summary>
    /// Counts the number of entries in an enum
    /// </summary>
    public static int CountEnum(System.Type type)
    {
        if (!type.IsEnum)
            Debug.LogError("HELPER: COUNTENUM NOT GIVEN AN ENUM");

        return System.Enum.GetValues(type).Length;
    }

    /// <summary>
    /// Adjusts a linear intepolation to match an ease in and ease out curve. Wikipedia smoothstep for image.
    /// </summary>
    /// <param name="percentage">Between 0.0 and 1.0</param>
    public static float SmoothStep(float percentage)
    {
        return percentage * percentage * (3f - 2f * percentage);
    }

    /// <summary>
    /// Adjusts a linear intepolation to match an ease in and ease out curve. Wikipedia smoothstep for image.
    /// </summary>
    /// <param name="percentage">Between 0.0 and 1.0</param>
    public static float SmootherStep(float percentage)
    {
        return percentage*percentage*percentage * (percentage * (6f * percentage - 15f) + 10f);
    }

    public static Vector3 ClampAngleOnPlane(Vector3 origin, Vector3 direction, float angle, Vector3 planeNormal)
    {
        float a = Vector3.Angle(origin, direction);

        if (a < angle)
            return direction;

        Vector3 r = Vector3.Cross(planeNormal, origin);

        float s = Vector3.Angle(r, direction);
        float rotationAngle = (s < 90 ? 1 : -1) * angle;
        Quaternion rotation = Quaternion.AngleAxis(rotationAngle, planeNormal);

        return rotation * origin;
    }

    public static bool PointAbovePlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        Vector3 direction = point - planePoint;
        return Vector3.Angle(direction, planeNormal) < 90;
    }

    public static bool Timer(float startTime, float duration)
    {
        return Time.time > startTime + duration;
    }

    public static float ClampAngle(float angle)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return angle;
    }

    [System.Serializable]
    public struct RangeFloat
    {
        [SerializeField]
        public float Min;
        [SerializeField]
        public float Max;
    }

    [System.Serializable]
    public struct RangeInt
    {
        [SerializeField]
        public int Min;
        [SerializeField]
        public int Max;
    }
}
