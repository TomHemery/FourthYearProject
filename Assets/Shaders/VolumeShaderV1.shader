Shader "Custom/VolumeShaderV1"
{
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		CGINCLUDE
		#include "UnityCG.cginc"

		//sets how far as a proportion of max distance a ray will advance at each step
		int _SamplingQuality;
		//the 3D texture to be rendered
		sampler3D _MainTex;
		//the density of individual 
		float _Density;
		//wether to use each colour channel 
		int _Red;
		int _Green;
		int _Blue;
		//the crop point for the x plane
		float _XCrop;

		struct v2f
		{
			float4 pos : SV_POSITION;
			float3 localPos : TEXCOORD0;
			float4 screenPos : TEXCOORD1;
			float3 worldPos : TEXCOORD2;
		};

		v2f vert(appdata_base v)
		{
			v2f OUT;
			OUT.pos = UnityObjectToClipPos(v.vertex);
			OUT.localPos = v.vertex.xyz;
			OUT.screenPos = ComputeScreenPos(OUT.pos);
			COMPUTE_EYEDEPTH(OUT.screenPos.z);
			OUT.worldPos = mul(unity_ObjectToWorld, v.vertex);
			return OUT;
		}

		// usual ray/cube intersection algorithm
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

		//calculate the value of this individual fragment (determines the colour of this point) 
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

			float3 start = rayStop;
			float dist = distance(rayStop, rayStart);
			float stepSize = dist / float(_SamplingQuality);
			float3 ds = normalize(rayStop - rayStart) * stepSize;

			for (int i = _SamplingQuality; i >= 0; --i)
			{
				float3 pos = start.xyz;
				pos.xyz = pos.xyz + 0.5f;
				float4 mask = tex3D(_MainTex, pos);

				if (_XCrop > pos.x) {
					mask.w = 0;
				}

				color.xyz += mask.xyz * mask.w;

				start -= ds;
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
			Cull front
			Blend One One
			ZWrite false

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			ENDCG

		}
	}
}