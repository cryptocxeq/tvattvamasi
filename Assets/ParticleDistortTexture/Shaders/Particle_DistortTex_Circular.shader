// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Particle Distort Texture/Circular" 
{
Properties 
{
	[Enum(Off,0,Front,1,Back,2)] _CullMode ("Culling Mode", int) = 0
	[Enum(Additive,1,AlphaBlend,10)] _DstBlend ("Desination Blend Options", Int) = 1
	_DistortX ("Distortion in X", Range (0,2)) = 1
	_DistortY ("Distortion in Y", Range (0,2)) = 0
	_MaskTex ("_MaskTex A", 2D) = "white" {}
	_Distort ("_Distort A", 2D) = "white" {}
	_MainTex ("_MainTex RGBA", 2D) = "white" {}
	_AlphaM ("Alpha Multiplier", Range (0,10)) = 1
	[HideInInspector][PerRendererData][Toggle] _Enabled("Mask Enabled", Float) = 1 
	[HideInInspector][PerRendererData][Toggle] _ClampHoriz("Clamp Alpha Horizontally", Float) = 0 
	[HideInInspector][PerRendererData][Toggle] _ClampVert("Clamp Alpha Vertically", Float) = 0 
	[HideInInspector][PerRendererData][Toggle] _UseAlphaChannel("Use Mask Alpha Channel (not RGB)", Float) = 0 
	[HideInInspector][PerRendererData][Toggle] _ScreenSpaceUI ("Is this screen space ui element?", Float) = 0 
	[HideInInspector][PerRendererData]_AlphaTex("Alpha Mask", 2D) = "white" {} 
	[HideInInspector][PerRendererData]_ClampBorder("Clamping Border", Float) = 0.01

}

Category 
{
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "ToJMasked" = "True" }
	Blend SrcAlpha [_DstBlend]
	Cull [_CullMode] Lighting Off ZWrite Off

	Lighting Off
	
	SubShader 
	{
		Pass 
		{
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Assets/Alpha Masking/ToJAlphaMasking.cginc" 

			sampler2D _MainTex;
			sampler2D _Distort;
			sampler2D _MaskTex;
			
			struct appdata_t 
			{
				fixed4 vertex : POSITION;
				fixed2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f 
			{
				fixed4 vertex : SV_POSITION;
				fixed2 texcoord : TEXCOORD0;
				fixed2 texcoord1 : TEXCOORD1;
				fixed4 color : COLOR;
				TOJ_MASK_COORDS(2) 
			};
			
			fixed4 _MainTex_ST;
			fixed4 _Distort_ST;
			fixed4 _MaskTex_ST;
			
			fixed _DistortX;
			fixed _DistortY;
			fixed _AlphaM;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_Distort);
				o.texcoord1 = v.texcoord;
				o.color = v.color;
				TOJ_TRANSFER_MASK(o, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : Color
			{
				fixed tex = tex2D(_MaskTex, i.texcoord1).a;
				fixed distort = tex2D(_Distort, i.texcoord).a;
				
				fixed2 distortUV = fixed2((
									i.texcoord1.x/tex //x UV
									-distort)*_DistortX,(
									i.texcoord1.y/tex //y UV
									-distort)*_DistortY);
									
				fixed4 tex2 = tex2D(_MainTex,distortUV);
				TOJ_APPLY_MASK(i, tex2.a); 
				return fixed4(tex2.rgb,tex2.a*tex*_AlphaM)*i.color;
			}
			ENDCG 
		}
	}	
}
}
