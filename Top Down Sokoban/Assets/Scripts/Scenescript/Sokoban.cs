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
	public int crateTile;
	public int heroOnDestinationTile;
	public int crateOnDestinationTile;
	public Color destinationColor;
	public Sprite tileSprite;
	public Sprite heroSprite;
	public Sprite ballSprite;
	int[,] levelData;
	int rows;
	int cols;
	Vector2 middleOffset=new Vector2();
	int ballCount;
	GameObject hero;
	Dictionary<GameObject,Vector2> balls;

	void Start () {
		ballCount=0;
		balls=new Dictionary<GameObject, Vector2>();
		ParseLevel();
		CreateLevel();
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
		middleOffset.x=cols*tileSize*0.5f-tileSize*0.5f;
		middleOffset.y=rows*tileSize*0.5f-tileSize*0.5f;;
	}
	void CreateLevel(){
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
						}else if(val==crateTile){
							ballCount++;
							ball = new GameObject("ball"+ballCount.ToString());
							ball.transform.localScale=Vector2.one*(tileSize-1);
							sr = ball.AddComponent<SpriteRenderer>();
							sr.sprite=ballSprite;
							sr.sortingOrder=1;
							sr.color=Color.black;
							ball.transform.position=GetScreenPointFromLevelIndices(i,j);
							balls.Add(ball, new Vector2(i,j));
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
