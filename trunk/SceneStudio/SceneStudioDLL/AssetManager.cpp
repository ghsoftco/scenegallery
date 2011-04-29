#include "Main.h"

void AssetManager::Init(AppState &state)
{
    _state = &state;
}

void AssetManager::FreeMemory()
{
    for(auto meshIterator = _meshes.begin(); meshIterator != _meshes.end(); meshIterator++)
    {
        meshIterator->second->FreeMemory();
        delete meshIterator->second;
    }
    for(auto meshIterator = _pickMeshes.begin(); meshIterator != _pickMeshes.end(); meshIterator++)
    {
        meshIterator->second->FreeMemory();
        delete meshIterator->second;
    }
    for(auto textureIterator = _textures.begin(); textureIterator != _textures.end(); textureIterator++)
    {
        textureIterator->second->FreeMemory();
        delete textureIterator->second;
    }
    for(auto modelIterator = _models.begin(); modelIterator != _models.end(); modelIterator++)
    {
        delete modelIterator->second;
    }

    _meshes.clear();
    _pickMeshes.clear();
    _textures.clear();
    _models.clear();
}

void AssetManager::AddMesh(const String &name, D3D9Mesh *mesh)
{
    _meshes[name] = mesh;
}

D3D9Mesh* AssetManager::GetMesh(const String &name, const FileCollection &meshes)
{
    auto result = _meshes.find(name);
    if(result == _meshes.end())
    {
        const FileCollectionFile *file = meshes.FindFile(name);
        Assert(file != NULL, "geometry not found");

        D3D9Mesh *newMesh = new D3D9Mesh(_state->GD);
        InputDataStream stream;
        stream.WrapMemory(file->Data);
        newMesh->LoadStream(stream);
        _meshes[name] = newMesh;
        return newMesh;
    }
    else
    {
        return result->second;
    }
}

D3D9Mesh* AssetManager::GetPickMesh(const String &name)
{
    auto result = _pickMeshes.find(name);
    if(result == _pickMeshes.end())
    {
        auto startMeshResult = _meshes.find(name);
        Assert(startMeshResult != _meshes.end(), "mesh not found");
        const D3D9Mesh &startMesh = *startMeshResult->second;
        
        D3D9Mesh *newMesh = new D3D9Mesh(startMesh.GetGD());
        newMesh->Allocate(startMesh.IndexCount(), startMesh.FaceCount());

        const MeshVertex *startVertices = startMesh.Vertices();
        const DWORD *startIndices = startMesh.Indices();

        MeshVertex *newVertices = newMesh->Vertices();
        DWORD *newIndices = newMesh->Indices();
        
        const UINT faceCount = newMesh->FaceCount();
        const UINT indexCount = faceCount * 3;

        for(UINT indexIndex = 0; indexIndex < indexCount; indexIndex++)
        {
            newIndices[indexIndex] = indexIndex;
        }

        for(UINT faceIndex = 0; faceIndex < faceCount; faceIndex++)
        {
            const UINT indexBase = faceIndex * 3;
            MeshVertex &v0 = newVertices[indexBase + 0];
            MeshVertex &v1 = newVertices[indexBase + 1];
            MeshVertex &v2 = newVertices[indexBase + 2];

            *(UINT32*)(&v0.Color) = faceIndex;
            *(UINT32*)(&v1.Color) = faceIndex;
            *(UINT32*)(&v2.Color) = faceIndex;

            v0.TexCoord = Vec2f(0.0f, 0.0f);
            v1.TexCoord = Vec2f(1.0f, 0.0f);
            v2.TexCoord = Vec2f(0.0f, 1.0f);

            v0.Pos = startVertices[startIndices[indexBase + 0]].Pos;
            v1.Pos = startVertices[startIndices[indexBase + 1]].Pos;
            v2.Pos = startVertices[startIndices[indexBase + 2]].Pos;
        }

        _pickMeshes[name] = newMesh;

        return newMesh;

    }
    else
    {
        return result->second;
    }
}

D3D9Texture* AssetManager::GetTexture(const String &name)
{
    auto result = _textures.find(name);
    if(result == _textures.end())
    {
        D3D9Texture *newTexture = new D3D9Texture;
        newTexture->Init(_state->GD);

        if(SkipTextureLoading)
        {
            Bitmap bmp(2, 2);
            bmp.Clear(RGBColor::RandomColor());
            newTexture->Load(bmp);
        }
        else
        {
            newTexture->LoadFromFile(_state->parameters.textureDirectory + name, D3DFMT_DXT1);
        }
        //bmp.LoadPNG(_state->parameters.textureDirectory + name);
        
        _textures[name] = newTexture;
        return newTexture;
    }
    else
    {
        return result->second;
    }
}

Model* AssetManager::GetModel(const String &name)
{
    auto result = _models.find(name);
    if(result == _models.end())
    {
        Model *newModel = new Model;
        newModel->LoadBinary(*_state, name);
        _models[name] = newModel;
        return newModel;
    }
    else
    {
        return result->second;
    }
}
