namespace Mapbox.Unity.Utilities
{
	using UnityEngine;
	using UnityEngine.UI;

	[RequireComponent(typeof(Button))]
	public class TelemetryConfigurationButton : MonoBehaviour
	{
		[SerializeField]
		bool _booleanValue = false;

		void Awake()
		{
			GetComponent<Button>().onClick.AddListener(SetPlayerPref);
		}

		void SetPlayerPref()
		{
			MapboxAccess.Instance.SetLocationCollectionState(_booleanValue);
			PlayerPrefs.Save();
		}
	}
}