using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fenderrio.ImageWarp
{
	[AddComponentMenu("UI/Image Warp")]
	public class NativeImageWarp : NativeTextureRenderer, IWarp
	{
		[SerializeField] private Vector3 m_cornerOffsetTL;
		private Vector3 m_cornerOffsetTLCache;
		public Vector3 cornerOffsetTL { get { return m_cornerOffsetTL; } set { if(SetStruct (ref m_cornerOffsetTL, value)) { UpdateMesh (); } } }

		[SerializeField] private Vector3 m_cornerOffsetTR;
		private Vector3 m_cornerOffsetTRCache;
		public Vector3 cornerOffsetTR { get { return m_cornerOffsetTR; } set { if(SetStruct(ref m_cornerOffsetTR, value)) { UpdateMesh (); } } }

		[SerializeField] private Vector3 m_cornerOffsetBR;
		private Vector3 m_cornerOffsetBRCache;
		public Vector3 cornerOffsetBR { get { return m_cornerOffsetBR; } set { if(SetStruct(ref m_cornerOffsetBR, value)) { UpdateMesh (); } } }

		[SerializeField] private Vector3 m_cornerOffsetBL;
		private Vector3 m_cornerOffsetBLCache;
		public Vector3 cornerOffsetBL { get { return m_cornerOffsetBL; } set { if(SetStruct(ref m_cornerOffsetBL, value)) { UpdateMesh (); } } }

		[SerializeField] private int m_numSubdivisions = 10;
		private int m_numSubdivisionsCache;
		public int numSubdivisions { get { return m_numSubdivisions; } set { if(SetStruct (ref m_numSubdivisions, value)) { UpdateMesh (); } } }

		[SerializeField] private bool m_bezierEdges = false;
		private bool m_bezierEdgesCache;
		public bool bezierEdges { get { return m_bezierEdges; } set { if(SetStruct (ref m_bezierEdges, value)) { UpdateMesh (); } } }

		[SerializeField] private Vector3 m_topBezierHandleA;
		private Vector3 m_topBezierHandleACache;
		public Vector3 topBezierHandleA { get { return m_topBezierHandleA; } set { if(SetStruct (ref m_topBezierHandleA, value)) { UpdateMesh (); } } }

		[SerializeField] private Vector3 m_topBezierHandleB;
		private Vector3 m_topBezierHandleBCache;
		public Vector3 topBezierHandleB { get { return m_topBezierHandleB; } set { if(SetStruct (ref m_topBezierHandleB, value)) { UpdateMesh (); } } }

		[SerializeField] private Vector3 m_leftBezierHandleA;
		private Vector3 m_leftBezierHandleACache;
		public Vector3 leftBezierHandleA { get { return m_leftBezierHandleA; } set { if(SetStruct (ref m_leftBezierHandleA, value)) { UpdateMesh (); } } }

		[SerializeField] private Vector3 m_leftBezierHandleB;
		private Vector3 m_leftBezierHandleBCache;
		public Vector3 leftBezierHandleB { get { return m_leftBezierHandleB; } set { if(SetStruct (ref m_leftBezierHandleB, value)) { UpdateMesh (); } } }

		[SerializeField] private Vector3 m_rightBezierHandleA;
		private Vector3 m_rightBezierHandleACache;
		public Vector3 rightBezierHandleA { get { return m_rightBezierHandleA; } set { if (SetStruct (ref m_rightBezierHandleA, value)) { UpdateMesh (); } } }
		
		[SerializeField] private Vector3 m_rightBezierHandleB;
		private Vector3 m_rightBezierHandleBCache;
		public Vector3 rightBezierHandleB { get { return m_rightBezierHandleB; } set { if(SetStruct (ref m_rightBezierHandleB, value)) { UpdateMesh (); } } }

		[SerializeField] private Vector3 m_bottomBezierHandleA;
		private Vector3 m_bottomBezierHandleACache;
		public Vector3 bottomBezierHandleA { get { return m_bottomBezierHandleA; } set { if(SetStruct (ref m_bottomBezierHandleA, value)) { UpdateMesh (); } } }

		[SerializeField] private Vector3 m_bottomBezierHandleB;
		private Vector3 m_bottomBezierHandleBCache;
		public Vector3 bottomBezierHandleB { get { return m_bottomBezierHandleB; } set { if(SetStruct (ref m_bottomBezierHandleB, value)) { UpdateMesh (); } } }

		[SerializeField] private float m_cropLeft;
		private float m_cropLeftCache;
		public float cropLeft { get { return m_cropLeft; } set { if(SetStruct(ref m_cropLeft, value)) { UpdateMesh (); } } }

		[SerializeField] private float m_cropRight;
		private float m_cropRightCache;
		public float cropRight { get { return m_cropRight; } set { if(SetStruct(ref m_cropRight, value)) { UpdateMesh (); } } }

		[SerializeField] private float m_cropTop;
		private float m_cropTopCache;
		public float cropTop { get { return m_cropTop; } set { if(SetStruct(ref m_cropTop, value)) { UpdateMesh (); } } }

		[SerializeField] private float m_cropBottom;
		private float m_cropBottomCache;
		public float cropBottom { get { return m_cropBottom; } set { if(SetStruct(ref m_cropBottom, value)) { UpdateMesh (); } } }

		[SerializeField]
		private WarpManager m_warpManager;
		public WarpManager warpManager { get { return m_warpManager; } }

		private WarpManager.WarpManagerInstanceData m_warpManagerData;
		private WarpManager.WarpManagerMeshData m_warpedMeshData;

		private Color[] m_meshColours;

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

		private void Update()
		{
			if (SetStruct<float> (ref m_cropLeftCache, m_cropLeft)) {    	UpdateMesh (); return; }
			if (SetStruct<float> (ref m_cropTopCache, m_cropTop)) {    		UpdateMesh (); return; }
			if (SetStruct<float> (ref m_cropRightCache, m_cropRight)) {    	UpdateMesh (); return; }
			if (SetStruct<float> (ref m_cropBottomCache, m_cropBottom)) {   UpdateMesh (); return; }

			if (SetStruct<Vector3> (ref m_cornerOffsetBLCache, m_cornerOffsetBL)) {    UpdateMesh (); return; }
			if (SetStruct<Vector3> (ref m_cornerOffsetBRCache, m_cornerOffsetBR)) {    UpdateMesh (); return; }
			if (SetStruct<Vector3> (ref m_cornerOffsetTLCache, m_cornerOffsetTL)) {    UpdateMesh (); return; }
			if (SetStruct<Vector3> (ref m_cornerOffsetTRCache, m_cornerOffsetTR)) {    UpdateMesh (); return; }

			if (SetStruct<int> (ref m_numSubdivisionsCache, m_numSubdivisions)) { UpdateMesh (); return; }
			if (SetStruct<bool> (ref m_bezierEdgesCache, m_bezierEdges)) { UpdateMesh (); return; }

			if (m_bezierEdges)
			{
				if (SetStruct<Vector3> (ref m_leftBezierHandleACache, m_leftBezierHandleA)) { UpdateMesh (); return; }
				if (SetStruct<Vector3> (ref m_leftBezierHandleBCache, m_leftBezierHandleB)) { UpdateMesh (); return; }
				if (SetStruct<Vector3> (ref m_topBezierHandleACache, m_topBezierHandleA)) { UpdateMesh (); return; }
				if (SetStruct<Vector3> (ref m_topBezierHandleBCache, m_topBezierHandleB)) { UpdateMesh (); return; }
				if (SetStruct<Vector3> (ref m_rightBezierHandleACache, m_rightBezierHandleA)) { UpdateMesh (); return; }
				if (SetStruct<Vector3> (ref m_rightBezierHandleBCache, m_rightBezierHandleB)) { UpdateMesh (); return; }
				if (SetStruct<Vector3> (ref m_bottomBezierHandleACache, m_bottomBezierHandleA)) { UpdateMesh (); return; }
				if (SetStruct<Vector3> (ref m_bottomBezierHandleBCache, m_bottomBezierHandleB)) { UpdateMesh (); return; }
			}
		}

		public override void UpdateMesh(bool a_changeMesh = true)
		{
			base.UpdateMesh(false);

			if (m_warpManager == null)
			{
				m_warpManager = new WarpManager ();
			}

			if (m_warpedMeshData == null)
			{
				m_warpedMeshData = new WarpManager.WarpManagerMeshData ();
			}

			PopulateWarpManagerData ();

			// Get warp mesh data
			m_warpManager.PopulateMesh (MeshVerts, m_warpManagerData, ref m_warpedMeshData);

			// Construct the colours array
			if (m_meshColours == null || m_meshColours.Length != m_warpedMeshData.m_positions.Length || m_meshColours[0].Equals(m_colour) == false)
			{
				m_meshColours = new Color[m_warpedMeshData.m_positions.Length];

				for (int idx = 0; idx < m_meshColours.Length; idx++)
				{
					m_meshColours [idx] = m_colour;
				}
			}

			// Update Mesh Values
			if (m_warpedMeshData.m_positions.Length > m_mesh.vertexCount)
			{
				m_mesh.vertices = m_warpedMeshData.m_positions;
				m_mesh.uv = m_warpedMeshData.m_uvs;
				m_mesh.triangles = m_warpedMeshData.m_indices;
			}
			else
			{
				m_mesh.triangles = m_warpedMeshData.m_indices;
				m_mesh.vertices = m_warpedMeshData.m_positions;
				m_mesh.uv = m_warpedMeshData.m_uvs;
			}

			m_mesh.colors = m_meshColours;

			if (!m_initialised)
			{
				SetupBezierHandlesInResetPose ();

				m_initialised = true;
			}

			// Update Cached values
			SetStruct<float> (ref m_cropLeftCache, m_cropLeft);
			SetStruct<float> (ref m_cropTopCache, m_cropTop);
			SetStruct<float> (ref m_cropRightCache, m_cropRight);
			SetStruct<float> (ref m_cropBottomCache, m_cropBottom);

			SetStruct<Vector3> (ref m_cornerOffsetBLCache, m_cornerOffsetBL);
			SetStruct<Vector3> (ref m_cornerOffsetBRCache, m_cornerOffsetBR);
			SetStruct<Vector3> (ref m_cornerOffsetTLCache, m_cornerOffsetTL);
			SetStruct<Vector3> (ref m_cornerOffsetTRCache, m_cornerOffsetTR);

			SetStruct<int> (ref m_numSubdivisionsCache, m_numSubdivisions);
			SetStruct<bool> (ref m_bezierEdgesCache, m_bezierEdges);

			if(m_bezierEdges)
			{
			    SetStruct<Vector3> (ref m_leftBezierHandleACache, m_leftBezierHandleA);
			    SetStruct<Vector3> (ref m_leftBezierHandleBCache, m_leftBezierHandleB);
			    SetStruct<Vector3> (ref m_topBezierHandleACache, m_topBezierHandleA);
			    SetStruct<Vector3> (ref m_topBezierHandleBCache, m_topBezierHandleB);
			    SetStruct<Vector3> (ref m_rightBezierHandleACache, m_rightBezierHandleA);
			    SetStruct<Vector3> (ref m_rightBezierHandleBCache, m_rightBezierHandleB);
			    SetStruct<Vector3> (ref m_bottomBezierHandleACache, m_bottomBezierHandleA);
			    SetStruct<Vector3> (ref m_bottomBezierHandleBCache, m_bottomBezierHandleB);
			}
		}

		private void PopulateWarpManagerData()
		{
			if (m_warpManagerData == null)
			{
				m_warpManagerData = new WarpManager.WarpManagerInstanceData ();
			}

			m_warpManagerData.m_flipX = m_flipX;
			m_warpManagerData.m_flipY = m_flipY;

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

		private void SetupBezierHandlesInResetPose()
		{
			m_topBezierHandleA = new Vector3(m_warpManager.Width / 3f,0,0);
			m_topBezierHandleB = new Vector3(-m_warpManager.Width / 3f,0,0);

			m_bottomBezierHandleA = new Vector3(-m_warpManager.Width / 3f,0,0);
			m_bottomBezierHandleB = new Vector3(m_warpManager.Width / 3f,0,0);

			m_leftBezierHandleA = new Vector3(0,m_warpManager.Height / 3f,0);
			m_leftBezierHandleB = new Vector3(0,-m_warpManager.Height / 3f,0);

			m_rightBezierHandleA = new Vector3(0,-m_warpManager.Height / 3f,0);
			m_rightBezierHandleB = new Vector3(0,m_warpManager.Height / 3f,0);
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
//			if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Image> () != null)
//			{
//				return true;
//			}
//			else
//			{
				return false;
//			}
		}
#endif
	}
}