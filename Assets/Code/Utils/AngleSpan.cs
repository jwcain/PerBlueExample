using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a span between two angles.
/// Handles wrapping around 0 degrees (lower is greater than upper, for example)
/// </summary>
public struct AngleSpan
{
    public float lower;
    public float upper;

    public AngleSpan(float lower, float upper)
    {
        this.lower = lower;
        this.upper = upper;
    }

    /// <summary>
    /// Calculates the angle span a point can travel about the origin without crossing the angles specified by some bounding points.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="spanAbout"></param>
    /// <param name="boundingPoints"></param>
    /// <returns></returns>
    public static AngleSpan ValidAnglesBetweenPoints(Vector3 origin, Vector3 spanAbout, Vector3[] boundingPoints)
    {
        //calcuate the default angle for the vector for the point form the origin
        (float theta, float r) armPolar = CartesianToPolar(spanAbout - origin);

        //intialize the span as unbounded
        float lowerBound = float.MinValue;
        float upperBound = float.MaxValue;

        //For each bounding point, lower the valid span
        for (int i = 0; i < boundingPoints.Length; i++)
        {
            (float theta, float r) pointPolar = CartesianToPolar(boundingPoints[i] - origin);
            //We project some points around the circle depending on what side they fall on so they can be solved for linearly.
            float angleForMinCheck = pointPolar.theta;
            float angleForMaxCheck = pointPolar.theta;
            //If the points polar is less than the arm, we want to project it around the circle for the Max check
            if (pointPolar.theta < armPolar.theta)
                angleForMaxCheck += Mathf.PI * 2;
            //If the points polar is greater than the arm, we want to project it around the circle for the Min check
            if (pointPolar.theta > armPolar.theta)
                angleForMinCheck -= Mathf.PI * 2;

            //Check to see if our projections make the angle span smaller (and update it if it does)
            if (lowerBound < angleForMinCheck)
                lowerBound = angleForMinCheck;
            if (upperBound > angleForMaxCheck)
                upperBound = angleForMaxCheck;
        }

        //Clamp
        while (lowerBound < 0.0f)
            lowerBound += Mathf.PI * 2;
        while (upperBound < 0.0f)
            upperBound += Mathf.PI * 2;
        while (lowerBound > Mathf.PI * 2)
            lowerBound -= Mathf.PI * 2;
        while (upperBound > Mathf.PI * 2)
            upperBound -= Mathf.PI * 2;

        return new AngleSpan(lowerBound, upperBound);
    }

    public static (float theta, float r) CartesianToPolar(Vector3 vector)
    {
        return (Mathf.Atan2(vector.y, vector.x), Mathf.Sqrt((vector.x * vector.x) + (vector.y * vector.y)));
    }

    public static Vector3 PolarToCartesian(float theta, float r)
    {
        return new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 0f);
    }

    /// <summary>
    /// Returns if the specied angle falls within this span. If it is outside out returns the closest bound to the specified angle
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="closestBound"></param>
    /// <returns></returns>
    public bool Inside(float angle, out float closestBound)
    {
        //Clamp the angle to one unit circle
        while (angle > Mathf.PI * 2)
            angle -= Mathf.PI * 2;
        while (angle < 0f)
            angle += Mathf.PI * 2;

        //We span across the 0 bound (wraparound)
        if (upper < lower)
        {
            //Split the problem into two sub-spans
            bool first = new AngleSpan(lower, Mathf.PI * 2).Inside(angle, out _);
            bool second = new AngleSpan(0f, upper).Inside(angle, out _);
            //Calc the mid
            float mid = lower +((upper - lower) / 2);

            closestBound = (angle < mid) ? upper : lower;
            return first || second;
        }
        else
        {
            //TODO: Remove the ternary operation below
            if (angle < lower || angle > upper)
            {
                closestBound = angle < lower ? lower : upper;
                return false;
            }
            else
            {
                closestBound = (angle < (lower + (upper / 2f))) ? lower : upper;
                return true;
            }
        }
    }
}
