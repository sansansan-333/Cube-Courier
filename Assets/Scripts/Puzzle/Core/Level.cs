using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private List<Cube> cubes = new List<Cube>();
    private Dictionary<Vector3Int, Cube> cubeDict = new Dictionary<Vector3Int, Cube>();
    public PuzzleState state;
    private Stack<Move> moves = new Stack<Move>();
    private LevelScreen levelScreen;

    private readonly float cubeMovingSpeed = 4f; // [m/s]
    private readonly Vector3Int[] adjDirections = new Vector3Int[6]{
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1),
    };

    public Level()
    {
        state = PuzzleState.Default;
    }

    void Start()
    {
        levelScreen = FindObjectOfType<LevelScreen>();
    }

    void Update()
    {

    }

    public List<Cube> GetCubes()
    {
        return new List<Cube>(cubes);
    }

    public void RemoveCube(Cube cube)
    {
        if(cubes.Remove(cube))
        {
            cubeDict.Remove(cube.position);
        }

#if UNITY_EDITOR
        DestroyImmediate(cube.gameObject);
#else
        Destroy(cube.gameObject);
#endif
    }

    public int CountCube()
    {
        return cubes.Count;
    }

    /// <summary>
    /// Try adding cube to the level. Fails if there is already a cube at the same position.
    /// </summary>
    /// <param name="cube"></param>
    /// <returns></returns>
    public bool TryToAddCube(Cube cube)
    {
        if(cubeDict.ContainsKey(cube.position)) return false;
        
        cubeDict[cube.position] = cube;
        cubes.Add(cube);
        return true;
    }

    /// <summary>
    /// Add a cube to the level. If there is already a cube at the same position, it will be destroyed.
    /// </summary>
    /// <param name="cube"></param>
    /// <returns></returns>
    public void ForceToAddCube(Cube cube)
    {
        if(!TryToAddCube(cube))
        {
            // remove existing cube
            var existingCube = cubeDict[cube.position];
            cubes.Remove(existingCube);
            Destroy(existingCube.gameObject);

            // add new one
            cubeDict[cube.position] = cube;
            cubes.Add(cube);
        }
    }

    /// <summary>
    /// Start to move a cube to the direction if possible.
    /// </summary>
    /// <returns>True if the cube can move in the direction. False otherwise.</returns>
    public bool TryToStartMovingCube(Cube cube, MoveDirection direction)
    {
        if(cube == null || !cubes.Contains(cube)) return false;

        if(CanGoTo(cube, direction))
        {
            var destination = cube.position + direction;

            // start to move a cube
            state = PuzzleState.CubeMoving;
            MotionSystem.Instance.StartMoving(
                cube.transform, 
                destination,
                cubeMovingSpeed,
                () => { 
                    FinalizeCubeMove(cube, Vector3Int.FloorToInt(cube.position + direction));

                    // record move 
                    moves.Push(new Move(
                        cube,
                        cube.position,
                        direction,
                        destination
                    ));
                }
            );
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UndoMove()
    {
        if(moves.Count > 0) DoInverseMoveOf(moves.Pop());
    }

    public bool IsLevelCompleted()
    {
        // return true if not all cubes are locked
        foreach(var cube in cubes)
        {
            if(cube is StaticCube) continue;
            else if(cube is ColorCube colorCube && colorCube.movable) return false;
        }

        return true;
    }

    private bool CanGoTo(Cube cube, MoveDirection direction)
    {
        var nextPosition = cube.position + direction;

        if(IsPositionOccupied(nextPosition)) return false; 
        if(!IsThereCubeBelow(nextPosition)) return false;

        return true;
    }

    private void FinalizeCubeMove(Cube cube, Vector3Int nextPosition)
    {
        // change state
        state = PuzzleState.Default;

        // change cube position
        cubeDict[cube.position] = null;
        cube.position = nextPosition;
        cubeDict[nextPosition] = cube;

        // lock adjacent cube if it has the same color
        if(cube is ColorCube colorCube)
        {
            foreach (var adjCube in GetAdjacentCubes(colorCube))
            {
                if (adjCube is ColorCube adjColorCube && colorCube.color == adjColorCube.color)
                {
                    LockCube(colorCube);
                    LockCube(adjColorCube);
                }
            }
        }

        // check if level is completed
        if(IsLevelCompleted()) 
        {
            state = PuzzleState.Completed;
            levelScreen.ShowCompletedPopup();
        }

        Debug.Log("move finalized");
    }

    private bool IsPositionOccupied(Vector3Int position)
    {
        return cubeDict.ContainsKey(position) && cubeDict[position] != null;
    }

    private bool IsThereCubeBelow(Vector3Int position)
    {
        foreach(var pos in cubeDict.Keys)
        {
            if(cubeDict[pos] != null &&
                position.x == pos.x && position.z == pos.z && position.y > pos.y) 
            {
                return true;
            }
        }

        return false;
    }

    private List<Cube> GetAdjacentCubes(Cube cube)
    {
        var adjCubes = new List<Cube>();
        for(int i = 0; i < adjDirections.Length; i++)
        {
            var adjPosition = cube.position + adjDirections[i];
            if(cubeDict.ContainsKey(adjPosition) && cubeDict[adjPosition] != null)
            {
                adjCubes.Add(cubeDict[adjPosition]);
            }
        }

        return adjCubes;
    }

    private void LockCube(ColorCube colorCube)
    {
        colorCube.movable = false;
    }

    private void DoInverseMoveOf(Move move)
    {

    }
}

public enum PuzzleState
{
    Default,
    CubeMoving,
    Completed,
}
