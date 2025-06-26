using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    public List<GameObject> allConveyor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SearchAndFlip(Vector2 point, Vector2 size)
    {
        int pointcntX = Mathf.RoundToInt(size.x) / 5;
        int pointcntY = Mathf.RoundToInt(size.y) / 5;

        float isOddX = 2.5f;
        float isOddY = 2.5f;

        List<Vector2> list = new List<Vector2>();

        if (pointcntX % 2 == 1)
        {
            isOddX = 0f;
            list.Add(point);
        }
        if (pointcntY % 2 == 1)
        {
            isOddY = 0f;
            if (list.Count == 0)
            {
                list.Add(point);
            }
        }


        Debug.Log(pointcntX / 2);

        if (pointcntY != 1)
        {
            if (pointcntX != 1)
            {
                for (int y = 1; y <= pointcntY / 2; y++)
                {
                    for (int x = 1; x <= pointcntX / 2; x++)
                    {
                        list.Add(new Vector2(point.x + (5.0f * x) - isOddX, point.y + (5.0f * y) - isOddY));
                    }
                    for (int x = 1; x <= pointcntX / 2; x++)
                    {
                        list.Add(new Vector2(point.x - (5.0f * x) + isOddX, point.y + (5.0f * y) - isOddY));
                    }
                }
                for (int y = 1; y <= pointcntY / 2; y++)
                {
                    for (int x = 1; x <= pointcntX / 2; x++)
                    {
                        list.Add(new Vector2(point.x + (5.0f * x) - isOddX, point.y - (5.0f * y) + isOddY));
                    }
                    for (int x = 1; x <= pointcntX / 2; x++)
                    {
                        list.Add(new Vector2(point.x - (5.0f * x) + isOddX, point.y - (5.0f * y) + isOddY));
                    }
                }
            }
            else
            {
                for (int y = 1; y <= pointcntY / 2; y++)
                {
                    list.Add(new Vector2(point.x, point.y + (5.0f * y) - isOddY));
                }
                for (int y = 1; y <= pointcntY / 2; y++)
                {
                    list.Add(new Vector2(point.x, point.y - (5.0f * y) + isOddY));
                }
            }
        }
        else
        {
            if (pointcntX != 1)
            {
                for (int x = 1; x <= pointcntX / 2; x++)
                {
                    list.Add(new Vector2(point.x + (5.0f * x) - isOddX, point.y));
                }
                for (int x = 1; x <= pointcntX / 2; x++)
                {
                    list.Add(new Vector2(point.x - (5.0f * x) + isOddX, point.y));
                }
            }
            else
            {
                //‰½‚à‚µ‚È‚¢
            }
        }

        for (int i = 0; i < allConveyor.Count; i++)
        {
            for (int j = 0; j < list.Count; j++)
            {
                if (allConveyor[i].transform.position.x == list[j].x && allConveyor[i].transform.position.y == list[j].y)
                {
                    ConveyorBelt conveyorBelt = allConveyor[i].GetComponent<ConveyorBelt>();
                    conveyorBelt.FlipYAxis();
                }
            }
        }
    }
}
