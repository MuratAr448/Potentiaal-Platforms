using UnityEngine;

public class Background : MonoBehaviour
{
    private enum BackGroundState
    {
        follow,
        sky,
        ground
    }
    [SerializeField] private BackGroundState backGround;
    private float startPosX,lenght,startPosY,hight;
    public GameObject Cam;
    public float parralaxEffect;
    void Start()
    {
        startPosX = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
        startPosY = transform.position.y;
        hight = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    
    void FixedUpdate()
    {
        float distanceX = Cam.transform.position.x * parralaxEffect;
        float distanceY = Cam.transform.position.y * parralaxEffect;
        float movement = Cam.transform.position.x * (1 - parralaxEffect);
        switch (backGround)
        {
            case BackGroundState.follow:
                transform.position = new Vector3(startPosX + distanceX, transform.position.y, transform.position.z);
                break;
            case BackGroundState.sky:
                if (distanceY>0)
                {
                    transform.position = new Vector3(startPosX + distanceX, startPosY + distanceY, transform.position.z);
                }
                break;
            case BackGroundState.ground:
                if (distanceY < 0)
                {
                    transform.position = new Vector3(startPosX + distanceX, startPosY + distanceY, transform.position.z);
                }
                break;
            default:
                break;
        }
        if (movement> startPosX+ lenght)
        {
            startPosX += lenght;
        }else if (movement< startPosX- lenght)
        {
            startPosX -= lenght;
        }
        
        movement = Cam.transform.position.y * (1 - parralaxEffect);

        if (movement > startPosY + hight)
        {
            startPosY += hight;
        }
        else if (movement < startPosY - hight)
        {
            startPosY -= hight;
        }
    }
}
