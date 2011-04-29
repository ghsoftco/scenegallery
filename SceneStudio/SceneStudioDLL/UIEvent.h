enum UIEventType
{
    UIEventSaveScene, UIEventUploadScene, UIEventClearSelection, UIEventRotate, UIEventResetRotation, UIEventScale,
    UIEventFlipFace, UIEventCopy, UIEventPaste, UIEventDelete, UIEventUndo, UIEventResetCamera, UIEventHelp,
    UIEventHomepage, UIEventTextSearch, UIEventShapeSearch, UIEventModelChosen, UIEventEnterInsertMode,
    UIEventEnterNormalMode, UIEventEnterSelectingMode, UIEventEnterLookingMode, UIEventDollyCamera,
    UIEventEnterMoveSurfacesMode, UIEventEnterMovePlaneMode, UIEventEnterMoveLevitateMode, 
    UIEventMouseDown, UIEventMouseUp, UIEventMouseMove, UIEventNULL,
};

class UIEvent
{
public:
    UIEvent()
    {
        _time = -1.0f;
    }
    UIEvent(float time, UIEventType type, String data);
    UIEvent(const String &s);
    
    static UIEventType tagToType(const String &tag);
    void print(std::ostream &os) const;

private:
    static map<UIEventType, String> fillTagMap();

    static map<UIEventType, String> tagMap;
    float _time;
    UIEventType _type;
    String _data;
};

ostream& operator<<(ostream &os, const UIEvent &e);