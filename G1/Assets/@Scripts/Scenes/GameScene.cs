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

        Managers.Object.Spawn<Hero>(Vector3.zero);

        return true;
    }

    public override void Clear() {
        // TODO
    }
}
