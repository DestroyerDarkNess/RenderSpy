Texture2D Tex;
SamplerState sam
{
	Filter = MIN_MAG_MIP_LINEAR;
	AddressU  = Wrap;
	AddressV  = Wrap;
};
							
struct VS_IN
{
	float2 TexCoord		: TEXCOORD;
	float4 Color		: COLOR;
	float2 TexCoordSize	: TEXCOORDSIZE;
	float2 TopLeft		: TOPLEFT;
	float2 TopRight		: TOPRIGHT;
	float2 BottomLeft	: BOTTOMLEFT;
	float2 BottomRight	: BOTTOMRIGHT;
};

struct GS_OUT
{
	float2 TexCoord : TEXCOORD;
	float4 Color	: COLOR;
	float4 Position : SV_POSITION;
};

struct PS_IN
{
	float2 TexCoord : TEXCOORD;
	float4 Color	: COLOR;
};

DepthStencilState DisableDepth
{
    DepthEnable = FALSE;
    DepthWriteMask = ZERO;
};

[maxvertexcount(4)]
void mainGS( point VS_IN input[1], inout TriangleStream<GS_OUT> TriStream )
{
	/*

	0 -- 1
	|  / |
	| /  |
	2 -- 3

	*/

	GS_OUT v;
	v.Color = input[0].Color;
	v.Position = float4(input[0].TopLeft, 0, 1);
	v.TexCoord = input[0].TexCoord;
	TriStream.Append(v);

	v.Position = float4(input[0].TopRight, 0, 1);
	v.TexCoord.x += input[0].TexCoordSize.x;
	TriStream.Append(v);

	v.Position = float4(input[0].BottomLeft, 0, 1);
	v.TexCoord.x = input[0].TexCoord.x;
	v.TexCoord.y += input[0].TexCoordSize.y;
	TriStream.Append(v);

	v.Position = float4(input[0].BottomRight, 0, 1);
	v.TexCoord.x += input[0].TexCoordSize.x;
	TriStream.Append(v);

	TriStream.RestartStrip();
}

VS_IN mainVS(VS_IN vs_in){
	return vs_in;
}			

float4 mainPS(PS_IN ps_in) : SV_TARGET 
{
	return Tex.Sample(sam, ps_in.TexCoord) * ps_in.Color;
}

float4 mainPremultipliedPS(PS_IN ps_in) : SV_TARGET
{
	float4 texel = Tex.Sample(sam, ps_in.TexCoord);
	texel.rgb /= texel.a;
	return texel  * ps_in.Color;
}

technique10 Render 
{
	pass p0 
	{	
		SetVertexShader		( CompileShader( vs_4_0 , mainVS() ) );
		SetGeometryShader	( CompileShader( gs_4_0 , mainGS() ) );
		SetPixelShader		( CompileShader( ps_4_0 , mainPS() ) );

		//SetDepthStencilState( DisableDepth, 0 );
	}
}

technique10 RenderPremultiplied
{
	pass p0
	{
		SetVertexShader(CompileShader(vs_4_0, mainVS()));
		SetGeometryShader(CompileShader(gs_4_0, mainGS()));
		SetPixelShader(CompileShader(ps_4_0, mainPremultipliedPS()));
	}
}