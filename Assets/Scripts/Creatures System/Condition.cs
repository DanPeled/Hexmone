using System;
public class Condition
{
    public string name { get; set; }
    public string description { get; set; }
    public string startMessage { get; set; }

    public Func<Creature, bool> onBeforeMove { get; set; }
    public Action<Creature> onAfterTurn { get; set; }
    public Action<Creature> onStart {get; set;}
}