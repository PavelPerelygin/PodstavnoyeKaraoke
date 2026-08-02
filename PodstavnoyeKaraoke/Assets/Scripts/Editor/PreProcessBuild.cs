using Controllers;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Editor
{
    public class PreProcessBuild :  IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public void OnPreprocessBuild(BuildReport report)
        {
        }
    }
}