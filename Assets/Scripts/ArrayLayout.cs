using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArrayLayout
{
	public ArrayLayout(int y, int x)
	{
		rows = new rowData[y];
		for (int i = 0; i < y; i++)
		{
			rows[i] = new rowData();
			rows[i].row = new bool[x];
		}
	}

	[System.Serializable]
	public struct rowData
	{
		public bool[] row;
	}

	public rowData[] rows; //creates a grid with a Y of 8, ultimately controlled by the CustPropertyDrawer.cs

	//internally, this object stores true if the lcoation is blocked, false if the location is usable.
	//this function flips those values and returns a new two-dimensional array that can be used by the grid
	public bool[,] ConvertToUsableArray()
	{
		bool[,] usableArray = new bool[rows[0].row.Length, rows.Length];
		for (int y = 0; y < rows.Length; y++)
		{
			for (int x = 0; x < rows[y].row.Length; x++)
			{
				usableArray[x, y] = !rows[y].row[x];
			}
		}
		return usableArray;
	}

}