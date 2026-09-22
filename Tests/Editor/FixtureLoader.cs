using System.IO;
using System;

namespace Laya.Unity.Tests
{
    internal static class FixtureLoader
    {
        internal static string Read(string name)
        {
            var unityPackagePath = Path.Combine(
                "Packages",
                "com.0x4401me.laya.unity",
                "Tests",
                "Editor",
                "Fixtures",
                name);
            if (File.Exists(unityPackagePath)) return File.ReadAllText(unityPackagePath);

            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", name));
        }
    }
}
