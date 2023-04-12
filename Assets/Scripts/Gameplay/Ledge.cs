using System.Collections;
using UnityEngine;
using DG.Tweening;
public class Ledge : MonoBehaviour
{
    public int xDir, yDir;
    void Awake()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
    }
    public bool TryToJump(Character character, Vector2 moveDir)
    {
        if (moveDir.x == xDir && moveDir.y == yDir)
        {
            StartCoroutine(Jump(character));
            return true;
        }
        return false;
    }
    IEnumerator Jump(Character character)
    {
        GameController.instance.PauseGame(true);
        var jumpDestination = character.transform.position + new Vector3(xDir, yDir) * 2;
        yield return character.transform.DOJump(jumpDestination, 0.3f, 1, 0.5f).WaitForCompletion();
        GameController.instance.PauseGame(false);
    }
}