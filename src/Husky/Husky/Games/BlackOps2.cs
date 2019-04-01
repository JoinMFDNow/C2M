﻿// ------------------------------------------------------------------------
// Husky - Call of Duty BSP Extractor
// Copyright (C) 2018 Philip/Scobalula
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// ------------------------------------------------------------------------
using PhilLibX;
using PhilLibX.IO;
using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using System.Threading;
using System.Globalization;

namespace Husky
{
    /// <summary>
    /// Bo2 Logic
    /// </summary>
    class BlackOps2
    {
        /// <summary>
        /// Bo2 GfxMap Asset (some pointers we skip over point to DirectX routines, etc. if that means anything to anyone)
        /// </summary>
        /// 


        public class XModelsJson
        {
            public Dictionary<int, IDictionary> XModels { get; set; }


        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct GfxMap
        {
            /// <summary>
            /// A pointer to the name of this GfxMap Asset
            /// </summary>
            public int NamePointer { get; set; }

            /// <summary>
            /// A pointer to the name of the map 
            /// </summary>
            public int MapNamePointer { get; set; }

            /// <summary>
            /// Unknown Bytes (Possibly counts for other data we don't care about)
            /// </summary>
            public fixed byte Padding[8];

            /// <summary>
            /// Number of Surfaces
            /// </summary>
            public int SurfaceCount { get; set; }

            /// <summary>
            /// Unknown Bytes (Possibly counts, pointers, etc. for other data we don't care about)
            /// </summary>
            public fixed byte Padding1[0x194];

            /// <summary>
            /// Number of Gfx Vertices (XYZ, etc.)
            /// </summary>
            public int GfxVertexCount { get; set; }

            /// <summary>
            /// Raw size of the Gfx Vertex Buffer
            /// </summary>
            public int GfxVertexBufferSize { get; set; }

            /// <summary>
            /// Pointer to the Gfx Vertex Data
            /// </summary>
            public int GfxVerticesPointer { get; set; }

            /// <summary>
            /// Unknown Bytes (more BSP data we probably don't care for)
            /// </summary>
            public fixed byte Padding3[0x10];

            /// <summary>
            /// Number of Gfx Indices (for Faces)
            /// </summary>
            public int GfxIndicesCount { get; set; }

            /// <summary>
            /// Pointer to the Gfx Index Data
            /// </summary>
            public int GfxIndicesPointer { get; set; }

            /// <summary>
            /// Unknown Bytes (more BSP data we probably don't care for)
            /// </summary>
            public fixed byte Padding4[0x144];

            /// <summary>
            /// Number of Static Models
            /// </summary>
            public int GfxStaticModelsCount { get; set; }

            /// <summary>
            /// Unknown Bytes (more BSP data we probably don't care for)
            /// </summary>
            public fixed byte Padding5[0x54];

            /// <summary>
            /// Pointer to the Gfx Index Data
            /// </summary>
            public int GfxSurfacesPointer { get; set; }

            /// <summary>
            /// Pointer to the Gfx Static Models
            /// </summary>
            public int GfxStaticModelsPointer { get; set; }
        }

        /// <summary>
        /// Gfx Map Surface
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct GfxSurface
        {
            /// <summary>
            /// Unknown Bytes
            /// </summary>
            public fixed byte Padding[0xC];

            /// <summary>
            /// offset of the vertex chunk within the vertex buffer 
            /// </summary>
            public int VertexBufferOffset { get; set; }

            /// <summary>
            /// Unknown Bytes
            /// </summary>
            public fixed byte Padding1[0x10];

            /// <summary>
            /// Base Vertex Index (this is what allows the GfxMap to have 65k+ verts with only 2 byte indices)
            /// </summary>
            public int VertexIndex { get; set; }

            /// <summary>
            /// Null Padding
            /// </summary>
            public int Padding2 { get; set; }

            /// <summary>
            /// Number of Vertices this surface has
            /// </summary>
            public ushort VertexCount { get; set; }

            /// <summary>
            /// Number of Faces this surface has
            /// </summary>
            public ushort FaceCount { get; set; }

            /// <summary>
            /// Base Face Index (this is what allows the GfxMap to have 65k+ faces with only 2 byte indices)
            /// </summary>
            public int FaceIndex { get; set; }

            /// <summary>
            /// Pointer to the Material Asset of this Surface
            /// </summary>
            public int MaterialPointer { get; set; }

            /// <summary>
            /// Unknown Bytes
            /// </summary>
            public fixed byte Padding4[0x1C];
        }

        /// <summary>
        /// Gfx Vertex 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct GfxVertex
        {
            /// <summary>
            /// X Position
            /// </summary>
            public float X { get; set; }

            /// <summary>
            /// Y Position
            /// </summary>
            public float Y { get; set; }

            /// <summary>
            /// Z Position
            /// </summary>
            public float Z { get; set; }

            /// <summary>
            /// Bi Normal
            /// </summary>
            public float BiNormal { get; set; }

            /// <summary>
            /// RGBA Color
            /// </summary>
            public GfxColor Color { get; set; }

            /// <summary>
            /// U Texture Position
            /// </summary>
            public ushort U { get; set; }

            /// <summary>
            /// V Texture Position
            /// </summary>
            public ushort V { get; set; }

            /// <summary>
            /// Packed Vertex Normal (same as XModels)
            /// </summary>
            public PackedUnitVector Normal { get; set; }

            /// <summary>
            /// Unknown Bytes (Possibly Tangent)
            /// </summary>
            public int Padding2 { get; set; }

            /// <summary>
            /// Unknown Bytes (Possibly Tangent)
            /// </summary>
            public int Padding3 { get; set; }
        }

        /// <summary>
        /// Gfx Static Model
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct GfxStaticModel
        {
            /// <summary>
            /// Null Padding
            /// </summary>
            public int Padding { get; set; }

            /// <summary>
            /// X Origin
            /// </summary>
            public float X { get; set; }

            /// <summary>
            /// Y Origin
            /// </summary>
            public float Y { get; set; }

            /// <summary>
            /// Z Origin
            /// </summary>
            public float Z { get; set; }

            /// <summary>
            /// 3x3 Rotation Matrix
            /// </summary>
            public fixed float Matrix[9];

            /// <summary>
            /// Model Scale 
            /// </summary>
            public float ModelScale { get; set; }

            /// <summary>
            /// Pointer to the XModel Asset
            /// </summary>
            public int ModelPointer { get; set; }

            /// <summary>
            /// Unknown Bytes
            /// </summary>
            public fixed byte UnknownBytes2[0x5C];
        }

        /// <summary>
        /// Call of Duty: Black Ops 2 Material Asset
        /// </summary>
        public unsafe struct Material
        {
            /// <summary>
            /// A pointer to the name of this material
            /// </summary>
            public int NamePointer { get; set; }

            /// <summary>
            /// Unknown Bytes (Flags, settings, etc.)
            /// </summary>
            public fixed byte UnknownBytes[0x50];

            /// <summary>
            /// Number of Images this Material has
            /// </summary>
            public byte ImageCount { get; set; }

            /// <summary>
            /// Unknown Bytes (Flags, settings, etc.)
            /// </summary>
            public fixed byte UnknownBytes1[0x7];

            /// <summary>
            /// A pointer to the Tech Set this Material uses
            /// </summary>
            public int TechniqueSetPointer { get; set; }

            /// <summary>
            /// A pointer to this Material's Image table
            /// </summary>
            public int ImageTablePointer { get; set; }

            /// <summary>
            /// Padding and Unknown Pointer
            /// </summary>
            public long Padding { get; set; }
        }

        /// <summary>
        /// Reads BSP Data
        /// </summary>
        public static void ExportBSPData(ProcessReader reader, long assetPoolsAddress, long assetSizesAddress, string gameType, Action<object> printCallback = null)
        {
            // Found her
            printCallback?.Invoke("Found supported game: Call of Duty: Black Ops 2");
            // Validate by XModel Name
            if (reader.ReadNullTerminatedString(reader.ReadInt32(reader.ReadInt32(assetPoolsAddress + 0x14) + 4)) == "defaultvehicle")
            {
                // Load BSP Pools (they only have a size of 1 so we have no free header)
                var gfxMapAsset = reader.ReadStruct<GfxMap>(reader.ReadInt32(assetPoolsAddress + 0x44));

                // Name
                string gfxMapName = reader.ReadNullTerminatedString(gfxMapAsset.NamePointer);
                string mapName = reader.ReadNullTerminatedString(gfxMapAsset.MapNamePointer);

                // Verify a BSP is actually loaded (if in base menu, etc, no map is loaded)
                if (String.IsNullOrWhiteSpace(gfxMapName))
                {
                    printCallback?.Invoke("No BSP loaded. Enter Main Menu or a Map to load in the required assets.");
                }
                else
                {
                    // New IW Map
                    var mapFile = new IWMap();
                    // Print Info
                    printCallback?.Invoke(String.Format("Loaded Gfx Map     -   {0}", gfxMapName));
                    printCallback?.Invoke(String.Format("Loaded Map         -   {0}", mapName));
                    printCallback?.Invoke(String.Format("Vertex Count       -   {0}", gfxMapAsset.GfxVertexCount));
                    printCallback?.Invoke(String.Format("Indices Count      -   {0}", gfxMapAsset.GfxIndicesCount));
                    printCallback?.Invoke(String.Format("Surface Count      -   {0}", gfxMapAsset.SurfaceCount));
                    printCallback?.Invoke(String.Format("Model Count        -   {0}", gfxMapAsset.GfxStaticModelsCount));

                    // Build output Folder
                    string outputName = Path.Combine("exported_maps", "black_ops_2", gameType, mapName, mapName);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputName));

                    // Stop watch
                    var stopWatch = Stopwatch.StartNew();

                    // Read Vertices
                    printCallback?.Invoke("Parsing vertex data....");
                    var vertexBuffer = reader.ReadBytes(gfxMapAsset.GfxVerticesPointer, gfxMapAsset.GfxVertexBufferSize);
                    printCallback?.Invoke(String.Format("Parsed vertex data in {0:0.00} seconds.", stopWatch.ElapsedMilliseconds / 1000.0));

                    // Reset timer
                    stopWatch.Restart();

                    // Read Indices
                    printCallback?.Invoke("Parsing surface indices....");
                    var indices = ReadGfxIndices(reader, gfxMapAsset.GfxIndicesPointer, gfxMapAsset.GfxIndicesCount);
                    printCallback?.Invoke(String.Format("Parsed indices in {0:0.00} seconds.", stopWatch.ElapsedMilliseconds / 1000.0));

                    // Reset timer
                    stopWatch.Restart();

                    // Read Indices
                    printCallback?.Invoke("Parsing surfaces....");
                    var surfaces = ReadGfxSufaces(reader, gfxMapAsset.GfxSurfacesPointer, gfxMapAsset.SurfaceCount);
                    printCallback?.Invoke(String.Format("Parsed surfaces in {0:0.00} seconds.", stopWatch.ElapsedMilliseconds / 1000.0));

                    // Reset timer
                    stopWatch.Restart();

                    // Write OBJ
                    printCallback?.Invoke("Converting to OBJ....");

                    // Create new OBJ
                    var obj = new WavefrontOBJ();

                    // Image Names (for Search String)
                    HashSet<string> imageNames = new HashSet<string>();

                    // Vertex Index Tracker
                    int vertexIndex = 0;

                    // Append Faces
                    foreach (var surface in surfaces)
                    {
                        // Create new Material
                        var material = ReadMaterial(reader, surface.MaterialPointer);

                        // Add to images
                        imageNames.Add(material.DiffuseMap);
                        imageNames.Add(material.NormalMap);
                        imageNames.Add(material.SpecularMap);

                        // Add it
                        obj.AddMaterial(material);

                        // Add points
                        for (ushort i = 0; i < surface.FaceCount; i++)
                        {
                            // Face Indices
                            var faceIndex1 = indices[i * 3 + surface.FaceIndex];
                            var faceIndex2 = indices[i * 3 + surface.FaceIndex + 1];
                            var faceIndex3 = indices[i * 3 + surface.FaceIndex + 2];

                            // Validate unique points, and write to OBJ
                            if (faceIndex1 != faceIndex2 && faceIndex1 != faceIndex3 && faceIndex2 != faceIndex3)
                            {
                                // new Obj Face
                                var objFace = new WavefrontOBJ.Face(material.Name);

                                // Unpack vertices
                                var vertex1 = UnpackVertex(ByteUtil.BytesToStruct<GfxVertex>(vertexBuffer, surface.VertexBufferOffset + (faceIndex1 * 36)));
                                var vertex2 = UnpackVertex(ByteUtil.BytesToStruct<GfxVertex>(vertexBuffer, surface.VertexBufferOffset + (faceIndex2 * 36)));
                                var vertex3 = UnpackVertex(ByteUtil.BytesToStruct<GfxVertex>(vertexBuffer, surface.VertexBufferOffset + (faceIndex3 * 36)));

                                // Add Offsets
                                obj.Vertices.Add(vertex1.Position);
                                obj.Vertices.Add(vertex2.Position);
                                obj.Vertices.Add(vertex3.Position);

                                // Add Normals
                                obj.Normals.Add(vertex1.Normal);
                                obj.Normals.Add(vertex2.Normal);
                                obj.Normals.Add(vertex3.Normal);

                                // Add UVs
                                obj.UVs.Add(vertex1.UV);
                                obj.UVs.Add(vertex2.UV);
                                obj.UVs.Add(vertex3.UV);

                                // Add points
                                objFace.Vertices[0] = new WavefrontOBJ.Face.Vertex(vertexIndex, vertexIndex, vertexIndex);
                                objFace.Vertices[2] = new WavefrontOBJ.Face.Vertex(vertexIndex + 1, vertexIndex + 1, vertexIndex + 1);
                                objFace.Vertices[1] = new WavefrontOBJ.Face.Vertex(vertexIndex + 2, vertexIndex + 2, vertexIndex + 2);

                                // Add to OBJ
                                obj.Faces.Add(objFace);

                                vertexIndex += 3;
                            }
                        }
                    }

                    // Save it
                    obj.Save(outputName + ".obj");

                    // Build search strinmg
                    string searchString = "";
                    // Loop through images, and append each to the search string (for Wraith/Greyhound)
                    foreach (string imageName in imageNames)
                        searchString += String.Format("{0},", Path.GetFileNameWithoutExtension(imageName));

                    // Loop through xmodels, and append each to the search string (for Wraith/Greyhound)
                    List<string> xmodelList = CreateXModelList(reader, gfxMapAsset.GfxStaticModelsPointer, (int)gfxMapAsset.GfxStaticModelsCount);

                    // Dump it
                    File.WriteAllText(outputName + "_search_string.txt", searchString);
                    File.WriteAllText(outputName + "_xmodelList.txt", String.Join(",", xmodelList.ToArray()));

                    // Create .JSON with XModel Data
                    Dictionary<int, IDictionary> ModelData = CreateXModelDictionary(reader, gfxMapAsset.GfxStaticModelsPointer, (int)gfxMapAsset.GfxStaticModelsCount);
                    XModelsJson ModelJson = new XModelsJson()
                    {
                        XModels = ModelData
                    };
                    using (StreamWriter file = File.CreateText(@outputName + ".json"))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(file, ModelJson);
                    }

                    // Read entities and dump to map
                    mapFile.Entities.AddRange(ReadStaticModels(reader, gfxMapAsset.GfxStaticModelsPointer, (int)gfxMapAsset.GfxStaticModelsCount));
                    mapFile.DumpToMap(outputName + ".map");

                    // Done
                    printCallback?.Invoke(String.Format("Converted to OBJ in {0:0.00} seconds.", stopWatch.ElapsedMilliseconds / 1000.0));
                }

            }
            else
            {
                printCallback?.Invoke("Call of Duty: Black Ops 2 is supported, but this EXE is not.");
            }
        }

        /// <summary>
        /// Reads Gfx Surfaces
        /// </summary>
        public static GfxSurface[] ReadGfxSufaces(ProcessReader reader, long address, int count)
        {
            // Preallocate short array
            GfxSurface[] surfaces = new GfxSurface[count];

            // Loop number of indices we have
            for (int i = 0; i < count; i++)
                // Add it
                surfaces[i] = reader.ReadStruct<GfxSurface>(address + i * 80);

            // Done
            return surfaces;
        }


        /// <summary>
        /// Reads Gfx Vertex Indices
        /// </summary>
        public static ushort[] ReadGfxIndices(ProcessReader reader, long address, int count)
        {
            // Preallocate short array
            ushort[] indices = new ushort[count];
            // Read buffer
            var byteBuffer = reader.ReadBytes(address, count * 2);
            // Copy buffer 
            Buffer.BlockCopy(byteBuffer, 0, indices, 0, byteBuffer.Length);
            // Done
            return indices;
        }

        /// <summary>
        /// Unpacks a vertex 
        /// </summary>
        /// <param name="packedVertex">Vertex to unpack</param>
        /// <returns></returns>
        public static Vertex UnpackVertex(GfxVertex packedVertex)
        {
            return new Vertex()
            {
                // Set offset
                Position = new Vector3(
                        packedVertex.X * 2.54,
                        packedVertex.Y * 2.54,
                        packedVertex.Z * 2.54),
                // Decode and set normal (from DTZxPorter - Wraith, same as XModels)
                Normal = VertexNormalUnpacking.MethodC(packedVertex.Normal),
                // Set UV
                UV = new Vector2(HalfFloats.ToFloat(packedVertex.U), 1 - HalfFloats.ToFloat(packedVertex.V))
            };
        }

        /// <summary>
        /// Reads a material for the given surface and its associated images
        /// </summary>
        public static WavefrontOBJ.Material ReadMaterial(ProcessReader reader, long address)
        {
            // Read Material
            var material = reader.ReadStruct<Material>(address);
            // Create new OBJ Image
            var objMaterial = new WavefrontOBJ.Material(Path.GetFileNameWithoutExtension(reader.ReadNullTerminatedString(reader.ReadInt32(address)).Replace("*", "").Replace(")", "").Replace("(", "")));
            // Loop over images
            for (byte i = 0; i < material.ImageCount; i++)
            {
                // Read Material Image
                var materialImage = reader.ReadStruct<MaterialImage32A>(material.ImageTablePointer + i * Marshal.SizeOf<MaterialImage32A>());
                // Check for color map for now
                if (materialImage.SemanticHash == 0xA0AB1041)
                    objMaterial.DiffuseMap = reader.ReadNullTerminatedString(reader.ReadInt32(materialImage.ImagePointer + 0x48));
                else if (materialImage.SemanticHash == 0x59D30D0F)
                    objMaterial.NormalMap = reader.ReadNullTerminatedString(reader.ReadInt32(materialImage.ImagePointer + 0x48));
                else if (materialImage.SemanticHash == 0x34ECCCB3)
                    objMaterial.SpecularMap = reader.ReadNullTerminatedString(reader.ReadInt32(materialImage.ImagePointer + 0x48));
            }
            // Done
            return objMaterial;
        }

        /// <summary>
        /// Reads Static Models
        /// </summary>
        public unsafe static List<IWMap.Entity> ReadStaticModels(ProcessReader reader, long address, int count)
        {
            // Resulting Entities
            List<IWMap.Entity> entities = new List<IWMap.Entity>(count);
            // Read buffer
            var byteBuffer = reader.ReadBytes(address, count * Marshal.SizeOf<GfxStaticModel>());
            // Loop number of models we have
            for (int i = 0; i < count; i++)
            {
                // Read Struct
                var staticModel = ByteUtil.BytesToStruct<GfxStaticModel>(byteBuffer, i * Marshal.SizeOf<GfxStaticModel>());
                // Model Name
                var modelName = reader.ReadNullTerminatedString(reader.ReadInt32(staticModel.ModelPointer));
                // New Matrix
                var matrix = new Rotation.Matrix();
                // Copy X Values
                matrix.Values[0] = staticModel.Matrix[0];
                matrix.Values[1] = staticModel.Matrix[1];
                matrix.Values[2] = staticModel.Matrix[2];
                // Copy Y Values
                matrix.Values[4] = staticModel.Matrix[3];
                matrix.Values[5] = staticModel.Matrix[4];
                matrix.Values[6] = staticModel.Matrix[5];
                // Copy Z Values
                matrix.Values[8] = staticModel.Matrix[6];
                matrix.Values[9] = staticModel.Matrix[7];
                matrix.Values[10] = staticModel.Matrix[8];
                // Convert to Euler
                var euler = matrix.ToEuler();
                // Add it
                entities.Add(IWMap.Entity.CreateMiscModel(modelName, new Vector3(staticModel.X, staticModel.Y, staticModel.Z), Rotation.ToDegrees(euler), staticModel.ModelScale));
            }
            // Done
            return entities;
        }

        public unsafe static Dictionary<int, IDictionary> CreateXModelDictionary(ProcessReader reader, long address, int count)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            // Read buffer
            var byteBuffer = reader.ReadBytes(address, count * Marshal.SizeOf<GfxStaticModel>());
            // Loop number of models we have
            Dictionary<int, IDictionary> MapModels = new Dictionary<int, IDictionary>(count);
            for (int i = 0; i < count; i++)
            {
                Dictionary<string, string> ModelData = new Dictionary<string, string>();
                List<double> Position = new List<double>();
                List<double> angles = new List<double>();
                // Read Struct
                var staticModel = ByteUtil.BytesToStruct<GfxStaticModel>(byteBuffer, i * Marshal.SizeOf<GfxStaticModel>());
                // Model Name
                var modelName = reader.ReadNullTerminatedString(reader.ReadInt32(staticModel.ModelPointer));
                if (modelName.Contains("."))
                {
                    modelName = modelName.Replace(".", null);
                }

                var matrix = new Rotation.Matrix();
                // Copy X Values
                matrix.Values[0] = staticModel.Matrix[0];
                matrix.Values[1] = staticModel.Matrix[1];
                matrix.Values[2] = staticModel.Matrix[2];
                // Copy Y Values
                matrix.Values[4] = staticModel.Matrix[3];
                matrix.Values[5] = staticModel.Matrix[4];
                matrix.Values[6] = staticModel.Matrix[5];
                // Copy Z Values
                matrix.Values[8] = staticModel.Matrix[6];
                matrix.Values[9] = staticModel.Matrix[7];
                matrix.Values[10] = staticModel.Matrix[8];
                // Convert to Euler
                var euler = matrix.ToEuler();
                // Add it
                if (string.IsNullOrEmpty(modelName) || modelName.Contains("?") == true || modelName.Contains("'") == true || modelName.Contains("\\") == true || modelName.Contains("fx") == true || modelName.Contains("viewmodel") == true || staticModel.ModelScale < 0.001 || staticModel.ModelScale > 10)
                {

                }
                else
                {
                    ModelData.Add("Name", modelName);
                    ModelData.Add("PosX", string.Format("{0:0.0000}", staticModel.X));
                    ModelData.Add("PosY", string.Format("{0:0.0000}", staticModel.Y));
                    ModelData.Add("PosZ", string.Format("{0:0.0000}", staticModel.Z));
                    ModelData.Add("RotX", string.Format("{0:0.0000}", (float)Rotation.ToDegrees(euler).X).ToString(CultureInfo.InvariantCulture));
                    ModelData.Add("RotY", string.Format("{0:0.0000}", (float)Rotation.ToDegrees(euler).Y).ToString(CultureInfo.InvariantCulture));
                    ModelData.Add("RotZ", string.Format("{0:0.0000}", (float)Rotation.ToDegrees(euler).Z).ToString(CultureInfo.InvariantCulture));
                    ModelData.Add("Scale", string.Format("{0:0.0000}", staticModel.ModelScale).ToString(CultureInfo.InvariantCulture));
                    MapModels.Add(i, new Dictionary<string, string>(ModelData));
                }
            }


            // Done
            return MapModels;
        }


        public unsafe static List<string> CreateXModelList(ProcessReader reader, long address, int count)
        {
            // Read buffer
            var byteBuffer = reader.ReadBytes(address, count * Marshal.SizeOf<GfxStaticModel>());
            // Loop number of models we have
            List<string> MapModels = new List<string>();
            for (int i = 0; i < count; i++)
            {
                // Read Struct
                var staticModel = ByteUtil.BytesToStruct<GfxStaticModel>(byteBuffer, i * Marshal.SizeOf<GfxStaticModel>());
                // Model Name
                var modelName = reader.ReadNullTerminatedString(reader.ReadInt32(staticModel.ModelPointer));
                // Add it
                if (!MapModels.Contains(modelName))
                {
                    if (string.IsNullOrEmpty(modelName) || modelName.Contains("?") == true || modelName.Contains("'") == true || modelName.Contains("\\") == true || modelName.Contains("fx") == true || modelName.Contains("viewmodel") == true || staticModel.ModelScale < 0.001 || staticModel.ModelScale > 10)
                    {

                    }
                    else
                    {
                        if (modelName.Contains("/") == true)
                        {
                            modelName = modelName.Split('/')[1];
                            if (MapModels.Contains(modelName) == false)
                            {
                                MapModels.Add(modelName);
                            }
                        }
                        else
                        {
                            MapModels.Add(modelName);
                        }
                    }
                }

            }


            // Done
            return MapModels;
        }
    }
}