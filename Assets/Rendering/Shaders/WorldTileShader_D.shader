Shader "Custom/WorldTileShader" {
    Properties{
        [Header(General)]
        _MainTex("Texture Atlas", 2D) = "white" {}
        [ShowAsVector2] _AtlasSize("Atlas Size Columns/Rows (int)", Vector) = (4, 4, 0, 0)

        [Header(Instanced)]
        _Color("Border Color", Color) = (1,1,1,1)
        _TexIndex("Sprite Index (int)", int) = 0


    }
        SubShader{
            Tags { "RenderType" = "Opaque"}
            Cull Back
            LOD 100

            CGPROGRAM
            #pragma target 3.0
            #pragma multi_compile_instancing  
            #pragma surface surf Standard

            struct Input
            {
                float2 uv_MainTex;
            };

            sampler2D _MainTex;
            float2 _AtlasSize;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            #define _TexIndex_prop Props
            UNITY_DEFINE_INSTANCED_PROP(int, _TexIndex)
            #define _TexIndex_prop Props
            UNITY_INSTANCING_BUFFER_END(Props)

            void surf(Input i, inout SurfaceOutputStandard o) {
                float2 uv = i.uv_MainTex;
                uv.x = uv.x / _AtlasSize.x;
                uv.y = uv.y / _AtlasSize.y;

                float4 c = tex2D(_MainTex, uv);

                o.Albedo = c.rgb;
                o.Alpha = 1;
            }



            ENDCG
        }
        FallBack "Diffuse"
}
