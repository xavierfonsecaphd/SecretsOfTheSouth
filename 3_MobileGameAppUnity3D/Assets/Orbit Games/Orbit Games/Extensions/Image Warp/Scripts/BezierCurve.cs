using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fenderrio.ImageWarp
{
	[System.Serializable]
	public class BezierCurve
	{
		[System.Serializable]
		public class BezierCurvePoint
		{
			[SerializeField]
			private Vector3 m_anchorPoint;
			[SerializeField]
			private Vector3 m_handlePointA;
			[SerializeField]
			private Vector3 m_handlePointB;
			[SerializeField]
			private bool m_mirroredHandles = true;
			[SerializeField]
			private bool m_handlesFollowAnchor = true;


			public Vector3 AnchorPoint
			{
				get { return m_anchorPoint; }
				set
				{
					if (m_handlesFollowAnchor)
					{
						Vector3 movementOffset = value - m_anchorPoint;

						m_anchorPoint = value;

						m_handlePointA += movementOffset;
						m_handlePointB += movementOffset;
					}
					else
					{
						m_anchorPoint = value;
					}
				}
			}

			public Vector3 HandlePointA
			{
				get {
					return m_handlePointA;
				}
				set {
					m_handlePointA = value;

					if (m_mirroredHandles)
					{
						m_handlePointB = m_anchorPoint + (m_anchorPoint - m_handlePointA);
					}
				}
			}

			public Vector3 HandlePointB
			{
				get {
					return m_handlePointB;
				}
				set {
					m_handlePointB = value;

					if (m_mirroredHandles)
					{
						m_handlePointA = m_anchorPoint + (m_anchorPoint - m_handlePointB);
					}
				}
			}

			public bool MirroredHandles { get { return m_mirroredHandles; } }
			public bool HandlesFollowAnchor { get { return m_handlesFollowAnchor; } }
		}


		private const int NUM_CURVE_SAMPLE_SUBSECTIONS = 50;
		private const int MAX_NUM_ANCHOR_POINTS = 5;
		
		public BezierCurvePoint[] m_pointsData;

		private Vector3[] m_temp_anchor_points;
		private Vector3 m_rot;

		public Vector3 GetCurvePoint(float progress, int num_anchors = 4, int curve_idx = -1, float yOffset = 0)
		{
			if(m_pointsData.Length < 2)
				return Vector3.zero;
			
			if(m_temp_anchor_points == null || m_temp_anchor_points.Length < num_anchors)
				m_temp_anchor_points = new Vector3[num_anchors];

			if(progress < 0)
				progress = 0;

			if(curve_idx < 0)
			{
				// Work out curve idx from progress
				curve_idx = Mathf.FloorToInt(progress);
				
				progress %= 1;
			}
			
			if(curve_idx >= m_pointsData.Length - 1)
			{
				curve_idx = m_pointsData.Length - 2;
				progress = 1;
			}
			

			for(int idx=1; idx < num_anchors; idx++)
			{
				if(num_anchors == 4)
				{
					if(idx == 1)
						m_temp_anchor_points[idx-1] = m_pointsData[curve_idx].AnchorPoint + ( (curve_idx == 0 ? m_pointsData[curve_idx].HandlePointA : m_pointsData[curve_idx].HandlePointB) -  m_pointsData[curve_idx].AnchorPoint) * progress;
					else if(idx == 2)
						m_temp_anchor_points[idx-1] = (curve_idx == 0 ? m_pointsData[curve_idx].HandlePointA : m_pointsData[curve_idx].HandlePointB) + ( m_pointsData[curve_idx+1].HandlePointA - (curve_idx == 0 ? m_pointsData[curve_idx].HandlePointA : m_pointsData[curve_idx].HandlePointB)) * progress;
					else if(idx == 3)
						m_temp_anchor_points[idx-1] = m_pointsData[curve_idx+1].HandlePointA + ( m_pointsData[curve_idx+1].AnchorPoint - m_pointsData[curve_idx+1].HandlePointA) * progress;
				}
				else
					m_temp_anchor_points[idx-1] = m_temp_anchor_points[idx-1] + (m_temp_anchor_points[idx] - m_temp_anchor_points[idx-1]) * progress;
			}
			
			if(num_anchors == 2)
			{
				// Reached the bezier curve point requested
				// Check for yOffset
				if(yOffset != 0)
				{
					// Calculate UpVector for this point on the curve
					Vector3 upVec = Vector3.Cross((m_temp_anchor_points[1] - m_temp_anchor_points[0]), Vector3.forward);

					m_temp_anchor_points[0] -= upVec.normalized * yOffset;
				}

				return m_temp_anchor_points[0];
			}
			else
				return GetCurvePoint(progress, num_anchors-1, curve_idx, yOffset);
		}
		
		public Vector3 GetCurvePointRotation(float progress, int curve_idx = -1)
		{
			if(m_pointsData.Length < 2)
				return Vector3.zero;
			
			if(curve_idx < 0)
			{
				// Work out curve idx from progress
				curve_idx = Mathf.FloorToInt(progress);
				
				progress %= 1;
			}
			
			if(curve_idx >= m_pointsData.Length - 1)
			{
				curve_idx = m_pointsData.Length - 2;
				progress = 1;
			}
			
			if(progress < 0)
				progress = 0;
			
			Vector3 point_dir_vec = GetCurvePoint(Mathf.Clamp(progress + 0.01f, 0, 1), curve_idx : curve_idx) - GetCurvePoint(Mathf.Clamp(progress - 0.01f, 0, 1), curve_idx : curve_idx);
			
			if(point_dir_vec.Equals(Vector3.zero))
			{
				return Vector3.zero;
			}
			
			m_rot = (Quaternion.AngleAxis(-90, point_dir_vec) * Quaternion.LookRotation(Vector3.Cross(point_dir_vec, Vector3.forward), Vector3.forward)).eulerAngles;
			
			// Clamp all axis rotations to be within [-180, 180] range for more sensible looking rotation transitions
			m_rot.x -= m_rot.x < 180 ? 0 : 360;
			m_rot.y -= m_rot.y < 180 ? 0 : 360;
			m_rot.z -= m_rot.z < 180 ? 0 : 360;
			
			return m_rot;
		}
		
		// Get an approximation of the belzier curve length
		float GetCurveLength(int curve_idx)
		{
			int num_precision_intervals = NUM_CURVE_SAMPLE_SUBSECTIONS;
			Vector3? last_point = null;
			Vector3 current_point;
			float curve_length = 0;
			
			for(int idx=0; idx < num_precision_intervals; idx++)
			{
				current_point = GetCurvePoint((float) idx / (num_precision_intervals-1), curve_idx : curve_idx);
				
				if(last_point != null)
				{
					curve_length += ((Vector3)(current_point - last_point)).magnitude;
				}
				
				last_point = current_point;
			}

			return curve_length;
		}
	}
}