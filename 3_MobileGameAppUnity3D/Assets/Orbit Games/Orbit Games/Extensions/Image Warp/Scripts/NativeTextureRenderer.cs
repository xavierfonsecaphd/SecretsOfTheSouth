using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fenderrio.ImageWarp
{
	[ExecuteInEditMode()]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class NativeTextureRenderer : MonoBehaviour
	{
		protected Mesh m_mesh;
		private MeshFilter m_meshFilter;
		private MeshRenderer m_meshRenderer;

		public MeshFilter MeshFilterCache {
			get {
				if (m_meshFilter == null)
				{
					m_meshFilter = GetComponent<MeshFilter> ();
				}
				return m_meshFilter;
			}
		}

		public MeshRenderer MeshRendererCache {
			get {
				if (m_meshRenderer == null)
				{
					m_meshRenderer = GetComponent<MeshRenderer> ();
				}
				return m_meshRenderer;
			}
		}

		private float m_width;
		private float m_height;

		public Vector2 Dimensions { get { return new Vector2(m_width, m_height); } }

		[SerializeField]
		protected Texture2D m_texture;

		[SerializeField]
		protected Color m_colour = Color.white;

		[SerializeField]
		protected bool m_flipX;

		[SerializeField]
		protected bool m_flipY;

		private Vector3[] m_verts = new Vector3[]{ new Vector3(-0.5f, -0.5f,0), new Vector3(-0.5f,0.5f,0), new Vector3(0.5f,0.5f,0), new Vector3(0.5f,-0.5f,0) };
		private int[] m_indices = new int[]{ 0,1,2,0,2,3 };
		private Vector2[] m_uvs = new Vector2[]{ Vector2.zero, new Vector2(0,1), Vector2.one, new Vector2(1,0) };
		private Color[] m_colours = new Color[]{ Color.white, Color.white, Color.white, Color.white };

		private Texture2D m_textureReference;

		public Vector3[] MeshVerts { get { return m_verts; } }

		public Color color
		{
			get { return m_colour; }
			set { m_colour = value; }
		}

		public bool flipX
		{
			get { return m_flipX; }
			set { m_flipX = value; }
		}

		public bool flipY
		{
			get { return m_flipY; }
			set { m_flipY = value; }
		}

		public Texture2D texture
		{
			get { return m_texture; }
			set { m_texture = value; }
		}

		private void OnEnable ()
		{
			// Force generate a new Material instance
			if(MeshRendererCache.sharedMaterial == null)
				MeshRendererCache.sharedMaterial = new Material( Shader.Find("Sprites/Default") );

			UpdateMesh ();
		}

		public virtual void UpdateMesh(bool a_changeMesh = true)
		{
			if (m_mesh == null)
			{
				m_mesh = new Mesh ();
				MeshFilterCache.mesh = m_mesh;
			}
				
			if (m_verts == null)
			{
				// Initialise mesh data arrays
				m_verts = new Vector3[]{ new Vector3(-0.5f, -0.5f,0), new Vector3(-0.5f,0.5f,0), new Vector3(0.5f,0.5f,0), new Vector3(0.5f,-0.5f,0) };
				m_indices = new int[]{ 0,1,2,0,2,3 };
				m_uvs = new Vector2[]{ Vector2.zero, new Vector2(0,1), Vector2.one, new Vector2(1,0) };
				m_colours = new Color[]{ m_colour, m_colour, m_colour, m_colour };
			}

			if (m_texture != null && (m_textureReference == null || m_textureReference != m_texture))
			{
				m_textureReference = m_texture;

				m_width = m_textureReference.width / 100f;
				m_height = m_textureReference.height / 100f;

				m_verts [0] = new Vector3 (-m_width / 2, -m_height / 2, 0);
				m_verts [1] = new Vector3 (-m_width / 2, m_height / 2, 0);
				m_verts [2] = new Vector3 (m_width / 2, m_height / 2, 0);
				m_verts [3] = new Vector3 (m_width / 2, -m_height / 2, 0);
			}

			if (!m_colour.Equals (m_colours [0]))
			{
				// Update mesh colours array
				m_colours = new Color[]{ m_colour, m_colour, m_colour, m_colour };
			}

			// Update Mesh Values
			if (a_changeMesh)
			{
				m_mesh.vertices = m_verts;
				m_mesh.uv = m_uvs;
				m_mesh.triangles = m_indices;
				m_mesh.colors = m_colours;
			}

			// Ensure latest Sprite is assigned to material
			Material myMat = MeshRendererCache.sharedMaterial;
			if (myMat != null)
			{
				myMat.mainTexture = m_textureReference;
			}
		}
	}
}