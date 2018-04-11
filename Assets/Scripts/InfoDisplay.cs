using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InfoDisplay : MonoBehaviour {

	public MultiDimensionalArray TextArea;

	public string[] placeholders = {"BP: ", "Sheilds: ", "Bids: ", "Cards: "};


	public void setText(List<List<int>> playerData)
	{
		for(int i =0; i<playerData.Count;i++)
		{
			for (int j = 0; j<playerData[i].Count; j++)
			{
				TextArea.TextData[i].TextData[j].text = placeholders[j] + playerData[i][j].ToString();
			}
		}
	}

}
