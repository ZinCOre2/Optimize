using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateWorld : MonoBehaviour
{
    [SerializeField] private Renderer world;
    [SerializeField] private Camera cam;
    
    Vector2 lastPoint = Vector2.zero;
    Texture2D texture; //Texture should be set to Read/Write and RGBA32bit in Inspector with no mipmaps
    
    // 0 - white, 1 - yellow, 2 - black, 3 - green, 4 - magenta, 5 - blue
    private int[,] _textureColors;
    private int _textureWidth;
    private int _textureHeight;

    void Start()
    {
        Application.targetFrameRate = 150;
        texture = Instantiate(world.material.mainTexture) as Texture2D;
        _textureWidth = texture.width;
        _textureHeight = texture.height;
        _textureColors = new int[_textureWidth, _textureHeight];

        //add yellow cells using perlin noise to create some patches of "minerals"
        for (int y = 0; y < _textureHeight; y++)
        {
            for (int x = 0; x < _textureWidth; x++)
            {
                if (Mathf.PerlinNoise(x / 20.0f, y / 20.0f) * 100 < 40)
                {
                    texture.SetPixel(x, y, Color.yellow);
                    _textureColors[x, y] = 1;
                }
            }
        }
        
        world.material.mainTexture = texture;
        texture.Apply();
    }

    int CountNeighbourColor(int x, int y, int colorId)
    {
        int count = 0;
        //loop through all 8 neighbouring cells and if their colour
        //is the one passed through then count it
        
        for (int ny = -1; ny <= 1; ny++)
        {
            for (int nx = -1; nx <= 1; nx++)
            {
                if (ny == 0 && nx == 0) continue; //ignore cell you are looking at neighbours
                
                var x2 = (x + nx + _textureWidth) % _textureWidth;
                var y2 = (y + ny + _textureHeight) % _textureHeight;
                
                if (_textureColors[x2, y2] == colorId)
                    count++;
            }
        }
        
        return count;
    }

    void SimulateWorld(int currentPointX, int currentPointY, int lastPointX, int lastPointY)
    {
        int[] pixelCoords = GetLinePixels(currentPointX, currentPointY, lastPointX, lastPointY);

        for (int i = 0; i < pixelCoords.Length; i += 2)
        {
            var xCenter = pixelCoords[i];
            var yCenter = pixelCoords[i + 1];

            for (int nx = -1; nx <= 1; nx++)
            {
                for (int ny = -1; ny <= 1; ny++)
                {
                    var x = (xCenter + nx + _textureWidth) % _textureWidth;
                    var y = (yCenter + ny + _textureHeight) % _textureHeight;
                    
                    var countNeighbour = CountNeighbourColor(x, y, 2);
                    //if a cell has a black neighbour and is not black itself
                    //set to green
                    //Residential Property
                    if (countNeighbour > 0 && _textureColors[x, y] == 0)
                    {
                        texture.SetPixel(x, y, Color.green);
                        _textureColors[x, y] = 3;
                    }
                    //if a cell has more than 4 black neighbours make it blue
                    //Commercial Property
                    else if (countNeighbour > 4)
                    {
                        texture.SetPixel(x, y, Color.blue);
                        _textureColors[x, y] = 5;
                    }
                
                    //if near a black cell but the cell is already yellow
                    //Mining Property
                    if (countNeighbour > 0 && _textureColors[x, y] == 1)
                    {
                        texture.SetPixel(x, y, Color.magenta);
                        _textureColors[x, y] = 4;
                    }

                    //if a cell is blue, green or magenta and has no black next to it then it should die = turn white)
                    //if road is taken away the cell should die/deallocate property
                    if (countNeighbour == 0 &&
                        (_textureColors[x, y] == 3 ||
                         _textureColors[x, y] == 5 ||
                         _textureColors[x, y] == 4))
                    {
                        texture.SetPixel(x, y, Color.white);
                        _textureColors[x, y] = 0;
                    }
                }
            }
        }

        // for (int y = lowestPointY - 1; y <= highestPointY + 1; y++)
        // {
        //     for (int x = lowestPointX - 1; x <= highestPointX + 1; x++)
        //     {
        //         var countNeighbour = CountNeighbourColor(x, y, Color.black);
        //         //if a cell has a black neighbour and is not black itself
        //         //set to green
        //         //Residential Property
        //         if (countNeighbour > 0 && texture.GetPixel(x,y) == Color.white)
        //         {
        //             texture.SetPixel(x, y, Color.green);
        //         }
        //         //if a cell has more than 4 black neighbours make it blue
        //         //Commercial Property
        //         else if (countNeighbour > 4)
        //         {
        //             texture.SetPixel(x, y, Color.blue);
        //         }
        //         
        //         //if near a black cell but the cell is already yellow
        //         //Mining Property
        //         if (countNeighbour > 0 && texture.GetPixel(x, y) == Color.yellow)
        //         {
        //             texture.SetPixel(x, y, Color.magenta);
        //         }
        //
        //         //if a cell is blue, green or magenta and has no black next to it then it should die = turn white)
        //         //if road is taken away the cell should die/deallocate property
        //         if (countNeighbour == 0 &&
        //             (texture.GetPixel(x, y) == Color.green ||
        //             texture.GetPixel(x, y) == Color.blue ||
        //             texture.GetPixel(x, y) == Color.magenta))
        //         {
        //             texture.SetPixel(x, y, Color.white);
        //         }
        //     }
        // }
        texture.Apply();
    }

    void Update()
    {
        //record start of mouse drawing (or erasing) to get the first position the mouse touches down
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            RaycastHit ray;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out ray))
            {
                lastPoint = new Vector2((int)(ray.textureCoord.x * texture.width),
                                        (int)(ray.textureCoord.y * texture.height));
            }
        }
        //draw a line between the last known location of the mouse and the current location
        if (Input.GetMouseButton(0))
        {
            RaycastHit ray;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out ray))
            {
                world.material.mainTexture = texture;

                DrawPixelLine((int)(ray.textureCoord.x * texture.width),
                                   (int)(ray.textureCoord.y * texture.height),
                                   (int)lastPoint.x, (int)lastPoint.y);
                
                SimulateWorld((int)(ray.textureCoord.x * texture.width), 
                    (int)(ray.textureCoord.y * texture.height), 
                    (int)lastPoint.x, (int)lastPoint.y);
                
                texture.Apply();
                lastPoint = new Vector2((int)(ray.textureCoord.x * texture.width),
                                   (int)(ray.textureCoord.y * texture.height));
            }
        }

        if (Input.GetMouseButton(1))
        {
            RaycastHit ray;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out ray))
            {
                world.material.mainTexture = texture;
                texture.SetPixel((int)(ray.textureCoord.x * texture.width),
                                   (int)(ray.textureCoord.y * texture.height), Color.white);
                
                SimulateWorld((int)(ray.textureCoord.x * texture.width), 
                    (int)(ray.textureCoord.y * texture.height), 
                    (int)lastPoint.x, (int)lastPoint.y);
                
                texture.Apply();
            }
        }
    }

    int[] GetLinePixels(int x, int y, int x2, int y2)
    {
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        
        int numerator = longest >> 1;
        int[] pixelCoords = new int[2 * longest + 2];

        for (int i = 0; i <= longest; i++)
        {
            pixelCoords[i * 2] = x;
            pixelCoords[i * 2 + 1] = y;
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }

        return pixelCoords;
    }

    
    //Draw a pixel by pixel line between two points
    //For more information on the algorithm see: https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
    //DO NOT MODIFY OR OPTMISE
    
    void DrawPixelLine(int x, int y, int x2, int y2)
    {
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            texture.SetPixel(x, y, Color.black);
            _textureColors[x, y] = 2;
            
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
        texture.Apply();
    }
}
