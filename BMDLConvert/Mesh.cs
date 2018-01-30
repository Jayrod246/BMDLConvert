using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BMDLConvert
{
    public abstract class Mesh
    {
        public Mesh()
        {
        }

        public Mesh(string filename)
        {
            Load(filename);
        }

        public T To<T>() where T : Mesh, new()
        {
            return new T()
            {
                Verts = this.Verts,
                Faces = this.Faces
            };
        }

        public void Load(string filename)
        {
            Load(File.ReadAllBytes(filename));
        }

        public void Load(byte[] buffer)
        {
            using (var memstream = new MemoryStream(buffer))
            using (var br = new BinaryReader(memstream, Encoding.Default, true))
            {
                var mb = new MeshBuilder();
                Read(br, mb);
                mb.CopyTo(this);
                Clean();
            }
        }

        public IList<Vertex> Verts { get; set; }
        public IList<Face> Faces { get; set; }

        public void Save(string filename)
        {
            using (var memstream = new MemoryStream())
            using (var bw = new BinaryWriter(memstream, Encoding.Default, true))
            {
                Clean();
                Write(bw);
                File.WriteAllBytes(filename, memstream.ToArray());
            }
        }

        private void Clean()
        {
            for (int i = Verts.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    var a = Verts[i];
                    var b = Verts[j];
                    if (a == b)
                    {
                        Verts.RemoveAt(i);
                        foreach (var f in Faces)
                        {
                            if (f.Vert1 == i)
                                f.Vert1 = j;
                            if (f.Vert2 == i)
                                f.Vert2 = j;
                            if (f.Vert3 == i)
                                f.Vert3 = j;
                        }
                        break;
                    }
                }
            }
        }

        protected abstract void Read(BinaryReader br, MeshBuilder mb);

        protected abstract void Write(BinaryWriter bw);
    }
}