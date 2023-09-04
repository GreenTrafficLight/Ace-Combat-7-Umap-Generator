
using System.CommandLine;

using UAssetAPI;


namespace Ace_Combat_7_Umap_Generator
{
    public class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand("Sample app for System.CommandLine");

            var assetInputPath = new Argument<string>
                (name: "input",
                description: "Path to the asset input directory");

            var assetOutputPath = new Argument<string>
                (name: "output",
                description: "Path to the asset output directory");

            var staticMeshActor = new Command("StaticMeshActor", "Generate static mesh actors for a umap")
            {
                assetInputPath,
                assetOutputPath
            };
            staticMeshActor.SetHandler(StaticMeshActor, assetInputPath, assetOutputPath);
            rootCommand.AddCommand(staticMeshActor);

            var unitBP = new Command("UnitBP", "Generate units for a umap")
            {
                assetInputPath,
                assetOutputPath
            };
            unitBP.SetHandler(UnitBP, assetInputPath, assetOutputPath);
            rootCommand.AddCommand(unitBP);

            var aiGameObject = new Command("AIGameObject", "Generate AIGameObjects for a umap")
            {
                assetInputPath,
                assetOutputPath
            };
            aiGameObject.SetHandler(AIGameObject, assetInputPath, assetOutputPath);
            rootCommand.AddCommand(aiGameObject);

            return rootCommand.Invoke(args);
        }

        static void StaticMeshActor(string assetInputPath, string assetOutputPath)
        {
            uint sig = umapGenerator.GetFileSignature(assetInputPath);
            if (sig == UAsset.ACE7_MAGIC)
            {
                AC7Decrypt ac7Decrypt = new AC7Decrypt();
                ac7Decrypt.Decrypt(assetInputPath, assetInputPath);
            }
            UAsset uasset = new UAsset(assetInputPath, UAssetAPI.UnrealTypes.EngineVersion.VER_UE4_18, null);
            umapGenerator.GenerateStaticMeshActorsCSV(uasset, assetOutputPath);
        }

        static void UnitBP(string assetInputPath, string assetOutputPath)
        {
            uint sig = umapGenerator.GetFileSignature(assetInputPath);
            if (sig == UAsset.ACE7_MAGIC)
            {
                AC7Decrypt ac7Decrypt = new AC7Decrypt();
                ac7Decrypt.Decrypt(assetInputPath, assetInputPath);
            }
            UAsset uasset = new UAsset(assetInputPath, UAssetAPI.UnrealTypes.EngineVersion.VER_UE4_18, null);
            umapGenerator.GenerateUnitsCSV(uasset, assetOutputPath);
        }

        static void AIGameObject(string assetInputPath, string assetOutputPath)
        {
            uint sig = umapGenerator.GetFileSignature(assetInputPath);
            if (sig == UAsset.ACE7_MAGIC)
            {
                AC7Decrypt ac7Decrypt = new AC7Decrypt();
                ac7Decrypt.Decrypt(assetInputPath, assetInputPath);
            }
            UAsset uasset = new UAsset(assetInputPath, UAssetAPI.UnrealTypes.EngineVersion.VER_UE4_18, null);
            umapGenerator.GenerateAIGameObjectsCSV(uasset, assetOutputPath);
        }
    }
}