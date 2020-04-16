using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Fenderrio.ImageWarp
{
	[CustomEditor( typeof(ImageWarp) )]
	public class ImageWarpEditor : UnityEditor.UI.ImageEditor {

		private Vector2 m_tempImageDimensionsVec = Vector2.zero;

		private ImageWarp m_imageWarp;
		private ImageWarp imageWarp { get {
				if (m_imageWarp == null)
				{
					m_imageWarp = target as ImageWarp;
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
			if (imageWarp.preserveAspect)
			{
				float widthRatio = imageWarp.sprite.rect.width / imageWarp.sprite.rect.height;

				if (imageWarp.rectTransform.rect.width > imageWarp.rectTransform.rect.height * widthRatio)
				{
					// Image dimensions are wider than the original image aspect ratio
					m_tempImageDimensionsVec = new Vector2 (imageWarp.rectTransform.rect.height * widthRatio, imageWarp.rectTransform.rect.height);
				}
				else
				{
					// Image dimensions are taller than the original image aspect ratio
					m_tempImageDimensionsVec = new Vector2 (imageWarp.rectTransform.rect.width, imageWarp.rectTransform.rect.width * (imageWarp.sprite.rect.height / imageWarp.sprite.rect.width));
				}
			}
			else
			{
				m_tempImageDimensionsVec = new Vector2 (imageWarp.rectTransform.rect.width, imageWarp.rectTransform.rect.height);
			}

			imageWarp.warpManager.OnSceneGUI (
				serializedObject,
				imageWarp.TransformComponent,
				m_tempImageDimensionsVec,
				imageWarp.RectTransformComponent.pivot);
		}
	}
}