using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene {
    public override bool Init()
    {
        if (base.Init() == false) {
            return false;
        }
        SceneType = Define.EScene.GameScene;

        GameObject map = Managers.Resource.Instantiate("BaseMap");
        map.transform.position = Vector3.zero;
        map.name = "@BaseMap";
        // TODO

        Hero hero = Managers.Object.Spawn<Hero>(Vector3.zero);

        Managers.UI.ShowBaseUI<UI_Joystick>();

        return true;
    }

    public override void Clear() {
        // TODO
    }
}
