using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    private List<Cube> cubes = new List<Cube>();
    private Dictionary<Vector3Int, Cube> cubeDict = new Dictionary<Vector3Int, Cube>();
    public PuzzleState state;
    private Stack<List<CubeMove>> moveHistory = new Stack<List<CubeMove>>();
    public ColorCube SelectedCube { get; private set; }
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
    /// Start to move a cube to the direction if possible. Cube will fall if it can.
    /// </summary>
    /// <returns>True if the cube can move in the direction. False otherwise.</returns>
    public bool TryToStartMovingCube(ColorCube cube, MoveDirection direction)
    {
        if(cube == null || !cubes.Contains(cube)) return false;

        if(CanGoTo(cube, direction) && CanMove(cube))
        {
            var originalPosition = cube.position;
            var positionAfterHorizontalMove = cube.position + direction;
            var finalPosition = positionAfterHorizontalMove;
            var cubeMoves = new List<CubeMove>();

            // move horizontally
            var motionQueue = new MotionSystem.MotionQueue();
            motionQueue.PushMotion(new MotionInfo(
                cube.transform,
                cube.position,
                positionAfterHorizontalMove,
                ResourceSystem.Instance.PuzzleSettings.cubeMovingSpeedCurve,
                cubeMovingSpeed
            ));

            // fall 
            if(!IsPositionOccupied(positionAfterHorizontalMove + Vector3Int.down))
            {
                var floorCube = GetCubeBelow(positionAfterHorizontalMove + Vector3Int.down);
                if (floorCube != null)
                {
                    Debug.Log("fall");
                    finalPosition = floorCube.position + Vector3Int.up;

                    motionQueue.PushMotion(new MotionInfo(
                        cube.transform,
                        positionAfterHorizontalMove,
                        finalPosition,
                        ResourceSystem.Instance.PuzzleSettings.cubeFallingSpeedCurve,
                        cubeMovingSpeed
                    ));
                }
            }

            // invoke move
            state = PuzzleState.CubeMoving;
            motionQueue.Run(onEnd: () => {
                FinalizeCubeMove(cube, finalPosition);

                // record move
                cubeMoves.Add(new CubeMove(cube, originalPosition, finalPosition));
            });

            moveHistory.Push(cubeMoves);

            /*

            // gather all cubes to move
            var cubesToMove = new List<ColorCube>();
            var position = cube.position;
            while(true)
            {
                position += Vector3Int.up;
                if(IsPositionOccupied(position))
                {
                    var cubeAbove = cubeDict[position];
                    if(cubeAbove is ColorCube colorCubeAbove && colorCubeAbove.movable && CanGoTo(colorCubeAbove, direction))
                    {
                        cubesToMove.Add(colorCubeAbove);
                    }
                    else break;
                }
                else break;
            }

            // start moving
            StartMovingCubes(cubesToMove, direction);
            */

            return true;
        }
        else
        {
            return false;
        }
    }

    private void StartMovingCubes(List<ColorCube> cubes, MoveDirection direction)
    {
        
    }

    public void UndoMove()
    {
        if(moveHistory.Count > 0) 
        {
            var moves = moveHistory.Pop();
            foreach(var move in moves)
            {
                DoInverseMoveOf(move);
            }
        }
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

    public void SelectCube(ColorCube colorCube)
    {
        colorCube.OnSelected();
        SelectedCube = colorCube;
    }

    public void DeselectCube()
    {
        if(IsCubeSelected())
        {
            SelectedCube.OnDeselected();
            SelectedCube = null;
        }
    }

    public bool IsCubeSelected()
    {
        return SelectedCube != null;
    }

    private bool CanGoTo(Cube cube, MoveDirection direction)
    {
        var nextPosition = cube.position + direction;

        if(IsPositionOccupied(nextPosition)) return false; 
        if(!IsThereCubeBelow(nextPosition)) return false;

        return true;
    }

    private bool CanMove(ColorCube colorCube)
    {
        var cubeAbove = GetCubeAt(colorCube.position + Vector3Int.up);
        if(cubeAbove != null)
        {
            if(cubeAbove is ColorCube colorCubeAbove && !colorCubeAbove.movable)
            {
                return true;
            }
            else return false;
        }
        else return true;
    }

    private void FinalizeCubeMove(Cube cube, Vector3Int finalPosition)
    {
        // change state
        state = PuzzleState.Default;

        // change cube position
        cubeDict[cube.position] = null;
        cube.position = finalPosition;
        cubeDict[finalPosition] = cube;

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

    private Cube GetCubeAt(Vector3Int position)
    {
        if(cubeDict.ContainsKey(position))
        {
            return cubeDict[position];
        }

        return null;
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

    private Cube GetCubeBelow(Vector3 position)
    {
        Cube underCube = null;

        // find upmost cube under the cube
        foreach (var pos in cubeDict.Keys)
        {
            if (cubeDict[pos] != null &&
                position.x == pos.x && position.z == pos.z && 
                position.y > pos.y)
            {
                if(underCube == null || underCube.position.y < cubeDict[pos].position.y)
                {
                    underCube = cubeDict[pos];
                }
            }
        }

        return underCube;
    }

    private void LockCube(ColorCube colorCube)
    {
        colorCube.movable = false;
        DeselectCube();
        colorCube.OnLocked();
    }

    private void UnlockCube(ColorCube colorCube)
    {
        colorCube.movable = true;
        colorCube.OnUnlocked();
    }

    private void DoInverseMoveOf(CubeMove move)
    {
        // unlock itself and adjacent cubes 
        if(!move.colorCube.movable) UnlockCube(move.colorCube);
        foreach(var adjCube in GetAdjacentCubes(move.colorCube))
        {
            if(adjCube is ColorCube adjColorCube && 
                adjColorCube.color == move.colorCube.color && 
                !adjColorCube.movable)
            {
                UnlockCube(adjColorCube);
            }
        }

        // revert cube position
        move.colorCube.position = move.startPosition;
        move.colorCube.transform.position = move.startPosition;
        cubeDict[move.endPosition] = null;
        cubeDict[move.startPosition] = move.colorCube;
    }
}

public enum PuzzleState
{
    Default,
    CubeMoving,
    Completed,
}
