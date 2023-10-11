
using System.IO;
using BT.Editor;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using File = System.IO.File;

/// <summary>
/// Responsible for initializing, configuring and
/// installing the behavior tree editor.
/// </summary>
public static class BTInstaller
{
    /// <summary>
    /// Path to the source bt config file.
    /// </summary>
    private const string configSrc = "Packages/com.ai.behavior-tree/Editor/Installer/bt.Config.json";
    
    /// <summary>
    /// The destination path of the config file.
    /// </summary>
    private const string configDest = "Assets/BT.Config/bt.config.json";
    
    /// <summary>
    /// Unserialized behavior tree configuration data.
    /// </summary>
    public static ConfigData btConfig { get; private set; }
    
    [InitializeOnLoadMethod]
    [MenuItem("Window/AI/Update behavior tree config")]
    private static void InitPackage()
    {
        // If there's no config folder, create it.
        if (!AssetDatabase.IsValidFolder("Assets/BT.Config"))
            AssetDatabase.CreateFolder("Assets", "BT.Config");
        
        // If there's not config file in asset folder, copy the default one and move it there.
        if (!File.Exists(configDest))
            FileUtil.CopyFileOrDirectoryFollowSymlinks(configSrc, configDest);
        
        // Read configuration data from bt.config.json
        string jsonString = File.ReadAllText(configDest);
        btConfig = JsonConvert.DeserializeObject<ConfigData>(jsonString);
        
        // Initialize and install script templates.
        InitScriptTemplates(btConfig.scriptTemplates);
    }
    
    /// <summary>
    /// Initialize and install script templates with supplied data.
    /// </summary>
    /// <param name="templates"> The template data used by the installer. </param>
    private static void InitScriptTemplates(ScriptTemplateData[] templates)
    {
        foreach (ScriptTemplateData template in templates)
        {
            // Find parent and child folders of this path.
            string[] folders = template.dest.Split("/");
            string childFolder = folders[^1];
            string parentFolder = folders[^2];
            
            // Check if the script folder already exist, if not create it inside
            // the asset folder.
            if (!AssetDatabase.IsValidFolder(template.dest))
                AssetDatabase.CreateFolder(parentFolder, childFolder);
            
            // Install all template files.
            foreach (string templateFile in template.templates)
            {
                // Build template filepath.
                string src = template.src + "/" +  templateFile;
                string dest = template.dest + "/" +  templateFile;
         
                // If file already exist replace it, otherwise copy file from directory to destination.
                if (File.Exists(dest))
                    FileUtil.ReplaceFile(src, dest);
                else
                    FileUtil.CopyFileOrDirectoryFollowSymlinks(src, dest);
            }
        }
    }
    
}
