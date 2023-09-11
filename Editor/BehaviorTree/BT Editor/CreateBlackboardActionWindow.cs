using System;
using UnityEditor;

namespace BT.Editor
{
    public class CreateBlackboardActionWindow : EditorWindow
    {
        private void OnGUI()
        {
            string inputText = EditorGUILayout.TextField("File name: ", "NewBlackboardAsset") + ".asset";
            string filepath = EditorGUILayout.TextField("File name: ", "NewBlackboardAsset") + ".asset";
        }
    }
}