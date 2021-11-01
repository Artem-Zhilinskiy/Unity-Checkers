using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;

namespace Checkers
{

    public class ObserverComponent : MonoBehaviour
    {
        public bool isRecorded; //Ведётся ли запись партии
        public bool PlayMode; //Включить или выключить воспроизведение

        private string path = Environment.CurrentDirectory + "//CheckersLog.txt"; //Задание пути файла

        //Корутина задержки
        Coroutine MainReplayCoroutine;

        //Корутина движения фишки
        Coroutine MoveChipCoroutine;

        //Создание файла
        private void FileCreation()
        {
            //var path = Environment.CurrentDirectory + "//CheckersLog.txt";
            var LogFile = File.Create(path);
            LogFile.Close();
            WriteToLog("Ход белых");
        }

        //Запись в файл текстовой строки
        public void WriteToLog(string _stringLog)
        {
            if (File.Exists(path))
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(_stringLog);
                }
            }
        }


        // Start is called before the first frame update
        void Start()
        {
            //Проверка, включён ли режим воспроизведения партии по записи
            if (PlayMode == true)
            {
                isRecorded = false;
                Debug.Log("Включён режим воспроизведения");

                //Блокировка ввода
                GameObject.Find("Main Camera").GetComponent<GameManager>()._isInputBlocked = true;

                //Считывание файла в список строк
                List<string> _file =  ReadLog();
                //Проверка работы метода ReadLog
                //CheckReadLog(_file);

                //Корутина обработки списка строк с задержками.
                MainReplayCoroutine = StartCoroutine(MainReplay(_file));
            }
            //Создание файла в режиме записи партии
            if (isRecorded == true) FileCreation();
        }

        private IEnumerator MainReplay(List<string> _file)
        {
            string _line;
            for (int i = 0; i<_file.Count; i++)
            {
                yield return new WaitForSecondsRealtime(2f);
                Debug.Log(_file[i]);
                _line = _file[i];
                if (_line.Contains("Выбрана клетка"))
                {
                    //Debug.Log("Метод по выбору клетки");
                    DeChooseCell();
                    ChooseCell(_line);
                }
                else if (_line.Contains("Передвижение фишки"))
                {
                    //Debug.Log("Метод по передвижению фишки");
                    MoveChip(_line);
                }
                else if (_line.Contains("Съедена фишка"))
                {
                    Debug.Log("Метод по уничтожению фишки");
                }
                else if (_line.Contains("Ход"))
                {
                    Debug.Log("Метод по передаче хода");
                }
                else if (_line.Contains("победили"))
                {
                    Debug.Log("Метод по определению победителя и окончанию режима воспроизведения");
                }
            }
        }

        private void ChooseCell(string _line)
        {
            //Распознание, какая клетка всё-таки выбрана
            string _cellName = _line.Substring(15);
            //Debug.Log(_cellName);
            foreach (var _cell in GameObject.Find("Main Camera").GetComponent<GameManager>()._blackCells)
            {
                if (_cell.gameObject.name == _cellName)
                {
                    var _cellScript = _cell.GetComponent<CellComponent>();
                    //Метод выделения клетки материалом _ChosenOne 
                    _cellScript.ChangeMaterial(GameObject.Find("Main Camera").GetComponent<GameManager>()._chosenOne);
                    //Debug.Log(_line);
                    return;
                }
            }
        }

        private void DeChooseCell()
        {
            foreach (var _cell in GameObject.Find("Main Camera").GetComponent<GameManager>()._blackCells)
            {
                var _cellScript = _cell.GetComponent<CellComponent>();
                _cellScript.ChangeMaterial(GameObject.Find("Main Camera").GetComponent<GameManager>()._trueColor);
            }
        }

        private void MoveChip(string _line)
        {
            string _exodusCell = _line.Substring(28, 2);
            string _targetCell = _line.Substring(41, 2);
            Debug.Log("exodus cell " + _exodusCell + " target cell " + _targetCell);
            //Определение двигаемой фишки
            Transform _chip = ChipDetect(_exodusCell);
            //Получение координат целевой клетки
            float _targetX = GameObject.Find(_targetCell).GetComponent<Transform>().position.x;
            float _targetZ = GameObject.Find(_targetCell).GetComponent<Transform>().position.z;
            //Запуск корутины движения фишки
            MoveChipCoroutine = StartCoroutine(ChipMove(_chip, _targetX, _targetZ));
        }

        //Получение Transform фишки, которая стоит на этой клетке
        private Transform ChipDetect(string _cellString)
        {
            Transform _cell = GameObject.Find(_cellString).GetComponent<Transform>();
            Transform _detectedChip = BaseClickComponent.Pair2(GameObject.Find("Main Camera").GetComponent<GameManager>()._blackChips, _cell);
            if (_detectedChip == null)
            {
                _detectedChip = BaseClickComponent.Pair2(GameObject.Find("Main Camera").GetComponent<GameManager>()._whiteChips, _cell);
            }
            return _detectedChip;
        }

        private IEnumerator ChipMove(Transform _chip, float x, float z)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(Time.deltaTime);
                Vector3 target = new Vector3(x, 0.1f, z); //Почему 0,1 Если мне надо, чтобы он пришёл в 1??? А если 1, то приходит в 10? Почему Y умножается на 10?
                _chip.position = Vector3.MoveTowards(_chip.position, target, Time.deltaTime * 5);

                if ((_chip.position.x == x) && (_chip.position.z == z))
                {
                    yield break;
                }
            }
        }

            //Модуль воспроизведения.
            //На старте проверяет, включён ли play mode. Если включён, то отключает input и берёт управление на себя.
            /*
            private void Replay()
            {
                Debug.Log("Включён режим воспроизведения");
                //Блокировка ввода
                GameObject.Find("Main Camera").GetComponent<GameManager>()._isInputBlocked = true;
                //Построчное чтение и выполнение
                using (StreamReader sr = new StreamReader(path))
                {
                    sr.ReadLine(); //Пропуск первой строки
                    string _line;
                    while (true)
                    {
                        if (!_stopFactor)
                        {
                            _line = sr.ReadLine();
                            //while ((_line = sr.ReadLine()) != null)
                            {
                                //Debug.Log(_line);
                                //Распознавание строк:
                                /*
                                1. Выбор клетки
                                2. Передвижение фишки
                                3. Уничтожение фишки
                                4. Передача хода
                                5. Победа
                                Исходные данные - поиск подстроки, выполнение метода
                                //1. Выбор клетки
                                if (_line.Contains("Выбрана клетка"))
                                {
                                    //Распознание, какая клетка всё-таки выбрана
                                    string _cellName = _line.Substring(15);
                                    //Debug.Log(_cellName);
                                    foreach (var _cell in GameObject.Find("Main Camera").GetComponent<GameManager>()._blackCells)
                                    {
                                        if (_cell.gameObject.name == _cellName)
                                        {
                                            var _cellScript = _cell.GetComponent<CellComponent>();
                                            //Метод выделения клетки материалом _ChosenOne 
                                            _cellScript.ChangeMaterial(GameObject.Find("Main Camera").GetComponent<GameManager>()._chosenOne);
                                            Debug.Log(_line);
                                            return;
                                        }
                                    }
                                }

                                //2.Передвижение фишки
                                if (_line.Contains("Передвижение фишки"))
                                {
                                    string _exodusCell = _line.Substring(28, 2);
                                    string _targetCell = _line.Substring(41, 2);
                                    Debug.Log("\nexodus cell " + _exodusCell + "\ntarget cell " + _targetCell);
                                    //Метод передвижения фишки с входными строковыми данными _exodusCell и _targetCell
                                    //корутина задержки
                                }

                                //3. Уничтожение фишки
                                if (_line.Contains("Съедена фишка"))
                                {
                                    string _cellEaten = _line.Substring(24);
                                    //Debug.Log("Съедена фишка на " + _cellEaten);
                                    //метод унитожения фишки и очистки массива
                                    //корутина задержки
                                }

                                //4. Передача хода => вращение камеры
                                if (_line.Contains("Ход"))
                                {
                                    //Метод вращения камеры
                                    //корутина задержки
                                    //Debug.Log(_line + ": вращение камеры");
                                }

                                //5. Победа
                                if (_line.Contains("победили"))
                                {
                                    Debug.Log(_line);
                                    //корутина задержки
                                    UnityEditor.EditorApplication.isPaused = true; //Конец режима
                                }
                            }
                        }
                        return;
                    }
                }
            }
    */
            /*
            private IEnumerator Replay(StreamReader sr)
            {
                yield return new WaitForSecondsRealtime(2f);
                Debug.Log("Тест");
                string _line = sr.ReadLine();
                Debug.Log(_line);
                //1. Выбор клетки
                if (_line.Contains("Выбрана клетка"))
                {
                    ChooseCell(_line);
                }
                yield return null;
            }
            */

        //Считывание файла в список строк
        private List<string> ReadLog()
        {
            using (StreamReader sr = new StreamReader(path))
            {
                List<string> _file = new List<string>();
                string _line = sr.ReadLine();
                do
                { 
                    //Debug.Log(_line);
                    _file.Add(_line);
                    _line = sr.ReadLine();
                } while (_line != null);
                return _file;
            }
        }

        //Проверка метода ReadLog()
        private void CheckReadLog(List<string> _file)
        {
            int i = 0;
            while (i <_file.Count)
            {
                Debug.Log(_file[i]);
                i++;
            }
        }
    }
}