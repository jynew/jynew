Shader "MyToon/Toon-Surface" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
		_Color("Main Color",color) = (1,1,1,1)
        _Ramp ("Ramp Texture", 2D) = "white" {}
        _Tooniness ("Tooniness", Range(0.1,20)) = 4
        _Outline ("Outline", Range(0,1)) = 0.4
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        CGPROGRAM
        #pragma surface surf Toon
 
        sampler2D _MainTex;
        sampler2D _Ramp;
        float _Tooniness;
        float _Outline;
		fixed4 _Color;
 
        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
        };
 
        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            
            half edge = saturate(dot (o.Normal, normalize(IN.viewDir))); 
			edge = edge < _Outline ? edge/4 : 1;
			
            o.Albedo = (floor(c.rgb * _Tooniness)/_Tooniness) * edge;
            o.Alpha = c.a;
        }
 
        half4 LightingToon(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
        {
        	float difLight = dot (s.Normal, lightDir);
        	float dif_hLambert = difLight * 0.5 + 0.5; 
        	
        	float rimLight = dot (s.Normal, viewDir);  
        	float rim_hLambert = rimLight * 0.5 + 0.5; 
        	
        	float3 ramp = tex2D(_Ramp, float2(dif_hLambert, rim_hLambert)).rgb;   
    
 			float4 c;  
            c.rgb = s.Albedo * _LightColor0.rgb * ramp * atten * 2;
            c.a = s.Alpha;
            return c;
        }
 
        ENDCG
    } 
    FallBack "Diffuse"
}
