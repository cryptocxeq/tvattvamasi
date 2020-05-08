using UnityEngine;

public class OnColliderHit : MonoBehaviour
{
	public System.Action onHit = () => { };

	public void OnHit()
	{
		onHit.Invoke();
	}
}