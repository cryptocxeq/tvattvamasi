Shader "Custom/Displacement"
{
 Properties
 {
  _MainTex ("Texture", 2D) = "white" {}
  _DispTex ("Displacement", 2D) = "white" {}
  _Speed ("Speed", Float) = 0.5
  _Intensity ("Intensity", Float) = 0.025
  _XDir ("XDir", Float) = 0.0
  _YDir ("YDir", Float) = 0.0
 }

 SubShader
 {
  Tags
  {
         "RenderType" = "Transparent"
         "Queue" = "Transparent"
  }

  Pass
  {
   ZWrite Off
   Blend SrcAlpha OneMinusSrcAlpha

   CGPROGRAM

   #pragma vertex vert
   #pragma fragment frag
   
   #include "UnityCG.cginc"

   struct appdata
   {
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    fixed4 vertexColor : COLOR;
   };

   struct v2f
   {
    float2 uv1 : TEXCOORD0;
    float2 uv2 : TEXCOORD1;
    float4 vertex : SV_POSITION;
    fixed4 col : COLOR;
   };

   sampler2D _MainTex;
   sampler2D _DispTex;
   float4 _MainTex_ST;
   float4 _DispTex_ST;
   float _Speed;
   float _Intensity;
   float _XDir;
   float _YDir;
   
   v2f vert (appdata v)
   {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv1 = TRANSFORM_TEX(v.uv, _DispTex);
    o.uv2 = TRANSFORM_TEX(v.uv, _MainTex);
    o.col = v.vertexColor;
    return o;
   }
   
   fixed4 frag (v2f i) : SV_Target
   {
    float t1 = _Time.y * _Speed * _XDir;
    float t2 = _Time.y * _Speed * _YDir;
    fixed disp = tex2D(_DispTex, i.uv1 + fixed2(t1, t2)).r;
    fixed4 col = tex2D(_MainTex, i.uv2 + disp * _Intensity);
    return col * i.col;
   }

   ENDCG
  }
 }
}