class Picker
{
public:
    void PickGeometry(AppState &state, UINT x, UINT y);
    void PickTriangle(AppState &state, UINT x, UINT y);
    
    //void SaveDebugBitmaps();

    /*__forceinline const Grid<UINT>& geometryIDGrid()
    {
        return _geometryIDGrid;
    }
    __forceinline const Grid<UINT>& faceIDGrid()
    {
        return _faceIDGrid;
    }
    __forceinline const Grid<Vec2f>& uvGrid()
    {
        return _uvGrid;
    }*/
    
private:
    //Grid<UINT> _geometryIDGrid;
    //Grid<UINT> _faceIDGrid;
    //Grid<Vec2f> _uvGrid;
};
