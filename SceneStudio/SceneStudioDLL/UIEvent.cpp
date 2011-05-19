#include "Main.h"

// Initialize static tag map
map<UIEventType, String> UIEvent::fillTagMap()
{
    map<UIEventType, String> m;
    m[UIEventSaveScene]                 = String("sa");
    m[UIEventUploadScene]               = String("us");
    m[UIEventClearSelection]            = String("cs");
    m[UIEventRotate]                    = String("ro");
    m[UIEventResetRotation]             = String("rr");
    m[UIEventScale]                     = String("sc");
    m[UIEventFlipFace]                  = String("ff");
    m[UIEventCopy]                      = String("co");
    m[UIEventPaste]                     = String("pa");
    m[UIEventDelete]                    = String("de");
    m[UIEventUndo]                      = String("un");
    m[UIEventResetCamera]               = String("rc");
    m[UIEventHelp]                      = String("he");
    m[UIEventHomepage]                  = String("ho");
    m[UIEventTextSearch]                = String("tq");
    m[UIEventShapeSearch]               = String("sq");
    m[UIEventModelChosen]               = String("cm");
    m[UIEventEnterInsertMode]           = String("in");
    m[UIEventEnterNormalMode]           = String("--");
    m[UIEventEnterSelectingMode]        = String("se");
    m[UIEventEnterLookingMode]          = String("lo");
    m[UIEventDollyCamera]               = String("do");
    m[UIEventEnterMoveSurfacesMode]     = String("ms");
    m[UIEventEnterMovePlaneMode]        = String("mp");
    m[UIEventEnterMoveLevitateMode]     = String("ml");
    m[UIEventMouseDown]                 = String("Md");
    m[UIEventMouseUp]                   = String("Mu");
    m[UIEventMouseMove]                 = String("Mm");
    // Interface side events (need to match definition in Common.cs)
    m[UIEventKeywordSearchExemplar]     = String("Se"); 
    m[UIEventKeywordSearchModel]        = String("Sm");
    m[UIEventClickExemplar]             = String("Ce"); 
    m[UIEventClickModel]                = String("Cm");

    return m;
}
map<UIEventType, String> UIEvent::tagMap = UIEvent::fillTagMap();

ostream& operator<<(ostream &os, const UIEvent &e)
{
    e.print(os);
    return os;
}

UIEvent::UIEvent(float time, UIEventType type, String data)
{
    _time = time;
    _type = type;
    _data = data;
}

UIEvent::UIEvent(const String &s)
{
    Vector<String> words;
    s.Partition('\t', words);
    if (words.Length() != 3)
    {
        SignalError("Tried to parse invalid UIEvent String");
    }
    else
    {
        _time = words[0].ConvertToFloat();
        _type = tagToType(words[1]);
        PersistentAssert(_type != UIEventNULL, "Unknown UIEvent tag encountered");
        _data = words[2];
    }
}

void UIEvent::print(std::ostream &os) const
{
    os << String(_time) << "\t" << tagMap[_type] << "\t" << _data;
}

UIEventType UIEvent::tagToType(const String &tag)
{
    UIEventType type = UIEventNULL;
    for (auto it = tagMap.begin(); it != tagMap.end(); ++it)
    {
        if (it->second == tag) type = it->first;
    }
    return type;
}