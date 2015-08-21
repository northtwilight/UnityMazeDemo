using UnityEngine;
//using System.Collections;
using System.Collections.Generic;

public class MazeRoom : ScriptableObject {

	public int settingsIndex;

	public MazeRoomSettings settings;

	private List<MazeCell> cells = new List<MazeCell>();

	public void Add(MazeCell cell) {
		cell.room = this;
		cells.Add(cell);
	}


	// Use this for initialization
	//void Start () {
	
	//}
	
	// Update is called once per frame
	//void Update () {
	
	//}
}
