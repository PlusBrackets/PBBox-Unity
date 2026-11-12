/*--------------------------------------------------------
 *Copyright (c) 2016-2026 PlusBrackets
 *@update: 2026.11.13
 *@author: PlusBrackets
 --------------------------------------------------------*/

namespace PBBox.Event
{
    public interface IEventSubjectManager
    {
        IEventSubject GetSubject<TEvent>(bool isLater = false, bool isInParamHandler = false);
        IEventSubject GetSubject(int eventId, bool isLater = false);
    }
}