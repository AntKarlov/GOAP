# GOAP - Tank Battle Sandbox

![](https://github.com/AntKarlov/GOAP/blob/master/Assets/Graphics/goap_pic.gif)

Проект демонстрирует реализацию GOAP (Goal-Oriented Action Planning) на примере небольшой игры.

В данном примере представлено две команды танко-ботов которые начинают бой не имея никакого снаряжения. Задача каждого из ботов обнаружить экипировку на карте и уничтожить противника. Все боты в примере используют общий сценарий действий, поэтому исход событий в большей степени зависит от из случайных действий ботов при поиске экипировки и противника.

Планируется развитие и улучшение проекта.

## Установка

* Склонируйте репозиторий удобным для вас способом.
* Откройте проект в Unity 2017.x или новее.

Установка каких-либо дополнительных плагинов и библиотек не требуется.

## Дополнительно

Чтобы поиграть с ботами, просто перетащите префаб TankPlayer на сцену.

![](https://github.com/AntKarlov/GOAP/blob/master/Assets/Graphics/tank_player.gif)

Управление танком WASD, стрельба - Spacebar.

Чтобы открыть AIDebugger выберите в Unity Menu > Anthill > AIDebugger, после выделите
любой игровой объект имеющий AIAgent на сцене чтобы увидеть его текущий план.

![](https://github.com/AntKarlov/GOAP/blob/master/Assets/Graphics/goap_pic.gif)

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
