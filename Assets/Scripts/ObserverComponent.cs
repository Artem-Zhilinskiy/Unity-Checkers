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

        //Создание файла
        private void FileCreation()
        {
            //var path = Environment.CurrentDirectory + "//CheckersLog.txt";
            var LogFile = File.Create(path);
            LogFile.Close();
            WriteToLog("Ход белых");
        }

        //Запись в файл текстовой строки
        public void WriteToLog (string _stringLog)
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
                Replay();
            }
            //Создание файла в режиме записи партии
            if (isRecorded == true) FileCreation();
        }

        //Модуль воспроизведения.
        //На старте проверяет, включён ли play mode. Если включён, то отключает input и берёт управление на себя.
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
                while ((_line = sr.ReadLine())!= null)
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
                    */
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
                                //Метод выделения клетки материалом _ChosenOne 
                                //Debug.Log(_line);
                            }
                        }
                    }

                    //2.Передвижение фишки
                    if (_line.Contains("Передвижение фишки"))
                    {
                        string _exodusCell = _line.Substring(28, 2);
                        string _targetCell = _line.Substring(41, 2);
                        //Debug.Log("\nexodus cell " + _exodusCell + "\ntarget cell " + _targetCell);
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
                        //UnityEditor.EditorApplication.isPaused = true; //Конец режима
                    }
                }
            }
        }
    }
}