Shader "UIEffect/UIEffect"
{
	Properties
	{
		[HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
		[HideInInspector] _ParamTex ("Parameter Texture", 2D) = "white" {}
		[Toggle(ADD)] _Add ("Add", Float) = 0
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
			Pass Keep
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Name "Default"

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#if !defined(SHADER_API_D3D11_9X) && !defined(SHADER_API_D3D9)
			#pragma target 2.0
			#else
			#pragma target 3.0
			#endif

			#pragma multi_compile_local _ ADD

		#include "UnityUI.cginc"

			#define UI_EFFECT 1
			#include "UIEffect.cginc"
			#include "UIEffectSprite.cginc"

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 param = tex2D(_ParamTex, float2(0.25, IN.eParam));
                fixed colorFactor = param.y;

				half4 color = tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd;

				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				color = ApplyColorEffect(color, fixed4(IN.color.rgb, colorFactor));
				color.a *= IN.color.a;

				return color;
			}
		ENDCG
		}
	}
}
