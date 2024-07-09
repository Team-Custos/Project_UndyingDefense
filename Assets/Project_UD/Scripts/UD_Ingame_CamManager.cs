using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_CamManager : MonoBehaviour
{
    UD_Ingame_InputSystem inputSystem;

    float camPosition_X = 0;
    float camPosition_Z = 0;

    public float camPosition_Y = 0;


    // Start is called before the first frame update
    void Start()
    {
        inputSystem = UD_Ingame_InputSystem.inst;   
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(camPosition_X, camPosition_Y, camPosition_Z);

        //ī
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            camPosition_Z -= 0.1f;
        }
        else if (Input.GetAxisRaw("Vertical") > 0)
        {
            camPosition_Z += 0.1f;
        }


        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            camPosition_X -= 0.1f;
        }
        else if(Input.GetAxisRaw("Horizontal") > 0)
        {
            camPosition_X += 0.1f;
        }

        if (inputSystem.IsWheelScrollUp)
        {
            camPosition_Y += 0.5f;
            
        }
        else if (inputSystem.IsWheelScrollDown)
        {
            camPosition_Y -= 0.5f;
        }
    }
}
