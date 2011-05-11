class Picker
{
public:
    void PickGeometry(AppState &state, UINT x, UINT y);
    void PickTriangle(AppState &state, UINT x, UINT y);
    
    void SaveModelNameGrid(AppState &state, String filename);
    void SaveModelNameGridAsBitmap(String filename);

    __forceinline const Grid<UINT>& geometryIDGrid()
    {
        return _geometryIDGrid;
    }
    
private:
    Grid<UINT> _geometryIDGrid;
    Grid<String> _modelNameGrid;
};
