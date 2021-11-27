using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public abstract class AudioTerrainMap : MonoBehaviour
{
    //Size of grid we wish to make
    public int xSize, ySize, zSize;
    //How big do we want the effect
    public float barScale;
    //Mesh to be manipulated
    public Mesh mesh;

    public Material material;
    //Vector3 array of vertices values
    public Vector3[] vertices;
    //Integer array of triangles values
    protected int[] triangles;


    

    // Start is called before the first frame update
    protected void Start()
    {
        //Initialises new mesh
        mesh = new Mesh();
        //Grabs mesh component and sets its mesh = to the mesh variable
        GetComponent<MeshFilter>().mesh = mesh;
        //Generate method to create the grid
        Generate();

        material.SetColor("_emission", AudioManipulator.newColour);
        OnColourChange.AddListener(delegate { SetColour(); });
    }
    protected void Update()
    {
        //UpdateMesh method to update the mesh each frame
        var audioReader = FindObjectOfType<AudioReader>();
        //Checks if audio ready is ready to start sending values to apply to mesh
        if (!audioReader.isReady) return;
        //Start update mesh method
        UpdateMesh();
    }
    //Method for generating the mesh is abstract so it can be used to make lots of different meshes
    protected abstract void Generate();
    //Method for updating mesh is abstract as this too is used for different mesh types
    protected abstract void UpdateMesh();

    /// <summary>
    /// Method for creating the triangles for the meshes
    /// </summary>
    protected virtual void CreateTriangles()
    {
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
    }

    void SetColour()
    {
        material.SetColor("_emission", AudioManipulator.newColour);
    }
    //Draws gizmos in scene view for debugging

    //void OnDrawGizmos()
    //{
    //    //Checks if any vertices exist
    //    if (vertices == null) return;
    //    //Loops through all vertices in array
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        //Draws sphere on each gizmo
    //        Gizmos.DrawSphere(vertices[i], .1f);
    //    }
    //}
}
