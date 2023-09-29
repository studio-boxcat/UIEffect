#ifndef UI_EFFECT_INCLUDED
#define UI_EFFECT_INCLUDED


sampler2D _ParamTex;

// Unpack float to low-precision [0-1] half2.
half2 UnpackToVec2(float value)
{
	const int PACKER_STEP = 4096;
	half2 unpacked;

	unpacked.x = (value % (PACKER_STEP)) / (PACKER_STEP - 1);
	value = floor(value / (PACKER_STEP));

	unpacked.y = (value % PACKER_STEP) / (PACKER_STEP - 1);
	return unpacked;
}

// Apply color effect.
fixed4 ApplyColorEffect(half4 color, half4 factor)
{
	#if ADD
	color.rgb += factor.rgb * factor.a;
	return color;
	#else
	color.rgb = lerp(color.rgb, factor.rgb, factor.a);
	return color;
	#endif
}

// Apply shiny effect.
half4 ApplyShinyEffect(half4 color, half2 shinyParam)
{
	fixed nomalizedPos = shinyParam.x;
	fixed4 param1 = tex2D(_ParamTex, float2(0.25, shinyParam.y));
	fixed4 param2 = tex2D(_ParamTex, float2(0.75, shinyParam.y));
	half location = param1.x * 2 - 0.5;
	fixed width = param1.y;
	fixed soft = param1.z;
	fixed brightness = param1.w;
	fixed gloss = param2.x;
	half normalized = 1 - saturate(abs((nomalizedPos - location) / width));
	half shinePower = smoothstep(0, soft, normalized);
	half3 reflectColor = lerp(fixed3(1,1,1), color.rgb * 7, gloss);

	color.rgb += color.a * (shinePower / 2) * brightness * reflectColor;


	return color;
}

#endif // UI_EFFECT_INCLUDED
