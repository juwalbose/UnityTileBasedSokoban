using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sokoban : MonoBehaviour {
	public string levelName;
	public float tileSize;
	public int invalidTile;
	public int groundTile;
	public int destinationTile;
	public int heroTile;
	public int ballTile;
	public int heroOnDestinationTile;
	public int ballOnDestinationTile;
	public Color destinationColor;
	public Sprite tileSprite;
	public Sprite heroSprite;
	public Sprite ballSprite;
	public KeyCode[] userInputKeys;//up, right, down, left
	int[,] levelData;
	int rows;
	int cols;
	Vector2 middleOffset=new Vector2();
	int ballCount;
	GameObject hero;
	Dictionary<GameObject,Vector2> occupants;//balls & hero

	void Start () {
		ballCount=0;
		occupants=new Dictionary<GameObject, Vector2>();
		ParseLevel();
		CreateLevel();
	}
	void Update(){
		ApplyUserInput();
	}

    private void ApplyUserInput()
    {
        if(Input.GetKeyUp(userInputKeys[0])){
			TryMoveHero(0);//up
		}else if(Input.GetKeyUp(userInputKeys[1])){
			TryMoveHero(1);//right
		}else if(Input.GetKeyUp(userInputKeys[2])){
			TryMoveHero(2);//down
		}else if(Input.GetKeyUp(userInputKeys[3])){
			TryMoveHero(3);//left
		}
    }
    private void TryMoveHero(int direction)
    {
        Vector2 heroPos;
		Vector2 oldHeroPos;
		occupants.TryGetValue(hero,out oldHeroPos);
		Debug.Log("hero "+oldHeroPos.x.ToString()+":"+oldHeroPos.y.ToString());
		heroPos=GetNextPositionAlong(oldHeroPos,direction);
		Debug.Log("hero next"+heroPos.x.ToString()+":"+heroPos.y.ToString());
		
		if(IsValidPosition(heroPos)){
			if(!IsOccuppied(heroPos)){
				//move hero
				RemoveOccuppant(oldHeroPos);
				hero.transform.position=GetScreenPointFromLevelIndices((int)heroPos.x,(int)heroPos.y);
				occupants[hero]=heroPos;
				if(levelData[(int)heroPos.x,(int)heroPos.y]==groundTile){
					levelData[(int)heroPos.x,(int)heroPos.y]=heroTile;
				}else if(levelData[(int)heroPos.x,(int)heroPos.y]==destinationTile){
					levelData[(int)heroPos.x,(int)heroPos.y]=heroOnDestinationTile;
				}
			}else{
				//try move ball
				Debug.Log("occuppied");
			}
		}else{Debug.Log("not valid");}
    }

    private void RemoveOccuppant(Vector2 objPos)
    {
        if(levelData[(int)objPos.x,(int)objPos.y]==heroTile||levelData[(int)objPos.x,(int)objPos.y]==ballTile){
			levelData[(int)objPos.x,(int)objPos.y]=groundTile;
		}else if(levelData[(int)objPos.x,(int)objPos.y]==heroOnDestinationTile){
			levelData[(int)objPos.x,(int)objPos.y]=destinationTile;
		}else if(levelData[(int)objPos.x,(int)objPos.y]==ballOnDestinationTile){
			levelData[(int)objPos.x,(int)objPos.y]=destinationTile;
		}
    }

    private bool IsOccuppied(Vector2 objPos)
    {
        return levelData[(int)objPos.x,(int)objPos.y]==ballTile;
    }

    private bool IsValidPosition(Vector2 objPos)
    {
        if(objPos.x>-1&&objPos.x<rows&&objPos.y>-1&&objPos.y<cols){
			return levelData[(int)objPos.x,(int)objPos.y]!=invalidTile;
		}else return false;
    }

    private Vector2 GetNextPositionAlong(Vector2 objPos, int direction)
    {
        switch(direction){
			case 0:
			objPos.x-=1;//up
			break;
			case 1:
			objPos.y+=1;//right
			break;
			case 2:
			objPos.x+=1;//down
			break;
			case 3:
			objPos.y-=1;//left
			break;
		}
		return objPos;
    }
    void ParseLevel(){
		TextAsset textFile = Resources.Load (levelName) as TextAsset;
		string[] lines = textFile.text.Split (new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);//split by new line, return
		string[] nums = lines[0].Split(new[] { ',' });//split by ,
		rows=lines.Length;//number of rows
		cols=nums.Length;//number of columns
		levelData = new int[rows, cols];
        for (int i = 0; i < rows; i++) {
			string st = lines[i];
            nums = st.Split(new[] { ',' });
			for (int j = 0; j < cols; j++) {
                int val;
                if (int.TryParse (nums[j], out val)){
                	levelData[i,j] = val;
				}
                else{
                    levelData[i,j] = -1;
				}
            }
        }
	}
	void CreateLevel(){
		middleOffset.x=cols*tileSize*0.5f-tileSize*0.5f;
		middleOffset.y=rows*tileSize*0.5f-tileSize*0.5f;;
		GameObject tile;
		SpriteRenderer sr;
		GameObject ball;
		int destinationCount=0;
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < cols; j++) {
                int val=levelData[i,j];
				if(val!=invalidTile){
					tile = new GameObject("tile"+i.ToString()+"_"+j.ToString());
					tile.transform.localScale=Vector2.one*(tileSize-1);
					sr = tile.AddComponent<SpriteRenderer>();
					sr.sprite=tileSprite;
					tile.transform.position=GetScreenPointFromLevelIndices(i,j);
					if(val==destinationTile){
						sr.color=destinationColor;
						destinationCount++;
					}else{
						if(val==heroTile){
							hero = new GameObject("hero");
							hero.transform.localScale=Vector2.one*(tileSize-1);
							sr = hero.AddComponent<SpriteRenderer>();
							sr.sprite=heroSprite;
							sr.sortingOrder=1;
							sr.color=Color.red;
							hero.transform.position=GetScreenPointFromLevelIndices(i,j);
							occupants.Add(hero, new Vector2(i,j));
							Debug.Log("hero "+i.ToString()+":"+j.ToString());
						}else if(val==ballTile){
							ballCount++;
							ball = new GameObject("ball"+ballCount.ToString());
							ball.transform.localScale=Vector2.one*(tileSize-1);
							sr = ball.AddComponent<SpriteRenderer>();
							sr.sprite=ballSprite;
							sr.sortingOrder=1;
							sr.color=Color.black;
							ball.transform.position=GetScreenPointFromLevelIndices(i,j);
							occupants.Add(ball, new Vector2(i,j));
						}
					}
				} 
            }
        }
		if(ballCount>destinationCount)Debug.LogError("there are more balls than destinations");
	}
	Vector2 GetScreenPointFromLevelIndices(int row,int col){
		return new Vector2(col*tileSize-middleOffset.x,row*-tileSize+middleOffset.y);
	}
	Vector2 GetLevelIndicesFromScreenPoint(float xVal,float yVal){
		return new Vector2((int)(yVal-middleOffset.y)/-tileSize,(int)(xVal+middleOffset.x)/tileSize);
	}
	Vector2 GetLevelIndicesFromScreenPoint(Vector2 pos){
		return GetLevelIndicesFromScreenPoint(pos.x,pos.y);
	}
}
