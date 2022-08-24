using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bestagon.Hexagon
{

    /// <summary>
    /// Represents a coordinate on a hexagon grid
    /// </summary>
    [System.Serializable]
    public struct Hex
    {
        #region Static
        #region Definitions
        public enum Side
        {
            Right = 0,
            UpRight = 1,
            UpLeft = 2,
            Left = 3,
            DownLeft = 4,
            DownRight = 5
        }
        
        public static readonly Hex[] Offsets = {
            new Hex(+1, 0),
            new Hex(0, +1),
            new Hex(-1, +1),
            new Hex(-1, 0),
            new Hex(0, -1),
            new Hex(+1, -1),
        };

        public IEnumerable<Hex> Neighbors()
        {
            for (int i = 0; i < Offsets.Length; i++)
            {
                yield return this + Offsets[i];
            }
        }
        #endregion

        #region Operators
        public static Hex operator +(Hex a, Hex b) => new Hex(a.q + b.q, a.r + b.r);
        public static Hex operator -(Hex a, Hex b) => new Hex(a.q - b.q, a.r - b.r);
        public static Hex operator *(Hex h, float m) => Hex.Round(h.q * m, h.r * m);
        public static Hex operator *(float m, Hex h) => h * m;
        public static Hex operator *(int m, Hex h) => h * m;
        public static Hex operator *(Hex h, int m) => new Hex(h.q * m, h.r * m);
        public static Hex operator /(Hex h, int m) => new Hex(h.q / m, h.r / m);

        public static bool operator ==(Hex a, Hex b) => Hex.Equals(a, b);
        public static bool operator !=(Hex a, Hex b) => !Hex.Equals(a, b);
        #endregion
        #endregion

        #region Constructors
        public Hex(int q /*x*/, int r/*y*/)
        {
            this.m_q = q;
            this.m_r = r;
        }

        public Hex(Vector2Int v) : this(v.x, v.y) { }
        public Hex(Vector3Int v) : this(v.x, v.z) { }
        #endregion

        #region Members
#pragma warning disable IDE1006 // Naming Styles
        [SerializeField] private int m_q;
        [SerializeField] private int m_r;
        public int q { get { return m_q; } }
        public int r { get { return m_r; } }
        public int s { get { return -m_q - m_r; } }

        public int magnitude { get {
                return Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s));
            }
        }
#pragma warning restore IDE1006 // Naming Styles
        #endregion


        #region Overrides
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Hex))
                return q == ((Hex)obj).q && r == ((Hex)obj).r;
            else if (obj.GetType() == typeof(Vector2Int))
                return q == ((Vector2Int)obj).x && r == ((Vector2Int)obj).y;
            else if (obj.GetType() == typeof(Vector3Int))
                return q == ((Vector3Int)obj).x && r == ((Vector3Int)obj).z;
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //Assume that the Hex positions will never be above +/- 32,000 in either axis
            return (q << 16) | (r & 0xFFFF);
        }

        public override string ToString()
        {
            return string.Format("Hex({0}, {1})", q, r);
        }

        public Vector2Int ToTilemap()
        {
            var col = q + (r - (r & 1)) / 2;
            var row = r;
            return new Vector2Int(col, row);
        }

        public static Hex FromUnityCell(Vector3Int cell)
        {
            var yCell = cell.x;
            var xCell = cell.y;
            var x = yCell - (xCell - (xCell & 1)) / 2;
            var z = xCell;
            var y = -x - z;
            return new Hex(x, z);
        }
        #endregion

        #region Static
        #region CONSTS
        public const float SQRT3 = 1.7320508075688772935274463415058723669428052538103806280558069794f;
        public const float SQRT3DIV2 = 0.8660254037844386467637231707529361834714026269051903140279034897f;
        public const float SQRT3DIV3 = 0.5773502691896257645091487805019574556476017512701268760186023264f;

        public static Hex zero { get { return new Hex(0, 0); } }
        #endregion

        /// <summary>
        /// Creates a ring of hexagons a distance from the center, with the specified radius
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static List<Hex> Ring(Hex center, int radius)
        {
            List<Hex> results = new List<Hex>();
            Ring_RefList(ref results, center, radius);
            return results;
        }

        /// <summary>
        /// Creates a filled area of hex space (shaped like a large hexagon)
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static List<Hex> Area(Hex center, int radius)
        {
            List<Hex> results = new List<Hex>();
            for (int i = 0; i <= radius; i++)
            {
                Ring_RefList(ref results, center, i);
            }
            return results;
        }

        /// <summary>
        /// Calculates a ring of hexes and places them in the reference data structure
        /// </summary>
        /// <param name="results"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        private static void Ring_RefList(ref List<Hex> results, Hex center, int radius)
        {
            if (radius == 0)
            {
                results.Add(center);
                return;
            }
            Hex cube = center + (Hex.Offsets[4] * radius);
            for (int i = 0; i < 6; i++)
            {
                for (int r = 0; r < radius; r++)
                {
                    results.Add(cube);
                    cube += Hex.Offsets[i];
                }
            }
        }

        /// <summary>
        /// Calcualtes the distance between two Hex in cube space
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int Distance(Hex a, Hex b)
        {
            return (Mathf.Abs(a.q - b.q) + Mathf.Abs(a.r - b.r) + Mathf.Abs(a.s - b.s)) / 2;
        }

        /// <summary>
        /// Rounds a floating point hex (in Hex space, NOT Unity Space) to the closest hex. Does not handle pixel perfect mapping
        /// </summary>
        /// <param name="q"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Hex Round(float q, float r)
        {
            float s = -q - r;
            int rx = Mathf.RoundToInt(q), ry = Mathf.RoundToInt(r), rz = Mathf.RoundToInt(s);
            float xDiff = Mathf.Abs((float)rx - q), yDiff = Mathf.Abs((float)ry - r), zDiff = Mathf.Abs((float)rz - s);

            if (xDiff > yDiff && xDiff > zDiff)
                rx = -ry - rz;
            else if (yDiff > zDiff)
                ry = -rx - rz;
            else
                _ = -rx - ry; //Case of RZ, left here for completeness

            return new Hex(rx, ry);
        }

        /// <summary>
        /// Returns a UID for a specific side (which is shared by two hexes)
        /// </summary>
        /// <param name="referenceHex"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public static (Hex, Hex.Side) GetSideUID(Hex referenceHex, Hex.Side side)
        {
            switch (side)
            {
                case Hex.Side.Right:
                case Hex.Side.UpRight:
                case Hex.Side.UpLeft:
                default:
                    return (referenceHex, side);
                case Hex.Side.Left:
                case Hex.Side.DownLeft:
                case Hex.Side.DownRight:
                    return (referenceHex + side.Offset(), side.Inverse());
            }
        }

        /// <summary>
        /// Returns if the specified hexes are in line of eachother on one axis
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool InLine(Hex a, Hex b)
        {
            //They are in line of 
            Hex offset = a - b;
            return offset.q == 0 || offset.r == 0 || offset.s == 0;
        }

        /// <summary>
        /// Returns the side that one hex is relative to antoher. Fails if the two hex are not inline.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="relative"></param>
        /// <param name="side"></param>
        /// <param name="adjacent"></param>
        /// <returns></returns>
        public static bool GetRelativeSide(Hex center, Hex relative, out Hex.Side side, out bool adjacent)
        {
            side = Side.Right;
            adjacent = false;

            if (center.Equals(relative) || InLine(center, relative) == false)
            {
                if (InLine(center, relative) == false)
                    Debug.LogError($"Inline failure {center} -> {relative}");
                return false;
            }

            adjacent = Hex.Distance(center, relative) == 1;
            Hex normalizedRelative = center + ((relative - center) / Hex.Distance(center, relative));

            for (int i = 0; i < Offsets.Length; i++)
            {
                if (normalizedRelative.Equals(center + Offsets[i]))
                {
                    side = (Side)i;
                    return true;
                }
            }
            Debug.LogError("Special relative failure");
            return false;
        }

        /// <summary>
        /// Rounds a world position to the closest possible side of the hexagon.
        /// Returns true if the mouse is close to the center of the hexagon (still provides closes side)
        /// </summary>
        /// <param name="center"></param>
        /// <param name="world"></param>
        /// <param name="side"></param>
        /// <param name="stickside"></param>
        /// <param name="angleTolderance"></param>
        /// <returns></returns>
        public static bool RoundToSide(Hex center, Vector3 world, out Hex.Side side)
        {
            Vector3 perspective = center.UnityPosition();
            Vector3 offset = world - perspective;
            float theta = Mathf.Atan2(offset.y, offset.x);
            theta *= Mathf.Rad2Deg;

            while (theta < 0.0f)
                theta += 360.0f;

            //Default set
            side = Hex.Side.Right;

            //Calculate how far from the center
            float r = Mathf.Sqrt((offset.x * offset.x) + (offset.y * offset.y));
            bool isCloseToCenter = (r < 1f / 8f);

            for (int i = 0; i <= 6; i++)
            {
                float baseAngle = i * 60f;
                if (Mathf.Abs(baseAngle - theta) < 60f / 2f)
                {
                    if (i == 6)
                        side = Side.Right;
                    else
                        side = (Side)i;
                    return isCloseToCenter;
                }
            }

            return isCloseToCenter;

        }

        /// <summary>
        /// Projects the current hex on a rotation about another hex
        /// </summary>
        /// <param name="about"></param>
        /// <param name="hex"></param>
        /// <param name="clockwise"></param>
        /// <returns></returns>
        public static Hex Rotate(Hex about, Hex hex, bool clockwise = true)
        {
            Hex relative = hex - about;
            return about + (clockwise ? new Hex(-relative.s, -relative.q) : new Hex(-relative.r, -relative.s));
        }

        /// <summary>
        /// Creates an ordered ring the specified distance away from a center point
        /// </summary>
        /// <param name="center"></param>
        /// <param name="dist"></param>
        /// <param name="clockwise"></param>
        /// <returns></returns>
        public static List<Hex> OrderedRing(Hex center, int dist, bool clockwise = true)
        {
            Side[] order = clockwise ?
                new Side[] { Side.DownRight, Side.DownLeft, Side.Left, Side.UpLeft, Side.UpRight, Side.Right } :
                new Side[] { Side.Left, Side.DownLeft, Side.DownRight, Side.Right, Side.UpRight, Side.UpLeft };
            List<Hex> ring = new List<Hex>();
            Hex ringPosition = center + Hex.Side.UpRight.Offset() * dist;
            ring.Add(ringPosition);
            for (int s = 0; s < order.Length; s++)
            {
                for (int k = 0; k < dist; k++)
                {
                    ringPosition += order[s].Offset();
                    if (ring.Contains(ringPosition) == false) //TODO: Test that this check can be eliminated
                        ring.Add(ringPosition);
                }
            }

            return ring;
        }

        /// <summary>
        /// Returns the list of hexes traversed for a rotation
        /// </summary>
        /// <param name="relative"></param>
        /// <param name="clockwise"></param>
        /// <returns></returns>
        public static List<Hex> RotationSweep(Hex about, Hex hex, bool clockwise = true)
        {
            //check for no movement
            if (Hex.zero.Equals(hex - about))
                return new List<Hex>() { hex };

            List<Hex> result = new List<Hex>();

            Hex rotation = Rotate(about, hex, clockwise);

            //Create the ordered ring of hexes for this distanec from the rotational center
            List<Hex> ring = OrderedRing(about, Hex.Distance(about, hex), clockwise);

            //Loop through that to find the provided hex, giving us the start position for the sweep
            int startPosition = -1;
            for (int s = 0; s < ring.Count; s++)
            {
                if (ring[s].Equals(hex))
                {
                    startPosition = s;
                    break;
                }
            }
            //Ensure we actually found a start position
            if (startPosition < 0)
            {
                Debug.LogError("Wrong ring");
                return new List<Hex>() { hex };
            }

            //Continue through the ring until we find the rotated hex position
            for (int i = 0; i < ring.Count; i++)
            {
                int index = startPosition + i;
                if (index >= ring.Count) index -= ring.Count;
                result.Add(ring[index]);
                if (ring[index].Equals(rotation))
                {
                    break;
                }
            }

            return result;
        }

        #endregion
    }
}