using UnityEngine;
using System.Collections.Generic;
public class NPCController : MonoBehaviour, Interactable
{
    public Dialog dialog;
    public List<Sprite> sprites;
    public List<Vector2> movementPattern;
    public float timeBetweenPatterns;
    NPCState state;
    float idleTimer = 0f;
    public Character character;

    SpriteAnimator spriteAnimator;
    void Start()
    {
        spriteAnimator = new SpriteAnimator(sprites, GetComponentInChildren<SpriteRenderer>());
        character = GetComponent<Character>();
        spriteAnimator.Start();
    }
    void Update()
    {
        if (state == NPCState.Idle){
            idleTimer += Time.deltaTime;
            if (idleTimer > 2f){
                idleTimer = 0f;
                if(movementPattern.Count > 0){
                    spriteAnimator.HandleUpdate();
                    var pattern = movementPattern[Random.Range(0, movementPattern.Count)];
                    state = NPCState.Walking;
                }
            }
        }

    }
    public void Interact(){
        StartCoroutine(DialogManager.instance.ShowDialog(dialog, null));
    }
}
public enum NPCState{
    Idle, Walking
}
