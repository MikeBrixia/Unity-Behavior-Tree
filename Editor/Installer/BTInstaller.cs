using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using File = System.IO.File;

public static class BTInstaller
{
    /// <summary>
   /// The path to the folder where all script templates are stored.
   /// </summary>
   private const string source = "Packages/com.ai.behavior-tree/Editor/Installer/ScriptTemplates";
    
   /// <summary>
   /// The path to the folder where all script text files must be moved and stored to be
   /// displayed in the asset window.
   /// </summary>
   private const string scriptPath = "Assets/ScriptTemplates";
   
   /// <summary>
   /// Script text files. This files are used to display script templates for BT customs scripts.
   /// </summary>
   private static readonly string[] sourceFiles =
   {
       "01-C# Templates__AI__Action-NewAction.cs.txt",
       "02-C# Templates__AI__Decorator-NewDecorator.cs.txt",
       "03-C# Templates__AI__Service-NewService.cs.txt"
   };
   
   [InitializeOnLoadMethod]
   private static void InitPackage()
   {
       // Check if the script folder already exist, if not create it inside
       // the asset folder.
       if (!AssetDatabase.IsValidFolder(scriptPath))
           AssetDatabase.CreateFolder("Assets", "ScriptTemplates");
       
       // Copy files to Asset/ScriptTemplates folder.
       foreach (string scriptFile in sourceFiles)
       { 
           // Build paths
           string src = System.IO.Path.Combine(source, scriptFile);
           string dest = System.IO.Path.Combine(scriptPath, scriptFile);
          
           // If file already exist replace it, otherwise copy file from directory to destination.
           if(File.Exists(dest))
               FileUtil.ReplaceFile(src, dest);
           else
               FileUtil.CopyFileOrDirectoryFollowSymlinks(src, dest);
       }
   }
}
