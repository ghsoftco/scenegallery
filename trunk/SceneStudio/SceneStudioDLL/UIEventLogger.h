class UIEventLogger
{
public:
    UIEventLogger();
    void FreeMemory();
    void Reset();
    void Log(UIEventType eventType, String eventData);
    void SaveASCII(ofstream &file);
    void LoadASCII(const String &filename);

private:
    Clock _clock;
    float _timeOffset;
    Vector<UIEvent> _events;
};