using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Action
{
    public GameObject[] showObjects; // Obiekty, które maj¹ siê pokazaæ
    public GameObject[] hideObjects; // Obiekty, które maj¹ znikn¹æ

    public void Execute()
    {
        foreach (GameObject obj in showObjects)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in hideObjects)
        {
            obj.SetActive(false);
        }
    }
}

[System.Serializable]
public class KeyAction
{
    public KeyCode key; // Klawisz, który bêdzie wychwytywany
    public bool isHold; // Czy klawisz ma byæ trzymany
    public float holdTime; // Czas trzymania klawisza w sekundach
    public List<Action> actions; // Lista akcji

    private float keyHoldTimer = 0f;

    public void Update()
    {
        if (isHold)
        {
            if (Input.GetKey(key))
            {
                keyHoldTimer += Time.deltaTime;
                if (keyHoldTimer >= holdTime)
                {
                    ExecuteActions();
                    keyHoldTimer = 0f; // Reset timer
                }
            }
            else
            {
                keyHoldTimer = 0f; // Reset timer when key is released
            }
        }
        else
        {
            if (Input.GetKeyDown(key))
            {
                ExecuteActions();
            }
        }
    }

    private void ExecuteActions()
    {
        foreach (var action in actions)
        {
            action.Execute();
        }
    }
}

public class KeyActionManager : MonoBehaviour
{
    public List<KeyAction> keyActions; // Lista KeyAction

    void Update()
    {
        foreach (var keyAction in keyActions)
        {
            keyAction.Update();
        }
    }
}
