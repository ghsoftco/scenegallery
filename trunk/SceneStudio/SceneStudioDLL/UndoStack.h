enum ActionType
{
    ActionInsert,
    ActionDelete,
    ActionMove,
    ActionRotate,
    ActionScale,
    ActionChangeFace,
    ActionResetRotation,
};

struct UndoStackEntry
{
    ActionType action;
    ModelInstance *model;   //only use this field to check for different models
    OutputDataStream data;
};

class App;
class UndoStack
{
public:
    void FreeMemory();
    
    void RecordAction(AppState &state, ActionType action, ModelInstance *model);
    void Undo(AppState &state);

    void Report(AppState &state, App &app) const;

private:
    UINT MemoryFootprint() const;

    Vector<UndoStackEntry*> _stack;
};
