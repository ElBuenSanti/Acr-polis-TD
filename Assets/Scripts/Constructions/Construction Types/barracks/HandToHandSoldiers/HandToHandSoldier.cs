using JetBrains.Annotations;
using UnityEngine;

public class HandToHandSoldier : MonoBehaviour
{
    public Barracks barrack;

    [SerializeField]
    private float speed = 5f;
    void Start()
    {
        Debug.Log("Soldado creado");
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime; //temporal
    }

    public void SoldierCreated()
    {

    }
}
