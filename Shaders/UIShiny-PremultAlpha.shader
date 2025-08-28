Shader "UIEffect/UIShiny-PremultAlpha"
{
	Properties
	{
		[HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
		[HideInInspector] _ParamTex ("Parameter Texture", 2D) = "white" {}
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
		Blend One OneMinusSrcAlpha

		Pass
		{
			Name "Default"

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#define UI_SHINY 1
			#include "UIEffect.cginc"
			#include "UIEffectSprite.cginc"

			fixed4 frag(v2f IN) : SV_Target
			{
                half4 color = IN.color * (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                // XXX: alpha 뿐만 아니라 rgb 값도 함께 감쇄시킴.
                // color.a *= m.x * m.y;
                color *= m.x * m.y;
                #endif

                // XXX: 텍스쳐 샘플링을 제외한 인풋 버텍스 알파만을 곱해서 감쇄시킴.
                // color.rgb *= color.a;
                color.rgb *= IN.color.a;

				color = ApplyShinyEffect(color, IN.eParam);

				return color;
			}
		ENDCG
		}
	}
}
