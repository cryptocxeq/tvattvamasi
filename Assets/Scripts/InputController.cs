using UnityEngine;

public class InputController : MonoBehaviour
{
	public System.Action<Vector3> onInputDown = (vec) => { };
	public System.Action<Vector3> onInput = (vec) => { };
	public System.Action onInputUp = () => { };

#if UNITY_ANDROID || UNITY_IOS
	private int lastTouchCount = 0;
#endif

	private void Update()
	{
		Vector3 inputPosition;
#if UNITY_ANDROID || UNITY_IOS
		if (Input.touchCount > 0)
		{
			inputPosition = Input.touches[0].position;
			if (lastTouchCount <= 0)
				onInputDown.Invoke(inputPosition);
			else
				onInput.Invoke(inputPosition);
		} else if (lastTouchCount > 0)
			onInputUp.Invoke();
		lastTouchCount = Input.touchCount;
#endif
#if UNITY_EDITOR || UNITY_STANDALONE
		inputPosition = Input.mousePosition;
		if (Input.GetMouseButtonDown(0))
			onInputDown.Invoke(inputPosition);
		else if (Input.GetMouseButton(0))
			onInput.Invoke(inputPosition);
		else if (Input.GetMouseButtonUp(0))
			onInputUp.Invoke();
#endif
	}
}