using UnityEngine;

namespace Bestagon.Hexagon
{
    /// <summary>
    /// Representation of an Axial Hex Coordinate (implicit Cube coordinate)
    /// </summary>
    /// 
    public static class SideExtensions
    {
        public static float Degrees(this Hex.Side side)
        {
            return (int)side * 60.0f;
        }
        public static Hex Offset(this Hex.Side side)
        {
            return Hex.Offsets[(int)side];
        }

        public static (float min, float max) PointRadians(this Hex.Side side)
        {
            switch (side)
            {
                default:
                case Hex.Side.Right:
                    return (330f * Mathf.Deg2Rad, 30f * Mathf.Deg2Rad);
                case Hex.Side.UpRight:
                    return (30f * Mathf.Deg2Rad, 90f * Mathf.Deg2Rad);
                case Hex.Side.UpLeft:
                    return (90f * Mathf.Deg2Rad, 150f * Mathf.Deg2Rad);
                case Hex.Side.Left:
                    return (150f * Mathf.Deg2Rad, 210f * Mathf.Deg2Rad);
                case Hex.Side.DownLeft:
                    return (210f * Mathf.Deg2Rad, 270f * Mathf.Deg2Rad);
                case Hex.Side.DownRight:
                    return (270f * Mathf.Deg2Rad, 330f * Mathf.Deg2Rad);
            }
        }

        public static float Radians(this Hex.Side side)
        {
            switch (side)
            {
                default:
                case Hex.Side.Right:
                    return 0f;
                case Hex.Side.UpRight:
                    return 60f * Mathf.Deg2Rad;
                case Hex.Side.UpLeft:
                    return 120f * Mathf.Deg2Rad;
                case Hex.Side.Left:
                    return 180f * Mathf.Deg2Rad;
                case Hex.Side.DownLeft:
                    return 240f * Mathf.Deg2Rad;
                case Hex.Side.DownRight:
                    return 300f * Mathf.Deg2Rad;
            }
        }

        public static (float clockwise, float counterclockwise) SidePointRadians(this Hex.Side side)
        {
            return (side.Radians() + (30f * Mathf.Deg2Rad), side.Radians() - (30f * Mathf.Deg2Rad));
        }

        public static Hex.Side Inverse(this Hex.Side side)
        {
            switch (side)
            {
                default:
                case Hex.Side.Right:
                    return Hex.Side.Left;
                case Hex.Side.UpRight:
                    return Hex.Side.DownLeft;
                case Hex.Side.UpLeft:
                    return Hex.Side.DownRight;
                case Hex.Side.Left:
                    return Hex.Side.Right;
                case Hex.Side.DownLeft:
                    return Hex.Side.UpRight;
                case Hex.Side.DownRight:
                    return Hex.Side.UpLeft;
            }
        }

        public static bool Adjacent(this Hex.Side side, Hex.Side other, out bool clockwise)
        {
            switch (side)
            {
                default:
                case Hex.Side.Right:
                    {
                        clockwise = other == Hex.Side.DownRight;
                        return other == Hex.Side.DownRight || other == Hex.Side.UpRight;
                    }
                case Hex.Side.UpRight:
                    {
                        clockwise = other == Hex.Side.Right;
                        return other == Hex.Side.Right || other == Hex.Side.UpLeft;
                    }
                case Hex.Side.UpLeft:
                    {
                        clockwise = other == Hex.Side.UpRight;
                        return other == Hex.Side.UpRight || other == Hex.Side.Left;
                    }
                case Hex.Side.Left:
                    {
                        clockwise = other == Hex.Side.UpLeft;
                        return other == Hex.Side.UpLeft || other == Hex.Side.DownLeft;
                    }
                case Hex.Side.DownLeft:
                    {
                        clockwise = other == Hex.Side.Left;
                        return other == Hex.Side.Left || other == Hex.Side.DownRight;
                    }
                case Hex.Side.DownRight:
                    {
                        clockwise = other == Hex.Side.DownLeft;
                        return other == Hex.Side.DownLeft || other == Hex.Side.Right;
                    }
            }
        }

        private static Hex.Side[] right_clockwise = new Hex.Side[]  { Hex.Side.DownRight, Hex.Side.DownLeft, Hex.Side.Left, Hex.Side.UpLeft, Hex.Side.UpRight };
        private static Hex.Side[] UpRight_clockwise = new Hex.Side[]{ Hex.Side.Right, Hex.Side.DownRight, Hex.Side.DownLeft, Hex.Side.Left, Hex.Side.UpLeft };
        private static Hex.Side[] UpLeft_clockwise = new Hex.Side[] { Hex.Side.UpRight, Hex.Side.Right, Hex.Side.DownRight, Hex.Side.DownLeft, Hex.Side.Left };
        private static Hex.Side[] Left_clockwise = new Hex.Side[]   { Hex.Side.UpLeft, Hex.Side.UpRight, Hex.Side.Right, Hex.Side.DownRight, Hex.Side.DownLeft };
        private static Hex.Side[] DownLeft_clockwise = new Hex.Side[] { Hex.Side.Left, Hex.Side.UpLeft, Hex.Side.UpRight, Hex.Side.Right, Hex.Side.DownRight };
        private static Hex.Side[] DownRight_clockwise = new Hex.Side[] { Hex.Side.DownLeft, Hex.Side.Left, Hex.Side.UpLeft, Hex.Side.UpRight, Hex.Side.Right};

        public static Hex.Side[] RotationalExpansion(this Hex.Side side, int dist, bool clockwise)
        {
            if (dist <= 0)
                return new Hex.Side[] { };

            Hex.Side[] rotationalSet;
            switch (side)
            {
                case Hex.Side.Right:
                    rotationalSet = right_clockwise;
                    break;
                case Hex.Side.UpRight:
                    rotationalSet = UpRight_clockwise;
                    break;
                case Hex.Side.UpLeft:
                    rotationalSet = UpLeft_clockwise;
                    break;
                case Hex.Side.Left:
                    rotationalSet = Left_clockwise;
                    break;
                case Hex.Side.DownLeft:
                    rotationalSet = DownLeft_clockwise;
                    break;
                case Hex.Side.DownRight:
                    rotationalSet = DownRight_clockwise;
                    break;
                default:
                    rotationalSet = null;
                    break;
            }
            if (rotationalSet == null)
            {
                return null;
            }

            Hex.Side[] returns = new Hex.Side[dist > 6 ? 6 : dist];
            for (int i = 0; i < returns.Length; i++)
            {
                returns[i] = rotationalSet[clockwise ? i : (rotationalSet.Length - i - 1)];
            }
            return returns;
        }

        /// <summary>
        /// Returns the two sides that indicate a clockwise rotation relative to this side
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public static Hex.Side[] ClockwiseIndications(this Hex.Side side)
        {
            switch (side)
            {
                default:
                case Hex.Side.Right:
                    return new Hex.Side[] { Hex.Side.DownRight, Hex.Side.DownLeft };
                case Hex.Side.UpRight:
                    return new Hex.Side[] { Hex.Side.Right, Hex.Side.DownRight };
                case Hex.Side.UpLeft:
                    return new Hex.Side[] { Hex.Side.UpRight, Hex.Side.Right };
                case Hex.Side.Left:
                    return new Hex.Side[] { Hex.Side.UpLeft, Hex.Side.UpRight };
                case Hex.Side.DownLeft:
                    return new Hex.Side[] { Hex.Side.Left, Hex.Side.UpLeft };
                case Hex.Side.DownRight:
                    return new Hex.Side[] { Hex.Side.DownLeft, Hex.Side.Left };
            }
        }
    }
}