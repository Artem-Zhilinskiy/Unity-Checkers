using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Self;

        [Tooltip("Цвет выбранного компонента"), SerializeField]
        public Material _chosenOne;

        [Tooltip("Цвет соседних компонентов"), SerializeField]
        public Material _neighbours;

        [Header("Массив чёрных клеток"), SerializeField]
        public Transform[] _blackCells;

        [Header("Массив белых шашек"), SerializeField]
        private Transform[] _whiteChips;

        [Header("Настройки чёрных шашек"), SerializeField]
        private Transform[] _blackChips;

        // Start is called before the first frame update
        void Start()
        {
            foreach (var x in _blackCells)
                x.gameObject.AddComponent<CellComponent>();
            //Привязывание скриптов фишек
            foreach (var x in _whiteChips)
                x.gameObject.AddComponent<ChipComponent>();
            foreach (var x in _blackChips)
                x.gameObject.AddComponent<ChipComponent>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}