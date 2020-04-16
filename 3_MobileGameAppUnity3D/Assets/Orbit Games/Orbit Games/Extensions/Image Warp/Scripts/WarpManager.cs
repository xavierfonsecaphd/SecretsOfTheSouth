using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fenderrio.ImageWarp
{
	[System.Serializable]
	public class WarpManager
	{
		public class WarpManagerInstanceData
		{
			public bool m_flipX = false;
			public bool m_flipY = false;

			public Vector3 m_cornerOffsetBL;
			public Vector3 m_cornerOffsetTL;
			public Vector3 m_cornerOffsetTR;
			public Vector3 m_cornerOffsetBR;
			public int m_numSubdivisions;
			public bool m_bezierEdges = false;
			public Vector3 m_topCurveHandleA;
			public Vector3 m_topCurveHandleB;
			public Vector3 m_leftCurveHandleA;
			public Vector3 m_leftCurveHandleB;
			public Vector3 m_rightCurveHandleA;
			public Vector3 m_rightCurveHandleB;
			public Vector3 m_bottomCurveHandleA;
			public Vector3 m_bottomCurveHandleB;

			public float m_cropLeft;
			public float m_cropTop;
			public float m_cropRight;
			public float m_cropBottom;
		}

		public class WarpManagerMeshData
		{
			public Vector3[] m_positions;
			public Vector2[] m_uvs;
			public int[] m_indices;

			public WarpManagerMeshData()
			{
				m_positions = new Vector3[0];
				m_uvs = new Vector2[0];
				m_indices = new int[0];
			}
		}

#if UNITY_EDITOR

		[MenuItem ("Tools/Fix legacy v1.1 bezier curved edges", false, 203)]
		static void FixLegacyBezierData ()
		{
			if (WarpHelper.FixLegacyBezierData (Selection.activeGameObject))
			{
				Debug.Log ("Backwards Compatibility pass applied to \"" + Selection.activeGameObject.name + "\"'s ImageWarp component");
			}
		}

		[MenuItem ("Tools/Fix legacy v1.1 bezier curved edges", true)]
		static bool ValidateFixLegacyBezierData ()
		{
			if (Selection.activeGameObject && Selection.activeGameObject.GetComponent (typeof(IWarp)) != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private static readonly int MAX_NUM_SUBDIVISIONS = 30;
#endif
		private static readonly Vector2[] DEFAULT_UV_VEC2S = new Vector2[] { new Vector2(0,0), new Vector2(0,1), new Vector2(1,1), new Vector2(1,0) };

		private Vector2 m_xUvDiffVec = new Vector2(1,0);
		private Vector2 m_yUvDiffVec = new Vector2(0,1);

		private Vector3 m_cornerPositionBL;
		private Vector3 m_cornerPositionTL;
		private Vector3 m_cornerPositionTR;
		private Vector3 m_cornerPositionBR;

		[SerializeField]
		private float m_width;
		[SerializeField]
		private float m_height;

		[SerializeField]
		private float m_preWarpWidth;
		[SerializeField]
		private float m_preWarpHeight;

		private int m_vertRowLength;
		private int m_numVertices;
		private bool m_bezierEdges = false;
		private int m_vertIndex = 0;
		private int m_quadIndex = 0;
		private float m_progressX = 0;
		private float m_progressY = 0;
		private Vector3 m_leftPoint, m_rightPoint;

		private BezierCurve m_topCurve;
		private BezierCurve m_leftCurve;
		private BezierCurve m_rightCurve;
		private BezierCurve m_bottomCurve;

		private Vector3[] m_topCurveVertOffsets;
		private Vector3[] m_leftCurveVertOffsets;
		private Vector3[] m_rightCurveVertOffsets;
		private Vector3[] m_bottomCurveVertOffsets;

		private Vector2[] m_tempMeshUvs = new Vector2[4];

#if UNITY_EDITOR
		// Editor script variables
		private Vector3 m_currentScale;
		private Vector3 m_cornerOffset;
		private Vector3 m_imagePosition;
		private Vector3 m_newHandlePosition;
		private Vector3 m_preWarpCornerPosition;
		private SerializedProperty m_memberProperty;
		private bool m_preGuiChanged;
		private bool m_guiChanged;
		private Vector3 m_topLeftCornerPosition, m_topRightCornerPosition, m_bottomRightCornerPosition, m_bottomLeftCornerPosition;
		private Vector3 m_handlePos;
		private Vector3 m_lineFrom;
		private Vector3 m_lineTo;
		private Vector3 m_topEdgeCropHandleAnchor;
		private Vector3 m_bottomEdgeCropHandleAnchor;
		private Vector3 m_leftEdgeCropHandleAnchor;
		private Vector3 m_rightEdgeCropHandleAnchor;

		private SerializedProperty m_topEdgeCropProperty;
		private SerializedProperty m_rightEdgeCropProperty;
		private SerializedProperty m_leftEdgeCropProperty;
		private SerializedProperty m_bottomEdgeCropProperty;

		private bool m_cropHandleMoveInitialised = false;
		private Vector3 m_cropDragStartHandlePosition;
		private Vector3 m_cropDragOppositeHandlePosition;
		private float m_cropDragStartDistanceFromOppositeHandle;
		private float m_cropDragStartCropValue;
		private bool m_dragDirectionFlipped = false;
		private float m_currentDistanceFromOppositeHandle;
		private float m_currentDistanceFromOriginalStartHandle;

		private Vector3 m_topEdgeCropHandlePosition;
		private Vector3 m_bottomEdgeCropHandlePosition;
		private Vector3 m_leftEdgeCropHandlePosition;
		private Vector3 m_rightEdgeCropHandlePosition;

		private const float BEZIER_HANDLE_SCALE = 0.05f;

		private GUIStyle m_titleTextGUIStyle;
		protected GUIStyle TitleTextGUIStyle { get {

				if (m_titleTextGUIStyle == null)
				{
					m_titleTextGUIStyle = new GUIStyle (EditorStyles.boldLabel);
				}
				m_titleTextGUIStyle.fontSize = 18;
				return m_titleTextGUIStyle;
			}
		}

		private GUIStyle m_subTitleTextGUIStyle;
		protected GUIStyle SubTitleTextGUIStyle { get {

				if (m_subTitleTextGUIStyle == null)
				{
					m_subTitleTextGUIStyle = new GUIStyle (EditorStyles.boldLabel);
				}
				m_subTitleTextGUIStyle.fontSize = 12;
				m_subTitleTextGUIStyle.fontStyle = FontStyle.BoldAndItalic;
				return m_subTitleTextGUIStyle;
			}
		}
#endif

		public float Width { get { return m_width; } }
		public float Height { get { return m_height; } }

		private bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (currentValue.Equals(newValue))
				return false;

			currentValue = newValue;
			return true;
		}

		public void PopulateMesh(Vector3[] a_meshVerts, WarpManagerInstanceData a_instanceData, ref WarpManagerMeshData a_meshData)
		{
			DEFAULT_UV_VEC2S.CopyTo (m_tempMeshUvs, 0);

			PopulateMesh (a_meshVerts, m_tempMeshUvs, a_instanceData, ref a_meshData);
		}

		public void PopulateMesh(Vector3[] a_meshVerts, Vector2[] a_meshUvs, WarpManagerInstanceData a_instanceData, ref WarpManagerMeshData a_meshData)
		{
			// Offset UV data with cropping offset values
			a_meshUvs [0].x += a_instanceData.m_cropLeft;
			a_meshUvs [1].x += a_instanceData.m_cropLeft;
			a_meshUvs [2].x -= a_instanceData.m_cropRight;
			a_meshUvs [3].x -= a_instanceData.m_cropRight;

			a_meshUvs [0].y += a_instanceData.m_cropBottom;
			a_meshUvs [3].y += a_instanceData.m_cropBottom;
			a_meshUvs [1].y -= a_instanceData.m_cropTop;
			a_meshUvs [2].y -= a_instanceData.m_cropTop;

			// Calculate UV offsets
			m_xUvDiffVec = a_meshUvs [3] - a_meshUvs [0];
			m_yUvDiffVec = a_meshUvs [1] - a_meshUvs [0];



			m_cornerPositionBL = a_meshVerts [0];
			m_cornerPositionTL = a_meshVerts [1];
			m_cornerPositionTR = a_meshVerts [2];
			m_cornerPositionBR = a_meshVerts [3];

			// Calculate pre warp width/height
			m_preWarpWidth = m_cornerPositionBR.x - m_cornerPositionBL.x;
			m_preWarpHeight = m_cornerPositionTL.y - m_cornerPositionBL.y;


			// offset the corner positions based on the cropping amounts
			if (a_instanceData.m_cropLeft != 0 || a_instanceData.m_cropRight != 0)
			{
				Vector3 bottomEdge = m_cornerPositionBR - m_cornerPositionBL;
				Vector3 topEdge = m_cornerPositionTR - m_cornerPositionTL;

				// calculate horizontal cropping first 
				m_cornerPositionBL += a_instanceData.m_cropLeft * bottomEdge;
				m_cornerPositionTL += a_instanceData.m_cropLeft * topEdge;
				m_cornerPositionTR -= a_instanceData.m_cropRight * topEdge;
				m_cornerPositionBR -= a_instanceData.m_cropRight * bottomEdge;
			}

			if (a_instanceData.m_cropBottom != 0 || a_instanceData.m_cropTop != 0)
			{
				Vector3 leftEdge = m_cornerPositionTL - m_cornerPositionBL;
				Vector3 rightEdge = m_cornerPositionTR - m_cornerPositionBR;

				// then vertical
				m_cornerPositionBL += a_instanceData.m_cropBottom * (leftEdge);
				m_cornerPositionTL -= a_instanceData.m_cropTop * (leftEdge);
				m_cornerPositionTR -= a_instanceData.m_cropTop * (rightEdge);
				m_cornerPositionBR += a_instanceData.m_cropBottom * (rightEdge);
			}


			// Calculate width/height
			m_width = m_cornerPositionBR.x - m_cornerPositionBL.x;
			m_height = m_cornerPositionTL.y - m_cornerPositionBL.y;


			// Calculate current corner positions
			m_cornerPositionBL += a_instanceData.m_cornerOffsetBL;
			m_cornerPositionTL += a_instanceData.m_cornerOffsetTL;
			m_cornerPositionTR += a_instanceData.m_cornerOffsetTR;
			m_cornerPositionBR += a_instanceData.m_cornerOffsetBR;



			if (a_instanceData.m_numSubdivisions < 1)
				a_instanceData.m_numSubdivisions = 1;

			m_vertRowLength = a_instanceData.m_numSubdivisions + 1;
			m_numVertices = m_vertRowLength * m_vertRowLength;


			if (a_meshData.m_positions.Length != m_numVertices)
			{
				a_meshData.m_positions = new Vector3[m_numVertices];
				a_meshData.m_uvs = new Vector2[m_numVertices];
			}
			if (a_meshData.m_indices.Length != (a_instanceData.m_numSubdivisions * a_instanceData.m_numSubdivisions) * 6)
			{
				a_meshData.m_indices = new int[(a_instanceData.m_numSubdivisions * a_instanceData.m_numSubdivisions) * 6];
			}

			m_vertIndex = 0;
			m_quadIndex = 0;

			m_bezierEdges = a_instanceData.m_bezierEdges;

			if (m_bezierEdges)
			{
				if (m_topCurve == null || m_topCurve.m_pointsData.Length != 2)
				{
					// Initialise the bezier curves
					InitialiseBezierCurves();
				}

				// Set bezier curve anchors and handles to mimick editor Handles positions
				m_topCurve.m_pointsData [0].AnchorPoint = m_cornerPositionTL;
				m_topCurve.m_pointsData [0].HandlePointA = m_cornerPositionTL + a_instanceData.m_topCurveHandleA;
				m_topCurve.m_pointsData [1].AnchorPoint = m_cornerPositionTR;
				m_topCurve.m_pointsData [1].HandlePointA = m_cornerPositionTR + a_instanceData.m_topCurveHandleB;

				m_leftCurve.m_pointsData [0].AnchorPoint = m_cornerPositionBL;
				m_leftCurve.m_pointsData [0].HandlePointA = m_cornerPositionBL + a_instanceData.m_leftCurveHandleA;
				m_leftCurve.m_pointsData [1].AnchorPoint = m_cornerPositionTL;
				m_leftCurve.m_pointsData [1].HandlePointA = m_cornerPositionTL + a_instanceData.m_leftCurveHandleB;

				m_rightCurve.m_pointsData [0].AnchorPoint = m_cornerPositionTR;
				m_rightCurve.m_pointsData [0].HandlePointA = m_cornerPositionTR + a_instanceData.m_rightCurveHandleA;
				m_rightCurve.m_pointsData [1].AnchorPoint = m_cornerPositionBR;
				m_rightCurve.m_pointsData [1].HandlePointA = m_cornerPositionBR + a_instanceData.m_rightCurveHandleB;

				m_bottomCurve.m_pointsData [0].AnchorPoint = m_cornerPositionBR;
				m_bottomCurve.m_pointsData [0].HandlePointA = m_cornerPositionBR + a_instanceData.m_bottomCurveHandleA;
				m_bottomCurve.m_pointsData [1].AnchorPoint = m_cornerPositionBL;
				m_bottomCurve.m_pointsData [1].HandlePointA = m_cornerPositionBL + a_instanceData.m_bottomCurveHandleB;

				// Calculate offset positions for each vert point on each curve
				if (m_topCurveVertOffsets == null || m_topCurveVertOffsets.Length != m_vertRowLength)
				{
					m_topCurveVertOffsets = new Vector3[m_vertRowLength];
					m_leftCurveVertOffsets = new Vector3[m_vertRowLength];
					m_rightCurveVertOffsets = new Vector3[m_vertRowLength];
					m_bottomCurveVertOffsets = new Vector3[m_vertRowLength];
				}

				for (int idx = 0; idx < m_vertRowLength; idx++)
				{
					if (idx == 0)
					{
						m_leftCurveVertOffsets [0] = m_leftCurve.m_pointsData [0].AnchorPoint;
						m_rightCurveVertOffsets [0] = m_rightCurve.m_pointsData [0].AnchorPoint;

						m_topCurveVertOffsets [0] = Vector3.zero; //m_topCurve.m_pointsData [0].AnchorPoint;
						m_bottomCurveVertOffsets [0] = Vector3.zero; //m_bottomCurve.m_pointsData [0].AnchorPoint;
					}
					else if (idx == m_vertRowLength - 1)
					{
						m_leftCurveVertOffsets [idx] = m_leftCurve.m_pointsData [1].AnchorPoint;
						m_rightCurveVertOffsets [idx] = m_rightCurve.m_pointsData [1].AnchorPoint;

						m_topCurveVertOffsets [idx] = Vector3.zero; //m_topCurve.m_pointsData [1].AnchorPoint;
						m_bottomCurveVertOffsets [idx] = Vector3.zero; //m_bottomCurve.m_pointsData [1].AnchorPoint;
					}
					else
					{
						m_progressX = idx / (float)(m_vertRowLength - 1);

						// Work out exact position of point on curve
						m_leftCurveVertOffsets [idx] = m_leftCurve.GetCurvePoint(m_progressX);
						m_rightCurveVertOffsets [idx] = m_rightCurve.GetCurvePoint(m_progressX);

						// Work out offset of curve point from non-curved point
						m_topCurveVertOffsets [idx] = m_topCurve.GetCurvePoint(m_progressX) - (m_cornerPositionTL + m_progressX * (m_cornerPositionTR - m_cornerPositionTL));
						m_bottomCurveVertOffsets [idx] = m_bottomCurve.GetCurvePoint(m_progressX) - (m_cornerPositionBR + m_progressX * (m_cornerPositionBL - m_cornerPositionBR));
					}
				}
			}




			// Step through subdivision verts and calculate positions and uvs
			for(int y=0; y < m_vertRowLength; y++)
			{
				m_progressY = y / (float)(m_vertRowLength - 1);

				if (m_bezierEdges)
				{
					m_leftPoint = m_leftCurveVertOffsets[y];
					m_rightPoint = m_rightCurveVertOffsets[m_vertRowLength - 1 - y];
				}
				else
				{
					m_leftPoint = m_cornerPositionBL + (m_cornerPositionTL - m_cornerPositionBL) * m_progressY;
					m_rightPoint = m_cornerPositionBR + (m_cornerPositionTR - m_cornerPositionBR) * m_progressY;
				}


				for (int x = 0; x < m_vertRowLength; x++)
				{
					m_vertIndex = (y * m_vertRowLength) + x;
					m_progressX = x / (float)(m_vertRowLength - 1);

					a_meshData.m_positions[m_vertIndex] = m_leftPoint + (m_progressX * (m_rightPoint - m_leftPoint));

					if (m_bezierEdges)
					{
						// Add on the top/bottom curve position offset
						a_meshData.m_positions[m_vertIndex] += m_bottomCurveVertOffsets[m_vertRowLength -1 -x] + m_progressY * ( m_topCurveVertOffsets[x] - m_bottomCurveVertOffsets[m_vertRowLength -1 -x]);
					}

					a_meshData.m_uvs[m_vertIndex] = a_meshUvs[0];

					if(a_instanceData.m_flipX)
						a_meshData.m_uvs[m_vertIndex] += (m_xUvDiffVec * (1 - m_progressX));
					else
						a_meshData.m_uvs[m_vertIndex] += (m_xUvDiffVec * m_progressX);

					if(a_instanceData.m_flipY)
						a_meshData.m_uvs[m_vertIndex] += (m_yUvDiffVec * (1 - m_progressY));
					else
						a_meshData.m_uvs[m_vertIndex] += (m_yUvDiffVec * m_progressY);

					if(x != m_vertRowLength - 1 && y != m_vertRowLength - 1)
					{
						a_meshData.m_indices[m_quadIndex * 6 + 0] = m_vertIndex;
						a_meshData.m_indices[m_quadIndex * 6 + 1] = m_vertIndex + m_vertRowLength;
						a_meshData.m_indices[m_quadIndex * 6 + 2] = m_vertIndex + m_vertRowLength + 1;

						a_meshData.m_indices[m_quadIndex * 6 + 3] = m_vertIndex;
						a_meshData.m_indices[m_quadIndex * 6 + 4] = m_vertIndex + m_vertRowLength + 1;
						a_meshData.m_indices[m_quadIndex * 6 + 5] = m_vertIndex + 1;

						m_quadIndex++;
					}
				}
			}
		}


		private void InitialiseBezierCurves()
		{
			m_topCurve = new BezierCurve (){m_pointsData = new BezierCurve.BezierCurvePoint[]{new BezierCurve.BezierCurvePoint(), new BezierCurve.BezierCurvePoint()}};
			m_leftCurve = new BezierCurve (){m_pointsData = new BezierCurve.BezierCurvePoint[]{new BezierCurve.BezierCurvePoint(), new BezierCurve.BezierCurvePoint()}};
			m_rightCurve = new BezierCurve (){m_pointsData = new BezierCurve.BezierCurvePoint[]{new BezierCurve.BezierCurvePoint(), new BezierCurve.BezierCurvePoint()}};
			m_bottomCurve = new BezierCurve (){m_pointsData = new BezierCurve.BezierCurvePoint[]{new BezierCurve.BezierCurvePoint(), new BezierCurve.BezierCurvePoint()}};
		}

		public void ResetAll(IWarp a_warp)
		{
			ResetCropping (a_warp);

			ResetCornerOffsets (a_warp);

			ResetBezierHandlesToDefault (a_warp);
		}

		public void ResetCropping(IWarp a_warp)
		{
			a_warp.cropLeft = 0;
			a_warp.cropTop = 0;
			a_warp.cropRight = 0;
			a_warp.cropBottom = 0;
		}

		public void ResetCornerOffsets(IWarp a_warp)
		{
			a_warp.cornerOffsetBL = Vector3.zero;
			a_warp.cornerOffsetTL = Vector3.zero;
			a_warp.cornerOffsetTR = Vector3.zero;
			a_warp.cornerOffsetBR = Vector3.zero;
		}

		public void ResetBezierHandlesToDefault(IWarp a_warp)
		{
			float croppedWidth = m_preWarpWidth * (1 - a_warp.cropLeft - a_warp.cropRight);
			float croppedHeight = m_preWarpHeight * (1 - a_warp.cropTop - a_warp.cropBottom);

			a_warp.topBezierHandleA = new Vector3(croppedWidth / 3f, 0,0);
			a_warp.topBezierHandleB = new Vector3(-croppedWidth / 3f, 0,0);

			a_warp.leftBezierHandleA = new Vector3(0, croppedHeight / 3f,0);
			a_warp.leftBezierHandleB = new Vector3(0, -croppedHeight / 3f,0);

			a_warp.rightBezierHandleA = new Vector3(0, -croppedHeight / 3f,0);
			a_warp.rightBezierHandleB = new Vector3(0, croppedHeight / 3f,0);

			a_warp.bottomBezierHandleA = new Vector3(-croppedWidth / 3f, 0,0);
			a_warp.bottomBezierHandleB = new Vector3(croppedWidth / 3f, 0,0);
		}

#if UNITY_EDITOR

		public void OnDrawGizmos(Transform a_transformComponent, WarpManagerMeshData a_meshData)
		{
			if (a_meshData == null || a_meshData.m_positions == null)
			{
				return;
			}

			m_lineFrom = Vector3.zero;
			m_lineTo = Vector3.zero;
			m_currentScale = a_transformComponent.lossyScale;

			Gizmos.color = new Color (1, 1, 1, 0.2f);

			if (!m_bezierEdges)
			{
				// Draw non-curved mesh grid
				for (int idx = 0; idx < m_vertRowLength; idx++)
				{
					m_lineFrom = a_transformComponent.position + a_transformComponent.rotation * Vector3.Scale (m_currentScale, a_meshData.m_positions [idx]);
					m_lineTo = a_transformComponent.position + a_transformComponent.rotation * Vector3.Scale (m_currentScale, a_meshData.m_positions [(a_meshData.m_positions.Length - m_vertRowLength) + idx]);

					Gizmos.DrawLine (m_lineFrom, m_lineTo);

					m_lineFrom = a_transformComponent.position + a_transformComponent.rotation * Vector3.Scale (m_currentScale, a_meshData.m_positions [idx * m_vertRowLength]);
					m_lineTo = a_transformComponent.position + a_transformComponent.rotation * Vector3.Scale (m_currentScale, a_meshData.m_positions [((idx + 1) * m_vertRowLength) - 1]);

					Gizmos.DrawLine (m_lineFrom, m_lineTo);
				}
			}
			else
			{
				// Draw warped mesh grid
				for (int x = 0; x < m_vertRowLength; x++)
				{
					for (int y = 0; y < m_vertRowLength; y++)
					{
						if (x > 0)
						{
							// Draw horizontal lines between verts
							m_lineFrom = a_transformComponent.position + a_transformComponent.rotation * Vector3.Scale( m_currentScale, a_meshData.m_positions [x + (y*m_vertRowLength)]);
							m_lineTo = a_transformComponent.position + a_transformComponent.rotation * Vector3.Scale( m_currentScale, a_meshData.m_positions [(x -1) + (y*m_vertRowLength)]);
							Gizmos.DrawLine (m_lineFrom, m_lineTo);
						}

						if (y > 0)
						{
							// Draw vertical lines
							m_lineFrom = a_transformComponent.position + a_transformComponent.rotation * Vector3.Scale( m_currentScale, a_meshData.m_positions [x + (y * m_vertRowLength)]);
							m_lineTo = a_transformComponent.position + a_transformComponent.rotation * Vector3.Scale( m_currentScale, a_meshData.m_positions [x + ((y - 1) * m_vertRowLength)]);
							Gizmos.DrawLine (m_lineFrom, m_lineTo);
						}
					}
				}
			}


		}

		public bool OnInspectorGUI (SerializedObject serializedObject)
		{
			m_guiChanged = GUI.changed;

			GUILayout.Space (15);

			EditorGUILayout.BeginVertical (EditorStyles.helpBox);

			GUILayout.Label ("Warp Settings", TitleTextGUIStyle);

			GUILayout.Space (10);

			m_memberProperty = serializedObject.FindProperty ("m_numSubdivisions");
			EditorGUILayout.PropertyField (m_memberProperty, new GUIContent("Mesh Subdivisions"));

			if (!m_guiChanged && GUI.changed)
			{
				if (m_memberProperty.intValue < 1)
					m_memberProperty.intValue = 1;
				else if (m_memberProperty.intValue > MAX_NUM_SUBDIVISIONS)
					m_memberProperty.intValue = MAX_NUM_SUBDIVISIONS;
			}

			GUILayout.Space (10);

			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("Cropping", SubTitleTextGUIStyle, GUILayout.Height(20));

			GUILayout.FlexibleSpace ();

			if (GUILayout.Button ("Reset", GUILayout.Width(60)))
			{
				ResetCropping (serializedObject);
			}

			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.PropertyField ( serializedObject.FindProperty ("m_cropLeft"), new GUIContent("Left"));
			EditorGUILayout.PropertyField ( serializedObject.FindProperty ("m_cropRight"), new GUIContent("Right") );
			EditorGUILayout.PropertyField ( serializedObject.FindProperty ("m_cropTop"), new GUIContent("Top") );
			EditorGUILayout.PropertyField ( serializedObject.FindProperty ("m_cropBottom"), new GUIContent("Bottom") );

			GUILayout.Space (10);

			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("Corner Offsets", SubTitleTextGUIStyle, GUILayout.Height(20));

			GUILayout.FlexibleSpace ();

			if (GUILayout.Button ("Reset", GUILayout.Width(60)))
			{
				ResetCornerOffsets (serializedObject);
			}

			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.PropertyField (serializedObject.FindProperty ("m_cornerOffsetTL"), new GUIContent("Top Left"));
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("m_cornerOffsetTR"), new GUIContent("Top Right"));
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("m_cornerOffsetBR"), new GUIContent("Bottom Right"));
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("m_cornerOffsetBL"), new GUIContent("Bottom Left"));

			GUILayout.Space (15);

			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("Bezier Edges", SubTitleTextGUIStyle, GUILayout.Height(20));

			GUILayout.FlexibleSpace ();
			
			if (GUILayout.Button ("Reset", GUILayout.Width(60)))
			{
				ResetBezierHandles (serializedObject);
			}
			
			EditorGUILayout.EndHorizontal ();


			EditorGUILayout.PropertyField (serializedObject.FindProperty ("m_bezierEdges"), new GUIContent("Enabled?"));

			if (serializedObject.FindProperty ("m_bezierEdges").boolValue)
			{
				m_memberProperty = serializedObject.FindProperty ("m_topBezierHandleA");
				EditorGUILayout.PropertyField (m_memberProperty);

				m_memberProperty = serializedObject.FindProperty ("m_topBezierHandleB");
				EditorGUILayout.PropertyField (m_memberProperty);

				m_memberProperty = serializedObject.FindProperty ("m_leftBezierHandleA");
				EditorGUILayout.PropertyField (m_memberProperty);

				m_memberProperty = serializedObject.FindProperty ("m_leftBezierHandleB");
				EditorGUILayout.PropertyField (m_memberProperty);

				m_memberProperty = serializedObject.FindProperty ("m_rightBezierHandleA");
				EditorGUILayout.PropertyField (m_memberProperty);

				m_memberProperty = serializedObject.FindProperty ("m_rightBezierHandleB");
				EditorGUILayout.PropertyField (m_memberProperty);

				m_memberProperty = serializedObject.FindProperty ("m_bottomBezierHandleA");
				EditorGUILayout.PropertyField (m_memberProperty);

				m_memberProperty = serializedObject.FindProperty ("m_bottomBezierHandleB");
				EditorGUILayout.PropertyField (m_memberProperty);
			}

			GUILayout.Space (15);

			if (GUILayout.Button ("Reset All"))
			{
				ResetCropping (serializedObject);

				ResetCornerOffsets (serializedObject);

				ResetBezierHandles (serializedObject);
			}

			EditorGUILayout.EndVertical ();

			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties ();

				return true;
			}

			return false;
		}

		private void ResetCropping(SerializedObject serializedObject)
		{
			serializedObject.FindProperty ("m_cropLeft").floatValue = 0;
			serializedObject.FindProperty ("m_cropTop").floatValue = 0;
			serializedObject.FindProperty ("m_cropRight").floatValue = 0;
			serializedObject.FindProperty ("m_cropBottom").floatValue = 0;
		}

		private void ResetCornerOffsets(SerializedObject serializedObject)
		{
			serializedObject.FindProperty ("m_cornerOffsetTR").vector3Value = Vector3.zero;
			serializedObject.FindProperty ("m_cornerOffsetBL").vector3Value = Vector3.zero;
			serializedObject.FindProperty ("m_cornerOffsetBR").vector3Value = Vector3.zero;
			serializedObject.FindProperty ("m_cornerOffsetTL").vector3Value = Vector3.zero;
		}

		private void ResetBezierHandles(SerializedObject serialisedObject)
		{
			float leftCrop = serialisedObject.FindProperty("m_cropLeft").floatValue;
			float rightCrop = serialisedObject.FindProperty ("m_cropRight").floatValue;
			float topCrop = serialisedObject.FindProperty("m_cropTop").floatValue;
			float bottomCrop = serialisedObject.FindProperty("m_cropBottom").floatValue;

			float croppedWidth = m_preWarpWidth * (1 - leftCrop - rightCrop);
			float croppedHeight = m_preWarpHeight * (1 - topCrop - bottomCrop);

			serialisedObject.FindProperty("m_topBezierHandleA").vector3Value = new Vector3(croppedWidth / 3f, 0,0);
			serialisedObject.FindProperty("m_topBezierHandleB").vector3Value = new Vector3(-croppedWidth / 3f, 0,0);

			serialisedObject.FindProperty("m_leftBezierHandleA").vector3Value = new Vector3(0,croppedHeight / 3f,0);
			serialisedObject.FindProperty("m_leftBezierHandleB").vector3Value = new Vector3(0,-croppedHeight / 3f,0);

			serialisedObject.FindProperty("m_rightBezierHandleA").vector3Value = new Vector3(0,-croppedHeight / 3f,0);
			serialisedObject.FindProperty("m_rightBezierHandleB").vector3Value = new Vector3(0,croppedHeight / 3f,0);

			serialisedObject.FindProperty("m_bottomBezierHandleA").vector3Value = new Vector3(-croppedWidth / 3f, 0,0);
			serialisedObject.FindProperty("m_bottomBezierHandleB").vector3Value = new Vector3(croppedWidth / 3f, 0,0);
		}

		public bool OnSceneGUI(SerializedObject serializedObject, Transform imageWarpTransform, Vector2 dimensions)
		{
			return OnSceneGUI (serializedObject, imageWarpTransform, dimensions, null);
		}

		public bool OnSceneGUI(SerializedObject serializedObject, Transform imageWarpTransform, Vector2 dimensions, Vector2? pivotPoint)
		{
			m_currentScale = imageWarpTransform.lossyScale;
			m_cornerOffset = new Vector3 (dimensions.x / 2f, dimensions.y / 2f);
			m_imagePosition = imageWarpTransform.position;

			if (pivotPoint != null)
			{
				// Take into account the pivot point offset
				m_imagePosition += imageWarpTransform.rotation *
								new Vector3 (	(0.5f - ((Vector2)pivotPoint).x) * m_currentScale.x * dimensions.x, 
												(0.5f - ((Vector2)pivotPoint).y) * m_currentScale.y * dimensions.y,
												0);
			}

			m_cornerOffset.Scale (m_currentScale);


			m_preGuiChanged = GUI.changed;



			// Calculate the normal corner positions
			m_cornerPositionTR = m_imagePosition + (imageWarpTransform.rotation * m_cornerOffset);
			m_cornerPositionBL = m_imagePosition - (imageWarpTransform.rotation * m_cornerOffset);
			m_cornerPositionBR = m_imagePosition + (imageWarpTransform.rotation * (new Vector3(m_cornerOffset.x, -m_cornerOffset.y)));
			m_cornerPositionTL = m_imagePosition + (imageWarpTransform.rotation * (new Vector3(-m_cornerOffset.x, m_cornerOffset.y)));


			// Calculate the cropped corner positions
			m_topEdgeCropProperty = serializedObject.FindProperty ("m_cropTop");
			m_rightEdgeCropProperty = serializedObject.FindProperty ("m_cropRight");
			m_leftEdgeCropProperty = serializedObject.FindProperty ("m_cropLeft");
			m_bottomEdgeCropProperty = serializedObject.FindProperty ("m_cropBottom");

			CropCorners(ref m_cornerPositionTL, ref m_cornerPositionTR, ref m_cornerPositionBR, ref m_cornerPositionBL, m_leftEdgeCropProperty.floatValue, m_topEdgeCropProperty.floatValue, m_rightEdgeCropProperty.floatValue, m_bottomEdgeCropProperty.floatValue);


			m_guiChanged = false;

			// top right
			m_guiChanged = GUI.changed;
			m_memberProperty = serializedObject.FindProperty ("m_cornerOffsetTR");
			m_newHandlePosition = m_topRightCornerPosition = Handles.PositionHandle (m_cornerPositionTR + (imageWarpTransform.rotation * Vector3.Scale (m_memberProperty.vector3Value, m_currentScale)), imageWarpTransform.rotation);
			if (!m_guiChanged && GUI.changed)
			{
				m_newHandlePosition -= m_cornerPositionTR;
				m_memberProperty.vector3Value = Quaternion.Inverse (imageWarpTransform.rotation) * (new Vector3 (m_newHandlePosition.x / m_currentScale.x, m_newHandlePosition.y / m_currentScale.y, m_newHandlePosition.z / m_currentScale.z));
			}

			// bottom left
			m_guiChanged = GUI.changed;
			m_memberProperty = serializedObject.FindProperty ("m_cornerOffsetBL");
			m_newHandlePosition = m_bottomLeftCornerPosition = Handles.PositionHandle ( m_cornerPositionBL + (imageWarpTransform.rotation * Vector3.Scale( m_memberProperty.vector3Value, m_currentScale )), imageWarpTransform.rotation);
			if (!m_guiChanged && GUI.changed)
			{
				m_newHandlePosition -= m_cornerPositionBL;
				m_memberProperty.vector3Value = Quaternion.Inverse (imageWarpTransform.rotation) * (new Vector3 (m_newHandlePosition.x / m_currentScale.x, m_newHandlePosition.y / m_currentScale.y, m_newHandlePosition.z / m_currentScale.z));
			}

			// bottom right
			m_guiChanged = GUI.changed;
			m_memberProperty = serializedObject.FindProperty ("m_cornerOffsetBR");
			m_newHandlePosition = m_bottomRightCornerPosition = Handles.PositionHandle (m_cornerPositionBR + (imageWarpTransform.rotation * Vector3.Scale( m_memberProperty.vector3Value, m_currentScale )), imageWarpTransform.rotation);
			if (!m_guiChanged && GUI.changed)
			{
				m_newHandlePosition -= m_cornerPositionBR;
				m_memberProperty.vector3Value = Quaternion.Inverse (imageWarpTransform.rotation) * (new Vector3 (m_newHandlePosition.x / m_currentScale.x, m_newHandlePosition.y / m_currentScale.y, m_newHandlePosition.z / m_currentScale.z));
			}

			// top left
			m_guiChanged = GUI.changed;
			m_memberProperty = serializedObject.FindProperty ("m_cornerOffsetTL");
			m_newHandlePosition = m_topLeftCornerPosition = Handles.PositionHandle (m_cornerPositionTL + (imageWarpTransform.rotation * Vector3.Scale( m_memberProperty.vector3Value, m_currentScale )), imageWarpTransform.rotation);
			if (!m_guiChanged && GUI.changed)
			{
				m_newHandlePosition -= m_cornerPositionTL;
				m_memberProperty.vector3Value = Quaternion.Inverse (imageWarpTransform.rotation) * (new Vector3 (m_newHandlePosition.x / m_currentScale.x, m_newHandlePosition.y / m_currentScale.y, m_newHandlePosition.z / m_currentScale.z));
			}


			// Handle bezier curve handles

			m_memberProperty = serializedObject.FindProperty ("m_bezierEdges");

			if (m_memberProperty.boolValue)
			{
				m_guiChanged = GUI.changed;
				m_memberProperty = serializedObject.FindProperty ("m_topBezierHandleA");
				m_handlePos = m_topLeftCornerPosition + (imageWarpTransform.rotation * Vector3.Scale( m_memberProperty.vector3Value, m_currentScale ));
#if UNITY_5_5_OR_NEWER
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotHandleCap);
#else
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotCap);
#endif
				Handles.DrawLine (m_topLeftCornerPosition, m_newHandlePosition);
				if (!m_guiChanged && GUI.changed)
				{
					m_newHandlePosition = m_newHandlePosition - m_topLeftCornerPosition;
					m_memberProperty.vector3Value = Quaternion.Inverse (imageWarpTransform.rotation) * (new Vector3 (m_newHandlePosition.x / m_currentScale.x, m_newHandlePosition.y / m_currentScale.y, m_newHandlePosition.z / m_currentScale.z));
				}


				m_guiChanged = GUI.changed;
				m_memberProperty = serializedObject.FindProperty ("m_topBezierHandleB");
				m_handlePos = m_topRightCornerPosition + (imageWarpTransform.rotation * Vector3.Scale( m_memberProperty.vector3Value, m_currentScale ));
#if UNITY_5_5_OR_NEWER
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotHandleCap);
#else
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotCap);
#endif
				Handles.DrawLine (m_topRightCornerPosition, m_newHandlePosition);
				if (!m_guiChanged && GUI.changed)
				{
					m_newHandlePosition = m_newHandlePosition - m_topRightCornerPosition;
					m_memberProperty.vector3Value = Quaternion.Inverse (imageWarpTransform.rotation) * (new Vector3 (m_newHandlePosition.x / m_currentScale.x, m_newHandlePosition.y / m_currentScale.y, m_newHandlePosition.z / m_currentScale.z));				
				}



				m_guiChanged = GUI.changed;
				m_memberProperty = serializedObject.FindProperty ("m_leftBezierHandleA");
				m_handlePos = m_bottomLeftCornerPosition + (imageWarpTransform.rotation * Vector3.Scale( m_memberProperty.vector3Value, m_currentScale ));
#if UNITY_5_5_OR_NEWER
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotHandleCap);
#else
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotCap);
#endif
				Handles.DrawLine (m_bottomLeftCornerPosition, m_newHandlePosition);
				if (!m_guiChanged && GUI.changed)
				{
					m_newHandlePosition = m_newHandlePosition - m_bottomLeftCornerPosition;
					m_memberProperty.vector3Value = Quaternion.Inverse (imageWarpTransform.rotation) * (new Vector3 (m_newHandlePosition.x / m_currentScale.x, m_newHandlePosition.y / m_currentScale.y, m_newHandlePosition.z / m_currentScale.z));
				}

				m_guiChanged = GUI.changed;
				m_memberProperty = serializedObject.FindProperty ("m_leftBezierHandleB");
				m_handlePos = m_topLeftCornerPosition + (imageWarpTransform.rotation * Vector3.Scale( m_memberProperty.vector3Value, m_currentScale ));
#if UNITY_5_5_OR_NEWER
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotHandleCap);
#else
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotCap);
#endif
				Handles.DrawLine (m_topLeftCornerPosition, m_newHandlePosition);
				if (!m_guiChanged && GUI.changed)
				{
					m_newHandlePosition = m_newHandlePosition - m_topLeftCornerPosition;
					m_memberProperty.vector3Value = Quaternion.Inverse (imageWarpTransform.rotation) * (new Vector3 (m_newHandlePosition.x / m_currentScale.x, m_newHandlePosition.y / m_currentScale.y, m_newHandlePosition.z / m_currentScale.z));
				}



				m_guiChanged = GUI.changed;
				m_memberProperty = serializedObject.FindProperty ("m_rightBezierHandleA");
				m_handlePos = m_topRightCornerPosition + (imageWarpTransform.rotation * Vector3.Scale( m_memberProperty.vector3Value, m_currentScale ));
#if UNITY_5_5_OR_NEWER
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotHandleCap);
#else
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotCap);
#endif
				Handles.DrawLine (m_topRightCornerPosition, m_newHandlePosition);
				if (!m_guiChanged && GUI.changed)
				{
					m_newHandlePosition = m_newHandlePosition - m_topRightCornerPosition;
					m_memberProperty.vector3Value = Quaternion.Inverse (imageWarpTransform.rotation) * (new Vector3 (m_newHandlePosition.x / m_currentScale.x, m_newHandlePosition.y / m_currentScale.y, m_newHandlePosition.z / m_currentScale.z));				
				}

				m_guiChanged = GUI.changed;
				m_memberProperty = serializedObject.FindProperty ("m_rightBezierHandleB");
				m_handlePos = m_bottomRightCornerPosition + (imageWarpTransform.rotation * Vector3.Scale( m_memberProperty.vector3Value, m_currentScale ));
#if UNITY_5_5_OR_NEWER
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotHandleCap);
#else
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotCap);
#endif
				Handles.DrawLine (m_bottomRightCornerPosition, m_newHandlePosition);
				if (!m_guiChanged && GUI.changed)
				{
					m_newHandlePosition = m_newHandlePosition - m_bottomRightCornerPosition;
					m_memberProperty.vector3Value = Quaternion.Inverse (imageWarpTransform.rotation) * (new Vector3 (m_newHandlePosition.x / m_currentScale.x, m_newHandlePosition.y / m_currentScale.y, m_newHandlePosition.z / m_currentScale.z));
				}



				m_guiChanged = GUI.changed;
				m_memberProperty = serializedObject.FindProperty ("m_bottomBezierHandleA");
				m_handlePos = m_bottomRightCornerPosition + (imageWarpTransform.rotation * Vector3.Scale( m_memberProperty.vector3Value, m_currentScale ));
#if UNITY_5_5_OR_NEWER
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotHandleCap);
#else
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotCap);
#endif
				Handles.DrawLine (m_bottomRightCornerPosition, m_newHandlePosition);
				if (!m_guiChanged && GUI.changed)
				{
					m_newHandlePosition = m_newHandlePosition - m_bottomRightCornerPosition;
					m_memberProperty.vector3Value = Quaternion.Inverse (imageWarpTransform.rotation) * (new Vector3 (m_newHandlePosition.x / m_currentScale.x, m_newHandlePosition.y / m_currentScale.y, m_newHandlePosition.z / m_currentScale.z));
				}

				m_guiChanged = GUI.changed;
				m_memberProperty = serializedObject.FindProperty ("m_bottomBezierHandleB");
				m_handlePos = m_bottomLeftCornerPosition + (imageWarpTransform.rotation * Vector3.Scale( m_memberProperty.vector3Value, m_currentScale ));
#if UNITY_5_5_OR_NEWER
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotHandleCap);
#else
				m_newHandlePosition = Handles.FreeMoveHandle( m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotCap);
#endif
				Handles.DrawLine (m_bottomLeftCornerPosition, m_newHandlePosition);
				if (!m_guiChanged && GUI.changed)
				{
					m_newHandlePosition = m_newHandlePosition - m_bottomLeftCornerPosition;
					m_memberProperty.vector3Value = Quaternion.Inverse (imageWarpTransform.rotation) * (new Vector3 (m_newHandlePosition.x / m_currentScale.x, m_newHandlePosition.y / m_currentScale.y, m_newHandlePosition.z / m_currentScale.z));
				}
			}





			// Calculate the normal corner positions
			m_cornerPositionTR = m_imagePosition + (imageWarpTransform.rotation * m_cornerOffset);
			m_cornerPositionBL = m_imagePosition - (imageWarpTransform.rotation * m_cornerOffset);
			m_cornerPositionBR = m_imagePosition + (imageWarpTransform.rotation * (new Vector3(m_cornerOffset.x, -m_cornerOffset.y)));
			m_cornerPositionTL = m_imagePosition + (imageWarpTransform.rotation * (new Vector3(-m_cornerOffset.x, m_cornerOffset.y)));

			// Warp the corners with the offsets
			m_memberProperty = serializedObject.FindProperty ("m_cornerOffsetTR");
			m_cornerPositionTR = m_cornerPositionTR + (imageWarpTransform.rotation * Vector3.Scale (m_memberProperty.vector3Value, m_currentScale));
			m_memberProperty = serializedObject.FindProperty ("m_cornerOffsetBL");
			m_cornerPositionBL = m_cornerPositionBL + (imageWarpTransform.rotation * Vector3.Scale (m_memberProperty.vector3Value, m_currentScale));
			m_memberProperty = serializedObject.FindProperty ("m_cornerOffsetBR");
			m_cornerPositionBR = m_cornerPositionBR + (imageWarpTransform.rotation * Vector3.Scale (m_memberProperty.vector3Value, m_currentScale));
			m_memberProperty = serializedObject.FindProperty ("m_cornerOffsetTL");
			m_cornerPositionTL = m_cornerPositionTL + (imageWarpTransform.rotation * Vector3.Scale (m_memberProperty.vector3Value, m_currentScale));


			if (!serializedObject.FindProperty ("m_bezierEdges").boolValue)
			{
				m_topEdgeCropHandlePosition = (m_topRightCornerPosition + m_topLeftCornerPosition) / 2f;
				m_bottomEdgeCropHandlePosition = (m_bottomRightCornerPosition + m_bottomLeftCornerPosition) / 2f;
				m_leftEdgeCropHandlePosition = (m_topLeftCornerPosition + m_bottomLeftCornerPosition) / 2f;
				m_rightEdgeCropHandlePosition = (m_topRightCornerPosition + m_bottomRightCornerPosition) / 2f;

				m_guiChanged = GUI.changed;
				m_handlePos = m_topEdgeCropHandlePosition;

				#if UNITY_5_5_OR_NEWER
				m_newHandlePosition = Handles.FreeMoveHandle (m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize (m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotHandleCap);
				#else
				m_newHandlePosition = Handles.FreeMoveHandle (m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotCap);
				#endif
				if (!m_guiChanged && GUI.changed)
				{
					if (!m_cropHandleMoveInitialised)
					{
						CacheCropHandleData (imageWarpTransform, m_handlePos, m_bottomEdgeCropHandlePosition, m_topEdgeCropProperty.floatValue, a_isVerticalCrop: true, a_isBottomOrLeftCrop: false);
					}

					m_topEdgeCropProperty.floatValue = CalculateCurrentCropValue (m_newHandlePosition, m_preWarpHeight);
				}


				m_guiChanged = GUI.changed;
				m_handlePos = m_bottomEdgeCropHandlePosition;

				#if UNITY_5_5_OR_NEWER
				m_newHandlePosition = Handles.FreeMoveHandle (m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize (m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotHandleCap);
				#else
				m_newHandlePosition = Handles.FreeMoveHandle (m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotCap);
				#endif
				if (!m_guiChanged && GUI.changed)
				{
					if (!m_cropHandleMoveInitialised)
					{
						CacheCropHandleData (imageWarpTransform, m_handlePos, m_topEdgeCropHandlePosition, m_bottomEdgeCropProperty.floatValue, a_isVerticalCrop: true, a_isBottomOrLeftCrop: true);
					}

					m_bottomEdgeCropProperty.floatValue = CalculateCurrentCropValue (m_newHandlePosition, m_preWarpHeight);
				}


				m_guiChanged = GUI.changed;
				m_handlePos = m_leftEdgeCropHandlePosition;

				#if UNITY_5_5_OR_NEWER
				m_newHandlePosition = Handles.FreeMoveHandle (m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize (m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotHandleCap);
				#else
				m_newHandlePosition = Handles.FreeMoveHandle (m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotCap);
				#endif
				if (!m_guiChanged && GUI.changed)
				{
					if (!m_cropHandleMoveInitialised)
					{
						CacheCropHandleData (imageWarpTransform, m_handlePos, m_rightEdgeCropHandlePosition, m_leftEdgeCropProperty.floatValue, a_isVerticalCrop: false, a_isBottomOrLeftCrop: true);
					}

					m_leftEdgeCropProperty.floatValue = CalculateCurrentCropValue (m_newHandlePosition, m_preWarpWidth);
				}


				m_guiChanged = GUI.changed;
				m_handlePos = m_rightEdgeCropHandlePosition;

				#if UNITY_5_5_OR_NEWER
				m_newHandlePosition = Handles.FreeMoveHandle (m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize (m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotHandleCap);
				#else
				m_newHandlePosition = Handles.FreeMoveHandle (m_handlePos, imageWarpTransform.rotation, HandleUtility.GetHandleSize(m_handlePos) * BEZIER_HANDLE_SCALE, Vector3.one, Handles.DotCap);
				#endif
				if (!m_guiChanged && GUI.changed)
				{
					if (!m_cropHandleMoveInitialised)
					{
						CacheCropHandleData (imageWarpTransform, m_handlePos, m_leftEdgeCropHandlePosition, m_rightEdgeCropProperty.floatValue, a_isVerticalCrop: false, a_isBottomOrLeftCrop: false);
					}

					m_rightEdgeCropProperty.floatValue = CalculateCurrentCropValue (m_newHandlePosition, m_preWarpWidth);
				}

				if (GUIUtility.hotControl == 0)
				{
					m_cropHandleMoveInitialised = false;
				}
			}






			serializedObject.ApplyModifiedProperties ();

			if (!m_preGuiChanged && GUI.changed)
			{
				return true;
			}

			return false;
		}

		private float CalculateCurrentCropValue(Vector3 a_newHandlePosition, float a_preWarpDimensionSize)
		{
			m_currentDistanceFromOppositeHandle = (a_newHandlePosition - m_cropDragOppositeHandlePosition).magnitude;
			m_currentDistanceFromOriginalStartHandle = (a_newHandlePosition - m_cropDragStartHandlePosition).magnitude;

			if (m_currentDistanceFromOppositeHandle > m_currentDistanceFromOriginalStartHandle)
			{
				return m_cropDragStartCropValue + ((m_cropDragStartDistanceFromOppositeHandle - m_currentDistanceFromOppositeHandle) / a_preWarpDimensionSize) * (m_dragDirectionFlipped ? -1 : 1);
			}
			else
			{
				return m_cropDragStartCropValue + (((a_newHandlePosition - m_cropDragStartHandlePosition).magnitude) / a_preWarpDimensionSize) * (m_dragDirectionFlipped ? -1 : 1);
			}
		}

		private void CacheCropHandleData(Transform a_imageWarpTransform, Vector3 a_startHandlePosition, Vector3 a_oppositeHandlePosition, float a_startCropValue, bool a_isVerticalCrop, bool a_isBottomOrLeftCrop)
		{
			m_cropDragStartHandlePosition = a_startHandlePosition;
			m_cropDragOppositeHandlePosition = a_oppositeHandlePosition;
			m_cropDragStartDistanceFromOppositeHandle = (a_startHandlePosition - a_oppositeHandlePosition).magnitude;
			m_cropDragStartCropValue = a_startCropValue;
			if (a_isVerticalCrop)
			{
				m_dragDirectionFlipped = ( Quaternion.Inverse( a_imageWarpTransform.rotation ) * (a_oppositeHandlePosition - a_startHandlePosition)).y > 0;
			}
			else
			{
				m_dragDirectionFlipped = ( Quaternion.Inverse( a_imageWarpTransform.rotation ) * (a_oppositeHandlePosition - a_startHandlePosition)).x > 0;
			}

			if (a_isBottomOrLeftCrop)
			{
				m_dragDirectionFlipped = !m_dragDirectionFlipped;
			}

			m_cropHandleMoveInitialised = true;
		}

		private void CropCorners(ref Vector3 a_topLeft, ref Vector3 a_topRight, ref Vector3 a_bottomRight, ref Vector3 a_bottomLeft, float a_leftCrop, float a_topCrop, float a_rightCrop, float a_bottomCrop)
		{
			if (a_leftCrop != 0 || a_rightCrop != 0)
			{
				Vector3 topEdge = a_topRight - a_topLeft;
				Vector3 bottomEdge = a_bottomRight - a_bottomLeft;

				a_bottomLeft += a_leftCrop * bottomEdge;
				a_topLeft += a_leftCrop * topEdge;
				a_topRight -= a_rightCrop * topEdge;
				a_bottomRight -= a_rightCrop * bottomEdge;
			}

			if (a_bottomCrop != 0 || a_topCrop != 0)
			{
				Vector3 leftEdge = a_topLeft - a_bottomLeft;
				Vector3 rightEdge = a_topRight - a_bottomRight;

				a_bottomLeft += a_bottomCrop * leftEdge;
				a_topLeft -= a_topCrop * leftEdge;
				a_topRight -= a_topCrop * rightEdge;
				a_bottomRight += a_bottomCrop * rightEdge;
			}
		}
#endif
	}
}