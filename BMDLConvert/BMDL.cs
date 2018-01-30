using System;
using System.Collections.Generic;
using System.IO;

namespace BMDLConvert
{
    public class BMDL : Mesh
    {
        public BMDL()
        {

        }

        public BMDL(string filename) : base(filename)
        {
        }

        protected override void Read(BinaryReader br, MeshBuilder mb)
        {
            if (br.ReadUInt32() != 0x03030001) throw new InvalidDataException("Bad magicnum");
            int vertCount = br.ReadUInt16();
            var triangleCount = br.ReadUInt16();
            br.BaseStream.Position += 40;
            for (int i = 0; i < vertCount; i++)
            {
                mb.AddVertex(Read3DMMFloat(br), Read3DMMFloat(br), Read3DMMFloat(br), Read3DMMFloat(br), Read3DMMFloat(br));
                br.BaseStream.Position += 12;
            }

            if (br.BaseStream.Position != 48 + vertCount * 32) throw new InvalidDataException();

            var faceList = new List<Face>();

            for (int i = 0; i < triangleCount; i++)
            {
                mb.AddFace(br.ReadUInt16() + 1, br.ReadUInt16() + 1, br.ReadUInt16() + 1);
                br.BaseStream.Position += 26;
            }
        }

        protected override void Write(BinaryWriter bw)
        {
            bw.Write(0x03030001);
            bw.Write((ushort)Verts.Count);
            bw.Write((ushort)Faces.Count);
            bw.BaseStream.Position += 40;

            for (int i = 0; i < Verts.Count; i++)
            {
                Write3DMMFloat(bw, Verts[i].X);
                Write3DMMFloat(bw, Verts[i].Y);
                Write3DMMFloat(bw, Verts[i].Z);
                Write3DMMFloat(bw, Verts[i].U);
                Write3DMMFloat(bw, Verts[i].V);
                bw.BaseStream.Position += 12;
            }

            if (bw.BaseStream.Position != 48 + Verts.Count * 32) throw new InvalidOperationException();

            for (int i = 0; i < Faces.Count; i++)
            {
                bw.Write((ushort)Faces[i].Vert1);
                bw.Write((ushort)Faces[i].Vert2);
                bw.Write((ushort)Faces[i].Vert3);
                bw.BaseStream.Position += 10;
                bw.Write((ushort)1);
                //bw.Write((ushort)Faces[i].Smoothing);
                bw.Write(new byte[14]);
            }
        }

        private float Read3DMMFloat(BinaryReader br)
        {
            return br.ReadInt32() / 65536f;
        }

        private void Write3DMMFloat(BinaryWriter bw, float value)
        {
            bw.Write((int)(value * 65536f));
        }
    }
}