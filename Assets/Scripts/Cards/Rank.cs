using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rank : ScriptableObject{

	public int BattlePoints;
	public int Sheild;
	public int rank;

	public Rank(){}

	public void Squire(){
		rank = 0;
		BattlePoints = 5;
		Sheild = 0;
	}
	public void knight(){
		rank =1;
		BattlePoints = 10;
		Sheild = Sheild - 5;
	}
	public void Cknight(){
		rank = 2;
		BattlePoints = 20;
		Sheild = Sheild - 7;
	}
	public void winner(){
		rank = 3;
		BattlePoints = 42;
		Sheild = Sheild - 10;
	}
}
