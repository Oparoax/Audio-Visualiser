using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.MyAssets
{
    public class AudioFloor : AudioTerrainMap
    {
        //Defines depth of vertices to pass into audioHistory function
        public static int requiredAudioHistory;
        //Which frequency bands are displayed on the floor
        [Range(1,7)] [SerializeField]public int Number_Of_Bands_Displayed;
        //Size of the triangles array to be parsed into the method
        int SizeOfArray;

        /// <summary>
        /// Class variable
        /// </summary>
        private AudioReader audioReader;

        private void Awake()
        {
            //Defines variable
            audioReader = FindObjectOfType<AudioReader>();
        }

        /// <summary>
        /// Generates mesh for Update mesh to deform
        /// </summary>
        protected override void Generate()
        {
            //Sets size of vertices array
            vertices = new Vector3[(xSize + 1)* (zSize + 1)];

            int vertRemainder = 0;
            //Sets deptOfFloor to define the size of the array in Set History
            //Checks for how far back in history we need to store by checking the size of the vertice array against the 
            if (vertices.Length % Number_Of_Bands_Displayed == 0)
            {
                requiredAudioHistory = vertices.Length / Number_Of_Bands_Displayed;
            }
            else
            {
                vertRemainder = vertices.Length % Number_Of_Bands_Displayed;
                requiredAudioHistory = Mathf.RoundToInt(vertices.Length + vertRemainder / Number_Of_Bands_Displayed);
            }
            
            //Loops through z values
            for (int i = 0, z = 0; z <= zSize; z++)
            {
                //Loops through vertices
                for (int x = 0; x <= xSize; x++, i++)
                {
                    //Applies the amplitude onto the vertices y value
                    vertices[i] = new Vector3(x, 0, z);
                }
            }
            //Sets size of triangles array
            SizeOfArray = xSize * zSize * 6;
            //Calls create triangles method in AudioTerrainMap
            triangles = CreateTriangles(SizeOfArray);
            //Applies calculated vertices and teriangles to mesh component and calculates normals
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        /// <summary>
        /// Boolean to stop overlapping in meshchanges
        /// </summary>
        private bool updateReady = true;

        /// <summary>
        /// Updates mesh
        /// </summary>
        protected override void UpdateMesh()
        {
            //Checks boolean if mesh is ready
            if (updateReady)
            {
                //Sets boolean to stop it repeating
                updateReady = false;
                //Starts coroutine to update the mesh
                applyFreqOverTime();
            }
        }

        /// <summary>
        /// Sets the frequency onto the vertices y values but applies older frequencies onto z values 
        /// </summary>
        void applyFreqOverTime()
        {
            //Loops threw the meshes depth
            for (int i = 0, previousBand = 0; i < vertices.Length; previousBand++)
            {
                //Loops through vertices and applys a frequency to the y value
                for (int bandOfAudio = 0; i < vertices.Length && bandOfAudio < Number_Of_Bands_Displayed; bandOfAudio++, i++)
                {
                    //
                    vertices[i].y = audioReader.audioBands[previousBand].frequencies[bandOfAudio] * barScale;
                }
            }

            //Calls create triangles method in AudioTerrainMap
            CreateTriangles(SizeOfArray);
            //Applies calculated vertices and teriangles to mesh component and calculates normals
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            //Boolean to determine if it is ready to start updating the mesh again
            updateReady = true;
        }

        void OnDrawGizmos()
        {
            //Checks if any vertices exist
            if (vertices == null) return;
            //Loops through all vertices in array
            for (int i = 0; i < vertices.Length; i++)
            {
                //Draws sphere on each gizmo
                Gizmos.DrawSphere(vertices[i], .1f);
            }
        }
    }
}
