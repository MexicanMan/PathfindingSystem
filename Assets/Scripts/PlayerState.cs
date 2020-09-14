using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerState", menuName = "States/PlayerState")]
public class PlayerState : ScriptableObject
{
    public int Money;

    public HeroState GreenHero;
    public HeroState YellowHero;
    public HeroState BlueHero;
}
