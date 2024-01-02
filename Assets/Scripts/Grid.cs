using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

namespace Spinach
{
    public enum DockPosition
    {
        Top,
        Right,
        Bottom,
        Left
    }

    /// <summary>
    /// A non-generic grid that stores ints
    /// </summary>
    public class Grid : Grid<int>
    {
        public Grid(int width, int height, float cellSize, Vector3 originPosition) : base(width, height, cellSize, originPosition) { }
    }

    public class Grid<TGridObject>
    {
        int width;
        int height;
        float cellSize;
        Vector3 originPosition;
        TGridObject[,] gridArray;
        bool[,] usableArray;

        public int GetWidth() { return width; }
        public int GetHeight() { return height; }

        public class OnGridValueChangedEventArgs : System.EventArgs
        {
            public int x;
            public int y;
            public TGridObject value;
        }

        public event System.EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;

        /// <summary>
        /// Creates a grid that is docked to the specified position
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="dockPosition"></param>
        /// <returns></returns>
        public static Grid<TGridObject> DockedGrid(int width, int height, DockPosition dockPosition, float cellSize)
        {
            Vector3 originPosition = CalculateOriginPosition(width, height, cellSize, dockPosition);
            return new Grid<TGridObject>(width, height, cellSize, originPosition);
        }

        /// <summary>
        /// Calculates the origin position of the grid based on the dock position
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="cellSize"></param>
        /// <param name="dockPosition"></param>
        /// <returns></returns>
        private static Vector3 CalculateOriginPosition(int width, int height, float cellSize, DockPosition dockPosition)
        {
            switch (dockPosition)
            {
                case DockPosition.Top:
                    return new Vector3(-width * cellSize / 2f, Camera.main.orthographicSize - height * cellSize);
                case DockPosition.Right:
                    return new Vector3(Camera.main.orthographicSize * Camera.main.aspect - width * cellSize, -height * cellSize / 2f);
                case DockPosition.Bottom:
                    return new Vector3(-width * cellSize / 2f, -Camera.main.orthographicSize);
                case DockPosition.Left:
                    return new Vector3(-Camera.main.orthographicSize * Camera.main.aspect, -height * cellSize / 2f);
                default:
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// Creates a grid that fits the screen and is centered
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>false
        /// <returns></returns>
        public static Grid<TGridObject> FitAndCenteredGrid(int width, int height)
        {
            float screenHeight = Camera.main.orthographicSize * 2f * 0.85f;
            float screenWidth = screenHeight * Camera.main.aspect;
            float cellSize = Mathf.Min(screenWidth / width, screenHeight / height);
            return CenteredGrid(width, height, cellSize);
        }

        /// <summary>
        /// Creates a centered grid with the specified cell size
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Grid<TGridObject> CenteredGrid(int width, int height, float cellSize)
        {
            Vector3 originPosition = new Vector3(-width * cellSize / 2f, -height * cellSize / 2f);
            return new Grid<TGridObject>(width, height, cellSize, originPosition);
        }

        public float GetCellSize() { return cellSize; }

        public Grid(int width, int height, float cellSize, Vector3 originPosition)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;
            this.gridArray = new TGridObject[width, height];
            this.usableArray = GetDefaultUsableArray(width, height);

            var debug = true;
            if (debug)
            {
                TextMesh[,] debugTextArray = new TextMesh[width, height];
                for (int x = 0; x < gridArray.GetLength(0); x++)
                {
                    for (int y = 0; y < gridArray.GetLength(1); y++)
                    {
                        //debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 20, Color.white, TextAnchor.MiddleCenter);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), color: Color.white, duration: 100f);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), color: Color.white, duration: 100f);
                    }
                }

                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), color: Color.white, duration: 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), color: Color.white, duration: 100f);

                // OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
                // {
                //     if (debugTextArray[eventArgs.x, eventArgs.y] != null)
                //     { debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y].ToString(); }
                // };
            }
        }

        private bool[,] GetDefaultUsableArray(int width, int height)
        {
            var usableArray = new bool[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    usableArray[x, y] = true;
                }
            }
            return usableArray;
        }

        public bool IsUsable(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return usableArray[x, y];
            }
            else
            {
                return false;
            }
        }

        public void SetUsable(bool[,] _usableArray)
        {
            this.usableArray = _usableArray;
        }

        public void SetUsable(int x, int y, bool _usable)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                usableArray[x, y] = _usable;
            }
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * cellSize + originPosition;
        }

        public Vector3 GetCellCenter(int x, int y)
        {
            return GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f;
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        }

        public bool HasWorldPosition(Vector3 worldPosition)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        public void SetValue(int x, int y, TGridObject value)
        {
            if (x >= 0 && y >= 0 && x < width && y < height && IsUsable(x, y))
            {
                gridArray[x, y] = value;
                OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y, value = value });
            }
        }

        public void SetValue(Vector3 worldPosition, TGridObject value)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            SetValue(x, y, value);
        }

        public TGridObject GetValue(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return gridArray[x, y];
            }
            else
            {
                return default;
            }
        }

        public TGridObject GetValue(Vector3 worldPosition)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return GetValue(x, y);
        }

        public IEnumerable<TGridObject> Each()
        {
            foreach (TGridObject item in gridArray)
            {
                yield return item;
            }
        }
    }
}