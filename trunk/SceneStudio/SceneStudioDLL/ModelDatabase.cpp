#include "Main.h"

void Sanitize(String &s)
{
    for(UINT characterIndex = 0; characterIndex < s.Length(); characterIndex++)
    {
        char &c = s[characterIndex];
        if(c >= 'A' && c <= 'Z')
        {
            c = c - 'A' + 'a';
        }
        if(!(c >= 'a' && c <= 'z'))
        {
            c = ' ';
        }
    }
}

void ModelDatabase::Init(AppState &state)
{
    LoadArchitecture(state);
    LoadValidScenes(state);
    LoadSceneNames(state);
    LoadSceneTags(state);
    LoadDescriptorSet(state);
    
    if(runningLocalDirectoryTasks)
    {
        GenerateModelTextureList(state);
    }
}

void ModelDatabase::AddModelTextureList(AppState &state, const String &modelName, ofstream &file) const
{
    Vector<UINT64> textures;
    Model::MakeTextureList(state, modelName, textures);
    file << modelName;
    for(UINT textureIndex = 0; textureIndex < textures.Length(); textureIndex++)
    {
        file << ' ' << String::Hash64ToHexString(textures[textureIndex]);
    }
    file << endl;
}

void ModelDatabase::GenerateModelTextureList(AppState &state) const
{
    Console::WriteLine("Generating model texture lists...");
    ofstream file( (state.parameters.baseAssetDirectory + String("SceneTextures.txt")).CString() );

    for(auto modelIterator = _database.begin(); modelIterator != _database.end(); modelIterator++)
    {
        const String &modelName = modelIterator->first;
        AddModelTextureList(state, modelName, file);
    }
    for(UINT architectureIndex = 0; architectureIndex < _architecture.Length(); architectureIndex++)
    {
        AddModelTextureList(state, _architecture[architectureIndex]->name, file);
    }
}

void ModelDatabase::LoadSceneTags(AppState &state)
{
    Vector<String> lines;
    Utility::GetFileLines(state.parameters.baseAssetDirectory + String("SceneTags.txt"), lines);
    for(UINT lineIndex = 0; lineIndex < lines.Length(); lineIndex++)
    {
        const String &curLine = lines[lineIndex];
        if(curLine.Length() > 5)
        {
            Vector<String> words;
            curLine.Partition('\t', words);
            if(words.Length() >= 2)
            {
                ModelInfo* curInfo = GetModelInfo(words[0]);
                /*if(curInfo == NULL)
                {
                    curInfo = new ModelInfo(words[0]);
                    curInfo->name = "*";
                    _database[words[0]] = curInfo;
                }*/
                if(curInfo != NULL)
                {
                    for(UINT tagIndex = 0; tagIndex < words.Length() - 1; tagIndex++)
                    {
                        curInfo->tags.PushEnd(words[tagIndex + 1]);
                    }
                }
            }
        }
    }
}

void ModelDatabase::LoadValidScenes(AppState &state)
{
    if(runningLocalDirectoryTasks)
    {
        Directory dir(state.parameters.baseAssetDirectory + String("Models\\"));
        for(UINT fileIndex = 0; fileIndex < dir.Files().Length(); fileIndex++)
        {
            const String &curFile = dir.Files()[fileIndex];
            if(curFile.Length() > 4)
            {
                _validScenes.insert(curFile);
            }
        }
    }
    else
    {
        Vector<String> lines;
        Utility::GetFileLines(state.parameters.baseAssetDirectory + String("SceneTextures.txt"), lines);
        for(UINT lineIndex = 0; lineIndex < lines.Length(); lineIndex++)
        {
            const String &curLine = lines[lineIndex];
            if(curLine.Length() > 5)
            {
                Vector<String> words;
                curLine.Partition(' ', words);
                if(words.Length() >= 1)
                {
                    const String &sceneName = words[0];
                    _validScenes.insert(sceneName);
                }
            }
        }
    }
}

void ModelDatabase::LoadArchitecture(AppState &state)
{
    Vector<String> lines;
    Utility::GetFileLines(state.parameters.baseAssetDirectory + String("ArchitectureList.txt"), lines);
    for(UINT lineIndex = 0; lineIndex < lines.Length(); lineIndex++)
    {
        const String &curLine = lines[lineIndex];
        if(curLine.Length() > 5)
        {
            Vector<String> words;
            curLine.Partition('\t', words);
            if(words.Length() == 2)
            {
                ArchitectureInfo *curInfo = new ArchitectureInfo;
                curInfo->name = words[0];
                curInfo->camera = words[1];
                _architecture.PushEnd(curInfo);
            }
        }
    }
}

void ModelDatabase::LoadSceneNames(AppState &state)
{
    //
    // This is a cache to avoid calling Utility::FileExists for every model in the database
    //
    //set<String, String::LexicographicComparison> allModelFiles;
    //Directory modelDirectory(state.parameters.modelDirectory);
    //for(UINT fileIndex = 0; fileIndex < modelDirectory.Files().Length(); fileIndex++)
    //{
    //    allModelFiles.insert(modelDirectory.Files()[fileIndex]);
    //}

    Vector<String> lines;
    Utility::GetFileLines(state.parameters.baseAssetDirectory + String("SceneNames.txt"), lines);
    for(UINT lineIndex = 0; lineIndex < lines.Length(); lineIndex++)
    {
        const String &curLine = lines[lineIndex];
        if(curLine.Length() > 5)
        {
            Vector<String> words;
            curLine.Partition('\t', words);
            if(words.Length() == 2)
            {
                const String &sceneName = words[0];
                ModelInfo* curInfo = GetModelInfo(sceneName);
                //if(curInfo == NULL && Utility::FileExists(state.parameters.modelDirectory + sceneName) ) // && allModelFiles.count(sceneName) > 0
                if(curInfo == NULL && _validScenes.count(sceneName) > 0)
                {
                    curInfo = new ModelInfo(sceneName);
                    _database[sceneName] = curInfo;
                }
                if(curInfo != NULL)
                {
                    curInfo->name = words[1];
                    Sanitize(curInfo->name);
                    curInfo->name.Partition(' ', curInfo->nameWords);
                }
            }
        }
    }
}

void ModelDatabase::LoadDescriptorSet(AppState &state)
{
    InputDataStream stream;
    stream.LoadFromFile(state.parameters.baseAssetDirectory + String("DescriptorSet.dat"));
    UINT modelCount, descriptorLength, K;
    stream >> modelCount >> descriptorLength >> K;
    Vector<ModelInfo*> allModels(modelCount);
    for(UINT modelIndex = 0; modelIndex < modelCount; modelIndex++)
    {
        String modelName;
        stream >> modelName;
        allModels[modelIndex] = GetModelInfo(modelName);
    }
    for(UINT modelIndex = 0; modelIndex < modelCount; modelIndex++)
    {
        if(allModels[modelIndex] == NULL)
        {
            for(UINT indexIndex = 0; indexIndex < K; indexIndex++)
            {
                unsigned __int16 shortIndex;
                stream >> shortIndex;
            }
        }
        else
        {
            ModelInfo &curModel = *allModels[modelIndex];
            curModel.similarGeometry.Allocate(K);
            for(UINT indexIndex = 0; indexIndex < K; indexIndex++)
            {
                unsigned __int16 shortIndex;
                stream >> shortIndex;
                curModel.similarGeometry[indexIndex] = allModels[shortIndex];
            }
        }
    }
}

ModelInfo* ModelDatabase::GetModelInfo(const String &scene) const
{
    const auto iterator = _database.find(scene);
    if(iterator == _database.end())
    {
        return NULL;
    }
    else
    {
        return iterator->second;
    }
}

ArchitectureInfo* ModelDatabase::GetArchitectureInfo(const String &name) const
{
    for(UINT architectureIndex = 0; architectureIndex < _architecture.Length(); architectureIndex++)
    {
        if(_architecture[architectureIndex]->name == name)
        {
            return _architecture[architectureIndex];
        }
    }
    return NULL;
}

float CompareSearchWords(const String &queryString, const String &modelString)
{
    if(queryString == modelString)
    {
        return 1.0f;
    }
    if(modelString.Contains(queryString))
    {
        return 0.5f;
    }
    UINT charactersInCommon = 0;
    const UINT minLength = Math::Min(queryString.Length(), modelString.Length());
    for(UINT characterIndex = 0; characterIndex < minLength; characterIndex++)
    {
        if(queryString[characterIndex] == modelString[characterIndex])
        {
            charactersInCommon++;
        }
    }
    if(charactersInCommon <= 4)
    {
        return 0;
    }
    else
    {
        return 0.1f * float(charactersInCommon - 3) / float(minLength - 4);
    }
}

struct QueryEntry
{
    ModelInfo *model;
    double value;
};

bool operator < (const QueryEntry &a, const QueryEntry &b)
{
    return (a.value > b.value);
}

void ModelDatabase::TextSearch(const String &query, String &result)
{
    Vector<String> queryWords;

    String sanitizedQuery = query;
    Sanitize(sanitizedQuery);

    sanitizedQuery.Partition(' ', queryWords);

    Vector<QueryEntry> allEntries;
    for(auto modelIterator = _database.begin(); modelIterator != _database.end(); modelIterator++)
    {
        ModelInfo &curModel = *modelIterator->second;
        
        QueryEntry curEntry;
        curEntry.model = &curModel;
        
        curEntry.value = 20.0f * CompareSearchWords(sanitizedQuery, curModel.name);
        if(queryWords.Length() > 1)
        {
            for(UINT queryWordIndex = 0; queryWordIndex < queryWords.Length(); queryWordIndex++)
            {
                curEntry.value += 6.0f * CompareSearchWords(queryWords[queryWordIndex], curModel.name);
            }
        }

        if(curModel.nameWords.Length() > 1)
        {
            for(UINT nameWordIndex = 0; nameWordIndex < curModel.nameWords.Length(); nameWordIndex++)
            {
                for(UINT queryWordIndex = 0; queryWordIndex < queryWords.Length(); queryWordIndex++)
                {
                    curEntry.value += 3.0f * CompareSearchWords(queryWords[queryWordIndex], curModel.nameWords[nameWordIndex]);
                }
            }
        }
        
        for(UINT tagIndex = 0; tagIndex < curModel.tags.Length(); tagIndex++)
        {
            for(UINT queryWordIndex = 0; queryWordIndex < queryWords.Length(); queryWordIndex++)
            {
                curEntry.value += CompareSearchWords(queryWords[queryWordIndex], curModel.tags[tagIndex]);
            }
        }
        
        curEntry.value -= 1e-5 * double(curModel.name.Length());
        curEntry.value -= 1e-3 * curModel.tags.Length();

        if(curEntry.value > 0.0)
        {
            allEntries.PushEnd(curEntry);
        }
    }

    allEntries.Sort();
    if(allEntries.Length() > 512)
    {
        allEntries.ReSize(512);
    }
    
    result.FreeMemory();
    for(UINT entryIndex = 0; entryIndex < allEntries.Length(); entryIndex++)
    {
        result += allEntries[entryIndex].model->hash;
        if(entryIndex != allEntries.Length() - 1)
        {
            result.PushEnd(' ');
        }
    }
}

void ModelDatabase::ShapeSearch(const String &query, String &result)
{
    ModelInfo *curInfo = GetModelInfo(query);
    if(curInfo == NULL || curInfo->similarGeometry.Length() == 0)
    {
        result = query;
    }
    else
    {
        result.FreeMemory();
        const UINT count = curInfo->similarGeometry.Length();
        for(UINT entryIndex = 0; entryIndex < count; entryIndex++)
        {
            result += curInfo->similarGeometry[entryIndex]->hash;
            if(entryIndex != count - 1)
            {
                result.PushEnd(' ');
            }
        }
    }
}
