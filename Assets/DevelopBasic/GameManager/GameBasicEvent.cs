using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBasic.Event
{
    public static class GameBasicEvent
    {
        public static event Action E_BeforeUnloadScene;
        public static void Call_BeforeUnloadScene(){E_BeforeUnloadScene?.Invoke();}
        public static event Action E_AfterLoadScene;
        public static void Call_AfterLoadScene(){E_AfterLoadScene?.Invoke();}
    }
}
