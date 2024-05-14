using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;



public enum D_EDIT_STATE
{
    NULL = 0,
    MOVE = 1,
    ROTATE = 2,
    SCALE = 3,
    GEN = 4
}






public class triD_toolset : MonoBehaviour
{

    [HideInInspector]
    public D_EDIT_STATE state;

    public RotateGizmo3D rotGizmo;
    public MoveGizmo3D moveGizmo;
    public ScaleGizmo3D scaleGizmo;


    // Start is called before the first frame update
    void Start()
    {
        ExitPrevious();
        state = D_EDIT_STATE.NULL;
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnDisable()
    {
        ExitPrevious();
    }


    public void ExitPrevious()
    {
        switch (state)
        {
            case D_EDIT_STATE.MOVE:
                moveGizmo.gameObject.SetActive(false);
                break;
            case D_EDIT_STATE.ROTATE:
                rotGizmo.gameObject.SetActive(false);
                break;
            case D_EDIT_STATE.SCALE:
                scaleGizmo.gameObject.SetActive(false);
                break;
            case D_EDIT_STATE.GEN:
                Cursor3D.instance.gameObject.SetActive(false);
                break;
        }

        state = D_EDIT_STATE.NULL;
    }




    public void EnterMove()
    {
        ExitPrevious();
        moveGizmo.gameObject.SetActive(true);

        state = D_EDIT_STATE.MOVE;
    }

    public void EnterRotation()
    {
        ExitPrevious();
        rotGizmo.gameObject.SetActive(true);

        state = D_EDIT_STATE.ROTATE;
    }
    public void EnterScaling()
    {
        ExitPrevious();
        scaleGizmo.gameObject.SetActive(true);

        state = D_EDIT_STATE.SCALE;
    }

    public void EnterGen()
    {
        ExitPrevious();

        Cursor3D.instance.gameObject.SetActive(true);

        state = D_EDIT_STATE.GEN;
    }

}
