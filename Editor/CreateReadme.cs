using UnityEditor;
using UnityEngine;
using System.IO;

public static class CreateReadme
{
    [MenuItem("Assets/Create/Readme.md", priority = 80)]
    public static void CreateReadmeFile()
    {
        // Determine where to create the file
        string folderPath = GetSelectedFolderPath();

        string filePath = Path.Combine(folderPath, "Readme.md");

        // Avoid overwriting an existing file — append a number if needed
        if (File.Exists(filePath))
        {
            int counter = 1;
            while (File.Exists(Path.Combine(folderPath, $"Readme_{counter}.md")))
                counter++;
            filePath = Path.Combine(folderPath, $"Readme_{counter}.md");
        }

        // Write default content
        string defaultContent =
            $"# {Path.GetFileNameWithoutExtension(Application.productName)}\n\n" +
            "## Overview\n\n" +
            "Describe your project here.\n\n" +
            "## Getting Started\n\n" +
            "1. Step one\n" +
            "2. Step two\n" +
            "3. Step three\n\n" +
            "## Requirements\n\n" +
            "- Unity version: \n" +
            "- Platform: \n\n" +
            "## License\n\n" +
            "MIT\n";

        File.WriteAllText(filePath, defaultContent);

        // Refresh and highlight the new file in the Project window
        AssetDatabase.Refresh();

        string assetPath = "Assets" + filePath.Substring(Application.dataPath.Length);
        TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        if (asset != null)
        {
            ProjectWindowUtil.ShowCreatedAsset(asset);
            Selection.activeObject = asset;
        }
    }

    /// <summary>
    /// Returns the currently selected folder in the Project window,
    /// or Assets/ if nothing (or a file) is selected.
    /// </summary>
    private static string GetSelectedFolderPath()
    {
        string path = Application.dataPath; // fallback: Assets/

        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            string selectedPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(selectedPath))
                continue;

            // Convert relative asset path to absolute path
            string absolutePath = Path.Combine(
                Application.dataPath.Replace("Assets", ""),
                selectedPath
            );

            if (Directory.Exists(absolutePath))
                return absolutePath;

            if (File.Exists(absolutePath))
                return Path.GetDirectoryName(absolutePath);
        }

        return path;
    }
}