#include "Main.h"

UIEventLogger::UIEventLogger()
{
    _timeOffset = 0.0;
}

void UIEventLogger::FreeMemory()
{
    _events.FreeMemory();
}

void UIEventLogger::Reset()
{
    _events.FreeMemory();
    _clock = Clock();
    _timeOffset = 0.0;
}

void UIEventLogger::Log(UIEventType eventType, String eventData)
{
    _events.PushEnd(UIEvent((float)_clock.Elapsed() + _timeOffset, eventType, eventData));
}

void UIEventLogger::SaveASCII(ofstream &file)
{
    file << "qs " << String(_events.Length()) << endl;
    for (UINT eventIndex = 0; eventIndex < _events.Length(); eventIndex++)
    {
        file << "q " << _events[eventIndex] << endl;
    }
    file << "qe " << String(_clock.Elapsed() + _timeOffset) << endl;
}

void UIEventLogger::LoadASCII(const String &filename)
{
    Vector<String> lines, words;
    Utility::GetFileLines(filename, lines);
    UINT totalEntries = 0;
    String modifiers;

    _events.FreeMemory();

    for (UINT i = 0; i < lines.Length(); i++)
    {
        const String &curLine = lines[i];
        if(curLine.Length() >= 2 && curLine.StartsWith("q"))
        {
            curLine.Partition(' ', words);
            const char c = curLine[1];
            if (c == 's')
            {
                totalEntries = words[1].ConvertToUnsignedInteger();
            }
            else if (c == 'e')
            {
                _timeOffset = words[1].ConvertToFloat();
            }
            else
            {
                const String &eventLine = curLine.RemovePrefix("q ");
                eventLine.Partition('\t', words);
                modifiers = "";
                if (words.Length() < 2)
                {
                    SignalError("Invalid UIEvent log line");
                }
                if (words.Length() == 3) modifiers = words[2];
                _events.PushEnd(UIEvent(words[0].ConvertToFloat(), UIEvent::tagToType(words[1]), modifiers));
            }
        }
    }
    Assert(totalEntries == _events.Length(), "Total of UIEvent log entries does not match record");
    _clock = Clock();
}