// Standalone helper functions that can be called from anywhere

using UnityEngine;
//using System.Collections.Generic;
public static class Helper
{
    /// <summary>
    /// Separates out the individual digits in an integer into a list
    /// Requires a positive number
    /// </summary>
    public static System.Collections.Generic.List<int> SeparateDigits(int number)
    {
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

    public static int CountEnum(System.Type type)
    {
        if (!type.IsEnum)
            Debug.LogWarning("HELPER: COUNTENUM NOT GIVEN AN ENUM");

        return System.Enum.GetValues(type).Length;
    }
}
