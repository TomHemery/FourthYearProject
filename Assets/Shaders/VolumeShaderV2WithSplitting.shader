Shader "Custom/VolumeShaderV2WithSplitting"
{
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent" "RenderType" = "Transparent"
		}

		CGINCLUDE
		#include "UnityCG.cginc"

		//sets how far as a proportion of max distance a ray will advance at each step
		int _SamplingQuality;

		//the 3D texture to be rendered
		sampler3D _MainTex;

		//the density of the colour values within the volume
		float _Density;

		//whether to use each colour channel
		int _Red;
		int _Green;
		int _Blue;

		//information on slicing plane
		float4 _PlanePos;
		float4 _PlaneNormal;
		int _ReversePlaneSlicing;

		//camera depth texture
		uniform sampler2D _CameraDepthTexture;
		uniform float4x4 _CameraInvViewMatrix;

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 localPos : TEXCOORD1;
		};

		//vertex shader
		v2f vert(appdata v)
		{
			v2f OUT;
			OUT.pos = UnityObjectToClipPos(v.vertex);
			OUT.localPos = v.vertex.xyz;
			OUT.uv = v.uv.xy;
			return OUT;
		}

		// ray/cube intersection algorithm
		struct Ray
		{
			float3 origin;
			float3 direction;
		};

		bool IntersectBox(Ray ray, out float entryPoint, out float exitPoint)
		{
			float3 invR = 1.0 / ray.direction;
			float3 tbot = invR * (float3(-0.5, -0.5, -0.5) - ray.origin);
			float3 ttop = invR * (float3(0.5, 0.5, 0.5) - ray.origin);
			float3 tmin = min(ttop, tbot);
			float3 tmax = max(ttop, tbot);
			float2 t = max(tmin.xx, tmin.yz);
			entryPoint = max(t.x, t.y);
			t = min(tmax.xx, tmax.yz);
			exitPoint = min(t.x, t.y);

			return entryPoint <= exitPoint;
		}

		//fragment shader (calculates the colour of a specific pixel)
		float4 frag(v2f IN) : COLOR
		{
			float4 color = float4(0,0,0,0);
			float3 localCameraPosition = UNITY_MATRIX_IT_MV[3].xyz;

			Ray ray;
			ray.origin = localCameraPosition;
			ray.direction = normalize(IN.localPos - localCameraPosition);

			float entryPoint, exitPoint;
			IntersectBox(ray, entryPoint, exitPoint);

			if (entryPoint < 0.0) entryPoint = 0.0;

			float3 rayStart = ray.origin + ray.direction * entryPoint;
			float3 rayStop = ray.origin + ray.direction * exitPoint;

			float dist = distance(rayStop, rayStart);
			float stepSize = dist / float(_SamplingQuality);
			float3 ds = normalize(rayStop - rayStart) * stepSize;
			float3 pos = rayStop.xyz + 0.5f;

			float2 uv = IN.uv;
			#if UNITY_UV_STARTS_AT_TOP
			uv.y = 1 - uv.y;
			#endif

			float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, uv).r);
			float3 r = ray.origin.xyz;
			r /= abs(r.z);
			r = mul(_CameraInvViewMatrix, r);
			depth *= length(r.xyz);

			for (int i = _SamplingQuality; i >= 0; --i)
			{
				float4 mask = float4(0, 0, 0, 0);
				//check if occluded by cutting plane 
				float3 between = pos - _PlanePos.xyz;
				float val = dot(between, _PlaneNormal.xyz);
				if (_ReversePlaneSlicing <= 0 && val > 0 || _ReversePlaneSlicing > 0 && val < 0) { //if not occluded by cutting plane
					float travelled = (_SamplingQuality - i) * ds;
					if (travelled < depth) { //if not occluded by something in the depth buffer
						//accumulate the colour of this point 
						mask = tex3D(_MainTex, pos);
						color.xyz += mask.xyz * mask.w;
					}
				}
				pos -= ds;
			}
			color *= _Density / (uint)_SamplingQuality;

			if (_Red == 0) color.x = 0;
			if (_Green == 0) color.y = 0;
			if (_Blue == 0) color.z = 0;

			return color;
		}
		ENDCG

		Pass
		{
			Blend One One
			Cull front
			ZWrite off
			ZTest Always

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			ENDCG

		}
	}
}
