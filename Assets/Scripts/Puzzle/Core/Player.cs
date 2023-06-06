using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private Level level;
    private Cube dragStartCube;
    private ColorCube selectedCube;

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
                RegisterDragStart(cube);
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
            if(IsCubeSelected())
            {
                var cube = GetCubeAtMousePosition();
                if (cube != null)
                {
                    if(cube == selectedCube)
                    {
                        DeselectCube(selectedCube);
                    }
                    else if(selectedCube.movable)
                    {
                        var direction = CalcuDirection(selectedCube, cube);
                        level.TryToStartMovingCube(selectedCube, direction);
                    }
                }
            }
            else
            {
                var cube = GetCubeAtMousePosition();
                if (cube != null && cube is ColorCube colorCube && colorCube.movable)
                {
                    SelectCube(colorCube);
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
    }

    private void SelectCube(ColorCube colorCube)
    {
        colorCube.OnSelected();
        selectedCube = colorCube;
    }

    private void DeselectCube(ColorCube colorCube)
    {
        colorCube.OnDeselected();
        selectedCube = null;
    }

    private bool IsDragRegistered()
    {
        return dragStartCube != null;
    }

    private bool IsCubeSelected()
    {
        return selectedCube != null;
    }

    private MoveDirection CalcuDirection(Cube start, Cube end)
    {
        var startPos = start.position;
        var endPos = end.position;
        Debug.Log(endPos - startPos);
        return new MoveDirection(endPos - startPos);
    }
}