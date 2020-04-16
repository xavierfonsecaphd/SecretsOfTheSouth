// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Secrets of the South/Displacement Shader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        //_Displacement ("UV Displacement Texture", 2D) = "white" {}
        _DisplacementScale ("Displacement Scale", Float) = 10
        _DisplacementEffect ("Displacement Effect", Range(0, 1)) = 0.1
        _DisplacementShift ("Displacement Shift", Range(0, 1)) = 0.1
		
        //_Dirt ("UV Dirt Texture", 2D) = "white" {}
        _DirtScale ("Dirt Scale", Float) = 10
        _DirtEffect ("Dirt Effect", Range(0, 1)) = 0.1
        _DirtShift ("Dirt Shift", Range(0, 1)) = 0.1
		
        _ShadeScaleX ("Shade Scale X", Float) = 10
        _ShadeScaleY ("Shade Scale Y", Float) = 10
        _ShadeEffect ("Detail Effect", Range(0, 1)) = 0.1

        _PaperScaleX ("Paper Scale X", Float) = 10
        _PaperScaleY ("Paper Scale Y", Float) = 10
        _PaperEffect ("Detail Effect", Range(0, 1)) = 0.1

        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float3 objectPosition : NORMAL;
                float2 shift : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float3 localPosition : NORMAL;
                UNITY_VERTEX_OUTPUT_STEREO
            };
			
            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            float4 _ObjectWorldPositionScale;
			
            sampler2D _Displacement;
            fixed _DisplacementScale;
            fixed _DisplacementEffect;
            fixed _DisplacementShift;
			
            sampler2D _Dirt;
            fixed _DirtScale;
            fixed _DirtEffect;
            fixed _DirtShift;
			
            fixed _ShadeScaleX;
            fixed _ShadeScaleY;
            fixed _ShadeEffect;

            fixed _PaperScaleX;
            fixed _PaperScaleY;
            fixed _PaperEffect;

			fixed hash( fixed n )
			{
			    return frac(sin(n)*43758.5453);
			}

			fixed noise( fixed3 x )
			{
			    // The noise function returns a value in the range -1.0f -> 1.0f

			    fixed3 p = floor(x);
			    fixed3 f = frac(x);

			    f       = f*f*(3.0-2.0*f);
			    fixed n = p.x + p.y*57.0 + 113.0*p.z;

			    return 2 * lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
			                   lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
			               lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
			                   lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z) - 1;
			}

			fixed4 quantize(fixed4 value, int numbers) {
				return round(value * numbers)/numbers;
			}

			fixed4 threshold(fixed4 value, fixed threshold) {
				return floor(abs(value) + threshold) * sign(value);
			}
			
			float rand(float3 co)
			{
				return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
			}

            v2f vert(appdata v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
				OUT.localPosition = OUT.worldPosition - ((v.objectPosition + float3(v.shift, 0)) * 10000.0);
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
				
				//return float4(IN.localPosition, 1);
				//half2 displace = tex2D(_Displacement, round(_Time.w) * _DisplacementShift * float2(1, 1) + IN.vertex.xy * _DisplacementScale).xy * 2 - 1;
				//half2 dirt = tex2D(_Dirt, round(_Time.w) * _DirtShift * float2(1, 1) + IN.vertex.xy * _DirtScale).xy * 2 - 1;
				
				fixed displaceShift = round(_Time.w) * _DisplacementShift * 137;
				fixed2 displace = fixed2(
					noise((IN.localPosition.xyz + displaceShift) * _DisplacementScale), 
					noise((IN.localPosition.xyz + displaceShift - fixed3(10, 10, 10)) * _DisplacementScale));
					
				fixed dirtShift = round(_Time.w) * _DirtShift * 137;
				fixed2 dirt = fixed2(
					noise((IN.localPosition.xyz + dirtShift) * _DirtScale), 
					noise((IN.localPosition.xyz + dirtShift - fixed3(100, 100, 100)) * _DirtScale));
					
				fixed4 shade = fixed4(1, 1, 1, 0) * noise(fixed4(IN.localPosition.x * _ShadeScaleX, IN.localPosition.y * _ShadeScaleY, 1, 1));
				fixed4 paper = fixed4(1, 1, 1, 0) * noise(fixed4(IN.localPosition.x * _PaperScaleX, IN.localPosition.y * _PaperScaleY, 1, 1));
				fixed4 spots = fixed4(1, 1, 1, 0) * noise(fixed4(IN.localPosition.y * 0.1, IN.localPosition.x * 0.1, 1, 1));

                fixed4 color = 
					(tex2D(_MainTex, IN.texcoord + displace * _DisplacementEffect + dirt * _DirtEffect) + _TextureSampleAdd) 
					* IN.color - quantize(shade, 2) * _ShadeEffect - paper * _PaperEffect;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

				//return float4(dirt,0,1);
                return color;
            }
        ENDCG
        }
    }
}