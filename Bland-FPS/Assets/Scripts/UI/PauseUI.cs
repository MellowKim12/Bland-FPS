using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField]
    private Button pauseBtn;

    private void Awake()
    {
        pauseBtn.onClick.AddListener(() =>
        {
            Cursor.lockState = CursorLockMode.Locked;
        });
    }



}
