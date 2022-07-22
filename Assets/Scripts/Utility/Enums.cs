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
    INVENTORY,
    BACK,
    FORTH,
    CONSOLE
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
    FIXED_TICK,
    EXIT
}

public enum CardFlag
{
    STARVING_DOG,
    BREAKING_FORM,
    REBIRTH,
    CYCLOPS_MONK,
    KING,
    ESSENCE,
    EARLY_GRAVE,
    SUNDIAL,
    CARD_C1,
    CARD_C2,
    CARD_C3,
    CARD_C4,
    CARD_D1,
    CARD_D2,
    CARD_D3,
    CARD_D4
}

public enum Faction
{
    CATALOGUE,
    DEITY
}

public enum EnemyMotionMode
{
    POINT,
    LINE,
    CIRCLE
}

public enum EnemyFireMode
{
    BEAM,
    RING,
    SPIRAL
}

public enum CombatState
{
    DORMANT,
    ONGOING,
    COMPLETE
}
