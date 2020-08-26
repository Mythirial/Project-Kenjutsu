using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GeneratedMesh
    {
        List<Vector3> _vertices = new List<Vector3>();
        List<Vector3> _normals = new List<Vector3>();
        List<Vector2> _uvs = new List<Vector2>();
        List<List<int>> _submeshIndices = new List<List<int>>();

        public List<Vector3> Vertices { get { return _vertices; } set { _vertices = value; } }
        public List<Vector3> Normals { get { return _normals; } set { _normals = value; } }
        public List<Vector2> UVs { get { return _uvs; } set { _uvs = value; } }
        public List<List<int>> SubmeshIndices { get { return _submeshIndices; } set { _submeshIndices = value; } }

        public void AddTriangle(MeshTriangle _triangle)
        {
            int currentVerticesCount = _vertices.Count;

            _vertices.AddRange(_triangle.Vertices);
            _normals.AddRange(_triangle.Normals);
            _uvs.AddRange(_triangle.UVs);

            if (_submeshIndices.Count < _triangle.SubmeshIndex + 1)
            {
                for (int i = _submeshIndices.Count; i < _triangle.SubmeshIndex + 1; i++)
                {
                    _submeshIndices.Add(new List<int>());
                }
            }

            for (int i = 0; i < 3; i++)
            {
                _submeshIndices[_triangle.SubmeshIndex].Add(currentVerticesCount + i);
            }
        }

        public void AddTriangle(Vector3[] _vertices, Vector3[] _normals, Vector2[] _uvs, int _submeshIndex, Vector4[] _tangents = null)
        {
            int currentVerticeCount = this._vertices.Count;

            this._vertices.AddRange(_vertices);
            this._normals.AddRange(_normals);
            this._uvs.AddRange(_uvs);

            if (_submeshIndices.Count < _submeshIndex + 1)
            {
                for (int i = _submeshIndices.Count; i < _submeshIndex + 1; i++)
                {
                    _submeshIndices.Add(new List<int>());
                }
            }

            for (int i = 0; i < 3; i++)
            {
                _submeshIndices[_submeshIndex].Add(currentVerticeCount + i);
            }
        }



        public Mesh GetGeneratedMesh()
        {
            Mesh mesh = new Mesh();
            mesh.SetVertices(_vertices);
            mesh.SetNormals(_normals);
            mesh.SetUVs(0, _uvs);
            mesh.SetUVs(1, _uvs);

            mesh.subMeshCount = _submeshIndices.Count;
            for (int i = 0; i < _submeshIndices.Count; i++)
            {
                mesh.SetTriangles(_submeshIndices[i], i);
            }
            return mesh;
        }

    }
}
