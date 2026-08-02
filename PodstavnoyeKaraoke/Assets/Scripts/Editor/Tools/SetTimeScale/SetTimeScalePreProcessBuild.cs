using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Editor.Tools.SetTimeScale
{
    public class SetTimeScalePreProcessBuild: IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public void OnPreprocessBuild(BuildReport report)
        {
            UnityEngine.Time.timeScale = 1f;
        }
    }
}