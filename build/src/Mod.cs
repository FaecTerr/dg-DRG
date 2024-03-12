using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;

[assembly: AssemblyTitle("Deep Duck Galactic")]


[assembly: AssemblyCompany("FaecTerr")]


[assembly: AssemblyDescription("Some stuff from DeepRockGalactic")]


[assembly: AssemblyVersion("1.0.0.0")]

namespace DuckGame.DeepDuckGalactic
{
    public class DeepDuckGalactic : Mod
    {
		// The mod's priority; this property controls the load order of the mod.
		public override Priority priority
		{
			get { return base.priority; }
		}

		// This function is run before all mods are finished loading.
		protected override void OnPreInitialize()
		{
			base.OnPreInitialize();
		}

		// This function is run after all mods are loaded.
		protected override void OnPostInitialize()
		{
			base.OnPostInitialize();
            copyLevels();
        }
        private static bool FilesAreEqual(FileInfo first, FileInfo second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }
            int iterations = (int)Math.Ceiling((double)first.Length / 8.0);
            using (FileStream fs = first.OpenRead())
            {
                using (FileStream fs2 = second.OpenRead())
                {
                    byte[] one = new byte[8];
                    byte[] two = new byte[8];
                    for (int i = 0; i < iterations; i++)
                    {
                        fs.Read(one, 0, 8);
                        fs2.Read(two, 0, 8);
                        if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private static void copyLevels()
        {
            string levelFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "DuckGame\\Levels\\DRG");
            if (!Directory.Exists(levelFolder))
            {
                Directory.CreateDirectory(levelFolder);
            }
            foreach (string sourcePath in Directory.GetFiles(Mod.GetPath<DeepDuckGalactic>("levels")))
            {
                string destPath = Path.Combine(levelFolder, Path.GetFileName(sourcePath));
                bool file_exists = File.Exists(destPath);
                if (!file_exists || !DeepDuckGalactic.FilesAreEqual(new FileInfo(sourcePath), new FileInfo(destPath)))
                {
                    if (file_exists)
                    {
                        File.Delete(destPath);
                    }
                    File.Copy(sourcePath, destPath);
                }
            }
        }
    }
}
