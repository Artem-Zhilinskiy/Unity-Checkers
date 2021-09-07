using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Self;

        //Чей сейчас ход: true - белых, false - чёрных.
        public bool _isWhiteMove;
        //Заблокировно ли управление
        public bool _isInputBlocked = false;

        [Tooltip("Цвет выбранного компонента"), SerializeField]
        public Material _chosenOne;

        [Tooltip("Цвет соседних компонентов"), SerializeField]
        public Material _neighbours;

        [Header("Массив чёрных клеток"), SerializeField]
        public Transform[] _blackCells;

        [Header("Массив белых шашек"), SerializeField]
        public Transform[] _whiteChips;

        [Header("Настройки чёрных шашек"), SerializeField]
        public Transform[] _blackChips;

        [Tooltip("Изначальный цвет клетки"), SerializeField]
        public Material _trueColor;



        Coroutine MoveAndDestroy;
        Coroutine MoveCamera;

        Vector3 _position = new Vector3(0.5f, 8f, -5f);
        Vector3 _pointToLook = new Vector3(3.5f, 0, 3.5f);
        public void MoveChip(Transform _chipChosen, Transform _chipToDestroy, float x, float z)
        {
            //Отключение ввода
            //TurnOffInput();
            GameObject.Find("Main Camera").GetComponent<GameManager>()._isInputBlocked = true;
            MoveAndDestroy = StartCoroutine(MoveAndDestroyCoroutine(_chipChosen, x, z));
            if (GameObject.Find("Main Camera").GetComponent<GameManager>()._isWhiteMove == true)
            {
                foreach (var _chip in GameObject.Find("Main Camera").GetComponent<GameManager>()._blackChips)
                {
                    if (_chipToDestroy != null)
                    {
                        if ((_chip.position.x == _chipToDestroy.position.x) && (_chip.position.z == _chipToDestroy.position.z))
                        {
                            List<Transform> tmp = new List<Transform>(GameObject.Find("Main Camera").GetComponent<GameManager>()._blackChips);
                            tmp.Remove(_chip);
                            Destroy(_chipToDestroy.gameObject);
                            GameObject.Find("Main Camera").GetComponent<GameManager>()._blackChips = tmp.ToArray();
                        }
                    }
                }
            }
            else
            {
                foreach (var _chip in GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips)
                {
                    if (_chipToDestroy != null)
                    {
                        if ((_chip.position.x == _chipToDestroy.position.x) && (_chip.position.z == _chipToDestroy.position.z))
                        {
                            List<Transform> tmp = new List<Transform>(GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips);
                            tmp.Remove(_chip);
                            Destroy(_chipToDestroy.gameObject);
                            GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips = tmp.ToArray();
                        }
                    }
                }
            }
        }

        private IEnumerator MoveAndDestroyCoroutine(Transform _chip, float x, float z)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(Time.deltaTime);
                Vector3 target = new Vector3(x, 0.1f, z); //Почему 0,1 Если мне надо, чтобы он пришёл в 1??? А если 1, то приходит в 10? Почему Y умножается на 10?
                _chip.position = Vector3.MoveTowards(_chip.position, target, Time.deltaTime* 5);
                
                if ((_chip.position.x == x) && (_chip.position.z == z))
                {
                    MoveCamera = StartCoroutine(MoveCameraCoroutine());
                    IsVictory();
                    yield break;
                }
                
            }
        }

        private IEnumerator MoveCameraCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.8f);
            var _camera = GameObject.Find("CameraObject").GetComponent<Transform>();
            for (int i = 0; i < 180; i++)
                {
                    yield return new WaitForSecondsRealtime(Time.deltaTime);
                    _camera.Rotate(0, 0, 1);
                }
                GameObject.Find("Main Camera").GetComponent<GameManager>()._isInputBlocked = false;
                yield break;
        }

        //Проверка условия победы
        private void IsVictory()
        {
            foreach (var _chip in GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips)
            {
                if ((_chip.position.z == 7) || (GameObject.Find("Main Camera").GetComponent<GameManager>()._blackChips == null))
                {
                    Debug.Log("Белые победили");
                    UnityEditor.EditorApplication.isPaused = true;
                }
            }
            foreach (var _chip in GameObject.Find("Main Camera").GetComponent<GameManager>()._blackChips)
            {
                if ((_chip.position.z == 0) || (GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips == null))
                {
                    Debug.Log("Чёрные победили");
                    UnityEditor.EditorApplication.isPaused = true;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _isWhiteMove = true;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}