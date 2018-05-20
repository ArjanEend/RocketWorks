using System.IO;
using UnityEditor;

public class PostBuildEventController : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        //here call your .csproj generation code
        var di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Assets"));
        FileInfo[] fis = di.GetFiles("*.cs", SearchOption.AllDirectories);

        var mainDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        FileInfo[] solutionFile = mainDir.GetFiles("*.sln", SearchOption.TopDirectoryOnly);
        for(int i = 0; i < solutionFile.Length; i++)
        {
            var fStream = solutionFile[i].OpenWrite();
            
        }


    }
}