using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BMDLConvert
{
    public class OBJ : Mesh
    {
        static readonly char[] SPACE_CHAR = new[] { ' ' };
        const string DEFAULT_NAME = "bmdl-mesh";

        public string Name { get; set; } = DEFAULT_NAME;

        public OBJ(string filename) : base(filename)
        {
        }

        public OBJ()
        {
        }

        protected override void Read(BinaryReader br, MeshBuilder mb)
        {
            using (var r = new StreamReader(br.BaseStream, Encoding.Default, true, 1024, true))
            {
                while (!r.EndOfStream)
                {
                    var line = r.ReadLine();
                    if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                        continue;
                    int commentStart;
                    if ((commentStart = line.IndexOf('#')) >= 0)
                        line = line.Remove(commentStart);
                    while (line != (line = line.Replace("  ", " "))) ;
                    var sp = line.Split(SPACE_CHAR, StringSplitOptions.RemoveEmptyEntries);

                    switch (sp[0].ToLowerInvariant())
                    {
                        case "v":
                            mb.AddVertex(float.Parse(sp[1]), float.Parse(sp[2]), float.Parse(sp[3]));
                            break;
                        case "vn":
                            mb.AddVertexN(float.Parse(sp[1]), float.Parse(sp[2]), float.Parse(sp[3]));
                            break;
                        case "vt":
                            mb.AddVertexT(float.Parse(sp[1]), float.Parse(sp[2]));
                            break;
                        case "f":
                            mb.AddFaceLine(line);
                            break;
                    }
                }
            }
        }

        protected override void Write(BinaryWriter bw)
        {
            using (var w = new StreamWriter(bw.BaseStream, Encoding.Default, 1024, true))
            {
                w.WriteLine($"# Converted using Jayrod's BMDLConvert");
                w.WriteLine($"# {DateTime.Now.ToLongDateString()}");
                w.WriteLine();
                var vDict = new Dictionary<string, int>();
                var vArr = new int[Verts.Count];
                var idx = 0;
                for (int i = 0; i < Verts.Count; i++)
                {
                    var v = Verts[i];
                    var vStr = $"v {v.X} {v.Y} {v.Z}";
                    if (!vDict.TryGetValue(vStr, out var vIdx))
                    {
                        w.WriteLine(vStr);
                        vDict[vStr] = vIdx = idx++;
                    }

                    vArr[i] = vIdx;
                    w.WriteLine($"vn {v.NormalX} {v.NormalY} {v.NormalZ}");
                    w.WriteLine($"vt {v.U} {v.V}");
                }

                w.WriteLine();
                w.WriteLine($"o {Name}");
                w.WriteLine($"g {Name}");
                w.WriteLine();

                foreach (var f in Faces)
                {
                    w.WriteLine($"f {vArr[f.Vert1] + 1}/{f.Vert1 + 1}/{f.Vert1 + 1} {vArr[f.Vert2] + 1}/{f.Vert2 + 1}/{f.Vert2 + 1} {vArr[f.Vert3] + 1}/{f.Vert3 + 1}/{f.Vert3 + 1}");
                }
            }
        }
    }
}
