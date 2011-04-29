#include "Main.h"

void Picker::PickGeometry(AppState &state, UINT x, UINT y)
{
    if(int(x) >= state.dimensions.x || int(y) >= state.dimensions.y)
    {
        return;
    }

    D3D9ProtectRenderTarget protector(state.device, true, true);

    D3D9RenderTargetSurface &surface = state.globalAssets.pickingSurfaceRGB;
    
    //surface.ReSizeToBackBuffer(state.GD, D3DFMT_A8R8G8B8);
    if(surface.Width() != 2)
    {
        surface.Init(state.GD, D3DFMT_A8R8G8B8, 2, 2);
    }
    surface.SetAsRenderTarget(state.GD);

    state.device->Clear( 0, NULL, D3DCLEAR_ZBUFFER | D3DCLEAR_TARGET, RGBColor(255, 255, 255, 255), 1.0f, 0 );
    
    Vec3f pixelCenter = Vec3f(Math::LinearMap(0, float(state.dimensions.x) - 1.0f, -1.0f, 1.0f, float(x) + 0.5f),
                              Math::LinearMap(0, float(state.dimensions.y) - 1.0f, 1.0f, -1.0f, float(y) + 0.5f),
                              0.0f);
    const float scaleFactor = 10000.0f;

    Matrix4 viewProj = state.viewProjection;
    Matrix4 m = Matrix4::Translation(-pixelCenter) * Matrix4::Scaling(Vec3f(scaleFactor, scaleFactor, 1.0f));
    state.viewProjection = state.viewProjection * m;
    state.scene.RenderPickingA(state);
    state.viewProjection = viewProj;

    surface.CopySurfaceToOffScreen(state.GD);

    auto capturedSurface = surface.OffScreenPlainSurface();

    D3DSURFACE_DESC desc;
    capturedSurface->GetDesc(&desc);
    
    D3DLOCKED_RECT rect;
    capturedSurface->LockRect(&rect, NULL, 0);
    BYTE* base = ((BYTE *)rect.pBits);
    state.selectedGeometryIndex = *(UINT32*)base;
    capturedSurface->UnlockRect();
}

void Picker::PickTriangle(AppState &state, UINT x, UINT y)
{
    if(int(x) >= state.dimensions.x || int(y) >= state.dimensions.y || state.selectedGeometryIndex == 0xFFFFFFFF)
    {
        return;
    }

    D3D9ProtectRenderTarget protector(state.device, true, true);

    D3D9RenderTargetSurface &surfaceRGB = state.globalAssets.pickingSurfaceRGB;
    D3D9RenderTargetSurface &surfaceUV = state.globalAssets.pickingSurfaceUV;
    
    if(surfaceRGB.Width() != 2)
    {
        surfaceRGB.Init(state.GD, D3DFMT_A8R8G8B8, 2, 2);
    }
    if(surfaceUV.Width() != 2)
    {
        surfaceUV.Init(state.GD, D3DFMT_G32R32F, 2, 2);
    }
    //surfaceRGB.ReSizeToBackBuffer(state.GD, D3DFMT_A8R8G8B8);
    //surfaceUV.ReSizeToBackBuffer(state.GD, D3DFMT_G32R32F);

    surfaceRGB.SetAsRenderTarget(state.GD, 0);
    surfaceUV.SetAsRenderTarget(state.GD, 1);

    state.device->Clear( 0, NULL, D3DCLEAR_ZBUFFER | D3DCLEAR_TARGET, RGBColor(255, 255, 255, 255), 1.0f, 0 );
    
    Vec3f pixelCenter = Vec3f(Math::LinearMap(0, float(state.dimensions.x) - 1.0f, -1.0f, 1.0f, float(x) + 0.5f),
                              Math::LinearMap(0, float(state.dimensions.y) - 1.0f, 1.0f, -1.0f, float(y) + 0.5f),
                              0.0f);
    const float scaleFactor = 10000.0f;

    Matrix4 viewProj = state.viewProjection;
    Matrix4 m = Matrix4::Translation(-pixelCenter) * Matrix4::Scaling(Vec3f(scaleFactor, scaleFactor, 1.0f));
    state.viewProjection = state.viewProjection * m;
    state.scene.RenderPickingB(state);
    state.viewProjection = viewProj;

    state.device->SetRenderTarget(1, NULL);

    surfaceRGB.CopySurfaceToOffScreen(state.GD);
    surfaceUV.CopySurfaceToOffScreen(state.GD);

    auto capturedSurfaceRGB = surfaceRGB.OffScreenPlainSurface();
    auto capturedSurfaceUV = surfaceUV.OffScreenPlainSurface();

    D3DLOCKED_RECT rect;
    capturedSurfaceRGB->LockRect(&rect, NULL, 0);
            
    BYTE* base = ((BYTE *)rect.pBits);
    state.selectedFaceIndex = *(UINT32*)base;

    capturedSurfaceRGB->UnlockRect();

    capturedSurfaceUV->LockRect(&rect, NULL, 0);
            
    base = ((BYTE *)rect.pBits);
    state.selectedUV = *(Vec2f*)base;
            
    capturedSurfaceUV->UnlockRect();
}

/*void Picker::SaveDebugBitmaps()
{
    const UINT width = _geometryIDGrid.Cols();
    const UINT height = _geometryIDGrid.Rows();
    
    Bitmap bmpA(width, height);
    Bitmap bmpB(width, height);
    Bitmap bmpC(width, height);

    bmpC.Clear(RGBColor::Black);
    
    map<UINT, RGBColor> colorMapA;
    map<UINT, RGBColor> colorMapB;
    for(UINT y = 0; y < height; y++)
    {
        for(UINT x = 0; x < width; x++)
        {
            UINT geometryID = _geometryIDGrid.Cell(y, x);
            if(colorMapA.find(geometryID) == colorMapA.end())
            {
                colorMapA[geometryID] = RGBColor::RandomColor();
            }
            bmpA[y][x] = colorMapA[geometryID];
            
            UINT triangleID = _faceIDGrid.Cell(y, x);
            if(colorMapB.find(triangleID) == colorMapB.end())
            {
                colorMapB[triangleID] = RGBColor::RandomColor();
            }
            bmpB[y][x] = colorMapB[triangleID];

            Vec2f UV = _uvGrid.Cell(y, x);
            bmpC[y][x] = RGBColor(Vec3f(UV, 0.0f));
        }
    }

    bmpA.SavePNG("GeometryIDs.png");
    bmpB.SavePNG("TriangleIDs.png");
    bmpC.SavePNG("UVs.png");
}*/
