using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameObjectArray
{
	[SerializeField]
	public GameObject[] array = new GameObject[0];
	public GameObject this[int index]{
		get {return array[index];}
		set {array[index] = value;}
	}
	public int Length{
		get {return array.Length;}
	}
	public GameObjectArray(int newLength)
	{
		array = new GameObject[newLength];
	}

}
/*
 * The MaterialInteractionSystem static class provides a singleton shortcut to a single instance of the
 * Interaction Table. By default it will use the one created by the editor window, but you could set it
 * up to use multiple tables (for whatever reason)- since all it really does is load in a table and
 * pass messages to it.
 **/
public static class MaterialInteractionSystem
{
	static MatInteractTable table;
	
	public static void SpawnEffect(PhysicMaterial striker, PhysicMaterial reciever, Vector3 point, Vector3 normal)
	{
		if(table == null)
		{
			table = (MatInteractTable)Resources.Load ("Table", typeof(MatInteractTable));
		}
		table.SpawnEffect(striker, reciever, point, normal);
	}
	
	public static void SpawnEffect(Collision collision, PhysicMaterial striker)
	{
		if(table == null)
		{
			table = (MatInteractTable)Resources.Load ("Table", typeof(MatInteractTable));
		}
		table.SpawnEffect(collision, striker);
	}
}

public class MatInteractTable : ScriptableObject {
	public List<PhysicMaterial> materialList = new List<PhysicMaterial>();
	public GameObject[,] prefabArray = new GameObject[0,0];
	public GameObjectArray[] arrayArray = new GameObjectArray[0];
	public GameObject defaultEffect;
	
	void Reset()
	{
		Debug.Log("Resetting");
		ForceArrayLoad();
	}
	
	public string GetMaterialName(int index)
	{
		return materialList[index].name;
	}
	
	void RemoveNullMaterials()
	{
		for(int i = 0; i < materialList.Count; ++i)
		{
			if (materialList[i] == null)
			{
				ShrinkPrefabArray(i);
				materialList.RemoveAt(i--);
			}
		}
	}
	
	public PhysicMaterial GetMaterial(int index)
	{
		RemoveNullMaterials();
		return materialList[index];
	}
	
	public void ForceArrayLoad()
	{
		if(arrayArray.Length != prefabArray.GetLength(0))
		{
			//Debug.Log("loading serialized arrays");
			prefabArray = new GameObject[arrayArray.Length, arrayArray.Length];
			for(int i = 0; i < prefabArray.GetLength(0); i++)
			{
				if(i >= materialList.Count)
				{
					
				}
				for(int j = 0; j < prefabArray.GetLength(1); j++)
				{
					prefabArray[i, j] = arrayArray[i][j];
				}
			}
		}
	}
	
	public GameObject GetEffect(PhysicMaterial striking, PhysicMaterial receiving)
	{
		ForceArrayLoad();
		if(striking == null || receiving == null)
		{
			if(defaultEffect == null)
			{
				Debug.LogError("Default interaction object not set! Bad behaviour may occur.");
				return new GameObject("Null Object");
			}
			return defaultEffect;
		}
		int strikeIndex = materialList.FindIndex(n => n.Equals(striking));
		int receiveIndex = materialList.FindIndex(receiving.Equals);
		if(strikeIndex >= 0 && receiveIndex >= 0)
		{
			if(prefabArray[strikeIndex, receiveIndex] != null)
			{
				return prefabArray[strikeIndex, receiveIndex];
			}
		}
		if(defaultEffect == null)
		{
			Debug.LogError("Default interaction object not set! Bad behaviour may occur.");
			return new GameObject("Null Object");
		}
		return defaultEffect;
	}
	
	public void SpawnEffect(Collision collision, PhysicMaterial striker)
	{
		GameObject effectPrefab = GetEffect(striker, collision.collider.sharedMaterial);
		Instantiate(effectPrefab, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
	}
	
	public void SpawnEffect(PhysicMaterial striker, PhysicMaterial reciever, Vector3 point, Vector3 normal)
	{
		GameObject effectPrefab = GetEffect(striker, reciever);
		Instantiate(effectPrefab, point, Quaternion.LookRotation(normal));
	}
	
	public void ResetList()
	{
		materialList = new List<PhysicMaterial>();
		prefabArray = new GameObject[0, 0];
		arrayArray = new GameObjectArray[0];
	}

	public void AddPhysicMaterial(PhysicMaterial addThisMat)
	{
		if(!materialList.Find(addThisMat.Equals))
		{
			materialList.Add(addThisMat);
			GrowPrefabArray();
		}
	}
	public void RemovePhysicMaterial(PhysicMaterial removeThisMat)
	{
		
		if(removeThisMat != null)
		{
			//Debug.Log("Removing an Object");
			int indexToRemove = materialList.FindIndex(removeThisMat.Equals);
			//Debug.Log(indexToRemove);
			if(indexToRemove >= 0)
			{
				materialList.Remove(removeThisMat);
				ShrinkPrefabArray(indexToRemove);
			}
		}
	}
	
	void GrowPrefabArray()
	{
		GameObject[,] newPrefabArray = new GameObject[prefabArray.GetLength(0) + 1, prefabArray.GetLength(1) + 1];
		arrayArray = new GameObjectArray[prefabArray.GetLength(0) + 1];
		for(int i = 0; i < prefabArray.GetLength(0); i++)
		{
			arrayArray[i] = new GameObjectArray(prefabArray.GetLength(1) + 1);
			for(int j = 0; j < prefabArray.GetLength(1); j++)
			{
				newPrefabArray[i, j] = prefabArray[i, j];
				arrayArray[i][j] = newPrefabArray[i, j];
			}
		}
		prefabArray = newPrefabArray;
	}
	
	static T[,] RemoveColumn<T>(T[,] array, int columnToRemove) {
		T[,] finalArray = new T[array.GetLength(0), array.GetLength(1) - 1];
		for(int i = 0; i < finalArray.GetLength (0); ++i)
		{
			for(int j = 0; j < finalArray.GetLength(1); ++j)
			{
				if(j < columnToRemove)
				{
					finalArray[i, j] = array[i, j];
				} else {
					finalArray[i, j] = array[i, j + 1];
				}
			}
		}
		return finalArray;
	}
	static T[,] RemoveRow<T>(T[,] array, int rowToRemove) {
		T[,] finalArray = new T[array.GetLength(0) - 1, array.GetLength(1)];
		for(int i = 0; i < finalArray.GetLength (1); ++i)
		{
			for(int j = 0; j < finalArray.GetLength(0); ++j)
			{
				if(j < rowToRemove)
				{
					finalArray[j, i] = array[j, i];
				} else {
					finalArray[j, i] = array[j + 1, i];
				}
			}
		}
		return finalArray;
	}
	void ShrinkPrefabArray(int indexToRemove)
	{
		prefabArray = RemoveColumn<GameObject>(RemoveRow<GameObject>(prefabArray, indexToRemove), indexToRemove);
		arrayArray = ConvertArray(prefabArray);
	}
	
	static GameObjectArray[] ConvertArray(GameObject[,] originalArray)
	{
		GameObjectArray[] retV = new GameObjectArray[originalArray.GetLength(0)];
		for(int i = 0; i < originalArray.GetLength(0); i++)
		{
			retV[i] = new GameObjectArray(originalArray.GetLength(1));
			for(int j = 0; j < originalArray.GetLength(1); j++)
			{
				retV[i][j] = originalArray[i, j];
			}
		}
		return retV;
	}
	public void FixArrays()
	{
		arrayArray = ConvertArray(prefabArray);
	}
	public void UpdateArrays()
	{
		if(arrayArray.Length != prefabArray.GetLength(0))
		{
			ForceArrayLoad();
		}
		arrayArray = ConvertArray(prefabArray);
	}
}
