Shader "Car Paint" {
    Properties {  
      _MainTex ("Texture", 2D) = "white" {} 
      _Cube ("Cubemap", CUBE) = "" {}
      _RimPower ("Rim Power", Range(0.1, 10.0)) = 3.0  
      _RimColor ("Rim Color", Color) = (0.65, 0.85, 0.85, 1.0) 
      _Color ("Main Color", Color) = (1,1,1,1) 
	  _ColorBoost("Color Boost", Range(0.0, 5.0)) = 1.35  
	  _BlendAmount("Blend Amount", Range(0.0, 1.0)) = 1.0  
    }  
    SubShader {  
      Tags { "RenderType" = "Opaque" }   
      CGPROGRAM   
      #pragma surface surf Lambert 
 		fixed _BlendAmount; 
 	    fixed _ColorBoost; 

      struct Input
      {  
		  float2 uv_MainTex; 
          float3 worldRefl;
          float3 viewDir;
          float3 color : COLOR; 
      };  
      sampler2D _MainTex;   
      samplerCUBE _Cube;
      float _RimPower;
      float4 _RimColor; 
      float4 _Color;
      
      void surf (Input IN, inout SurfaceOutput o) {  
      
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex); 
		half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));    	  	
		o.Albedo = lerp( c.rgb / 1.25, _Color.rgb * c.rgb * _ColorBoost, c.a * _BlendAmount);                     
		o.Emission = texCUBE (_Cube, IN.worldRefl).rgb * _RimColor.rgb * 2 * pow (rim, _RimPower);      
      }  
      ENDCG  
    }   
    Fallback "Diffuse"  
}


