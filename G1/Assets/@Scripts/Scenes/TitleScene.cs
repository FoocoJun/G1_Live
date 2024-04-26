using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    // Start is called before the first frame update
    void Start()
    {
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) => {
            Debug.Log($"{key} {count}/{totalCount}");

            if (count == totalCount) {
                //Managers.Data.init();
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
