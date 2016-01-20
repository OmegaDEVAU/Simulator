using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_ID : NetworkBehaviour {

	[SyncVar] public string playerUniqueIdentity;
    [SerializeField]
    private NetworkInstanceId playerNetID;
    [SerializeField]
    private Transform myTransform;

	public override void OnStartLocalPlayer ()
	{
		GetNetIdentity();
		SetIdentity();
	}

	// Use this for initialization
	void Awake () 
	{
		myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(myTransform.name == "" || myTransform.name == "Player(Clone)")
		{
			SetIdentity();
		}
	}

	[Client]
	void GetNetIdentity()
	{
		playerNetID = GetComponent<NetworkIdentity>().netId;
		CmdTellServerMyIdentity(MakeUniqueIdentity());
	}
	
	void SetIdentity()
	{
		if(!isLocalPlayer)
		{
			myTransform.name = playerUniqueIdentity;
		}
		else
		{
			myTransform.name = MakeUniqueIdentity();
		}
	}

	string MakeUniqueIdentity()
	{
		string uniqueName = "Player " + playerNetID.ToString();
		return uniqueName;
	}

	[Command]
	void CmdTellServerMyIdentity(string name)
	{
		playerUniqueIdentity = name;
	}


}
