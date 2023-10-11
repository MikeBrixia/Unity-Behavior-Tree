using System;
using System.Collections.Generic;

namespace BT.Editor
{
    /// <summary>
    /// Configuration data used to initialize/install behavior tree editor.
    /// </summary>
    [Serializable]
    public struct ConfigData
    {
        /// <summary>
        /// The src path of the behavior tree.
        /// </summary>
        public string src;
        
        /// <summary>
        /// All the nodes mapped to their respective views.
        /// </summary>
        public Dictionary<string, string> nodeViews;
        
        /// <summary>
        /// All the nodes default nodes mapped to their default views.
        /// </summary>
        public Dictionary<string, string> defaultNodeViews;
        
        /// <summary>
        /// All the registered behavior tree script templates.
        /// </summary>
        public ScriptTemplateData[] scriptTemplates;
        
        /// <summary>
        /// The destination where the config file we'll be copied
        /// </summary>
        public string dest;
    }
    
    /// <summary>
    /// Data used to define behavior tree script templates.
    /// </summary>
    public struct ScriptTemplateData
    {
        /// <summary>
        /// The path where script templates txt files are located.
        /// </summary>
        public string src;
        
        /// <summary>
        /// The destination where you want to install the script templates
        /// </summary>
        public string dest;
        
        /// <summary>
        /// All the script templates.
        /// </summary>
        public string[] templates;
    }
}