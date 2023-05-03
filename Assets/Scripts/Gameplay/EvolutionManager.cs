using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
public class EvolutionManager : MonoBehaviour
{
    public AudioClip evolutionMusic;
    public GameObject evolutionUI;
    public Image creatureImage;
    public static EvolutionManager instance;
    public event Action onStartEvolution, onCompleteEvolution;
    void Awake()
    {
        instance = this;
    }
    public IEnumerator Evolve(Creature creature, Evolution evolution)
    {
        onStartEvolution?.Invoke();
        evolutionUI.SetActive(true);
        AudioManager.i.PlayMusic(evolutionMusic);
        creatureImage.sprite = creature.GetFrontSprite();
        yield return DialogManager.instance.ShowDialogText($"{creature.GetName()} is evolving");

        
        var oldCreature = creature._base;


        creature.Evolve(evolution);
        creatureImage.sprite = creature.GetFrontSprite();
        yield return DialogManager.instance.ShowDialogText($"{oldCreature.Name} evolved into {creature.GetName()}");


        evolutionUI.SetActive(false);
        onCompleteEvolution?.Invoke();
    }
}