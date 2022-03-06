using UnityEngine;

namespace HuntroxGames.Utils
{
    public class FieldAndPropertyCommandExample : MonoBehaviour
    {
        //Fields
        [ConsoleCommand] public string playerName = "Huntrox";
        [ConsoleCommand] public CharacterClass playerClass = CharacterClass.Mage;
        [ConsoleCommand("SetSpeed",MonoObjectExecutionType.Option)] public float speed = 5;
        [ConsoleCommand("SetDead")] public bool isDead = false;
        [ConsoleCommand] public Color factionColor = Color.cyan;
        [ConsoleCommand] public Vector3 spawnPoint = Vector3.zero;

        [ReadOnly] public int health = 10;

        //Properties setter / getter
        [ConsoleCommand("SetHealth", "[int]",MonoObjectExecutionType.Option)]
        [ConsoleCommand("GetHealth",MonoObjectExecutionType.Option)]
        public int Health
        {
            get => health;
            set
            {
                Debug.Log("Health Property Value: " + value);
                health = value;
            }
        }

        //Property types
        [ConsoleCommand("Position")] public Vector3 MyPosition => transform.position;
        [ConsoleCommand("Rotation")] public Quaternion Rotation => transform.rotation;
        [ConsoleCommand("EulerAngles")] public Vector3 EulerAngles => transform.eulerAngles;
        [ConsoleCommand] public PersonClass MyPersonClass => new PersonClass();
    }

    public enum CharacterClass
    {
        Mage,
        Warrior,
        Archer
    }

    public class PersonClass
    {
        public string name;
        public string lastName;
        public int age;

        public PersonClass()
        {
            name = "Dominique";
            lastName = "Sanford";
            age = 69;
        }

        public override string ToString()
            => $"Name: {name}, LastName: {lastName}, Age: {age}";
    }
}