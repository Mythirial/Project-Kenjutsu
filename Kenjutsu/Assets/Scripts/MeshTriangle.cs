using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class MeshTriangle
    {
        List<Vector3> _vertices          = new List<Vector3>();
        List<Vector3> _normals           = new List<Vector3>();
        List<Vector2> _uvs               = new List<Vector2>();
        int _submeshIndex;

        public List<Vector3> Vertices           { get { return _vertices; }          set { _vertices = value; } }
        public List<Vector3> Normals            { get { return _normals; }           set { _normals = value; } }
        public List<Vector2> UVs                { get { return _uvs; }               set { _uvs = value; } }
        public int SubmeshIndex                 { get { return _submeshIndex; }      set { SubmeshIndex = value; } }

        public MeshTriangle(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int submeshIndex) 
        {
            Clear();

            _vertices.AddRange(vertices);
            _normals.AddRange(normals);
            _uvs.AddRange(uvs);

            _submeshIndex = submeshIndex;
        }

        public void Clear()
        {
            _vertices.Clear();
            _normals.Clear();
            _uvs.Clear();
        
            _submeshIndex = 0;
        }
    }
}
