using UnityEngine;
using Fenderrio.ImageWarp;

public class ImageWarpDemo : MonoBehaviour {

	public ImageWarp m_imageWarper;

	public NativeImageWarp m_nativeImageWarp;

	public float m_cropSpeed = 0.01f;
	public Vector3 m_warpVector = new Vector3(0.01f, 0.01f, 0.01f);

	void Start ()
	{
		m_imageWarper.cornerOffsetBL = Vector3.zero;
		m_imageWarper.cornerOffsetTL = new Vector3(-20f, 20f, 0);
		m_imageWarper.cornerOffsetTR = new Vector3(20f, 20f, 0);
		m_imageWarper.cornerOffsetBR = Vector3.zero;

		m_imageWarper.numSubdivisions = 12;

		m_imageWarper.bezierEdges = true;
	}

	void Update()
	{
		if (Input.GetKey (KeyCode.LeftArrow))
		{
			m_nativeImageWarp.cropLeft -= m_cropSpeed;
			m_nativeImageWarp.cropRight += m_cropSpeed;

			m_nativeImageWarp.cornerOffsetTL -= m_warpVector;
		}
		else if (Input.GetKey (KeyCode.RightArrow))
		{
			m_nativeImageWarp.cropLeft += m_cropSpeed;
			m_nativeImageWarp.cropRight -= m_cropSpeed;

			m_nativeImageWarp.cornerOffsetTL += m_warpVector;
		}
	}
}