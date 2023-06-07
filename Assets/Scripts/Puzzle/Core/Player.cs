using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private Level level;
    private ColorCube dragStartCube;

    public Player(){}

    public void ManagedUpdate()
    {
        level = PuzzleManager.Instance.CurrentLevel;
        if(level == null) return;

        if(level.state == PuzzleState.Default)
        {
            // MoveCubeWithDrag();
            MoveCubeWithClick();
        }
    }

    private void MoveCubeWithDrag()
    {
        // start of drag
        if (Input.GetMouseButtonDown(0))
        {
            var cube = GetCubeAtMousePosition();
            if (cube != null && cube is ColorCube colorCube && colorCube.movable)
            {
                RegisterDragStart(colorCube);
            }
        }

        // end of drag
        if (Input.GetMouseButtonUp(0))
        {
            if (IsDragRegistered())
            {
                var cube = GetCubeAtMousePosition();
                if (cube != null)
                {
                    var direction = CalcuDirection(dragStartCube, cube);
                    Debug.Log(direction);
                    level.TryToStartMovingCube(dragStartCube, direction);
                }

                dragStartCube = null;
            }
        }
    }

    private void MoveCubeWithClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(level.IsCubeSelected())
            {
                var cube = GetCubeAtMousePosition();
                if (cube != null)
                {
                    if(level.SelectedCube.movable)
                    {
                        // move cube
                        var direction = CalcuDirection(level.SelectedCube, cube);
                        level.TryToStartMovingCube(level.SelectedCube, direction);
                    }
                }
            }
            else
            {
                // select cube
                var cube = GetCubeAtMousePosition();
                if (cube != null && cube is ColorCube colorCube && colorCube.movable)
                {
                    level.SelectCube(colorCube);
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

    private void RegisterDragStart(ColorCube colorCube)
    {
        dragStartCube = colorCube;
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