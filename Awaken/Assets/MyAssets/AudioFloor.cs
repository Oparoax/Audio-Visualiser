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
        public static int depthOfFloor;

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
            //Sets deptOfFloor to define the size of the array in Set History
            depthOfFloor = zSize + 1;
            //Sets size of vertices array
            vertices = new Vector3[(xSize + 1)* (zSize + 1)];
            //Loops through z values
            for (int i = 0, z = 0; z <= zSize; z++)
            {
                //Loops through vertices
                for (int x = 0; x <= xSize; x++, i++)
                {
                    //Applies the amplitude onto the vertices y value
                    vertices[i] = new Vector3(x, AudioReader.Amplitude, z);
                }
            }
            //Sets size of triangles array
            triangles = new int[xSize * zSize * 6];
            //Calls create triangles method in AudioTerrainMap
            CreateTriangles();
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
            //Loops around each row of vertices
            for (int i = 0, iMax = vertices.Length - 1, z = 0; z <= zSize; z++)
            {
                //Loops around vertices in row
                for (int x = 0; x <= xSize; x++, i++, iMax--)
                {
                    //Checks if current band exists
                    if (audioReader.audioBands[z] != null)
                    {
                        //Applies change on mesh vertices by applying the frequency multiplied by a scale value
                        vertices[i].y = audioReader.audioBands[z].frequencies[x] * barScale;
                        vertices[iMax].y = audioReader.audioBands[z].frequencies[x] * barScale;
                        
                    }
                }
            }
            //Calls create triangles method in AudioTerrainMap
            CreateTriangles();
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
