using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
public class EvolutionManager : MonoBehaviour
{
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
        creatureImage.sprite = creature._base.frontSprite;
        yield return DialogManager.instance.ShowDialogText($"{creature._base.name} is evolving");

        
        var oldCreature = creature._base;


        creature.Evolve(evolution);
        creatureImage.sprite = creature._base.frontSprite;
        yield return DialogManager.instance.ShowDialogText($"{oldCreature.name} evolved into {creature._base.name}");


        evolutionUI.SetActive(false);
        onCompleteEvolution?.Invoke();
    }
}