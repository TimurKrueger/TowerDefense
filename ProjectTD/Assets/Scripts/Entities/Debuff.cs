using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebuffList
{
    PhysicalDamageOverTime,

}

[System.Serializable]
public static class DebuffManager {

    /// <summary>
    /// Apply a debuff to a certain Entity. The parameters define which debuff will be applied and its strength.\n
    /// List of Debuffs and the impact of their strength:\n
    /// 1. Physical Damage Over Time: Strength = Damage Per Second
    /// </summary>
    /// <param name="e">The entity the debuff will be applied to.</param>
    /// <param name="debuff">The debuff which will be applied.</param>
    /// <param name="duration">The duration of the debuff.</param>
    /// <param name="strength">The strength of the debuff.\nNote: Actual effect can vary depending on the Debuff.</param>
    public static void ApplyDebuff(Entity e, DebuffList debuff, float duration, float strength)
    {
        Debuff newDebuff = null;

        switch (debuff)
        {
            case DebuffList.PhysicalDamageOverTime:
                newDebuff = new PhysicalDamageOverTime(e, duration, strength);
                break;
        }

        if(newDebuff == null)
        {
            Debug.LogWarning("Couldn't create Debuff! Maybe the index has been selected wrongly! Selected index was "+(int) debuff+" which maps to "+(DebuffList)(int)debuff);
            return;
        }

        e.debuffs.Add(newDebuff);
    }
}

public abstract class Debuff
{

    public float remainingTime = 1.0f;
    public float originalTime = 1.0f;
    public float strength = 1.0f;
    public Entity target = null;
    public DebuffList type;
    public bool paused = false;

    public Debuff(Entity target, float duration, float strength)
    {
        this.target = target;
        originalTime = duration;
        remainingTime = originalTime;
        this.strength = strength;
    }

    /// <summary>
    /// Trigger the Debuff. If it returns false, its time is over and it should be deleted
    /// </summary>
    /// <param name="elapsedTime"></param>
    /// <returns></returns>
    public abstract bool Trigger(float elapsedTime);

    public override string ToString()
    {
        //TODO: REPLACE TARGET.NAME WITH TARGET UID FOR SAVING PURPOSES
        return "<Debuff>\n\t<Type>" + type + "</Type>\n" +
            "\t<Duration>" + originalTime + "</Duration>\n" +
            "\t<RemainingDuration>" + remainingTime + "</RemainingDuration>\n" +
            "\t<Strength>" + strength + "</Strength>\n" +
            "\t<Target>" + target.name + "</Target>\n" +
            "\t<Paused>" + paused + "</Paused>\n" +
            "</Debuff>";
    }
}

public class PhysicalDamageOverTime : Debuff
{
    public PhysicalDamageOverTime(Entity target, float duration, float strength) : base(target, duration, strength)
    {
        type = DebuffList.PhysicalDamageOverTime;
    }

    public override bool Trigger(float elapsedTime)
    {
        //Debug.Log("Triggered on " + target.name + " with strength " + strength + " and duration " + remainingTime + " / " + originalTime);
        remainingTime -= elapsedTime;
        target.hp -= elapsedTime / 1 * strength;

        return remainingTime > 0.0f;
    }
}