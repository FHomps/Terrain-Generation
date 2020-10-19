Shader "TestShader" {
	SubShader{
		Pass {
			CGPROGRAM

			#pragma vertex vert             
			#pragma fragment frag

			struct vertInput {
				float4 pos : POSITION;
			};

			struct vertOutput {
				float4 pos : SV_POSITION;
				float y : HEIGHT;
			};

			vertOutput vert(vertInput input) {
				vertOutput o;
				o.pos = UnityObjectToClipPos(input.pos);
				o.y = input.pos.y;
				return o;
			}

			half4 frag(vertOutput output) : COLOR {
				float f = (output.y + 1) / 2;
				return half4(f, f, f, 1.0);
			}
			ENDCG
		}
	}
}