using UnityEngine;
using UnityEngine.Events;
using System;
using static AudioManipulator;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public abstract class AudioTerrainMap : MonoBehaviour
{
    //Size of grid we wish to make
    public int xSize, ySize, zSize;
    //How big do we want the effect
    public float barScale;
    //Mesh to be manipulated
    public Mesh mesh;
    //Material on the object
    public Material material;
    //Vector3 array of vertices values
    public Vector3[] vertices;
    //Integer array of triangles values
    protected int[] triangles;
    //Variable for AudioManipulator Class
    protected AudioManipulator audioManipulator;

    // Start is called before the first frame update
    void Start()
    {
        //Initialises new mesh
        mesh = new Mesh();
        //Grabs mesh component and sets its mesh = to the mesh variable
        GetComponent<MeshFilter>().mesh = mesh;
        //Generate method to create the grid
        Generate();
        //Defines audioManipulator
        audioManipulator = FindObjectOfType<AudioManipulator>();
        //Adds listener to colour change event to change the mesh emission colour
        audioManipulator.OnColourChanged.AddListener(SetColour);
    }
    void Update()
    {
        //UpdateMesh method to update the mesh each frame
        var audioReader = FindObjectOfType<AudioReader>();
        //Checks if audio ready is ready to start sending values to apply to mesh
        if (!audioReader.isReady) return;
        //Start update mesh method
        UpdateMesh();
    }

    /// <summary>
    /// Method for generating the mesh is abstract so it can be used to make lots of different meshes
    /// </summary>
    protected abstract void Generate();

    /// <summary>
    /// Method for updating mesh is abstract as this too is used for different mesh types
    /// </summary>
    protected abstract void UpdateMesh();

    /// <summary>
    /// Method for creating the triangles for the meshes
    /// </summary>
    protected virtual int[] CreateTriangles(int arraySize)
    {
        //Sets size of triangles array
        triangles = new int[arraySize];
        //Intialises interators
        int vertex = 0;
        int tris = 0;
        //Loops through z axis
        for (int z = 0; z < zSize; z++)
        {
            //Loops through vertices
            for (int x = 0; x < xSize; x++, vertex++)
            {
                //Creates 2 triangles at set coordinates
                triangles[tris + 0] = vertex + 0;
                triangles[tris + 1] = vertex + xSize + 1;
                triangles[tris + 2] = vertex + 1;
                triangles[tris + 3] = vertex + 1;
                triangles[tris + 4] = vertex + xSize + 1;
                triangles[tris + 5] = vertex + xSize + 2;
                //Iterates for next triangle position
                tris += 6;
            }
            //Iterator to create the last triangle in mesh
            vertex++;

        }
        return triangles;
    }

    /// <summary>
    /// Sets emission colour of material to the colour on the event
    /// </summary>
    /// <param name="args">Colour of shader when event triggered</param>
    void SetColour(ColourChangedArgs args)
    {
        //Sets material's emission colour to colour sent by event
        material.SetColor("_emission", args.colour);
    }
}
