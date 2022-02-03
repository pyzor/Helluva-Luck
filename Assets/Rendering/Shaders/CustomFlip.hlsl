#ifndef CUSTOM_FLIP_INCLUDED
#define CUSTOM_FLIP_INCLUDED

void CustomFlip_float(float2 UV, float2 Size, float Index, bool FlipX, out float2 Out){
	
	Index = fmod(Index, Size.x * Size.y);
	float2 tileCount = float2(1.0 / Size.x, 1.0 / Size.y);
	//float tileY = abs(Invert.y * Size.y - (floor(Index * tileCount.x) + Invert.y * 1));
	//float tileX = abs(Invert.x * Size.x - ((Index - Size.x * floor(Index * tileCount.x)) + Invert.x * 1));

	//float tileY = floor(Index * tileCount.x);
	//float tileX = Index - Size.x * floor(Index * tileCount.x);
	 
	float tileY = abs(Size.y - (floor(Index * tileCount.x) + 1));
	float tileX = Index - Size.x * floor(Index * tileCount.x);
	
	Out = (UV + float2(tileX, tileY)) * tileCount;

	//Out = UV;
	//if(FlipX) {
	//	float offsetX = fmod(Index, Size.x) * tileCount.x;
	//	Out.x = (tileCount.x - (Out.x - offsetX)) + offsetX;
	//}
	
	//float2 offset = float2(1/Size.x, 1/Size.y);
	//
	//if(FlipX) {
	//	UV = float2(-UV.x, UV.y);
	//}
	//float2 uv = float2(UV.x / Size.x, UV.y / Size.y);
	//float index = Index % (Size.x * Size.y);
	//float rowIndex;
	//modf(index / Size.y, rowIndex);
	//
	//float2 tile = float2(index * offset.x, 1 - offset.y);
	//tile.x -= rowIndex * offset.x * Size.x;
	//tile.y -= rowIndex * offset.y;
	//
	//Out = uv + tile;
	////if(FlipX) {
	////	Out.x = ((offset.x + tile.x) - Out.x) + tile.x;
	////}

	/*
	
	float2 _Flipbook_Invert = float2(FlipX, FlipY);

	void Unity_Flipbook_float(float2 UV, float Width, float Height, float Tile, float2 Invert, out float2 Out)
	{
	    Tile = fmod(Tile, Width * Height);
	    float2 tileCount = float2(1.0, 1.0) / float2(Width, Height);
	    float tileY = abs(Invert.y * Height - (floor(Tile * tileCount.x) + Invert.y * 1));
	    float tileX = abs(Invert.x * Width - ((Tile - Width * floor(Tile * tileCount.x)) + Invert.x * 1));
	    Out = (UV + float2(tileX, tileY)) * tileCount;
	}
	
	*/


}


#endif







