
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class gridplacement : MonoBehaviour
{
    public List<GameObject> blockPrefabs;
    public Vector2 size;

    public Transform OtherBlocksParent;

    public float zPos;

    public Vector3[] Xsides;
    public Vector3[] Ysides;
    public Vector3[] sides;

    // public List<Vector2> blockPlacements = new List<Vector2>();

    HashSet<List<int>> mapList = new HashSet<List<int>>();

    public int blockType = 0;
    public int rot = 0;

    public string MapString;

    public ToggleGroup toggleGroup;

    public bool disablePhysicsOnStart = true; 

    List<block> oldblockpos = new List<block>();

    bool inPlayMode;

    public GameObject player;


    

    [SerializeField]
    public GameObject editorCam;
    [SerializeField]
    public GameObject playingCam;

    [SerializeField]
    public Transform msEffectParent;
    [SerializeField]
    public iTween.EaseType EaseType;

    public TMP_InputField MapNameInputField;

    public Transform MapsParent;

    public GameObject MapButtonPrefab;

    public tileMapHandler tileMapHandler;


    // public SimulationMode2D simulationMode;

    // Start is called before the first frame update

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(disablePhysicsOnStart){
            Physics2D.simulationMode = SimulationMode2D.Script;
            Debug.Log("Disabled physics");
        }
    }

    IEnumerator importMapFromFileIE(string MapName)
    {

        clearMap();
        Debug.Log("importing map with name: " + MapName + " and info: " + SaveSystem.ReadString(MapName.Replace(" ", "-")));
        yield return null;
        yield return null;
        importMapAsString(SaveSystem.ReadString(MapName.Replace(" ", "-")));


    }



    void clearMap(){
        clearChildren(OtherBlocksParent);
        tileMapHandler.clearMap();

        mapList = new HashSet<List<int>>();
    }

    void clearChildren(Transform parent){
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
    void importMapFromFile(string MapName){
        StartCoroutine(importMapFromFileIE(MapName));
        
        
    }
    void Start()
    {
        importMapFromFile(MapNameInputField.text);
        ListMapsToLoad(MapButtonPrefab, MapsParent);

        sides = Xsides.Union(Ysides).ToArray();
        HashSet<List<int>> list = exportMap();


        foreach (List<int> row in list)
        {
            foreach (int element in row)
            {
                // Do something with the element
                Debug.Log(element);
            }
        }

        foreach (GameObject b in blockPrefabs)
        {
            GameObject msEffectBlock = Instantiate(b, msEffectParent.position, msEffectParent.rotation, msEffectParent);

            BoxCollider2D boxCollider = msEffectBlock.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                Destroy(boxCollider);
            }
            Rigidbody2D rb = msEffectBlock.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Destroy(rb);
            }

            // // Get the sprite renderer component
            // SpriteRenderer renderer = msEffectBlock.GetComponent<SpriteRenderer>();

            // // Get the current color of the material
            // Color currentColor = renderer.color;

            // // Set the new alpha value (0-1)
            // currentColor.a = 0.5f;

            // // Set the new color back to the sprite renderer
            // renderer.color = currentColor;
        }



    }

    void ListMapsToLoad(GameObject buttonPrefab, Transform mapListParent){
        clearChildren(mapListParent);
        string[] mapNames = SaveSystem.getAllSavedMapNames();

        for (int i = 0; i < mapNames.Length; i++)
        {
            addButtonToMapList(buttonPrefab, mapListParent, mapNames[i]);
        }
    }

    void addButtonToMapList(GameObject buttonPrefab, Transform mapListParent, string mapName){
        GameObject newButton = Instantiate(buttonPrefab, mapListParent);
        changeTextOfButton(newButton, mapName);
        newButton.GetComponent<Button>().onClick.AddListener(() => importMapFromFile(mapName));
        
        
    }

    void changeTextOfButton(GameObject button, string newText){
        button.GetComponentInChildren<TMP_Text>().text = newText;
    }

    

    // Update is called once per frame
    void Update()
    {
        // blockType += Mathf.RoundToInt(Input.mouseScrollDelta.y);
        if(!inPlayMode){

            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            int x = Mathf.RoundToInt(worldPosition.x / 2.5f);
            int y = Mathf.RoundToInt(worldPosition.y / 2.5f);
            if(!EventSystem.current.IsPointerOverGameObject()){
                
                if(Input.GetMouseButton(0) ){
                    

                    if(blockType == toggleGroup.transform.childCount - 1){
                        removeBlock(x, y);
                        
                    }
                    else {
                        addBlock(x, y, blockType, rot);
                    }

                }
                rot = (rot + Mathf.RoundToInt(Input.mouseScrollDelta.y)) % 4 ;
            }
            

            if(blockType != toggleGroup.transform.childCount - 1){

                GameObject child = msEffectParent.GetChild(blockType).gameObject;
                iTween.RotateTo(child, iTween.Hash("z", rot * 90, "time", 0.1));
                iTween.MoveTo(child, iTween.Hash("x", x * 2.5f, "y", y * 2.5f, "time", 0.1, "easetype", EaseType));  
            }
        }
        
    }

    HashSet<List<int>> exportMap(){

        return mapList;
    }

    // void saveMapToBinnary(){
    //     int[,] map = {{0, 0}, {0, 0}};
    //     foreach (List<int> i in mapList)
    //     {
    //         for (int b = 0; b < i.Count(); b++)
    //         {
    //             map[a, b] = i[b];
    //         }
    //     }
    //     SaveSystem.SaveMap(map);
    // }
    public string exportMapAsString(){
        String MapString = "";
        foreach (List<int> row in mapList)
        {
            foreach (int element in row)
            {
                // Do something with the element
                
                MapString += element;
                MapString += ' ';

            
            
            }
            MapString += ',';
        }
        // Debug.Log(MapString);
        SaveSystem.WriteString(MapNameInputField.text.Replace(" ", "-"), MapString);
        return MapString;
    }

    public void hellooo(){
        exportMapAsString();
        ListMapsToLoad(MapButtonPrefab, MapsParent);
    }

    void importMapAsString(string mapStr){
        foreach (string bstring in mapStr.Split(','))
        {
            if(bstring == "") continue;

            string[] elements = bstring.Split(' ');
            int[] numbers = new int[elements.Length];

            for (int i = 0; i < elements.Length; i++)
            {
                if(elements[i] == "") continue;
                numbers[i] = int.Parse(elements[i]);
            }
            addBlock(
                numbers[0],
                numbers[1],
                numbers[2],
                numbers[3]
            );
        }

    }

    void removeBlock(int x, int y){
        for (int b = 0; b < OtherBlocksParent.transform.childCount; b++)
        {
            Transform block = OtherBlocksParent.transform.GetChild(b);

            if(block.position.x == x * 2.5f && block.position.y == y * 2.5f){
                Destroy(block.gameObject);

                // var itemToRemove = mapList.Where(r => r[0] == x && r[1] == y);
                mapList.RemoveWhere(r => r[0] == x && r[1] == y);
                exportMapAsString();
                return;
            }


        }

        tileMapHandler.changeBlock(x, y, false);
    }



    void addBlock(int x, int y, int blockType, int Rotation){
        Vector3 worldPosition = new Vector3(x * 2.5F, y * 2.5F, zPos);


        List<int> block = new List<int>() {
                                x, 
                                y,
                                blockType,
                                Rotation
                            };

        if(mapList.Any(x => x.SequenceEqual(block))) return;


        mapList.Add(block);


        // MapString = exportMapAsString();
        
        if(blockType != 0){
            worldPosition += Vector3.back * 2;
            Instantiate(blockPrefabs[blockType], worldPosition, Quaternion.Euler(0, 0, Rotation * 90), OtherBlocksParent);
            return;
        }
        
        tileMapHandler.changeBlock(x, y, true);

        


            

            // if(child.)
    }

    bool isBlockHere(Vector3 pos, List<int> bb){
        return pos.x == bb[0] && pos.y == bb[1];
    }

    

    public void updateSelectedItem(){

        Toggle toggle = toggleGroup.ActiveToggles().FirstOrDefault();
        Debug.Log(toggle.transform.GetSiblingIndex());
        blockType = toggle.transform.GetSiblingIndex();
    }
    
    public void StartOrStopSim(){
        if(inPlayMode) StopSim();
        else StartSim();
    }

    void StartSim(){

        

        swichCam(true);
        oldblockpos.Clear();

        for (int b = 0; b < OtherBlocksParent.transform.childCount; b++)
        {
            GameObject block = OtherBlocksParent.transform.GetChild(b).gameObject;

            if(block.GetComponent<Rigidbody2D>() != null){
                oldblockpos.Add(new block(block.gameObject));

            }

        }

        oldblockpos.Add(new block(player));

        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        


    }
    void StopSim(){


        swichCam(false);
        Physics2D.simulationMode = SimulationMode2D.Script;
        foreach (block b in oldblockpos)
        {
            b.resetTransform();
        }
    }

    void swichCam(bool isPlaying){
        inPlayMode = isPlaying;
        editorCam.SetActive(!isPlaying);
        playingCam.SetActive(isPlaying);
    }
}








class block{

    private Vector3 pos;
    private Quaternion rot;
    private GameObject gameObject;
    public block( GameObject igameObject){
        pos = igameObject.transform.position;
        rot = igameObject.transform.rotation;
        gameObject = igameObject;
    }

    public void resetTransform(){
        gameObject.transform.position = pos;
        gameObject.transform.rotation = rot;

        // Debug.Log("eff");

    }
}