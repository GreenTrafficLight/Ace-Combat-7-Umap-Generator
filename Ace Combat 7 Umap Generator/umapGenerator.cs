using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace Ace_Combat_7_Umap_Generator
{
    public class umapGenerator
    {
        public static void GenerateStaticMeshActorsCSV(UAsset uasset, string outputPath)
        {
            FileStream fs = File.Create(outputPath + "\\" + Path.GetFileNameWithoutExtension(uasset.FilePath) + "_StaticMeshActors.csv");
            StreamWriter streamWriter = new StreamWriter(fs);
            streamWriter.WriteLine("---,Actor,Location,Rotation,Scale,Mesh");

            for (int i = 0; i < uasset.Exports.Count; i++)
            {
                var assetExport = uasset.Exports[i];
                FName assetExportClassTypeName = assetExport.GetExportClassType();
                string assetExportClassType = assetExportClassTypeName.Value.Value;

                if (assetExportClassType == "StaticMeshActor")
                {
                    NormalExport assetNormalExport = (NormalExport)assetExport;
                    NormalExport staticMeshComponentNormalExport = (NormalExport)uasset.Exports[((FPackageIndex)assetNormalExport["StaticMeshComponent"].RawValue).Index - 1];

                    string actorPath = "Class'/Script/Engine.StaticMeshActor'";
                    string meshPath = "";

                    // Mesh
                    var meshPackageIndex = ((FPackageIndex)staticMeshComponentNormalExport["StaticMesh"].RawValue).Index + 1;
                    var meshImport = uasset.Imports[-meshPackageIndex];
                    while (meshImport.OuterIndex.Index != 0)
                        meshImport = uasset.Imports[-meshImport.OuterIndex.Index - 1];
                    meshPath = $"\"StaticMesh'{meshImport.ObjectName}.{meshImport.ObjectName.ToString().Substring(meshImport.ObjectName.ToString().LastIndexOf("/") + 1)}'\"";

                    string locationValue = GetVectorValue(staticMeshComponentNormalExport, "RelativeLocation", new FVector(1, 1, 1));
                    string rotationValue = GetRotatorValue(staticMeshComponentNormalExport, "RelativeRotation", new FRotator(0, 0, 0));
                    string scaleValue = GetVectorValue(staticMeshComponentNormalExport, "RelativeScale3D", new FVector(1, 1, 1));

                    streamWriter.WriteLine($"{assetExport.ObjectName},{actorPath},{locationValue},{rotationValue},{scaleValue},{meshPath}");
                }
            }
            streamWriter.Close();
        }
    
        public static void GenerateUnitsCSV(UAsset uasset, string outputPath)
        {
            FileStream fs = File.Create(outputPath + "\\" + Path.GetFileNameWithoutExtension(uasset.FilePath) + "_Units.csv");
            StreamWriter streamWriter = new StreamWriter(fs);
            streamWriter.WriteLine("---,Actor,Location,Rotation,Scale,bIsMoving,bUsingSpecifiedFormation,bActivateFromStart,Faction,SubFaction,MinAltitude,MaxAltitude,Radius,HostileSearchDuration,bIsNotActivateDeadObjectOnRestart,FormationOffset,bNotUnitIconCandidate");

            for (int i = 0; i < uasset.Exports.Count; i++)
            {
                var assetExport = uasset.Exports[i];
                FName assetExportClassTypeName = assetExport.GetExportClassType();
                string assetExportClassType = assetExportClassTypeName.Value.Value;

                if (assetExportClassType == "UnitBP_C")
                {
                    NormalExport assetNormalExport = (NormalExport)assetExport;
                    NormalExport rootComponentNormalExport = (NormalExport)uasset.Exports[((FPackageIndex)assetNormalExport["RootComponent"].RawValue).Index - 1];

                    string actorPath = "BlueprintGeneratedClass'/Game/Editor/UnitBP.UnitBP_C'";

                    string locationValue = GetVectorValue(rootComponentNormalExport, "RelativeLocation", new FVector(1, 1, 1));
                    string rotationValue = GetRotatorValue(rootComponentNormalExport, "RelativeRotation", new FRotator(0, 0, 0));
                    string scaleValue = GetVectorValue(rootComponentNormalExport, "RelativeScale3D", new FVector(1, 1, 1));
                    string bIsMovingValue = GetBoolValue(assetNormalExport, "bIsMoving", "False");
                    string bUsingSpecifiedFormationValue = GetBoolValue(assetNormalExport, "bUsingSpecifiedFormation", "False");
                    string bActivateFromStartValue = GetBoolValue(assetNormalExport, "bActivateFromStart", "True");
                    string factionValue = GetEnumValue(assetNormalExport, "Faction", "Enemy");
                    string subFactionValue = GetEnumValue(assetNormalExport, "SubFaction", "None");
                    float minAltitudeValue = GetFloatValue(assetNormalExport, "MinAltitude", 8000.0f);
                    float maxAltitudeValue = GetFloatValue(assetNormalExport, "MaxAltitude", 120000.0f);
                    float radiusValue = GetFloatValue(assetNormalExport, "Radius", 80000.0f);
                    float hostileSearchDurationValue = GetFloatValue(assetNormalExport, "HostileSearchDuration", 0.5f);
                    string bIsNotActivateDeadObjectOnRestartValue = GetBoolValue(assetNormalExport, "bIsNotActivateDeadObjectOnRestart", "False");
                    string formationOffsetValue = GetVectorValue(assetNormalExport, "FormationOffset", new FVector(0, 0, 0));
                    string bNotUnitIconCandidateValue = GetBoolValue(assetNormalExport, "bNotUnitIconCandidate", "False");

                    streamWriter.WriteLine($"{assetExport.ObjectName},{actorPath},{locationValue},{rotationValue},{scaleValue},{bIsMovingValue},{bUsingSpecifiedFormationValue},{bActivateFromStartValue},{factionValue},{subFactionValue},{minAltitudeValue},{maxAltitudeValue},{radiusValue},{hostileSearchDurationValue},{bIsNotActivateDeadObjectOnRestartValue},{formationOffsetValue},{bNotUnitIconCandidateValue}");
                }
            }
            streamWriter.Close();
        }

        public static void GenerateAIGameObjectsCSV(UAsset uasset, string outputPath)
        {
            FileStream fs = File.Create(outputPath + "\\" + Path.GetFileNameWithoutExtension(uasset.FilePath) + "_AIGameObjects.csv");
            StreamWriter streamWriter = new StreamWriter(fs);
            streamWriter.WriteLine("---,Actor,Location,Rotation,Scale,AILevel,SpeakerID,SpawnCollisionHandlingMethod");
            
            for (int i = 0; i < uasset.Exports.Count; i++)
            {
                var assetExport = uasset.Exports[i];
                FName assetExportClassTypeName = assetExport.GetExportClassType();
                string assetExportClassType = assetExportClassTypeName.Value.Value;

                Import aiGameObjectImport = null;
                if (assetExport.TemplateIndex.IsImport())
                {
                    aiGameObjectImport = assetExport.TemplateIndex.ToImport(uasset);
                }
                    
                if (aiGameObjectImport != null && aiGameObjectImport.ClassPackage.ToString().Contains("/Game/Blueprint/AI/Actors/"))
                {
                    NormalExport assetNormalExport = (NormalExport)assetExport;
                    NormalExport rootComponentNormalExport = (NormalExport)uasset.Exports[((FPackageIndex)assetNormalExport["RootComponent"].RawValue).Index - 1];

                    //string SpawnCollisionHandlingMethod = GetEnumValue()
                    var actorPath = $"BlueprintGeneratedClass'{aiGameObjectImport.ClassPackage}.{aiGameObjectImport.ClassPackage.ToString().Substring(aiGameObjectImport.ClassPackage.ToString().LastIndexOf("/") + 1)}_C'";
                    string locationValue = GetVectorValue(rootComponentNormalExport, "RelativeLocation", new FVector(1, 1, 1));
                    string rotationValue = GetRotatorValue(rootComponentNormalExport, "RelativeRotation", new FRotator(0, 0, 0));
                    string scaleValue = GetVectorValue(rootComponentNormalExport, "RelativeScale3D", new FVector(1, 1, 1));
                    byte aiLevelValue = GetByteValue(assetNormalExport, "AILevel", 2);
                    string speakerIDValue = GetStringValue(assetNormalExport, "SpeakerID", "");
                    string spawnCollisionHandlingMethodValue = GetEnumValue(assetNormalExport, "SpawnCollisionHandlingMethod", "AdjustIfPossibleButAlwaysSpawn");

                    streamWriter.WriteLine($"{assetExport.ObjectName},{actorPath},{locationValue},{rotationValue},{scaleValue},{aiLevelValue},{speakerIDValue},{spawnCollisionHandlingMethodValue}");
                }
            }
            streamWriter.Close();
        }

        public static void GenerateAIPathsCSV(UAsset uasset, string outputPath)
        {
            FileStream fs = File.Create(outputPath + "\\" + Path.GetFileNameWithoutExtension(uasset.FilePath) + "_AIPaths.csv");
            StreamWriter streamWriter = new StreamWriter(fs);

            for (int i = 0; i < uasset.Exports.Count; i++)
            {
                var assetExport = uasset.Exports[i];
                FName assetExportClassTypeName = assetExport.GetExportClassType();
                string assetExportClassType = assetExportClassTypeName.Value.Value;

                if (assetExportClassType == "AIPathBP_C")
                {
                }
            }
            streamWriter.Close();
        }
        
        public static uint GetFileSignature(string path)
        {
            byte[] buffer = new byte[4];
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var bytes_read = fs.Read(buffer, 0, buffer.Length);
                fs.Close();
            }
            return BitConverter.ToUInt32(buffer, 0);
        }

        private static string GetVectorValue(NormalExport normalExport, string key, FVector fVector)
        {
            var propertyData = normalExport[key];
            if (propertyData != null)
            {
                VectorPropertyData vector = (VectorPropertyData)((List<PropertyData>)propertyData.RawValue)[0];
                return $"\"(X={vector.Value.XFloat},Y={vector.Value.YFloat},Z={vector.Value.ZFloat})\"";
            }
            return $"\"(X={fVector.XFloat},Y={fVector.YFloat},Z={fVector.ZFloat})\"";
        }

        private static string GetRotatorValue(NormalExport normalExport, string key, FRotator fRotator)
        {
            var propertyData = normalExport[key];
            if (propertyData != null)
            {
                RotatorPropertyData rotator = (RotatorPropertyData)((List<PropertyData>)propertyData.RawValue)[0];
                return $"\"(Pitch={rotator.Value.PitchFloat},Yaw={rotator.Value.YawFloat},Roll={rotator.Value.RollFloat})\"";
            }
            return $"\"(Pitch={fRotator.PitchFloat},Yaw={fRotator.YawFloat},Roll={fRotator.RollFloat})\"";
        }

        private static string GetBoolValue(NormalExport normalExport, string key, string defaultValue)
        {
            BoolPropertyData propertyData = (BoolPropertyData)normalExport[key];
            if (propertyData != null)
                return propertyData.RawValue.ToString();
            return defaultValue;
        }

        private static float GetFloatValue(NormalExport normalExport, string key, float defaultValue)
        {
            FloatPropertyData propertyData = (FloatPropertyData)normalExport[key];
            if (propertyData != null)
                return propertyData.Value;
            return defaultValue;
        }

        private static string GetEnumValue(NormalExport normalExport, string key, string defaultValue)
        {
            EnumPropertyData propertyData = (EnumPropertyData)normalExport[key];
            if (propertyData != null)
                return propertyData.RawValue.ToString().Substring(propertyData.RawValue.ToString().IndexOf("::") + "::".Length);
            return defaultValue;
        }
    
        private static string GetStringValue(NormalExport normalExport, string key, string defaultValue)
        {
            StrPropertyData propertyData = (StrPropertyData)normalExport[key];
            if (propertyData != null)
                return propertyData.Value.Value;
            return defaultValue;
        }

        private static byte GetByteValue(NormalExport normalExport, string key, byte defaultValue)
        {
            BytePropertyData propertyData = (BytePropertyData)normalExport[key];
            if (propertyData != null)
                return propertyData.Value;
            return defaultValue;
        }
    }
}
