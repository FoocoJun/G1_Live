using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager {
    private Vector2 _moveDir;
    private EJoystickState _joystickState;

    #region Hero
    public Vector2 MoveDir {
        get { return _moveDir; }
        set {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(value);
        }
    }

    public EJoystickState JoystickState {
        get { return _joystickState; }
        set {
            _joystickState = value;
            OnJoystickStateChanged?.Invoke(value);
        }
    }
    #endregion

    #region Action
    public event Action<Vector2> OnMoveDirChanged;
    public event Action<EJoystickState> OnJoystickStateChanged;
    #endregion
}
