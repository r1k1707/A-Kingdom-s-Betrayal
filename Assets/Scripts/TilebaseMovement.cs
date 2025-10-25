using System.Collections;
using UnityEngine;

public class TilebaseMovement : MonoBehaviour
{

    [SerializeField] float speed;
    bool isMoving;

    public enum MoveDirection
    {
        idle,
        up,
        down,
        left,
        right
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            ObstacleCheck(transform.position + Vector3.up);
        }
        if (Input.GetKey(KeyCode.A))
        {
            ObstacleCheck(transform.position + Vector3.left);
        }
        if (Input.GetKey(KeyCode.S))
        {
            ObstacleCheck(transform.position + Vector3.down);
        }
        if (Input.GetKey(KeyCode.D))
        {
            ObstacleCheck(transform.position + Vector3.right);
        }
    }

    void ObstacleCheck(Vector3 moveDirection)
    {
        if (isMoving == true)
        {
            return;
        }

        if (Physics2D.OverlapBox(moveDirection, new Vector2(0.8f, 0.8f), 0))
        {

        }
        else
        {
            isMoving = true;
            StartCoroutine(Move(moveDirection));
        }
    }

    IEnumerator Move(Vector3 endPos)
    {
        float t = 1;
        Vector3 startPos = transform.position;
        while (t < 1)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        isMoving = false;
    }
}
