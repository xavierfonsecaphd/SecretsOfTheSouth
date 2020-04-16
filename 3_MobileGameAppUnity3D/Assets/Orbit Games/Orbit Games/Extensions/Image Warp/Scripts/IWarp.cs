using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fenderrio.ImageWarp
{
	public interface IWarp
	{
		// Properties
		Vector3 cornerOffsetTL { get; set; }
		Vector3 cornerOffsetTR { get; set; }
		Vector3 cornerOffsetBL { get; set; }
		Vector3 cornerOffsetBR { get; set; }

		int numSubdivisions { get; set; }
		bool bezierEdges { get; set; }

		Vector3 topBezierHandleA { get; set; }
		Vector3 topBezierHandleB { get; set; }
		Vector3 rightBezierHandleA { get; set; }
		Vector3 rightBezierHandleB { get; set; }
		Vector3 bottomBezierHandleA { get; set; }
		Vector3 bottomBezierHandleB { get; set; }
		Vector3 leftBezierHandleA { get; set; }
		Vector3 leftBezierHandleB { get; set; }

		float cropLeft { get; set; }
		float cropTop { get; set; }
		float cropRight { get; set; }
		float cropBottom { get; set; }

		// Convenience methods
		void ResetAll ();
		void ResetCropping ();
		void ResetCornerOffsets ();
		void ResetBezierHandlesToDefault ();
	}
}