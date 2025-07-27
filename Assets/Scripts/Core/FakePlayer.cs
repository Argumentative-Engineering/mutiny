using UnityEngine;
using UnityEngine.InputSystem;

public class FakePlayer : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            var fakeGamepad = InputSystem.AddDevice<Gamepad>();
            PlayerInputManager.instance.JoinPlayer(-1, -1, null, fakeGamepad);
        }
    }
}
