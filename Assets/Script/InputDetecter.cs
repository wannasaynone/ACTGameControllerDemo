using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine//把API要用的enum加入namespace
{
    public enum InputType
    {
        Hold,//持續
        Down,//按下時
        Up//放開時
    }
}

public class InputDetecter  {

    /// <summary>
    /// 統一在這裡檢查玩家輸入，如果有一些要限制玩家輸入的場合將一併在這裡檢查
    /// </summary>
    public static bool GetPlayerControllerInput(KeyCode KeyCode, InputType Type)
    {
        switch(Type)
        {
            case InputType.Hold:
                return Input.GetKey(KeyCode);
            case InputType.Down:
                return Input.GetKeyDown(KeyCode);
            case InputType.Up:
                return Input.GetKeyUp(KeyCode);
            default:
                return false;
        }
    }

}
