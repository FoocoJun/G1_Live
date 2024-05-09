using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class Hero : Creature {
    public override bool Init() {
        if (base.Init() == false) {
            return false;
        }

        CreatureType = ECreatureType.Hero;
        CreatureState = ECreatureState.Idle;

        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        return true;
    }

    public override void SetInfo(int templateId) {
        base.SetInfo(templateId);

        // State
        CreatureState = ECreatureState.Idle;
    }

    private void HandleOnJoystickStateChanged(EJoystickState joystickState) {
        switch (joystickState) {
            case EJoystickState.PointerDown:
                CreatureState = ECreatureState.Move;
                break;
            case EJoystickState.Drag:
                break;
            case EJoystickState.PointerUp:
                CreatureState = ECreatureState.Idle;
                break;
            default:
                break;
        }
    }
}
