using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MyAssets
{
    public class AudioBar : AudioTerrainMap
    {
        /// <summary>
        /// Creates the first instance of the mesh to be manipulated in UpdateMesh
        /// </summary>
        protected override void Generate()
        {
            //Sets size of vertices array
            vertices = new Vector3[(xSize + 1) * (zSize + 1)];
            //Loops through z axis
            for (int i = 0, z = 0; z <= zSize; z++)
            {
                //Loops through x axis
                for (int x = 0; x <= xSize; x++, i++)
                {
                    //Creates vertices using x and z value
                    vertices[i] = new Vector3(x, 0, z);
                }
            }
            //Sets size of triangles array
            int SizeOfArray = xSize * zSize * 6;

            //Calls create triangles method in AudioTerrainMap
            triangles = CreateTriangles(SizeOfArray);
            //Applies stored vertices and triangles to the mesh component and applies normals
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        /// <summary>
        /// Updates mesh to deform with sound
        /// </summary>
        protected override void UpdateMesh()
        {
            //Sets for loop variables
            int vertMin = 0;
            int vertMax = 18;
            int audioBand = 0;
            int startVert = 17, endVert = 33;

            //Regenerates mesh
            Generate();

            //Loops through each vertice
            for (int i = 0; i < startVert; i++)
            {
                //Checks if the vertice id is even to skip odd values
                if (i % 2 == 0) continue;
                //Checks if the vertice id is within the set range so it only applies to the middle vertice
                if (i + startVert >= startVert + vertMin && i + endVert <= startVert + vertMax)
                {
                    //Applies the frequency of the set audioband to the vertices y value
                    vertices[i + startVert].y += Mathf.Abs(AudioReader.audioBandBuffer[audioBand] * barScale);
                }
                //Increments to change range checks and audioband
                vertMin += 2;
                vertMax += 2;
                audioBand += 1;
            }
            //Applies stored vertices and triangles to the mesh component and applies normals
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }
    }
}
