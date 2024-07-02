using UnityEngine; 

public class SlimeBallsController : MonoBehaviour 
{ 
    [SerializeField] private GameObject slimePoison; 

    private void OnDestroy() 
    { 
        GameObject poison = Instantiate(
            slimePoison,
            transform.position, 
            Quaternion.identity,GameObject.Find("Object_Grid").transform
            ); 
        poison.GetComponent<PoisonController>().PoisonSetup(5f); 
    } 
}