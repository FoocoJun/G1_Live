using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : InitBase {
    private BaseObject _target;
    public BaseObject Target {
        get { return _target; }
        set {
            _target = value;
        }
    }

    public override bool Init() {
        if (base.Init() == false) {
            return false;
        }

        // 카메라 기본 배율
        Camera.main.orthographicSize = 15.0f;

        return true;
    }

    // 모든 update가 끝난 후 작동. 카메라 로직은 이쪽으로
    void LateUpdate() {
        if (Target == null) {
            return;
        }

        Vector3 targetPosition = new Vector3(Target.CenterPosition.x, Target.CenterPosition.y, -10f);
        transform.position = targetPosition;
    }
}
