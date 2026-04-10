using Mono.Cecil.Cil;
using UnityEngine;

public class ConstructionController : MonoBehaviour
{
    public ConstructionDatabase database;
    public BaseConstruction construction;

    private ConstructionType type; //condiciones iniciales 
    private GodType god;
    private int level = 1;

    public void Initialize(ConstructionType t, GodType g) //como comenzará el edificio
    {
        type = t;
        god = g;
        level = 1;

        UpdateConstruction();
    }

    public void Upgrade()
    {
        level++;
        UpdateConstruction();
    }

    void UpdateConstruction()
    {
        var data = database.GetData(type, god, level); //busca la construcción solicitada
        construction.Initialize(data); //actualiza
    }
}