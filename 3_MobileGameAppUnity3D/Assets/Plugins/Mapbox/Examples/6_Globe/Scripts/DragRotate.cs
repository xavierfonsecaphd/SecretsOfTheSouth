namespace Mapbox.Examples
{
	using UnityEngine;

	namespace Scripts.Utilities
	{
		public class DragRotate : MonoBehaviour
		{
			[SerializeField]
			Transform _objectToRotate = null;

			[SerializeField]
			float _multiplier = 0.0f;

			Vector3 _startTouchPosition;

			void Update()
			{
				if (Input.GetMouseButtonDown(0))
				{
					_startTouchPosition = Input.mousePosition;
				}

				if (Input.GetMouseButton(0))
				{
					var dragDelta = Input.mousePosition - _startTouchPosition;
					var axis = new Vector3(0f, -dragDelta.x * _multiplier, 0f);
					_objectToRotate.RotateAround(_objectToRotate.position, axis, _multiplier);
				}
			}
		}
	}
}
