using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class straightLineDrawing : MonoBehaviour
{
    public LineRenderer lineRend;
    public EdgeCollider2D edgeCol;
    private Transform player;
    public float lineLength;
    public Item.ItemType paintType;
    public int divisor = 2;

    void Awake()
    {
        if (player == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players != null && players.Length > 0) { player = players[0].transform; }
        }
    }
    List<Vector2> points;

    public void UpdateLine(Vector2 mousePos)
    {
        if (mousePos == null) { Debug.Log("Null Reference mousePos"); return; }
        if (points == null)
        {
            if ((mousePos.x - player.position.x) * player.transform.localScale.x < 0)
            {
                player.transform.localScale = new Vector3(-1 * player.transform.localScale.x, player.transform.localScale.y, player.transform.localScale.z);
            }
            points = new List<Vector2>();
            Vector2 firstPoint = player.Find("paintgun").Find("Start").position;
            Vector2 diff = mousePos - firstPoint;
            diff.Normalize();
            diff = new Vector2(diff.x / divisor, diff.y / divisor);
            Vector2 currPoint = firstPoint;


            StartCoroutine(lineDraw(currPoint, diff));

            return;
        }
        //Check if mouse has moved enough for us to insert through point
        //If it has: Insert point at mouse position

    }

    IEnumerator lineDraw(Vector2 currPoint, Vector2 unitVector)
    {
        Player playerComponent = player.GetComponent<Player>();
        while (Input.GetMouseButton(0) && playerComponent.HasPaint())
        {
            SetPoint(currPoint);
            currPoint = currPoint + unitVector;
            playerComponent.UsePaint(paintType, 10); //change this line to change amount of paint used
            playerComponent.UsePaint(20); //change this line to change amount of paint used
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    void SetPoint(Vector2 point)
    {
        points.Add(point);

        lineRend.positionCount = points.Count; //All points
        lineRend.SetPosition(points.Count - 1, point);

        if (points.Count > 1)
        {
            edgeCol.points = points.ToArray();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (gameObject.tag == "Poison")
        {
            Poison(col.gameObject);
        }
        else if (gameObject.tag == "InstantKill")
        {
            InstantKill(col.gameObject);
        }
        else if (gameObject.tag == "Damage")
        {
            Damage(col.gameObject);
        }

    }

    public void BossTriggerEntered(GameObject boss)
    {
        Debug.Log("Trigger Entered");
        Enemy e = boss.GetComponent<Enemy>();
        if (e == null || !e.isBoss)
        {
            return;
        }
        if (gameObject.tag == "Poison")
        {
            Poison(boss);
        }
        else if (gameObject.tag == "InstantKill")
        {
            InstantKill(boss);
        }
        else if (gameObject.tag == "Damage")
        {
            Damage(boss);
        }

    }

    void Poison(GameObject entity)
    {
        Player p = entity.GetComponent<Player>();
        if (p != null)
        {
            p.Poison();
        }
        Enemy e = entity.GetComponent<Enemy>();
        if (e != null)
        {
            e.Poison();
        }
    }

    void InstantKill(GameObject entity)
    {
        Player p = entity.GetComponent<Player>();
        if (p != null)
        {
            p.TakeDamage(p.health);
        }
        Enemy e = entity.GetComponent<Enemy>();
        if (e != null)
        {
            if (e.isBoss)
            {
                e.TakeDamage(5);
            }
            else
            {
                e.TakeDamage(e.health);
            }
        }
    }

    void Damage(GameObject entity)
    {
        Player p = entity.GetComponent<Player>();
        if (p != null)
        {
            p.TakeDamage(1);
        }
        Enemy e = entity.GetComponent<Enemy>();
        if (e != null)
        {
            e.TakeDamage(1);
        }
    }


}
