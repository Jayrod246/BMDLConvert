using System.IO;

namespace BMDLConvert
{
    class Program
    {
        const string OBJ_EXT = ".obj";
        const string BMDL_EXT = ".bmdl";

        static void Main(string[] args)
        {
            if (args.Length == 2 && TryGetMesh(args[0], out var mesh))
            {
                if (SaveMesh(args[1], mesh))
                    System.Console.WriteLine("Mesh converted successfully.");
            }
        }

        private static bool SaveMesh(string filename, Mesh mesh)
        {
            var dir = Path.GetDirectoryName(filename);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var ext = Path.GetExtension(filename).ToLowerInvariant();

            if (File.Exists(filename))
            {
                int i = 2;
                var fname = Path.GetFileNameWithoutExtension(filename);
                string newFilename;
                do
                {
                    newFilename = Path.Combine(dir, $"{fname}_{i++}{ext}");
                }
                while (File.Exists(newFilename));
                filename = newFilename;
            }

            switch (ext)
            {
                case OBJ_EXT:
                    mesh.To<OBJ>().Save(filename);
                    return true;
                case BMDL_EXT:
                    mesh.To<BMDL>().Save(filename);
                    return true;
            }

            return false;
        }

        private static bool TryGetMesh(string filename, out Mesh mesh)
        {
            if (File.Exists(filename))
            {
                var ext = Path.GetExtension(filename).ToLowerInvariant();

                switch (ext)
                {
                    case OBJ_EXT:
                        mesh = new OBJ(filename);
                        return true;
                    case BMDL_EXT:
                        mesh = new BMDL(filename);
                        return true;
                }
            }

            mesh = null;
            return false;
        }
    }
}
