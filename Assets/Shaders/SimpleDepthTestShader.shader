Shader "Custom/DepthTestingShader"
{
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True"
		}

		Pass
		{
			Blend One One
			Cull Front
			ZWrite Off
			ZTest Always

			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			//camera depth texture
			uniform sampler2D _CameraDepthTexture;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				float4 projPos : TEXCOORD0;
			};

			//vertex shader
			v2f vert(appdata v)
			{
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.projPos = ComputeScreenPos(o.position);
				return o;
			}

			//fragment shader (calculates the colour of a specific pixel)
			float4 frag(v2f i) : COLOR
			{
				//work out how far from the camera the current fragment in the depth buffer is 
				float currDistToCamera = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);
				//return that value as the red component of the colour 
				return float4(currDistToCamera, 0, 0, 1);
			}
			ENDCG
		}
	}
}