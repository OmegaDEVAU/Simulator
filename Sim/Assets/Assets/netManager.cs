using UnityEngine;
using UnityEngine.Networking;

public class netManager : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    Camera menuCamera;

	void Start ()
    {
	    if (!isLocalPlayer)
        {
            for (int i = 0; 1 < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
	}
}
