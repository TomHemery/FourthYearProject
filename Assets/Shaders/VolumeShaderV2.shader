Shader "Custom/VolumeShaderV2"
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
			#pragma target 4.0
			#pragma vertex vert
			#pragma fragment frag

			//sets how far as a proportion of max distance a ray will advance at each step
			int _SamplingQuality;

			//the 3D texture to be rendered
			sampler3D _MainTex;

			//the density of the colour values within the volume
			float _Density;

			//the threshold for color values as read from the texture
			float _Threshold;

			//camera depth texture
			uniform sampler2D _CameraDepthTexture;

			//whether to use each colour channel
			int _Red;
			int _Green;
			int _Blue;

			//information on occlusion plane (erases parts of volumes)
			float4 _OcclusionPlanePos;
			float4 _OcclusionPlaneNormal;
			int _DoOcclusion;

			//information on cutting plane (slices volume in two - works the same as occlusion plane)
			#define MAX_CUTTING_PLANES 5
			float4 _CuttingPlanePositions[MAX_CUTTING_PLANES];
			float4 _CuttingPlaneNormals[MAX_CUTTING_PLANES];
			int _NumCuttingPlanes;
			int _DoCutting;


			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 clipPos : SV_POSITION; //clipspace position
				float4 projPos : TEXCOORD0; //projected position
				float3 localPos : TEXCOORD1; //local position
			};

			//vertex shader
			v2f vert(appdata v)
			{
				v2f o;
				o.clipPos = UnityObjectToClipPos(v.vertex);
				o.projPos = ComputeScreenPos(o.clipPos);
				o.localPos = v.vertex.xyz;
				return o;
			}

			// ray struct
			struct Ray
			{
				float3 origin;
				float3 direction;
			};

			//ray / cube intersection (generates entry and exit point of the ray)
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
			fixed4 frag(v2f i) : COLOR
			{
				//calculate the position of the camera relative to this point 
				float3 localCameraPosition = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));

				//create a ray starting at the (local) camera position and heading towards the (local) position of this fragment
				Ray ray;
				ray.origin = localCameraPosition;
				ray.direction = normalize(i.localPos - localCameraPosition);

				//calcualte the entry and exit points of the ray into the box (as distances along the ray)
				float entryPoint, exitPoint;
				IntersectBox(ray, entryPoint, exitPoint);
				if (entryPoint < 0.0) entryPoint = 0.0;

				//calculate the entry and exit points as vectors (rayStart, rayStop)
				float3 rayStart = ray.origin + ray.direction * entryPoint;
				float3 rayStop = ray.origin + ray.direction * exitPoint;

				//calculate the size of step through the volume and a vector that represents that exact step along the ray
				float stepSize = (exitPoint - entryPoint) / float(_SamplingQuality);
				float3 step = normalize(rayStop - rayStart) * stepSize;

				//set pos (the current position along the ray) to be the end of the ray (the exit point from the box)
				float3 pos = rayStop.xyz;

				//work out how far from the camera the current fragment in the depth buffer is 
				float currDistToCamera = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);

				bool sampledAtLastPoint = false;

				//step from the back of the box forward along the ray, sampling the texture as we go
				fixed4  color = fixed4(0, 0, 0, 0);
				[loop]
				for (int stepCount = _SamplingQuality; stepCount >= 0; --stepCount)
				{
					//work out the distance to the camera from the current position
					float distToCamera = length(localCameraPosition - pos);
					if (distToCamera <= currDistToCamera)//if we aren't occluded by depth buffer objects
					{
						bool sample = true;
						if (_DoOcclusion != 0) { //if we are considering the occlusion plane
							//check if occluded by plane 
							float3 between = pos - _OcclusionPlanePos.xyz;
							float val = dot(between, _OcclusionPlaneNormal.xyz);
							if (val <= 0) sample = false;
						}
						if (sample && _DoCutting != 0) { //if we are considering the cutting planes
							//check if occluded by any cutting plane in the list
							int index = 0;
							[unroll(MAX_CUTTING_PLANES)]
							for (index; index < _NumCuttingPlanes; index++) {
								float3 between = pos - _CuttingPlanePositions[index].xyz;
								float val = dot(between, _CuttingPlaneNormals[index].xyz);
								if (val <= 0) {
									sample = false;
									break;
								}	
							}
						}
						if(sample) { //if we aren't occluded by the plane, or cut off
							fixed4 mask = tex3D(_MainTex, pos + 0.5f);
							if (mask.x * 0.3 + mask.y * 0.59 + mask.z * 0.11 * mask.w > _Threshold) { //check that brightness (ish) is bigger than threshold 
								color.xyz += mask.xyz * mask.w;
							}
							sampledAtLastPoint = true; //flag that we have actually taken a sample of the volume
						}
						else if (sampledAtLastPoint) { //if we stop sampling then we know that the ray has left the samplable area of the volume and we can stop the ray 
							break;
						}
					}
					pos -= step; //move forward along the ray
				}
				//scale color based on density
				color *= _Density / (uint)_SamplingQuality;

				//remove channels as desired by user
				if (_Red == 0) color.x = 0;
				if (_Green == 0) color.y = 0;
				if (_Blue == 0) color.z = 0;

				//float3 res = );

				//return the color
				return color;
			}

		ENDCG

		}
	}
}