using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OGImageEffect : BaseMeshEffect {

    UIVertex vert = new UIVertex();
    private Vector2 random;
    private Image image;

    protected override void OnEnable()
    {
        base.OnEnable();
        random = Random.insideUnitCircle;
    }

    protected override void Awake()
    {
        base.Awake();
        var canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();
        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vert, i);
            vert.normal = transform.localPosition * 0.0001f;
            vert.uv1 = random;
            vh.SetUIVertex(vert, i);
        }
    }

    Vector3 previousPosition = Vector3.zero;
    private void Update()
    {
        var position = transform.position;
        if (position != previousPosition)
        {
            image.SetVerticesDirty();
        }
        previousPosition = position;
    }
}
