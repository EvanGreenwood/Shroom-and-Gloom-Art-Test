Shader "MathBad/Wireframe"
{
    Properties
    {
        _MainTex("MainTex (Base(RGB) Alpha(A))", 2D) = "white" { }

        _MainColor("MainColor", Color) = (0,0,0,1)
        _MainPower("MainPower", Float) = 1.0

        _WireColor("WireColor", Color) = (0,1,0,1)
        _WirePower("WirePower", Float) = 1.0

        _Width("Width", Float) = 4.0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma target 4.0
            #pragma vertex vert addshadow
            #pragma geometry geom
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct v2g
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            struct g2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 dist : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _MainColor, _WireColor;
            float _MainPower, _WirePower;
            float _Width;

            v2g vert(appdata_base v)
            {
                v2g o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g v[3], inout TriangleStream<g2f> triStream)
            {
                float2 WIN_SIZE = float2(_ScreenParams.x / 2.0, _ScreenParams.y / 2.0);

                // frag position
                float2 p0 = WIN_SIZE * v[0].pos.xy / v[0].pos.w;
                float2 p1 = WIN_SIZE * v[1].pos.xy / v[1].pos.w;
                float2 p2 = WIN_SIZE * v[2].pos.xy / v[2].pos.w;

                // barycentric coordinates
                float2 v0 = p2 - p1;
                float2 v1 = p2 - p0;
                float2 v2 = p1 - p0;

                // triangles area
                float area = abs(v1.x * v2.y - v1.y * v2.x);

                g2f o;
                o.pos = v[0].pos;
                o.uv = v[0].uv;
                o.dist = float3(area / length(v0), 0, 0);
                triStream.Append(o);

                o.pos = v[1].pos;
                o.uv = v[1].uv;
                o.dist = float3(0, area / length(v1), 0);
                triStream.Append(o);

                o.pos = v[2].pos;
                o.uv = v[2].uv;
                o.dist = float3(0, 0, area / length(v2));
                triStream.Append(o);
            }

            half4 frag(g2f v) : COLOR
            {
                float d = min(v.dist.x, min(v.dist.y, v.dist.z));   // distance of frag from triangles center
                float I = exp2(-1.0 * d * d * _Width);              // fade based on dist from center

                float4 tex_sample = tex2D(_MainTex, v.uv);
                float4 main_col = fixed4(tex_sample.xyz * _MainColor.xyz * _MainPower, _MainColor.w);
                float4 wire_col = fixed4(_WireColor.xyz * _WirePower, _WireColor.w);
                half4 col = lerp(main_col, wire_col, I);

                // UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}