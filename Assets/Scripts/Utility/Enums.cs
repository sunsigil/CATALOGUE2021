using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlLayer
{
    WORLD,
    MENU,
    POPUP
}

public enum InputCode
{
    CONFIRM,
    CANCEL,
    ACTION,
    UP,
    DOWN,
    LEFT,
    RIGHT,
    JOURNAL,
    INVENTORY
}

public enum ShrineFlag
{
    BOOTH,
    ARCH,
    EYE,
    MONUMENT
}

public enum RuneFlag
{
    SKILL_A,
    POWER_A,
    SKILL_B,
    POWER_B,
    SKILL_C,
    POWER_C,
    SKILL_D,
    POWER_D
}

public enum StateSignal
{
    ENTER,
    TICK,
    EXIT
}
