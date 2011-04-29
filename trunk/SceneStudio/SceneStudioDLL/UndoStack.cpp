#include "Main.h"

void UndoStack::FreeMemory()
{
    _stack.DeleteMemory();
}

void UndoStack::Report(AppState &state, App &app) const
{
    app.Status(String("Undo depth: ") + String(_stack.Length()), RGBColor::Red);
    app.Status(String("Undo footprint: ") + String(double(MemoryFootprint()) / (1024.0 * 1024.0)) + String("MB"), RGBColor::Red);
}

void UndoStack::RecordAction(AppState &state, ActionType action, ModelInstance *model)
{
    if(_stack.Length() == 0 ||
       action != _stack.Last()->action ||
       model != _stack.Last()->model ||
       action == ActionInsert ||
       action == ActionDelete)
    {
        UndoStackEntry *newEntry = new UndoStackEntry;
        newEntry->action = action;
        newEntry->model = model;
        state.scene.SaveSceneBinary(state, newEntry->data);
        _stack.PushEnd(newEntry);

        if(_stack.Length() > MaxUndoDepth)
        {
            delete _stack.First();
            _stack.PopFront();
        }
    }
}

void UndoStack::Undo(AppState &state)
{
    if(_stack.Length() > 0)
    {
        state.selectedModel = NULL;

        InputDataStream stream;
        stream.WrapMemory(_stack.Last()->data.Data());
        state.scene.LoadSceneBinary(state, stream);
        delete _stack.Last();
        _stack.PopEnd();

        state.sceneDirty = true;
    }
}

UINT UndoStack::MemoryFootprint() const
{
    UINT result = 0;
    for(UINT stackIndex = 0; stackIndex < _stack.Length(); stackIndex++)
    {
        const UndoStackEntry &curEntry = *_stack[stackIndex];
        result += curEntry.data.Data().Length();
    }
    return result;
}
