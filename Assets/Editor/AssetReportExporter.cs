
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

public class AssetReportExporter
{
    [MenuItem("Tools/Asset Report/Export Scene Dependencies CSV")]
    static void ExportReport()
    {
        string[] scenes = AssetDatabase.FindAssets("t:Scene")
            .Select(AssetDatabase.GUIDToAssetPath)
            .ToArray();

        string[] dependencies =
            AssetDatabase.GetDependencies(scenes, true);

        StringBuilder csv = new StringBuilder();
        csv.AppendLine("Asset Name,Asset Type,Asset Path");

        foreach (string path in dependencies.Distinct().OrderBy(p => p))
        {
            string name = Path.GetFileName(path);
            string extension = Path.GetExtension(path);
            string type = extension;

            if (string.IsNullOrEmpty(extension))
                type = "Other";

            csv.AppendLine(
                $"\"{name}\",\"{type}\",\"{path}\""
            );
        }

        string outputPath = Path.Combine(
            Application.dataPath,
            "../Unity_Asset_Report.csv"
        );

        File.WriteAllText(outputPath, csv.ToString());

        Debug.Log("Asset report exported to: " + outputPath);

        EditorUtility.RevealInFinder(outputPath);
    }
}
