using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private Level level;
    private Cube dragStartCube;

    public Player(){}

    public void ManagedUpdate()
    {
        level = PuzzleManager.Instance.CurrentLevel;
        if(level == null) return;

        if(level.state == PuzzleState.Default)
        {
            // start of drag
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("drag start");
                var cube = GetCubeAtMousePosition();
                if(cube != null && cube is ColorCube colorCube && colorCube.movable)
                {
                    RegisterDragStart(cube);
                }
            }

            // end of drag
            if(Input.GetMouseButtonUp(0))
            {
                if(IsDragRegistered())
                {
                    var cube = GetCubeAtMousePosition();
                    Debug.Log($"mouse up on {cube}");
                    if(cube != null)
                    {
                        var direction = CalcuDirection(dragStartCube, cube);
                        Debug.Log(direction);
                        level.TryToStartMovingCube(dragStartCube, direction);
                    }

                    dragStartCube = null;
                }
            }
        }
    }

    private Cube GetCubeAtMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            return hit.collider.gameObject.GetComponent<Cube>();
        }

        return null;
    }

    private void RegisterDragStart(Cube cube)
    {
        dragStartCube = cube;
        Debug.Log($"drag start: {cube}");
    }

    private bool IsDragRegistered()
    {
        return dragStartCube != null;
    }

    private MoveDirection CalcuDirection(Cube start, Cube end)
    {
        var startPos = start.position;
        var endPos = end.position;
        Debug.Log(endPos - startPos);
        return new MoveDirection(endPos - startPos);
    }
}