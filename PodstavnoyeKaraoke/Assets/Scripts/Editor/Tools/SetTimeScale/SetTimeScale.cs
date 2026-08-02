using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[InitializeOnLoad]
public class SetTimeScale
{
	private const string m_scalex01 = "Tools/TimeScale/x0.1";
	private const string m_scalex1 = "Tools/TimeScale/FromGame";
	private const string m_scalex2 = "Tools/TimeScale/x2";
	private const string m_scalex10 = "Tools/TimeScale/x10";
	private const string m_scalex100 = "Tools/TimeScale/x100";

	private static float gForceScaleSetByPlayer = -1.0f;
	
	static SetTimeScale()
	{
		EditorApplication.update += UpdateSetTimeScale;
		EditorApplication.playModeStateChanged += PlayModeStateChanged;
	}
	
	static void PlayModeStateChanged(PlayModeStateChange newState)
	{
		if (newState == PlayModeStateChange.EnteredPlayMode)
		{
			gForceScaleSetByPlayer = -1.0f;
			Time.timeScale = 1.0f;
		}
	}
	
	static void UpdateSetTimeScale()
	{
		if (gForceScaleSetByPlayer != -1.0f)
		{
			if (gForceScaleSetByPlayer != Time.timeScale)
				Time.timeScale = gForceScaleSetByPlayer;
		}
	}
	
	[MenuItem(m_scalex01, true)]
	private static bool Scale01Validate() {
		Menu.SetChecked(m_scalex01, gForceScaleSetByPlayer == 0.1f);
		return true;
	}
	
	[MenuItem(m_scalex01)]
	public static void SetScale01()
	{
		gForceScaleSetByPlayer = 0.1f;
		Time.timeScale = 0.1f;		
	}
	
	[MenuItem(m_scalex1, true)]
	private static bool Scale1Validate()
	{
		Menu.SetChecked(m_scalex1, gForceScaleSetByPlayer == -1f);
		return true;
	}
	
	[MenuItem(m_scalex1)]
	public static void SetScale1()
	{
		gForceScaleSetByPlayer = -1.0f;
		Time.timeScale = 1f;		
	}
	
	[MenuItem(m_scalex2, true)]
	private static bool Scale2Validate()
	{
		Menu.SetChecked(m_scalex2, gForceScaleSetByPlayer == 2f);
		return true;
	}
	
	[MenuItem(m_scalex2)]
	public static void SetScale2()
	{
		gForceScaleSetByPlayer = 2.0f;
		Time.timeScale = 2f;
	}
	
	[MenuItem(m_scalex10, true)]
	private static bool Scale10Validate()
	{
		Menu.SetChecked(m_scalex10, gForceScaleSetByPlayer == 10f);
		return true;
	}
	
	[MenuItem(m_scalex10)]
	public static void SetScale10()
	{
		gForceScaleSetByPlayer = 10.0f;
		Time.timeScale = 10f;		
	}
	
	[MenuItem(m_scalex100, true)]
	private static bool Scale100Validate()
	{
		Menu.SetChecked(m_scalex100, gForceScaleSetByPlayer == 100f);
		return true;
	}
	
	[MenuItem(m_scalex100)]
	public static void SetScale100()
	{
		gForceScaleSetByPlayer = 100.0f;
		Time.timeScale = 100f;		
	}
}
#endif