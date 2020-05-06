using UnityEngine;

#if UNITY_EDITOR
public class DebugController : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			MainApp.Instance.portalGenerator.GeneratePortal();
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			MainApp.Instance.UpdateMask();
		}

		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			MainApp.Instance.portalGenerator.StartScalingSequence();
		}
	}
}
#endif