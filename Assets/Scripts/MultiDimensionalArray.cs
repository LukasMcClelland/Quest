using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class MultiDimensionalArray{
	[System.Serializable]
	public struct TextFeilds{public Text[] TextData;}
	public TextFeilds[] TextData = new TextFeilds[3]; 
}
