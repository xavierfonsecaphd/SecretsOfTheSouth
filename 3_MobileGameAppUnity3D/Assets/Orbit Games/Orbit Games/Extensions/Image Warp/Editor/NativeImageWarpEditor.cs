using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Fenderrio.ImageWarp
{
	[CustomEditor( typeof(NativeImageWarp) )]
	public class NativeImageWarpEditor : NativeTextureRendererEditor {

		private NativeImageWarp m_nativeImageWarp;
		private NativeImageWarp nativeImageWarp {
			get {
				if (m_nativeImageWarp == null)
				{
					m_nativeImageWarp = (NativeImageWarp) target;
				}
				return m_nativeImageWarp;
			}
		}

		public override void OnInspectorGUI ()
		{
			// Draw default inspector content
			base.OnInspectorGUI();

			bool baseGUIChanged = GUI.changed;

			if (nativeImageWarp.warpManager.OnInspectorGUI (serializedObject) && !baseGUIChanged)
			{
				nativeImageWarp.UpdateMesh (true);
			}
		}

		private void OnSceneGUI()
		{
			if (nativeImageWarp.warpManager.OnSceneGUI (
				   serializedObject,
				   nativeImageWarp.TransformComponent,
				   nativeImageWarp.Dimensions))
			{
				nativeImageWarp.UpdateMesh (true);
			}
		}
	}
}