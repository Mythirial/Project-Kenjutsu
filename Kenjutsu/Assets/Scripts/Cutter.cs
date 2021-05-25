using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Cutter : MonoBehaviour
    {
        public static bool currentlyCutting;
        public static Mesh originalMesh;

        public static void Cut(GameObject originalGameObject, Vector3 contactPoint, Vector3 direction, Material cutMaterial = null, bool fill = true, bool addRigidbody = false)
        {
            if (currentlyCutting)
            {
                return;
            }

            currentlyCutting = true;

            //We are instantiating a plane through our initial object to seperate the left and right side from each other
            Plane plane = new Plane(originalGameObject.transform.InverseTransformDirection(-direction), originalGameObject.transform.InverseTransformPoint(contactPoint));
            originalMesh = originalGameObject.GetComponent<MeshFilter>().mesh;
            List<Vector3> addedVertices = new List<Vector3>();

            //We are getting two new generated meshes for our left and right side
            GeneratedMesh leftMesh = new GeneratedMesh();
            GeneratedMesh rightMesh = new GeneratedMesh();

            //Some meshes use different submeshes to have multiple materials attached to them
            //in an early iteration I had an extra script to turn everything into one mesh to make my life a little easier
            //however the result was not great because I could only slice objects that had one single material
            for (int i = 0; i < originalMesh.subMeshCount; i++)
            {
                var submeshIndices = originalMesh.GetTriangles(i);

                //We are now going through the submesh indices as triangles to determine on what side of the mesh they are.
                for (int j = 0; j < submeshIndices.Length; j += 3)
                {
                    var triangleIndexA = submeshIndices[j];
                    var triangleIndexB = submeshIndices[j + 1];
                    var triangleIndexC = submeshIndices[j + 2];

                    MeshTriangle currentTriangle = GetTriangle(triangleIndexA, triangleIndexB, triangleIndexC, i);

                    //We are now using the plane.getside function to see on which side of the cut our trianle is situated 
                    //or if it might be cut through
                    bool triangleALeftSide = plane.GetSide(originalMesh.vertices[triangleIndexA]);
                    bool triangleBLeftSide = plane.GetSide(originalMesh.vertices[triangleIndexB]);
                    bool triangleCLeftSide = plane.GetSide(originalMesh.vertices[triangleIndexC]);

                    //All three vertices are on the left side of the plane, so they need to be added to the left
                    //mesh
                    if (triangleALeftSide && triangleBLeftSide && triangleCLeftSide)
                    {
                        leftMesh.AddTriangle(currentTriangle);
                    }
                    //All three vertices are on the right side of the mesh.
                    else if (!triangleALeftSide && !triangleBLeftSide && !triangleCLeftSide)
                    {
                        rightMesh.AddTriangle(currentTriangle);
                    }
                    else
                    {

                        CutTriangle(plane, currentTriangle, triangleALeftSide, triangleBLeftSide, triangleCLeftSide, leftMesh, rightMesh, addedVertices);
                    }
                }
            }

            //Filling our cut
            if (fill == true)
            {
                FillCut(addedVertices, plane, leftMesh, rightMesh);
            }

            Mesh finishedLeftMesh = leftMesh.GetGeneratedMesh();
            Mesh finishedRightMesh = rightMesh.GetGeneratedMesh();

            originalGameObject.GetComponent<MeshFilter>().mesh = finishedLeftMesh;
            originalGameObject.AddComponent<MeshCollider>().sharedMesh = finishedLeftMesh;
            originalGameObject.GetComponent<MeshCollider>().convex = true;

            Material[] mats = new Material[finishedLeftMesh.subMeshCount];
            for (int i = 0; i < finishedLeftMesh.subMeshCount; i++)
            {
                mats[i] = originalGameObject.GetComponent<MeshRenderer>().material;
            }
            originalGameObject.GetComponent<MeshRenderer>().materials = mats;

            GameObject rightGO = new GameObject();
            rightGO.transform.position = originalGameObject.transform.position + (Vector3.up * .05f);
            rightGO.transform.rotation = originalGameObject.transform.rotation;
            rightGO.transform.localScale = originalGameObject.transform.localScale;
            rightGO.AddComponent<MeshRenderer>();
            mats = new Material[finishedRightMesh.subMeshCount];
            for (int i = 0; i < finishedRightMesh.subMeshCount; i++)
            {
                mats[i] = originalGameObject.GetComponent<MeshRenderer>().material;
            }
            rightGO.GetComponent<MeshRenderer>().materials = mats;

            rightGO.AddComponent<MeshFilter>().mesh = finishedRightMesh;

            rightGO.AddComponent<MeshCollider>().sharedMesh = finishedRightMesh;
            rightGO.GetComponent<MeshCollider>().convex = true;

            if (addRigidbody == true)
            {
                rightGO.AddComponent<Rigidbody>();
            }

            currentlyCutting = false;

        }

        private static MeshTriangle GetTriangle(int triangleIndexA, int triangleIndexB, int triangleIndexC, int submeshIndex)
        {
            //Adding the Vertices at the triangleIndex
            Vector3[] verticesToAdd = new Vector3[]
            {
                originalMesh.vertices[triangleIndexA],
                originalMesh.vertices[triangleIndexB],
                originalMesh.vertices[triangleIndexC]
            };

            //Adding the normals at the triangle index
            Vector3[] normalsToAdd = new Vector3[]
            {
                originalMesh.normals[triangleIndexA],
                originalMesh.normals[triangleIndexB],
                originalMesh.normals[triangleIndexC]
            };

            //adding the uvs at the triangleIndex
            Vector2[] uvsToAdd = new Vector2[]
            {
                originalMesh.uv[triangleIndexA],
                originalMesh.uv[triangleIndexB],
                originalMesh.uv[triangleIndexC]
            };

            return new MeshTriangle(verticesToAdd, normalsToAdd, uvsToAdd, submeshIndex);
        }

        private static void CutTriangle(Plane plane, MeshTriangle triangle, bool triangleALeftSide, bool triangleBLeftSide, bool triangleCLeftSide,
            GeneratedMesh leftSide, GeneratedMesh rightSide, List<Vector3> addedVertices)
        {
            List<bool> leftSideList = new List<bool>();
            leftSideList.Add(triangleALeftSide);
            leftSideList.Add(triangleBLeftSide);
            leftSideList.Add(triangleCLeftSide);

            MeshTriangle leftMeshTriangle = new MeshTriangle(new Vector3[2], new Vector3[2], new Vector2[2], triangle.SubmeshIndex);
            MeshTriangle rightMeshTriangle = new MeshTriangle(new Vector3[2], new Vector3[2], new Vector2[2], triangle.SubmeshIndex);

            bool left = false;
            bool right = false;

            for (int i = 0; i < 3; i++)
            {
                if (leftSideList[i])
                {
                    if (!left)
                    {
                        left = true;

                        leftMeshTriangle.Vertices[0] = triangle.Vertices[i];
                        leftMeshTriangle.Vertices[1] = leftMeshTriangle.Vertices[0];

                        leftMeshTriangle.UVs[0] = triangle.UVs[i];
                        leftMeshTriangle.UVs[1] = leftMeshTriangle.UVs[0];

                        leftMeshTriangle.Normals[0] = triangle.Normals[i];
                        leftMeshTriangle.Normals[1] = leftMeshTriangle.Normals[0];
                    }
                    else
                    {
                        leftMeshTriangle.Vertices[1] = triangle.Vertices[i];
                        leftMeshTriangle.Normals[1] = triangle.Normals[i];
                        leftMeshTriangle.UVs[1] = triangle.UVs[i];
                    }
                }
                else
                {
                    if (!right)
                    {
                        right = true;

                        rightMeshTriangle.Vertices[0] = triangle.Vertices[i];
                        rightMeshTriangle.Vertices[1] = rightMeshTriangle.Vertices[0];

                        rightMeshTriangle.UVs[0] = triangle.UVs[i];
                        rightMeshTriangle.UVs[1] = rightMeshTriangle.UVs[0];

                        rightMeshTriangle.Normals[0] = triangle.Normals[i];
                        rightMeshTriangle.Normals[1] = rightMeshTriangle.Normals[0];

                    }
                    else
                    {
                        rightMeshTriangle.Vertices[1] = triangle.Vertices[i];
                        rightMeshTriangle.Normals[1] = triangle.Normals[i];
                        rightMeshTriangle.UVs[1] = triangle.UVs[i];
                    }
                }
            }

            plane.Raycast(new Ray(leftMeshTriangle.Vertices[0], (rightMeshTriangle.Vertices[0] - leftMeshTriangle.Vertices[0]).normalized), out var distance);

            var normalizedDistance = distance / (rightMeshTriangle.Vertices[0] - leftMeshTriangle.Vertices[0]).magnitude;
            Vector3 vertLeft = Vector3.Lerp(leftMeshTriangle.Vertices[0], rightMeshTriangle.Vertices[0], normalizedDistance);
            addedVertices.Add(vertLeft);

            Vector3 normalLeft = Vector3.Lerp(leftMeshTriangle.Normals[0], rightMeshTriangle.Normals[0], normalizedDistance);
            Vector2 uvLeft = Vector2.Lerp(leftMeshTriangle.UVs[0], rightMeshTriangle.UVs[0], normalizedDistance);

            plane.Raycast(new Ray(leftMeshTriangle.Vertices[1], (rightMeshTriangle.Vertices[1] - leftMeshTriangle.Vertices[1]).normalized), out distance);

            normalizedDistance = distance / (rightMeshTriangle.Vertices[1] - leftMeshTriangle.Vertices[1]).magnitude;
            Vector3 vertRight = Vector3.Lerp(leftMeshTriangle.Vertices[1], rightMeshTriangle.Vertices[1], normalizedDistance);
            addedVertices.Add(vertRight);

            Vector3 normalRight = Vector3.Lerp(leftMeshTriangle.Normals[1], rightMeshTriangle.Normals[1], normalizedDistance);
            Vector2 uvRight = Vector2.Lerp(leftMeshTriangle.UVs[1], rightMeshTriangle.UVs[1], normalizedDistance);

            //TESTING OUR FIRST TRIANGLE
            Vector3[] updatedVertices = new Vector3[] { leftMeshTriangle.Vertices[0], vertLeft, vertRight };
            Vector3[] updatedNormals = new Vector3[] { leftMeshTriangle.Normals[0], normalLeft, normalRight };
            Vector2[] updatedUVs = new Vector2[] { leftMeshTriangle.UVs[0], uvLeft, uvRight };

            var currentTriangle = new MeshTriangle(updatedVertices, updatedNormals, updatedUVs, triangle.SubmeshIndex);

            //If our vertices aren't the same
            if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
            {
                if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
                {
                    FlipTriangle(currentTriangle);
                }
                leftSide.AddTriangle(currentTriangle);
            }

            //SECOND TRIANGLE 
            updatedVertices = new Vector3[] { leftMeshTriangle.Vertices[0], leftMeshTriangle.Vertices[1], vertRight };
            updatedNormals = new Vector3[] { leftMeshTriangle.Normals[0], leftMeshTriangle.Normals[1], normalRight };
            updatedUVs = new Vector2[] { leftMeshTriangle.UVs[0], leftMeshTriangle.UVs[1], uvRight };


            currentTriangle = new MeshTriangle(updatedVertices, updatedNormals, updatedUVs, triangle.SubmeshIndex);
            //If our vertices aren't the same
            if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
            {
                if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
                {
                    FlipTriangle(currentTriangle);
                }
                leftSide.AddTriangle(currentTriangle);
            }

            //THIRD TRIANGLE 
            updatedVertices = new Vector3[] { rightMeshTriangle.Vertices[0], vertLeft, vertRight };
            updatedNormals = new Vector3[] { rightMeshTriangle.Normals[0], normalLeft, normalRight };
            updatedUVs = new Vector2[] { rightMeshTriangle.UVs[0], uvLeft, uvRight };

            currentTriangle = new MeshTriangle(updatedVertices, updatedNormals, updatedUVs, triangle.SubmeshIndex);
            //If our vertices aren't the same
            if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
            {
                if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
                {
                    FlipTriangle(currentTriangle);
                }
                rightSide.AddTriangle(currentTriangle);
            }

            //FOURTH TRIANGLE 
            updatedVertices = new Vector3[] { rightMeshTriangle.Vertices[0], rightMeshTriangle.Vertices[1], vertRight };
            updatedNormals = new Vector3[] { rightMeshTriangle.Normals[0], rightMeshTriangle.Normals[1], normalRight };
            updatedUVs = new Vector2[] { rightMeshTriangle.UVs[0], rightMeshTriangle.UVs[1], uvRight };

            currentTriangle = new MeshTriangle(updatedVertices, updatedNormals, updatedUVs, triangle.SubmeshIndex);
            //If our vertices aren't the same
            if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
            {
                if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
                {
                    FlipTriangle(currentTriangle);
                }
                rightSide.AddTriangle(currentTriangle);
            }
        }

        private static void FlipTriangle(MeshTriangle triangle)
        {
            Vector3 temp = triangle.Vertices[2];
            triangle.Vertices[2] = triangle.Vertices[0];
            triangle.Vertices[0] = temp;

            temp = triangle.Normals[2];
            triangle.Normals[2] = triangle.Normals[0];
            triangle.Normals[0] = temp;

            Vector2 temp2 = triangle.UVs[2];
            triangle.UVs[2] = triangle.UVs[0];
            triangle.UVs[0] = temp2;

        }

        public static void FillCut(List<Vector3> addedVertices, Plane plane, GeneratedMesh leftMesh, GeneratedMesh rightMesh)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> polygons = new List<Vector3>();

            for (int i = 0; i < addedVertices.Count; i++)
            {
                if (!vertices.Contains(addedVertices[i]))
                {
                    polygons.Clear();
                    polygons.Add(addedVertices[i]);
                    polygons.Add(addedVertices[i + 1]);

                    vertices.Add(addedVertices[i]);
                    vertices.Add(addedVertices[i + 1]);

                    EvaluatePairs(addedVertices, vertices, polygons);
                    Fill(polygons, plane, leftMesh, rightMesh);
                }
            }
        }

        public static void EvaluatePairs(List<Vector3> addedVertices, List<Vector3> vertices, List<Vector3> polygons)
        {
            bool isDone = false;
            while (!isDone)
            {
                isDone = true;
                for (int i = 0; i < addedVertices.Count; i += 2)
                {
                    if (addedVertices[i] == polygons[polygons.Count - 1] && !vertices.Contains(addedVertices[i + 1]))
                    {
                        isDone = false;
                        polygons.Add(addedVertices[i + 1]);
                        vertices.Add(addedVertices[i + 1]);
                    }
                    else if (addedVertices[i + 1] == polygons[polygons.Count - 1] && !vertices.Contains(addedVertices[i]))
                    {
                        isDone = false;
                        polygons.Add(addedVertices[i]);
                        vertices.Add(addedVertices[i]);
                    }
                }
            }
        }

        public static void Fill(List<Vector3> vertices, Plane plane, GeneratedMesh leftMesh, GeneratedMesh rightMesh)
        {
            //Firstly we need the center we do this by adding up all the vertices and then calculating the average
            Vector3 centerPosition = Vector3.zero;
            for (int i = 0; i < vertices.Count; i++)
            {
                centerPosition += vertices[i];
            }
            centerPosition = centerPosition / vertices.Count;

            //We now need an Upward Axis we use the plane we cut the mesh with for that 
            Vector3 up = new Vector3()
            {
                x = plane.normal.x,
                y = plane.normal.y,
                z = plane.normal.z
            };

            Vector3 left = Vector3.Cross(plane.normal, up);

            for (int i = 0; i < vertices.Count; i++)
            {
                var displacement = vertices[i] - centerPosition;
                var uv1 = new Vector2()
                {
                    x = .5f + Vector3.Dot(displacement, left),
                    y = .5f + Vector3.Dot(displacement, up)
                };

                displacement = vertices[(i + 1) % vertices.Count] - centerPosition;
                var uv2 = new Vector2()
                {
                    x = .5f + Vector3.Dot(displacement, left),
                    y = .5f + Vector3.Dot(displacement, up)
                };

                Vector3[] verticesList = new Vector3[] { vertices[i], vertices[(i + 1) % vertices.Count], centerPosition };
                Vector3[] normals = new Vector3[] { -plane.normal, -plane.normal, -plane.normal };
                Vector2[] uvs = new Vector2[] { uv1, uv2, new Vector2(0.5f, 0.5f) };

                MeshTriangle currentTriangle = new MeshTriangle(verticesList, normals, uvs, originalMesh.subMeshCount + 1);

                if (Vector3.Dot(Vector3.Cross(verticesList[1] - verticesList[0], verticesList[2] - verticesList[0]), normals[0]) < 0)
                {
                    FlipTriangle(currentTriangle);
                }
                leftMesh.AddTriangle(currentTriangle);

                normals = new Vector3[] { plane.normal, plane.normal, plane.normal };
                currentTriangle = new MeshTriangle(verticesList, normals, uvs, originalMesh.subMeshCount + 1);

                if (Vector3.Dot(Vector3.Cross(verticesList[1] - verticesList[0], verticesList[2] - verticesList[0]), normals[0]) < 0)
                {
                    FlipTriangle(currentTriangle);
                }
                rightMesh.AddTriangle(currentTriangle);

            }
        }
    }
}

