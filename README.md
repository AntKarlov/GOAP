# GOAP - Tank Battle Sandbox

![](https://github.com/AntKarlov/GOAP/blob/master/Assets/Graphics/goap_pic.gif)

Данный проект демонстрирует реализацию GOAP (Goal-Oriented Action Planning) на примере небольшой игры.

В данной игре-примере представлено две команды танко-ботов которые начинают бой не имея никакого снаряжения. Задача каждого из ботов обнаружить экипировку на карте и уничтожить противника. Все боты в примере используют общий сценарий действий, за счет чего результаты схватки определяются случайным образом (исходя из рандомных действий ботов поиска экипировки и противника).

В будущем планируется развитие и улучшение проекта.

## Установка

* Склонируйте репозиторий удобным для вас способом.
* Откройте проект в Unity 5.5.x или новее.

## Известные проблемы

* Боты кажутся «глупыми» по той причине, что для перемещения используют Way-Points вместо Nav-Mesh.
* Сенсор обнаружения препятсвий у ботов реализован плохо и скорее для «галочки» (тестирования прерываний состояния).
* Отсуствуют фабрики объектов и эффекты.

## Links

* [WHAT is GOAP?](http://alumni.media.mit.edu/~jorkin/goap.html)
* [GPGOAP: General Purpose Goal Oriented Action Planning](https://github.com/stolk/GPGOAP)
* [ReGoap: Generic C# GOAP library with Unity3d examples](https://github.com/luxkun/ReGoap)

## License

MIT https://opensource.org/licenses/MIT
