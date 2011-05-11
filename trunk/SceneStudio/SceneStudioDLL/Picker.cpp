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

void Picker::SaveModelNameGrid(AppState &state, String filename)
{
    D3D9ProtectRenderTarget protector(state.device, true, true);
    //D3D9RenderTargetSurface &surface = state.globalAssets.pickingSurfaceRGB;
    //surface.ReSizeToBackBuffer(state.GD, D3DFMT_A8R8G8B8);
    //surface.SetAsRenderTarget(state.GD);
    //state.device->Clear( 0, NULL, D3DCLEAR_ZBUFFER | D3DCLEAR_TARGET, RGBColor(255, 255, 255, 255), 1.0f, 0 );
    //state.scene.RenderPickingA(state);
    //surface.CopySurfaceToOffScreen(state.GD);
    //auto capturedSurface = surface.OffScreenPlainSurface();

    D3D9RenderTargetSurface &surfaceScreenshotSize = state.globalAssets.pickingSurfaceUV;
    surfaceScreenshotSize.Init(state.GD, D3DFMT_A8R8G8B8, screenshotWidth, screenshotHeight);
    
    surfaceScreenshotSize.SetAsRenderTarget(state.GD);
    state.device->Clear( 0, NULL, D3DCLEAR_ZBUFFER | D3DCLEAR_TARGET, RGBColor(255, 255, 255, 255), 1.0f, 0 );
    state.scene.RenderPickingA(state);
    surfaceScreenshotSize.CopySurfaceToOffScreen(state.GD);

    auto capturedSurface2 = surfaceScreenshotSize.OffScreenPlainSurface();
    D3DTEXTUREFILTERTYPE filterPoint = D3DTEXF_POINT;
    //state.GD.GetDevice()->StretchRect(capturedSurface, NULL, capturedSurface2, NULL, filterPoint);
    //D3DXLoadSurfaceFromSurface(capturedSurface2, NULL, NULL, capturedSurface, NULL, NULL, filterPoint, 0);
    
    D3DSURFACE_DESC desc;
    capturedSurface2->GetDesc(&desc);
    
    _geometryIDGrid.Allocate(desc.Height, desc.Width);
    
    D3DLOCKED_RECT rect;
    capturedSurface2->LockRect(&rect, NULL, 0);
    for(UINT row = 0; row < desc.Height; row++)
    {
        memcpy(&_geometryIDGrid(row, 0), ((BYTE *)rect.pBits) + rect.Pitch * row, sizeof(RGBColor) * desc.Width);
    }
    capturedSurface2->UnlockRect();

    _modelNameGrid.Allocate(desc.Height, desc.Width);

    for(UINT y = 0; y < desc.Height; y++)
    {
        for(UINT x = 0; x < desc.Width; x++)
        {
            UINT geometryID = _geometryIDGrid(y, x);
            String modelName = state.scene.GetModelNameFromGeometryIndex(geometryID);
            _modelNameGrid(y, x) = modelName;
        }
    }
    _modelNameGrid.SaveToASCIIFile(filename);
    SaveModelNameGridAsBitmap(filename + String(".png"));
}

void Picker::SaveModelNameGridAsBitmap(String filename)
{
    const UINT width = _modelNameGrid.Cols();
    const UINT height = _modelNameGrid.Rows();
    
    Bitmap bmp(width, height);
    map<UINT, RGBColor> colorMap;

    for(UINT y = 0; y < height; y++)
    {
        for(UINT x = 0; x < width; x++)
        {
            String id = _modelNameGrid(y, x);
            if(colorMap.find(id.Hash32()) == colorMap.end())
            {
                colorMap[id.Hash32()] = RGBColor::RandomColor();
            }
            bmp[y][x] = colorMap[id.Hash32()];
        }
    }
    bmp.FlipVertical();
    bmp.SavePNG(filename);
}