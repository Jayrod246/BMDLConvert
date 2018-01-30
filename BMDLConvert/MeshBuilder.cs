using System;
using System.Collections.Generic;
using System.Linq;

namespace BMDLConvert
{
    public class MeshBuilder
    {
        List<string> vLines;
        List<string> vnLines;
        List<string> vtLines;
        List<string> fLines;

        public MeshBuilder()
        {
            vLines = new List<string>();
            vnLines = new List<string>();
            vtLines = new List<string>();
            fLines = new List<string>();
        }

        public void AddVertex(float x, float y, float z)
        {
            vLines.Add($"v {x} {y} {z} 1.0");
        }

        public void AddVertex(float x, float y, float z, float u, float v)
        {
            AddVertex(x, y, z);
            AddVertexT(u, v);
        }

        public void AddVertexT(float u, float v)
        {
            vtLines.Add($"vt {u} {v} 0.0");
        }

        public void AddVertex(float x, float y, float z, float nx, float ny, float nz)
        {
            AddVertex(x, y, z);
            AddVertexN(nx, ny, nz);
        }

        public void AddVertexN(float nx, float ny, float nz)
        {
            vnLines.Add($"vn {nx} {ny} {nz}");
        }

        public void AddVertex(float x, float y, float z, float nx, float ny, float nz, float u, float v)
        {
            AddVertex(x, y, z, u, v);
            AddVertexN(nx, ny, nz);
        }

        public void AddFace(int v0, int v1, int v2)
        {
            fLines.Add($"f {v0} {v1} {v2}");
        }

        public void AddFaceT(int v0, int v1, int v2, int vt0, int vt1, int vt2)
        {
            fLines.Add($"f {v0}/{vt0} {v1}/{vt1} {v2}/{vt2}");
        }

        public void AddFaceN(int v0, int v1, int v2, int vn0, int vn1, int vn2)
        {
            fLines.Add($"f {v0}//{vn0} {v1}//{vn1} {v2}//{vn2}");
        }

        public void AddFace(int v0, int v1, int v2, int vt0, int vt1, int vt2, int vn0, int vn1, int vn2)
        {
            AddFaceLine($"f {v0}/{vt0}/{vn0} {v1}/{vt1}/{vn1} {v2}/{vt2}/{vn2}");
        }

        public void CopyTo(Mesh mesh)
        {
            var verts = new List<Vertex>();
            var faceIndices = new List<int>();
            var dict = new Dictionary<string, int>();

            foreach (var f in fLines)
            {
                var indices = f.Substring(2).Split(' ');

                for (int i = 0; i < 3; i++)
                {
                    int vIdx;
                    int vnIdx;
                    int vtIdx;

                    var values = indices[i].Split('/');
                    Vertex vertex;
                    if (!dict.TryGetValue(indices[i], out int vertexIndex))
                    {
                        switch (values.Length)
                        {
                            case 1:
                                if (int.TryParse(values[0], out vIdx))
                                    vertex = CreateVertex(vLines[vIdx - 1], null, null);
                                else
                                    throw new InvalidOperationException();
                                break;
                            case 2:
                                if (int.TryParse(values[0], out vIdx) && int.TryParse(values[1], out vtIdx))
                                    vertex = CreateVertex(vLines[vIdx - 1], vtLines[vtIdx - 1], null);
                                else
                                    throw new InvalidOperationException();
                                break;
                            case 3:
                                if (int.TryParse(values[0], out vIdx) && int.TryParse(values[2], out vnIdx))
                                {
                                    string vt = null;
                                    if (values[1].Length > 0 && int.TryParse(values[1], out vtIdx))
                                        vt = vtLines[vtIdx - 1];
                                    vertex = CreateVertex(vLines[vIdx - 1], vt, vnLines[vnIdx - 1]);
                                }
                                else
                                    throw new InvalidOperationException();
                                break;
                            default:
                                throw new InvalidOperationException();
                        }

                        verts.Add(vertex);
                        dict[indices[i]] = vertexIndex = verts.Count - 1;
                    }

                    faceIndices.Add(vertexIndex);
                }
            }

            mesh.Faces = FaceIndices(faceIndices);
            mesh.Verts = verts;
        }

        public void AddFaceLine(string line)
        {
            while (line != (line = line.Replace("  ", " "))) ;
            fLines.Add(line);
        }

        private Face[] FaceIndices(IList<int> faceIndices)
        {
            return enumerateFaces().ToArray();

            IEnumerable<Face> enumerateFaces()
            {
                for (int i = 0; i < faceIndices.Count; i += 3)
                {
                    yield return new Face()
                    {
                        Vert1 = faceIndices[i],
                        Vert2 = faceIndices[i + 1],
                        Vert3 = faceIndices[i + 2]
                    };
                }
            }
        }

        private Vertex CreateVertex(string v, string vt, string vn)
        {
            float x;
            float y;
            float z;
            float nx;
            float ny;
            float nz;
            float u;
            float _v;

            if (!string.IsNullOrEmpty(v) && v.StartsWith("v "))
            {
                var values = v.Substring(2).Split(' ');
                float.TryParse(values[0], out x);
                float.TryParse(values[1], out y);
                float.TryParse(values[2], out z);
            }
            else
            {
                x = y = z = 0f;
            }

            if (!string.IsNullOrEmpty(vn) && vn.StartsWith("vn "))
            {
                var values = vn.Substring(3).Split(' ');
                float.TryParse(values[0], out nx);
                float.TryParse(values[1], out ny);
                float.TryParse(values[2], out nz);
            }
            else
            {
                nx = ny = nz = 0f;
            }

            if (!string.IsNullOrEmpty(vt) && vt.StartsWith("vt "))
            {
                var values = vt.Substring(3).Split(' ');
                float.TryParse(values[0], out u);
                float.TryParse(values[1], out _v);
            }
            else
            {
                u = _v = 0f;
            }

            return new Vertex()
            {
                X = x,
                Y = y,
                Z = z,
                NormalX = nx,
                NormalY = ny,
                NormalZ = nz,
                U = u,
                V = _v
            };
        }
    }
}
