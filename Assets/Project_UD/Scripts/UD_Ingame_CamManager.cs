using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_CamManager : MonoBehaviour
{
    float camPosition_X = 0;
    float camPosition_Z = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(camPosition_X, 20, camPosition_Z);

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
    }
}
