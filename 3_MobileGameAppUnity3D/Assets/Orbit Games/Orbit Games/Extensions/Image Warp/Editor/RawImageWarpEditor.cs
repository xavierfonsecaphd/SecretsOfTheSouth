using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Fenderrio.ImageWarp
{
	[CustomEditor( typeof(RawImageWarp) )]
	public class RawImageWarpEditor : UnityEditor.UI.RawImageEditor {

		private RawImageWarp m_imageWarp;
		private RawImageWarp imageWarp { get {
				if (m_imageWarp == null)
				{
					m_imageWarp = target as RawImageWarp;
				}
				return m_imageWarp;
			}
		}

		public override void OnInspectorGUI ()
		{
			// Draw default inspector content
			base.OnInspectorGUI();

			if (imageWarp.warpManager.OnInspectorGUI (serializedObject))
			{
				imageWarp.ForceUpdateGeometry ();
			}
		}

		private void OnSceneGUI()
		{
			imageWarp.warpManager.OnSceneGUI (
				serializedObject,
				imageWarp.TransformComponent,
				new Vector2 (imageWarp.rectTransform.rect.width, imageWarp.rectTransform.rect.height),
				imageWarp.RectTransformComponent.pivot);
		}
	}
}