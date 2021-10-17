using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class WorldGen : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] TileBase[] tiles;

    [SerializeField] int worldWidth;
    [SerializeField] int worldHeight;

    [Range(0,100)]
    [SerializeField] int randomFillPercent;
    [SerializeField] int maxLineTileLength;

    [SerializeField] int seed;
    [SerializeField] bool useRandomSeed;

    [SerializeField] float surfaceAverageHeightMultiplier = 10;
    [SerializeField] float surfaceHeightMultiplier = 10;
    [SerializeField] float perlinSpeed = 0.02f;

    private readonly float surfaceHeightPosition = 0.66f;
    private readonly float cavernHeightPosition = 0.5f;
    private readonly float largeCavernHeightPosition = 0.30f;
    private readonly float perlinOffsetMax = 10000f;

    private Vector2 perlinOffset;
    private float perlinAddition;

    private int[] surfaceHeights;
    private int[] cavernLayerHeights;
    private int surfaceHeightAverage;
    private int cavernLayerHeightAverage;
    private int cavernLayerHeight;
    private int largeCavernLayerHeight;
    private int surfaceLayerHeight;

    private bool bigCaves;

    int[,] map;

    private void Start() {

        surfaceLayerHeight = (int)(worldHeight * surfaceHeightPosition);
        cavernLayerHeight = (int)(worldHeight * cavernHeightPosition);
        largeCavernLayerHeight = (int)(worldHeight * largeCavernHeightPosition);

        GenerateWorld();
    }

    private void GenerateWorld() {
        map = new int [worldWidth,worldHeight];

        //GenerateWorldBase();

        //GenerateCaves();

        //GenerateDifferentCaves();

        //AddSand();
        
        //GenerateOre();

        //RenderMap();
    }

    #region Resources

    private void GenerateWorldBase() {

        surfaceHeights = new int[worldWidth];
        surfaceHeightAverage = (int)(worldHeight * surfaceHeightPosition);
        cavernLayerHeights = new int[worldWidth];
        cavernLayerHeightAverage = (int)(worldHeight * cavernHeightPosition);

        SetSeed(seed);

        perlinOffset = new Vector2(UnityEngine.Random.Range(0f, perlinOffsetMax), UnityEngine.Random.Range(0f, perlinOffsetMax));
        perlinAddition = 0;

        FastNoiseLite noise = new FastNoiseLite();

        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetSeed(seed);
        noise.SetFrequency(0.015f);

        noise.SetFractalType(FastNoiseLite.FractalType.None);

        int sandBiomeBorderStart = UnityEngine.Random.Range(worldWidth / 2, worldWidth - 50);   //right half of world
        int sandBiomeBorderEnd = UnityEngine.Random.Range(sandBiomeBorderStart, sandBiomeBorderStart + (int)(worldWidth * .4f) - 50);

        for (int x = 0; x < worldWidth; x++) {

            float noiseX = perlinOffset.x + perlinAddition;
            float noiseY = perlinOffset.y + perlinAddition;

            

            surfaceHeightAverage += (int)((Mathf.Clamp(noise.GetNoise(noiseX, noiseY), 0, 1) - .475f) * surfaceAverageHeightMultiplier);
            surfaceHeights[x] = surfaceHeightAverage + (int)(noise.GetNoise(-noiseX, -noiseY) * surfaceHeightMultiplier);

            cavernLayerHeightAverage += (int)((Mathf.Clamp(noise.GetNoise(noiseX, noiseY), 0, 1) - .475f) * (surfaceAverageHeightMultiplier * .3f));
            cavernLayerHeights[x] = cavernLayerHeightAverage + (int)(noise.GetNoise(-noiseX + 1, -noiseY - 1) * surfaceHeightMultiplier * .45f);

            perlinAddition += perlinSpeed;

            

            for (int y = 0; y < worldHeight; y++) {
                if (IsInMapRange(x,y) && y < (cavernLayerHeights[x] + 4 + UnityEngine.Random.Range(-5,5))) {
                        map[x,y] = 2;

                }
                else if (IsInMapRange(x,y) && y <= surfaceHeights[x]) {

                    if (x > sandBiomeBorderStart + UnityEngine.Random.Range(-2,5) && x < sandBiomeBorderEnd + UnityEngine.Random.Range(-3,5)) {
                        map[x,y] = 5;
                    }
                    else {
                        map[x,y] = 1;
                    }
                }
                else {
                    map[x,y] = 0;
                }
            }
        }
    }

    private void AddSand () {

        FastNoiseLite noise = new FastNoiseLite();

        noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        noise.SetSeed(seed);
        noise.SetFrequency(0.1f);

        noise.SetFractalType(FastNoiseLite.FractalType.PingPong);
        noise.SetFractalOctaves(1);

        noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
        noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);

        noise.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
        noise.SetDomainWarpAmp(25);
        

        for (int x = 0; x < worldWidth; x++) {
            for (int y = (int)(surfaceLayerHeight * 0.85f) + UnityEngine.Random.Range(-5,2); y < worldHeight; y++) {
                if (IsInMapRange(x,y) && noise.GetNoise(x,y) > 0.85f && map[x,y] > 0) {     //1 desert every 800ish tiles
                    map[x,y] = 5;
                }
            }
        }

        List<List<Coord>> smallDirtRegions = GetRegions(1);

        foreach (List<Coord> dirtRegion in smallDirtRegions) {  //fill in dirt isolated in sand
            if (dirtRegion.Count < 200) {     
                foreach (Coord tile in dirtRegion) {
                    map[tile.tileX,tile.tileY] = 5;
                }
            }
        }

        noise.SetFrequency(.8f);

        //put dirt in stone and stone in dirt
        for (int x = 0; x < worldWidth; x++) {
            for (int y = cavernLayerHeight + UnityEngine.Random.Range(-5,2); y < surfaceLayerHeight - 10 - UnityEngine.Random.Range(-3,8); y++) {
                if (IsInMapRange(x,y) && noise.GetNoise(x,y) > 0.75f && map[x,y] == 1) {
                    map[x,y] = 2;
                }
            }
            for (int y = 0; y < cavernLayerHeight + 10 - UnityEngine.Random.Range(-3,8); y++) {
                if (IsInMapRange(x,y) && noise.GetNoise(x,y) > 0.75f && map[x,y] == 2) {
                    map[x,y] = 1;
                }
            }
        }
    }

    private void GenerateOre() {

        FastNoiseLite noise = new FastNoiseLite();

        noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        noise.SetSeed(seed);
        noise.SetFrequency(0.04f);

        noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
        noise.SetFractalOctaves(3);
        noise.SetFractalLacunarity(1.5f);
        noise.SetFractalGain(10);
        noise.SetFractalWeightedStrength(0);

        noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
        noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);

        for (int x = 0; x < worldWidth; x++) {
            for (int y = 0; y < worldHeight; y++) {
                if (IsInMapRange(x,y) && y < cavernHeightPosition && noise.GetNoise(x,y) > 0.2f && map[x,y] > 0) {
                    map[x,y] = 3;
                }
            }
        }

        List<List<Coord>> copperOreRegions = GetRegions(3);

        foreach (List<Coord> oreRegion in copperOreRegions) {   //get the list of tiles in the region
            if (oreRegion.Count < 17f || oreRegion.Count > 50f) {     //if the region is less than wallThresholdSize turn to empty
                foreach (Coord tile in oreRegion) {
                    map[tile.tileX,tile.tileY] = 4;
                }
            }
        }
    }

    #endregion
    
    #region Caverns

    private void GenerateCaves() {    //caves generated with randomfill
        if (useRandomSeed) {
            seed = (int)System.DateTime.Now.Ticks;
        }

        randomFillPercent = 53;
        maxLineTileLength = 7;
        bigCaves = false;

        System.Random pseudoRNG = new System.Random(seed.GetHashCode());

        for (int x = 0; x < worldWidth; x++) {
            for (int y = 0; y < cavernLayerHeight; y++) {

                if (x == 0 || x == worldWidth - 1 || y == 0 || y == cavernLayerHeight - 1) {    //edges of map are walls
                    map[x,y] = 2;
                }
                else {
                    map[x,y] = (pseudoRNG.Next(0,100) < randomFillPercent)? 2 : 0;
                }
            }
        }

        for (int i = 0; i < 3; i++) {
            SmoothCaves();
        }
        ProcessCaves(50, 50);
    }

    private void GenerateDifferentCaves() {

        FastNoiseLite noise = new FastNoiseLite();

        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
        noise.SetFrequency(0.03f);

        noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
        noise.SetFractalOctaves(3);
        noise.SetFractalLacunarity(1.4f);
        noise.SetFractalGain(1.1f);
        noise.SetFractalWeightedStrength(1f);

        for (int x = 0; x < worldWidth; x++) {
            for (int y = 0; y < worldHeight; y++) {
                if (IsInMapRange(x,y) && y < (largeCavernLayerHeight + UnityEngine.Random.Range(-15,15)) && noise.GetNoise(x,y) > 0.5f) {
                    map[x,y] = 0;
                }
                if (IsInMapRange(x,y) && y < (surfaceLayerHeight + UnityEngine.Random.Range(-20,-5)) && noise.GetNoise(x,y) > 0.7f && map[x,y] != 5) {
                    map[x,y] = 0;
                }
            }
        }
    }

    private void SmoothCaves() {
        for (int x = 0; x < worldWidth; x++) {
            for (int y = 0; y < cavernLayerHeight; y++) {

                int neighbourFilledTiles = GetSurroundingFilledTileCount(x, y);

                if (neighbourFilledTiles > 4) {
                    map[x,y] = 2;
                }
                else if (neighbourFilledTiles < 4) {
                    map[x,y] = 0;
                }
            }
        }
    }

    private void ProcessCaves(int wallThresholdSize, int roomThresholdSize) {   //bad method name
        List<List<Coord>> wallRegions = GetRegions(1);
        int wallHoldSize = wallThresholdSize;

        List<List<Coord>> roomRegions = GetRegions(0);
        int roomHoldSize = roomThresholdSize;

        List<Room> survivingRooms = new List<Room>();   //list of rooms that wont be culled

        foreach (List<Coord> roomRegion in roomRegions) {   //get the list of tiles in the region
            if (roomRegion.Count < roomThresholdSize && !bigCaves) {     //if the region is less than roomThresholdSize change all tiles in room to wall
                foreach (Coord tile in roomRegion) {
                    map[tile.tileX,tile.tileY] = 3;
                }
            }
            else {
                survivingRooms.Add(new Room(roomRegion, map));
            }
        }

        survivingRooms.Sort();

        survivingRooms[0].isMainRoom = true;
        survivingRooms[0].isAccessableFromMainRoom = true;

        ConnectClosestRooms(survivingRooms);

        foreach (List<Coord> wallRegion in wallRegions) {   //get the list of tiles in the region
            if (wallRegion.Count < wallThresholdSize) {     //if the region is less than wallThresholdSize turn to empty
                foreach (Coord tile in wallRegion) {
                    map[tile.tileX,tile.tileY] = 0;
                }
            }
        }
    }

    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false) {

        List<Room> roomListA = new List<Room>();    //list of rooms not accessable from main
        List<Room> roomListB = new List<Room>();    //list of rooms accessable from main that will connect to rooms in roomListA

        if (forceAccessibilityFromMainRoom) {
            foreach (Room room in allRooms) {
                if (room.isAccessableFromMainRoom) {
                    roomListB.Add(room);
                }
                else {
                    roomListA.Add(room);
                }
            }
        }
        else {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;
        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool possibleConnectionFound = false;

        foreach(Room roomA in roomListA) {
            if (!forceAccessibilityFromMainRoom) {
                possibleConnectionFound = false;

                if(roomA.connectedRooms.Count > 0) {
                    continue;
                }
            }

            foreach(Room roomB in roomListB) {   //want to compare roomA to every other room connected to main to find closest
                if (roomA == roomB || roomA.IsConnected(roomB)) {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++) {    //loop through all edge tiles in room
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++) {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];

                        if (!IsInMapRange(tileA.tileX, tileA.tileY) || !IsInMapRange(tileB.tileX, tileB.tileY)) {
                            continue;
                        }

                        int distanceBetweenRooms = ((tileA.tileX - tileB.tileX) * (tileA.tileX - tileB.tileX)) + ((tileA.tileY - tileB.tileY) * (tileA.tileY - tileB.tileY));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {  //found new best connection or havent already found connection
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }
            if (possibleConnectionFound && !forceAccessibilityFromMainRoom) {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }
        if (possibleConnectionFound && forceAccessibilityFromMainRoom && bestTileB.tileY < (bigCaves? largeCavernLayerHeight : cavernLayerHeight)) {    //wont connect to "rooms" above surface
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom && bestTileB.tileY < (bigCaves? largeCavernLayerHeight : cavernLayerHeight)) {
            ConnectClosestRooms(allRooms, true);    //any rooms not still connected to main are forced to connect
        }
    }

    private void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {      //creates passage between rooms in caves
        Room.ConnectRooms(roomA, roomB);
        //Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100);

        List<Coord> line = GetLine (tileA, tileB);

        if (line.Count < maxLineTileLength) {  //**still does calculations for other lines, fix that sometime
            foreach (Coord tile in line) {
            DrawCircle(tile, 4);
            }
        }
    }

    private List<List<Coord>> GetRegions(int tileType) {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[worldWidth,cavernLayerHeight];

        for (int x = 0; x < worldWidth; x++) {
            for (int y = 0; y < cavernLayerHeight; y++) {
                if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
                    List<Coord> newRegion = GetRegionTiles(x,y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion) {
                        mapFlags[tile.tileX, tile.tileY] = 1;
                    }
                }
            }
        }

        return regions;
    }

    private List<Coord> GetRegionTiles(int startX, int startY) {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[worldWidth,cavernLayerHeight];
        int tileType = map [startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue (new Coord (startX,startY));
        mapFlags [startX, startY] = 1;  //looked at this tile, set flag to 1

        while (queue.Count > 0) {
            Coord tile = queue.Dequeue(); //returns first item in queue and removes it from queue
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {    //look at 8 surrounding tiles

                    if (IsBelowSurface(x,y) && (y == tile.tileY || x == tile.tileX)) {    //dont look at diagonal tiles
                        if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
                            mapFlags[x,y] = 1;
                            queue.Enqueue(new Coord(x,y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    #endregion

    #region Noise functions
    
    private bool CheckPerlinLevel(Coord tile, float perlinSpeed, float perlinLevel)
    {
        return (Mathf.PerlinNoise(
                    perlinOffset.x + tile.tileX * perlinSpeed,
                    perlinOffset.y + tile.tileY * perlinSpeed) +
                Mathf.PerlinNoise(
                    perlinOffset.x - tile.tileX * perlinSpeed,
                    perlinOffset.y - tile.tileY * perlinSpeed)) / 2f >= perlinLevel;
    }

    #endregion

    #region Useful functions

    private void DrawCircle(Coord c, int radius) {
        for (int x = -radius; x <= radius; x++) {
            for (int y = -radius; y <= radius; y++) {

                if (x*x + y*y <= radius*radius) {   //inside circle
                    int realX = c.tileX + x;
                    int realY = c.tileY + y;

                    if (IsInMapRange(realX, realY)) {
                        map[realX,realY] = 0;
                    }
                }
            }
        }
    }

    private List<Coord> GetLine(Coord from, Coord to) {     //gets list of tile coords in a line from tileA to tileB
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;

        int dx = to.tileX - x;
        int dy = to.tileY - y;

        bool inverted = false;
        int step = Math.Sign(dx);   //x increment (1 or -1)
        int gradientStep = Math.Sign(dy);   //y increment

        int longest = Mathf.Abs (dx);
        int shortest = Mathf.Abs (dy);

        if (longest < shortest) {   //inverted Bresenham line algo
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;

        for (int i = 0; i < longest; i++) {
            line.Add(new Coord(x,y));

            if (inverted) {
                y += step;
            }
            else {
                x += step;
            }

            gradientAccumulation += shortest;

            if (gradientAccumulation >= longest) {
                if (inverted) {
                    x += gradientStep;
                }
                else {
                    y += gradientStep;
                }

                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    Vector3 CoordToWorldPoint(Coord tile) {
        return new Vector3 (-(worldWidth / 2) + .5f + tile.tileX, -(worldHeight / 2) + .5f + tile.tileY);
    }

    private int GetSurroundingFilledTileCount(int gridX, int gridY) {
        int wallCount = 0;
        
        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++) {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++) {  //loops around a 3x3 square surrounding the gridX,gridY tile
                if (IsBelowSurface(neighborX, neighborY)) {
                    if (neighborX != gridX || neighborY != gridY) {
                        if (map[neighborX, neighborY] != 0) {
                            wallCount++;
                        }
                    }
                }
                else {
                    wallCount++;    //more walls edge of map
                }
            }
        }

        return wallCount;
    }

    private bool IsInMapRange(int x, int y) {
        return x >= 0 && x < worldWidth && y >= 0 && y < worldHeight;
    }

    private bool IsBelowSurface(int x, int y) {
        if (bigCaves) {
            return x >= 0 && x < worldWidth && y >= 0 && y < largeCavernLayerHeight;
        }
        else {
            return x >= 0 && x < worldWidth && y >= 0 && y < cavernLayerHeight;
        }
        
    }

    #endregion

    public void SetSeed(int newSeed)
    {
        seed = useRandomSeed ? (int)System.DateTime.Now.Ticks : newSeed;
        UnityEngine.Random.InitState((int)seed);
    }

    public void RenderMap() {

        int tileTypeToRender;

        for (int x = 0; x < worldWidth; x++) {
            for (int y = 0; y < worldHeight; y++) {

                tileTypeToRender = map[x,y];

                if (tileTypeToRender > 0) {
                    tilemap.SetTile(new Vector3Int((int)(-worldWidth/2 + x + .5f), (int)(-worldHeight/2 + y + .5f), 0), tiles[tileTypeToRender]);
                    //tilemap.SetColliderType(new Vector3Int((int)(-worldWidth/2 + x + .5f), (int)(-worldHeight/2 + y + .5f), 0), Tile.ColliderType.Sprite);
                }
                else {
                    tilemap.SetTile(new Vector3Int((int)(-worldWidth/2 + x + .5f), (int)(-worldHeight/2 + y + .5f), 0), null);
                }
            }
        }
    }

    public static void UpdateMap(int[,] map, Tilemap tilemap) {//Takes in our map and tilemap, setting null tiles where needed
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (map[x, y] == 0)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }

    public int GetTileType(int x, int y) {
        return map[x,y];
    }

    public void SetTileType(int x, int y, int type) {
        map[x,y] = type;
        tilemap.SetTile(new Vector3Int((int)(-worldWidth/2 + x + .5f), (int)(-worldHeight/2 + y + .5f), 0), tiles[type]);
    }

    struct Coord {
        public int tileX;
        public int tileY;

        public Coord(int x, int y) {
            tileX = x;
            tileY = y;
        }
    }

    private void OnDrawGizmos() {
        if (map != null) {
            for (int x = 0; x < worldWidth; x++) {
                for (int y = 0; y < worldHeight; y++) {

                    if (map[x,y] == 0) {
                        Gizmos.color = Color.white;
                    }
                    else if (map[x,y] == 1) {
                        Gizmos.color = Color.black;
                    }
                    else if (map[x,y] == 2) {
                        Gizmos.color = Color.gray;
                    }
                    else if (map[x,y] == 3) {
                        Gizmos.color = Color.red;
                    }
                    else if (map[x,y] == 4) {
                        Gizmos.color = Color.green;
                    }
                    else if (map[x,y] == 5) {
                        Gizmos.color = Color.yellow;
                    }
                    Vector3 position = new Vector3(-worldWidth/2 + x + .5f, -worldHeight/2 + y + .5f);
                    Vector3 size = new Vector3(1,1,.1f);
                    Gizmos.DrawCube(position, size);
                }
            }
        }
    }

    class Room : IComparable<Room> {
        public List<Coord> tiles;   //all tiles in room
        public List<Coord> edgeTiles;
        public List<Room> connectedRooms;

        public int roomSize;    //#tiles in room

        public bool isAccessableFromMainRoom;
        public bool isMainRoom;

        public Room() {    //empty room constructor
        }

        public Room(List<Coord> roomTiles, int[,] map) {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();

            foreach (Coord tile in tiles) {

                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {

                        if (x == tile.tileX || y == tile.tileY) {   //exclude diagonal
                            if (map[x,y] > 0) {
                                edgeTiles.Add(tile);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void SetAccessibleFromMainRoom() {
            if (!isAccessableFromMainRoom) {

                isAccessableFromMainRoom = true;

                foreach (Room connectedRoom in connectedRooms) {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB) {
            if (roomA.isAccessableFromMainRoom) {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.isAccessableFromMainRoom) {  //if either room is accessable from the main room, set the other to accessable to main
                roomA.SetAccessibleFromMainRoom();
            }

            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom) {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom) {  //from IComparable interface
            return otherRoom.roomSize.CompareTo(roomSize);  //check if other room is bigger
        }
    }
}
