using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpiteDependFromCount{
	[SerializeField] private Sprite sprite;
	public Sprite GetSprite{get => sprite;}
	[SerializeField] private int requireCount;
	public int RequireCount{get => requireCount;}

}
[System.Serializable]
public class ListSpriteDependFromCount{
    [SerializeField] private List<SpiteDependFromCount> listSpriteGoldHeap = new List<SpiteDependFromCount>();
    public Sprite GetSprite(int tact){
    	Sprite result = listSpriteGoldHeap[0].GetSprite;
        int num = -1;
    	for(int i = 0; i < listSpriteGoldHeap.Count; i++){
    		if(tact >= listSpriteGoldHeap[i].RequireCount){
                num = i;
    		}
    	}
        if(num >= 0) result = listSpriteGoldHeap[num].GetSprite;
    	return result;
    }
}