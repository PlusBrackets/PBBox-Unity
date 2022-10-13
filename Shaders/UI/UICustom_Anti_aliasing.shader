// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PBBox/UI/Custom_Anti-Aliasing"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1, 1, 1, 1)
        
        /* UI */
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        /* -- */

        //Saturate
        _Brightness("Brightness",Float) = 1
        _Saturation("Saturation",Float) = 1
        _Contrast("Contrast",Float) = 1
        //end
        //Anti-aliasing
        _AnitAliasingPixel("Anit-Aliasing Pixel Count",int) = 1
        _UVX_Weight("UV_x Weight",Range(0,1)) = 1
        _UVY_Weight("UV_y Weight",Range(0,1)) = 1
        _UV_Offset("UV Offset",Range(0,1)) = 0.5
        //end
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        /* UI */
        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
        /* -- */
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_UI_ALPHACLIP
            // #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityUI.cginc"
            #include "UnityCG.cginc"

            fixed4 _TextureSampleAdd; // Unity管理：图片格式用Alpha8 
            float4 _ClipRect;// Unity管理：2D剪裁使用
            sampler2D _MainTex;

            fixed4 _Color;
            half _Brightness;
            half _Saturation;
            half _Contrast;
            int _AnitAliasingPixel;
            half _UVX_Weight;
            half _UVY_Weight;
            half _UV_Offset;
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                half2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);// 实例化处理
                OUT.vertex = UnityObjectToClipPos(IN.vertex);// 模型空间到裁剪空间
                OUT.color = IN.color;
                OUT.texcoord = IN.texcoord;
                return OUT;
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord)*IN.color;
                //改变亮度
                c.rgb*= _Brightness;
                //改变饱和度
                fixed gray = 0.2125 * c.r + 0.7154*c.g + 0.0721 * c.b;
                fixed3 grayC = fixed3(gray,gray,gray);
                c.rgb = lerp(grayC,c.rgb,_Saturation);
                //end
                //改变对比度
                fixed3 avgC = fixed3(0.5,0.5,0.5);
                c.rgb = lerp(avgC,c.rgb,_Contrast);
                // c.rgb*=c.a;
                //end
                /* --------- */

                float smoothValue = 0;
                float deltaX = _AnitAliasingPixel*fwidth(IN.texcoord.x);
                float deltaY = _AnitAliasingPixel*fwidth(IN.texcoord.y);
                float vx = abs(IN.texcoord.x - _UV_Offset)/(0.5 + abs(0.5 - _UV_Offset)) + deltaX;
                float vy = abs(IN.texcoord.y - _UV_Offset)/(0.5 + abs(0.5 - _UV_Offset)) + deltaY;

                float smoothValueX = _UVX_Weight * step(1.0, vx) * (vx - 1) / deltaX;
                float smoothValueY = _UVY_Weight * step(1.0, vy) * (vy - 1) / deltaY;
                smoothValue = max(smoothValueX, smoothValueY);
                c.a *= smoothstep(c.a, 0.0f, smoothValue);
                // c.a = IN.texcoord.x;
                
                return c;
            }


            ENDCG
        }
    }
}