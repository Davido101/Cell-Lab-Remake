using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;

public struct LegacyProgrammableValue
{
    public short inputCellState;
    public short equationType;
    public float a;
    public float b;
    public float c;
}

public struct LegacyMode
{
    public int version;
    public float red;
    public float green;
    public float blue;
    public float splitMass;
    public float splitRatio;
    public float splitAngle;
    public float child1Angle;
    public float child2Angle;
    public float nutrientPriority;
    public int child1Mode;
    public int child2Mode;
    public bool makeAdhesin;
    public bool child1KeepAdhesin;
    public bool child2KeepAdhesin;
    public int cellType;
    public int empty;
    public bool prioritize;
    public bool initial;
    public bool child1Mirror;
    public bool child2Mirror;
    public float adhesinStiffness;
    public LegacyProgrammableValue flagellocyteSwimForce;
    public LegacyProgrammableValue buoyocyteDensity;
    public LegacyProgrammableValue myocyteMuscleBending;
    public LegacyProgrammableValue myocyteMuscleContraction;
    public LegacyProgrammableValue myocyteMuscleLift;
    public LegacyProgrammableValue stemocytePath1Signal;
    public LegacyProgrammableValue stemocytePath2Signal;
    public LegacyProgrammableValue neurocyteChannel1OutputValue;
    public LegacyProgrammableValue neurocyteChannel2OutputValue;
    public LegacyProgrammableValue neurocyteChannel3OutputValue;
    public LegacyProgrammableValue neurocyteChannel4OutputValue;
    public float senseocyteOutputValue;
    public float redSmell;
    public float greenSmell;
    public float blueSmell;
    public float smellColorThreshold;
    public float adhesinLength;
    public float cytoskeleton;
    public int virusCopyFrom;
    public int gameteCompatibleMode;
    public int senseocyteSmellOutputSignalType;
    public int senseocyteSmellType;
    public int secrocyteSecretionType;
    public int neurocyteChannel1SignalType;
    public int neurocyteChannel2SignalType;
    public int neurocyteChannel3SignalType;
    public int neurocyteChannel4SignalType;
    public int maxConnections;
    public int telomeres;
}

public struct LegacySubstrate
{
    public int version;
    public double age;
    public int cellCount;
    public int envVersion;
    public double nutrientRate;
    public double nutrientChunkRate;
    public double radiation;
    public double lightAmount;
    public double lightDirectionChange;
    public double lightRange;
    public double viscosity;
    public int cellTypeCount;
    public bool phagocytes;
    public bool flagellocytes;
    public bool photocytes;
    public bool devorocytes;
    public bool lipocytes;
    public bool keratinocytes;
    public bool buoyocytes;
    public bool glueocytes;
    public bool virocytes;
    public bool nitrocytes;
    public bool stereocytes;
    public bool senseocytes;
    public bool myocytes;
    public bool neurocytes;
    public bool secrocytes;
    public bool stemocytes;
    public bool gametes;
    public bool ciliocytes;
    public bool contaminate;
    public double gravity;
    public double density;
    public double densityGradient;
    public bool killCellsAtEdge;
    public double nitrates;
    public int maxCells;
    public int maxFood;
    public double radius;
    public double dynamicFriction;
    public double staticFriction;
    public bool pointMutations;
    public float salinity;
    public bool cellAging;
    public double nutrientLumpiness;
    public double nutrientLumpSize;
    public bool mobileFood;
    public float nutrientCoating;
}

public class FileManager : MonoBehaviour
{
    // New Format
    public static void LoadSubstrate(string path, ref Substrate substrate)
    {
        throw new NotImplementedException();
    }

    public static void SaveSubstrate(string path, ref Substrate substrate)
    {
        throw new NotImplementedException();
    }

    public static void LoadGenome(string path)
    {
        throw new NotImplementedException();
    }

    public static void SaveGenome(string path)
    {
        throw new NotImplementedException();
    }

    // Old Format
    public static bool LoadLegacySubstrate(string path, ref Substrate substrate)
    {
        BinaryHandler substrateFile = new BinaryHandler(path, true);
        return ParseLegacySubstrate(substrateFile, ref substrate);
    }

    public static void SaveLegacySubstrate(string path, ref Substrate substrate)
    {
        BinaryHandler substrateFile = new BinaryHandler(path, false);
        ConvertLegacySubstrate(substrateFile, ref substrate);
    }

    public static void LoadLegacyGenome(string path)
    {
        throw new NotImplementedException();
    }

    public static void SaveLegacyGenome(string path)
    {
        throw new NotImplementedException();
    }

    private static bool ParseLegacySubstrate(BinaryHandler substrateFile, ref Substrate substrate)
    {
        byte[] header = new byte[] { 0xac, 0xed, 0x00, 0x05 };
        if (!substrateFile.Verify(header) && !substrateFile.Verify(0x77))
        {
            return false;
        }

        // Discard Block and Block Length
        substrateFile.Discard(2);

        object legacySubstrateObj = (object)new LegacySubstrate();
        FillStruct(substrateFile, ref legacySubstrateObj);
        LegacySubstrate legacySubstrate = (LegacySubstrate)legacySubstrateObj;
        substrate.age = (float)legacySubstrate.age;
        substrate.lightAmount = (float)legacySubstrate.lightAmount;
        substrate.lightRange = (float)legacySubstrate.lightRange;
        substrate.radius = (float)legacySubstrate.radius * 500;

        byte[] cell_data = substrateFile.ReadAll();
        substrateFile.Overwrite(GZip.Decompress(cell_data));
        File.WriteAllBytes("C:\\Users\\lazar\\PycharmProjects\\genomeEditor\\export_cell", GZip.Decompress(cell_data));

        // Check for cells header
        if (!substrateFile.Verify(header))
        {
            return false;
        }

        List<byte> data = new List<byte>();
        while (true)
        {
            if (substrateFile.ReadByte() == null)
            {
                break;
            }
            int block_length = substrateFile.ReadInt();
            byte[] block = substrateFile.Read(block_length);
            block.ToList().ForEach(b => data.Add(b));
        }

        substrateFile.Overwrite(data.ToArray());
        double lightAngle = substrateFile.ReadDouble();
        substrate.lightAngle = (float)lightAngle;
        for (int i = 0; i < legacySubstrate.cellCount; i++)
        {
            if (!ParseLegacyCell(substrateFile, ref substrate))
                return false;
        }
        int foodCount = substrateFile.ReadInt();
        for (int i = 0; i < foodCount; i++)
        {
            float x = substrateFile.ReadFloat() * 500;
            float y = substrateFile.ReadFloat() * 500;
            float size = substrateFile.ReadFloat();
            float xVel = substrateFile.ReadFloat(); // we should add support for mobile food (velocity for food)
            float yVel = substrateFile.ReadFloat();
            float coating = substrateFile.ReadFloat();
            Food food = substrate.SpawnFood(x, y, size, coating);
        }

        return true;
    }

    private static void ConvertLegacySubstrate(BinaryHandler substrateFile, ref Substrate substrate)
    {
        byte[] header = new byte[] { 0xac, 0xed, 0x00, 0x05 };
        substrateFile.WriteBytes(header);
        substrateFile.WriteByte(0x77);
        substrateFile.WriteByte(0xbf);

        substrateFile.WriteInt(95);
        substrateFile.WriteDouble(substrate.age);
        substrateFile.WriteInt(substrate.cells.Count);
        substrateFile.WriteInt(95);
        substrateFile.WriteDouble(0); // nutrient rate
        substrateFile.WriteDouble(0); // nutrient chunk rate
        substrateFile.WriteDouble(0); // radiation
        substrateFile.WriteDouble(substrate.lightAmount);
        substrateFile.WriteDouble(0); // light direction change
        substrateFile.WriteDouble(substrate.lightRange);
        substrateFile.WriteDouble(0); // viscosity
        substrateFile.WriteInt(18);
        substrateFile.WriteBool(true);
        substrateFile.WriteBool(true);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteDouble(0); // gravity
        substrateFile.WriteDouble(0); // density
        substrateFile.WriteDouble(0); // density gradient
        substrateFile.WriteBool(false); // kill cells at edge
        substrateFile.WriteDouble(1); // nitrates
        substrateFile.WriteInt(Math.Max(substrate.cells.Count + 500, 1000)); // max cells
        substrateFile.WriteInt(Math.Max(substrate.foods.Count + 2000, 3500)); // max food
        substrateFile.WriteDouble(substrate.radius / 500);
        substrateFile.WriteDouble(0); // dynamic friction
        substrateFile.WriteDouble(0); // static friction
        substrateFile.WriteBool(false); // only point mutations
        substrateFile.WriteFloat(0.33f); // salinity
        substrateFile.WriteBool(true); // cell aging
        substrateFile.WriteDouble(0); // nutrient lumpiness
        substrateFile.WriteDouble(0); // nutrient lump size
        substrateFile.WriteBool(false); // mobile food
        substrateFile.WriteFloat(0); // nutrient coating

        BinaryHandler cellFile = new BinaryHandler();
        cellFile.WriteDouble(substrate.lightAngle);

        for (int i = 0; i < substrate.cells.Count; i++)
        {
            Cell currentCell = substrate.cells[i];
            ConvertLegacyCell(substrateFile, ref currentCell);
        }

        cellFile.WriteInt(substrate.foods.Count);
        for (int i = 0; i < substrate.foods.Count; i++)
        {
            Food food = substrate.foods[i];
            substrateFile.WriteFloat(food.position.x * 500);
            substrateFile.WriteFloat(food.position.y * 500);
            substrateFile.WriteFloat(food.size);
            substrateFile.WriteFloat(0); // x velocity
            substrateFile.WriteFloat(0); // y velocity
            substrateFile.WriteFloat(0); // coating
        }

        cellFile.Flush();
        substrateFile.WriteBytes(GZip.Compress(Binary.ConvertToJavaStream(cellFile.GetData())));
        cellFile.Close();

        substrateFile.Flush();
        substrateFile.Close();
    }

    private static bool ParseLegacyCell(BinaryHandler substrateFile, ref Substrate substrate)
    {
        int version = substrateFile.ReadInt();
        double x = substrateFile.ReadDouble() * 500;
        double y = substrateFile.ReadDouble() * 500;
        double angle = substrateFile.ReadDouble();
        double m = substrateFile.ReadDouble();
        double xVelocity = substrateFile.ReadDouble();
        double yVelocity = substrateFile.ReadDouble();
        double angularVelocity = substrateFile.ReadDouble();
        double n = substrateFile.ReadDouble();
        double radius = substrateFile.ReadDouble();
        double mass = substrateFile.ReadDouble();
        double age = substrateFile.ReadDouble();
        int linkCount = substrateFile.ReadInt();
        for (int i = 0; i < MathF.Min(linkCount, 20); i++)
        {
            ParseLegacyLink(substrateFile); // create link when we support it
        }
        for (int i = 0; i < linkCount - MathF.Min(linkCount, 20); i++)
        {
            ParseLegacyLink(substrateFile); // don't create link
        }
        int adhesinConnectionCount = substrateFile.ReadInt();
        bool dead = substrateFile.ReadBool();
        float red = substrateFile.ReadFloat();
        float green = substrateFile.ReadFloat();
        float blue = substrateFile.ReadFloat();
        int modeCount = substrateFile.ReadInt();
        LegacyMode[] modes = ParseLegacyGenome(substrateFile, modeCount, true);
        int activeMode = substrateFile.ReadInt();

        Type cellType = GetCellType(modes[activeMode].cellType);
        if (cellType == null)
        {
            Debug.LogError("Cell Type '" + activeMode.ToString() + "' not yet added to the game.");
        }
        else
        {
            Cell cell = substrate.SpawnCell(cellType, (float)x, (float)y, new Color(red, green, blue));
            cell.mass = (float)mass * 10;
            cell.angle = (float)angle;
            cell.radius = (float)radius * 500;
            cell.velocity = new Vector2((float)xVelocity * 500, (float)yVelocity * 500);
            cell.age = (float)age;
        }

        int tag = substrateFile.ReadInt();
        int F = substrateFile.ReadInt();
        double t = substrateFile.ReadDouble();
        double u = substrateFile.ReadDouble();
        double v = substrateFile.ReadDouble();
        double nitrogen = substrateFile.ReadDouble();
        bool mirrored = substrateFile.ReadBool();
        int O = 0; // this is random in the real game
        float[] signals = new float[4];
        float[] W = new float[4];
        for (int i = 0; i < 4; i++)
        {
            signals[i] = substrateFile.ReadFloat();
            W[i] = substrateFile.ReadFloat();
        }
        float toxins = substrateFile.ReadFloat();
        float injury = substrateFile.ReadFloat();
        float aa = substrateFile.ReadFloat();
        float ab = substrateFile.ReadFloat();
        float ac = substrateFile.ReadFloat();
        float lipids = substrateFile.ReadFloat();
        float mutations = substrateFile.ReadFloat();
        float telomeres = substrateFile.ReadFloat();
        double lift = substrateFile.ReadDouble();

        return true;
    }

    private static void ConvertLegacyCell(BinaryHandler substrateFile, ref Cell cell)
    {
        substrateFile.WriteInt(95);
        substrateFile.WriteDouble(cell.position.x / 500);
        substrateFile.WriteDouble(cell.position.y / 500);
        substrateFile.WriteDouble(cell.angle);
        substrateFile.WriteDouble(0); // m
        substrateFile.WriteDouble(cell.velocity.x / 500);
        substrateFile.WriteDouble(cell.velocity.y / 500);
        substrateFile.WriteDouble(0); // angular velocity
        substrateFile.WriteDouble(0); // n
        substrateFile.WriteDouble(cell.radius / 500);
        substrateFile.WriteDouble(cell.mass / 10);
        substrateFile.WriteDouble(cell.age);
        substrateFile.WriteInt(0); // link count
        // link converter would go here
        substrateFile.WriteInt(0); // adhesin connection count
        substrateFile.WriteBool(cell.dead);
        substrateFile.WriteFloat(cell.color.r);
        substrateFile.WriteFloat(cell.color.g);
        substrateFile.WriteFloat(cell.color.b);
        substrateFile.WriteInt(1); // mode count
        // mode converter would go here but for now just generate a test current mode
        ConvertLegacyMode(substrateFile, ref cell);
        substrateFile.WriteInt(0); // active mode
        substrateFile.WriteInt(0); // tag
        substrateFile.WriteInt(0); // F
        substrateFile.WriteDouble(0); // t
        substrateFile.WriteDouble(0); // u
        substrateFile.WriteDouble(0); // v
        substrateFile.WriteDouble(1); // nitrogen
        substrateFile.WriteBool(false); // mirrored
        // signals
        for (int i = 0; i < 4; i++)
        {
            substrateFile.WriteFloat(0);
            substrateFile.WriteFloat(0);
        }
        substrateFile.WriteFloat(0); // toxins
        substrateFile.WriteFloat(0); // injury
        substrateFile.WriteFloat(0); // aa
        substrateFile.WriteFloat(0); // ab
        substrateFile.WriteFloat(0); // ac
        substrateFile.WriteFloat(0); // lipids
        substrateFile.WriteFloat(0); // mutations
        substrateFile.WriteFloat(-1); // telomeres
        substrateFile.WriteDouble(0); // lift
    }

    private static bool ParseLegacyLink(BinaryHandler substrateFile)
    {
        int connectedCell = substrateFile.ReadInt();
        double connectedCellAngle = substrateFile.ReadDouble();
        double rootCellAngle = substrateFile.ReadDouble();
        bool fromCellDivision = substrateFile.ReadBool();
        double e = substrateFile.ReadDouble();
        double f = substrateFile.ReadDouble();
        float k = substrateFile.ReadFloat();
        float l = substrateFile.ReadFloat();

        return true;
    }

    private static LegacyMode[] ParseLegacyGenome(BinaryHandler substrateFile, int modeCount, bool fromSubstrate)
    {
        LegacyMode[] modes = new LegacyMode[modeCount];
        if (!fromSubstrate)
        {
            int version = substrateFile.ReadInt();
        }
        for (int i = 0; i < modeCount; i++)
        {
            modes[i] = ParseLegacyMode(substrateFile);
        }

        return modes;
    }

    private static LegacyMode ParseLegacyMode(BinaryHandler substrateFile)
    {
        object mode = (object)new LegacyMode();
        FillStruct(substrateFile, ref mode);

        return (LegacyMode)mode;
    }

    private static void ConvertLegacyMode(BinaryHandler substrateFile, ref Cell cell)
    {
        substrateFile.WriteInt(95);
        substrateFile.WriteFloat(cell.color.r);
        substrateFile.WriteFloat(cell.color.g);
        substrateFile.WriteFloat(cell.color.b);
        substrateFile.WriteFloat(0);
        substrateFile.WriteFloat(0);
        substrateFile.WriteFloat(0);
        substrateFile.WriteFloat(0);
        substrateFile.WriteFloat(0);
        substrateFile.WriteFloat(0);
        substrateFile.WriteInt(0);
        substrateFile.WriteInt(0);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(true);
        substrateFile.WriteBool(true);
        substrateFile.WriteInt(GetCellType(cell.type));
        substrateFile.WriteInt(0);
        substrateFile.WriteBool(true);
        substrateFile.WriteBool(true);
        substrateFile.WriteBool(false);
        substrateFile.WriteBool(false);
        substrateFile.WriteFloat(0);
        for (int i = 0; i < 11; i++)
        {
            ConvertLegacyProgrammableValue(substrateFile);
        }
        substrateFile.WriteFloat(0);
        substrateFile.WriteFloat(0);
        substrateFile.WriteFloat(0);
        substrateFile.WriteFloat(0);
        substrateFile.WriteFloat(0);
        substrateFile.WriteFloat(0);
        substrateFile.WriteInt(0);
        substrateFile.WriteInt(0);
        substrateFile.WriteInt(0);
        substrateFile.WriteInt(0);
        substrateFile.WriteInt(0);
        substrateFile.WriteInt(0);
        substrateFile.WriteInt(0);
        substrateFile.WriteInt(0);
        substrateFile.WriteInt(0);
        substrateFile.WriteInt(0);
        substrateFile.WriteInt(-1);

    }

    private static LegacyProgrammableValue ParseLegacyProgrammableValue(BinaryHandler substrateFile)
    {
        object programmableValue = (object)new LegacyProgrammableValue();
        FillStruct(substrateFile, ref programmableValue);

        return (LegacyProgrammableValue)programmableValue;
    }

    private static void ConvertLegacyProgrammableValue(BinaryHandler substrateFile)
    {
        substrateFile.WriteShort(0);
        substrateFile.WriteShort(0);
        substrateFile.WriteFloat(0);
        substrateFile.WriteFloat(0);
        substrateFile.WriteFloat(0);
    }

    private static Type? GetCellType(int mode)
    {
        Type[] cell_types = new Type[]
        {
            typeof(Phagocyte),
            typeof(Flagellocyte),
            typeof(Photocyte),
            typeof(Devorocyte),
            typeof(Lipocyte),
            typeof(Keratinocyte),
        };

        if (mode > cell_types.Length - 1)
        {
            return null;
        }
        else
        {
            return cell_types[mode];
        }
    }

    private static int GetCellType(Type cellType)
    {
        if (cellType == typeof(Phagocyte))
        {
            return 0;
        }
        else if (cellType == typeof(Flagellocyte))
        {
            return 1;
        }
        else if (cellType == typeof(Photocyte))
        {
            return 2;
        }
        else if (cellType == typeof(Devorocyte))
        {
            return 3;
        }
        else if (cellType == typeof(Lipocyte))
        {
            return 4;
        }
        else if (cellType == typeof(Keratinocyte))
        {
            return 5;
        }
        else
        {
            return 0;
        }
    }

    private static void FillStruct(BinaryHandler substrateFile, ref object obj)
    {
        System.Reflection.FieldInfo[] fields = obj.GetType().GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            if (field.FieldType == typeof(bool))
            {
                field.SetValue(obj, substrateFile.ReadBool());
            }
            else if (field.FieldType == typeof(Int16))
            {
                field.SetValue(obj, substrateFile.ReadShort());
            }
            else if (field.FieldType == typeof(Int32))
            {
                field.SetValue(obj, substrateFile.ReadInt());
            }
            else if (field.FieldType == typeof(float))
            {
                field.SetValue(obj, substrateFile.ReadFloat());
            }
            else if (field.FieldType == typeof(double))
            {
                field.SetValue(obj, substrateFile.ReadDouble());
            }
            else if (field.FieldType == typeof(LegacyProgrammableValue))
            {
                object oldProgrammableValue = (object)new LegacyProgrammableValue();
                FillStruct(substrateFile, ref oldProgrammableValue);
                field.SetValue(obj, oldProgrammableValue);
            }
            else if (field.FieldType == typeof(LegacyMode))
            {
                object oldMode = (object)new LegacyMode();
                FillStruct(substrateFile, ref oldMode);
                field.SetValue(obj, oldMode);
            }
        }
    }
}