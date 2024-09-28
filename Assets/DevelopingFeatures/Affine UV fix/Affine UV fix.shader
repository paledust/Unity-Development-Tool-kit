Shader "Affine UV fix" {
		
Properties {
	_MainTex ("Main Texture", 2D) = ""
}

SubShader {
	Pass 
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float3 uv0 : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			float3 uv0 : TEXCOORD0;
		};

		sampler2D _MainTex;

		v2f vert (appdata v)
		{
			v2f o;

			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv0 = v.uv0;
			return o;
		}

		fixed4 frag (v2f i) : SV_Target
		{
			fixed4 col = tex2D(_MainTex, float2(i.uv0.x/i.uv0.z, i.uv0.y));
			return col;
		}
	ENDCG
}}

}