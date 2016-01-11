using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;


//[CustomEditor(typeof (MatInteractTable))]

public class MatInteractEditor : EditorWindow {
	
	MatInteractTable table;
	bool showAddRemove = false;
	bool showPingTester = false;
	bool showSettings = false;
	bool confirmReset = false;
	PhysicMaterial addThisMat;
	PhysicMaterial removeThisMat;
	PhysicMaterial testStriking;
	PhysicMaterial testReceiving;
	string tempTablePath = "";
	//Vector2 cornerPosition = new Vector2(0, 0);
	Vector2 windowPosition = new Vector2(0, 0);
	void OnProjectChange(){
		table = ForceTableReload ();
	}
	[MenuItem ("Window/Material Interaction Table")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		EditorWindow.GetWindow (typeof (MatInteractEditor));
	}
	
	string GetTableAssetPath()	
	{
		string tablePath = EditorPrefs.GetString ("MatInteractionTablePath");
		if(tablePath == "")
		{
			tablePath = "Assets/MaterialInteraction";
			EditorPrefs.SetString ("MatInteractionTablePath", tablePath);
		}
		if(!Directory.Exists(tablePath + "/Resources"))
		{
			Directory.CreateDirectory(tablePath + "/Resources");
		}
		return tablePath + "/Resources/Table.asset";
	}
	
	MatInteractTable GetTable()
	{
		return table ?? (MatInteractTable)AssetDatabase.LoadAssetAtPath (GetTableAssetPath(), typeof(MatInteractTable)) ?? ScriptableObject.CreateInstance<MatInteractTable>();
	}
	
	MatInteractTable ForceTableReload()
	{
		return (MatInteractTable)AssetDatabase.LoadAssetAtPath (GetTableAssetPath(), typeof(MatInteractTable)) ?? ScriptableObject.CreateInstance<MatInteractTable>();
	}
	
	public void OnGUI() {
		table = GetTable ();
		if(table == null)
		{
			GUILayout.Label("Material Interaction Table not found! This should never happen, please email me at kierenwallace@senet.com.au with details about how you got this to occur.");
			return;
		}
		//DrawDefaultInspector();
		if(showSettings)
		{
			if(confirmReset)
			{
				GUILayout.Label ("ARE YOU SURE?");
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button ("Yes"))
				{
					Undo.RegisterUndo(table, "Reset Table");
					table.ResetList();
					confirmReset = false;
					showSettings = false;
				}
				if(GUILayout.Button ("No"))
				{
					confirmReset = false;
				}
				EditorGUILayout.EndHorizontal();
				return;
			}
			if(tempTablePath == "")
			{
				tempTablePath = EditorPrefs.GetString ("MatInteractionTablePath");
			}
			tempTablePath = EditorGUILayout.TextField("Resource Path:", tempTablePath, GUILayout.ExpandWidth(true));
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button ("Save"))
			{
				EditorPrefs.SetString ("MatInteractionTablePath", tempTablePath);
				table = ForceTableReload ();
				showSettings = false;
			}
			if(GUILayout.Button ("Cancel"))
			{
				tempTablePath = EditorPrefs.GetString ("MatInteractionTablePath");
				showSettings = false;
			}
			EditorGUILayout.EndHorizontal();
			confirmReset = GUILayout.Button ("Reset All", GUILayout.ExpandWidth(false));
			return;
		}
		windowPosition = EditorGUILayout.BeginScrollView(windowPosition);
		showAddRemove = EditorGUILayout.Foldout(showAddRemove, "Add or Remove Material Types");
		if(showAddRemove)
		{
			EditorGUI.indentLevel = 1;
			addThisMat = (PhysicMaterial)EditorGUILayout.ObjectField("Add Material", addThisMat, typeof (PhysicMaterial), false);
			if(addThisMat != null)
			{
				Undo.RegisterUndo(table, "Add Material");
				table.AddPhysicMaterial(addThisMat);
				EditorUtility.SetDirty(table);
				addThisMat = null;
			}
			removeThisMat = (PhysicMaterial)EditorGUILayout.ObjectField("Remove Material", removeThisMat, typeof (PhysicMaterial), false);
			
			if(removeThisMat != null)
			{
				table.RemovePhysicMaterial(removeThisMat);
				removeThisMat = null;
			}
			//EditorGUILayout.LabelField("Array Size: ", table.prefabArray.GetLength(0) + ", " + table.prefabArray.GetLength(1));
			EditorGUI.indentLevel = 0;
		}
		
		showPingTester = EditorGUILayout.Foldout(showPingTester, "Test Material Interactions");
		if(showPingTester)
		{
			EditorGUI.indentLevel = 1;
			testStriking = (PhysicMaterial)EditorGUILayout.ObjectField("Striking", testStriking, typeof (PhysicMaterial), false);
			testReceiving = (PhysicMaterial)EditorGUILayout.ObjectField("Receiving", testReceiving, typeof (PhysicMaterial), false);
			if(GUILayout.Button("Ping Effect"))
			{
				EditorGUIUtility.PingObject(table.GetEffect(testStriking, testReceiving));
				testStriking = null;
				testReceiving = null;
			}
			EditorGUI.indentLevel = 0;
		}
		GameObject tempDefault = (GameObject)EditorGUILayout.ObjectField("Default Effect", table.defaultEffect, typeof (GameObject), false);
		if(table.defaultEffect != tempDefault)
		{
			Undo.RegisterUndo(table, "Default effect change");
			table.defaultEffect = tempDefault;
		}
		
		EditorGUILayout.Space();
		int removeIndex = -1;
		EditorGUILayout.BeginHorizontal(); {
			GUILayout.Space(100);
			GUILayout.Label("Receiving");
		} EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal(); {
			EditorGUILayout.BeginVertical();
			{
				GUILayout.Label("Striking");
				for(int j = 0; j < table.prefabArray.GetLength(1); j++)
				{
					int labelWidth = 70;
					if(showAddRemove)
					{
						labelWidth -= 20;
					}
					EditorGUILayout.BeginHorizontal();{
						GUILayout.Label(table.GetMaterialName(j), GUILayout.Width(labelWidth), GUILayout.Height(20));
						if(showAddRemove)
						{
							if(GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
							{
								removeIndex = j;
							}
						}
					} EditorGUILayout.EndHorizontal();
				}
			} EditorGUILayout.EndVertical();
			for(int j = 0; j < table.prefabArray.GetLength(1); j++)
			{
				EditorGUILayout.BeginVertical();{
					int labelWidth = 50;
					GUILayout.Label(table.GetMaterialName(j), GUILayout.Width(labelWidth));
					for(int i = 0; i < table.prefabArray.GetLength(0); i++)
					{
						GameObject tempEffect = (GameObject)EditorGUILayout.ObjectField(table.prefabArray[i, j], typeof (GameObject), false, GUILayout.Width(labelWidth), GUILayout.Height(20));
						if(table.prefabArray[i, j] != tempEffect)
						{
							Undo.RegisterUndo(table, "Change Effect");
							table.prefabArray[i, j] = tempEffect;
						}
					}
				} EditorGUILayout.EndVertical();
			}
			GUILayout.FlexibleSpace();
		} EditorGUILayout.EndHorizontal();
		if(removeIndex >= 0)
		{
			Undo.RegisterUndo(table, "Remove Physic Material");
			table.RemovePhysicMaterial(table.GetMaterial(removeIndex));
		}
		
		showSettings = GUILayout.Button ("Settings", GUILayout.ExpandWidth(false));
		
		EditorGUILayout.EndScrollView();
		if(GUI.changed)
		{
			EditorUtility.SetDirty(table);
			table.UpdateArrays();
		}
	}
	
	void OnInspectorUpdate()
	{
		this.Repaint();
		if(table == null)
		{
			return;
		}
		table.UpdateArrays();
	}
	
	void OnDestroy()
	{
		if(table)
		{
			if(AssetDatabase.GetAssetPath (table) == "")
			{
				//Debug.Log ("Creating Asset");
				AssetDatabase.CreateAsset(table, GetTableAssetPath());
			} else {
				//Debug.Log ("Saving Asset");
				AssetDatabase.SaveAssets();
			}
		}
	}
}
