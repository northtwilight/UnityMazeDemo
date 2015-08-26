using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {
	// VARIABLES
	// ============================
	// public int sizeX, sizeZ;

	public MazeCell cellPrefab;
	private MazeCell[,] cells;

	public float generationStepDelay;

	public IntVector2 size;
	public IntVector2 RandomCoordinates {
		get {
			return new IntVector2(Random.Range (0, size.x), Random.Range (0, size.z));
		}
	}
	public IntVector2 ZeroCoordinates {
		get {
			return new IntVector2(0, 0);
		}
	}

	public bool ContainsCoordinates (IntVector2 coordinate) {
		return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
	}


	public MazePassage passagePrefab;
	//public MazeWall wallPrefab;
	public MazeWall[] wallPrefabs;

	public MazeDoor doorPrefab;

	[Range(0f, 1f)]
	public float doorProbability;

	public MazeCell GetCell (IntVector2 coordinates) {
		return cells[coordinates.x, coordinates.z];
	}


	public IEnumerator Generate () {
		WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
		cells = new MazeCell[size.x, size.z];
		List<MazeCell> activeCells = new List<MazeCell>();
		DoFirstGenerationStep(activeCells);

		//IntVector2 coordinates = RandomCoordinates;
		while (activeCells.Count > 0) {

			yield return delay;
			DoNextGenerationStep(activeCells);

			//while (ContainsCoordinates(coordinates) && GetCell(coordinates) == null) {   
			//CreateCell(coordinates);
			//coordinates += MazeDirections.RandomValue.ToIntVector2();
			//coordinates.z += 1;
		}

		//for (int x = 0; x < size.x; x++) {
		//	for (int z = 0; z < size.z; z++) {
		//		yield return delay;
		//		CreateCell(new IntVector2(x, z));
		//	}
	}

	private MazeCell CreateCell (IntVector2 coordinates) {
		MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
		cells[coordinates.x, coordinates.z] = newCell;
		newCell.coordinates = coordinates;
		newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
		newCell.transform.parent = transform;
		newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
		return newCell;
	}

	private void DoFirstGenerationStep (List<MazeCell> activeCells) {
		activeCells.Add(CreateCell(RandomCoordinates));
		//activeCells.Add(CreateCell(ZeroCoordinates));
	}

	private void DoNextGenerationStep (List<MazeCell> activeCells) {
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

	private void CreatePassage (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		MazePassage prefab = Random.value < doorProbability	? doorPrefab : passagePrefab;

		//MazePassage passage = Instantiate(passagePrefab) as MazePassage;
		MazePassage passage = Instantiate(prefab) as MazePassage;

		passage.Initialize(cell, otherCell, direction);

		passage.Initialize(otherCell, cell, direction.GetOpposite());
	}

	private void CreateWall (MazeCell cell, MazeCell otherCell, MazeDirection direction) {
		//MazeWall wall = Instantiate(wallPrefab) as MazeWall;
		MazeWall wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)]) as MazeWall;

		wall.Initialize(cell, otherCell, direction);
		if (otherCell != null) {
			//wall = Instantiate(wallPrefab) as MazeWall;
			wall = Instantiate(wallPrefabs[Random.Range (0, wallPrefabs.Length)]) as MazeWall;

			wall.Initialize(otherCell, cell, direction.GetOpposite());
		}
	}


	

}
