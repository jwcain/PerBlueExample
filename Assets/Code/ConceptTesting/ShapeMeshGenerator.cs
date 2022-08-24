using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bestagon.Hexagon;
using System.Linq;

public class ShapeMeshGenerator : MonoBehaviour
{
    [SerializeField] float r = 0.1f;
    [SerializeField] float mapSize = .6f;

    [SerializeField] MeshFilter meshFilter;

    static Dictionary<Hex.Side, Vector3[]> sidePointsMap;

    public GameObject BuildMesh(Hex[] hexs)
    {
        if (sidePointsMap == null)
            sidePointsMap = new Dictionary<Hex.Side, Vector3[]>()
            {
                {Hex.Side.Right, GetPointOffsets(Hex.Side.Right) },
                {Hex.Side.UpRight, GetPointOffsets(Hex.Side.UpRight) },
                {Hex.Side.UpLeft, GetPointOffsets(Hex.Side.UpLeft) },
                {Hex.Side.Left, GetPointOffsets(Hex.Side.Left) },
                {Hex.Side.DownLeft, GetPointOffsets(Hex.Side.DownLeft) },
                {Hex.Side.DownRight, GetPointOffsets(Hex.Side.DownRight) }
            };

        GameObject meshHolder = new GameObject();
        meshHolder.transform.parent = this.transform;
        meshHolder.transform.position = new Vector3(0.0f, 0.0f, -0.05f);

        List<Vector3[]> triangles = new List<Vector3[]>();

        for (int i = 0; i < hexs.Length; i++)
        {
            for (int s = 0; s < 6; s++)
            {
                Hex.Side side = (Hex.Side)s;
                Hex.Side ccSide = side.RotationalExpansion(1, false)[0];
                Hex.Side clockwiseSide = side.RotationalExpansion(1, true)[0];
                bool hasConnection = hexs.Contains(hexs[i] + side.Offset());
                bool ccSideHasConnection = hexs.Contains(hexs[i] + ccSide.Offset());
                bool clockwiseSideHasConnection = hexs.Contains(hexs[i] + clockwiseSide.Offset());

                Vector3[] triangle = new Vector3[3];

                triangle[0] = hexs[i].UnityPosition();

                if (hasConnection)
                {
                    triangle[2] = hexs[i].UnityPosition() + sidePointsMap[side][hexs.Contains(hexs[i] + clockwiseSide.Offset()) ? 4 : 2];
                    triangle[1] = hexs[i].UnityPosition() + sidePointsMap[side][hexs.Contains(hexs[i] + ccSide.Offset()) ? 5 : 3];
                }
                else if (ccSideHasConnection && clockwiseSideHasConnection)
                {
                    //Draw the two far sides
                    triangle[1] = hexs[i].UnityPosition() + sidePointsMap[ccSide][2];
                    triangle[2] = hexs[i].UnityPosition() + sidePointsMap[clockwiseSide][3];
                    //triangle[1] = positions[i].UnityPosition() + sidePointsMap[side][0];
                    //triangle[2] = positions[i].UnityPosition() + sidePointsMap[side][1];
                }
                else if (ccSideHasConnection)
                {
                    triangle[2] = hexs[i].UnityPosition() + sidePointsMap[side][1];
                    triangle[1] = hexs[i].UnityPosition() + sidePointsMap[ccSide][2];
                }
                else if (clockwiseSideHasConnection)
                {

                    triangle[1] = hexs[i].UnityPosition() + sidePointsMap[side][0];
                    triangle[2] = hexs[i].UnityPosition() + sidePointsMap[clockwiseSide][3];
                }
                else
                {
                    //Draw the two close sides
                    triangle[1] = hexs[i].UnityPosition() + sidePointsMap[side][0];
                    triangle[2] = hexs[i].UnityPosition() + sidePointsMap[side][1];
                }
                triangles.Add(triangle);
            }
        }
        meshFilter = meshHolder.gameObject.AddComponent<MeshFilter>();
        meshHolder.gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("SolutionMat");
        int[] meshTriangles = new int[triangles.Count * 3];
        Vector3[] meshVerticies = new Vector3[triangles.Count * 3];
        int meshVerticeIndex = 0;
        for (int i = 0; i < triangles.Count; i++)
        {
            meshVerticies[meshVerticeIndex] = triangles[i][0];
            meshTriangles[meshVerticeIndex] = meshVerticeIndex++;

            meshVerticies[meshVerticeIndex] = triangles[i][1];
            meshTriangles[meshVerticeIndex] = meshVerticeIndex++;

            meshVerticies[meshVerticeIndex] = triangles[i][2];
            meshTriangles[meshVerticeIndex] = meshVerticeIndex++;
        }

        Mesh newMesh = new Mesh();

        newMesh.SetVertices(meshVerticies);
        newMesh.SetTriangles(meshTriangles, 0);
        newMesh.RecalculateNormals();
        meshFilter.mesh = newMesh;
        return meshHolder;
    }

    Vector3 PolarToCartesian(float theta, float r)
    {
        return new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 0f);
    }

    Vector3[] GetPointOffsets(Hex.Side side)
    {
        Vector3[] ret = new Vector3[6];
        ret[0] = PolarToCartesian(side.SidePointRadians().clockwise, r); // ClockwiseClose
        ret[1] = PolarToCartesian(side.SidePointRadians().counterclockwise, r); // CCClose

        Vector3 relativeUpper = PolarToCartesian(side.RotationalExpansion(1, true)[0].SidePointRadians().counterclockwise, r);
        Vector3 relativeLower = PolarToCartesian(side.RotationalExpansion(1, false)[0].SidePointRadians().clockwise, r);

        ret[2] = Vector3.Lerp(relativeUpper, side.Offset().UnityPosition() + relativeUpper, 0.5f); // ClockwiseFar
        ret[3] = Vector3.Lerp(relativeLower, side.Offset().UnityPosition() + relativeLower, 0.5f); // CCFar
        ret[4] = PolarToCartesian(side.RotationalExpansion(1, true)[0].SidePointRadians().clockwise, mapSize / 2f); // ClockwiseALT
        ret[5] = PolarToCartesian(side.RotationalExpansion(1, false)[0].SidePointRadians().counterclockwise, mapSize / 2f); // CounterClockwiseALT

        return ret;
    }
}
