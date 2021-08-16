using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton

    public static PlayerManager instance;

     void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject player;
    public Camera _camera;

    private Vector3 offset;

    void Start()
    {
        // Gets offset from current position of the camera
        
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        // Camera follows player
        if (!Input.GetButton("Fire2"))
        { 
            transform.position = player.transform.position + offset; 
        }
            
    }
}
