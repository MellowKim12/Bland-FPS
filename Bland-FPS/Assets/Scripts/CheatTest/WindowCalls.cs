using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

// Credit to @486176652066756E21 on unknowncheats for base source code

public static class WindowCalls
{

    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, int fx, int dy, uint dwDat, int dwExtraInfo);

    public static IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
    }

    public static void LeftClick(float delay)
    {
        Wait(delay);
        mouse_event(0x0002, 0, 0, 0, 0);
        Wait(delay);
        mouse_event(0x0004, 0, 0, 0, 0);
        Wait(delay);
    }

    public static bool IsInCross<T>(T target, Camera localPlayerCam)
    {
        Ray ray = localPlayerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, float.MaxValue))
        {
            if (raycastHit.collider.gameObject.tag == "Player")
                return true;

        }
        return false;
    }



}
