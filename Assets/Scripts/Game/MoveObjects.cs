using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


enum Direction {
    left,
    right,
    up,
    down
}


public class MoveObjects : MonoBehaviour

{
    public Vector3 rotationPoint;
    public float previousTime;
    private bool buttonPressed = false;
    public float fallTime = 0.8f;
    public static int height = 20;
    public static int width = 10;
    public static Transform[,] grid = new Transform[width, height];

    private Vector2 startTouchPosition;
    private Vector2 currentPosition;
    private Vector2 endTouchPosition;
    private bool stopTouch = false;
    private float swipeRange = 100;
    public float tapRange;



    void changePosition(Direction direction) {
        if (direction == Direction.left && this.buttonPressed) {
            transform.position += new Vector3(-1, 0, 0);
            if (!ValidMove()) {
                transform.position -= new Vector3(-1, 0, 0);
            }
            this.buttonPressed = false;
            
        }

        if (direction == Direction.right && this.buttonPressed) {
            transform.position += new Vector3(1, 0, 0);
            if (!ValidMove()) {
                transform.position -= new Vector3(1, 0, 0);
            }
            this.buttonPressed = false;
            
        }

        if (direction == Direction.up && this.buttonPressed ) {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
            if (!ValidMove()) {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            }
            this.buttonPressed = false;
            
        }

        if (direction == Direction.down && this.buttonPressed ) {
            GetDown();
            this.buttonPressed = false;
            // if (Time.time - previousTime > fallTime / 10) {
            //     GetDown();
            //     if (!ValidMove()) {
            //         transform.position -= new Vector3(0, -1, 0);
            //         AddToGrid();
            //         CheckForLines();

            //         this.enabled = false;
            //         FindObjectOfType<Spawner>().NewTetraminoes();
                    
            //     }
            //     previousTime = Time.time; 
            // }
        }
    }

    public void GetDown() {
        while (ValidMove()) {
            transform.position += new Vector3(0, -1, 0);
        }
        if (!ValidMove()) {
            transform.position -= new Vector3(0, -1, 0);
            AddToGrid();
            CheckForLines();
        }
    }
   
    
    void Update() {

        Swipe();
        
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            this.buttonPressed = true;
            this.changePosition(Direction.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            this.buttonPressed = true;
            this.changePosition(Direction.right);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            this.buttonPressed = true;
            this.changePosition(Direction.up);
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            this.buttonPressed = true;
            this.changePosition(Direction.down);
        }
        if (Time.time - previousTime > fallTime) {
            transform.position += new Vector3(0, -1, 0);
            if (!ValidMove()) {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                CheckForLines();

                this.enabled = false;
                FindObjectOfType<Spawner>().NewTetraminoes();
                
            }
            previousTime = Time.time; 
        }
    }

    public void Swipe() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            startTouchPosition = Input.GetTouch(0).position;
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            currentPosition = Input.GetTouch(0).position;
            Vector2 Distance = currentPosition - startTouchPosition;

            if (!stopTouch) {
                this.buttonPressed = true;
                if (Distance.x < -swipeRange) {
                    this.changePosition(Direction.left);
                    stopTouch = true;
                }
                if (Distance.x > swipeRange) {
                    this.changePosition(Direction.right);
                    stopTouch = true;
                }
                if (Distance.y > swipeRange) {
                    this.changePosition(Direction.up);
                    stopTouch = true;
                }
                if (Distance.y < -swipeRange) {
                    this.changePosition(Direction.down);
                    stopTouch = true;
                }
            }
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
            stopTouch = false;
            endTouchPosition = Input.GetTouch(0).position;
            Vector2 Distance = endTouchPosition - startTouchPosition;

            if (Mathf.Abs(Distance.x) < tapRange && Mathf.Abs(Distance.y) < tapRange) {
                print("tap");
            }
        }


    }

    void CheckForLines() {
        for (int i = height - 1; i >= 0; i--) {
            if (HasLine(i)) {
                DeleteLine(i);
                RowDown(i);
            }
        }
    }


    bool HasLine(int i) {
        for (int j = 0; j < width; j++) {
            if (grid[j, i] == null) {
                return false;
            }
        }
        return true;
    }

    void DeleteLine(int i) {
        for (int j = 0; j < width; j++) {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
        }
    }

    void RowDown(int i) {
        for (int y = i; y < height; y++) {
            for (int j = 0; j < width; j++) {
                if (grid[j, y] != null) {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0); 
                }
            }
        }
    }

    void AddToGrid() {
        foreach ( Transform children in transform) {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            grid[roundedX, roundedY] = children;
        }
    }


    bool ValidMove() {

        foreach ( Transform children in transform) {
        
             int roundedX = Mathf.RoundToInt(children.transform.position.x);
             int roundedY = Mathf.RoundToInt(children.transform.position.y);

             if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height) {
                return false;
             }  

             if (grid[roundedX, roundedY] != null) {
                 return false;
             }
       
        }
        return true;
    }
}
