Shader "Unlit/WaveShader"
{

	Properties
	{
		_WaveLength("Wave Length", Float) = 2.0
		_WaveMinClamp("Wave Min Clamp", Range(-1.0, 0.0)) = 0.0
		_WaveMaxClamp("Wave Max Clamp", Range(0.0, 1.0)) = 1.0
		_StepScale("Scale", Float) = 0.1
		_PeakAmplitude("Peak Amplitude", Float) = 2.0
		_LineColor("Line Color", Color) = (0.0, 1.0, 0.0, 1.0)
		_LineStrength("Line Strength", Float) = 0.1
		_BackgroundColor("Background Color", Color) = (0.0, 0.0, 0.0, 1.0)
		_SpeedMultiplier("Speed Multiplier",  Float) = 1.0
		_FadeAmount("Fade Amount", Float) = 0.12
		_FadeStrength("Fade Strength", Float) = 5.0

	}
		SubShader
	{
		Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
		LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#define pi 3.14159265
			#define p2p_time 6.0
            
			#include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 animatedUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };


			float _WaveLength;
			float _StepScale;
			float _PeakAmplitude;
			float _FadeAmount;
			float _FadeStrength;
			float _LineStrength;
			float _SpeedMultiplier;
			float _WaveMinClamp;
			float _WaveMaxClamp;

			fixed4 _LineColor;
			fixed4 _BackgroundColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                o.animatedUV = v.uv * _StepScale - (0.5 * _StepScale);
				o.animatedUV.x += (100 * _Time * _SpeedMultiplier);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				
				float mag = 1.0 - (distance(i.animatedUV.y, clamp(sin((2.0*pi/ _WaveLength) * i.animatedUV.x) * _PeakAmplitude, _WaveMinClamp * _PeakAmplitude * pi, _WaveMaxClamp * _PeakAmplitude * pi)));
				mag = pow(mag, _LineStrength);

				float alpha = distance(0.5, i.uv.x) * _FadeAmount;
				alpha = pow(alpha, _FadeStrength);

				fixed4 col = lerp(clamp(_LineColor * mag, 0 ,1.0), _BackgroundColor, alpha);

                // apply fog
                return col;
            }
            ENDCG
        }

	
    }
}
