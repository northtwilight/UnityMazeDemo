using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {
	public IntVector2 size;																// 1 cc : 4

	public MazeCell cellPrefab;															// 2 cc : 1

	public float generationStepDelay;													// 3 cc : 3

	public MazePassage passagePrefab;													// 4 cc : 8

	public MazeDoor doorPrefab;															// 5 cc : 10
	
	[Range(0f, 1f)]																		// 6 cc : 11
	public float doorProbability;														// 7 cc : 12

	public MazeWall[] wallPrefabs;														// 8 cc : 9

	public MazeRoomSettings[] roomSettings;												// 9 cc : 14

	private MazeCell[,] cells;															// 10 cc : 2

	private List<MazeRoom> rooms = new List<MazeRoom>(); 								// 11 cc : 15



	public IntVector2 RandomCoordinates {												// 12 cc : 5
		get {
			return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
		}
	}


//	public IntVector2 ZeroCoordinates {													// 12a - extra for checking
//		get {
//			return new IntVector2(0, 0);
//		}
//	}


	public bool ContainsCoordinates (IntVector2 coordinate) {							// 13 cc : 7
		return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
	}


	//public MazeWall wallPrefab;
	public MazeCell GetCell (IntVector2 coordinates) {									// 14 cc : 13
		return cells[coordinates.x, coordinates.z];
	}


	public IEnumerator Generate () {													// 15 cc : 16
		WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
		cells = new MazeCell[size.x, size.z];
		List<MazeCell> activeCells = new List<MazeCell>();
		DoFirstGenerationStep(activeCells);

		while (activeCells.Count > 0) {
			yield return delay;
			DoNextGenerationStep(activeCells);
		}
	}


	private void DoFirstGenerationStep (List<MazeCell> activeCells) {					// 16 cc : 18
		MazeCell newCell = CreateCell(RandomCoordinates);
		newCell.Initialize(CreateRoom(-1));
		activeCells.Add(newCell);
		//activeCells.Add(CreateCell(ZeroCoordinates));
	}


	private void DoNextGenerationStep (List<MazeCell> activeCells) {					// 17 cc : 19
		int currentIndex = activeCells.Count - 1;
		MazeCell currentCell = activeCells[currentIndex];

		if (currentCell.IsFullyInitialized) {
			activeCells.RemoveAt(currentIndex);
			return;
		}

		MazeDirection direction = currentCell.RandomUninitializedDirection;
		IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();

		if (ContainsCoordinates(coordinates)) {
			MazeCell neighbor = GetCell(coordinates); 
			if (neighbor == null) {
				neighbor = CreateCell(coordinates);
				CreatePassage(currentCell, neighbor, direction);
				activeCells.Add(neighbor);

			} else if (currentCell.room.settingsIndex == neighbor.room.settingsIndex) {	// new	
				CreatePassageInSameRoom(currentCell, neighbor, direction);				// new

			} else {
				CreateWall(currentCell, neighbor, direction);
				// test this
				//activeCells.RemoveAt(currentIndex);
			}
		} else {
			CreateWall(currentCell, null, direction);
			// and this
			//activeCells.RemoveAt(currentIndex);
		}
	}
	

	private MazeCell CreateCell (IntVector2 coordinates) {								// 18 cc : 17
		MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
		cells[coordinates.x, coordinates.z] = newCell;
		newCell.coordinates = coordinates;
		newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
		newCell.transform.parent = transform;
		newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
		return newCell;
	}


	private void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction) {	// 19 cc : 20
		MazePassage prefab = Random.value < doorProbability	? doorPrefab : passagePrefab;

		//MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		MazePassage passage = Instantiate(prefab) as MazePassage;

		passage.Initialize(cell, otherCell, direction);
		passage = Instantiate(prefab) as MazePassage;

		if (passage is MazeDoor) {
			otherCell.Initialize(CreateRoom(cell.room.settingsIndex));
		} else {
			otherCell.Initialize(cell.room);
		}

		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}


	// 																							// 20cc : 23
	
	private void CreatePassageInSameRoom(MazeCell cell, MazeCell otherCell, MazeDirection direction) { 
		MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		passage.Initialize(cell, otherCell, direction);
	}


	private void CreateWall (MazeCell cell, MazeCell otherCell, MazeDirection direction) {		// 21cc: 20
		MazeWall wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;
		wall.Initialize(cell, otherCell, direction);

		if (otherCell != null) {
			wall = Instantiate(wallPrefabs[Random.Range (0, wallPrefabs.Length)]) as MazeWall;
			wall.Initialize(otherCell, cell, direction.GetOpposite());
		}
	}


	private MazeRoom CreateRoom(int indexToExclude) {											// 22cc : 21
		MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
		newRoom.settingsIndex = Random.Range(0, roomSettings.Length);
		if (newRoom.settingsIndex == indexToExclude) {
			newRoom.settingsIndex = (newRoom.settingsIndex + 1) % roomSettings.Length;
		}
		newRoom.settings = roomSettings[newRoom.settingsIndex];
		rooms.Add(newRoom);
		return newRoom;
	}



}