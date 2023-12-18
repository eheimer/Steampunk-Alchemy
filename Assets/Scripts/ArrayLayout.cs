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

}