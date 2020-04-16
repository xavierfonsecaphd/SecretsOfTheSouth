using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fenderrio.ImageWarp
{
	[AddComponentMenu("UI/Image Warp")]
	public class ImageWarp : Image, IWarp
	{
		[SerializeField] private Vector3 m_cornerOffsetTL;
		public Vector3 cornerOffsetTL { get { return m_cornerOffsetTL; } set { if (SetStruct(ref m_cornerOffsetTL, value)) SetVerticesDirty (); } }

		[SerializeField] private Vector3 m_cornerOffsetTR;
		public Vector3 cornerOffsetTR { get { return m_cornerOffsetTR; } set { if (SetStruct(ref m_cornerOffsetTR, value)) SetVerticesDirty(); } }

		[SerializeField] private Vector3 m_cornerOffsetBR;
		public Vector3 cornerOffsetBR { get { return m_cornerOffsetBR; } set { if (SetStruct(ref m_cornerOffsetBR, value)) SetVerticesDirty(); } }

		[SerializeField] private Vector3 m_cornerOffsetBL;
		public Vector3 cornerOffsetBL { get { return m_cornerOffsetBL; } set { if (SetStruct(ref m_cornerOffsetBL, value)) SetVerticesDirty(); } }

		[SerializeField] private int m_numSubdivisions = 10;
		public int numSubdivisions { get { return m_numSubdivisions; } set { if (SetStruct (ref m_numSubdivisions, value)) SetVerticesDirty (); } }

		[SerializeField] private bool m_bezierEdges = false;
		public bool bezierEdges { get { return m_bezierEdges; } set { if (SetStruct (ref m_bezierEdges, value)) SetVerticesDirty (); } }

		[SerializeField] private Vector3 m_topBezierHandleA;
		public Vector3 topBezierHandleA { get { return m_topBezierHandleA; } set { if (SetStruct (ref m_topBezierHandleA, value)) SetVerticesDirty (); } }

		[SerializeField] private Vector3 m_topBezierHandleB;
		public Vector3 topBezierHandleB { get { return m_topBezierHandleB; } set { if (SetStruct (ref m_topBezierHandleB, value)) SetVerticesDirty (); } }

		[SerializeField] private Vector3 m_leftBezierHandleA;
		public Vector3 leftBezierHandleA { get { return m_leftBezierHandleA; } set { if (SetStruct (ref m_leftBezierHandleA, value)) SetVerticesDirty (); } }

		[SerializeField] private Vector3 m_leftBezierHandleB;
		public Vector3 leftBezierHandleB { get { return m_leftBezierHandleB; } set { if (SetStruct (ref m_leftBezierHandleB, value)) SetVerticesDirty (); } }

		[SerializeField] private Vector3 m_rightBezierHandleA;
		public Vector3 rightBezierHandleA { get { return m_rightBezierHandleA; } set { if (SetStruct (ref m_rightBezierHandleA, value)) SetVerticesDirty (); } }

		[SerializeField] private Vector3 m_rightBezierHandleB;
		public Vector3 rightBezierHandleB { get { return m_rightBezierHandleB; } set { if (SetStruct (ref m_rightBezierHandleB, value)) SetVerticesDirty (); } }

		[SerializeField] private Vector3 m_bottomBezierHandleA;
		public Vector3 bottomBezierHandleA { get { return m_bottomBezierHandleA; } set { if (SetStruct (ref m_bottomBezierHandleA, value)) SetVerticesDirty (); } }

		[SerializeField] private Vector3 m_bottomBezierHandleB;
		public Vector3 bottomBezierHandleB { get { return m_bottomBezierHandleB; } set { if (SetStruct (ref m_bottomBezierHandleB, value)) SetVerticesDirty (); } }

		[SerializeField] private float m_cropLeft;
		public float cropLeft { get { return m_cropLeft; } set { if (SetStruct(ref m_cropLeft, value)) SetVerticesDirty (); } }

		[SerializeField] private float m_cropRight;
		public float cropRight { get { return m_cropRight; } set { if (SetStruct(ref m_cropRight, value)) SetVerticesDirty (); } }

		[SerializeField] private float m_cropTop;
		public float cropTop { get { return m_cropTop; } set { if (SetStruct(ref m_cropTop, value)) SetVerticesDirty (); } }

		[SerializeField] private float m_cropBottom;
		public float cropBottom { get { return m_cropBottom; } set { if (SetStruct(ref m_cropBottom, value)) SetVerticesDirty (); } }


		[SerializeField]
		private WarpManager m_warpManager;
		public WarpManager warpManager { get { return m_warpManager; } }

		private List<UIVertex> m_meshVerts;
		private Vector3[] m_meshVertsVec3;
		private Vector2[] m_meshUvsVec2;
		private WarpManager.WarpManagerInstanceData m_warpManagerData;
		private WarpManager.WarpManagerMeshData m_warpedMeshData;

		private RectTransform m_rectTransform;
		public RectTransform RectTransformComponent {
			get {
				if (m_rectTransform == null)
				{
					m_rectTransform = rectTransform;
				}
				return m_rectTransform;
			}
		}

		private Transform m_transform;
		public Transform TransformComponent {
			get {
				if (m_transform == null)
					m_transform = transform;
				return m_transform;
			}
		}

		[SerializeField, HideInInspector]
		private bool m_initialised = false;

		private bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (currentValue.Equals(newValue))
				return false;

			currentValue = newValue;
			return true;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			// get default mesh positions
			base.OnPopulateMesh(vh);

			if (type != Type.Simple)
			{
				Debug.LogWarning ("Slice, Tiled and Filled sprite types aren't supported by UIImageWarp. Please set to 'Simple'.");
				return;
			}

			if (m_warpManager == null)
			{
				m_warpManager = new WarpManager ();
			}

			if (m_warpedMeshData == null)
			{
				m_warpedMeshData = new WarpManager.WarpManagerMeshData ();
			}

			PopulateWarpManagerData ();

			if (m_meshVerts == null)
			{
				m_meshVerts = new List<UIVertex> ();
				m_meshVertsVec3 = new Vector3[4];
				m_meshUvsVec2 = new Vector2[4];
			}

			// Grab the default calculated mesh vert positions
			vh.GetUIVertexStream (m_meshVerts);

			// Populate vector3 array of mesh vert positions
			m_meshVertsVec3 [0] = m_meshVerts [0].position;
			m_meshVertsVec3 [1] = m_meshVerts [1].position;
			m_meshVertsVec3 [2] = m_meshVerts [2].position;
			m_meshVertsVec3 [3] = m_meshVerts [4].position;

			// Populate vector2 array of mesh vert uvs
			m_meshUvsVec2 [0] = m_meshVerts[0].uv0;
			m_meshUvsVec2 [1] = m_meshVerts[1].uv0;
			m_meshUvsVec2 [2] = m_meshVerts[2].uv0;
			m_meshUvsVec2 [3] = m_meshVerts[4].uv0;

			// Get warp mesh data
			m_warpManager.PopulateMesh (m_meshVertsVec3, m_meshUvsVec2, m_warpManagerData, ref m_warpedMeshData);


			// Repopulate mesh data with Warped data
			vh.Clear();

			for (int idx = 0; idx < m_warpedMeshData.m_positions.Length; idx++)
			{
				vh.AddVert(m_warpedMeshData.m_positions[idx], color, m_warpedMeshData.m_uvs[idx]);
			}

			for (int idx = 0; idx < m_warpedMeshData.m_indices.Length; idx += 3)
			{
				vh.AddTriangle(m_warpedMeshData.m_indices[idx], m_warpedMeshData.m_indices[idx + 1], m_warpedMeshData.m_indices[idx + 2]);
			}


			if (!m_initialised)
			{
				m_topBezierHandleA = new Vector3(m_warpManager.Width / 3f,0,0);
				m_topBezierHandleB = new Vector3(-m_warpManager.Width / 3f,0,0);
				m_bottomBezierHandleA = new Vector3(-m_warpManager.Width / 3f,0,0);
				m_bottomBezierHandleB = new Vector3(m_warpManager.Width / 3f,0,0);
				m_leftBezierHandleA = new Vector3(0,m_warpManager.Height / 3f,0);
				m_leftBezierHandleB = new Vector3(0,-m_warpManager.Height / 3f,0);
				m_rightBezierHandleA = new Vector3(0,-m_warpManager.Height / 3f,0);
				m_rightBezierHandleB = new Vector3(0,m_warpManager.Height / 3f,0);

				m_initialised = true;
			}
		}

		private void PopulateWarpManagerData()
		{
			if (m_warpManagerData == null)
			{
				m_warpManagerData = new WarpManager.WarpManagerInstanceData ();
			}

			m_warpManagerData.m_bezierEdges = m_bezierEdges;
			m_warpManagerData.m_numSubdivisions = m_numSubdivisions;

			m_warpManagerData.m_cornerOffsetBL = m_cornerOffsetBL;
			m_warpManagerData.m_cornerOffsetTL = m_cornerOffsetTL;
			m_warpManagerData.m_cornerOffsetTR = m_cornerOffsetTR;
			m_warpManagerData.m_cornerOffsetBR = m_cornerOffsetBR;

			m_warpManagerData.m_bottomCurveHandleA = m_bottomBezierHandleA;
			m_warpManagerData.m_bottomCurveHandleB = m_bottomBezierHandleB;
			m_warpManagerData.m_leftCurveHandleA = m_leftBezierHandleA;
			m_warpManagerData.m_leftCurveHandleB = m_leftBezierHandleB;
			m_warpManagerData.m_rightCurveHandleA = m_rightBezierHandleA;
			m_warpManagerData.m_rightCurveHandleB = m_rightBezierHandleB;
			m_warpManagerData.m_topCurveHandleA = m_topBezierHandleA;
			m_warpManagerData.m_topCurveHandleB = m_topBezierHandleB;

			m_warpManagerData.m_cropLeft = m_cropLeft;
			m_warpManagerData.m_cropTop = m_cropTop;
			m_warpManagerData.m_cropRight = m_cropRight;
			m_warpManagerData.m_cropBottom = m_cropBottom;
		}

		public void ForceUpdateGeometry()
		{
			UpdateGeometry ();

#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				UnityEditor.EditorUtility.SetDirty(this);
			}
#endif
		}

		public void ResetAll()
		{
			m_warpManager.ResetAll (this);

			ForceUpdateGeometry ();
		}

		public void ResetCropping()
		{
			m_warpManager.ResetCropping (this);

			ForceUpdateGeometry ();
		}

		public void ResetCornerOffsets()
		{
			m_warpManager.ResetCornerOffsets (this);

			ForceUpdateGeometry ();
		}

		public void ResetBezierHandlesToDefault()
		{
			m_warpManager.ResetBezierHandlesToDefault (this);

			ForceUpdateGeometry ();
		}

#if UNITY_EDITOR

		void OnDrawGizmosSelected()
		{
			if (m_warpManager == null)
			return;
			
			m_warpManager.OnDrawGizmos (TransformComponent, m_warpedMeshData);
		}

		[MenuItem ("Tools/Convert UI Image to UI Image Warp", false, 201)]
		static void ConvertToUIImageWarp ()
		{
			if (WarpHelper.ConvertImageToImageWarp (Selection.activeGameObject))
			{
				Debug.Log (Selection.activeGameObject.name + "'s Image component converted into a UIImageWarp component");
			}
		}

		[MenuItem ("Tools/Convert UI Image to UI Image Warp", true)]
		static bool ValidateConvertToUIImageWarp ()
		{
			if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Image> () != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
#endif
	}
}