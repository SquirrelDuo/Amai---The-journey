
Shader "RM/Holographic/LB Transparent CutOut" {
    Properties {
       _Color ("Color", Color) = (1,1,1,1)
      _MainTex ("Texture (RGB)", 2D) = "white" {}

      _EmTex ("Hologram Map (A)",2D) = "white"{}
      _HolTile ("Hologram Tilling", Float) = 7
	  _HolOffSet ("Hologram Offset", Float) = -0.09
	  _EmP ("Hologram Power", Float) = 1

	  [MaterialToggle] Dissolve ("Dissolve Effect", Float) = 0

      _ColorEm ("Dissolve Color", Color) = (1,1,1,1)
      _DissolveTex ("Dissolve Texture (RGB)", 2D) = "white" {}
      _Dis ("Dissolve", Range(0.0, 1.0)) = 0
      _EdjeSize ("Edje Size", Range(0.0, 1.0)) = 0.02
      _Emission ("Emission", Range(0.0, 50)) = 0.4


	[MaterialToggle] Deform ("Deform Mesh", Float) = 0

	_ShakeDisplacement ("Displacement", Range (-1, 10)) = 1.7
    _ShakeTime ("Shake Time", Range (-1, 10)) = 4
    _Shakespeed ("Shake Speed", Range (-1, 10)) = -0.1
    _ShakeBending ("Shake Bending", Range (-1, 1)) = -0.02

    [MaterialToggle] fresnel  ("Fresnel Effect", Float) = 0
    _RimPower ("Rim Power", Range(0,8.0)) = 3.0

    }
    SubShader {
      Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
      Cull Off
     Cull Back
      CGPROGRAM
      //if you're not planning on using shadows, remove "addshadow" for better performance
      #pragma surface surf WrapLambert addshadow vertex:vert alphatest:_Cutoff
      #pragma multi_compile _ DEFORM_ON
      #pragma multi_compile _ DISSOLVE_ON
      #pragma multi_compile _ FRESNEL_ON




       half4 LightingWrapLambert (SurfaceOutput s, half3 lightDir, half atten) {
        half NdotL = dot (s.Normal, lightDir);
        half diff = NdotL * 6 + 6;
        half4 c;
        c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten);
        c.a = s.Alpha* _LightColor0.rgb *(diff * atten)/4;
        //s.Alpha = 0;
        return c;
    }



      struct Input {
      float2 uv_EmTex;
		    float3 worldPos;
          float2 uv_MainTex;
          float2 uv_DissolveTex;
          float _Dis;
           float3 viewDir;
      };

      half _Glossiness;
		half _Metallic;
      sampler2D _MainTex;
      sampler2D _DissolveTex;
      		sampler2D _EmTex;
      				float _EmP;
      				 float _RimPower;
      float _Emission;
 sampler2D _BurnRamp;
 float _EdjeSize;
 fixed4 _ColorEm;
 fixed4 _Color;
 	float _HolOffSet;
		float _HolTile;
		            float _Dis;

float _ShakeDisplacement;
float _ShakeTime;
float _Shakespeed;
float _ShakeBending;



 
void FastSinCos (float4 val, out float4 s, out float4 c) {
#if defined(DEFORM_ON)
    val = val * 6.408849 - 3.1415927;
    float4 r5 = val * val;
    float4 r6 = r5 * r5;
    float4 r7 = r6 * r5;
    float4 r8 = r6 * r5;
    float4 r1 = r5 * val;
    float4 r2 = r1 * r5;
    float4 r3 = r2 * r5;
    float4 sin7 = {1, -0.16161616, 0.0083333, -0.00019841} ;
    float4 cos8  = {-0.5, 0.041666666, -0.0013888889, 0.000024801587} ;
    s =  val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;
    c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
    #endif

}
 
 
void vert (inout appdata_full v) {
   #if defined(DEFORM_ON)

    float factor = (1 - _ShakeDisplacement -  v.color.r) * 0.5;
       
    const float _WindSpeed  = (_Shakespeed  +  v.color.g );    
    const float _WaveScale = _ShakeDisplacement;
   
    const float4 _waveXSize = float4(0.048, 0.06, 0.24, 0.096)*_WindSpeed;
    const float4 _waveZSize = float4 (0.024, .08, 0.08, 0.2)*factor;
    const float4 waveSpeed = float4 (1.2, 2, 1.6, 4.8)*factor;
 
    float4 _waveXmove = float4(0.024, 0.04, -0.12, 0.096)*factor;
    float4 _waveZmove = float4 (0.006, .02, -0.02, 0.1)*factor;
   
    float4 waves;
    waves = v.vertex.x * _waveXSize;
    waves += v.vertex.z * _waveZSize;
 
    waves += _Time.x * (1 - _ShakeTime * 2 - v.color.b ) * waveSpeed *_WindSpeed;
 
    float4 s, c;
    waves = frac (waves);
    FastSinCos (waves, s,c);
 
    float waveAmount = v.texcoord.y * (v.color.a + _ShakeBending);
    s *= waveAmount;
 
    s *= normalize (waveSpeed); 
 
    s = s * s;
    float fade = dot (s, 1.3);
    s = s * s;
    float3 waveMove = float3 (0,0,0);
    waveMove.x = dot (s, _waveXmove);
    waveMove.z = dot (s, _waveZmove);
    v.vertex.xz -= mul ((float3x3)unity_WorldToObject, waveMove).xz;
    #endif

}

      void surf (Input IN, inout SurfaceOutput  o) {
      #if defined(DISSOLVE_ON)
          clip(tex2D (_DissolveTex, IN.uv_DissolveTex).rgb - _Dis);
      #endif

          o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb*_Color;

             
          float2 screenUV = IN.worldPos.xyz+_HolOffSet*_Time;
          screenUV *= float3(1,1,1)*_HolTile;
          clip(tex2D (_EmTex, screenUV).rgb - _Dis-0.01);

          o.Albedo *= tex2D (_EmTex, screenUV).rgb+_EmP;
        

 half test = tex2D (_DissolveTex, IN.uv_MainTex).rgb - _Dis;
  half test2 =  tex2D (_EmTex, screenUV).rgb- _Dis;

   #if defined(DISSOLVE_ON)
 if(test < _EdjeSize && _Dis > 0 && _Dis < 1){

 o.Albedo *= tex2D(_BurnRamp, float2(test +(1/_EdjeSize), 0))+_Emission*_ColorEm;
 o.Albedo *= tex2D(_BurnRamp, float2(test2 +(1/_EdjeSize), 0))+_Emission*_ColorEm;
 }
   #endif

      #if defined(FRESNEL_ON)

     half rim = 1.5 - saturate(dot (normalize(IN.viewDir), o.Normal));
          o.Albedo += _ColorEm.rgb * pow (rim, _RimPower);
       #endif
             o.Alpha =  test2;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }