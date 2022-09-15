Shader "Custom/HW_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Height("Height", Range(-10,10)) = 0.5
        _Distortion("Distortion", Range(0,20)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            float _Height;
            float _Distortion;

            //struct appdata
            //{
            //    float4 vertex : POSITION;
            //    float2 uv : TEXCOORD0;
            //};

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_full v)
            {
                v2f o;
                v.vertex.y += _Height; // сдвиг 
                //v.vertex.xyz += v.normal * _Height * v.texcoord.x; // подъем края

                //v.vertex.xyz += v.normal * _Height; // увеличение сферы

                //v.vertex.xyz += v.normal * _Height * v.texcoord.x * v.texcoord.x; // изгиб

                v.vertex.xyz += sin(v.normal * _Distortion * v.texcoord.x); // изгиб

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {                
                fixed4 col = tex2D(_MainTex, i.uv);                
                return col;
            }
            ENDCG
        }
    }
}
