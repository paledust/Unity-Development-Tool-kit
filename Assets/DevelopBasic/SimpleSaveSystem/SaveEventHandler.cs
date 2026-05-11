using System;

public static class SaveEventHandler
{
    public static event Action E_OnBeginSave;
    public static void Call_OnBeginSave()=>E_OnBeginSave?.Invoke();
    public static event Action E_OnCompleteSave;
    public static void Call_OnCompleteSave()=>E_OnCompleteSave?.Invoke();
}
